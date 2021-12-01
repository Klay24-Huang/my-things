/****** Object:  Table [dbo].[TB_WalletWithdrawInvoiceInfo]    Script Date: 2021/11/29 下午 03:16:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TB_WalletWithdrawInvoiceInfo](
	[SEQNO] [int] NOT NULL,
	[SALAMT] [numeric](15, 0) NOT NULL,
	[TAXAMT] [numeric](15, 0) NOT NULL,
	[FEEAMT] [numeric](15, 0) NOT NULL,
	[INV_SENDCD] [tinyint] NULL,
	[INV_CUSTID] [varchar](11) NULL,
	[INV_CUSTNM] [varchar](30) NULL,
	[INV_ADDR] [nvarchar](152) NULL,
	[INVCARRIER] [varchar](20) NULL,
	[NPOBAN] [varchar](20) NULL,
	[RNDCODE] [varchar](4) NULL,
	[RVBANK] [varchar](7) NULL,
	[RVACNT] [varchar](16) NULL,
	[RV_NAME] [nvarchar](60) NULL,
	[INV_NO] [varchar](10) NULL,
	[INV_DATE] [date] NULL,
	[UPDTime] [datetime] NOT NULL,
	[UPDUser] [varchar](10) NOT NULL,
	[UPDPRGID] [varchar](10) NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[MKUser] [varchar](10) NOT NULL,
	[MKPRGID] [varchar](10) NOT NULL,
	[DocURL] [varchar](255) NULL,
 CONSTRAINT [PK_TB_WalletWithdrawInvoiceInfo] PRIMARY KEY CLUSTERED 
(
	[SEQNO] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_WalletWithdrawInvoiceInfo] ADD  CONSTRAINT [DF_TB_WalletWithdrawInvoiceInfo_SALAMT]  DEFAULT ((0)) FOR [SALAMT]
GO

ALTER TABLE [dbo].[TB_WalletWithdrawInvoiceInfo] ADD  CONSTRAINT [DF_TB_WalletWithdrawInvoiceInfo_TAXAMT]  DEFAULT ((0)) FOR [TAXAMT]
GO

ALTER TABLE [dbo].[TB_WalletWithdrawInvoiceInfo] ADD  CONSTRAINT [DF_TB_WalletWithdrawInvoiceInfo_FEEAMT]  DEFAULT ((0)) FOR [FEEAMT]
GO

