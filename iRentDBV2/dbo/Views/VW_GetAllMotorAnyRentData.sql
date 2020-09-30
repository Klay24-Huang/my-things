CREATE VIEW [dbo].[VW_GetAllMotorAnyRentData]
	AS 
SELECT Car.[CarNo],CarStatus.[CID],[Token],[deviceType],[ACCStatus],[GPSStatus],[GPSTime],[GPRSStatus],[Speed],[Volt],[Latitude],[Longitude],[Millage],[deviceCourse]
      ,[deviceRPM],[device2TBA],[device3TBA],[deviceRSOC],[deviceRDistance],[deviceMBA],[deviceMBAA],[deviceMBAT_Hi],[deviceMBAT_Lo],[deviceRBA]
      ,[deviceRBAA],[deviceRBAT_Hi],[deviceRBAT_Lo],[deviceLBA],[deviceLBAA],[deviceLBAT_Hi],[deviceLBAT_Lo],[deviceTMP]
      ,[deviceCur],[deviceTPS],[deviceVOL],[deviceErr],[deviceALT],[deviceGx],[deviceGy],[deviceGz]
      ,[deviceBLE_Login],[deviceBLE_BroadCast],[devicePwr_Mode],[deviceReversing],[devicePut_Down]
      ,[devicePwr_Relay],[deviceStart_OK],[deviceHard_ACC],[deviceEMG_Break],[deviceSharp_Turn]
      ,[deviceBat_Cover],[extDeviceStatus1],[extDeviceData5],[extDeviceData6],Car.available 
	  ,FullProj.[PROJID],[PRONAME],[PRODESC],[CarBrend],[CarTypeName],CarInfo.CarType,FullProj.[ShowStart], FullProj.[ShowEnd]
	  ,FullProj.[PRSTDT],FullProj.[PRENDT],FullProj.[PRICE],FullProj.[PRICE_H]
	  ,OperatorBase.[OperatorICon],OperatorBase.[Score]  -- 2020.09.30 育誠 增加業者、業者評分
	 FROM TB_CarStatus AS CarStatus
INNER JOIN TB_Car AS Car WITH(NOLOCK) ON Car.CarNo=CarStatus.CarNo
INNER JOIN TB_CarInfo As CarInfo WITH(NOLOCK) ON CarInfo.CarNo=CarStatus.CarNo
INNER JOIN VW_FullProjectCollection AS FullProj WITH(NOLOCK) ON Car.nowStationID=FullProj.StationID AND CarInfo.CarType=FullProj.CARTYPE AND FullProj.IOType='O' AND FullProj.PROJTYPE=4
INNER JOIN TB_OperatorBase As OperatorBase On OperatorBase.OperatorID=Car.Operator
WHERE Car.available<2 AND ((FullProj.[ShowStart]<=GETDATE() AND FullProj.[ShowEnd]>GETDATE()) OR FullProj.[PRSTDT]<=GETDATE() AND FullProj.[PRENDT]>GETDATE())