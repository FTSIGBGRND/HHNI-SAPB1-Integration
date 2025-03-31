using SAPbobsCOM;
using System;
using System.IO;
using System.Xml;

namespace FTSISAPB1iService
{
    class ImportPaymentsXML
    {
        private static DateTime dteStart;
        private static string strTransType;

        private static string strObjType, strFObjType, strVersion, strRefNum;
        private static string strMsgBod, strStatus, strPostDocNum, strPostDocEnt;

        private static bool blExist = false;

        public static void importXMLPostPayments(string strFile)
        {
            string[] strFValue;

            SAPbobsCOM.Payments oPayments;

            try
            {

                XmlDocument xmlDoc = new XmlDocument();

                GlobalVariable.strFileName = Path.GetFileName(strFile);

                strFValue = Path.GetFileNameWithoutExtension(strFile).Split(Convert.ToChar("_"));

                strFObjType = strFValue[3];
                strRefNum = strFValue[4];
                strVersion = strFValue[6];

                strTransType = "Payments - Import From File (xml)";

                //validate xml file to be process
                if (validateXMLData(strFile, strVersion))
                {
                    //process valid xml file
                    if (!(GlobalVariable.oCompany.InTransaction))
                        GlobalVariable.oCompany.StartTransaction();

                    oPayments = null;
                    oPayments = (SAPbobsCOM.Payments)GlobalVariable.oCompany.GetBusinessObjectFromXML(strFile, 0);

                    if (blExist == false)
                    {
                        //post transaction if not exist in SAP Business One Marketing Documents 
                        if (oPayments.Add() != 0)
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

                            strMsgBod = string.Format("Successfully Posted {0} - {1}. Posted Payment Number: {1} ", GlobalVariable.strDocType, GlobalVariable.strFileName, strPostDocNum);

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

                            //transfer file and send alert
                            TransferFile.transferProcFiles("Import", "E", GlobalVariable.strFileName);

                            GlobalFunction.sendAlert(strStatus, "Import", strMsgBod, GlobalVariable.oObjectType, GlobalVariable.oCompany.GetNewObjectKey().ToString());
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
        private static bool validateXMLData(string strFilePath, string strVersion)
        {
            string strQuery;

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

                    //check details data and validation
                    xmlNodLst = xmlDoc.SelectNodes(string.Format("BOM/BO/{0}/row", GlobalVariable.strTableLine1));
                    foreach (XmlNode xmlNod1 in xmlNodLst)
                    {

                        //validation details if needed
                    }

                    //invoice details data and validation
                    xmlNodLst = xmlDoc.SelectNodes(string.Format("BOM/BO/{0}/row", GlobalVariable.strTableLine2));
                    foreach (XmlNode xmlNod3 in xmlNodLst)
                    {
                        //validation details if needed

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
