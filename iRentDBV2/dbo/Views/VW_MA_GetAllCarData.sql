CREATE VIEW [dbo].[VW_MA_GetAllCarData]
	AS 
	SELECT           Car.CarNo AS CarNo, Car.StationID, Car.nowStationID AS NowStationID, 
                            ISNULL(Station2.Location, '未知') AS NowStationName, Car.available AS online, ISNULL(BookingMain.order_number, 
                            0) AS OrderStatus, IIF(BookingMain.car_mgt_status>=15,DATEADD(MINUTE,15,Detail.final_stop_time),IIF(BookingMain.stop_time IS NULL,DATEADD(MINUTE,15,GETDATE()),DATEADD(MINUTE,10,BookingMain.stop_time))) AS BookingStart
							,IIF(BookingMain.car_mgt_status>=15,DATEADD(MINUTE,30,Detail.final_stop_time),IIF(BookingMain.stop_time IS NULL,DATEADD(MINUTE,30,GETDATE()),DATEADD(MINUTE,25,BookingMain.stop_time))) AS BookingEnd
							, ISNULL(CarStatus.Latitude,-1.0) AS Lat, ISNULL(CarStatus.Longitude,-1) AS Lng, ISNULL(CarStatus.GPSTime,'1911-01-01 00:00:00') AS GPSTime,ISNULL(CleanData.lastCleanTime,'2019-07-13 00:00:00') AS LastClean,
							CarInfo.UncleanCount AS AfterRent,ISNULL(Station2.ManageStationID,'未設管轄') AS ManageStationID,
							ISNULL(NewControl.RentCount,0) AS NewRentCount,ISNULL(NewControl.UnCleanCount,0) AS NewCleanCount,
							ISNULL(NewControl.LastCleanTime,'2020-03-23 00:00:00') AS LastCleanTime,ISNULL(NewControl.LastRentTime,'2020-03-23 00:00:00') AS LastRentTime
							,ISNULL(NewControl.LastMaintenanceMilage,0) AS LastMaintenanceMilage,CarStatus.deviceLowVoltage AS LowPowStatus,CarStatus.Millage AS Milage
FROM              dbo.TB_Car AS Car WITH (NOLOCK) 
INNER JOIN        dbo.TB_CarInfo AS CarInfo WITH (NOLOCK) ON Car.CarNo = CarInfo.CarNo AND    CarInfo.CID <>'' AND CarInfo.IsMotor=0
LEFT OUTER JOIN   dbo.TB_CarCleanData AS CleanData WITH(NOLOCK) ON CleanData.CarNo=CarInfo.CarNo
LEFT OUTER JOIN   dbo.TB_iRentStation AS Station2  WITH (NOLOCK)ON Station2.StationID = Car.nowStationID
LEFT OUTER JOIN   dbo.TB_OrderMain AS BookingMain WITH (NOLOCK) ON BookingMain.CarNo = Car.CarNo AND BookingMain.cancel_status = 0 AND BookingMain.car_mgt_status<15 AND GETDATE() BETWEEN BookingMain.start_time AND 
                            BookingMain.stop_time 
LEFT OUTER JOIN   dbo.TB_CarStatus AS CarStatus WITH (NOLOCK) ON CarStatus.CarNo = Car.CarNo
LEFT OUTER JOIN   dbo.TB_iRentClearControl AS NewControl WITH(NOLOCK) ON NewControl.CarNo=Car.CarNo
LEFT JOIN TB_OrderDetail as Detail ON Detail.order_number=BookingMain.order_number
WHERE          (Car.nowStationID NOT IN ('X0WR','X1JT','X1KX','X1KZ','X1KY'))
  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetAllCarData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetAllCarData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'整備人員取得汽車資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetAllCarData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetAllCarData';


GO