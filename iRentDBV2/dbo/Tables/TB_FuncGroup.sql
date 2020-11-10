CREATE TABLE [dbo].[TB_FuncGroup]
(
    [SEQNO]       INT NOT NULL IDENTITY,
	[FuncGroupID] VARCHAR(50) NOT NULL, 
    [FuncGroupName] NVARCHAR(50) NOT NULL DEFAULT N'', 
    [SD] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [ED] DATETIME NOT NULL DEFAULT '2099-12-31 23:59:59', 
    [A_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [A_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    [U_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [U_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    CONSTRAINT [PK_TB_FuncGroup] PRIMARY KEY (SEQNO) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能群組',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroup',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能群組編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroup',
    @level2type = N'COLUMN',
    @level2name = N'FuncGroupID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能群組名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroup',
    @level2type = N'COLUMN',
    @level2name = N'FuncGroupName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'起日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroup',
    @level2type = N'COLUMN',
    @level2name = N'SD'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'迄日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroup',
    @level2type = N'COLUMN',
    @level2name = N'ED'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroup',
    @level2type = N'COLUMN',
    @level2name = N'A_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroup',
    @level2type = N'COLUMN',
    @level2name = N'U_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroup',
    @level2type = N'COLUMN',
    @level2name = N'A_SYSDT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroup',
    @level2type = N'COLUMN',
    @level2name = N'U_SYSDT'
GO

CREATE INDEX [IX_TB_FuncGroup_Search] ON [dbo].[TB_FuncGroup] ([ED], [SD], [FuncGroupID], [FuncGroupName])
