﻿CREATE TABLE [dbo].[TB_FuncGroupList]
(
	[FSEQNO] INT NOT NULL IDENTITY, 
    [FuncGroupID] INT NOT NULL DEFAULT 0, 
    [FuncID] VARCHAR(50) NOT NULL DEFAULT '', 
    [FuncName] NVARCHAR(50) NOT NULL DEFAULT N'', 
       [SD] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [ED] DATETIME NOT NULL DEFAULT '2099-12-31 23:59:59', 
    [A_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [A_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    [U_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [U_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    CONSTRAINT [PK_TB_FuncGroupList] PRIMARY KEY ([FSEQNO]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能群組pk',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupList',
    @level2type = N'COLUMN',
    @level2name = N'FuncGroupID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupList',
    @level2type = N'COLUMN',
    @level2name = N'FuncID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupList',
    @level2type = N'COLUMN',
    @level2name = N'FuncName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'起日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupList',
    @level2type = N'COLUMN',
    @level2name = N'SD'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'迄日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupList',
    @level2type = N'COLUMN',
    @level2name = N'ED'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能列表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FuncGroupList',
    @level2type = NULL,
    @level2name = NULL