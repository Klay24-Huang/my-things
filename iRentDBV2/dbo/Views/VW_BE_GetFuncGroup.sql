CREATE VIEW [dbo].[VW_BE_GetFuncGroup]
	AS
	SELECT  [SEQNO]
      ,[FuncGroupID]
      ,[FuncGroupName]
      ,[StartDate]
      ,[EndDate] FROM [TB_FuncGroup]
                                                                  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFuncGroup';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFuncGroup';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取出功能群組列表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFuncGroup';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFuncGroup';


GO
