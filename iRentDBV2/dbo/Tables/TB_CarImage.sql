CREATE TABLE [dbo].[TB_CarImage]
(
	[OrderNo] BIGINT NOT NULL DEFAULT 0, 
    [Mode] TINYINT NOT NULL DEFAULT 0, 
    [ImageType] TINYINT NOT NULL DEFAULT 0, 
    [Image] VARCHAR(MAX) NOT NULL DEFAULT '', 
    [HasUpload] TINYINT NOT NULL DEFAULT 0,
    [MKTime] DATETIME NULL DEFAULT(DATEADD(HOUR,8,GETDATE())),
    [UPDTime] DATETIME NULL,
    CONSTRAINT [PK_TB_CarImage] PRIMARY KEY ([OrderNo], [Mode], [ImageType]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否上傳至storage;0:否;1:是',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarImage',
    @level2type = N'COLUMN',
    @level2name = N'HasUpload'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'圖片base64，上傳後改為此照片檔名',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarImage',
    @level2type = N'COLUMN',
    @level2name = N'Image'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'出還車：0:出車;1:還車',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarImage',
    @level2type = N'COLUMN',
    @level2name = N'Mode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarImage',
    @level2type = N'COLUMN',
    @level2name = N'OrderNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'圖片類型 1:左前;2:右前;3:左後;4:右後;5:電子簽名;6:左前車損;7:右前車損;8:左後車損;9:右後車損',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarImage',
    @level2type = N'COLUMN',
    @level2name = N'ImageType'
GO

CREATE INDEX [IX_TB_CarImage_Search] ON [dbo].[TB_CarImage] ([HasUpload], [OrderNo], [Mode])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarImage',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarImage',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'出還車照',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarImage',
    @level2type = NULL,
    @level2name = NULL