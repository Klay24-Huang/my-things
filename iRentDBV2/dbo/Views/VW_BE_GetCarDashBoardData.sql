CREATE VIEW [dbo].[VW_BE_GetCarDashBoardData]
	AS 
	SELECT ISNULL(Car.nowStationID, '') AS StationID ,
		   ISNULL(Station.[Location], '') AS StationName,
		   ISNULL(Car.CarNo, '未綁定車輛') AS CarNo ,
		   --ISNULL(CASE WHEN CarStatus.extDeviceStatus1=1 THEN 0 WHEN CarStatus.extDeviceStatus1=0 THEN 1 END, ISNULL(Car.available, 1)) AS NowStatus,
		   IIF(ISNULL(OrderMain.order_number,0) <> 0,0,1) AS NowStatus,
		   ISNULL(deviceType, 0) AS deviceType ,
		   ISNULL(CarInfo.deviceToken, '') AS deviceToken,
		   CarStatus.CID,
		   ISNULL(CarTypeName, '') AS CarTypeName ,
		   ISNULL([ACCStatus], 0) AS ACCStatus ,
		   ISNULL([GPSStatus], 0) AS GPSStatus ,
		   ISNULL([OBDStatus], 0) AS OBDStatus ,
		   ISNULL([GPRSStatus], 0) AS GPRSStatus ,
		   ISNULL([PowerOnStatus], 0) AS PowerOnStatus ,
		   ISNULL([CentralLockStatus], 0) AS CentralLockStatus ,
		   ISNULL([IndoorLightStatus], 0) AS IndoorLightStatus ,
		   ISNULL([SecurityStatus], 0) AS SecurityStatus ,
		   IIF(ISNULL(DoorStatus, 0)='N', 0, IIF(CHARINDEX('0', DoorStatus)>0, 1, 0)) AS DoorStatus ,
		   ISNULL([Speed], 0) AS Speed ,
		   ISNULL([Volt], 0.0) AS Volt ,
		   ISNULL(CarStatus.[Latitude], 0) AS Latitude ,
		   ISNULL(CarStatus.[Longitude], 0) AS Longitude ,
		   ISNULL([Millage], -1.0) AS Millage ,
		   ISNULL([device2TBA], -1.0) AS TBA2 ,
		   ISNULL([device3TBA], -1.0) AS TBA3 ,
		   ISNULL([deviceMBA], -1.0) AS MBA ,
		   ISNULL([deviceRBA], -1.0) AS RBA ,
		   ISNULL([deviceLBA], -1.0) AS LBA ,
		   ISNULL([deviceBLE_Login], 0) AS BLELogin ,
		   ISNULL([deviceBLE_BroadCast], 0) AS BroadCast ,
		   ISNULL([deviceBat_Cover], 0) AS BatCover ,
		   ISNULL([deviceLowVoltage], 0) AS MotorLowVol ,
		   ISNULL([extDeviceStatus1], 0) AS CarRent ,
		   ISNULL([extDeviceStatus2], 0) AS iButton ,
		   ISNULL([extDeviceData3], '') AS iButtonKey ,
		   ISNULL([extDeviceData4], '') AS CustomerCardNo ,
		   ISNULL([extDeviceData5], '') AS BLE_DeviceName ,
		   ISNULL([extDeviceData6], '') AS BLE_DeviceKey ,
		   ISNULL([GPSTime], '') AS GPSTime ,
		   ISNULL(CarStatus.[UPDTime], '') AS LastUpdate ,
		   ISNULL(CarTypData.isMoto, IIF(LEN(CarStatus.CID)>4, 0, IIF(SUBSTRING(CarStatus.CID, 1, 1)='B', 1, 0))) AS isMoto ,
		   ISNULL(CarInfo.IsCens, IIF(LEN(CarStatus.CID)>4, 1, 0)) AS IsCens
	--FROM TB_CarStatus AS CarStatus
	--LEFT JOIN TB_Car AS Car WITH(NOLOCK) ON Car.CarNo=CarStatus.CarNo
	--20201110 UPD BY JERRY 修改查詢邏輯，改以車輛檔為主
	FROM TB_Car AS Car WITH(NOLOCK)
	LEFT JOIN TB_CarStatus AS CarStatus WITH(NOLOCK) ON Car.CarNo=CarStatus.CarNo
	LEFT JOIN TB_CarType AS CarTypData WITH(NOLOCK) ON CarTypData.CarType=Car.CarType
	LEFT JOIN TB_CarInfo AS CarInfo WITH(NOLOCK) ON CarInfo.CarNo=CarStatus.CarNo
	LEFT JOIN TB_CarMachine AS CarMachine WITH(NOLOCK) ON CarMachine.MachineNo=CarStatus.CID
	LEFT JOIN TB_iRentStation AS Station WITH(NOLOCK) ON Station.StationID=Car.nowStationID
	LEFT JOIN TB_OrderMain AS OrderMain WITH(NOLOCK) ON OrderMain.CarNo=Car.CarNo AND OrderMain.car_mgt_status>=4 And OrderMain.car_mgt_status<16 AND OrderMain.cancel_status=0
GO
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetCarDashBoardData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetCarDashBoardData';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台車輛中控台取得資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetCarDashBoardData';

GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetCarDashBoardData';

GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetCarDashBoardData';