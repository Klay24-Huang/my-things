CREATE TABLE [dbo].[TB_OperationPowerGroup]
(
	[OperationPowerGroupId] INT NOT NULL IDENTITY, 
    [OperationPowerName] NVARCHAR(100) NOT NULL DEFAULT N'', 
     [use_flag] TINYINT NOT NULL, 
    [A_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [A_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    [U_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [U_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    CONSTRAINT [PK_TB_OperationPowerGroup] PRIMARY KEY ([OperationPowerGroupId]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能群組名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerGroup',
    @level2type = N'COLUMN',
    @level2name = N'OperationPowerName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否啟用：0:停用;1:啟用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerGroup',
    @level2type = N'COLUMN',
    @level2name = N'use_flag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerGroup',
    @level2type = N'COLUMN',
    @level2name = N'A_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerGroup',
    @level2type = N'COLUMN',
    @level2name = N'A_SYSDT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerGroup',
    @level2type = N'COLUMN',
    @level2name = N'U_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerGroup',
    @level2type = N'COLUMN',
    @level2name = N'U_SYSDT'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能群組',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerGroup',
    @level2type = NULL,
    @level2name = NULL