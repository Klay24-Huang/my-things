CREATE TABLE [dbo].[TB_CarTypeImage]
(
	[CarTypeImageID] INT NOT NULL, 
    [CarTypeGroupID] INT NOT NULL DEFAULT 0, 
    [APP] TINYINT NOT NULL DEFAULT 0, 
    [ImageSizeType] INT NOT NULL DEFAULT 0, 
	[use_flag] [TINYINT] NOT NULL DEFAULT (1),
	[MKTime] [DATETIME] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [DATETIME] NULL, 
    CONSTRAINT [PK_TB_CarTypeImage] PRIMARY KEY ([CarTypeImageID]) 
)

GO

CREATE INDEX [IX_TB_CarTypeImage_Search] ON [dbo].[TB_CarTypeImage] ([APP], [CarTypeGroupID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'對應TB_CarTypeGroup PK',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarTypeImage',
    @level2type = N'COLUMN',
    @level2name = N'CarTypeGroupID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'APP類型：0:Android;1:iOS',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarTypeImage',
    @level2type = N'COLUMN',
    @level2name = N'APP'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'尺吋，從0開始，ios只有0~2，android是0~6',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarTypeImage',
    @level2type = N'COLUMN',
    @level2name = N'ImageSizeType'
		GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarTypeImage', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarTypeImage', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarTypeImage', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型圖檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarTypeImage',
    @level2type = NULL,
    @level2name = NULL