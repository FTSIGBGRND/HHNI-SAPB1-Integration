using Newtonsoft.Json.Linq;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace FTSISAPB1iService
{
    class WebAPIRequest
    {
        private static DateTime dteStart;

        private static string strTBaseURL, strTEndPoint, strTCId, strTCScrt,
                              strTRsrc, strTGType, strTUsername, strTPassword;
        public static bool postWebAPIRequest(string jsonData, string strUrl, string strAccessToken)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {

                    if (!string.IsNullOrEmpty(strAccessToken))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", strAccessToken));
                        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    }

                    var url = new Uri(strUrl);

                    var payLoad = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    httpClient.Timeout = TimeSpan.FromMinutes(5);
                    var response = httpClient.PostAsync(url, payLoad).Result;
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    if (responseContent.StartsWith("<"))
                    {
                        throw new HttpRequestException("Error Posting Request. Check Error Log file for more details.");
                    }
                    else
                    {
                        var jsonObject = JObject.Parse(responseContent);

                        if (response.IsSuccessStatusCode)
                            return true;
                        else
                        {

                            GlobalVariable.intErrNum = (int)jsonObject["statusCode"];
                            GlobalVariable.strErrMsg = string.Format("Posting error occured while processing Post Web API Request. {0}", (string)jsonObject["message"]);

                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalVariable.intErrNum = -111;
                SystemFunction.errorAppend(ex.StackTrace);
                GlobalVariable.strErrMsg = string.Format("Exception error occured while processing Post Web API Request. {0}", ex.Message.ToString());

                return false;

            }
        }
        public static bool getWebAPICredentials()
        {
            SAPbobsCOM.Recordset oRSCred;

            string strQuery;

            dteStart = DateTime.Now;

            try
            {

                strQuery = string.Format("SELECT TOP 1 OISS.\"U_TokenBURL\", OISS.\"U_TokenEP\", OISS.\"U_TokenCId\", OISS.\"U_TokenCScrt\", OISS.\"U_TokenRsrc\"," +
                                         "             OISS.\"U_TokenGType\", OISS.\"U_TokenUserName\", OISS.\"U_TokenPassword\", OISS.\"U_TokenAPIKey\", OISS.\"U_EPBURL\" " +
                                         "FROM \"@FTOISS\" \"OISS\" " +
                                         "WHERE OISS.\"Code\" = '{0}' ", GlobalVariable.strIntCode);

                oRSCred = null;
                oRSCred = (SAPbobsCOM.Recordset)GlobalVariable.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                oRSCred.DoQuery(strQuery);

                strTBaseURL = oRSCred.Fields.Item("U_TokenBURL").Value.ToString();
                strTEndPoint = oRSCred.Fields.Item("U_TokenEP").Value.ToString();
                strTCId = oRSCred.Fields.Item("U_TokenCId").Value.ToString();
                strTCScrt = oRSCred.Fields.Item("U_TokenCScrt").Value.ToString();
                strTRsrc = oRSCred.Fields.Item("U_TokenRsrc").Value.ToString();
                strTGType = oRSCred.Fields.Item("U_TokenGType").Value.ToString();
                strTUsername = oRSCred.Fields.Item("U_TokenUserName").Value.ToString();
                strTPassword = oRSCred.Fields.Item("U_TokenPassword").Value.ToString();

                GlobalVariable.strAPIKey = oRSCred.Fields.Item("U_TokenAPIKey").Value.ToString();
                GlobalVariable.strEPBaseUrl = oRSCred.Fields.Item("U_EPBURL").Value.ToString();


                if (string.IsNullOrEmpty(GlobalVariable.strEPBaseUrl))
                {
                    SystemFunction.transHandler("Web API Request", "API Settings", "", "", "", "", dteStart, "E", "-004", "Integration Setup is missing. Please Check FTSI SAP Business One Integration Service Setup.");
                    return false;
                }

            }
            catch (Exception ex)
            {
                SystemFunction.transHandler("Web API Request", "API Settings", "", "", "", "", dteStart, "E", "-111", ex.Message.ToString());
                return false;
            }

            return true;
        }
        public static string getAccessToken()
        {
            string strAccessToken = "";
            string strError, strErrorDescription;

            dteStart = DateTime.Now;

            try
            {
                if (getWebAPICredentials())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Accept", "application/json");

                        var requestBody = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("grant_type", strTGType),
                        new KeyValuePair<string, string>("client_id", strTCId),
                        new KeyValuePair<string, string>("client_secret", strTCScrt),
                        new KeyValuePair<string, string>("resource", strTRsrc)
                    });

                        var response = client.PostAsync(strTBaseURL + strTEndPoint, requestBody).Result;
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        if (responseContent.StartsWith("<"))
                        {
                            SystemFunction.errorAppend(responseContent);
                            throw new HttpRequestException("Error Requesting Access Token. Check Error Log file for more details.");
                        }
                        else
                        {
                            var jsonObject = JObject.Parse(responseContent);


                            if (response.IsSuccessStatusCode)
                            {
                                strAccessToken = (string)jsonObject["access_token"];
                                SystemFunction.transHandler("Web API Request", "Access Token", "", "", "", "", dteStart, "S", "", "Successfully Requested New Access Token!");
                            }
                            else
                            {
                                strAccessToken = "";

                                strError = (string)jsonObject["error"];
                                strErrorDescription = (string)jsonObject["error_description"];

                                SystemFunction.transHandler("Web API Request", "Access Token", "", "", "", "", dteStart, "E", "-005", string.Format("{0} {1}", strError, strErrorDescription));
                            }
                        }
                    }
                }

                return strAccessToken;
            }
            catch (Exception ex)
            {
                SystemFunction.transHandler("Web API Request", "Access Token", "", "", "", "", dteStart, "E", "-111", ex.Message.ToString());
                return strAccessToken;
            }
        }


        public static string getAPIEndPoint(string strEPCode)
        {
            SAPbobsCOM.Recordset oRSCred;

            string strQuery, strAPIEndPoint, strEPStatus;
            int intEPTime;
            int timeOfDay = Convert.ToInt16(DateTime.Now.ToString("HH:mm").Replace(":", ""));

            strQuery = string.Format("SELECT ISS1.\"U_EPURL\", ISS1.\"U_Timing\", ISS1.\"U_Time\", ISS1.\"U_ProcEP\" " +
                                     "FROM \"@FTISS1\" \"ISS1\" " +
                                     "WHERE ISS1.\"Code\" = '{0}' AND ISS1.\"U_EPCode\" = '{1}' ", GlobalVariable.strIntCode, strEPCode);

            oRSCred = null;
            oRSCred = (SAPbobsCOM.Recordset)GlobalVariable.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            oRSCred.DoQuery(strQuery);

            if (oRSCred.RecordCount > 0)
            {
                if (oRSCred.Fields.Item("U_Timing").Value.ToString() == "R")
                    strAPIEndPoint = oRSCred.Fields.Item("U_EPURL").Value.ToString();
                else
                {
                    intEPTime = Convert.ToInt32(oRSCred.Fields.Item("U_Time").Value.ToString());
                    strEPStatus = oRSCred.Fields.Item("U_ProcEP").Value.ToString();

                    if (intEPTime <= timeOfDay && strEPStatus == "Y")
                        strAPIEndPoint = oRSCred.Fields.Item("U_EPURL").Value.ToString();
                    else
                        strAPIEndPoint = "";
                }
            }
            else
                strAPIEndPoint = "";

            return strAPIEndPoint;

        }
        public static bool validateEP(string strEPCode)
        {
            SAPbobsCOM.Recordset oRSCred;

            string strQuery;

            strQuery = string.Format("SELECT ISS1.\"U_EPURL\", ISS1.\"U_Timing\", ISS1.\"U_Time\", ISS1.\"U_ProcEP\" " +
                                     "FROM \"@FTISS1\" \"ISS1\" " +
                                     "WHERE ISS1.\"Code\" = '{0}' AND ISS1.\"U_EPCode\" = '{1}' AND  ISS1.\"U_ProcEP\" = 'Y'", GlobalVariable.strIntCode, strEPCode);

            oRSCred = null;
            oRSCred = (SAPbobsCOM.Recordset)GlobalVariable.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            oRSCred.DoQuery(strQuery);

            if (oRSCred.RecordCount > 0)
                return true;
            else
                return false;

        }
    }
}
