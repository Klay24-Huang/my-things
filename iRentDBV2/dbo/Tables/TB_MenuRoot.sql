CREATE TABLE [dbo].[TB_MenuRoot]
(
	[MenuId] INT NOT NULL IDENTITY, 
    [MenuName] NVARCHAR(20) NOT NULL, 
    [use_flag] TINYINT NOT NULL, 
    [Sort] INT NOT NULL DEFAULT(0),
    [A_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [A_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    [U_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [U_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    CONSTRAINT [PK_TB_MenuRoot] PRIMARY KEY ([MenuId]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能列',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MenuRoot',
    @level2type = N'COLUMN',
    @level2name = N'MenuId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否啟用：0:停用;1:啟用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MenuRoot',
    @level2type = N'COLUMN',
    @level2name = N'use_flag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MenuRoot',
    @level2type = N'COLUMN',
    @level2name = N'A_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MenuRoot',
    @level2type = N'COLUMN',
    @level2name = N'A_SYSDT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MenuRoot',
    @level2type = N'COLUMN',
    @level2name = N'U_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MenuRoot',
    @level2type = N'COLUMN',
    @level2name = N'U_SYSDT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Menu頁',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MenuRoot',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排序，數字越小越左邊',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MenuRoot',
    @level2type = N'COLUMN',
    @level2name = N'Sort'