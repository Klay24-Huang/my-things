CREATE TABLE [dbo].[TB_CreditCardBindLog]
(
    [IDNO] VARCHAR(20) NOT NULL DEFAULT '', 
    [OrderNo] VARCHAR(50) NOT NULL DEFAULT '', 
    [PaymentType] VARCHAR(2) NOT NULL DEFAULT '04', 
    [RtnCode] VARCHAR(4) NOT NULL DEFAULT '', 
    [RtnMessage] NVARCHAR(200) NOT NULL DEFAULT N'', 
    [ResultCode] VARCHAR(8) NOT NULL DEFAULT '', 
    [ResultMessage] NVARCHAR(200) NOT NULL DEFAULT N'', 
    [SendTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [ReceiveTime] DATETIME NULL, 
    CONSTRAINT [PK_TB_CreditCardBindLog] PRIMARY KEY ([OrderNo], [IDNO]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CreditCardBindLog',
    @level2type = N'COLUMN',
    @level2name = N'OrderNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'帳號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CreditCardBindLog',
    @level2type = N'COLUMN',
    @level2name = N'IDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'支付工具類別：03: Account Link;04:信用卡(預設)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CreditCardBindLog',
    @level2type = N'COLUMN',
    @level2name = N'PaymentType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交易結果代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CreditCardBindLog',
    @level2type = N'COLUMN',
    @level2name = N'RtnCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交易結果訊息',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CreditCardBindLog',
    @level2type = N'COLUMN',
    @level2name = N'RtnMessage'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'回傳代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CreditCardBindLog',
    @level2type = N'COLUMN',
    @level2name = N'ResultCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'回傳訊息',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CreditCardBindLog',
    @level2type = N'COLUMN',
    @level2name = N'ResultMessage'