CREATE TABLE [dbo].[TB_ParkingSpace]
(
	[OrderNo] BIGINT NOT NULL, 
    [SEQNO] [tinyint] NOT NULL DEFAULT 0,
    [ParkingImage] VARCHAR(MAX) NOT NULL DEFAULT '', 
    [ParkingSpace] NVARCHAR(256) NOT NULL DEFAULT N'' ,
    [HasUpload] TINYINT NOT NULL DEFAULT 0,
    [MKTime] DATETIME NULL DEFAULT(DATEADD(HOUR,8,GETDATE())),
    [UPDTime] DATETIME NULL, 
    CONSTRAINT [PK_TB_ParkingSpace] PRIMARY KEY ([OrderNo]),
)

GO

CREATE INDEX [IX_TB_ParkingSpace_Search] ON [dbo].[TB_ParkingSpace] ([HasUpload])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'照片檔，未上傳storage時是base64，上傳後為storage檔名',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ParkingSpace',
    @level2type = N'COLUMN',
    @level2name = N'ParkingImage'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'停車位置',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ParkingSpace',
    @level2type = N'COLUMN',
    @level2name = N'ParkingSpace'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否上傳，0:否;1:是',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ParkingSpace',
    @level2type = N'COLUMN',
    @level2name = N'HasUpload'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'停車位置',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ParkingSpace',
    @level2type = NULL,
    @level2name = NULL