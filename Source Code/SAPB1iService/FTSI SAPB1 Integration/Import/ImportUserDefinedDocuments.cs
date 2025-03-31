namespace FTSISAPB1iService
{
    class ImportUserDefinedDocuments
    {
        public static void _ImportUserDefinedDocuments()
        {
            ImportSalesOrder._ImportSalesOrder(); 
            ImportARCreditMemo._ImportARCreditMemo();
        }
        public static bool importUpdateSAPDocuments(string strTable, string strField, string strFValue, string strStatus, string strParam)
        {
            string strQuery;

            //update SAP B1 Marketing Documents
            strQuery = string.Format("UPDATE {0} SET \"{1}\" = '{2}', \"U_isExtract\" = '{3}' WHERE \"DocNum\" = '{4}' ", strTable, strField, strFValue, strStatus, strParam);
            if (!(SystemFunction.executeQuery(strQuery)))
                return false;
            else
                return true;

        }
    }

}
