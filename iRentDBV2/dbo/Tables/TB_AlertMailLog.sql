CREATE TABLE [dbo].[TB_AlertMailLog]
(
	[AlertID] BIGINT NOT NULL, 
    [EventType] INT NOT NULL DEFAULT 0, 
    [Receiver] VARCHAR(1024) NOT NULL DEFAULT '', 
    [Sender] VARCHAR(256) NOT NULL DEFAULT '', 
    [HasSend] TINYINT NOT NULL DEFAULT 0, 
    [MKTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [UPDTime] DATETIME NULL 
)

GO

CREATE INDEX [IX_TB_AlertMailLog_Search] ON [dbo].[TB_AlertMailLog] ([HasSend], [EventType], [MKTime])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'告警信',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AlertMailLog',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'告警類別',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AlertMailLog',
    @level2type = N'COLUMN',
    @level2name = N'EventType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'收件者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AlertMailLog',
    @level2type = N'COLUMN',
    @level2name = N'Receiver'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'寄件者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AlertMailLog',
    @level2type = N'COLUMN',
    @level2name = N'Sender'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否發送：0:否;1:是;2:失敗',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AlertMailLog',
    @level2type = N'COLUMN',
    @level2name = N'HasSend'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AlertMailLog',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AlertMailLog',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'