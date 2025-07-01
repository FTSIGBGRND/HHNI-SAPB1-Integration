DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_ITEM_MASTERDATA`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_ITEM_MASTERDATA`(
	IN Id VARCHAR(36)
)
BEGIN
	-- HEADER --
	SELECT		Series,
			U_ItemID,
			U_PackageID,
			U_ProcedureID,
			ItemName,
			ItmsGrpCod,
			UgpEntry,
			InvntItem,
			SellItem,
			PrchseItem,
			DfltWH,
			MngMethod,
			BuyUnitMsr,
			SalUnitMsr,
			InvntryUOM,
			GLMethod,
			U_CustTag,
			U_Package,
			U_RefNum,
			Id AS 'U_Id'
	FROM ftoitm T1
	WHERE T1.Id = Id;
END$$

DELIMITER ;