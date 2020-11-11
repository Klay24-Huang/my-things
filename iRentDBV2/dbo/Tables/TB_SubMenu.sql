CREATE TABLE [dbo].[TB_SubMenu]
(
	[SubMenuID] INT NOT NULL IDENTITY, 
    [SubMenuCode] NVARCHAR(50) NOT NULL DEFAULT N'', 
    [SubMenuName] NVARCHAR(50) NOT NULL DEFAULT N'', 
    [MenuController] VARCHAR(100) NOT NULL DEFAULT '', 
    [MenuAction] VARCHAR(100) NOT NULL DEFAULT '', 
    [RootMenuID] INT NOT NULL DEFAULT 0, 
    [isNewWindow] TINYINT NOT NULL DEFAULT 0,
    [OperationPowerGroupId] int not null default 1,
    [Sort] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_TB_SubMenu] PRIMARY KEY ([SubMenuID]) 
)

GO

CREATE INDEX [IX_TB_SubMenu_Search] ON [dbo].[TB_SubMenu] ([RootMenuID], [Sort])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'子選單',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SubMenu',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'子選單代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SubMenu',
    @level2type = N'COLUMN',
    @level2name = N'SubMenuCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'子選單名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SubMenu',
    @level2type = N'COLUMN',
    @level2name = N'SubMenuName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'子選單Controller，對應mvc controller',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SubMenu',
    @level2type = N'COLUMN',
    @level2name = N'MenuController'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'子選單action，對應mvc action',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SubMenu',
    @level2type = N'COLUMN',
    @level2name = N'MenuAction'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'父選單id，TB_MenuRoot PK',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SubMenu',
    @level2type = N'COLUMN',
    @level2name = N'RootMenuID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否開新視窗(0:否;1:是;2:內頁)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SubMenu',
    @level2type = N'COLUMN',
    @level2name = N'isNewWindow'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排序，升冪',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SubMenu',
    @level2type = N'COLUMN',
    @level2name = N'Sort'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能群組',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SubMenu',
    @level2type = N'COLUMN',
    @level2name = N'OperationPowerGroupId'