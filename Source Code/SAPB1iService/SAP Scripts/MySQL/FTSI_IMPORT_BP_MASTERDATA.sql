DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_BP_MASTERDATA`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_BP_MASTERDATA`(
	IN Id VARCHAR(36)
)
BEGIN
	-- HEADER --
	SELECT		Series,
			CardName,
			U_CompanyID,
			U_ChargeTo,
			U_LegalEntity, 
			GroupCode, 
			Currency,
			GroupNum, 
			DebPayAcct, 
			VatStatus, 
			WTLiable AS 'WtLiable', 
			WTCode, 
			E_mail, 
			Address, 
			Discount,
			Id AS 'U_Id',
			U_RefNum
		
	FROM ftocrd T1 
	WHERE T1.Id = Id;
	
	-- Contact Employee -- 
	SELECT		NAME AS 'Name'
	FROM ftocpr T1 
	WHERE T1.Id = Id;
END$$

DELIMITER ;