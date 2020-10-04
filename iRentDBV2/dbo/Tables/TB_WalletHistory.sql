CREATE TABLE [dbo].[TB_WalletHistory]
(
[HistoryID] BIGINT NOT NULL IDENTITY ,
     [IDNO] VARCHAR(20) NOT NULL, 
    [WalletMemberID] VARCHAR(20) NOT NULL, 
    [WalletAccountID] VARCHAR(20) NOT NULL,
    [Mode] TINYINT NOT NULL DEFAULT 1,
    [TransDate] DATETIME NOT NULL, 
    [StoreTransId] VARCHAR(50) NOT NULL DEFAULT '', 
    [TransId] VARCHAR(50) NOT NULL DEFAULT '', 
    [Amount] INT NOT NULL DEFAULT 0, 
    [OrderNo] BIGINT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_TB_WalletHistory] PRIMARY KEY ([HistoryID]), 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交易記錄',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WalletHistory',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'0:消費;1:儲值;2:轉贈給他人;3:受他人轉贈;4:退款',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WalletHistory',
    @level2type = N'COLUMN',
    @level2name = N'Mode'
GO

CREATE INDEX [IX_TB_WalletHistory_Search] ON [dbo].[TB_WalletHistory] ([IDNO], [WalletMemberID], [WalletAccountID])

GO

CREATE INDEX [IX_TB_WalletHistory_Search_Order] ON [dbo].[TB_WalletHistory] ([OrderNo])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號，當為付款及還車時才有值',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WalletHistory',
    @level2type = N'COLUMN',
    @level2name = N'OrderNo'