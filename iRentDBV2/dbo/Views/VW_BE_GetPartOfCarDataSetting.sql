CREATE VIEW [dbo].[VW_BE_GetPartOfCarDataSetting]
	AS 
	SELECT Car.[CarNo]
      ,ISNULL(SStation.[Location],'') AS StationName
      ,ISNULL(NStation.[Location],'') AS NowStationName
	  ,ISNULL(CarTypeData.CarTypeName,'') AS CarTypeName
	  ,ISNULL(CarTypeData.CarBrend,'') AS CarBrend
      ,Car.[available] AS NowStatus
	  ,ISNULL(CarInfo.Memo,'') AS Memo
	  ,ISNULL(CarInfo.CID,'') AS CID
	  ,Car.StationID
	  ,Car.nowStationID

  FROM [dbo].[TB_Car] AS Car
  LEFT JOIN [dbo].[TB_CarInfo] AS CarInfo On CarInfo.CarNo=Car.CarNo
  LEFT JOIN [dbo].[TB_CarType] AS CarTypeData ON Car.CarType=CarTypeData.CarType
  LEFT JOIN [dbo].[TB_iRentStation] AS SStation ON SStation.StationID=Car.StationID
  LEFT JOIN [dbo].[TB_iRentStation] AS NStation ON NStation.StationID=Car.nowStationID
    GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPartOfCarDataSetting';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPartOfCarDataSetting';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台車輛資料設定使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPartOfCarDataSetting';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPartOfCarDataSetting';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetPartOfCarDataSetting';
