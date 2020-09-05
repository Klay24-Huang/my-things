CREATE TABLE [dbo].[TB_CarStatus]
(
	[CarNo] VARCHAR(10) NOT NULL DEFAULT '', 
    [CID] VARCHAR(10) NOT NULL DEFAULT '', 
	[Token] VARCHAR(512) NOT NULL DEFAULT'',
	[deviceType] TINYINT NOT NULL DEFAULT 0,
    [ACCStatus] TINYINT NULL, 
    [GPSStatus] TINYINT NULL, 
    [GPSTime] DATETIME NULL, 
    [OBDStatus] TINYINT NULL, 
    [GPRSStatus] TINYINT NULL, 
    [PowerOnStatus] TINYINT NULL, 
    [CentralLockStatus] TINYINT NULL, 
    [DoorStatus] VARCHAR(10) NULL, 
    [LockStatus] VARCHAR(10) NULL, 
    [IndoorLightStatus] TINYINT NULL, 
    [SecurityStatus] TINYINT NULL, 
    [Speed] INT NULL, 
    [Volt] FLOAT NULL, 
    [Latitude] DECIMAL(12, 7) NULL, 
    [Longitude] DECIMAL(12, 7) NULL, 
    [Millage] FLOAT NULL, 
	[deviceCourse] FLOAT NULL,
	[deviceRPM] FLOAT NULL,
	[device2TBA] FLOAT NULL,
	[device3TBA] FLOAT NULL,
	[deviceRSOC] VARCHAR(10) NULL,
    [deviceRDistance] VARCHAR(10) NULL,
    [deviceMBA]        FLOAT NULL,
	[deviceMBAA]       FLOAT NULL,
	[deviceMBAT_Hi]    FLOAT NULL,
	[deviceMBAT_Lo]    FLOAT NULL,
	[deviceRBA]        FLOAT NULL,
	[deviceRBAA]       FLOAT NULL,
	[deviceRBAT_Hi]    FLOAT NULL,
	[deviceRBAT_Lo]    FLOAT NULL,
	[deviceLBA]        FLOAT NULL,
	[deviceLBAA]       FLOAT NULL,
	[deviceLBAT_Hi]    FLOAT NULL,
	[deviceLBAT_Lo]    FLOAT NULL,
	[deviceTMP]        FLOAT NULL,
	[deviceCur]        FLOAT NULL,
	[deviceTPS]        FLOAT NULL,
	[deviceVOL]        FLOAT NULL,
	[deviceErr]        FLOAT NULL,
	[deviceALT]        FLOAT NULL,
	[deviceGx]        FLOAT NULL,
	[deviceGy]        FLOAT NULL,
	[deviceGz]        FLOAT NULL,
	[deviceBLE_Login]  TINYINT NULL,
	[deviceBLE_BroadCast] TINYINT NULL,
	[devicePwr_Mode]   TINYINT NULL,
	[deviceReversing]  TINYINT NULL,
	[devicePut_Down]   TINYINT NULL,
	[devicePwr_Relay]  TINYINT NULL,
	[deviceStart_OK]   TINYINT NULL,
	[deviceHard_ACC]   TINYINT NULL,
	[deviceEMG_Break]  TINYINT NULL,
	[deviceSharp_Turn] TINYINT NULL,
	[deviceBat_Cover]  TINYINT NULL,
    [deviceLowVoltage] TINYINT NULL,
	[extDeviceStatus1] TINYINT NULL,
	[extDeviceStatus2] TINYINT NULL,
	[extDeviceData1]   BIGINT NULL,
	[extDeviceData2]   BIGINT NULL,
	[extDeviceData3]   VARCHAR(50) NULL DEFAULT '',
	[extDeviceData4]   VARCHAR(1024) NULL,
	[extDeviceData5]   VARCHAR(128) NULL,
	[extDeviceData6]   VARCHAR(256) NULL,
	[extDeviceData7]   VARCHAR(512) NULL,
    [WriteTime] DATETIME NULL ,
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_CarStatus] PRIMARY KEY ([CarNo], [CID], [Token], [deviceType]),
)

GO

CREATE INDEX [IX_TB_CarStatus_Search] ON [dbo].[TB_CarStatus] ([CarNo], [Longitude], [Latitude], [UPDTime])

GO

CREATE INDEX [IX_TB_CarStatus_SearchOfUpdate] ON [dbo].[TB_CarStatus] ([CID], [GPSTime])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車輛即時狀態總表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'CarNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車機編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'CID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車機token，遠傳提供',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'Token'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'類型：0:汽車;1:機車',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車輛發動狀態，發動為1，熄火為0',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'ACCStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'GPS狀態：有效為1，無效為0',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'GPSStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'GPS定位時間，utc',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'GPSTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'OBD狀態，1上線;0:離線',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'OBDStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'GPRS狀態：上線為1，離線為0',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'GPRSStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'引擎狀態，發動為1，熄火為0',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'PowerOnStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'中控鎖狀態：1為上鎖，0為解鎖',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'CentralLockStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車門狀態：1111關門;0000開門',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'DoorStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'門鎖狀態：1為上鎖，0為解鎖，四個門鎖分別為：駕駛門鎖、副駕駛門、乘客門鎖、後行李箱門鎖',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'LockStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車內燈：1為開啟，0為關閉',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'IndoorLightStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'防盜鎖狀態：1為開啟，0為關閉',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'SecurityStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'速度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'Speed'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'電壓',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'Volt'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'緯度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'Latitude'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'經度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'Longitude'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'里程',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'Millage'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'方向角',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceCourse'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'轉速',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceRPM'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'總電量兩顆平均',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'device2TBA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'總電量三顆平均',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'device3TBA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'總電量',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceRSOC'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預估里程',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceRDistance'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'核心電池',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceMBA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'核心電池電流',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceMBAA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'核心電池最高溫度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceMBAT_Hi'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'核心電池最低溫度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceMBAT_Lo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'右邊電池',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceRBA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'右共享電池電流',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceRBAA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'右共享電池最高溫度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceRBAT_Hi'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'右共享電池最低溫度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceRBAT_Lo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'左共享電池',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceLBA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'左共享電池電流',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceLBAA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'左共享電池最高溫度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceLBAT_Hi'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'左共享電池最低溫度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceLBAT_Lo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'直流測大電容溫度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceTMP'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'馬達端電流',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceCur'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'把手位置',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceTPS'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'直流測電壓',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceVOL'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'現狀故障碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceErr'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'現狀警告碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceALT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'三軸平方和最大值的X軸G 值',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceGx'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'三軸平方和最大值的Y軸G 值',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceGy'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'三軸平方和最大值的Z軸G 值',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceGz'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'使用者連接藍牙模組：1:有;0:否',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceBLE_Login'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'藍牙廣播：1:有;0:否',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceBLE_BroadCast'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Power Mode，1:有;0:無',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'devicePwr_Mode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'倒車檔狀態',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceReversing'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'腳架狀態',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'devicePut_Down'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Power Relay',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'devicePwr_Relay'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'馬達',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceStart_OK'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'急加速',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceHard_ACC'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'急煞',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceEMG_Break'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'急轉彎',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceSharp_Turn'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'電池蓋',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceBat_Cover'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'低電壓警示（機車專用）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'deviceLowVoltage'
GO

CREATE INDEX [IX_TB_CarStatus_SearchPower] ON [dbo].[TB_CarStatus] ([deviceLowVoltage], [Volt], [deviceType])

GO

CREATE NONCLUSTERED INDEX IX_Search_MotoAnyRent
ON [dbo].[TB_CarStatus] ([GPSTime])
INCLUDE ([ACCStatus],[GPSStatus],[GPRSStatus],[Speed],[Volt],[Latitude],[Longitude],[Millage],[deviceCourse],[deviceRPM],[device2TBA],[device3TBA],[deviceRSOC],[deviceRDistance],[deviceMBA],[deviceMBAA],[deviceMBAT_Hi],[deviceMBAT_Lo],[deviceRBA],[deviceRBAA],[deviceRBAT_Hi],[deviceRBAT_Lo],[deviceLBA],[deviceLBAA],[deviceLBAT_Hi],[deviceLBAT_Lo],[deviceTMP],[deviceCur],[deviceTPS],[deviceVOL],[deviceErr],[deviceALT],[deviceGx],[deviceGy],[deviceGz],[deviceBLE_Login],[deviceBLE_BroadCast],[devicePwr_Mode],[deviceReversing],[devicePut_Down],[devicePwr_Relay],[deviceStart_OK],[deviceHard_ACC],[deviceEMG_Break],[deviceSharp_Turn],[deviceBat_Cover],[extDeviceStatus1],[extDeviceData5],[extDeviceData6])
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'租約狀態',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'extDeviceStatus1'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汽車專用，iButton扣壓，是為1，否為0',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'extDeviceStatus2'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Log ID',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'extDeviceData1'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'GCP ID',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'extDeviceData2'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汽車專用，iButton編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'extDeviceData3'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汽車專用，顧客卡號，若有多個卡號，請用英文逗點分隔 (e.g. “1234567890,2468013579”)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'extDeviceData4'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機車專用，租約成立時，回傳藍芽廣播名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'extDeviceData5'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機車專用，租約成立時，回傳藍芽廣播密碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'extDeviceData6'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'刷卡機回傳資訊， “OK或NG,卡號”',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarStatus',
    @level2type = N'COLUMN',
    @level2name = N'extDeviceData7'