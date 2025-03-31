using SAPbobsCOM;
using System;

namespace FTSISAPB1iService
{
    class FTSISAPB1Integration
    {
        public static void _FTSISAPB1Integration()
        {
            SAPbobsCOM.Recordset oRecordset;

            DateTime dteLastProc;

            string strAlRun, strProcSer, strQuery, strRunRepData;
            int timeOfDay;

            try
            {
                timeOfDay = Convert.ToInt16(DateTime.Now.ToString("HH:mm").Replace(":", ""));

                strQuery = string.Format("SELECT TOP 1 OISS.\"U_CompCode\", OISS.\"Code\", OISS.\"U_ExportFile\", OISS.\"U_ExportPath\", OISS.\"U_RunRepDta\"," +
                                         "             OISS.\"U_ImportFile\", OISS.\"U_ImportPath\", OISS.\"U_AlwaysRun\", OISS.\"U_ProcSer\", " +
                                         "             ISNULL(OISS.\"U_LProcDate\", '1900/1/1') AS \"U_LProcDate\", OISS.\"U_Delimiter\" " +
                                         "FROM \"@FTOISS\" \"OISS\" " +
                                         "WHERE OISS.\"Code\" = '{0}' AND OISS.\"U_ProcessTime\" <= {1} ", GlobalVariable.strIntCode, timeOfDay);

                oRecordset = null;
                oRecordset = (SAPbobsCOM.Recordset)GlobalVariable.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                oRecordset.DoQuery(strQuery);

                if (oRecordset.RecordCount > 0)
                {
                    strAlRun = oRecordset.Fields.Item("U_AlwaysRun").Value.ToString();
                    strProcSer = oRecordset.Fields.Item("U_ProcSer").Value.ToString();
                    dteLastProc = Convert.ToDateTime(oRecordset.Fields.Item("U_LProcDate").Value.ToString());
                    strRunRepData = oRecordset.Fields.Item("U_RunRepDta").Value.ToString();

                    if (!(string.IsNullOrEmpty(oRecordset.Fields.Item("U_Delimiter").Value.ToString())))
                        GlobalVariable.chrDlmtr = Convert.ToChar(oRecordset.Fields.Item("U_Delimiter").Value.ToString());

                    GlobalVariable.strExpExt = oRecordset.Fields.Item("U_ExportFile").Value.ToString();
                    GlobalVariable.strExpConfPath = oRecordset.Fields.Item("U_ExportPath").Value.ToString();
                    GlobalVariable.strImpExt = oRecordset.Fields.Item("U_ImportFile").Value.ToString();
                    GlobalVariable.strImpConfPath = oRecordset.Fields.Item("U_ImportPath").Value.ToString();
                    GlobalVariable.strCompany = oRecordset.Fields.Item("U_CompCode").Value.ToString();

                    if (dteLastProc != DateTime.Today)
                        updateWEBAPIEndPoint();

                    if (strRunRepData == "Y")
                    {
                        reprocessErrorData();
                    }

                    if (strAlRun == "Y")
                    {
                        Import._Import();
                        Export._Export();
                    }
                    else
                    {
                        if (dteLastProc != DateTime.Today || strProcSer == "Y")
                        {
                            Import._Import();
                            Export._Export();
                        }
                    }



                    //updateServiceSetup();
                }
                else
                {
                    SystemFunction.transHandler("Initialization", "", "", "", "", "", DateTime.Now, "E", "-001", "Integration Setup is missing. Please Check FTSI SAP Business One Integration Service Setup.");
                }
            }
            catch (Exception ex)
            {
                SystemFunction.errorAppend(string.Format("Integration Setup is missing. Please Check FTSI SAP Business One Integration Service Setup.", ex.Message.ToLower()));
            }
        }

        private static void reprocessErrorData()
        {
            try
            {
                SQLSettings.executeQuery("CALL FTSI_POS_IMPORT_REPROCESS_ERROR()");
            }
            catch (Exception ex)
            {
                SystemFunction.transHandler("Initialization", "", "", "", "", "", DateTime.Now, "E", "-001", string.Format("Error Reprocessing Data. {0}", ex.Message));
            }
            finally
            {
                if (!SystemFunction.executeQuery(string.Format("UPDATE \"@FTOISS\" SET \"U_RunRepDta\" = 'N' WHERE \"Code\" = '{0}'", GlobalVariable.strIntCode)))
                {
                    SystemFunction.transHandler("Initialization", "", "", "", "", "", DateTime.Now, "E", "-001", "Error Updating FTOISS Table.");
                }
            }
        }

        public static void updateServiceSetup()
        {
            string strQuery;

            strQuery = string.Format("UPDATE \"@FTOISS\" SET \"U_ProcSer\" = 'N', \"U_LProcDate\" = '{0}' WHERE \"Code\" = '{1}' ", DateTime.Today, GlobalVariable.strIntCode);
            if (!(SystemFunction.executeQuery(strQuery)))
            {
                GlobalVariable.intErrNum = -001;
                GlobalVariable.strErrMsg = string.Format("Error updating FTS1 SAP Business One Integration Service Setup.");

                SystemFunction.transHandler("Initialization", "", "", "", "", "", DateTime.Now, "E", GlobalVariable.intErrNum.ToString(), GlobalVariable.strErrMsg);
            }
        }
        public static void updateWEBAPIEndPoint()
        {
            string strQuery;

            strQuery = string.Format("UPDATE \"@FTISS1\" SET \"U_ProcEP\" = 'Y' WHERE \"Code\" = '{0}' ", GlobalVariable.strIntCode);
            if (!(SystemFunction.executeQuery(strQuery)))
            {
                GlobalVariable.intErrNum = -001;
                GlobalVariable.strErrMsg = string.Format("Error updating FTS1 SAP Business One Integration Service Setup.");

                SystemFunction.transHandler("Initialization", "", "", "", "", "", DateTime.Now, "E", GlobalVariable.intErrNum.ToString(), GlobalVariable.strErrMsg);
            }
        }

    }

}
