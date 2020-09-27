CREATE TABLE [dbo].[TB_ProjectRuleOfCarTypeGroup]
(
	[ProjectRuleOfCarTypeGroupID] INT NOT NULL IDENTITY, 
    [ProjectRuleOfCarTypeGroupName] NVARCHAR(50) NOT NULL DEFAULT '',
	[DefaultCountdown] INT NOT NULL DEFAULT 30, 
    [DefaultStopTime] INT NOT NULL DEFAULT 60, 
	[use_flag] [tinyint] NOT NULL DEFAULT 2,
	[MKTime] [datetime] NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())),
	[UPDTime] [datetime] NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())), 
    CONSTRAINT [PK_TB_ProjectRuleOfCarTypeGroup] PRIMARY KEY ([ProjectRuleOfCarTypeGroupID]),
)

GO

CREATE INDEX [IX_TB_ProjectRuleOfCarTypeGroup_Search] ON [dbo].[TB_ProjectRuleOfCarTypeGroup] ([ProjectRuleOfCarTypeGroupID], [use_flag])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案控制群組',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRuleOfCarTypeGroup',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'群組名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRuleOfCarTypeGroup',
    @level2type = N'COLUMN',
    @level2name = N'ProjectRuleOfCarTypeGroupName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預設倒數時間(分)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRuleOfCarTypeGroup',
    @level2type = N'COLUMN',
    @level2name = N'DefaultCountdown'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預設還車時間（分）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRuleOfCarTypeGroup',
    @level2type = N'COLUMN',
    @level2name = N'DefaultStopTime'
	GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否啟用(0:停用;1:啟用;2:僅測試身份可視)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRuleOfCarTypeGroup',
    @level2type = N'COLUMN',
    @level2name = N'use_flag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRuleOfCarTypeGroup',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最近一次修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRuleOfCarTypeGroup',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO