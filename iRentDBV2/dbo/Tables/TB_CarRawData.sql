﻿CREATE TABLE [dbo].[TB_CarRawData]
(
	[RawDataID] BIGINT NOT NULL IDENTITY ,
	 [CarNo] VARCHAR(10) NOT NULL DEFAULT '',
	[CID] VARCHAR(10) NOT NULL DEFAULT '', 
	[Token] VARCHAR(512) NOT NULL DEFAULT'',
	[deviceType] INT NOT NULL DEFAULT 0,
    [ACCStatus] INT NULL, 
    [GPSStatus] INT NULL, 
    [GPSTime] DATETIME NULL, 
    [OBDStatus] INT NULL, 
    [GPRSStatus] INT NULL, 
    [PowerOnStatus] INT NULL, 
    [CentralLockStatus] INT NULL, 
    [DoorStatus] VARCHAR(10) NULL, 
    [LockStatus] VARCHAR(10) NULL, 
    [IndoorLightStatus] INT NULL, 
    [SecurityStatus] INT NULL, 
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
	[deviceBLE_Login]  INT NULL,
	[deviceBLE_BroadCast] INT NULL,
	[devicePwr_Mode]   INT NULL,
	[deviceReversing]  INT NULL,
	[devicePut_Down]   INT NULL,
	[devicePwr_Relay]  INT NULL,
	[deviceStart_OK]   INT NULL,
	[deviceHard_ACC]   INT NULL,
	[deviceEMG_Break]  INT NULL,
	[deviceSharp_Turn] INT NULL,
	[deviceBat_Cover]  INT NULL,
    [deviceLowVoltage] INT NULL,
	[extDeviceStatus1] INT NULL,
	[extDeviceStatus2] INT NULL,
	[extDeviceData1]   BIGINT NULL,
	[extDeviceData2]   BIGINT NULL,
	[extDeviceData3]   VARCHAR(50) NULL DEFAULT '',
	[extDeviceData4]   VARCHAR(1024) NULL,
	[extDeviceData5]   VARCHAR(128) NULL,
	[extDeviceData6]   VARCHAR(256) NULL,
	[extDeviceData7]   VARCHAR(512) NULL, 
	[deviceName]	   VARCHAR(50) NULL,
		[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
    CONSTRAINT [PK_TB_CarRawData] PRIMARY KEY ([RawDataID]),
)
