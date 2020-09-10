CREATE TABLE [dbo].[TB_MilageSetting]
(
	[ProjID] VARCHAR(50) NOT NULL, 
    [CarType] VARCHAR(50) NOT NULL DEFAULT '', 
    [SDate] DATETIME NOT NULL, 
    [EDate] DATETIME NOT NULL, 
    [MilageBase] FLOAT NOT NULL DEFAULT 3.0 ,
	 [use_flag] [tinyint] NOT NULL DEFAULT (1),
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_MilageSetting] PRIMARY KEY ([ProjID], [CarType], [SDate],[use_flag],[EDate]), 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MilageSetting',
    @level2type = N'COLUMN',
    @level2name = N'ProjID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MilageSetting',
    @level2type = N'COLUMN',
    @level2name = N'CarType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'起始日期',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MilageSetting',
    @level2type = N'COLUMN',
    @level2name = N'SDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'結束日期',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MilageSetting',
    @level2type = N'COLUMN',
    @level2name = N'EDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'每公里費用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MilageSetting',
    @level2type = N'COLUMN',
    @level2name = N'MilageBase'
			GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MilageSetting', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MilageSetting', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MilageSetting', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'里程費設定檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MilageSetting',
    @level2type = NULL,
    @level2name = NULL