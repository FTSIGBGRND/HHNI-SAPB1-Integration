DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_AR_CREDIT_MEMO`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_AR_CREDIT_MEMO`(
	IN Id VARCHAR(36)
)
BEGIN
	-- HEADER --
	SELECT  	CardCode,
			CardName,
			DATE_FORMAT(T1.DocDate, "%Y%m%d") 'DocDate',
			DATE_FORMAT(T1.DocDueDate, "%Y%m%d") 'DocDueDate',
			DATE_FORMAT(T1.TaxDate, "%Y%m%d") 'TaxDate',
			DocRate,
			DocType,
			U_RefNum, 
			U_FileName,
			Id AS 'U_Id'
			
	FROM ftorin T1
	WHERE T1.Id = Id;
	
	
	-- Lines --
	SELECT  	ItemCode,
			Quantity,
			PriceBefDi,
			AccountCode AS 'AcctCode',
			WTLiable,
			VatGroup,
			T2.GroupNum AS 'GroupNum',
			U_RefNum

	FROM ftrin1 T1
	LEFT JOIN ftcrd1 T2
		ON T1.AccountCode = T2.DebPayAcct
	WHERE T1.Id = Id;

END$$

DELIMITER ;