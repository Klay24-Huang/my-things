CREATE TABLE [dbo].[TB_TaishinWalletStoreValueLog]
(
	[SEQNO] INT NOT NULL PRIMARY KEY IDENTITY, 
    [GUID] VARCHAR(32) NOT NULL DEFAULT (''), 
    [MerchantId] VARCHAR(16) NOT NULL DEFAULT (''), 
    [AccountId] VARCHAR(20) NOT NULL DEFAULT (''), 
    [POSId] VARCHAR(20) NOT NULL DEFAULT (''), 
    [StoreId] VARCHAR(20) NOT NULL DEFAULT (''), 
    [StoreTransDate] VARCHAR(14) NOT NULL DEFAULT (''), 
    [StoreTransId] VARCHAR(20) NOT NULL DEFAULT (''), 
    [TransmittalDate] VARCHAR(8) NOT NULL DEFAULT (''), 
    [TransDate] VARCHAR(14) NOT NULL DEFAULT (''), 
    [TransId] VARCHAR(30) NOT NULL DEFAULT (''), 
    [SourceTransId] VARCHAR(30) NOT NULL DEFAULT (''), 
    [TransType] VARCHAR(4) NOT NULL DEFAULT (''), 
    [AmountType] VARCHAR NOT NULL DEFAULT (''), 
    [Amount] INT NOT NULL DEFAULT 0, 
    [Bonus] INT NOT NULL DEFAULT 0, 
    [BonusExpiredate] VARCHAR(8) NOT NULL DEFAULT (''), 
    [BarCode] VARCHAR(20) NOT NULL DEFAULT (''), 
    [StoreValueReleaseDate] VARCHAR(8) NOT NULL DEFAULT (''), 
    [StoreValueExpireDate] VARCHAR(8) NOT NULL DEFAULT (''), 
    [SourceFrom] VARCHAR NOT NULL DEFAULT (''), 
    [AccountingStatus] VARCHAR NOT NULL DEFAULT (''), 
    [GiftCardBarCode] VARCHAR(16) NOT NULL DEFAULT (''), 
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
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = 'SEQNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'GUID',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'GUID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'商店代號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'MerchantId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'會員虛擬代號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'AccountId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'POS編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'POSId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'店家編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'StoreId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'店家交易時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'StoreTransDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'商店訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'StoreTransId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'商店營收日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'TransmittalDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交易時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'TransDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'台新訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'TransId'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'原始台新訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'SourceTransId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交易類別',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'TransType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'金額類別',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'AmountType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'儲值金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'Amount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'紅利點數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'Bonus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'紅利到期日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'BonusExpiredate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'儲值條碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'BarCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'履保起日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'StoreValueReleaseDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'履保迄日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'StoreValueExpireDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交易來源',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'SourceFrom'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'帳務處理狀態',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'AccountingStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'禮物卡條碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'GiftCardBarCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否轉檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'TRANFLG'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'轉檔時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'TRANDT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'批次編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_TaishinWalletStoreValueLog',
    @level2type = N'COLUMN',
    @level2name = N'BatchId'