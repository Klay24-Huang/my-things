CREATE VIEW [dbo].[VW_BE_GetEVTimeLine]
	AS 
	SELECT CarInfo.CarNo,CarInfo.FactoryYear,CarInfo.CCNum,RawData.GPSStatus,RawData.Speed,RawData.Latitude,RawData.Longitude,RawData.GPSTime
FROM TB_CarInfo AS CarInfo
INNER JOIN TB_CarRawData AS RawData ON RawData.CID=CarInfo.CID
  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetEVTimeLine';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetEVTimeLine';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台-車輛軌跡查詢', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetEVTimeLine';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetEVTimeLine';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetEVTimeLine';
