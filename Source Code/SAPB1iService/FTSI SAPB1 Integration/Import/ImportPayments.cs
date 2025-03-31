using System;
using System.IO;

namespace FTSISAPB1iService
{
    class ImportPayments
    {
        private static DateTime dteStart;
        private static string strTransType;
        public static void _ImportPayments()
        {

            //importFromFiles();
            ImportIncomingPayment._ImportIncomingPayment();

        }
        private static void importFromFiles()
        {

            string[] strImpExt;

            try
            {
                dteStart = DateTime.Now;

                strTransType = "Payments - Import From File";

                if (!string.IsNullOrEmpty(GlobalVariable.strImpExt))
                {
                    strImpExt = GlobalVariable.strImpExt.Split(Convert.ToChar("|"));

                    foreach (string fileimport in strImpExt)
                    {
                        foreach (var strFile in Directory.GetFiles(GlobalVariable.strImpPath, string.Format("*{0}_{1}_{2}", GlobalVariable.strCompany, "PAY", fileimport)))
                        {
                            dteStart = DateTime.Now;

                            if (fileimport == "*.xml" || fileimport == "*.XML")
                                ImportPaymentsXML.importXMLPostPayments(strFile);
                        }
                    }
                }

                GC.Collect();
            }
            catch (Exception ex)
            {

                GlobalVariable.intErrNum = -111;
                GlobalVariable.strErrMsg = ex.Message.ToString();

                SystemFunction.transHandler("Import", strTransType, GlobalVariable.intObjType.ToString(), "", "", "", dteStart, "E", GlobalVariable.intErrNum.ToString(), GlobalVariable.strErrMsg);

                GC.Collect();

            }
        }
    }
}
