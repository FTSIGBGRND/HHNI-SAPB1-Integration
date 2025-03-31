using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;


//********************************************************************************************
// DATE CREATED : December 2008
// REMARKS      : JOHN WILSON DE LOS SANTOS ( PROGRAMMER )
// CLASS NAME   : addon.cs
// VERSION      : Version 2.0
// NOTE         : THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY OF
//                ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//                THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//                PARTICULAR PURPOSE.
//********************************************************************************************

namespace AddOn
{
    public partial class addon : UserControl
    {
        /***************Please dont delete or change this code...******************************************/
        
        #region System Code
        public addon()
        {
            InitializeComponent();
        }
        public addon(string sConnectionString)
        {
            InitializeComponent();
            
            SystemFunction.SetApplication(sConnectionString);

            if (UI.SBO_Application != null)
            {      
                UI.displayStatus();
                UI.changeStatus("Connecting to DI API.");
                //UI.SBO_Application.MessageBox("Add UDO Failed" + System.Environment.NewLine + "Table Name: " + System.Environment.NewLine + "UDO Name: " + System.Environment.NewLine + "UDO Description: "  + System.Environment.NewLine + "Error No : " + System.Environment.NewLine + "Error Desciption : ", 1, "Ok", "", "");
                if (!(SystemFunction.SetConnectionContext() == 0))
                {
                    UI.SBO_Application.MessageBox("Failed setting a connection to DI API", 1, "OK", "", "");
                    UI.hideStatus();
                    System.Environment.Exit(0); //  Terminating the Add-On Application
                }

                //DI.oCompany = (SAPbobsCOM.Company)UI.SBO_Application.Company.GetDICompany();

                if (!(SystemFunction.ConnectToCompany() == 0))
                {

                    //UI.SBO_Application.MessageBox("Failed connecting to the company's Data Base", 1, "Ok", "", "");
                    UI.SBO_Application.MessageBox("Failed connecting to the company's Data Base. \nError Code: " + DI.oCompany.GetLastErrorCode().ToString() + "\nError Description: " + DI.oCompany.GetLastErrorDescription(), 1, "Ok", "", "");
                    UI.hideStatus();
                    System.Environment.Exit(0);
                }
                else
                {

                    globalvar.addondescription = "";

                    globalvar.userid = DI.oCompany.UserName;

                    onConnectToSBO(ref globalvar.addondescription);

                    UI.changeStatus("Connected to DI API.");
                    UI.changeStatus("Checking Add-on UDT's, Create if necessary");

                    DI.oCompany.StartTransaction();
                    /*
                     *  System Table
                     *  Please dont delete
                    */
                    #region @FTNKEY
                    if (DI.createUDT("FTNKEY", "Next Key", SAPbobsCOM.BoUTBTableType.bott_NoObject) == false)
                    {

                        if (DI.oCompany.InTransaction)
                        {
                            DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                        UI.hideStatus();
                        UI.SBO_Application.MessageBox("Failed in Creating User Define Tables", 1, "Ok", "", "");
                        System.Environment.Exit(0);
                    }
                    if (DI.createUDT("FTLIC", "LICENSE", SAPbobsCOM.BoUTBTableType.bott_MasterData) == false)
                    {
                        if (DI.oCompany.InTransaction)
                        {
                            DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                        UI.hideStatus();
                        UI.SBO_Application.MessageBox("Failed in Creating User Define Tables", 1, "Ok", "", "");
                        System.Environment.Exit(0);
                    }
                    #endregion
                    /*
                     *  End System Table
                    */
                    if (onInitTables() == false)
                    {
                        if (DI.oCompany.InTransaction)
                        {
                            DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                        UI.hideStatus();
                        UI.SBO_Application.MessageBox("Failed in Creating User Define Tables. Please see the Error Log File.", 1, "Ok", "", "");
                        System.Environment.Exit(0);
                    }
                    else
                    {
                        if (DI.oCompany.InTransaction)
                        {
                            DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        }
                    }


                    DI.oCompany.StartTransaction();
                    GC.Collect();
                    UI.changeStatus("Checking Add-on UDF's, Create if necessary");
                    /*
                     *  System Fields
                     *  Please dont delete
                    */
                    #region System Fields
                    if (DI.isUDFexists("@FTNKEY", "Nkey") == false)
                    {
                        if (DI.createUDF("@FTNKEY", "Nkey", "Next Key", SAPbobsCOM.BoFieldTypes.db_Numeric, 11, "", "", "") == false)
                        {
                            if (DI.oCompany.InTransaction)
                            {
                                DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            }
                            UI.hideStatus();
                            UI.SBO_Application.MessageBox("Failed in Creating User Define Fields", 1, "Ok", "", "");
                            System.Environment.Exit(0);
                        }
                    }
                    if (DI.isUDFexists("@FTLIC", "HKEY") == false)
                    {
                        if (DI.createUDF("@FTLIC", "HKEY", "Hardware Key", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "", "", "") == false)
                        {
                            if (DI.oCompany.InTransaction)
                            {
                                DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            }
                            UI.hideStatus();
                            UI.SBO_Application.MessageBox("Failed in Creating User Define Fields", 1, "Ok", "", "");
                            System.Environment.Exit(0);
                        }
                    }
                    if (DI.isUDFexists("@FTLIC", "AddOn") == false)
                    {
                        if (DI.createUDF("@FTLIC", "AddOn", "AddOn", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, "", "", "") == false)
                        {
                            if (DI.oCompany.InTransaction)
                            {
                                DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            }
                            UI.hideStatus();
                            UI.SBO_Application.MessageBox("Failed in Creating User Define Fields", 1, "Ok", "", "");
                            System.Environment.Exit(0);
                        }
                    }
                    if (DI.isUDFexists("@FTLIC", "Expiry") == false)
                    {
                        if (DI.createUDF("@FTLIC", "Expiry", "Expiry", SAPbobsCOM.BoFieldTypes.db_Alpha, 20, "", "", "") == false)
                        {
                            if (DI.oCompany.InTransaction)
                            {
                                DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            }
                            UI.hideStatus();
                            UI.SBO_Application.MessageBox("Failed in Creating User Define Fields", 1, "Ok", "", "");
                            System.Environment.Exit(0);
                        }
                    }
                    if (DI.isUDFexists("@FTLIC", "Key") == false)
                    {
                        if (DI.createUDF("@FTLIC", "Key", "Key", SAPbobsCOM.BoFieldTypes.db_Alpha, 210, "", "", "") == false)
                        {
                            if (DI.oCompany.InTransaction)
                            {
                                DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            }
                            UI.hideStatus();
                            UI.SBO_Application.MessageBox("Failed in Creating User Define Fields", 1, "Ok", "", "");
                            System.Environment.Exit(0);
                        }
                    }
                    #endregion
                    /*
                     *  End System Fields
                    */
                    if (onInitFields() == false)
                    {
                        if (DI.oCompany.InTransaction)
                        {
                            DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                        UI.hideStatus();
                        UI.SBO_Application.MessageBox("Failed in Creating User Define Fields. Please see the Error Log File.", 1, "Ok", "", "");
                        System.Environment.Exit(0);
                    }
                    else
                    {

                        if (DI.oCompany.InTransaction)
                        {
                            DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        }
                    }
                    GC.Collect();

                    DI.oCompany.StartTransaction();

                    globalvar.gb_installedUDO = false;
                    UI.changeStatus("Checking Add-on UDO's, Create if necessary");
                    //if (DI.createUDO("FTLIC", "", SAPbobsCOM.BoUDOObjType.boud_MasterData, "FTLIC", "", "Code", false, false, false, true) == false)
                    //{
                    //    if (DI.oCompany.InTransaction)
                    //    {
                    //        DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                    //    }
                    //    UI.hideStatus();
                    //    UI.SBO_Application.MessageBox("Failed in Creating User Define Objects", 1, "Ok", "", "");
                    //    System.Environment.Exit(0);
                    //}
                    if (onRegisterUDO() == false)
                    {
                        if (DI.oCompany.InTransaction)
                        {
                            DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        }
                        UI.hideStatus();
                        UI.SBO_Application.MessageBox("Failed in Creating User Define Objects. Please see the Error Log File.", 1, "Ok", "", "");
                        System.Environment.Exit(0);
                    }
                    else
                    {

                        if (DI.oCompany.InTransaction)
                        {
                            DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                        }
                    }
                    //UI.changeStatus("Checking License.");
                    if (!SystemFunction.checklicense(globalvar.addondescription))
                    {
                        UI.hideStatus();
                        UI.SBO_Application.MessageBox("No License For this server. \n Please get a License File from fasttrack.", 1, "Ok", "", "");
                        /*
                         *  System Menu
                         *  Please dont delete
                        */
                        //43524

                        SAPbouiCOM.Menus m_menus;
                        m_menus = UI.SBO_Application.Menus;
                        if (!m_menus.Exists("FTLIC"))
                        {
                            m_menus = m_menus.Item("43524").SubMenus;
                            m_menus.Add("FTLIC", "FT - License Admin", SAPbouiCOM.BoMenuType.mt_STRING, 3);
                            m_menus = null;
                        }
                        /*
                         *  System Menu
                         *  Please dont delete
                        */
                        return;
                    }
                    else
                    {
                        GC.Collect();
                        //UI.changeStatus("Checking Add-on Stored Procedure, Create if necessary");
                        //if (CreateStoredProc() == false)
                        //{
                        //    UI.hideStatus();
                        //    UI.SBO_Application.MessageBox("Failed in Creating Stored Procedure", 1, "Ok", "", "");
                        //    System.Environment.Exit(0);
                        //}
                        if (globalvar.gb_installedUDO == true)
                        {
                            UI.hideStatus();
                            DI.logoff();
                            return;
                        }
                        UI.changeStatus("Updating Add-on " + globalvar.addondescription + " menus...");
                        /*
                         *  System Menu
                         *  Please dont delete
                        */
                        //43524

                        SAPbouiCOM.Menus m_menus;
                        m_menus = UI.SBO_Application.Menus;
                        if (!m_menus.Exists("FTLIC"))
                        {
                            m_menus = m_menus.Item("43524").SubMenus;
                            m_menus.Add("FTLIC", "FT - License Admin", SAPbouiCOM.BoMenuType.mt_STRING, 3);
                            m_menus = null;
                        }
                        /*
                         *  System Menu
                         *  Please dont delete
                        */
                        if (onInitMenus() == false)
                        {
                            if (DI.oCompany.InTransaction)
                            {
                                DI.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            }
                            UI.hideStatus();
                            UI.SBO_Application.MessageBox("Failed in Creating User Menus", 1, "Ok", "", "");
                            System.Environment.Exit(0);
                        }
                        UI.changeStatus("Setting Filters");
                        if (onInitFilters() == false)
                        {
                            UI.hideStatus();
                            UI.SBO_Application.MessageBox("Failed in Setting Filters", 1, "Ok", "", "");
                            System.Environment.Exit(0);
                        }
                        UI.changeStatus("Executing stored procedure/s...");
                        if (onCreateStoredProcedure() == false)
                        {
                            UI.hideStatus();
                            UI.SBO_Application.MessageBox("Failed in Executing stored procedure/s", 1, "Ok", "", "");
                            System.Environment.Exit(0);
                        }
                        UI.changeStatus("Uploading Report Layout/s...");
                        if (onInitReports() == false)
                        {
                            UI.hideStatus();
                            UI.SBO_Application.MessageBox("Failed in Uploading Report/s", 1, "Ok", "", "");
                            System.Environment.Exit(0);
                        }
                        UI.hideStatus();
                    }
                    UI.SBO_Application.StatusBar.SetText("Fast Track " + globalvar.addondescription + " Add-on  Successfully connected!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }

               
            }
            else
            {
                UI.hideStatus();
                System.Windows.Forms.MessageBox.Show("Failed connecting to UI API");
            }

        }
        #endregion

        /**************************************************************************************************/

        public static Boolean onConnectToSBO(ref string addondescription)
        {
            addondescription = "FTSI SAPB1 Integration Service AddOn";
            return true;
        }
        public static void onItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {

        }
        public static Boolean onInitFields()
        {

            #region INTEGRATION SERVICE LOG

            if (DI.isUDFexists("@FTISL", "Process") == false)
                if (DI.createUDF("@FTISL", "Process", "Process", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISL", "TransType") == false)
                if (DI.createUDF("@FTISL", "TransType", "Transaction Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISL", "ObjType") == false)
                if (DI.createUDF("@FTISL", "ObjType", "Object Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISL", "TransDate") == false)
                if (DI.createUDF("@FTISL", "TransDate", "Transaction Date", SAPbobsCOM.BoFieldTypes.db_Date, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISL", "FileName") == false)
                if (DI.createUDF("@FTISL", "FileName", "FileName", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISL", "TrgtDocKey") == false)
                if (DI.createUDF("@FTISL", "TrgtDocKey", "Target Document Key", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISL", "TrgtDocNum") == false)
                if (DI.createUDF("@FTISL", "TrgtDocNum", "Target Document No", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISL", "StartTime") == false)
                if (DI.createUDF("@FTISL", "StartTime", "StartTime", SAPbobsCOM.BoFldSubTypes.st_Time, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISL", "EndTime") == false)
                if (DI.createUDF("@FTISL", "EndTime", "EndTime", SAPbobsCOM.BoFldSubTypes.st_Time, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISL", "Status") == false)
                if (DI.createUDF("@FTISL", "Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISL", "ErrorCode") == false)
                if (DI.createUDF("@FTISL", "ErrorCode", "Error Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISL", "Remarks") == false)
                if (DI.createUDF("@FTISL", "Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            #endregion

            #region INTEGRATION SERVICE SETUP

            /***********************************  SERVICE SETUP **********************************************************************/

            if (DI.isUDFexists("@FTOISS", "CompCode") == false)
                if (DI.createUDF("@FTOISS", "CompCode", "Company Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "ExportFile") == false)
                if (DI.createUDF("@FTOISS", "ExportFile", "Export File Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "ExportPath") == false)
                if (DI.createUDF("@FTOISS", "ExportPath", "Export Path", SAPbobsCOM.BoFieldTypes.db_Alpha, 200, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "ImportFile") == false)
                if (DI.createUDF("@FTOISS", "ImportFile", "Import File Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "ImportPath") == false)
                if (DI.createUDF("@FTOISS", "ImportPath", "Import Path", SAPbobsCOM.BoFieldTypes.db_Alpha, 200, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "Delimiter") == false)
                if (DI.createUDF("@FTOISS", "Delimiter", "Delimiter", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, "", "P - Pipe, T - Tab, C - Comma", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "ProcessTime") == false)
                if (DI.createUDF("@FTOISS", "ProcessTime", "Process Time", SAPbobsCOM.BoFldSubTypes.st_Time, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "AlwaysRun") == false)
                if (DI.createUDF("@FTOISS", "AlwaysRun", "Services Always Running?", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, "N", "N - No, Y - Yes", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "ProcSer") == false)
                if (DI.createUDF("@FTOISS", "ProcSer", "Process Service", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, "N", "N - No, Y - Yes", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "LProcDate") == false)
                if (DI.createUDF("@FTOISS", "LProcDate", "Last Process Date", SAPbobsCOM.BoFieldTypes.db_Date, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "RunRepDta") == false)
                if (DI.createUDF("@FTOISS", "RunRepDta", "Reprocess Error Data?", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, "N", "N - No, Y - Yes", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "RunRepFil") == false)
                if (DI.createUDF("@FTOISS", "RunRepFil", "Reprocess Error File?", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, "N", "N - No, Y - Yes", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "RepFilDate") == false)
                if (DI.createUDF("@FTOISS", "RepFilDate", "Reprocess File Date", SAPbobsCOM.BoFieldTypes.db_Date, 0, "", "", "") == false)
                    return false;

            /********************************** FTP SETTINGS ***********************************************************************************/

            if (DI.isUDFexists("@FTOISS", "FTPHost") == false)
                if (DI.createUDF("@FTOISS", "FTPHost", "FTP Host Name", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "FTPUserName") == false)
                if (DI.createUDF("@FTOISS", "FTPUserName", "FTP Username", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "FTPPassword") == false)
                if (DI.createUDF("@FTOISS", "FTPPassword", "FTP Password", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "FTPPort") == false)
                if (DI.createUDF("@FTOISS", "FTPPort", "FTP Port", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "FTPImpPath") == false)
                if (DI.createUDF("@FTOISS", "FTPImpPath", "FTP Import Path", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "FTPExpPath") == false)
                if (DI.createUDF("@FTOISS", "FTPExpPath", "FTP Export Path", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "FTPRetSucPath") == false)
                if (DI.createUDF("@FTOISS", "FTPRetSucPath", "FTP Return Success Path", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;


            if (DI.isUDFexists("@FTOISS", "FTPRetErrPath") == false)
                if (DI.createUDF("@FTOISS", "FTPRetErrPath", "FTP Return Error Path", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            /********************************** SMTP E-MAIL SETTINGS ******************************************************************************/


            if (DI.isUDFexists("@FTOISS", "SMTPEnable") == false)
                if (DI.createUDF("@FTOISS", "SMTPEnable", "SMTP Enable", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, "N", "N - No, Y - Yes", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "SMTPHost") == false)
                if (DI.createUDF("@FTOISS", "SMTPHost", "SMTP Host  Name", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "SMTPPort") == false)
                if (DI.createUDF("@FTOISS", "SMTPPort", "SMTP Port", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "SMTPUserName") == false)
                if (DI.createUDF("@FTOISS", "SMTPUserName", "SMTP Username", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "SMTPPassword") == false)
                if (DI.createUDF("@FTOISS", "SMTPPassword", "SMTP Password", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "EMailSubject") == false)
                if (DI.createUDF("@FTOISS", "EMailSubject", "E-Mail Subject", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "EMailTo") == false)
                if (DI.createUDF("@FTOISS", "EMailTo", "E-Mail To", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "EMailCC") == false)
                if (DI.createUDF("@FTOISS", "EMailCC", "E-Mail CC", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "AttchPath") == false)
                if (DI.createUDF("@FTOISS", "AttchPath", "Attachment Path", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            /********************************** WEB API SETTINGS ******************************************************************************/

            if (DI.isUDFexists("@FTOISS", "TokenBURL") == false)
                if (DI.createUDF("@FTOISS", "TokenBURL", "JWT Base URL", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "TokenEP") == false)
                if (DI.createUDF("@FTOISS", "TokenEP", "Token End Point", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "TokenCId") == false)
                if (DI.createUDF("@FTOISS", "TokenCId", "Token Client ID", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "TokenCScrt") == false)
                if (DI.createUDF("@FTOISS", "TokenCScrt", "Token Client Secret", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "TokenRsrc") == false)
                if (DI.createUDF("@FTOISS", "TokenRsrc", "Token Client Resource", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "TokenGType") == false)
                if (DI.createUDF("@FTOISS", "TokenGType", "Token Grand Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "TokenUserName") == false)
                if (DI.createUDF("@FTOISS", "TokenUserName", "Token Username", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "TokenPassword") == false)
                if (DI.createUDF("@FTOISS", "TokenPassword", "Token Password", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "TokenAPIKey") == false)
                if (DI.createUDF("@FTOISS", "TokenAPIKey", "Token API Key", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "EPBURL") == false)
                if (DI.createUDF("@FTOISS", "EPBURL", "End Point URL", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISS1", "EPCode") == false)
                if (DI.createUDF("@FTISS1", "EPCode", "End Point Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISS1", "EPName") == false)
                if (DI.createUDF("@FTISS1", "EPName", "End Point Name", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISS1", "Timing") == false)
                if (DI.createUDF("@FTISS1", "Timing", "Integration Timing", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, "", "S - Scheduled, R - Realtime", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISS1", "Time") == false)
                if (DI.createUDF("@FTISS1", "Time", "Integration Time", SAPbobsCOM.BoFldSubTypes.st_Time, 0, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISS1", "ProcEP") == false)
                if (DI.createUDF("@FTISS1", "ProcEP", "Process End Point", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, "", "Y - Yes, N - No", "") == false)
                    return false;

            if (DI.isUDFexists("@FTISS1", "EPURL") == false)
                if (DI.createUDF("@FTISS1", "EPURL", "End Point URL", SAPbobsCOM.BoFieldTypes.db_Memo, 0, "", "", "") == false)
                    return false;

            /********************************** SQL CONNECTION SETTINGS ******************************************************************************/

            if (DI.isUDFexists("@FTOISS", "SQLServerType") == false)
                if (DI.createUDF("@FTOISS", "SQLServerType", "SQL Server Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 5, "", "MSSQL - Microsoft SQL Server, MySQL - MySQL", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "SQLServerName") == false)
                if (DI.createUDF("@FTOISS", "SQLServerName", "Server Name", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "SQLPort") == false)
                if (DI.createUDF("@FTOISS", "SQLPort", "Server Port", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "SQLUserName") == false)
                if (DI.createUDF("@FTOISS", "SQLUserName", "SQL Username", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "SQLPassword") == false)
                if (DI.createUDF("@FTOISS", "SQLPassword", "SQL Password", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            if (DI.isUDFexists("@FTOISS", "SQLDBName") == false)
                if (DI.createUDF("@FTOISS", "SQLDBName", "Database Name", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, "", "", "") == false)
                    return false;

            #endregion

            return true;
        }
        public static Boolean onInitMenus()
        {

            SAPbouiCOM.Menus m_menus;
            SAPbouiCOM.Menus[] m_menus2;
            SAPbouiCOM.MenuItem innerMenu;
            int[] i;
            int a;

            m_menus = UI.SBO_Application.Menus;
            m_menus2 = new SAPbouiCOM.Menus[20];

            i = new int[20];
            for (a = 0; a < m_menus.Count; a++)
            {
                //Modules
                if (m_menus.Item(a).UID == "43520")
                {
                    m_menus2[1] = m_menus.Item(a).SubMenus;
                    for (i[1] = 0; i[1] < m_menus2[1].Count; i[1]++)
                    {

                        //Administration
                        if (m_menus2[1].Item(i[1]).UID == "3328")
                        {
                            m_menus2[2] = m_menus2[1].Item(i[1]).SubMenus;
                            for (i[2] = 0; i[2] < m_menus2[2].Count; i[2]++)
                            {
                                // Setup
                                if (m_menus2[2].Item(i[2]).UID == "43525")
                                {
                                    m_menus2[3] = m_menus2[2].Item(i[2]).SubMenus;

                                    if (!m_menus2[3].Exists("FTISA"))
                                        innerMenu = m_menus2[3].Add("FTISA", "Integration Service AddOn", SAPbouiCOM.BoMenuType.mt_POPUP, 11);

                                    for (i[3] = 0; i[3] < m_menus2[3].Count; i[3]++)
                                    {
                                        if (m_menus2[3].Item(i[3]).UID == "FTISA")
                                        {
                                            m_menus2[4] = m_menus2[3].Item(i[3]).SubMenus;

                                            if (!m_menus2[4].Exists("FTISA1"))
                                                innerMenu = m_menus2[4].Add("FTISA1", "Intgeration Service Setup", SAPbouiCOM.BoMenuType.mt_STRING, 0);
                                            
                                        }
                                    }
                                }
                            }
                        }                       
                    }
                }
            }

            GC.Collect();
            return true;
        }
        public static Boolean onInitTables()
        {

            if (DI.createUDT("FTOISS", "FT Integration Service SetUp H", SAPbobsCOM.BoUTBTableType.bott_MasterData) == false)
                return false;

            if (DI.createUDT("FTISS1", "FT Integration Service SetUp L", SAPbobsCOM.BoUTBTableType.bott_MasterDataLines) == false)
                return false;

            if (DI.createUDT("FTISL", "FT Integration Service Log", SAPbobsCOM.BoUTBTableType.bott_NoObject) == false)
                return false;

            return true;
        }
        public static Boolean onInitReports()
        {
            string filename = "";

            if (Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Reports") == false)
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Reports");
            }

            if (Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Reports"))
            {
                DirectoryInfo DirInfo = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + "\\Reports");
                FileInfo[] Files = DirInfo.GetFiles("*.rpt");
                if (Files.Length > 0)
                {
                    if (!DI.initreports())
                    {
                        return false;
                    }

                    if (!DI.uploadReportType())
                    {
                        return false;
                    }
                    foreach (FileInfo file in Files)
                    {
                        filename = Path.GetFileNameWithoutExtension(file.Name);
                        if (!DI.uploadReportLayout(filename, DirInfo.FullName + "\\" + file.ToString()))
                        {
                            return false;
                        }
                        else
                        {
                            file.Delete();
                        }
                    }
                }
            }
            return true;
        }
        public static void onMenuEvent(string FormUID, ref SAPbouiCOM.MenuEvent eventType, ref bool BubbleEvent)
        {
            int formIndex;
            if (eventType.BeforeAction)
            {

                switch (eventType.MenuUID)
                {
                    case "FTISA1":
                        formIndex = UI.generateFormIndex();
                        globalvar.sboform[formIndex] = new userform_integrationservicesetup();
                        globalvar.sboform[formIndex].createForm(formIndex);
                        break;
            
                }

            }
        }
        public static Boolean onRegisterUDO()
        {

            if (DI.createUDO("FTOISS", "Integration Service Setup", SAPbobsCOM.BoUDOObjType.boud_MasterData, "FTOISS", "FTISS1", "Code", false, false, false, false, true) == false)
                return false;

            return true;
        }
        public static Boolean onInitFilters()
        {
            return true;
        }
        public static void onStatusBarEvent(string Text, SAPbouiCOM.BoStatusBarMessageType MessageType)
        {

        }
        public static Boolean onCreateStoredProcedure()
        {
            //string filepath = "";

            //if (Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Queries") == false)
            //{
            //    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Queries");
            //}

            //if (Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Queries"))
            //{
            //    DirectoryInfo DirInfo = new DirectoryInfo(System.Windows.Forms.Application.StartupPath + "\\Queries");
            //    FileInfo[] Files = DirInfo.GetFiles("*.sql");

            //    if (Files.Length > 0)
            //    {
            //        if (SystemFunction.CreateStoredProc())
            //        {
            //            foreach (FileInfo file in Files)
            //            {
            //                filepath = DirInfo.FullName + "\\" + file.Name;
            //                if (!DI.execstoredproc(filepath))
            //                {
            //                    return false;
            //                }
            //                else
            //                {
            //                    file.Delete();
            //                }
            //            }
            //        }
            //    }
            //}
            return true;
        }
        public static void onFormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
           
        }

    }
}