CREATE TABLE [dbo].[TB_NYRefund]
(
	[order_number] INT NOT NULL PRIMARY KEY, 
    [IDNO] VARCHAR(10) NOT NULL DEFAULT (''), 
    [DiffDay] INT NOT NULL DEFAULT 0, 
    [ChkFLG] VARCHAR(1) NOT NULL DEFAULT(''), 
    [ChkTime] DATETIME NULL,
    [PayFLG] VARCHAR(1) NOT NULL DEFAULT(''),
    [OrderAmt] [int] NOT NULL,
    [RefundAmt] INT NOT NULL DEFAULT 0, 
    [MKTime] DATETIME NULL, 
    [UPDTime] DATETIME NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'退款金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYRefund',
    @level2type = N'COLUMN',
    @level2name = N'RefundAmt'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYRefund',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYRefund',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYRefund',
    @level2type = N'COLUMN',
    @level2name = N'order_number'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'會員編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYRefund',
    @level2type = N'COLUMN',
    @level2name = N'IDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'提早取消天數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYRefund',
    @level2type = N'COLUMN',
    @level2name = N'DiffDay'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否付款',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYRefund',
    @level2type = N'COLUMN',
    @level2name = N'PayFLG'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排程是否確認',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYRefund',
    @level2type = N'COLUMN',
    @level2name = N'ChkFLG'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂金',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NYRefund',
    @level2type = N'COLUMN',
    @level2name = N'OrderAmt'