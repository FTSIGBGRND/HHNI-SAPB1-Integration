DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_ITEM_MASTERDATA`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_ITEM_MASTERDATA`(
	IN Id VARCHAR(36)
)
BEGIN
	-- HEADER --
	SELECT		Series,
			ItemCode,
			U_ItemID,
			U_PackageID,
			U_ProcedureID,
			ItemName,
			ItmsGrpCod,
			UgpEntry,
			InvntItem,
			SellItem,
			PrchseItem,
			DfltWH AS 'DefaultWarehouse'


	FROM ftoitm T1
	WHERE T1.Id = Id;

	-- Addresses Table --
	SELECT		MngMethod,
			BuyUnitMsr,
			SalUnitMsr,
			InvntryUOM,
			GLMethod,
			CustTag,
			PACKAGE AS 'Package'

	FROM ftitm1 T1 
	WHERE T1.Id = Id;
END$$

DELIMITER ;