DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_GET_PROCESS_DATA`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_GET_PROCESS_DATA`(
  IN ObjType INT
)
BEGIN

-- BUSINESS PARTNER -- 
IF ObjType = 2
THEN 
	SELECT 'ftocrd' AS MySQLTable,
	        Id,
	        U_RefNum
	        
	FROM ftocrd T1
	WHERE IFNULL(IntegrationStatus, 'P') = 'P';
	
END IF;

-- ITEM MASTER DATA -- 
IF ObjType = 4
THEN 
	SELECT 'ftoitm' AS MySQLTable,
	        Id,
	        U_RefNum
	        
	FROM ftoitm T1
	WHERE IFNULL(IntegrationStatus, 'P') = 'P';
	
END IF;

-- AR CREDIT MEMO -- 
IF ObjType = 14
THEN 
	SELECT 'ftorin' AS MySQLTable,
	        Id,
		DocCur,
		DocType,
		U_RefNum
		
	FROM ftorin
	WHERE IFNULL(Posted, 'N') = 'N'
	AND IFNULL(IntegrationStatus, 'P') = 'P';
END IF;

-- SALES ORDER -- 
IF ObjType = 17
THEN 
	SELECT 'ftordr' AS MySQLTable,
	       T1.Id,
	       T1.U_RefNum,
	       T1.DocType,
	       T1.CardCode,
	       T1.DocCur,
	       T2.ItemCode
	       
	FROM ftordr T1
	LEFT JOIN ftrdr1 T2
		ON T1.Id = T2.Id
	WHERE IFNULL(Posted, 'N') = 'N'
	AND IFNULL(IntegrationStatus, 'P') = 'P';
END IF;

-- INCOMING PAYMENT -- 
IF ObjType = 24 
THEN 
	SELECT 'ftorct' AS MySQLTable,
		Id,
		U_RefNum
		
	FROM ftorct
	WHERE IFNULL(CardCode, '') <> '' 
	AND IFNULL(IntegrationStatus, 'P') = 'P';
END IF;

-- SALES BOM  -- 
IF ObjType = 66
THEN 
	SELECT 'ftoitt' AS MySQLTable,
	       T1.Id,
	       T1.Code,
	       T2.ItemCode,
	       T1.U_RefNum
	       
	FROM ftoitt T1
	LEFT JOIN ftitt1 T2
		ON T1.Id = T2.Id
	WHERE IFNULL(IntegrationStatus, 'P') = 'P';
END IF;
END$$

DELIMITER ;