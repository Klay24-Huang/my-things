CREATE VIEW [dbo].[VW_GetDefPowerList]
	AS 
	SELECT  OP.OperationPowerID
      ,[OperationPowerGroupId]
      ,OP.OperationPowerCode AS Code
	  ,OP.OperationPowerName AS OPName
  FROM [dbo].[TB_OperationPowerConsist] AS Consist
  LEFT JOIN [dbo].[TB_OperationPower] AS OP ON OP.OperationPowerID=Consist.OperationPowerID

  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetDefPowerList';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetDefPowerList';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取出權限名稱及代碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetDefPowerList';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetDefPowerList';


GO