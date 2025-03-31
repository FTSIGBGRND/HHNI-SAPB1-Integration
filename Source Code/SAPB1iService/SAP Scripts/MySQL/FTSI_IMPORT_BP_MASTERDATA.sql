DELIMITER $$

USE `ftdbw_halcyon`$$

DROP PROCEDURE IF EXISTS `FTSI_IMPORT_BP_MASTERDATA`$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `FTSI_IMPORT_BP_MASTERDATA`(
	IN Id VARCHAR(36)
)
BEGIN
	-- HEADER --
	SELECT		Series, 
			CardCode, 
			U_CompanyID,
			U_ChargeTo,
			U_LegalEntity, 
			CardName, 
			GroupCode, 
			Currency,
			U_Id

	FROM ftocrd T1
	WHERE T1.Id = Id;
	-- Addresses Table --
	SELECT		GroupNum, 
			DebPayAcct, 
			VatStatus, 
			ECVatGroup, 
			WTLiable, 
			WTCode, 
			E_mail, 
			Address, 
			CntctPrsn, 
			Discount,
			U_RefNum
		
	FROM ftcrd1 T1 
	WHERE T1.Id = Id;
END$$

DELIMITER ;