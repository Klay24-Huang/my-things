CREATE VIEW [dbo].[VW_GetAllAnyRentData]
	AS 

	  	SELECT Car.[CarNo],CarStatus.[CID],[Token],[deviceType],[ACCStatus],[GPSStatus],[GPSTime],[OBDStatus],[GPRSStatus],[PowerOnStatus],[CentralLockStatus],[DoorStatus],[LockStatus],[IndoorLightStatus],[SecurityStatus]
      ,[Speed],[Volt],[Latitude],[Longitude],[Millage],[extDeviceStatus1],[extDeviceStatus2],[extDeviceData3] ,[extDeviceData4],[extDeviceData7],WriteTime,Car.available ,Car.nowStationID
	  ,FullProj.[PROJID],[PRONAME],[PRODESC],[CarBrend],FullProj.CarTypeName,CarInfo.CarType,FullProj.[ShowStart], FullProj.[ShowEnd],FullProj.[PRSTDT],FullProj.[PRENDT],FullProj.[PRICE],FullProj.[PRICE_H]
	  ,op.OperatorICon, op.Score, cg.CarTypeImg, cg.Seat --2020-09-30 eason 補欄位 供應商, 幾顆星評價, 車輛圖示, 座位數
      FROM TB_CarStatus AS CarStatus
INNER JOIN 	  TB_Car AS Car WITH(NOLOCK) ON Car.CarNo=CarStatus.CarNo
INNER JOIN TB_OperatorBase op on op.OperatorID = Car.Operator --2020-09-30 eason
INNER JOIN TB_CarTypeGroupConsist cgc on cgc.CarType = Car.CarType --2020-09-30 eason
INNER JOIN TB_CarTypeGroup cg on cg.CarTypeGroupID = cgc.CarTypeGroupID --2020-09-30 eason
INNER JOIN TB_CarInfo As CarInfo WITH(NOLOCK) ON CarInfo.CarNo=CarStatus.CarNo
INNER JOIN VW_FullProjectCollection AS FullProj WITH(NOLOCK) ON Car.nowStationID=FullProj.StationID AND CarInfo.CarType=FullProj.CARTYPE AND FullProj.IOType='O' AND FullProj.PROJTYPE=3
	  WHERE Car.available<2 AND ((FullProj.[ShowStart]<=GETDATE() AND FullProj.[ShowEnd]>GETDATE()) OR FullProj.[PRSTDT]<=GETDATE() AND FullProj.[PRENDT]>GETDATE())
