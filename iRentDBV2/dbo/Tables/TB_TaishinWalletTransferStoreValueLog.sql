CREATE TABLE [dbo].[TB_TaishinWalletTransferStoreValueLog] (
    [SEQNO]           INT           IDENTITY (1, 1) NOT NULL,
    [GUID]            VARCHAR (32)  DEFAULT ('') NOT NULL,
    [MerchantId]      VARCHAR (16)  DEFAULT ('') NOT NULL,
    [AccountId]       VARCHAR (20)  DEFAULT ('') NOT NULL,
    [BarCode]         VARCHAR (21)  DEFAULT ('') NOT NULL,
    [POSId]           VARCHAR (20)  DEFAULT ('') NOT NULL,
    [StoreId]         VARCHAR (20)  DEFAULT ('') NOT NULL,
    [StoreTransDate]  VARCHAR (14)  DEFAULT ('') NOT NULL,
    [StoreTransId]    VARCHAR (20)  DEFAULT ('') NOT NULL,
    [TransmittalDate] VARCHAR (8)   DEFAULT ('') NOT NULL,
    [TransDate]       VARCHAR (14)  DEFAULT ('') NOT NULL,
    [TransId]         VARCHAR (30)  DEFAULT ('') NOT NULL,
    [Amount]          INT           DEFAULT ((0)) NOT NULL,
    [ActualAmount]    INT           DEFAULT ((0)) NOT NULL,
    [TransAccountId]  VARCHAR (420) DEFAULT ('') NOT NULL,
    [SourceFrom]      VARCHAR (1)   DEFAULT ('') NOT NULL,
    [AmountType]      VARCHAR (1)   DEFAULT ('') NOT NULL,
    [BatchId]         VARCHAR (30)  DEFAULT ('') NULL,
    [TRANFLG]         VARCHAR (1)   DEFAULT ('') NOT NULL,
    [TRANDT]          DATETIME      NULL,
    [MKTime]          DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([SEQNO] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'轉檔時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'TRANDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否轉檔', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'TRANFLG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'批次編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'BatchId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'金額類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'AmountType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易來源', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'SourceFrom';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'受贈的會員虛擬帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'TransAccountId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'實際金額 (交易金額乘轉贈人數)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'ActualAmount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'Amount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'台新訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'TransId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'TransDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店營收日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'TransmittalDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'StoreTransId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'店家交易時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'StoreTransDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'店家編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'StoreId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'POS編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'POSId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易條碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'BarCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員虛擬帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'AccountId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'MerchantId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'由商店端產生，雙方識別的唯一值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Log流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueLog', @level2type = N'COLUMN', @level2name = N'SEQNO';

