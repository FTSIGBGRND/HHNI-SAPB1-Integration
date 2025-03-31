using SAPbobsCOM;
using System;
using System.Data;
using System.IO;
using System.Xml;

namespace FTSISAPB1iService
{
    class ImportDocumentsXML
    {
        private static DateTime dteStart;
        private static string strTransType;

        private static string strObjType, strFObjType, strVersion, strRefNum;
        private static string strMsgBod, strStatus, strPostDocNum, strPostDocEnt;

        private static bool blExist = false;

        public static void importXMLPostDocument(string strFile)
        {
            string[] strFValue;

            SAPbobsCOM.Documents oDocuments;

            try
            {

                XmlDocument xmlDoc = new XmlDocument();

                GlobalVariable.strFileName = Path.GetFileName(strFile);

                strFValue = Path.GetFileNameWithoutExtension(strFile).Split(Convert.ToChar("_"));

                strFObjType = strFValue[3];
                strRefNum = strFValue[4];
                strVersion = strFValue[6];

                strTransType = "Documents - Import From File (xml)";

                //validate xml file to be process
                if (validateXMLData(strFile, strVersion))
                {
                    //process valid xml file
                    if (!(GlobalVariable.oCompany.InTransaction))
                        GlobalVariable.oCompany.StartTransaction();

                    oDocuments = null;
                    oDocuments = (SAPbobsCOM.Documents)GlobalVariable.oCompany.GetBusinessObjectFromXML(strFile, 0);

                    if (blExist == false)
                    {
                        //post transaction if not exist in SAP Business One Marketing Documents 
                        if (oDocuments.Add() != 0)
                        {
                            //return error if not successfully posted
                            GlobalVariable.intErrNum = GlobalVariable.oCompany.GetLastErrorCode();
                            GlobalVariable.strErrMsg = GlobalVariable.oCompany.GetLastErrorDescription();

                            strStatus = "E";
                            strMsgBod = string.Format("Error Posting {0} - {1}.\r" +
                                                      "Error Code: {2}\rDescription: {3} ", GlobalVariable.strDocType, GlobalVariable.strFileName,
                                                                                            GlobalVariable.intErrNum.ToString(), GlobalVariable.strErrMsg);

                            SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), GlobalVariable.strFileName, "", "", dteStart, strStatus, GlobalVariable.intErrNum.ToString(), GlobalVariable.strErrMsg);

                        }
                        else
                        {
                            //return success if document is posted
                            strStatus = "S";

                            strPostDocEnt = GlobalVariable.oCompany.GetNewObjectKey().ToString();
                            strPostDocNum = GlobalFunction.getDocNum(GlobalVariable.intObjType, strPostDocEnt);

                            strMsgBod = string.Format("Successfully Posted {0} - {1}. Posted Document Number: {1} ", GlobalVariable.strDocType, GlobalVariable.strFileName, strPostDocNum);

                            GlobalVariable.intErrNum = 0;
                            GlobalVariable.strErrMsg = strMsgBod;

                            SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), GlobalVariable.strFileName, strPostDocEnt, strPostDocNum, dteStart, strStatus, GlobalVariable.intErrNum.ToString(), GlobalVariable.strErrMsg);

                            if (GlobalVariable.oCompany.InTransaction)
                                GlobalVariable.oCompany.EndTransaction(BoWfTransOpt.wf_Commit);
                        }

                        //transfer file and send alert
                        TransferFile.transferProcFiles("Import", strStatus, GlobalVariable.strFileName);

                        GlobalFunction.sendAlert(strStatus, "Import", strMsgBod, GlobalVariable.oObjectType, GlobalVariable.oCompany.GetNewObjectKey().ToString());

                    }
                    else
                    {
                        //error if transaction already uploaded base on reference number
                        if (GlobalVariable.blAlwUpdte == false)
                        {
                            GlobalVariable.intErrNum = -999;
                            GlobalVariable.strErrMsg = string.Format("Reference Number - {0} already uploaded.", strRefNum);

                            strStatus = "E";
                            strMsgBod = string.Format("Error Posting {0} - {1}.\r" +
                                                      "Error Code: {2}\rDescription: {3} ", GlobalVariable.strDocType, GlobalVariable.strFileName,
                                                                                            GlobalVariable.intErrNum.ToString(), GlobalVariable.strErrMsg);

                            SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), GlobalVariable.strFileName, "", "", dteStart, strStatus, GlobalVariable.intErrNum.ToString(), GlobalVariable.strErrMsg);

                            TransferFile.transferProcFiles("Import", "E", GlobalVariable.strFileName);

                            GlobalFunction.sendAlert(strStatus, "Import", strMsgBod, GlobalVariable.oObjectType, GlobalVariable.oCompany.GetNewObjectKey().ToString());
                        }
                        else
                        {
                            //if there is an update with the documents (need to define mapping because xml update is not working specially with partial transactions made
                        }

                    }

                }
                else
                {
                    //error if validation with files failed

                    TransferFile.transferProcFiles("Import", "E", GlobalVariable.strFileName);

                    GlobalFunction.sendAlert(strStatus, "Import", GlobalVariable.strErrMsg, GlobalVariable.oObjectType, GlobalVariable.oCompany.GetNewObjectKey().ToString());
                }

                GC.Collect();
            }
            catch (Exception ex)
            {
                SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), "", "", "", dteStart, "E", "-111", ex.Message.ToString());

                TransferFile.transferProcFiles("Import", "E", GlobalVariable.strFileName);
            }
        }
        public static bool importTempXMLDocument(string strFile, string strRefNum)
        {

            SAPbobsCOM.Documents oDocuments;

            try
            {
                oDocuments = null;
                oDocuments = (SAPbobsCOM.Documents)GlobalVariable.oCompany.GetBusinessObjectFromXML(strFile, 0);
                if (GlobalVariable.intObjType == 112)
                {
                    oDocuments.DocObjectCode = GlobalVariable.oBObjectType;
                }

                if (oDocuments.Add() != 0)
                {
                    GlobalVariable.intErrNum = GlobalVariable.oCompany.GetLastErrorCode();
                    GlobalVariable.strErrMsg = GlobalVariable.oCompany.GetLastErrorDescription();

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                GlobalVariable.intErrNum = -111;
                GlobalVariable.strErrMsg = ex.Message;

                return false;

            }
            finally
            {
                GlobalFunction.cleanTempFiles();
            }
        }
        public static bool importPaymentXMLDocument(string strFile, string strRefNum)
        {
            SAPbobsCOM.Payments oPayments;

            try
            {
                oPayments = GlobalVariable.oCompany.GetBusinessObjectFromXML(strFile, 0);

                if (oPayments.Add() != 0)
                {
                    GlobalVariable.intErrNum = GlobalVariable.oCompany.GetLastErrorCode();
                    GlobalVariable.strErrMsg = GlobalVariable.oCompany.GetLastErrorDescription();

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                GlobalVariable.intErrNum = -111;
                GlobalVariable.strErrMsg = ex.Message;
                return false;
            }
            finally
            {
                GlobalFunction.cleanTempFiles();
            }
        }

        public static bool importBOMXMLDocument(string strFile, string strRefNum)
        {
            SAPbobsCOM.ProductTrees oProductTrees;
            try
            {
                oProductTrees = GlobalVariable.oCompany.GetBusinessObjectFromXML(strFile, 0);

                if (oProductTrees.Add() != 0)
                {
                    GlobalVariable.intErrNum = GlobalVariable.oCompany.GetLastErrorCode();
                    GlobalVariable.strErrMsg = GlobalVariable.oCompany.GetLastErrorDescription();

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                GlobalVariable.intErrNum = -111;
                GlobalVariable.strErrMsg = ex.Message;
                return false;
            }
            finally
            {
                GlobalFunction.cleanTempFiles();
            }
        }
        public static bool importProductTreeFromXML(string strFile, string strRefNum, string strCode)
        {
            string strItemId, strGetItemCode = string.Empty;

            SAPbobsCOM.ProductTrees productTrees;
            SAPbobsCOM.ProductTrees_Lines productTreesLines;
            productTrees = GlobalVariable.oCompany.GetBusinessObject(BoObjectTypes.oProductTrees);
            try
            {

                if (productTrees.GetByKey(strCode))
                {
                    DataSet oDataSet = SQLSettings.getDataFromMySQL(string.Format("CALL FTSI_IMPORT_SALES_BOM('{0}')", strRefNum));

                    oDataSet.Tables[1].TableName = "ITT1";
                    for (int i = 0; i < oDataSet.Tables["ITT1"].Rows.Count; i++)
                    {
                        strItemId = oDataSet.Tables[1].Rows[i]["Code"].ToString();

                        strGetItemCode = GlobalFunction.getCodebyId("OITM", strItemId, "ItemCode", "U_ItemID");
                        oDataSet.Tables["ITT1"].Rows[i]["Code"] = strGetItemCode;
                    }

                    // Update header
                    //productTrees.Warehouse = oDataSet.Tables[0].Rows[0]["ToWH"].ToString();
                    //productTrees.Quantity = Convert.ToDouble(oDataSet.Tables[0].Rows[0]["Qauntity"].ToString());


                    // Update sales bom lines
                    productTreesLines = productTrees.Items;

                    // Check if current row count is same with update
                    bool blSameRowCount = productTreesLines.Count == oDataSet.Tables[1].Rows.Count;

                    if (!blSameRowCount)
                    {

                        // delete current lines
                        int intRowCount = productTreesLines.Count;
                        for (int  i = 0;  i < intRowCount;  i++)
                        {
                            productTreesLines.SetCurrentLine(i);
                            productTreesLines.Delete();
                        }

                        // add new lines
                        foreach (DataRow row in oDataSet.Tables[1].Rows)
                        {
                            productTreesLines.Add();
                            productTreesLines.SetCurrentLine(Convert.ToInt32(row["ChildNum"].ToString()));
                            productTreesLines.ItemCode = strGetItemCode;
                            productTreesLines.Quantity = Convert.ToDouble(row["Quantity"].ToString());
                        }

                    }
                    else
                    {
                        // update lines
                        foreach (DataRow row in oDataSet.Tables[1].Rows)
                        {
                            productTreesLines.SetCurrentLine(Convert.ToInt32(row["ChildNum"].ToString()));
                            productTreesLines.ItemCode = strGetItemCode;
                            productTreesLines.Quantity = Convert.ToDouble(row["Quantity"].ToString());
                        }
                    }


                    if (productTrees.Update() != 0)
                    {
                        GlobalVariable.intErrNum = GlobalVariable.oCompany.GetLastErrorCode();
                        GlobalVariable.strErrMsg = GlobalVariable.oCompany.GetLastErrorDescription();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    productTrees = null;
                    productTrees = GlobalVariable.oCompany.GetBusinessObjectFromXML(strFile, 0);

                    if (productTrees.Add() != 0)
                    {
                        GlobalVariable.intErrNum = GlobalVariable.oCompany.GetLastErrorCode();
                        GlobalVariable.strErrMsg = GlobalVariable.oCompany.GetLastErrorDescription();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                GlobalVariable.intErrNum = -111;
                GlobalVariable.strErrMsg = ex.Message;
                return false;

            }
            finally
            {
                SystemFunction.releaseObj(productTrees);
                GlobalFunction.cleanTempFiles();
            }
        }

        public static bool importBPMasterDataXMLDocument(string strFile, string strRefNum)
        {
            SAPbobsCOM.BusinessPartners oBusinessPartners;
            try
            {
                oBusinessPartners = GlobalVariable.oCompany.GetBusinessObjectFromXML(strFile, 0);

                if (oBusinessPartners.Add() != 0)
                {
                    GlobalVariable.intErrNum = GlobalVariable.oCompany.GetLastErrorCode();
                    GlobalVariable.strErrMsg = GlobalVariable.oCompany.GetLastErrorDescription();

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                GlobalVariable.intErrNum = -111;
                GlobalVariable.strErrMsg = ex.Message;
                return false;
            }
            finally
            {
                GlobalFunction.cleanTempFiles();
            }
        }

        public static bool importItemMasterDataXMLDocument(string strFile, string strRefNum)
        {
            SAPbobsCOM.Items oItems;
            try
            {
                oItems = GlobalVariable.oCompany.GetBusinessObjectFromXML(strFile, 0);

                if (oItems.Add() != 0)
                {
                    GlobalVariable.intErrNum = GlobalVariable.oCompany.GetLastErrorCode();
                    GlobalVariable.strErrMsg = GlobalVariable.oCompany.GetLastErrorDescription();

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                GlobalVariable.intErrNum = -111;
                GlobalVariable.strErrMsg = ex.Message;
                return false;
            }
            finally
            {
                GlobalFunction.cleanTempFiles();
            }
        }
       
        private static bool validateXMLData(string strFilePath, string strVersion)
        {
            string strQuery;

            string strBaseLine = "", strBaseRef = "", strBaseType = "",
                   strCodeBars;

            bool blRetVal = true, blSaveDoc = false;

            XmlDocument xmlDoc = new XmlDocument();
            XmlNodeList xmlNodLst;

            SAPbobsCOM.Recordset oRecordset;

            try
            {

                xmlDoc.Load(strFilePath);

                //get object type of xml to be process
                xmlNodLst = xmlDoc.SelectNodes("BOM/BO/AdmInfo");
                foreach (XmlNode xmlNod in xmlNodLst)
                {
                    strObjType = xmlNod.SelectSingleNode("Object").InnerText;
                    GlobalFunction.getObjType(Convert.ToInt32(strObjType));
                }

                //validate obeject type on filename vs xml data
                if (strFObjType != strObjType)
                {
                    GlobalVariable.strErrMsg = string.Format("File Object Type Mismatch - {1}.", GlobalVariable.strErrMsg, GlobalVariable.strFileName);
                    SystemFunction.transHandler("Import", strTransType, strObjType, GlobalVariable.strFileName, "", "", dteStart, "E", "-999", GlobalVariable.strErrMsg);
                    return false;
                }

                //validate if file already uploaded
                strQuery = string.Format("SELECT \"DocEntry\" FROM {0} WHERE \"U_FileName\" = '{1}' ", GlobalVariable.strTableHeader, GlobalVariable.strFileName);

                oRecordset = null;
                oRecordset = (SAPbobsCOM.Recordset)GlobalVariable.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                oRecordset.DoQuery(strQuery);

                if (oRecordset.RecordCount > 0)
                {
                    GlobalVariable.strErrMsg = string.Format("{0} \rFile Already Uploaded - {1}.", GlobalVariable.strErrMsg, GlobalVariable.strFileName);
                    blRetVal = false;
                }

                //validate if reference already uploaded
                strQuery = string.Format("SELECT \"DocEntry\" FROM {0} WHERE \"U_RefNum\" = '{1}' ", GlobalVariable.strTableHeader, strRefNum);

                oRecordset = null;
                oRecordset = (SAPbobsCOM.Recordset)GlobalVariable.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                oRecordset.DoQuery(strQuery);

                if (oRecordset.RecordCount > 0)
                    blExist = true;
                else
                    blExist = false;

                //continue process if valid file and reference number or allow update with already uploaded data in SAP Business One
                if ((blExist == false) || (blExist == true && GlobalVariable.blAlwUpdte))
                {
                    //header data and validation
                    xmlNodLst = xmlDoc.SelectNodes(string.Format("BOM/BO/{0}/row", GlobalVariable.strTableHeader));
                    foreach (XmlNode xmlNod in xmlNodLst)
                    {
                        //validation header if needed
                    }

                    //line details data and validation
                    xmlNodLst = xmlDoc.SelectNodes(string.Format("BOM/BO/{0}/row", GlobalVariable.strTableLine1));
                    foreach (XmlNode xmlNod1 in xmlNodLst)
                    {

                        //validation details if needed

                        if (xmlNod1.SelectSingleNode("U_BaseType") != null)
                            strBaseType = xmlNod1.SelectSingleNode("U_BaseType").InnerText;

                        if (xmlNod1.SelectSingleNode("U_BaseRef") != null)
                            strBaseRef = xmlNod1.SelectSingleNode("U_BaseRef").InnerText;

                        if (xmlNod1.SelectSingleNode("U_BaseLine") != null)
                            strBaseLine = xmlNod1.SelectSingleNode("U_BaseLine").InnerText;

                        //if base document is apply - update reference data base on SAP Business One
                        if (!(string.IsNullOrEmpty(strBaseRef)))
                        {
                            #region "Remove Existing Base Reference / Unnecessary Fields"

                            if (xmlNod1.SelectSingleNode("CodeBars") != null)
                            {
                                strCodeBars = xmlNod1.SelectSingleNode("CodeBars").InnerText;

                                if (string.IsNullOrEmpty(strCodeBars))
                                {
                                    XmlNode CodeBars = xmlNod1.SelectSingleNode("CodeBars");
                                    xmlNod1.RemoveChild(CodeBars);
                                    blSaveDoc = true;
                                }
                            }

                            if (xmlNod1.SelectSingleNode("BaseLine") != null)
                            {
                                XmlNode BaseLine = xmlNod1.SelectSingleNode("BaseLine");
                                xmlNod1.RemoveChild(BaseLine);
                                blSaveDoc = true;
                            }

                            if (xmlNod1.SelectSingleNode("BaseRef") != null)
                            {
                                XmlNode BaseRef = xmlNod1.SelectSingleNode("BaseRef");
                                xmlNod1.RemoveChild(BaseRef);
                                blSaveDoc = true;
                            }

                            if (xmlNod1.SelectSingleNode("BaseEntry") != null)
                            {
                                XmlNode BaseEntry = xmlNod1.SelectSingleNode("BaseEntry");
                                xmlNod1.RemoveChild(BaseEntry);
                                blSaveDoc = true;
                            }

                            if (xmlNod1.SelectSingleNode("BaseType") != null)
                            {
                                XmlNode BaseType = xmlNod1.SelectSingleNode("BaseType");
                                xmlNod1.RemoveChild(BaseType);
                                blSaveDoc = true;
                            }

                            #endregion

                            GlobalFunction.getBaseType(Convert.ToInt32(strBaseType));

                            strQuery = string.Format("SELECT {0}.\"DocEntry\", {0}.\"DocNum\", {1}.\"LineNum\" " +
                                                     "FROM {0} INNER JOIN {1} ON {0}.\"DocEntry\" = {1}.\"DocEntry\" " +
                                                     "WHERE {0}.\"U_RefNum\" = '{2}' AND " +
                                                     "      {1}.\"LineNum\" = '{3}' ", GlobalVariable.strBTableHeader, GlobalVariable.strBTableLine1, strBaseRef, strBaseLine);

                            oRecordset = null;
                            oRecordset = (SAPbobsCOM.Recordset)GlobalVariable.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                            oRecordset.DoQuery(strQuery);

                            if (oRecordset.RecordCount > 0)
                            {
                                XmlNode NBaseType = xmlDoc.CreateElement("BaseType");
                                NBaseType.InnerText = GlobalVariable.intBObjType.ToString();
                                xmlNod1.PrependChild(NBaseType);
                                blSaveDoc = true;

                                XmlNode NBaseLine = xmlDoc.CreateElement("BaseLine");
                                NBaseLine.InnerText = oRecordset.Fields.Item("LineNum").Value.ToString();
                                xmlNod1.PrependChild(NBaseLine);
                                blSaveDoc = true;

                                XmlNode NBaseRef = xmlDoc.CreateElement("BaseRef");
                                NBaseRef.InnerText = oRecordset.Fields.Item("DocNum").Value.ToString();
                                xmlNod1.PrependChild(NBaseRef);
                                blSaveDoc = true;

                                XmlNode NBaseEntry = xmlDoc.CreateElement("BaseEntry");
                                NBaseEntry.InnerText = oRecordset.Fields.Item("DocEntry").Value.ToString();
                                xmlNod1.PrependChild(NBaseEntry);
                                blSaveDoc = true;

                            }
                            else
                            {
                                GlobalVariable.strErrMsg = string.Format("{0} \rBase Document Not Found for Base Reference {0} - Line # {1} with Filename {2}.", GlobalVariable.strErrMsg, strBaseRef, strBaseLine, GlobalVariable.strFileName);

                                blSaveDoc = false;
                                blRetVal = false;
                            }

                        }
                    }

                    //freight data and validation
                    xmlNodLst = xmlDoc.SelectNodes(string.Format("BOM/BO/{0}/row", GlobalVariable.strTableLine3));
                    foreach (XmlNode xmlNod3 in xmlNodLst)
                    {
                        //validation details if needed

                        if (xmlNod3.SelectSingleNode("U_BaseType") != null)
                            strBaseType = xmlNod3.SelectSingleNode("U_BaseType").InnerText;

                        if (xmlNod3.SelectSingleNode("U_BaseRef") != null)
                            strBaseRef = xmlNod3.SelectSingleNode("U_BaseRef").InnerText;

                        if (xmlNod3.SelectSingleNode("U_BaseLine") != null)
                            strBaseLine = xmlNod3.SelectSingleNode("U_BaseLine").InnerText;

                        //if base document is apply - update reference data base on SAP Business One
                        if (!(string.IsNullOrEmpty(strBaseRef)))
                        {
                            #region "Remove Existing Base Reference / Unnecessary Fields"

                            if (xmlNod3.SelectSingleNode("BaseLine") != null)
                            {
                                XmlNode BaseLine = xmlNod3.SelectSingleNode("BaseLine");
                                xmlNod3.RemoveChild(BaseLine);
                                blSaveDoc = true;
                            }

                            if (xmlNod3.SelectSingleNode("BaseRef") != null)
                            {
                                XmlNode BaseRef = xmlNod3.SelectSingleNode("BaseRef");
                                xmlNod3.RemoveChild(BaseRef);
                                blSaveDoc = true;
                            }

                            if (xmlNod3.SelectSingleNode("BaseEntry") != null)
                            {
                                XmlNode BaseEntry = xmlNod3.SelectSingleNode("BaseEntry");
                                xmlNod3.RemoveChild(BaseEntry);
                                blSaveDoc = true;
                            }

                            if (xmlNod3.SelectSingleNode("BaseType") != null)
                            {
                                XmlNode BaseType = xmlNod3.SelectSingleNode("BaseType");
                                xmlNod3.RemoveChild(BaseType);
                                blSaveDoc = true;
                            }

                            #endregion

                            GlobalFunction.getBaseType(Convert.ToInt32(strBaseType));

                            strQuery = string.Format("SELECT {0}.\"DocEntry\", {0}.\"DocNum\", {1}.\"LineNum\" " +
                                                     "FROM {0} INNER JOIN {1} ON {0}.\"DocEntry\" = {1}.\"DocEntry\" " +
                                                     "WHERE {0}.\"U_RefNum\" = '{2}' AND " +
                                                     "      {1}.\"LineNum\" = '{3}' ", GlobalVariable.strBTableHeader, GlobalVariable.strBTableLine3, strBaseRef, strBaseLine);

                            oRecordset = null;
                            oRecordset = (SAPbobsCOM.Recordset)GlobalVariable.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                            oRecordset.DoQuery(strQuery);

                            if (oRecordset.RecordCount > 0)
                            {
                                XmlNode NBaseType = xmlDoc.CreateElement("BaseType");
                                NBaseType.InnerText = GlobalVariable.intBObjType.ToString();
                                xmlNod3.PrependChild(NBaseType);
                                blSaveDoc = true;

                                XmlNode NBaseLine = xmlDoc.CreateElement("BaseLnNum");
                                NBaseLine.InnerText = oRecordset.Fields.Item("LineNum").Value.ToString();
                                xmlNod3.PrependChild(NBaseLine);
                                blSaveDoc = true;

                                XmlNode NBaseRef = xmlDoc.CreateElement("BaseRef");
                                NBaseRef.InnerText = oRecordset.Fields.Item("DocNum").Value.ToString();
                                xmlNod3.PrependChild(NBaseRef);
                                blSaveDoc = true;

                                XmlNode NBaseEntry = xmlDoc.CreateElement("BaseAbsEnt");
                                NBaseEntry.InnerText = oRecordset.Fields.Item("DocEntry").Value.ToString();
                                xmlNod3.PrependChild(NBaseEntry);
                                blSaveDoc = true;

                            }
                            else
                            {
                                GlobalVariable.strErrMsg = string.Format("{0} \rBase Document Not Found for Base Reference {0} - Line # {1} with Filename {2}.", GlobalVariable.strErrMsg, strBaseRef, strBaseLine, GlobalVariable.strFileName);

                                blSaveDoc = false;
                                blRetVal = false;
                            }

                        }
                    }

                    GC.Collect();

                    //update xml file
                    if (blSaveDoc == true)
                        xmlDoc.Save(strFilePath);

                    //return if validation failed
                    if (blRetVal == false)
                    {
                        SystemFunction.transHandler("Import", strTransType, strObjType, GlobalVariable.strFileName, "", "", dteStart, "E", "-999", GlobalVariable.strErrMsg);
                        return false;
                    }
                    else
                        return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), GlobalVariable.strFileName, "", "", dteStart, "E", "-111", ex.Message.ToString());
                return false;
            }
        }
    }
}
