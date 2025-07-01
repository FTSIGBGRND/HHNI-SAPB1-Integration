DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_GET_PROCESS_DATA`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_GET_PROCESS_DATA`(
  IN ObjType INT
)
BEGIN
IF ObjType = 2
THEN 
	SELECT 'ftocrd' AS MySQLTable,
	        Id,
	        Series,
	        U_RefNum,
	        CardName
	        
	FROM ftocrd T1
	WHERE IFNULL(IntegrationStatus, 'P') = 'P'
	LIMIT 1000;
	
END IF;
IF ObjType = 4
THEN 
	SELECT 'ftoitm' AS MySQLTable,
	        Id,
	        Series,
	        U_RefNum,
	        ItemName
	        
	FROM ftoitm T1
	WHERE IFNULL(IntegrationStatus, 'P') = 'P'
	LIMIT 1000;
	
END IF;
IF ObjType = 14
THEN 
	SELECT 'ftorin' AS MySQLTable,
	        Id,
		DocCur,
		DocType,
		U_RefNum
		
	FROM ftorin
	WHERE IFNULL(Posted, 'N') = 'N'
	AND IFNULL(IntegrationStatus, 'P') = 'P'
	LIMIT 1000;
END IF;
IF ObjType = 17
THEN 
	SELECT 'ftordr' AS MySQLTable,
	       T1.Id,
	       T1.U_RefNum,
	       T1.DocType,
	       T1.CardCode,
	       T1.DocCur
	       
	FROM ftordr T1
	WHERE IFNULL(Posted, 'N') = 'N'
	AND IFNULL(IntegrationStatus, 'P') = 'P'
	LIMIT 1000;
END IF;
IF ObjType = 24 
THEN 
	SELECT 'ftorct' AS MySQLTable,
		Id,
		U_RefNum
		
	FROM ftorct
	WHERE IFNULL(CardCode, '') <> '' 
	AND IFNULL(IntegrationStatus, 'P') = 'P'
	LIMIT 1000;
END IF;
IF ObjType = 66
THEN 
	SELECT 'ftoitt' AS MySQLTable,
	       T1.Id,
	       T1.U_RefNum
	       
	FROM ftoitt T1
	WHERE IFNULL(IntegrationStatus, 'P') = 'P'
	LIMIT 1000;
END IF;
END$$

DELIMITER ;