using DocumentFormat.OpenXml.ExtendedProperties;
using MySql.Data.MySqlClient;
using SAPbobsCOM;
using System;
using System.Data;
using System.IO;

namespace FTSISAPB1iService
{
    class ImportIncomingPayment
    {
        // why no posted in header lines (ftorct)

        private static DateTime dteStart;
        private static string strTransType;
        private static string strSapDocNum;


        public static void _ImportIncomingPayment()
        {
            string strId = string.Empty;
            string strU_RefNum = string.Empty;
            string strPostU_RefNum = string.Empty; 
            string strPostDocEntry, strPostDocNum;
            string strXmlPath = string.Empty;
            string strMySQLTable;

            try
            {
                dteStart = DateTime.Now;

                // Initialize Object Type.
                GlobalFunction.getObjType(24);
                strTransType = "Payments - " + GlobalVariable.strDocType;

                // Get All data for processing using Stored Procedure
                DataSet dsProcessData = SQLSettings.getDataFromMySQL(string.Format("CALL FTSI_IMPORT_GET_PROCESS_DATA({0})", GlobalVariable.intObjType));

                // Run process for each row
                foreach (DataRow oDataRow in dsProcessData.Tables[0].Rows)
                {
                    strId = oDataRow["Id"].ToString();
                    strMySQLTable = oDataRow["MySQLTable"].ToString();
                    strU_RefNum = oDataRow["U_RefNum"].ToString();

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
                        // Get Document Header and Line Details
                        DataSet dsBusinessObject = SQLSettings.getDataFromMySQL(string.Format("CALL FTSI_IMPORT_INCOMING_PAYMENT('{0}')", strId));

                        // Rename DataTables.
                        // NOTE: Make sure to rename DataTable because the names will be used as TAGS in XML file.
                        dsBusinessObject.Tables[0].TableName = "ORCT";
                        //dsBusinessObject.Tables[1].TableName = "RCT2";

                        // Process XML File Creation
                        strXmlPath = GenerateFilePath(dsBusinessObject.Tables["ORCT"].Rows[0]["U_RefNum"].ToString());
                        if (!XMLGenerator.GenerateXMLFile(GlobalVariable.oObjectType, dsBusinessObject, strXmlPath))
                        {
                            // Update Staging DB
                            SQLSettings.executeQuery(string.Format("UPDATE {0} SET IntegrationStatus = 'E', IntegrationMessage = \"Failed to Generate XML File\" WHERE Id = '{1}'", strMySQLTable, strId));

                            GC.Collect();
                            continue;
                        }

                        // Start XML Import
                        StartCompanyTransaction();

                        if (ImportDocumentsXML.importPaymentXMLDocument(strXmlPath, strId))
                        {
                            // Get Posted DocEntry and DocNum
                            strPostDocEntry = GlobalVariable.oCompany.GetNewObjectKey().ToString();
                            strPostDocNum = GlobalFunction.getDocNum(GlobalVariable.intObjType, strPostDocEntry);

                            // Output to Integration Log
                            SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), Path.GetFileName(strXmlPath), strPostDocEntry, strPostDocNum, dteStart, "S", GlobalVariable.intObjType.ToString(), string.Format("Successfully Posted {0}", strTransType));

                            // Update Staging DB
                            SQLSettings.executeQuery(string.Format("UPDATE {0} SET IntegrationStatus = 'S', IntegrationMessage = \"Successfully Posted\" WHERE Id = '{1}'", strMySQLTable, strId));

                            EndCompanyTransaction(BoWfTransOpt.wf_Commit);
                        }
                        else
                        {
                            SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), Path.GetFileName(strXmlPath), "", "", dteStart, "E", "-" + GlobalVariable.intObjType.ToString(), string.Format("Error Posting SAP Business Object: {0}", GlobalVariable.strErrMsg.Replace("\\", "").Replace("\"", "'")));

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
