CREATE TABLE [dbo].[TB_tmpFeedBackPIC]
(
	[tmpFeedBackPICID] INT NOT NULL, 
    [OrderNo] BIGINT NOT NULL DEFAULT 0, 
    [SEQNO] TINYINT NOT NULL DEFAULT 0, 
    [FeedBackFile] VARCHAR(MAX) NOT NULL ,
    [MKTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [UPDTime] DATETIME NOT NULL, 
    CONSTRAINT [PK_TB_tmpFeedBackPIC] PRIMARY KEY ([tmpFeedBackPICID]), 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_tmpFeedBackPIC',
    @level2type = N'COLUMN',
    @level2name = N'OrderNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'序號，1~4',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_tmpFeedBackPIC',
    @level2type = N'COLUMN',
    @level2name = N'SEQNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'照片base64',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_tmpFeedBackPIC',
    @level2type = N'COLUMN',
    @level2name = N'FeedBackFile'
GO

CREATE INDEX [IX_TB_tmpFeedBackPIC_Search_OrderNo] ON [dbo].[TB_tmpFeedBackPIC] ([OrderNo])
