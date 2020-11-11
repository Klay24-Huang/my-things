CREATE VIEW [dbo].[VW_GetMenuList]
	AS 
	SELECT 
      RootMenu.MenuId
      ,RootMenu.[MenuName]
      ,RootMenu.[Sort] 
	  ,ISNULL(SubMenu.[SubMenuCode]			 ,'') AS [SubMenuCode]
      ,ISNULL(SubMenu.[SubMenuName]			 ,'') AS [SubMenuName]
      ,ISNULL(SubMenu.[MenuController]		 ,'') AS [MenuController]
	  ,ISNULL(SubMenu.[MenuAction]			 ,'') AS [MenuAction]
	  ,ISNULL(SubMenu.[Sort],0) AS SubMenuSort
      ,ISNULL(SubMenu.[isNewWindow]			 ,0) AS [isNewWindow]
      ,ISNULL(SubMenu.[OperationPowerGroupId],0) AS [OperationPowerGroupId]
 
  FROM [dbo].[TB_MenuRoot] AS RootMenu 
  LEFT JOIN [TB_SubMenu] AS SubMenu ON RootMenu.MenuId=SubMenu.RootMenuID
  WHERE use_flag=1

  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetMenuList';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetMenuList';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取出選單及權限', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetMenuList';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetMenuList';


GO