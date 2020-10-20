CREATE TABLE [dbo].[TB_MasterCard]
(
	[MasterCardID] INT NOT NULL IDENTITY, 
    [ManagerId] NVARCHAR(10) NOT NULL DEFAULT N'', 
    [CardNo] VARCHAR(20) NOT NULL DEFAULT '',
    [CarNo] VARCHAR(20) NOT NULL DEFAULT '',
    [CID] VARCHAR(10) NOT NULL DEFAULT '',
    [last_Opt] [NVARCHAR](10) NOT NULL DEFAULT 'SYS',
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_MasterCard] PRIMARY KEY ([MasterCardID]) 
)

GO

CREATE INDEX [IX_TB_MasterCard_Search] ON [dbo].[TB_MasterCard] ([ManagerId], [CID], [CardNo])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'萬用卡',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MasterCard',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'員工編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MasterCard',
    @level2type = N'COLUMN',
    @level2name = N'ManagerId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'卡號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MasterCard',
    @level2type = N'COLUMN',
    @level2name = N'CardNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車機編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MasterCard',
    @level2type = N'COLUMN',
    @level2name = N'CID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MasterCard',
    @level2type = N'COLUMN',
    @level2name = N'CarNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MasterCard',
    @level2type = N'COLUMN',
    @level2name = N'last_Opt'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MasterCard',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最後更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MasterCard',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'