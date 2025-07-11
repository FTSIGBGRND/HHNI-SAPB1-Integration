﻿using MySql.Data.MySqlClient;
using SAPbobsCOM;
using System;
using System.Data;
using System.IO;

namespace FTSISAPB1iService
{
    public class ImportBillOfMaterials
    {
        private static DateTime dteStart;
        private static string strTransType;

        public static void _ImportBillOfMaterials()
        {
            string strId = string.Empty;
            string strU_RefNum = string.Empty;
            string strDocEntry = string.Empty;
            string strXmlPath = string.Empty;
            string strMySQLTable;
            string strCode = string.Empty;
            string strItemId = string.Empty;
            string strGetItemCode, strGetCode;
            try
            {
                dteStart = DateTime.Now;

                // Initialize Object Type.
                GlobalFunction.getObjType(66);
                strTransType = "Master Data - " + GlobalVariable.strDocType;

                // Get All data for processing using Stored Procedure
                DataSet dsProcessData = SQLSettings.getDataFromMySQL(string.Format("CALL FTSI_IMPORT_GET_PROCESS_DATA('{0}')", GlobalVariable.intObjType));

                // Process Retrieved Data from Stored Procedure
                foreach (DataRow oDataRow in dsProcessData.Tables[0].Rows)
                {
                    //strCode = oDataRow["Code"].ToString();
                    strId = oDataRow["Id"].ToString();
                    strMySQLTable = oDataRow["MySQLTable"].ToString();
                    //strItemId = oDataRow["ItemCode"].ToString();
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

                        DataSet dsBusinessObject = SQLSettings.getDataFromMySQL(string.Format("CALL FTSI_IMPORT_SALES_BOM('{0}')", strId));

                       

                        // Rename DataTables.
                        // NOTE: Make sure to rename DataTable because the names will be used as TAGS in XML file.
                        dsBusinessObject.Tables[0].TableName = "OITT";

                            strCode = dsBusinessObject.Tables[0].Rows[0]["Code"].ToString();
                            strGetCode = GlobalFunction.getCodebyId("OITM", strCode, "ItemCode", "U_ItemID");
                            dsBusinessObject.Tables["OITT"].Rows[0]["Code"] = strGetCode;

                        dsBusinessObject.Tables[1].TableName = "ITT1";

                        for (int i = 0; i < dsBusinessObject.Tables["ITT1"].Rows.Count; i++)
                        {
                            strItemId = dsBusinessObject.Tables[1].Rows[i]["Code"].ToString();
                            strGetItemCode = GlobalFunction.getCodebyId("OITM", strItemId, "ItemCode", "U_ItemID");
                            dsBusinessObject.Tables["ITT1"].Rows[i]["Code"] = strGetItemCode;
                        }

                        // Process XML File Creation
                        strXmlPath = GenerateFilePath(strDocEntry);
                        if (!XMLGenerator.GenerateXMLFile(GlobalVariable.oObjectType, dsBusinessObject, strXmlPath))
                        {
                            // Update Staging DB
                            SQLSettings.executeQuery(string.Format("UPDATE {0} SET IntegrationStatus = 'E', IntegrationMessage = \"Failed to Generate XML File\" WHERE Id = '{1}'", strMySQLTable, strId));

                            continue;
                        }

                        // Start XML Import
                        StartCompanyTransaction();

                        if (ImportDocumentsXML.importProductTreeFromXML(strXmlPath, strId, strGetCode))
                        {
                            // Output to Integration Log
                            SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), Path.GetFileName(strXmlPath), "", "", dteStart, "S", GlobalVariable.intObjType.ToString(), string.Format("Successfully Posted {0} - ItemCode: {1}", strTransType, strCode));

                            // Update Staging DB
                            SQLSettings.executeQuery(string.Format("UPDATE {0} SET IntegrationStatus = 'S', IntegrationMessage = \"Successfully Posted\" WHERE Id = '{1}'", strMySQLTable, strId));

                            EndCompanyTransaction(BoWfTransOpt.wf_Commit);
                        }
                        else
                        {
                            // Output to Integration Log
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

                SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), Path.GetFileName(strXmlPath), "", strU_RefNum, dteStart, "E", GlobalVariable.intErrNum.ToString(), GlobalVariable.strErrMsg);

                GC.Collect();

                EndCompanyTransaction(BoWfTransOpt.wf_RollBack);
            }

        }
        public static void StartCompanyTransaction()
        {
            if (!(GlobalVariable.oCompany.InTransaction))
                GlobalVariable.oCompany.StartTransaction();
        }

        public static void EndCompanyTransaction(BoWfTransOpt transOpt)
        {
            if (GlobalVariable.oCompany.InTransaction)
                GlobalVariable.oCompany.EndTransaction(transOpt);
        }
        public static string GenerateFilePath(string strDocEntry)
        {
            return GlobalVariable.strTempPath + string.Format("{0}_DOC_{1}_{2}_{3}_{4}_1.xml", GlobalVariable.strCompany, GlobalVariable.strTableHeader, GlobalVariable.intObjType, strDocEntry, DateTime.Today.ToString("MMddyyyy"));
        }
    }
}
