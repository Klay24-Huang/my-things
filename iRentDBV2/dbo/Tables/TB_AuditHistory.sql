CREATE TABLE [dbo].[TB_AuditHistory]
(
	[AuditHistoryID] BIGINT NOT NULL IDENTITY, 
    [IDNO]      VARCHAR(10) NOT NULL DEFAULT '',
    [AuditUser] NVARCHAR(10) NOT NULL DEFAULT N'', 
    [AuditDate] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [HandleItem] TINYINT NOT NULL DEFAULT 0, 
    [HandleType] TINYINT NOT NULL DEFAULT 0, 
    [IsReject] TINYINT NOT NULL DEFAULT 0,
    [RejectReason] NVARCHAR(1024) NOT NULL DEFAULT N'', 
    [RejectExplain] NVARCHAR(1024) NOT NULL DEFAULT N'', 
     [MKTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    CONSTRAINT [PK_TB_AuditHistory] PRIMARY KEY ([AuditHistoryID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'身份證',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AuditHistory',
    @level2type = N'COLUMN',
    @level2name = N'IDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'審核歷程',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AuditHistory',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'審核者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AuditHistory',
    @level2type = N'COLUMN',
    @level2name = N'AuditUser'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'審核日期',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AuditHistory',
    @level2type = N'COLUMN',
    @level2name = N'AuditDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'處理事項',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AuditHistory',
    @level2type = N'COLUMN',
    @level2name = N'HandleItem'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'處理區分',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AuditHistory',
    @level2type = N'COLUMN',
    @level2name = N'HandleType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'審核結果：0:通過;1:不通過',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AuditHistory',
    @level2type = N'COLUMN',
    @level2name = N'IsReject'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'不通過原因',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AuditHistory',
    @level2type = N'COLUMN',
    @level2name = N'RejectReason'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'不通過說明',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AuditHistory',
    @level2type = N'COLUMN',
    @level2name = N'RejectExplain'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AuditHistory',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO

CREATE INDEX [IX_TB_AuditHistory_Search] ON [dbo].[TB_AuditHistory] ([IDNO])
