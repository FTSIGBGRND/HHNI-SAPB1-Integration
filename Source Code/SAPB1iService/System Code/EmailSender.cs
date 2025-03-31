using SAPbobsCOM;
using System;
using System.IO;
using System.Net.Mail;

namespace FTSISAPB1iService
{
    class EmailSender
    {
        private static DateTime dteStart;
        private static string strSMTPHost, strEmailUserName, strEmailPassword,
                              strEmailSubject, strEmailTo, strEmailCC, strSMTPEnbl,
                              strAttchPath;

        private static int intEmailPort;
        public static bool sendSMTPEmail(string strProcess, string strSubject, string strMailTo, string strMailCC, string strBody)
        {
            string[] strAEMailTo, strAEMailCC;

            string strAttPath = System.Windows.Forms.Application.StartupPath + "\\E-Mail Settings\\Attachment\\";

            string strSMTPSettings = System.Windows.Forms.Application.StartupPath + "\\E-Mail Settings\\E-Mail Connect Settings.ini";

            try
            {
                dteStart = DateTime.Now;

                if (getSMTPCredentials(strProcess, strSMTPSettings))
                {
                    strEmailTo = strEmailTo + strMailTo;
                    strEmailCC = strEmailCC + strMailCC;

                    strAEMailTo = strEmailTo.Split(Convert.ToChar(";"));
                    strAEMailCC = strEmailCC.Split(Convert.ToChar(";"));

                    MailMessage emailmsg = new MailMessage();
                    SmtpClient smtpServer = new SmtpClient(strSMTPHost, intEmailPort);

                    emailmsg.From = new MailAddress(strEmailUserName);

                    for (int intTo = 0; intTo < strAEMailTo.Length; intTo++)
                    {
                        if (!string.IsNullOrEmpty(strAEMailTo[intTo].Trim()))
                            emailmsg.To.Add(strAEMailTo[intTo].Trim());
                    }

                    for (int intCC = 0; intCC < strAEMailCC.Length; intCC++)
                    {
                        if (!string.IsNullOrEmpty(strAEMailCC[intCC].Trim()))
                            emailmsg.CC.Add(strAEMailCC[intCC].Trim());
                    }

                    emailmsg.Subject = strEmailSubject + strSubject;

                    emailmsg.Body = strBody;

                    smtpServer.EnableSsl = true;

                    foreach (var strFile in Directory.GetFiles(strAttPath, "*.*"))
                    {
                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(strFile);
                        emailmsg.Attachments.Add(attachment);
                    }

                    smtpServer.Credentials = new System.Net.NetworkCredential(strEmailUserName, strEmailPassword);
                    smtpServer.ServicePoint.MaxIdleTime = 2;
                    smtpServer.Send(emailmsg);

                    return true;

                }
                else
                {

                    SystemFunction.transHandler("Initialization", "E-Mail Sender", "", "", "", "", dteStart, "E", "-003", "Integration Setup is missing. Please Check FTSI SAP Business One Integration Service Setup.");
                    return false;
                }
            }
            catch (Exception ex)
            {

                SystemFunction.transHandler(strProcess, "E-Mail Sender", "", "", "", "", dteStart, "E", "-111", ex.Message.ToString());
                return false;
            }
        }
        public static bool getSMTPCredentials(string strProcess, string strPathConnect)
        {
            SAPbobsCOM.Recordset oRSCred;

            string strQuery;

            try
            {
                oRSCred = null;
                strQuery = string.Format("SELECT TOP 1 OISS.\"U_SMTPEnable\", OISS.\"U_SMTPHost\", OISS.\"U_SMTPUserName\", OISS.\"U_SMTPPassword\", " +
                                         "             OISS.\"U_SMTPPort\", OISS.\"U_EMailSubject\", OISS.\"U_EMailTo\", OISS.\"U_EMailCC\", " +
                                         "             OISS.\"U_AttchPath\" " +
                                         "FROM \"@FTOISS\" \"OISS\" " +
                                         "WHERE OISS.\"Code\" = '{0}' ", GlobalVariable.strIntCode);

                oRSCred = null;
                oRSCred = (SAPbobsCOM.Recordset)GlobalVariable.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                oRSCred.DoQuery(strQuery);

                strSMTPEnbl = oRSCred.Fields.Item("U_SMTPEnable").Value.ToString();

                if (strSMTPEnbl != "Y")
                    return false;

                strSMTPHost = oRSCred.Fields.Item("U_SMTPHost").Value.ToString();

                intEmailPort = Convert.ToInt32(oRSCred.Fields.Item("U_SMTPPort").Value.ToString());
                strEmailUserName = oRSCred.Fields.Item("U_SMTPUserName").Value.ToString();
                strEmailPassword = oRSCred.Fields.Item("U_SMTPPassword").Value.ToString();
                strEmailTo = oRSCred.Fields.Item("U_EMailTo").Value.ToString();
                strEmailCC = oRSCred.Fields.Item("U_EMailCC").Value.ToString();
                strEmailSubject = oRSCred.Fields.Item("U_EMailSubject").Value.ToString();
                strAttchPath = oRSCred.Fields.Item("U_AttchPath").Value.ToString();

            }
            catch (Exception ex)
            {
                SystemFunction.transHandler("Initialization", "E -Mail Sender", "", "", "", "", dteStart, "E", "-111", ex.Message.ToString());
                return false;
            }
            return true;
        }
    }
}
