CREATE VIEW [dbo].[VW_BE_GetUserData]
	AS 
	SELECT Manager.[SEQNO]
      ,Manager.[Account]
      ,Manager.[UserName]
      ,Manager.[Operator]
	  ,ISNULL(Operator.OperatorName,'') AS OperatorName
      ,Manager.[UserGroup]
      ,Manager.[UserGroupID]
	  ,ISNULL(UserGroup.UserGroupName,'') AS UserGroupName
      ,IIF(ISNULL(Manager.[PowerList],'')='',ISNULL(FuncPower.FuncGroupPower,''),Manager.[PowerList]) AS PowerList
      ,Manager.[StartDate]
      ,Manager.[EndDate]

  FROM [dbo].[TB_Manager] AS Manager
  LEFT JOIN TB_UserGroup As UserGroup ON UserGroup.USEQNO=Manager.UserGroupID --AND (DATEADD(HOUR,8,GETDATE()) BETWEEN UserGroup.StartDate AND UserGroup.EndDate)
    LEFT JOIN TB_OperatorBase AS Operator ON Operator.OperatorID=Manager.Operator --AND (DATEADD(HOUR,8,GETDATE()) BETWEEN Operator.StartDate AND Operator.EndDate)
  LEFT JOIN TB_FuncGroupPower AS FuncPower ON FuncPower.FuncGroupID=UserGroup.FuncGroupID
  /*上線後可修正↓或將↑註解拿掉
    INNER JOIN TB_UserGroup As UserGroup ON UserGroup.USEQNO=Manager.UserGroupID AND (DATEADD(HOUR,8,GETDATE()) BETWEEN UserGroup.StartDate AND UserGroup.EndDate)
    INNER JOIN TB_OperatorBase AS Operator ON Operator.OperatorID=Manager.Operator AND (DATEADD(HOUR,8,GETDATE()) BETWEEN Operator.StartDate AND Operator.EndDate)
    LEFT JOIN TB_FuncGroupPower AS FuncPower ON FuncPower.FuncGroupID=UserGroup.FuncGroupID
  */

  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetUserData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetUserData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取出使用者維護列表資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetUserData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetUserData';


GO