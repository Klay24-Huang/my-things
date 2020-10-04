CREATE TABLE [dbo].[TB_UserWallet]
(
	[IDNO] VARCHAR(20) NOT NULL, 
    [WalletMemberID] VARCHAR(20) NOT NULL, 
    [WalletAccountID] VARCHAR(20) NOT NULL,
    [Status]  VARCHAR(20) NOT NULL DEFAULT(''),
    [Email]   VARCHAR(200) NOT NULL DEFAULT(''),
    [PhoneNo] VARCHAR(20) NOT NULL, 
    [Amount] INT NOT NULL DEFAULT 0,
    [CreateDate] DATETIME NOT NULL, 
 
    [LastTransDate] DATETIME NULL, 
    [LastStoreTransId] VARCHAR(50) NOT NULL DEFAULT '', 
    [LastTransId] VARCHAR(50) NOT NULL DEFAULT '', 
    CONSTRAINT [PK_TB_UserWallet] PRIMARY KEY ([WalletAccountID], [WalletMemberID], [IDNO]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'使用者帳號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserWallet',
    @level2type = N'COLUMN',
    @level2name = N'IDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'錢包虛擬id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserWallet',
    @level2type = N'COLUMN',
    @level2name = N'WalletMemberID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'錢包主檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserWallet',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'開戶時申請的mail',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserWallet',
    @level2type = N'COLUMN',
    @level2name = N'Email'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'開戶時申請的電話',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserWallet',
    @level2type = N'COLUMN',
    @level2name = N'PhoneNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'剩餘總金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserWallet',
    @level2type = N'COLUMN',
    @level2name = N'Amount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'開戶日期',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserWallet',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'錢包狀態',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserWallet',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最近一次台新訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserWallet',
    @level2type = N'COLUMN',
    @level2name = N'LastTransId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最近一次訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_UserWallet',
    @level2type = N'COLUMN',
    @level2name = N'LastStoreTransId'