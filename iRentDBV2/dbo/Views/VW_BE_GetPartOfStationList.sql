CREATE VIEW [dbo].[VW_BE_GetPartOfStationList]
	AS 
	SELECT [StationID]
      ,[Location]
      ,[ADDR]
      ,[Latitude]
      ,[Longitude]
      ,ISNULL(City.CityName,'') AS CityName
      ,ISNULL(AreaData.AreaName,'') AS AreaName
      ,[AllowParkingNum]
      ,[NowOnlineNum]

  FROM [dbo].[TB_iRentStation] AS Station
  LEFT JOIN [TB_AreaZip] AS AreaData ON AreaData.AreaID=Station.AreaID
  LEFT JOIN [TB_City] AS City ON City.CityID=Station.CityID
    GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPartOfStationList';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPartOfStationList';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台據點管理列表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPartOfStationList';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPartOfStationList';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPartOfStationList';