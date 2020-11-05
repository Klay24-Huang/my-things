CREATE VIEW [dbo].[VW_BE_GetPolygonData]
	AS 
	SELECT [BLOCK_ID]
      ,[StationID]
      ,[BlockName]
      ,[BlockType]
      ,[PolygonMode]
      ,[Longitude]
      ,[Latitude]
      ,[MAPColor]
      ,[StartDate]
      ,[EndDate]

  FROM [TB_Polygon]
      GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPolygonData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPolygonData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取得電子欄柵資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPolygonData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPolygonData';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPolygonData';
