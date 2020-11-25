CREATE TABLE [dbo].[TB_NPR340]
(
	[SEQNO] INT NOT NULL IDENTITY, 
    [CUSTID] VARCHAR(20) NOT NULL DEFAULT '', 
    [ORDNO] VARCHAR(50) NOT NULL DEFAULT '', 
    [CNTRNO] VARCHAR(50) NOT NULL DEFAULT '', 
    [PAYMENTTYPE] VARCHAR(10) NOT NULL DEFAULT '', 
    [CARNO] VARCHAR(10) NOT NULL DEFAULT '',
    [NORDNO] VARCHAR(50) NOT NULL DEFAULT '', 
    [PAYDATE] VARCHAR(30) NOT NULL DEFAULT '', 
    [AUTH_CODE] VARCHAR(10) NOT NULL DEFAULT '', 
    [AMOUNT] VARCHAR(20) NOT NULL DEFAULT '', 
    [CDTMAN] NVARCHAR(10) NOT NULL DEFAULT '', 
    [CARDNO] VARCHAR(30) NOT NULL DEFAULT '', 
    [POLNO] VARCHAR(30) NOT NULL DEFAULT '', 
    [MerchantTradeNo] VARCHAR(40) NOT NULL DEFAULT '', 
    [ServerTradeNo] VARCHAR(50)   NOT NULL DEFAULT '', 
    [isRetry] INT NOT NULL DEFAULT 1, 
    [MKTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [UPDTime] DATETIME NULL, 
    CONSTRAINT [PK_TB_NPR340] PRIMARY KEY ([SEQNO]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'客戶代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'CUSTID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預約編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'ORDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'合約編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'CNTRNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'欠費類型 1.租金 2.罰單 3.停車費 4. ETAG',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'PAYMENTTYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'CARNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'網路刷卡訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'NORDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'刷卡日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'PAYDATE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'刷卡授權碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'AUTH_CODE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'沖銷金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'AMOUNT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'刷卡人',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'CDTMAN'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'信用卡號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'CARDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'罰單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'POLNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號(自訂)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'MerchantTradeNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'台新訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'ServerTradeNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N':0:否;1:是',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = N'COLUMN',
    @level2name = N'isRetry'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租欠費沖銷',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE INDEX [IX_TB_NPR340_Search] ON [dbo].[TB_NPR340] ([CUSTID], [ORDNO], [NORDNO], [CNTRNO], [POLNO], [AMOUNT], [isRetry])
