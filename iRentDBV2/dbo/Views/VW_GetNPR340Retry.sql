CREATE VIEW [dbo].[VW_GetNPR340Retry]
	AS 
    SELECT
    [CUSTID]
      ,[ORDNO]
      ,[CNTRNO]
      ,[PAYMENTTYPE]
      ,[CARNO]
      ,[NORDNO]
      ,[PAYDATE]
      ,[AUTH_CODE]
      ,[AMOUNT]
      ,[CDTMAN]
      ,[CARDNO]
      ,[POLNO]
      ,[MerchantTradeNo]
      ,[ServerTradeNo]
      

  FROM [dbo].[TB_NPR340] WITH(NOLOCK) WHERE isRetry=1 
  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetNPR340Retry';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetNPR340Retry';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取出要重送NPR340沖銷的清單', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetNPR340Retry';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetNPR340Retry';


GO