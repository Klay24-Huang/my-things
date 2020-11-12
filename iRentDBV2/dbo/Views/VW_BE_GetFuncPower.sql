CREATE VIEW [dbo].[VW_BE_GetFuncPower]
	AS 
	SELECT [FuncGroupID],[FuncGroupPower]  FROM [TB_FuncGroupPower]
	GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFuncPower';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFuncPower';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取出功能群組權限', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFuncPower';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFuncPower';


GO