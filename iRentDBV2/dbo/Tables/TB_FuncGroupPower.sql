CREATE TABLE [dbo].[TB_FuncGroupPower]
(
	[FuncGroupPowerID] INT NOT NULL IDENTITY, 
    [FuncGroupID] INT NOT NULL DEFAULT 0, 
    [FuncGroupPower] VARCHAR(MAX) NOT NULL DEFAULT '', 
     [A_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [A_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    [U_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [U_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    CONSTRAINT [PK_TB_FuncGroupPower] PRIMARY KEY ([FuncGroupPowerID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'PK',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupPower',
    @level2type = N'COLUMN',
    @level2name = N'FuncGroupPowerID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能群組權限',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupPower',
    @level2type = NULL,
    @level2name = NULL
    GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupPower',
    @level2type = N'COLUMN',
    @level2name = N'A_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupPower',
    @level2type = N'COLUMN',
    @level2name = N'U_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupPower',
    @level2type = N'COLUMN',
    @level2name = N'A_SYSDT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupPower',
    @level2type = N'COLUMN',
    @level2name = N'U_SYSDT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'對應TB_FuncGroup的pk（seqno)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupPower',
    @level2type = N'COLUMN',
    @level2name = 'FuncGroupID'
GO

CREATE INDEX [IX_TB_FuncGroupPower_Search] ON [dbo].[TB_FuncGroupPower] ([FuncGroupID])
