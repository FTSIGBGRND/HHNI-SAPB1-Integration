using System;
using System.IO;

namespace FTSISAPB1iService
{
    class Import
    {
        public static void _Import()
        {
            importSharedFiles();
            importFTPFiles();
            importSQLDB();

            ImportDocuments._ImportDocuments();
            ImportPayments._ImportPayments();
            ImportBPMasterData._ImportBPMasterData();
            ImportUserDefinedItems._ImportUserDefinedItems();
            ImportItemMasterData._ImportItemMasterData();
        }

        private static void importSQLDB()
        {
            SQLSettings.connectSQLDB();
        }

        private static void importFTPFiles()
        {
            string[] strImpExt;

            if (!string.IsNullOrEmpty(GlobalVariable.strImpExt))
            {
                strImpExt = GlobalVariable.strImpExt.Split(Convert.ToChar("|"));

                for (int intStr = 0; intStr < strImpExt.Length; intStr++)
                {
                    TransferFile.importSFTPFiles(strImpExt[intStr]);
                }
            }
        }
        private static void importSharedFiles()
        {
            string strFileName;

            string[] strImpExt;

            if (!string.IsNullOrEmpty(GlobalVariable.strImpExt))
            {
                strImpExt = GlobalVariable.strImpExt.Split(Convert.ToChar("|"));

                for (int intStr = 0; intStr < strImpExt.Length; intStr++)
                {
                    if (!(string.IsNullOrEmpty(GlobalVariable.strImpConfPath)))
                    {
                        foreach (var strFile in Directory.GetFiles(GlobalVariable.strImpConfPath, strImpExt[intStr]))
                        {
                            strFileName = GlobalVariable.strImpPath + Path.GetFileName(strFile);
                            File.Move(strFile, strFileName);
                        }
                    }
                }
            }
        }
    }
}
