CREATE TABLE [dbo].[TB_TaishinWalletPayTransactionRLog] (
    [SEQNO]                INT             IDENTITY (1, 1) NOT NULL,
    [DifferentType]        VARCHAR (1)     DEFAULT ('') NOT NULL,
    [GUID]                 VARCHAR (32)    DEFAULT ('') NOT NULL,
    [MerchantId]           VARCHAR (16)    DEFAULT ('') NOT NULL,
    [AccountId]            VARCHAR (20)    NULL,
    [BarCode]              VARCHAR (25)    NULL,
    [POSId]                VARCHAR (20)    NULL,
    [StoreId]              VARCHAR (20)    NULL,
    [StoreTransDate]       VARCHAR (14)    DEFAULT ('') NOT NULL,
    [StoreTransId]         VARCHAR (20)    DEFAULT ('') NOT NULL,
    [TransmittalDate]      VARCHAR (8)     DEFAULT ('') NOT NULL,
    [TransDate]            VARCHAR (14)    DEFAULT ('') NOT NULL,
    [TransId]              VARCHAR (30)    DEFAULT ('') NOT NULL,
    [SourceTransId]        VARCHAR (30)    NULL,
    [TransType]            VARCHAR (4)     DEFAULT ('') NOT NULL,
    [BonusFlag]            VARCHAR (1)     DEFAULT ('') NOT NULL,
    [PriceCustody]         VARCHAR (1)     DEFAULT ('') NOT NULL,
    [SmokeLiqueurFlag]     VARCHAR (1)     DEFAULT ('') NOT NULL,
    [Amount]               INT             DEFAULT ((0)) NOT NULL,
    [ActualAmount]         INT             DEFAULT ((0)) NOT NULL,
    [Bonus]                INT             DEFAULT ((0)) NOT NULL,
    [Memo]                 NVARCHAR (4000) NULL,
    [ReturnCode]           VARCHAR (4)     NULL,
    [ReturnMessage]        VARCHAR (30)    NULL,
    [SourceFrom]           VARCHAR (1)     DEFAULT ('') NOT NULL,
    [SmokeAmount]          INT             NULL,
    [ActualGiftCardAmount] INT             DEFAULT ((0)) NOT NULL,
    [ActualOtherAmount]    INT             DEFAULT ((0)) NOT NULL,
    [BatchId]              VARCHAR (30)    DEFAULT ('') NOT NULL,
    [TRANFLG]              VARCHAR (1)     DEFAULT ('') NOT NULL,
    [TRANDT]               DATETIME        NULL,
    [MKTime]               DATETIME        DEFAULT ([Get_TWDATE]()) NOT NULL,
    CONSTRAINT [PK_TB_TaishinWalletPayTransactionRLog_SEQNO] PRIMARY KEY CLUSTERED ([SEQNO] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'轉檔時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'TRANDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否轉檔', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'TRANFLG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'批次編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'BatchId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'折抵其它金額(扣除實體禮物卡與紅利折抵後的金額)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'ActualOtherAmount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'折抵實體禮物卡金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'ActualGiftCardAmount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'菸品金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'SmokeAmount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易來源
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'SourceFrom';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'回傳訊息(狀態說明)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'ReturnMessage';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'回傳代碼(異動狀態)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'ReturnCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註 (比對欄位有差異會註記於此欄位)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'Memo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'折抵紅利點數/退還紅利 ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'Bonus';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'實際金額 (扣除紅利折抵後的金額)
(交易類別為交易退款時，請放入0)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'ActualAmount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易金額/退款金額(不含紅利)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'Amount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否購買菸酒類商品
Y=是
N=否', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'SmokeLiqueurFlag';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'價金保管
Y=是
N=否', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'PriceCustody';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否使用紅利點數
Y=是
N=否', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'BonusFlag';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易類別
T001=交易扣款
T002=交易退款', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'TransType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原台新訂單編號
退款時此欄位應有值
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'SourceTransId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'台新訂單編號
(交易類別為交易退款時，請放入交易扣款的台新訂單編號)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'TransId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易時間YYYYMMDDhhmmss', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'TransDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店營收日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'TransmittalDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'StoreTransId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'店家交易時間(發動交易日期) YYYYMMDDhhmmss', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'StoreTransDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'店家編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'StoreId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'POS編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'POSId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易條碼 (此欄位與會員須虛擬帳號擇一有值)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'BarCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員虛擬帳號 (此欄位與交易條碼需擇一有值)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'AccountId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'商店代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'MerchantId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'由商店端產生，雙方識別的唯一值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'差異類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'DifferentType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Log流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TaishinWalletPayTransactionRLog', @level2type = N'COLUMN', @level2name = N'SEQNO';

