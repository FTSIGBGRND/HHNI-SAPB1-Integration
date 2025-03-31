using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SAPbouiCOM;

namespace AddOn
{
    public partial class userform_integrationservicesetup : AddOn.Form
    {
        private static bool blF5 = false;
        public userform_integrationservicesetup()
        {
            InitializeComponent();
        }
        public override void comboselect(string FormUID, ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.comboselect(FormUID, ref pVal, ref BubbleEvent);

            SAPbouiCOM.Matrix oMatrix;
            SAPbouiCOM.CommonSetting oCommonSetting;

            string strTiming;

            if (!pVal.BeforeAction && pVal.ItemChanged)
            {
                switch (pVal.ItemUID)
                {
                    case "grd1":

                        oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item("grd1").Specific;
                        switch (pVal.ColUID)
                        {
                            case "Timing":

                                oForm.Freeze(true);

                                strTiming = getColumnSelectedValue("grd1", "Timing", pVal.Row, "");

                                if (strTiming == "R")
                                {
                                    oCommonSetting = oMatrix.CommonSetting;
                                    oCommonSetting.SetCellEditable(pVal.Row, 4, false);

                                    oCommonSetting = oMatrix.CommonSetting;
                                    oCommonSetting.SetCellEditable(pVal.Row, 5, false);

                                }
                                else
                                {
                                    oCommonSetting = oMatrix.CommonSetting;
                                    oCommonSetting.SetCellEditable(pVal.Row, 4, true);

                                    oCommonSetting = oMatrix.CommonSetting;
                                    oCommonSetting.SetCellEditable(pVal.Row, 5, true);
                                }

                                oForm.Update();
                                oForm.Freeze(false);

                                break;


                        }

                        break;
                }
            }
        }
        public override void itempressed(string FormUID, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.itempressed(FormUID, ref pVal, ref BubbleEvent);

            SAPbobsCOM.Recordset oRecordset;

            if (pVal.BeforeAction)
            {
                switch (pVal.ItemUID)
                {
                    case "F1":

                        oForm.Freeze(true);
                        blF5 = false;
                        oForm.PaneLevel = 101;
                        oForm.Freeze(false);

                        break;

                    case "F2":

                        oForm.Freeze(true);
                        blF5 = false;
                        oForm.PaneLevel = 102;
                        oForm.Freeze(false);

                        break;

                    case "F3":

                        oForm.Freeze(true);
                        blF5 = false;
                        oForm.PaneLevel = 103;
                        oForm.Freeze(false);

                        break;

                    case "F4":

                        oForm.Freeze(true);
                        blF5 = false;
                        oForm.PaneLevel = 104;
                        oForm.Freeze(false);

                        break;

                    case "F5":

                        oForm.Freeze(true);

                        oForm.PaneLevel = 105;

                        if (blF5 == false)
                            oForm.Items.Item("F51").Click(SAPbouiCOM.BoCellClickType.ct_Regular);
                        else
                            oForm.Items.Item("F52").Click(SAPbouiCOM.BoCellClickType.ct_Regular);

                        oForm.Freeze(false);

                        break;

                    case "F51":

                        oForm.Freeze(true);
                        oForm.PaneLevel = 106;
                        blF5 = false;
                        oForm.Freeze(false);

                        break;

                    case "F52":

                        oForm.Freeze(true);
                        oForm.PaneLevel = 107;
                        blF5 = true;
                        oForm.Freeze(false);

                        break;

                }
            }
        }
        public override void onGetCreationParams(ref SAPbouiCOM.BoFormBorderStyle io_BorderStyle, ref string is_FormType, ref string is_ObjectType, ref string xmlPath)
        {
            base.onGetCreationParams(ref io_BorderStyle, ref is_FormType, ref is_ObjectType, ref xmlPath);

            is_ObjectType = "FTOISS";
            is_FormType = "100000001";
            io_BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
        }
        public override void onFormCreate(ref bool ab_visible, ref bool ab_center)
        {
            base.onFormCreate(ref ab_visible, ref ab_center);

            SAPbouiCOM.Item oItem;
            SAPbouiCOM.CheckBox oCheckBox;
            SAPbouiCOM.EditText oEditText;
            SAPbouiCOM.Matrix oMatrix;
            SAPbouiCOM.Column oColumn;

            SAPbobsCOM.Recordset oRecordset;

            oForm.DataSources.UserDataSources.Add("F1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
            oForm.DataSources.UserDataSources.Add("F2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
            oForm.DataSources.UserDataSources.Add("F3", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
            oForm.DataSources.UserDataSources.Add("F4", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
            oForm.DataSources.UserDataSources.Add("F5", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
            oForm.DataSources.UserDataSources.Add("F51", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
            oForm.DataSources.UserDataSources.Add("F52", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);

            oForm.Title = "SAP B1 Integration Service Setup";
            oForm.Width = 850;
            oForm.Height = 450;

            oForm.Freeze(true);

            oItem = createFolder(6, 15, 150, 19, "F1", "Service Setup", "", true, "", "F1");
            oItem.AffectsFormMode = false;
            oItem = createFolder(150, 15, 150, 19, "F2", "FTP Settings", "F1", true, "", "F2");
            oItem.AffectsFormMode = false;
            oItem = createFolder(300, 15, 150, 19, "F3", "SMTP E-Mail Settings", "F1", true, "", "F3");
            oItem.AffectsFormMode = false;
            oItem = createFolder(450, 15, 165, 19, "F4", "SQL Connetion Settings", "F1", true, "", "F4");
            oItem.AffectsFormMode = false;
            oItem = createFolder(600, 15, 150, 19, "F5", "Web API Settings", "F1", true, "", "F5");
            oItem.AffectsFormMode = false;

            oItem = createFolder(16, 50, 150, 19, "F51", "JWT Access", "", true, "", "F51");
            oItem.AffectsFormMode = false;
            oItem.FromPane = 105;
            oItem.ToPane = 107;

            oItem = createFolder(150, 50, 150, 19, "F52", "End Point", "F51", true, "", "F52");
            oItem.AffectsFormMode = false;
            oItem.FromPane = 105;
            oItem.ToPane = 107;

            oItem = createRectangle(6, 37, 820, 330, "R1");

            oItem = createRectangle(16, 72, 800, 280, "R2");
            oItem.FromPane = 105;
            oItem.ToPane = 107;

            oForm.Items.Item("F1").Click(SAPbouiCOM.BoCellClickType.ct_Regular);

            #region SERVICE SETUP

            oItem = createEditText(170, 60, 150, 14, "CompCode", true, "@FTOISS", "U_CompCode");
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 60, 150, 14, "stCompCode", "Company Code", "CompCode");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createEditText(170, 90, 150, 14, "Code", true, "@FTOISS", "Code");
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 90, 150, 14, "stCode", "Service Code", "Code");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createEditText(170, 105, 350, 14, "Name", true, "@FTOISS", "Name");
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 105, 150, 14, "stName", "Service Name", "Name");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createEditText(170, 135, 350, 14, "ExpType", true, "@FTOISS", "U_ExportFile");
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 135, 150, 14, "stExpType", "Export File Type", "ExpType");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createEditText(170, 150, 620, 14, "ExpPath", true, "@FTOISS", "U_ExportPath");
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 150, 150, 14, "stExpPath", "Export Path", "ExpPath");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createButton(795, 150, 20, 14, "btnB1", "...");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createEditText(170, 165, 350, 14, "ImpType", true, "@FTOISS", "U_ImportFile");
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 165, 150, 14, "stImpType", "Import File Type", "ImpType");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createStaticText(520, 165, 200, 14, "stImpEx", "ex... (*.csv|*.xml|*.txt|*.xlsx)", "");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createEditText(170, 180, 620, 14, "ImpPath", true, "@FTOISS", "U_ImportPath");
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 180, 150, 14, "stImpPath", "Import Path", "ImpPath");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createButton(795, 180, 20, 14, "btnB2", "...");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createCombobox(170, 195, 150, 14, "Delimiter", true, "@FTOISS", "U_Delimiter");
            oItem.DisplayDesc = true;
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 195, 150, 14, "stDlmter", "File Delimeter", "Delimiter");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createEditText(170, 225, 150, 14, "ProcTime", true, "@FTOISS", "U_ProcessTime");
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 225, 150, 14, "stPrcTme", "Process Time", "ProcTime");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createCombobox(170, 240, 150, 14, "AlwysRun", true, "@FTOISS", "U_AlwaysRun");
            oItem.DisplayDesc = true;
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 240, 150, 14, "stAlwysRun", "Service Always Running?", "AlwysRun");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createCombobox(170, 255, 150, 14, "ProcSer", true, "@FTOISS", "U_ProcSer");
            oItem.DisplayDesc = true;
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 255, 150, 14, "stProcSer", "Process Service?", "ProcSer");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createEditText(170, 270, 150, 14, "LProcDate", true, "@FTOISS", "U_LProcDate");
            oItem.Enabled = false;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 270, 150, 14, "sLProcDate", "Last Process Date", "LProcDate");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createCombobox(170, 300, 150, 14, "RunRepDta", true, "@FTOISS", "U_RunRepDta");
            oItem.DisplayDesc = true;
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 300, 150, 14, "stRepData", "Reprocess Error Data?", "RunRepDta");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createCombobox(170, 315, 150, 14, "RunRepFil", true, "@FTOISS", "U_RunRepFil");
            oItem.DisplayDesc = true;
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 315, 150, 14, "stRepFile", "Reprocess Error File?", "RunRepFil");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            oItem = createCombobox(170, 330, 150, 14, "RepDate", true, "@FTOISS", "U_RepFilDate");
            oItem.DisplayDesc = true;
            oItem.Enabled = true;
            oItem.FromPane = 101;
            oItem.ToPane = 101;
            oItem = createStaticText(16, 330, 150, 14, "stRepDate", "Reprocess File Date", "RepDate");
            oItem.FromPane = 101;
            oItem.ToPane = 101;

            #endregion

            #region FTP SETTINGS

            oItem = createEditText(170, 60, 350, 14, "FTPHost", true, "@FTOISS", "U_FTPHost");
            oItem.Enabled = true;
            oItem.FromPane = 102;
            oItem.ToPane = 102;
            oItem = createStaticText(16, 60, 150, 14, "stFTPHost", "FTP Host", "FTPHost");
            oItem.FromPane = 102;
            oItem.ToPane = 102;

            oItem = createEditText(170, 90, 150, 14, "FTPUName", true, "@FTOISS", "U_FTPUserName");
            oItem.Enabled = true;
            oItem.FromPane = 102;
            oItem.ToPane = 102;
            oItem = createStaticText(16, 90, 150, 14, "stFTPUName", "FTP Username", "FTPUName");
            oItem.FromPane = 102;
            oItem.ToPane = 102;

            oItem = createEditText(170, 105, 150, 14, "FTPPass", true, "@FTOISS", "U_FTPPassword");
            oEditText = (SAPbouiCOM.EditText)oItem.Specific;
            oEditText.IsPassword = true;
            oItem.Enabled = true;
            oItem.FromPane = 102;
            oItem.ToPane = 102;
            oItem = createStaticText(16, 105, 150, 14, "stFTPPass", "FTP Password", "FTPPass");
            oItem.FromPane = 102;
            oItem.ToPane = 102;

            oItem = createEditText(170, 120, 150, 14, "FTPPort", true, "@FTOISS", "U_FTPPort");
            oItem.Enabled = true;
            oItem.FromPane = 102;
            oItem.ToPane = 102;
            oItem = createStaticText(16, 120, 150, 14, "stFTPPort", "FTP Port", "FTPPort");
            oItem.FromPane = 102;
            oItem.ToPane = 102;

            oItem = createEditText(170, 150, 635, 14, "FTPImpPath", true, "@FTOISS", "U_FTPImpPath");
            oItem.Enabled = true;
            oItem.FromPane = 102;
            oItem.ToPane = 102;
            oItem = createStaticText(16, 150, 150, 14, "stFImpPath", "FTP Import Path", "FTPImpPath");
            oItem.FromPane = 102;
            oItem.ToPane = 102;

            oItem = createEditText(170, 165, 635, 14, "FTPExpPath", true, "@FTOISS", "U_FTPExpPath");
            oItem.Enabled = true;
            oItem.FromPane = 102;
            oItem.ToPane = 102;
            oItem = createStaticText(16, 165, 150, 14, "stFExpPath", "FTP Export Path", "FTPExpPath");
            oItem.FromPane = 102;
            oItem.ToPane = 102;

            oItem = createEditText(170, 180, 635, 14, "FTPRSPath", true, "@FTOISS", "U_FTPRetSucPath");
            oItem.Enabled = true;
            oItem.FromPane = 102;
            oItem.ToPane = 102;
            oItem = createStaticText(16, 180, 150, 14, "stFRSPath", "FTP Return Success Path", "FTPRSPath");
            oItem.FromPane = 102;
            oItem.ToPane = 102;

            oItem = createEditText(170, 195, 635, 14, "FTPREPath", true, "@FTOISS", "U_FTPRetSucPath");
            oItem.Enabled = true;
            oItem.FromPane = 102;
            oItem.ToPane = 102;
            oItem = createStaticText(16, 195, 150, 14, "stFREPath", "FTP Return Error Path", "FTPREPath");
            oItem.FromPane = 102;
            oItem.ToPane = 102;

            #endregion

            #region SMTP E-MAIL SETTINGS

            oItem = createCheckBox(16, 60, 150, 15, "SMTPEnable", "Y", "N", true, "@FTOISS", "U_SMTPEnable");
            oItem.Enabled = true;
            oItem.FromPane = 103;
            oItem.ToPane = 103;
            oCheckBox = (SAPbouiCOM.CheckBox)oItem.Specific;
            oCheckBox.Caption = "Enable SMTP E-Mail";

            oItem = createEditText(170, 90, 350, 14, "SMTPHost", true, "@FTOISS", "U_SMTPHost");
            oItem.Enabled = true;
            oItem.FromPane = 103;
            oItem.ToPane = 103;
            oItem = createStaticText(16, 90, 150, 14, "stSMTPHost", "SMTP Host", "SMTPHost");
            oItem.FromPane = 103;
            oItem.ToPane = 103;

            oItem = createEditText(170, 105, 150, 14, "SMTPPort", true, "@FTOISS", "U_SMTPPort");
            oItem.Enabled = true;
            oItem.FromPane = 103;
            oItem.ToPane = 103;
            oItem = createStaticText(16, 105, 150, 14, "stSMTPPort", "SMTP Port", "SMTPPort");
            oItem.FromPane = 103;
            oItem.ToPane = 103;

            oItem = createEditText(170, 135, 150, 14, "SMTPUName", true, "@FTOISS", "U_SMTPUserName");
            oItem.Enabled = true;
            oItem.FromPane = 103;
            oItem.ToPane = 103;
            oItem = createStaticText(16, 135, 150, 14, "stSMTPUNme", "SMTP Username", "SMTPUName");
            oItem.FromPane = 103;
            oItem.ToPane = 103;

            oItem = createEditText(170, 150, 150, 14, "SMTPPass", true, "@FTOISS", "U_SMTPPassword");
            oEditText = (SAPbouiCOM.EditText)oItem.Specific;
            oEditText.IsPassword = true;
            oItem.Enabled = true;
            oItem.FromPane = 103;
            oItem.ToPane = 103;
            oItem = createStaticText(16, 150, 150, 14, "stSMTPPass", "SMTP Password", "SMTPPass");
            oItem.FromPane = 103;
            oItem.ToPane = 103;

            oItem = createEditText(170, 180, 635, 14, "EMailSbjct", true, "@FTOISS", "U_EMailSubject");
            oItem.Enabled = true;
            oItem.FromPane = 103;
            oItem.ToPane = 103;
            oItem = createStaticText(16, 180, 150, 14, "sMailSbjct", "E-Mail Subject", "EMailSbjct");
            oItem.FromPane = 103;
            oItem.ToPane = 103;

            oItem = createExtEditText(170, 195, 635, 45, "EMailTo", true, "@FTOISS", "U_EMailTo");
            oItem.Enabled = true;
            oItem.FromPane = 103;
            oItem.ToPane = 103;
            oItem = createStaticText(16, 195, 150, 14, "stEMailTo", "E-Mail To", "EMailTo");
            oItem.FromPane = 103;
            oItem.ToPane = 103;

            oItem = createExtEditText(170, 241, 635, 45, "EMailCC", true, "@FTOISS", "U_EMailCC");
            oItem.Enabled = true;
            oItem.FromPane = 103;
            oItem.ToPane = 103;
            oItem = createStaticText(16, 241, 150, 14, "stEMailCC", "E-Mail CC", "EMailCC");
            oItem.FromPane = 103;
            oItem.ToPane = 103;

            oItem = createEditText(170, 290, 610, 14, "AttchPath", true, "@FTOISS", "U_AttchPath");
            oItem.Enabled = true;
            oItem.FromPane = 103;
            oItem.ToPane = 103;
            oItem = createStaticText(16, 290, 150, 14, "stAttPath", "Attachment Path", "AttchPath");
            oItem.FromPane = 103;
            oItem.ToPane = 103;

            oItem = createButton(785, 290, 20, 14, "btnB3", "...");
            oItem.FromPane = 103;
            oItem.ToPane = 103;

            #endregion

            #region SQL CONNECTION SETTINGS

            oItem = createCombobox(170, 60, 200, 14, "SrvrType", true, "@FTOISS", "U_SQLServerType");
            oItem.DisplayDesc = true;
            oItem.Enabled = true;
            oItem.FromPane = 104;
            oItem.ToPane = 104;
            oItem = createStaticText(16, 60, 150, 14, "stSrvrType", "SQL Server Type", "SrvrType");
            oItem.FromPane = 104;
            oItem.ToPane = 104;

            oItem = createEditText(170, 90, 200, 14, "SQLSrvrNme", true, "@FTOISS", "U_SQLServerName");
            oItem.Enabled = true;
            oItem.FromPane = 104;
            oItem.ToPane = 104;
            oItem = createStaticText(16, 90, 150, 14, "stSQLSvrNm", "SQL Server Name", "SQLSrvrNme");
            oItem.FromPane = 104;
            oItem.ToPane = 104;

            oItem = createEditText(170, 105, 200, 14, "SQLPort", true, "@FTOISS", "U_SQLPort");
            oItem.Enabled = true;
            oItem.FromPane = 104;
            oItem.ToPane = 104;
            oItem = createStaticText(16, 105, 150, 14, "stPort", "SQL Port", "SQLPort");
            oItem.FromPane = 104;
            oItem.ToPane = 104;

            oItem = createEditText(170, 135, 200, 14, "SQLUsrNme", true, "@FTOISS", "U_SQLUserName");
            oItem.Enabled = true;
            oItem.FromPane = 104;
            oItem.ToPane = 104;
            oItem = createStaticText(16, 135, 150, 14, "stSQLUsrNm", "SQL Username", "SQLUsrNme");
            oItem.FromPane = 104;
            oItem.ToPane = 104;

            oItem = createEditText(170, 150, 200, 14, "SQLPsswrd", true, "@FTOISS", "U_SQLPassword");
            oEditText = (SAPbouiCOM.EditText)oItem.Specific;
            oEditText.IsPassword = true;
            oItem.Enabled = true;
            oItem.FromPane = 104;
            oItem.ToPane = 104;
            oItem = createStaticText(16, 150, 150, 14, "stSQLPass", "SQL Password", "SQLPsswrd");
            oItem.FromPane = 104;
            oItem.ToPane = 104;

            oItem = createEditText(170, 180, 200, 14, "SQLDBNme", true, "@FTOISS", "U_SQLDBName");
            oItem.Enabled = true;
            oItem.FromPane = 104;
            oItem.ToPane = 104;
            oItem = createStaticText(16, 180, 150, 14, "stSQLDBNme", "SQL Database Name", "SQLDBNme");
            oItem.FromPane = 104;
            oItem.ToPane = 104;

            #endregion

            #region WEB API SETTINGS

            oItem = createEditText(200, 90, 610, 14, "TBURL", true, "@FTOISS", "U_TokenBURL");
            oItem.Enabled = true;
            oItem.FromPane = 106;
            oItem.ToPane = 106;
            oItem = createStaticText(36, 90, 150, 14, "stTBURL", "Token Base URL", "TBURL");
            oItem.FromPane = 106;
            oItem.ToPane = 106;

            oItem = createEditText(200, 105, 610, 14, "TokenEP", true, "@FTOISS", "U_TokenEP");
            oItem.Enabled = true;
            oItem.FromPane = 106;
            oItem.ToPane = 106;
            oItem = createStaticText(36, 105, 150, 14, "stTokenEP", "Token End Point", "TokenEP");
            oItem.FromPane = 106;
            oItem.ToPane = 106;

            oItem = createEditText(200, 135, 610, 14, "TokenCId", true, "@FTOISS", "U_TokenCId");
            oItem.Enabled = true;
            oItem.FromPane = 106;
            oItem.ToPane = 106;
            oItem = createStaticText(36, 135, 150, 14, "stTokenCId", "Client ID", "TokenCId");
            oItem.FromPane = 106;
            oItem.ToPane = 106;

            oItem = createEditText(200, 150, 610, 14, "TCScrt", true, "@FTOISS", "U_TokenCScrt");
            oItem.Enabled = true;
            oItem.FromPane = 106;
            oItem.ToPane = 106;
            oItem = createStaticText(36, 150, 150, 14, "stTCScrt", "Client Secret", "TCScrt");
            oItem.FromPane = 106;
            oItem.ToPane = 106;

            oItem = createEditText(200, 165, 610, 14, "TRsrc", true, "@FTOISS", "U_TokenRsrc");
            oItem.Enabled = true;
            oItem.FromPane = 106;
            oItem.ToPane = 106;
            oItem = createStaticText(36, 165, 150, 14, "stTRsrc", "Resource", "TRsrc");
            oItem.FromPane = 106;
            oItem.ToPane = 106;

            oItem = createEditText(200, 180, 610, 14, "TGType", true, "@FTOISS", "U_TokenGType");
            oItem.Enabled = true;
            oItem.FromPane = 106;
            oItem.ToPane = 106;
            oItem = createStaticText(36, 180, 150, 14, "stTGType", "Grand Type", "TGType");
            oItem.FromPane = 106;
            oItem.ToPane = 106;

            oItem = createEditText(200, 210, 150, 14, "TUName", true, "@FTOISS", "U_TokenUserName");
            oItem.Enabled = true;
            oItem.FromPane = 106;
            oItem.ToPane = 106;
            oItem = createStaticText(36, 210, 150, 14, "stTUNme", "Username", "TUName");
            oItem.FromPane = 106;
            oItem.ToPane = 106;

            oItem = createEditText(200, 225, 150, 14, "TPass", true, "@FTOISS", "U_TokenPassword");
            oEditText = (SAPbouiCOM.EditText)oItem.Specific;
            oEditText.IsPassword = true;
            oItem.Enabled = true;
            oItem.FromPane = 106;
            oItem.ToPane = 106;
            oItem = createStaticText(36, 225, 150, 14, "stTPass", "Password", "TPass");
            oItem.FromPane = 106;
            oItem.ToPane = 106;

            oItem = createEditText(200, 255, 610, 14, "TAPIKey", true, "@FTOISS", "U_TokenAPIKey");
            oItem.Enabled = true;
            oItem.FromPane = 106;
            oItem.ToPane = 106;
            oItem = createStaticText(36, 255, 150, 14, "stTAPIKey", "Token API Key", "TAPIKey");
            oItem.FromPane = 106;
            oItem.ToPane = 106;

            oItem = createEditText(200, 90, 583, 14, "EPBURL", true, "@FTOISS", "U_EPBURL");
            oItem.Enabled = true;
            oItem.FromPane = 107;
            oItem.ToPane = 107;
            oItem = createStaticText(36, 90, 150, 14, "stEPBURL", "End Point Base URL", "EPBURL");
            oItem.FromPane = 107;
            oItem.ToPane = 107;

            oItem = createMatrix(36, 120, 765, 220, "grd1");
            oItem.AffectsFormMode = false;
            oMatrix = (SAPbouiCOM.Matrix)oItem.Specific;
            oMatrix.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single;
            oItem.FromPane = 107;
            oItem.ToPane = 107;

            oColumn = createMatrixEditText("grd1", 20, "LineId", "#", true, "@FTISS1", "LineId");
            oColumn.Editable = false;

            oColumn = createMatrixEditText("grd1", 120, "EPCode", "End Point Code", true, "@FTISS1", "U_EPCode");
            oColumn.Editable = true;

            oColumn = createMatrixEditText("grd1", 220, "EPName", "End Point Name", true, "@FTISS1", "U_EPName");
            oColumn.Editable = true;

            oColumn = createMatrixComboBox("grd1", 100, "Timing", "Timing", true, "@FTISS1", "U_Timing");
            oColumn.Editable = true;
            oColumn.DisplayDesc = true;

            oColumn = createMatrixEditText("grd1", 80, "Time", "Time", true, "@FTISS1", "U_Time");
            oColumn.Editable = false;

            oColumn = createMatrixComboBox("grd1", 100, "ProcEP", "Process End Point", true, "@FTISS1", "U_ProcEP");
            oColumn.Editable = true;
            oColumn.DisplayDesc = true;

            oColumn = createMatrixEditText("grd1", 480, "EPURL", "End Point URL", true, "@FTISS1", "U_EPURL");
            oColumn.Editable = true;

            #endregion

            oForm.DataBrowser.BrowseBy = "Code";

            oItem = createButton(6, 380, 80, 19, "1", "");
            oItem = createButton(90, 380, 80, 19, "2", "");

            oRecordset = null;
            oRecordset = (SAPbobsCOM.Recordset)DI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            oRecordset.DoQuery("SELECT \"Code\" FROM \"@FTOISS\" ");

            if (oRecordset.RecordCount > 0)
                oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE;
            else
                oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE;
        }
        public override void onadd(ref bool BubbleEvent)
        {
            base.onadd(ref BubbleEvent);

            oForm.Freeze(true);

            uf_ValidateGrid();

            if (!(uf_ValidateSave()))
            {
                uf_AddRow();

                BubbleEvent = false;
                oForm.Freeze(false);

                return;
            }

            oForm.Freeze(false);
        }
        public override void onaddmode()
        {
            base.onaddmode();

            oForm.Freeze(true);

            setItemEnabled("Code", true);
            setItemEnabled("Name", true);

            uf_AddRow();

            itemclick("F1");
            itemclick("CompCode");

            oForm.Freeze(false);

        }
        public override void onaddsuccess()
        {
            base.onaddsuccess();

            oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE;
        }
        public override void onaddrow(string matrixuid, ref bool BubbleEvent, bool innerevent)
        {
            base.onaddrow(matrixuid, ref BubbleEvent, innerevent);

            oForm.Freeze(true);

            uf_AddRow();

            GC.Collect();
            oForm.Freeze(false);
        }
        public override void onfindmode()
        {
            base.onfindmode();

            SAPbobsCOM.Recordset oRecordset;

            oForm.Freeze(true);

            oRecordset = null;
            oRecordset = (SAPbobsCOM.Recordset)DI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            oRecordset.DoQuery("SELECT \"Code\" FROM \"@FTOISS\" ");

            setItemEnabled("Code", true);

            setItemString("Code", oRecordset.Fields.Item("Code").Value.ToString());

            itemclick("1");
            itemclick("CompCode");

            setItemEnabled("Code", false);

            oForm.Freeze(false);
        }
        public override void onupdate(ref bool BubbleEvent)
        {
            base.onupdate(ref BubbleEvent);

            oForm.Freeze(true);

            uf_ValidateGrid();

            if (!(uf_ValidateSave()))
            {
                uf_AddRow();

                BubbleEvent = false;
                oForm.Freeze(false);

                return;
            }

            oForm.Freeze(false);

        }
        public override void onsetmatrixmenu(string matrixuid, bool focused)
        {
            base.onsetmatrixmenu(matrixuid, focused);

            switch (matrixuid)
            {
                case "grd1":

                    oForm.EnableMenu("1292", focused);
                    oForm.EnableMenu("1293", focused);

                    break;
            }

            GC.Collect();
        }
        public override void validate(string FormUID, ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.validate(FormUID, ref pVal, ref BubbleEvent);

            SAPbouiCOM.Matrix oMatrix;

            string strCode, strEPBURL;

            if (!pVal.BeforeAction && pVal.ItemChanged)
            {
                switch (pVal.ItemUID)
                {
                    case "EPBURL":

                        oForm.Freeze(true);

                        oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item("grd1").Specific;
                        
                        strEPBURL = getItemString("EPBURL");

                        if (!(string.IsNullOrEmpty(strEPBURL)) && (oMatrix.VisualRowCount == 0))
                            uf_AddRow();

                        oForm.Update();
                        oForm.Freeze(false);

                        break;

                    case "grd1":

                        oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item("grd1").Specific;
                        switch (pVal.ColUID)
                        {
                            case "EPCode":

                                oForm.Freeze(true);

                                strCode = getColumnString("grd1", pVal.ColUID, pVal.Row, "");

                                if (!(string.IsNullOrEmpty(strCode)) && (pVal.Row == oMatrix.VisualRowCount))
                                    uf_AddRow();

                                oForm.Update();
                                oForm.Freeze(false);

                                break;
                        }
                        break;
                }
            }
        }
        private void uf_AddRow()
        {

            SAPbouiCOM.Matrix oMatrix;
            int intLineId;

            oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item("grd1").Specific;
            if (oMatrix.RowCount == 0)
            {
                oForm.DataSources.DBDataSources.Item("@FTISS1").Clear();
            }

            intLineId = oMatrix.VisualRowCount + 1;

            oForm.DataSources.DBDataSources.Item("@FTISS1").InsertRecord(0);
            oForm.DataSources.DBDataSources.Item("@FTISS1").Offset = oForm.DataSources.DBDataSources.Item("@FTISS1").Size - 1;
            oForm.DataSources.DBDataSources.Item("@FTISS1").SetValue("LineId", oForm.DataSources.DBDataSources.Item("@FTISS1").Size - 1, intLineId.ToString());
            oForm.DataSources.DBDataSources.Item("@FTISS1").SetValue("U_EPCode", oForm.DataSources.DBDataSources.Item("@FTISS1").Size - 1, "");
            oForm.DataSources.DBDataSources.Item("@FTISS1").SetValue("U_EPName", oForm.DataSources.DBDataSources.Item("@FTISS1").Size - 1, "");
            oForm.DataSources.DBDataSources.Item("@FTISS1").SetValue("U_Timing", oForm.DataSources.DBDataSources.Item("@FTISS1").Size - 1, "");
            oForm.DataSources.DBDataSources.Item("@FTISS1").SetValue("U_Time", oForm.DataSources.DBDataSources.Item("@FTISS1").Size - 1, "");
            oForm.DataSources.DBDataSources.Item("@FTISS1").SetValue("U_ProcEP", oForm.DataSources.DBDataSources.Item("@FTISS1").Size - 1, "");
            oForm.DataSources.DBDataSources.Item("@FTISS1").SetValue("U_EPURL", oForm.DataSources.DBDataSources.Item("@FTISS1").Size - 1, "");

            oMatrix.AddRow(1, -1);

            GC.Collect();

        }
        private void uf_ValidateGrid()
        {
            string strEPCode;

            SAPbouiCOM.Matrix oMatrix;

            oForm.Freeze(true);

            oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item("grd1").Specific;
            for (int ll_row = oMatrix.VisualRowCount; ll_row > 0; ll_row--)
            {
                strEPCode = getColumnString("grd1", "EPCode", ll_row, "");
                if (string.IsNullOrEmpty(strEPCode))
                    oMatrix.DeleteRow(ll_row);

            }
            for (int ll_row = 1; ll_row <= oMatrix.VisualRowCount; ll_row++)
            {
                setColumnString("grd1", "LineId", ll_row, ll_row.ToString());
            }

            oForm.Freeze(false);

            GC.Collect();
        }
        private bool uf_ValidateSave()
        {

            if (string.IsNullOrEmpty(getItemString("CompCode")))
            {
                UI.SBO_Application.StatusBar.SetText("Company Code is missing.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                return false;
            }

            if (string.IsNullOrEmpty(getItemString("Code")))
            {
                UI.SBO_Application.StatusBar.SetText("SAP Business One Integration Code is missing.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                return false;
            }
     
            if (string.IsNullOrEmpty(getItemString("Name")))
            {
                UI.SBO_Application.StatusBar.SetText("SAP Business One Integration Name is missing.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                return false;
            }

            return true;
        }
    }
}
