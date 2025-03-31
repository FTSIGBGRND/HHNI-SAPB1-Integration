using CrystalDecisions.CrystalReports.Engine;
using MySql.Data.MySqlClient;
using SAPbobsCOM;
using System;
using System.Data;
using System.Data.SqlClient;

namespace FTSISAPB1iService
{
    class SQLSettings
    {
        private static DateTime dteStart;
        public static bool connectSQLDB()
        {
            SAPbobsCOM.Recordset oRSCred;

            string strQuery;
            string strServer, strPort, strSQLDB, strDBUserName, strDBPassword;

            dteStart = DateTime.Now;

            try
            {

                oRSCred = null;
                strQuery = string.Format("SELECT TOP 1 OISS.\"U_SQLServerType\", OISS.\"U_SQLServerName\",  OISS.\"U_SQLPort\", OISS.\"U_SQLUserName\", OISS.\"U_SQLPassword\", OISS.\"U_SQLDBName\" " +
                                         "FROM \"@FTOISS\" \"OISS\" " +
                                         "WHERE OISS.\"Code\" = '{0}' ", GlobalVariable.strIntCode);

                oRSCred = null;
                oRSCred = (SAPbobsCOM.Recordset)GlobalVariable.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                oRSCred.DoQuery(strQuery);

                strServer = oRSCred.Fields.Item("U_SQLServerName").Value.ToString();

                if (!(string.IsNullOrEmpty(strServer)))
                {
                    GlobalVariable.strSQLType = oRSCred.Fields.Item("U_SQLServerType").Value.ToString();

                    strServer = oRSCred.Fields.Item("U_SQLServerName").Value.ToString();
                    strPort = oRSCred.Fields.Item("U_SQLPort").Value.ToString();

                    strDBUserName = oRSCred.Fields.Item("U_SQLUserName").Value.ToString();
                    strDBPassword = oRSCred.Fields.Item("U_SQLPassword").Value.ToString();

                    strSQLDB = oRSCred.Fields.Item("U_SQLDBName").Value.ToString();

                    if (GlobalVariable.strSQLType == "MSSQL")
                    {

                        GlobalVariable.SqlCon = new SqlConnection(string.Format("Data Source = {0}; Initial Catalog = {1}; User ID = {2}; Password = {3}", strServer, strSQLDB, strDBUserName, strDBPassword));

                        if (GlobalVariable.SqlCon.State == ConnectionState.Closed)
                            GlobalVariable.SqlCon.Open();

                        if (GlobalVariable.SqlCon.State == ConnectionState.Open)
                            GlobalVariable.SqlCon.Close();
                    }
                    else
                    {
                        GlobalVariable.MySqlCon = new MySqlConnection(string.Format("Server = {0}; Port = {1}; UID = {2}; PWD = {3}; Database = {4}", strServer, strPort, strDBUserName, strDBPassword, strSQLDB));

                        if (GlobalVariable.MySqlCon.State == ConnectionState.Closed)
                            GlobalVariable.MySqlCon.Open();

                        if (GlobalVariable.MySqlCon.State == ConnectionState.Open)
                            GlobalVariable.MySqlCon.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                SystemFunction.transHandler("Initialization", "SQL Settings", "", "", "", "", dteStart, "E", "-111", ex.Message.ToString());
                return false;
            }

            return true;
        }
        public static bool updateBaseMySQL(string strTable, string strField, string strStatus, string strFilter, string strParam)
        {

            string strQuery;

            MySqlCommand MySqlCmd;

            try
            {
                if (GlobalVariable.MySqlCon.State == ConnectionState.Closed)
                    GlobalVariable.MySqlCon.Open();

                //update base record
                strQuery = string.Format("UPDATE {0} SET {1} = '{2}' WHERE {3} = '{4}' ", strTable, strField, strStatus, strFilter, strParam);
                MySqlCmd = new MySqlCommand(strQuery, GlobalVariable.MySqlCon);
                MySqlCmd.ExecuteNonQuery();

                return true;

            }
            catch (Exception ex)
            {
                SystemFunction.transHandler("Initialization", "SQL Settings", "", "", "", "", dteStart, "E", "-111", string.Format("Error updating base MySQL Reference. {0}", ex.Message.ToString()));
                return false;
            }
            finally
            {
                if (GlobalVariable.MySqlCon.State == ConnectionState.Open)
                    GlobalVariable.MySqlCon.Close();
            }
        }

        public static DataSet getDataFromMySQL(string strQuery)
        {
            try
            {


                DataSet oDataSet = new DataSet();


                if (GlobalVariable.MySqlCon.State == ConnectionState.Closed)
                    GlobalVariable.MySqlCon.Open();

                using (MySqlDataAdapter MySqlDtaAdptr = new MySqlDataAdapter(strQuery, GlobalVariable.MySqlCon))
                {
                    MySqlDtaAdptr.Fill(oDataSet);
                }

                return oDataSet;
            }
            catch (Exception ex)
            {
                SystemFunction.errorAppend(string.Format("Error Processing Data - {0}. {1}", strQuery, ex.Message));
                throw ex;
            }
            finally
            {
                if (GlobalVariable.MySqlCon.State == ConnectionState.Open)
                    GlobalVariable.MySqlCon.Close();
            }
        }

        public static void executeQuery(string strQuery)
        {
            MySqlCommand MySqlCmd;
            try
            {

                if (GlobalVariable.MySqlCon.State == ConnectionState.Closed)
                    GlobalVariable.MySqlCon.Open();

                //update base record
                MySqlCmd = new MySqlCommand(strQuery, GlobalVariable.MySqlCon);
                MySqlCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;   
            }
            finally
            {
               if (GlobalVariable.MySqlCon.State == ConnectionState.Open)
                    GlobalVariable.MySqlCon.Close();
            }
        }
    }
}
