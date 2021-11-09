CREATE TABLE [dbo].[TB_OrderAuthAmount] (
    [Seqno]           BIGINT       IDENTITY (1, 1) NOT NULL,
    [order_number]    BIGINT       CONSTRAINT [DF_TB_OrderAuthAmount_order_number] DEFAULT ((0)) NOT NULL,
    [MerchantTradeNo] VARCHAR (50) CONSTRAINT [DF_TB_OrderAuthAmount_MerchantTradeNo] DEFAULT ('') NOT NULL,
    [BankTradeNo]     VARCHAR (50) CONSTRAINT [DF_TB_OrderAuthAmount_transaction_no] DEFAULT ('') NOT NULL,
    [IDNO]            VARCHAR (10) CONSTRAINT [DF_TB_OrderAuthAmount_IDNO] DEFAULT ('') NOT NULL,
    [CardType]        INT          CONSTRAINT [DF_TB_OrderAuthAmount_CardType] DEFAULT ((0)) NOT NULL,
    [AuthType]        INT          CONSTRAINT [DF_TB_OrderAuthAmount_AuthType] DEFAULT ((0)) NOT NULL,
    [Status]          INT          CONSTRAINT [DF_TB_OrderAuthAmount_Status] DEFAULT ((0)) NOT NULL,
    [final_price]     INT          CONSTRAINT [DF_TB_OrderAuthAmount_final_price] DEFAULT ((0)) NOT NULL,
    [A_PRGID]         VARCHAR (20) CONSTRAINT [DF_TB_OrderAuthAmount_A_PRGID] DEFAULT ('') NOT NULL,
    [A_USERID]        VARCHAR (20) CONSTRAINT [DF_TB_OrderAuthAmount_A_USERID] DEFAULT ('') NOT NULL,
    [A_SYSDT]         DATETIME     CONSTRAINT [DF_TB_OrderAuthAmount_A_SYSDT] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [U_PRGID]         VARCHAR (20) CONSTRAINT [DF_TB_OrderAuthAmount_U_PRGID] DEFAULT ('') NOT NULL,
    [U_USERID]        VARCHAR (20) CONSTRAINT [DF_TB_OrderAuthAmount_U_USERID] DEFAULT ('') NOT NULL,
    [U_SYSDT]         DATETIME     CONSTRAINT [DF_TB_OrderAuthAmount_U_SYSDT] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    CONSTRAINT [PK_TB_OrderAuthAmount] PRIMARY KEY NONCLUSTERED ([Seqno] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_TB_OrderAuthAmount]
    ON [dbo].[TB_OrderAuthAmount]([MerchantTradeNo] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'U_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'U_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'U_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'A_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'A_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'A_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'授權金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'final_price';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0:未處理; 1:處理中; 2:已處理', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'Status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'授權類別TB_Code.CodeID (16:預約; 17:訂金; 18:延長用車; 19:取車; 20:逾時; 21:欠費; 22:還車)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'AuthType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'信用卡類別(0:和泰 ; 1:台新)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'CardType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'身分證號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'IDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'銀行回傳的交易序號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'BankTradeNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'廠商訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'MerchantTradeNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'order_number';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂單預授權金額檔流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthAmount', @level2type = N'COLUMN', @level2name = N'Seqno';

