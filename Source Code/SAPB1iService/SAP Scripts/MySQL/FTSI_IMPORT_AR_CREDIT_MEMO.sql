DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_AR_CREDIT_MEMO`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_AR_CREDIT_MEMO`(
	IN Id VARCHAR(36), DocCur VARCHAR(5),  DocType VARCHAR(1)
)
BEGIN
	
	-- HEADER --
	IF DocCur = '' THEN
	-- If DocCur is empty, exclude DocCur and DocRate
	SELECT  CardCode,
		CardName,
		DATE_FORMAT(T1.DocDate, "%Y%m%d") AS 'DocDate',
		DATE_FORMAT(T1.DocDueDate, "%Y%m%d") AS 'DocDueDate',
		DATE_FORMAT(T1.TaxDate, "%Y%m%d") AS 'TaxDate',
		DocType,
		U_RefNum,
		Id AS 'U_Id'
		
	FROM ftorin T1
		WHERE T1.Id = Id;
	
	ELSE
	-- If DocCur has value, include DocCur and DocRate
	SELECT  CardCode,
		CardName,
		DATE_FORMAT(T1.DocDate, "%Y%m%d") AS 'DocDate',
		DATE_FORMAT(T1.DocDueDate, "%Y%m%d") AS 'DocDueDate',
		DATE_FORMAT(T1.TaxDate, "%Y%m%d") AS 'TaxDate',
		DocType,
		DocCur,
		DocRate,
		U_RefNum,
		Id AS 'U_Id'
		
	FROM ftorin T1
		WHERE T1.Id = Id;
	END IF;
	
	-- Item Type --
	
	IF DocType = 'I' THEN
	-- Lines --
	SELECT  	ItemCode,
			Quantity,
			PriceBefDi,
			AccountCode AS 'AcctCode',
			WTLiable AS 'WtLiable',
			VatGroup,
			U_RefNum
	FROM ftrin1 T1
	WHERE T1.Id = Id;
	
	END IF;
	
	-- Service Type --
	
	IF DocType = 'S' THEN
	-- Lines --
	SELECT  	Quantity,
			PriceBefDi,
			AccountCode AS 'AcctCode',
			WTLiable AS 'WtLiable',
			VatGroup,
			U_RefNum
	FROM ftrin1 T1
	WHERE T1.Id = Id;
	
	END IF;
END$$

DELIMITER ;