/****** Object:  Table [dbo].[TB_WalletStoreCvsPaymentId]    Script Date: 2021/10/15 ¤U¤È 03:44:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TB_WalletStoreCvsPaymentId](
	[SEQNO] [int] IDENTITY(1,1) NOT NULL,
	[CvsIdentifier] [varchar](5) NOT NULL,
	[PaymentId] [varchar](20) NOT NULL,
	[MKTime] [datetime] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_WalletStoreCvsPaymentId] ADD  CONSTRAINT [DF_TB_WalletStoreCvsPaymentId_CvsIdentifier]  DEFAULT ('') FOR [CvsIdentifier]
GO

ALTER TABLE [dbo].[TB_WalletStoreCvsPaymentId] ADD  CONSTRAINT [DF_TB_WalletStoreCvsPaymentId_PaymentId]  DEFAULT ('') FOR [PaymentId]
GO


