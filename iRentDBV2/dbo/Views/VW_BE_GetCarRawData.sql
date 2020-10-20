CREATE VIEW [dbo].[VW_BE_GetCarRawData]
	AS 
	SELECT [deviceType],CarNo ,[CID],ISNULL([ACCStatus],0) AS [ACCStatus]
      ,ISNULL([GPSStatus]		 ,0) AS [GPSStatus]
      ,ISNULL([OBDStatus]		 ,0) AS [OBDStatus]
      ,ISNULL([GPRSStatus]		 ,0) AS [GPRSStatus]
      ,ISNULL([PowerOnStatus]	 ,0) AS [PowerOnStatus]
      ,ISNULL([CentralLockStatus],0) AS [CentralLockStatus]
      ,ISNULL([DoorStatus],'') AS [DoorStatus]
      ,ISNULL([LockStatus],'') AS [LockStatus]
      ,ISNULL([IndoorLightStatus],0) AS	[IndoorLightStatus]
      ,ISNULL([SecurityStatus]	 ,0) AS	[SecurityStatus]	 
      ,ISNULL([deviceBat_Cover]	 ,0) AS	BatCover	 
      ,ISNULL([deviceLowVoltage] ,0) AS MotorLowVol
      ,[MKTime]
  FROM [dbo].[TB_CarRawData]
    GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetCarRawData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetCarRawData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取出車機event', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetCarRawData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetCarRawData';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetCarRawData';