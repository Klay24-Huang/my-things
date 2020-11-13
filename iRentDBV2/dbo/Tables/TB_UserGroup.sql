﻿CREATE TABLE [dbo].[TB_UserGroup]
(
	[USEQNO] INT NOT NULL IDENTITY, 
    [UserGroupID] VARCHAR(50) NOT NULL DEFAULT '', 
    [UserGroupName] NVARCHAR(50) NOT NULL DEFAULT N'', 
    [FuncGroupID] INT NOT NULL DEFAULT 0,
    [OperatorID] INT NOT NULL DEFAULT 0,
    [StartDate] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [EndDate] DATETIME NOT NULL DEFAULT '2099-12-31 23:59:59', 
    [A_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [A_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    [U_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [U_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    CONSTRAINT [PK_TB_UserGroup] PRIMARY KEY ([USEQNO]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'使用者群組',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserGroup',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'使用者群組編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserGroup',
    @level2type = N'COLUMN',
    @level2name = N'UserGroupID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'使用者群組名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserGroup',
    @level2type = N'COLUMN',
    @level2name = N'UserGroupName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'起日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserGroup',
    @level2type = N'COLUMN',
    @level2name = 'StartDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'迄日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserGroup',
    @level2type = N'COLUMN',
    @level2name = 'EndDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'業者別 PK',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserGroup',
    @level2type = N'COLUMN',
    @level2name = N'OperatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預設的功能群組id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserGroup',
    @level2type = N'COLUMN',
    @level2name = N'FuncGroupID'