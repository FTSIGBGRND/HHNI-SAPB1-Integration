DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_SALES_ORDER`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_SALES_ORDER`(
	IN Id VARCHAR(36), DocCur VARCHAR(5), DocType VARCHAR(1)
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
		NumAtCard,
		DocType,
		U_RefNum,
		Id AS 'U_Id'
		
	FROM ftordr T1
		WHERE T1.Id = Id;
	
	ELSE
	-- If DocCur has value, include DocCur and DocRate
	SELECT  CardCode,
		CardName,
		DATE_FORMAT(T1.DocDate, "%Y%m%d") AS 'DocDate',
		DATE_FORMAT(T1.DocDueDate, "%Y%m%d") AS 'DocDueDate',
		DATE_FORMAT(T1.TaxDate, "%Y%m%d") AS 'TaxDate',
		NumAtCard,
		DocType,
		DocCur,
		DocRate,
		U_RefNum,
		Id AS 'U_Id'
		
	FROM ftordr T1
		WHERE T1.Id = Id;
	END IF;
	
	-- Item Type --
	
	IF DocType = 'I' THEN
	-- LINES --
	SELECT	LineNum,
		ItemCode,
		DESCRIPTION AS 'Dscription',
		Quantity,
		Price AS 'PriceBefDi',
		VatGroup,
		U_ArNo,
		U_NameOfCrew,
		U_Peme,
		U_Principal,
		U_Vessel,
		U_Position,
		U_Age,
		OcrCode,
		DiscPrcnt,
		U_DiscType
		
	FROM ftrdr1 T1
	WHERE T1.Id = Id;
	
	END IF;
	
	-- Service Type --
	
	IF DocType = 'S' THEN
	-- LINES --
	SELECT	LineNum,
		DESCRIPTION AS 'Dscription',
		AccountCode AS 'AcctCode',
		Quantity,
		Price AS 'PriceBefDi',
		VatGroup,
		U_ArNo,
		U_NameOfCrew,
		U_Peme,
		U_Principal,
		U_Vessel,
		U_Position,
		U_Age,
		OcrCode,
		DiscPrcnt,
		U_DiscType
		
	FROM ftrdr1 T1
	WHERE T1.Id = Id;
	
	END IF;
END$$

DELIMITER ;