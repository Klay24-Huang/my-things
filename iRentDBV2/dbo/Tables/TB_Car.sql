﻿CREATE TABLE [dbo].[TB_Car]
(
	[CarNo] [varchar](10) NOT NULL DEFAULT '',
	[TSEQNO] [varchar](32) NOT NULL DEFAULT '',
	[StationID] [varchar](10) NOT NULL DEFAULT '',
	[nowStationID] [varchar](10) NOT NULL DEFAULT '',
	[CarType] [VARCHAR](10) NOT NULL DEFAULT '',
	[available] [tinyint] NOT NULL DEFAULT 2,
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_Car] PRIMARY KEY ([CarNo]),
)

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車號，pk' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Car', @level2type=N'COLUMN',@level2name=N'CarNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'和運車輛ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Car', @level2type=N'COLUMN',@level2name=N'TSEQNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所屬站點' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Car', @level2type=N'COLUMN',@level2name=N'StationID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'目前站點' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Car', @level2type=N'COLUMN',@level2name=N'nowStationID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車型代碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Car', @level2type=N'COLUMN',@level2name=N'CarType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'目前狀態：0:出租中;1:可出租;2:未上線' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Car', @level2type=N'COLUMN',@level2name=N'available'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Car', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Car', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO
CREATE INDEX [IX_TB_Car_Search] ON [dbo].[TB_Car] ([nowStationID], [available], [StationID], [CarType])
GO
CREATE NONCLUSTERED INDEX IX_GetAnyRentSearch ON [dbo].[TB_Car] ([available]) INCLUDE ([nowStationID])
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車輛總表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Car',
    @level2type = NULL,
    @level2name = NULL