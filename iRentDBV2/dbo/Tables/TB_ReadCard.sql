CREATE TABLE [dbo].[TB_ReadCard]
(
	[ReadCardID] BIGINT NOT NULL IDENTITY, 
    [CID] VARCHAR(20) NOT NULL DEFAULT '', 
    [CardNo] VARCHAR(50) NOT NULL DEFAULT '', 
	[Status] VARCHAR(10) NOT NULL DEFAULT '',
    [GPSTime] DATETIME NOT NULL, 
    [ReadTime] DATETIME NOT NULL  DEFAULT DATEADD(HOUR,8,GETDATE()),
    CONSTRAINT [PK_TB_ReadCard] PRIMARY KEY ([ReadCardID]) 
)

GO

CREATE UNIQUE  INDEX [IX_TB_ReadCard_Search] ON [dbo].[TB_ReadCard] ([CID], [GPSTime])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車機編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReadCard',
    @level2type = N'COLUMN',
    @level2name = N'CID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'卡號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReadCard',
    @level2type = N'COLUMN',
    @level2name = N'CardNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'狀態，只有遠傳的會有值',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReadCard',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'刷卡的GPS時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReadCard',
    @level2type = N'COLUMN',
    @level2name = N'GPSTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'寫入時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReadCard',
    @level2type = N'COLUMN',
    @level2name = 'ReadTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'讀卡記錄',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReadCard',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE UNIQUE INDEX [IX_TB_ReadCard_SearchByReadTime] ON [dbo].[TB_ReadCard] ([CID], [CardNo], [ReadTime])
