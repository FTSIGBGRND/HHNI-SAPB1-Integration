namespace FTSISAPB1iService
{
    public class ImportUserDefinedItems
    {
        public static void _ImportUserDefinedItems()
        {
            //ImportItemMasterData._ImportItemMasterData();      // add itm1 table on staging db
            ImportBillOfMaterials._ImportBillOfMaterials();
        }
    }
}
