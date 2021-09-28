/****** Object:  Table [dbo].[TB_WalletWithdrawInvoiceInfo]    Script Date: 2021/9/28 ¤U¤È 01:15:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TB_WalletWithdrawInvoiceInfo](
	[SEQNO] [int] NOT NULL,
	[TradeAMT] [decimal](12, 0) NOT NULL,
	[SALAMT] [numeric](15, 0) NOT NULL,
	[TAXAMT] [numeric](15, 0) NOT NULL,
	[FEEAMT] [numeric](15, 0) NOT NULL,
	[INV_CUSTID] [varchar](11) NULL,
	[INV_CUSTNM] [varchar](30) NULL,
	[INV_ADDR] [nvarchar](152) NULL,
	[INVCARRIER] [varchar](20) NULL,
	[NPOBAN] [varchar](20) NULL,
	[RNDCODE] [varchar](4) NULL,
	[RVBANK] [varchar](7) NULL,
	[RVACNT] [varchar](16) NULL,
	[RV_NAME] [nvarchar](60) NULL,
	[UPDTime] [datetime] NOT NULL,
	[UPDUser] [varchar](10) NOT NULL,
	[UPDPRGID] [varchar](10) NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[MKUser] [varchar](10) NOT NULL,
	[MKPRGID] [varchar](10) NOT NULL,
 CONSTRAINT [PK_TB_WalletWithdrawInvoiceInfo] PRIMARY KEY CLUSTERED 
(
	[SEQNO] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_WalletWithdrawInvoiceInfo] ADD  CONSTRAINT [DF_TB_WalletWithdrawInvoiceInfo_TradeAMT]  DEFAULT ((0)) FOR [TradeAMT]
GO

ALTER TABLE [dbo].[TB_WalletWithdrawInvoiceInfo] ADD  CONSTRAINT [DF_TB_WalletWithdrawInvoiceInfo_SALAMT]  DEFAULT ((0)) FOR [SALAMT]
GO

ALTER TABLE [dbo].[TB_WalletWithdrawInvoiceInfo] ADD  CONSTRAINT [DF_TB_WalletWithdrawInvoiceInfo_TAXAMT]  DEFAULT ((0)) FOR [TAXAMT]
GO

ALTER TABLE [dbo].[TB_WalletWithdrawInvoiceInfo] ADD  CONSTRAINT [DF_TB_WalletWithdrawInvoiceInfo_FEEAMT]  DEFAULT ((0)) FOR [FEEAMT]
GO

ALTER TABLE [dbo].[TB_WalletWithdrawInvoiceInfo]  WITH CHECK ADD  CONSTRAINT [FK_TB_WalletWithdrawInvoiceInfo_TB_WalletTradeMain] FOREIGN KEY([SEQNO])
REFERENCES [dbo].[TB_WalletTradeMain] ([SEQNO])
GO

ALTER TABLE [dbo].[TB_WalletWithdrawInvoiceInfo] CHECK CONSTRAINT [FK_TB_WalletWithdrawInvoiceInfo_TB_WalletTradeMain]
GO


