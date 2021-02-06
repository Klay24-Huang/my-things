CREATE VIEW [dbo].[VW_NPR330QueryTrade]
	AS 
	SELECT
      Main.[NPR330Save_ID]

      ,[Amount]
	  ,REPLACE(IRENTORDNO,'H','') AS ArrearOrder
	  ,Main.TaishinTradeNo AS ArrearTaishinTradeNo
  FROM [dbo].[TB_NPR330Detail] AS Detail
  INNER JOIN [dbo].[TB_NPR330Save] AS Main ON Main.NPR330Save_ID=Detail.NPR330Save_ID AND Main.IsPay=1
    GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_NPR330QueryTrade';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_NPR330QueryTrade';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'NPR330組合查出台新交易序號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_NPR330QueryTrade';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_NPR330QueryTrade';


GO
