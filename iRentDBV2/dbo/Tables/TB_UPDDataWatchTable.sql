CREATE TABLE [dbo].[TB_UPDDataWatchTable]
(
	[Id] INT NOT NULL  IDENTITY, 
    [AreaList] DATETIME NULL, 
    [LoveCode] DATETIME NULL, 
    [NormalRent] DATETIME NULL, 
    [Polygon] DATETIME NULL,
	[Parking] DATETIME NULL,
	 [MKTime]    DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
	  [UPDTime]    DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL, 
    CONSTRAINT [PK_TB_UPDDataWatchTable] PRIMARY KEY ([Id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'資料更新列表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UPDDataWatchTable',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'PK',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UPDDataWatchTable',
    @level2type = N'COLUMN',
    @level2name = N'Id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'行政區',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UPDDataWatchTable',
    @level2type = N'COLUMN',
    @level2name = N'AreaList'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'愛心捐贈碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UPDDataWatchTable',
    @level2type = N'COLUMN',
    @level2name = N'LoveCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同站租還據點',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UPDDataWatchTable',
    @level2type = N'COLUMN',
    @level2name = N'NormalRent'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'電子柵欄',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UPDDataWatchTable',
    @level2type = N'COLUMN',
    @level2name = N'Polygon'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UPDDataWatchTable',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最近一次更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UPDDataWatchTable',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'停車場最近更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UPDDataWatchTable',
    @level2type = N'COLUMN',
    @level2name = N'Parking'