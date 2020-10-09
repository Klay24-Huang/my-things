CREATE TABLE [dbo].[TB_OperationPowerConsist]
(
	[OperationPowerConsistId] INT NOT NULL IDENTITY, 
    [OperationPowerGroupId] INT NOT NULL DEFAULT 0, 
    [OperationPowerID] INT NOT NULL DEFAULT 0, 
    [use_flag] TINYINT NOT NULL, 
    [A_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [A_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    [U_USERID] VARCHAR (10)  DEFAULT 'SYS',
    [U_SYSDT]  DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    CONSTRAINT [PK_TB_OperationPowerConsist] PRIMARY KEY ([OperationPowerConsistId]) 
)

GO

CREATE INDEX [IX_TB_OperationPowerConsist_Search] ON [dbo].[TB_OperationPowerConsist] ([OperationPowerGroupId])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作功能群組代碼，對應TB_OperationPowerGroup PK',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerConsist',
    @level2type = N'COLUMN',
    @level2name = N'OperationPowerGroupId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作功能代碼，對應TB_OperationPower PK',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerConsist',
    @level2type = N'COLUMN',
    @level2name = N'OperationPowerID'
    GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否啟用：0:停用;1:啟用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerConsist',
    @level2type = N'COLUMN',
    @level2name = N'use_flag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerConsist',
    @level2type = N'COLUMN',
    @level2name = N'A_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerConsist',
    @level2type = N'COLUMN',
    @level2name = N'A_SYSDT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerConsist',
    @level2type = N'COLUMN',
    @level2name = N'U_USERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerConsist',
    @level2type = N'COLUMN',
    @level2name = N'U_SYSDT'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能群組包含功能列表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperationPowerConsist',
    @level2type = NULL,
    @level2name = NULL