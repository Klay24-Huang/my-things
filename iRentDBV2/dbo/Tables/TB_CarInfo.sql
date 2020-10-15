﻿CREATE TABLE [dbo].[TB_CarInfo]
(
	[CarNo] [VARCHAR](10) NOT NULL DEFAULT '',
	[TSEQNO] [INT] NOT NULL DEFAULT 0,
	[CarType] [VARCHAR](6) NOT NULL DEFAULT '',
	[RentCount] [INT] NOT NULL DEFAULT 0,
	[UncleanCount] [INT] NOT NULL DEFAULT 0,
	[HoildayPrice] [INT] NOT NULL DEFAULT 0,
	[WeekdayPrice] [INT] NOT NULL DEFAULT 0,
	[Seat] [TINYINT] NOT NULL DEFAULT 0,
	[FactoryYear] [VARCHAR](6) NOT NULL DEFAULT '',
	[CarColor] [NVARCHAR](3) NOT NULL DEFAULT N'',
	[EngineNO] [VARCHAR](50) NOT NULL DEFAULT '',
	[BodyNO] [VARCHAR](50) NOT NULL DEFAULT '',
	[CCNum] [INT] NOT NULL DEFAULT 0,
	[CID] [VARCHAR](10) NOT NULL DEFAULT '',
	[deviceToken] [VARCHAR](256) NOT NULL DEFAULT '',
	[IsCens] [TINYINT] NOT NULL DEFAULT 0,
	[IsMotor] [TINYINT] NOT NULL DEFAULT 0,
	[AreaID] [TINYINT] NOT NULL DEFAULT 0,
	[OperationID] INT NOT NULL DEFAULT 0,
	[HoildayPriceByMinutes] [FLOAT] NOT NULL DEFAULT 0.0,
	[WeekdayPriceByMinutes] [FLOAT] NOT NULL DEFAULT 0.0, 
    [Memo] [NVARCHAR](100) NOT NULL DEFAULT '',
    [last_Opt] [NVARCHAR](10) NOT NULL DEFAULT 'SYS',
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_CarInfo] PRIMARY KEY ([CarNo]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車輛主檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarInfo',
    @level2type = NULL,
    @level2name = NULL
	GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車牌號碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'CarNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'和運序號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'TSEQNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車輛類型代碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'CarType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出租次數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'RentCount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'未清潔次數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'UncleanCount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'假日售價' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'HoildayPrice'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'平日價' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'WeekdayPrice'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車輛座位數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'Seat'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出廠年月(YYYYmm)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'FactoryYear'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車子顏色' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'CarColor'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'引擎編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'EngineNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車身編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'BodyNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'cc數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'CCNum'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarInfo', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'加盟者id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarInfo',
    @level2type = N'COLUMN',
    @level2name = N'OperationID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車機編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarInfo',
    @level2type = N'COLUMN',
    @level2name = N'CID'
GO

CREATE INDEX [IX_TB_CarInfo_Search] ON [dbo].[TB_CarInfo] ([CID], [CarType], [OperationID])

GO

CREATE INDEX [IX_TB_CarInfo_assignCar] ON [dbo].[TB_CarInfo] ([RentCount], [CarType])
GO
CREATE NONCLUSTERED INDEX IX_ForSearchAnyData
ON [dbo].[TB_CarInfo] ([CarType])

GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'遠傳車機token',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarInfo',
    @level2type = N'COLUMN',
    @level2name = N'deviceToken'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否為興聯車機(0:否;1:是)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarInfo',
    @level2type = N'COLUMN',
    @level2name = N'IsCens'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否為機車（0:否;1:是)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarInfo',
    @level2type = N'COLUMN',
    @level2name = N'IsMotor'
GO

CREATE INDEX [IX_TB_CarInfo_SearchByMachine] ON [dbo].[TB_CarInfo] ([CarNo], [CID], [deviceToken], [IsCens], [IsMotor])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'備註',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarInfo',
    @level2type = N'COLUMN',
    @level2name = N'Memo'