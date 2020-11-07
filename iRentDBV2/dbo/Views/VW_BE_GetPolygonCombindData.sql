CREATE VIEW [dbo].[VW_BE_GetPolygonCombindData]
	AS 
	SELECT VW.[BLOCK_ID]
      ,VW.[StationID]
	  ,ISNULL(Station.Latitude,0.00000) AS Latitude
	  ,ISNULL(Station.Longitude,0.00000) AS Longitude
      ,[BlockName]
      ,[BlockType]
      ,[PolygonMode]
      ,VW.[Longitude] AS PolygonLongitude
      ,VW.[Latitude]  AS PolygonLatitude
      ,[MAPColor]
      ,[StartDate]
      ,[EndDate]
  FROM [VW_BE_GetPolygonData] AS VW
  LEFT JOIN TB_iRentStation AS Station ON Station.StationID=VW.StationID

        GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPolygonCombindData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPolygonCombindData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台電子欄柵資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPolygonCombindData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPolygonCombindData';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPolygonCombindData';
