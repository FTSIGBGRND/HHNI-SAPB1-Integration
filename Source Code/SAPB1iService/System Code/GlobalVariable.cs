using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;

namespace FTSISAPB1iService
{
    class GlobalVariable
    {
        public static SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();

        public static SqlConnection SapSQLCon = new SqlConnection();
        public static OdbcConnection myOdbcConnection;

        public static MySqlConnection MySqlCon = new MySqlConnection();
        public static SqlConnection SqlCon = new SqlConnection();
        public static SqlConnection SapCon = new SqlConnection();

        #region "File Location"

        public static string strFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public static string strSQLScriptPath, strSAPScriptPath, strImpPath, strExpPath, strErrLogPath, strConPath,
                             strExpSucPath, strImpSucPath, strExpErrPath, strImpErrPath, strFileName, strTempPath,
                             strImpConfPath, strExpConfPath, strAttImpPath, strAttExpPath, strArcImpPath, strArcExpPath,
                             strCRPath;

        #endregion

        #region "System Variable"

        public static int intErrNum, intRetVal, intObjType, intBObjType;

        public static bool blinstalledUDO;

        public static SAPbobsCOM.BoObjectTypes oObjectType;
        public static SAPbobsCOM.BoObjectTypes oBObjectType;

        public static string strEPBaseUrl, strAPIKey;

        public static string strServer, strDBType, strDBUserName, strDBPassword, strSBOCompany, strSBOUserName, strSBOPassword;
        public static string strSQLType;

        public static string strErrLog, strErrMsg;
        public static string strDocType, strBDocType;

        public static string strEncryptKey = "Fasttrack SAP B1 Connection Settings Generator Encryption Program";

        public static string strTableHeader, strTableLine1, strTableLine2, strTableLine3, strTableLine5;

        public static string strBTableHeader, strBTableLine1, strBTableLine2, strBTableLine3, strBTableLine5;

        public static char chrDlmtr;

        public static string strImpExt, strExpExt;

        #endregion

        #region "Program Variable"

        public static bool blAlwUpdte;
        public static string strCompany = "HHNI";

        public static string strIntCode = "FTSIS";

        #endregion

        #region "DataTable"

        public static DataTable oDTImpData = new DataTable("ImportData");

        #endregion




    }
}
