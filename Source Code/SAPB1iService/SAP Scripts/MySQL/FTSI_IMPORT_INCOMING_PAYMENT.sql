DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_INCOMING_PAYMENT`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_INCOMING_PAYMENT`(
	IN Id VARCHAR(36)
)
BEGIN
	-- HEADER --
	SELECT  	CardCode,
			CardName,
			DATE_FORMAT(T1.DocDate, "%Y%m%d") 'DocDate',
			DATE_FORMAT(T1.DocDueDate, "%Y%m%d") 'DocDueDate',
			DATE_FORMAT(T1.TaxDate, "%Y%m%d") 'TaxDate',
			DocType,
			U_RefNum, 
			U_FileName,
			TrsfrAcct,
			DocTotal,
			Id AS 'U_Id'
	FROM ftorct T1
	WHERE T1.HeaderId = Id;
END$$

DELIMITER ;