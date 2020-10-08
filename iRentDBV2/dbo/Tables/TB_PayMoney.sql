CREATE TABLE [dbo].[TB_PayMoney]
(
	[PayMoneyId] BIGINT NOT NULL, 
    [PayType] INT NOT NULL DEFAULT 0, 
    [CreditCardNo] VARCHAR(50) NOT NULL DEFAULT '', 
    [StoreTradeNo] VARCHAR(256) NOT NULL DEFAULT '', 
    [OrderNo] BIGINT NOT NULL DEFAULT 0,
    [Amount] INT NOT NULL DEFAULT 0, 
    [ServiceTradeNo] VARCHAR(256) NOT NULL DEFAULT '', 
    [MKTime] [datetime] NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())),
    CONSTRAINT [PK_TB_PayMoney] PRIMARY KEY ([PayMoneyId]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'金流付款記錄列表，包括錢包及信用卡',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PayMoney',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'付款方式：0:信用卡;1:錢包',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PayMoney',
    @level2type = N'COLUMN',
    @level2name = N'PayType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'信用卡隱碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PayMoney',
    @level2type = N'COLUMN',
    @level2name = N'CreditCardNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交易序號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PayMoney',
    @level2type = N'COLUMN',
    @level2name = N'StoreTradeNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'台新交易序號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PayMoney',
    @level2type = N'COLUMN',
    @level2name = N'ServiceTradeNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號，如果不是租金，則代入0',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PayMoney',
    @level2type = N'COLUMN',
    @level2name = N'OrderNo'