CREATE TABLE [dbo].[TB_TaishinWalletPayTransactionLog]
(
	[SEQNO] INT NOT NULL PRIMARY KEY IDENTITY, 
    [GUID] VARCHAR(32) NOT NULL DEFAULT (''), 
	[MerchantId] VARCHAR(16) NOT NULL DEFAULT(''),
	[AccountId] VARCHAR(20) NOT NULL DEFAULT(''),
	[BarCode] VARCHAR(25) NOT NULL DEFAULT(''),
	[POSId] VARCHAR(20) NOT NULL DEFAULT(''),
	[StoreId] VARCHAR(20) NOT NULL DEFAULT(''),
	[StoreTransDate] VARCHAR(14) NOT NULL DEFAULT(''),
	[StoreTransId] VARCHAR(20) NOT NULL DEFAULT(''),
	[TransmittalDate] VARCHAR(8) NOT NULL DEFAULT(''),
	[TransDate] VARCHAR(14) NOT NULL DEFAULT(''),
	[TransId] VARCHAR(30) NOT NULL DEFAULT(''),
	[SourceTransId] VARCHAR(30) NOT NULL DEFAULT(''),
	[TransType] VARCHAR(4) NOT NULL DEFAULT(''),
	[BonusFlag] VARCHAR(1) NOT NULL DEFAULT(''),
	[PriceCustody] VARCHAR(1) NOT NULL DEFAULT(''),
	[SmokeLiqueurFlag] VARCHAR(1) NOT NULL DEFAULT(''),
	[Amount] INT NOT NULL DEFAULT 0, 
	[ActualAmount] INT NOT NULL DEFAULT 0, 
	[Bonus] INT NOT NULL DEFAULT 0, 
	[SourceFrom] VARCHAR NOT NULL DEFAULT(''),
	[AccountingStatus] VARCHAR NOT NULL DEFAULT(''),
	[SmokeAmount] INT NOT NULL DEFAULT 0, 
	[ActualGiftCardAmount] INT NOT NULL DEFAULT 0, 
	[BatchId] VARCHAR(30) NULL DEFAULT (''),
    [TRANFLG] VARCHAR NOT NULL DEFAULT ('N'), 
    [TRANDT] DATETIME NULL, 
    [MKTime] DATETIME NOT NULL
)
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Log流水號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = 'SEQNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'GUID',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'GUID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'商店代號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'MerchantId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'會員虛擬代號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'AccountId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交易條碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'BarCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'POS編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'POSId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'店家編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'StoreId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'店家交易時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'StoreTransDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'商店訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'StoreTransId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'商店營收日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'TransmittalDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交易時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'TransDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'台新訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'TransId'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'原始台新訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'SourceTransId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交易類別',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'TransType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否使用紅利點數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'BonusFlag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'價金保管',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'PriceCustody'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否購買菸酒類商品',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'SmokeLiqueurFlag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'儲值金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'Amount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'實際金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'ActualAmount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'紅利點數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'Bonus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交易來源',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'SourceFrom'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'帳務處理狀態',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'AccountingStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'菸品金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'SmokeAmount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'禮物卡帳戶扣款金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'ActualGiftCardAmount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'批次編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'BatchId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否轉檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'TRANFLG'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'轉檔時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletPayTransactionLog',
    @level2type = N'COLUMN',
    @level2name = N'TRANDT'
GO

