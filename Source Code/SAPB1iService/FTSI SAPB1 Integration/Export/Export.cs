namespace FTSISAPB1iService
{
    class Export
    {
        public static void _Export()
        {
            ExportItems._ExportItems();

            ExportDocuments._ExportDocuments();

            ExportPayments._ExportPayments();
        }
    }
}
