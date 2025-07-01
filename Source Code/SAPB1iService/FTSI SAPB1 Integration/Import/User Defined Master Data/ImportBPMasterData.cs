using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using SAPbobsCOM;
using System;
using System.Data;
using System.IO;

namespace FTSISAPB1iService
{
    class ImportBPMasterData
    {
        private static DateTime dteStart;
        private static string strTransType;
        private static string strSapDocNum;

        public static void _ImportBPMasterData()
        {
            //string strItemCode = string.Empty;
            string strId = string.Empty;
            string strU_RefNum = string.Empty;
            string strCardName = string.Empty;
            string strXmlPath = string.Empty;
            string strSeries = string.Empty;
            string strGroupCode = string.Empty;
            string strMySQLTable;

            try
            {
                dteStart = DateTime.Now;

                // Initialize Object Type.
                GlobalFunction.getObjType(2);
                strTransType = "Master - " + GlobalVariable.strDocType;

                // Get All data for processing using Stored Procedure
                DataSet dsProcessData = SQLSettings.getDataFromMySQL(string.Format("CALL FTSI_IMPORT_GET_PROCESS_DATA({0})", GlobalVariable.intObjType));

                // Run process for each row
                foreach (DataRow oDataRow in dsProcessData.Tables[0].Rows)
                {
                    strId = oDataRow["Id"].ToString();
                    strMySQLTable = oDataRow["MySQLTable"].ToString();
                    strSeries = oDataRow["Series"].ToString();
                    strU_RefNum = oDataRow["U_RefNum"].ToString();
                    strCardName = oDataRow["CardName"].ToString();

                    try
                    {
                        // Validation: Check if U_RefNum exists
                        if (GlobalFunction.checkRefNum(strU_RefNum, GlobalVariable.strTableHeader))
                        {   
                            SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), Path.GetFileName(strXmlPath), "", strU_RefNum, dteStart, "E", "", $"Validation failed: U_RefNum '{strU_RefNum}' already exists.");
                            SQLSettings.executeQuery(string.Format("UPDATE {0} SET IntegrationStatus = 'E', IntegrationMessage = \"U_RefNum already exist\" WHERE Id = '{1}'", strMySQLTable, strId));

                            GC.Collect();
                            continue; //Move on to the next DataRow in the loop;
                        }

                        // Validation: Check if CardName exists
                        if (GlobalFunction.checkMasterData(strCardName, GlobalVariable.strTableHeader, "CardName"))
                        {
                            SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), Path.GetFileName(strXmlPath), "", strU_RefNum, dteStart, "E", "", $"Validation failed: strCardName '{strCardName}' already exists.");
                            SQLSettings.executeQuery(string.Format("UPDATE {0} SET IntegrationStatus = 'E', IntegrationMessage = \"CardName already exist\" WHERE Id = '{1}'", strMySQLTable, strId));

                            GC.Collect();
                            continue; //Move on to the next DataRow in the loop;
                        }

                        // Get Document Header and Line Details
                        DataSet dsBusinessObject = SQLSettings.getDataFromMySQL(string.Format("CALL FTSI_IMPORT_BP_MASTERDATA('{0}')", strId));

                        // Rename DataTables.
                        // NOTE: Make sure to rename DataTable because the names will be used as TAGS in XML file.
                        dsBusinessObject.Tables[0].TableName = "OCRD";

                        if (strSeries.ToString() == "0")
                        {
                            strGroupCode = dsBusinessObject.Tables[0].Rows[0]["GroupCode"].ToString();
                            strSeries = GlobalFunction.getUSeriesbyGroupCode("OCRG", strGroupCode, "GroupCode");
                            dsBusinessObject.Tables["OCRD"].Rows[0]["Series"] = Convert.ToInt32(strSeries);
                        }

                        // Process XML File Creation
                        strXmlPath = GenerateFilePath(dsBusinessObject.Tables["OCRD"].Rows[0]["U_RefNum"].ToString());
                        if (!XMLGenerator.GenerateXMLFile(GlobalVariable.oObjectType, dsBusinessObject, strXmlPath))
                        {
                            // Update Staging DB
                            SQLSettings.executeQuery(string.Format("UPDATE {0} SET IntegrationStatus = 'E', IntegrationMessage = \"Failed to Generate XML File\" WHERE Id = '{1}'", strMySQLTable, strId));

                            GC.Collect();
                            continue;
                        }

                        // Start XML Import
                        StartCompanyTransaction();

                        if (ImportDocumentsXML.importBPMasterDataXMLDocument(strXmlPath, strId))
                        {
                            // Output to Integration Log
                            SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), Path.GetFileName(strXmlPath), "", "", dteStart, "S", GlobalVariable.intObjType.ToString(), string.Format("Successfully Posted {0}", strTransType));

                            // Update Staging DB
                            SQLSettings.executeQuery(string.Format("UPDATE {0} SET IntegrationStatus = 'S', IntegrationMessage = \"Successfully Posted\" WHERE Id = '{1}'", strMySQLTable, strId));

                            EndCompanyTransaction(BoWfTransOpt.wf_Commit);
                        }
                        else
                        {
                            // Output to Integration Log
                            SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), Path.GetFileName(strXmlPath), "", "", dteStart, "E", "-" + GlobalVariable.intObjType.ToString(), "Error Posting SAP Business Object");

                            // Update Staging DB
                            SQLSettings.executeQuery(string.Format("UPDATE {0} SET IntegrationStatus = 'E', IntegrationMessage = \"{1}\" WHERE Id = '{2}'", strMySQLTable, GlobalVariable.strErrMsg.Replace("\\", "").Replace("\"", "'"), strId));

                            EndCompanyTransaction(BoWfTransOpt.wf_RollBack);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobalVariable.intErrNum = -111;
                        GlobalVariable.strErrMsg = string.Format("Error Processing Import. {0}", ex.Message.ToString());

                        SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), Path.GetFileName(strXmlPath), "", strU_RefNum, dteStart, "E", GlobalVariable.intErrNum.ToString(), GlobalVariable.strErrMsg);

                        // Update Staging DB
                        SQLSettings.executeQuery(string.Format("UPDATE {0} SET IntegrationStatus = 'E', IntegrationMessage = \"{1}\" WHERE Id = '{2}'", strMySQLTable, GlobalVariable.strErrMsg.Replace("\\", "").Replace("\"", "'"), strId));

                        GC.Collect();

                        EndCompanyTransaction(BoWfTransOpt.wf_RollBack);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalVariable.intErrNum = -111;
                GlobalVariable.strErrMsg = string.Format("Error Processing Import. {0}", ex.Message.ToString());

                SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), Path.GetFileName(strXmlPath), strId, strU_RefNum, dteStart, "E", GlobalVariable.intErrNum.ToString(), GlobalVariable.strErrMsg);

                GC.Collect();

                EndCompanyTransaction(BoWfTransOpt.wf_RollBack);
            }
        }

        private static void StartCompanyTransaction()
        {
            if (!(GlobalVariable.oCompany.InTransaction))
                GlobalVariable.oCompany.StartTransaction();
        }

        private static void EndCompanyTransaction(BoWfTransOpt transOpt)
        {
            if (GlobalVariable.oCompany.InTransaction)
                GlobalVariable.oCompany.EndTransaction(transOpt);
        }

        private static string GenerateFilePath(string strRefNum)
        {
            return GlobalVariable.strTempPath + string.Format("{0}_DOC_{1}_{2}_{3}_1.xml", GlobalVariable.strCompany, GlobalVariable.strTableHeader, GlobalVariable.intObjType, strRefNum, DateTime.Today.ToString("MMddyyyy"));
        }
    }
}
