DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_SALES_BOM`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_SALES_BOM`(
	IN Id  VARCHAR(36)
)
BEGIN
	-- HEADER --
	SELECT		CODE AS 'Code',
			NAME AS 'Name',
			TreeType,
			U_RefNum,
			U_Id

	FROM ftoitt T1
	WHERE T1.Id = Id;

	-- Addresses Table --
	SELECT		TYPE AS 'Type',
			ItemCode,
			Quantity,
			Warehouse,
			IssueMthd
		
	FROM ftitt1 T1 
	WHERE T1.Id = Id;
END$$

DELIMITER ;