CREATE TABLE [dbo].[TB_OperationPower]
(
	[OperationPowerID] INT NOT NULL IDENTITY, 
    [OperationPowerCode] VARCHAR(50) NOT NULL DEFAULT '',
    [OperationPowerName] NVARCHAR(50) NOT NULL DEFAULT N'', 
    [use_flag] TINYINT NOT NULL, 
    [A_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [A_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    [U_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [U_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    CONSTRAINT [PK_TB_OperationPower] PRIMARY KEY ([OperationPowerID]) 
)
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否啟用：0:停用;1:啟用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPower',
    @level2type = N'COLUMN',
    @level2name = N'use_flag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPower',
    @level2type = N'COLUMN',
    @level2name = N'A_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPower',
    @level2type = N'COLUMN',
    @level2name = N'A_SYSDT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPower',
    @level2type = N'COLUMN',
    @level2name = N'U_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPower',
    @level2type = N'COLUMN',
    @level2name = N'U_SYSDT'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作功能名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPower',
    @level2type = N'COLUMN',
    @level2name = N'OperationPowerName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作功能列表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPower',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPower',
    @level2type = N'COLUMN',
    @level2name = N'OperationPowerCode'
GO

CREATE INDEX [IX_TB_OperationPower_Search] ON [dbo].[TB_OperationPower] ([use_flag])
