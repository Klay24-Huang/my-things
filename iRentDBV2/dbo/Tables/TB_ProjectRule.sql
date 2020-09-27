CREATE TABLE [dbo].[TB_ProjectRule]
(
	[ProjectRuleID] INT NOT NULL IDENTITY, 
    [ProjID] VARCHAR(10) NOT NULL DEFAULT '', 
    [AllCarType] TINYINT NOT NULL DEFAULT 0, 
    [ProjectRuleOfCarTypeGroup] INT NOT NULL DEFAULT 0, 
    [DefaultCountdown] INT NOT NULL DEFAULT 30, 
    [DefaultStopTime] INT NOT NULL DEFAULT 60, 
	[use_flag] [tinyint] NOT NULL DEFAULT 2,
	[MKTime] [datetime] NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())),
	[UPDTime] [datetime] NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())),
    CONSTRAINT [PK_TB_ProjectRule] PRIMARY KEY ([ProjectRuleID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案規則，設定取車倒數時間…等',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRule',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRule',
    @level2type = N'COLUMN',
    @level2name = N'ProjID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否專案全車型適用(0:否;1:是)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRule',
    @level2type = N'COLUMN',
    @level2name = N'AllCarType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'適用車型群組（當AllCarType=0時，以此欄位值對應TB_ProjectRuleOfCarTypeGroup PK)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRule',
    @level2type = N'COLUMN',
    @level2name = N'ProjectRuleOfCarTypeGroup'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否啟用(0:停用;1:啟用;2:僅測試身份可視)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRule',
    @level2type = N'COLUMN',
    @level2name = N'use_flag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRule',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最近一次修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRule',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO

CREATE INDEX [IX_TB_ProjectRule_Search] ON [dbo].[TB_ProjectRule] ([AllCarType], [ProjID], [use_flag])

GO

CREATE INDEX [IX_TB_ProjectRule_SearchOfGroup] ON [dbo].[TB_ProjectRule] ([ProjID], [use_flag], [ProjectRuleOfCarTypeGroup])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預設倒數時間（分）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRule',
    @level2type = N'COLUMN',
    @level2name = N'DefaultCountdown'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預設還車時間（分）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRule',
    @level2type = N'COLUMN',
    @level2name = N'DefaultStopTime'