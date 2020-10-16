CREATE VIEW [dbo].[VW_GetCarDetail]
	AS 
	SELECT TOP (1000) CarInfo.[CarNo]

      ,[RentCount]
      ,[UncleanCount]
      ,[HoildayPrice]
      ,[WeekdayPrice]
      ,[Seat]
      ,[FactoryYear]
      ,[CarColor]
      ,[EngineNO]
      ,[BodyNO]
      ,[CCNum]
      ,[CID]
      ,[IsCens]
      ,[IsMotor]
	    ,ISNULL(SStation.[Location],'') AS StationName
      ,ISNULL(NStation.[Location],'') AS NowStationName
	  ,ISNULL(CarTypeData.CarTypeName,'') AS CarTypeName
	  ,ISNULL(CarTypeData.CarBrend,'') AS CarBrend
      ,Car.[available] AS NowStatus
      ,[HoildayPriceByMinutes]
      ,[WeekdayPriceByMinutes]
      ,[HasIButton]
      ,[iButtonKey]
	  ,Memo
	  ,ISNULL(Mobile.MobileNum,'') AS MobileNo

  FROM [dbo].[TB_CarInfo] AS CarInfo
  LEFT JOIN [dbo].[TB_Car] AS Car ON Car.CarNo=CarInfo.CarNo
    LEFT JOIN [dbo].[TB_CarType] AS CarTypeData ON Car.CarType=CarTypeData.CarType
  LEFT JOIN [dbo].[TB_iRentStation] AS SStation ON SStation.StationID=Car.StationID
  LEFT JOIN [dbo].[TB_iRentStation] AS NStation ON NStation.StationID=Car.nowStationID
  LEFT JOIN [dbo].[TB_CarMachine] AS CarMachine ON CarMachine.MachineNo=CarInfo.CID
  LEFT JOIN [dbo].TB_Mobile AS Mobile ON Mobile.MobileID=CarMachine.CarMachineID
      GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarDetail';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarDetail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車輛明細_後台車輛資料設定使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarDetail';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarDetail';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarDetail'