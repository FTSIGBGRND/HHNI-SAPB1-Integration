using System;
using System.IO;

namespace FTSISAPB1iService
{
    class ImportDocuments
    {
        private static DateTime dteStart;
        private static string strTransType;

        public static void _ImportDocuments()
        {
            //importFromFiles();

            ImportUserDefinedDocuments._ImportUserDefinedDocuments();

        }
        private static void importFromFiles()
        {

            string[] strImpExt;

            try
            {
                dteStart = DateTime.Now;

                strTransType = "Documents - Import From File";

                if (!string.IsNullOrEmpty(GlobalVariable.strImpExt))
                {
                    strImpExt = GlobalVariable.strImpExt.Split(Convert.ToChar("|"));

                    foreach (string fileimport in strImpExt)
                    {
                        foreach (var strFile in Directory.GetFiles(GlobalVariable.strImpPath, string.Format("*{0}_{1}_{2}", GlobalVariable.strCompany, "DOC", fileimport)))
                        {
                            dteStart = DateTime.Now;

                            if (fileimport == "*.xml" || fileimport == "*.XML")
                                ImportDocumentsXML.importXMLPostDocument(strFile);
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
