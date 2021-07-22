CREATE TABLE [dbo].[TB_TaishinWalletStoreValueRLog] (
    [SEQNO]                 INT             IDENTITY (1, 1) NOT NULL,
    [DifferentType]         VARCHAR (1)     DEFAULT ('') NOT NULL,
    [GUID]                  VARCHAR (32)    NULL,
    [MerchantId]            VARCHAR (16)    DEFAULT ('') NOT NULL,
    [AccountId]             VARCHAR (20)    NULL,
    [POSId]                 VARCHAR (20)    NULL,
    [StoreId]               VARCHAR (20)    NULL,
    [StoreTransDate]        VARCHAR (14)    DEFAULT ('') NOT NULL,
    [StoreTransId]          VARCHAR (20)    DEFAULT ('') NOT NULL,
    [TransmittalDate]       VARCHAR (8)     DEFAULT ('') NOT NULL,
    [TransDate]             VARCHAR (14)    NULL,
    [TransId]               VARCHAR (30)    DEFAULT ('') NOT NULL,
    [SourceTransId]         VARCHAR (30)    NULL,
    [TransType]             VARCHAR (4)     DEFAULT ('') NOT NULL,
    [AmountType]            VARCHAR (1)     DEFAULT ('') NOT NULL,
    [Amount]                INT             DEFAULT ((0)) NOT NULL,
    [Bonus]                 INT             NULL,
    [BonusExpiredate]       VARCHAR (8)     NULL,
    [BarCode]               VARCHAR (20)    NULL,
    [StoreValueReleaseDate] VARCHAR (8)     NULL,
    [StoreValueExpireDate]  VARCHAR (8)     NULL,
    [SourceFrom]            VARCHAR (1)     DEFAULT ('') NOT NULL,
    [ReturnCode]            VARCHAR (4)     NULL,
    [ReturnMessage]         VARCHAR (30)    NULL,
    [Memo]                  NVARCHAR (4000) NULL,
    [GiftCardBarCode]       VARCHAR (16)    NULL,
    [GiftCardAmount]        VARCHAR (16)    DEFAULT ('') NOT NULL,
    [NonGiftCardAmount]     VARCHAR (16)    DEFAULT ('') NOT NULL,
    [SettledFlag]           VARCHAR (1)     DEFAULT ('') NOT NULL,
    [BatchId]               VARCHAR (30)    DEFAULT ('') NOT NULL,
    [TRANFLG]               VARCHAR (1)     DEFAULT ('') NOT NULL,
    [TRANDT]                DATETIME        NULL,
    [MKTime]                DATETIME        DEFAULT ([dbo].[Get_TWDATE]()) NOT NULL,
    CONSTRAINT [PK_TB_TaishinWalletStoreValueRLog_SEQNO] PRIMARY KEY CLUSTERED ([SEQNO] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'轉檔時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TRANDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否轉檔', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TRANFLG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'批次編號(同檔案名稱，不含副檔名)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'BatchId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'儲值餘額結清註記(Y/N)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'SettledFlag';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'非實體物卡餘額(帳戶總餘額減掉實體禮物卡餘額)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'NonGiftCardAmount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'實體禮物卡餘額
(含台新履保實體禮物卡&非台新履保實體禮物卡)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'GiftCardAmount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'禮物卡條碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'GiftCardBarCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註 (比對欄位有差異會註記於此欄位)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'Memo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'回傳訊息(狀態說明)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'ReturnMessage';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'回傳代碼(異動狀態)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'ReturnCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易來源
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'SourceFrom';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'履保迄日YYYYMMDD', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'StoreValueExpireDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'履保起日YYYYMMDD', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'StoreValueReleaseDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'儲值條碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'BarCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'紅利到期日YYYYMMDD', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'BonusExpiredate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'紅利點數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'Bonus';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'儲值金額/儲值退款金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'Amount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'金額類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'AmountType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TransType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原台新訂單編號
退款時此欄位應有值
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'SourceTransId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'台新訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TransId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易時間YYYYMMDDhhmmss', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TransDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店營收日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'TransmittalDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'StoreTransId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'店家交易時間(發動交易日期) YYYYMMDDhhmmss', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'StoreTransDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'店家編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'StoreId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'POS編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'POSId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員虛擬帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'AccountId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'MerchantId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'由商店端產生，雙方識別的唯一值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'差異類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'DifferentType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Log流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletStoreValueRLog', @level2type = N'COLUMN', @level2name = N'SEQNO';

