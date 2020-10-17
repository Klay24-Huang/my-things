CREATE TABLE [dbo].[TB_NPR340PaymentDetail]
(
		  [CNTRNO] BIGINT        DEFAULT ((0)) NOT NULL, 
    [PAYMENTTYPE] INT NOT NULL DEFAULT 0, 
    [PAYCD] INT NOT NULL DEFAULT 0, 
    [PAYAMT] INT NOT NULL DEFAULT 0, 
    [PORDNO] VARCHAR(256) NOT NULL DEFAULT '', 
    [PAYMEMO] NVARCHAR(500) NOT NULL DEFAULT N'', 
     [MKTime]     DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    CONSTRAINT [PK_TB_NPR340PaymentDetail] PRIMARY KEY ([CNTRNO], [PAYMENTTYPE],[PORDNO]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'支付方式：1:信用卡;2:電子錢包-信用卡;3:電子錢包-虛擬帳卡;4:電子錢包-超商',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340PaymentDetail',
    @level2type = N'COLUMN',
    @level2name = N'PAYMENTTYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'欠費類型 1.租金;2.罰單 3.停車費;4. ETAG;5.營損 / 代收停車費',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340PaymentDetail',
    @level2type = N'COLUMN',
    @level2name = 'PAYCD'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'支付金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340PaymentDetail',
    @level2type = N'COLUMN',
    @level2name = N'PAYAMT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'支付訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340PaymentDetail',
    @level2type = N'COLUMN',
    @level2name = N'PORDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'支付說明',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340PaymentDetail',
    @level2type = N'COLUMN',
    @level2name = N'PAYMEMO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租340沖銷的支付明細',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340PaymentDetail',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租合約編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340PaymentDetail',
    @level2type = N'COLUMN',
    @level2name = N'CNTRNO'