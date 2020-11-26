CREATE VIEW [dbo].[VW_MA_GetAllMotoData]
	AS 
	SELECT           Car.CarNo AS CarNo, Car.StationID, Car.nowStationID AS NowStationID, 
                            ISNULL(Station2.Location, '未知') AS NowStationName, Car.available AS online, ISNULL(BookingMain.order_number, 
                            0) AS OrderStatus, IIF(BookingMain.car_mgt_status>=15,DATEADD(minute,15,Detail.final_stop_time),IIF(BookingMain.stop_time is null,DATEADD(minute,15,GETDATE()),DATEADD(minute,10,BookingMain.stop_time))) AS BookingStart
							,IIF(BookingMain.car_mgt_status>=15,DATEADD(minute,30,Detail.final_stop_time),IIF(BookingMain.stop_time is null,DATEADD(minute,30,GETDATE()),DATEADD(minute,25,BookingMain.stop_time))) AS BookingEnd
							, ISNULL(CarStatus.Latitude,-1.0) AS Lat ,ISNULL( CarStatus.Longitude,-1.0) AS Lng, CarStatus.GPSTime AS GPSTime, CarStatus.deviceLowVoltage AS LowPowStatus,ISNULL(CleanData.lastCleanTime,'2019-07-13 00:00:00') AS LastClean,
							Moto.UncleanCount AS AfterRent,ISNULL(Station2.ManageStationID,'未設管轄') AS ManageStationID,
							ISNULL(NewControl.RentCount,0) AS NewRentCount,ISNULL(NewControl.UnCleanCount,0) AS NewCleanCount,
							ISNULL(NewControl.LastCleanTime,'2020-03-23 00:00:00') AS LastCleanTime,ISNULL(NewControl.LastRentTime,'2020-03-23 00:00:00') AS LastRentTime
							,ISNULL(NewControl.LastMaintenanceMilage,0) AS LastMaintenanceMilage,CONVERT(int,ISNULL(CarStatus.Millage,0)) AS Milage
FROM              dbo.TB_Car AS Car WITH (NOLOCK) INNER JOIN
                            dbo.TB_CarInfo AS Moto WITH (NOLOCK) ON Moto.CarNo = Car.CarNo AND Moto.IsMotor=1 
LEFT JOIN dbo.TB_CarStatus AS CarStatus WITH(NOLOCK) ON Moto.CID=CarStatus.CID
LEFT JOIN dbo.TB_iRentStation AS Station2 ON Station2.StationID = Car.nowStationID

LEFT JOIN dbo.TB_OrderMain AS BookingMain WITH (NOLOCK) ON BookingMain.CarNo = Car.CarNo AND 
                            BookingMain.cancel_status = 0 AND GETDATE() BETWEEN BookingMain.start_time AND 
                            BookingMain.stop_time AND BookingMain.car_mgt_status<15
							LEFT JOIN TB_OrderDetail AS Detail ON BookingMain.order_number=Detail.order_number
							LEFT OUTER JOIN
							dbo.TB_CarCleanData AS CleanData WITH(NOLOCK) ON CleanData.CarNo=Moto.CarNo

							LEFT OUTER JOIN   dbo.TB_iRentClearControl AS NewControl WITH(NOLOCK) ON NewControl.CarNo=Car.CarNo
WHERE          (Car.nowStationID IN ('X0WR','X1JT','X1KX','X1KZ','X1KY'))
GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetAllMotoData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetAllMotoData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'整備人員取得機車資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetAllMotoData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetAllMotoData';


GO