CREATE VIEW [dbo].[VW_BE_GetUserGroup]
	AS
	SELECT [USEQNO]
      ,[UserGroupID]
      ,[UserGroupName]
      ,UserGroup.[OperatorID]
      ,UserGroup.[StartDate]
      ,UserGroup.[EndDate]
      ,Operator.OperatorName
  FROM [TB_UserGroup] AS UserGroup
  LEFT JOIN TB_OperatorBase AS Operator ON Operator.OperatorID=UserGroup.OperatorID
                                                              GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetUserGroup';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetUserGroup';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取得使用者群組列表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetUserGroup';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetUserGroup';


GO