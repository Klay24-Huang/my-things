CREATE TABLE [dbo].[TB_TaishinWalletTransferStoreValueRLog] (
    [SEQNO]             INT             IDENTITY (1, 1) NOT NULL,
    [DifferentType]     VARCHAR (1)     DEFAULT ('') NOT NULL,
    [GUID]              VARCHAR (32)    DEFAULT ('') NOT NULL,
    [MerchantId]        VARCHAR (16)    DEFAULT ('') NOT NULL,
    [AccountId]         VARCHAR (20)    DEFAULT ('') NOT NULL,
    [BarCode]           VARCHAR (21)    NULL,
    [POSId]             VARCHAR (20)    NULL,
    [StoreId]           VARCHAR (20)    NULL,
    [StoreTransDate]    VARCHAR (14)    DEFAULT ('') NOT NULL,
    [StoreTransId]      VARCHAR (20)    DEFAULT ('') NOT NULL,
    [TransmittalDate]   VARCHAR (8)     NULL,
    [TransDate]         VARCHAR (14)    DEFAULT ('') NOT NULL,
    [TransId]           VARCHAR (30)    DEFAULT ('') NOT NULL,
    [Amount]            INT             DEFAULT ((0)) NOT NULL,
    [ActualAmount]      INT             DEFAULT ((0)) NOT NULL,
    [TransAccountId]    VARCHAR (420)   DEFAULT ('') NOT NULL,
    [SourceFrom]        VARCHAR (1)     DEFAULT ('') NOT NULL,
    [Memo]              NVARCHAR (4000) NULL,
    [ReturnCode]        VARCHAR (4)     NULL,
    [ReturnMessage]     VARCHAR (30)    NULL,
    [AmountType]        VARCHAR (1)     NULL,
    [GiftCardAmount]    VARCHAR (16)    DEFAULT ('') NOT NULL,
    [NonGiftCardAmount] VARCHAR (16)    DEFAULT ('') NOT NULL,
    [BatchId]           VARCHAR (30)    DEFAULT ('') NOT NULL,
    [TRANFLG]           VARCHAR (1)     DEFAULT ('') NOT NULL,
    [TRANDT]            DATETIME        NULL,
    [MKTime]            DATETIME        DEFAULT ([dbo].[Get_TWDATE]()) NOT NULL,
    CONSTRAINT [PK_TB_TaishinWalletTransferStoreValueRLog_SEQNO] PRIMARY KEY CLUSTERED ([SEQNO] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'轉檔時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TRANDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否轉檔', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TRANFLG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'批次編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'BatchId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'非實體物卡餘額
 (帳戶總餘額減掉實體禮物卡餘額)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'NonGiftCardAmount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'實體禮物卡餘額
(含台新履保實體禮物卡&非台新履保實體禮物卡)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'GiftCardAmount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'金額類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'AmountType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'回傳訊息(狀態說明)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'ReturnMessage';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'回傳代碼(異動狀態)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'ReturnCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註 (比對欄位有差異會註記於此欄位)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'Memo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易來源', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'SourceFrom';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'贈送/受贈的會員虛擬帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TransAccountId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'實際金額 (交易金額乘轉贈人數)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'ActualAmount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'Amount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'台新訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TransId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易時間YYYYMMDDhhmmss', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TransDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店營收日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TransmittalDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'StoreTransId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'店家交易時間(發動交易日期) YYYYMMDDhhmmss', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'StoreTransDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'店家編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'StoreId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'POS編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'POSId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易條碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'BarCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員虛擬帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'AccountId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'MerchantId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'由商店端產生，雙方識別的唯一值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'差異類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'DifferentType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Log流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletTransferStoreValueRLog', @level2type = N'COLUMN', @level2name = N'SEQNO';

