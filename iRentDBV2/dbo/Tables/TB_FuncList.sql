CREATE TABLE [dbo].[TB_FuncList]
(
	[FuncID] INT NOT NULL IDENTITY, 
    [MenuId] INT NOT NULL DEFAULT 0, 
    [FuncName] NVARCHAR(150) NOT NULL DEFAULT N'', 
    [FuncController] VARCHAR(100) NOT NULL DEFAULT '',
    [FuncAction] VARCHAR(100) NOT NULL DEFAULT '',
    [IsNewWindow] TINYINT NOT NULL DEFAULT 0,
    [OperationPowerGroup] INT NOT NULL DEFAULT 0, 
      [Sort] INT NOT NULL DEFAULT(0),
      [BrevityCode] VARCHAR(10) NOT NULL DEFAULT '',
    [use_flag] TINYINT NOT NULL, 
     [A_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [A_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    [U_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [U_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    CONSTRAINT [PK_TB_FuncList] PRIMARY KEY ([FuncID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能列表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'關連TB_MenuRoot PK',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'MenuId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'FuncName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作功能群組',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'OperationPowerGroup'
    GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否啟用：0:停用;1:啟用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'use_flag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'A_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'A_SYSDT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'U_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'U_SYSDT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排序，數字越小，越上方',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'Sort'
GO

CREATE INDEX [IX_TB_FuncList_Search] ON [dbo].[TB_FuncList] ([use_flag], [MenuId])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能連結的Controller',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'FuncController'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Func連結的Action',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'FuncAction'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否開新視窗：0:否;1:是',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncList',
    @level2type = N'COLUMN',
    @level2name = N'IsNewWindow'