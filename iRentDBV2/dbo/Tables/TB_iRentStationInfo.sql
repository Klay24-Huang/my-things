CREATE TABLE [dbo].[TB_iRentStationInfo]
(
	[iRentStationInfoID] INT NOT NULL IDENTITY, 
    [StationID] VARCHAR(10) NOT NULL DEFAULT '',
    [StationPic] VARCHAR(MAX) NOT NULL DEFAULT '', 
    [PicDescription] NVARCHAR(400) NOT NULL DEFAULT N'', 
    [use_flag] [tinyint] NOT NULL DEFAULT (1),
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_iRentStationInfo] PRIMARY KEY ([iRentStationInfoID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'存放據點照片及該照片的簡介',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStationInfo',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE INDEX [IX_TB_iRentStationInfo_Search] ON [dbo].[TB_iRentStationInfo] ([StationID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'據點代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStationInfo',
    @level2type = N'COLUMN',
    @level2name = N'StationID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'據點照片',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStationInfo',
    @level2type = N'COLUMN',
    @level2name = N'StationPic'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'據點說明',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStationInfo',
    @level2type = N'COLUMN',
    @level2name = N'PicDescription'
GO
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_iRentStationInfo', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_iRentStationInfo', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_iRentStationInfo', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO