CREATE TABLE [dbo].[TB_SPECLock]
(
	[Id] INT NOT NULL IDENTITY, 
    [SPECLockCode] VARCHAR(10) NOT NULL DEFAULT '', 
    [SPECLockName] NVARCHAR(50) NOT NULL DEFAULT N'', 
    [use_flag] [tinyint] NOT NULL DEFAULT 2,
    [last_Opt] NVARCHAR(10) NOT NULL DEFAULT N'',
    [MKTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [UPDTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    CONSTRAINT [PK_TB_SPECLock] PRIMARY KEY ([Id]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'特殊身份代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SPECLock',
    @level2type = N'COLUMN',
    @level2name = N'SPECLockCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'特殊身份名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SPECLock',
    @level2type = N'COLUMN',
    @level2name = N'SPECLockName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最後更新者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SPECLock',
    @level2type = N'COLUMN',
    @level2name = N'last_Opt'
    GO
    EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否啟用(0:停用;1:啟用;)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SPECLock',
    @level2type = N'COLUMN',
    @level2name = N'use_flag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SPECLock',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最近一次修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SPECLock',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO