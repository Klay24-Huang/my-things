CREATE TABLE [dbo].[TB_CardSettingLog]
(
	[CardSettingLogId] BIGINT NOT NULL IDENTITY, 
    [CarNo] VARCHAR(10) NOT NULL DEFAULT '', 
    [CID] VARCHAR(10) NOT NULL DEFAULT '', 
    [Mode] TINYINT NOT NULL DEFAULT 0, 
    [CardType] TINYINT NOT NULL DEFAULT 0, 
    [CardNo] VARCHAR(1100) NOT NULL DEFAULT '', 
    [IsSuccess] TINYINT NOT NULL DEFAULT 0,
    [MKTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    CONSTRAINT [PK_TB_CardSettingLog] PRIMARY KEY ([CardSettingLogId]) 
)

GO

CREATE INDEX [IX_TB_CardSettingLog_Search] ON [dbo].[TB_CardSettingLog] ([CID], [CarNo])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'卡號發送log',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CardSettingLog',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'當下的車號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CardSettingLog',
    @level2type = N'COLUMN',
    @level2name = N'CarNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車機編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CardSettingLog',
    @level2type = N'COLUMN',
    @level2name = N'CID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'模式：0:清空;1:寫入',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CardSettingLog',
    @level2type = N'COLUMN',
    @level2name = N'Mode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'卡別：0:一般;1:萬用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CardSettingLog',
    @level2type = N'COLUMN',
    @level2name = N'CardType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'卡號，以，分隔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CardSettingLog',
    @level2type = N'COLUMN',
    @level2name = N'CardNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車機回傳是否成功：0:失敗;1:成功',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CardSettingLog',
    @level2type = N'COLUMN',
    @level2name = N'IsSuccess'