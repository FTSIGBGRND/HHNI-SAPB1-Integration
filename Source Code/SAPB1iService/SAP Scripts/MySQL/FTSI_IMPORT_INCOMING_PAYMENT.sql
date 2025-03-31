DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_INCOMING_PAYMENT`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_INCOMING_PAYMENT`(
	IN Id VARCHAR(36)
)
BEGIN
	-- HEADER --
	SELECT  CardCode,
			CardName,
			DocDate,
			DocDueDate,
			TaxDate,
			DocType,
			U_RefNum, 
			U_FileName,
			Id AS 'U_Id'
			
	FROM ftorct T1
	WHERE T1.Id = Id;
	-- Lines --
	SELECT  TrsfrAcct,
			Total,
			U_RefNum

	FROM ftrct1 T1
	WHERE T1.HeaderId = Id;
END$$

DELIMITER ;