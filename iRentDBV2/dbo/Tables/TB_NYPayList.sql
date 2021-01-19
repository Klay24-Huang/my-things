CREATE TABLE [dbo].[TB_NYPayList]
(
	[order_number] INT NOT NULL PRIMARY KEY, 
	[PAYDATE] VARCHAR(8) NOT NULL DEFAULT(''),
	[PAYAMT] INT NOT NULL DEFAULT(0),
	[NORDNO] VARCHAR(50) NOT NULL DEFAULT(''),
    [MKTime] DATETIME NULL,
	[UPDTime] DATETIME NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'付費日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYPayList',
    @level2type = N'COLUMN',
    @level2name = N'PAYDATE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'付費金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYPayList',
    @level2type = N'COLUMN',
    @level2name = N'PAYAMT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'網刷編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYPayList',
    @level2type = N'COLUMN',
    @level2name = N'NORDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYPayList',
    @level2type = N'COLUMN',
    @level2name = N'order_number'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYPayList',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYPayList',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'