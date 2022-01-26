/****** Object:  Table [dbo].[TB_WalletTradeMain]    Script Date: 2021/10/15 ¤U¤È 03:43:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TB_WalletTradeMain](
	[SEQNO] [int] IDENTITY(1,1) NOT NULL,
	[ORGID] [varchar](5) NOT NULL,
	[IDNO] [varchar](10) NOT NULL,
	[TaishinNO] [varchar](30) NOT NULL,
	[TradeType] [varchar](20) NOT NULL,
	[TradeKey] [varchar](50) NOT NULL,
	[TradeDate] [datetime] NOT NULL,
	[TradeAMT] [decimal](12, 0) NOT NULL,
	[F_CONTNO] [varchar](20) NULL,
	[F_INTFNO] [varchar](30) NULL,
	[F_TRFCOD] [varchar](1) NOT NULL,
	[F_FEECHK] [varchar](1) NOT NULL,
	[ORDNO] [varchar](12) NULL,
	[ShowFLG] [int] NOT NULL,
	[RetryTimes] [int] NOT NULL,
	[UPDTime] [datetime] NOT NULL,
	[UPDUser] [varchar](10) NOT NULL,
	[UPDPRGID] [varchar](10) NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[MKUser] [varchar](10) NOT NULL,
	[MKPRGID] [varchar](10) NOT NULL,
 CONSTRAINT [PK_TB_WalletTradeMain] PRIMARY KEY CLUSTERED 
(
	[SEQNO] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_WalletTradeMain] ADD  CONSTRAINT [DF_TB_WalletTradeMain_F_INFNO]  DEFAULT ('') FOR [TaishinNO]
GO

ALTER TABLE [dbo].[TB_WalletTradeMain] ADD  CONSTRAINT [DF_TB_WalletTradeMain_TradeType]  DEFAULT ((0)) FOR [TradeType]
GO

ALTER TABLE [dbo].[TB_WalletTradeMain] ADD  CONSTRAINT [DF_TB_WalletTradeMain_TradeKey]  DEFAULT ((0)) FOR [TradeKey]
GO

ALTER TABLE [dbo].[TB_WalletTradeMain] ADD  CONSTRAINT [DF_TB_WalletTradeMain_TradeAMT]  DEFAULT ((0)) FOR [TradeAMT]
GO

ALTER TABLE [dbo].[TB_WalletTradeMain] ADD  CONSTRAINT [DF_TB_WalletTradeMain_T_TRFCOD]  DEFAULT ('N') FOR [F_TRFCOD]
GO

ALTER TABLE [dbo].[TB_WalletTradeMain] ADD  CONSTRAINT [DF_TB_WalletTradeMain_F_FEECHK]  DEFAULT ('N') FOR [F_FEECHK]
GO

ALTER TABLE [dbo].[TB_WalletTradeMain] ADD  CONSTRAINT [DF_TB_WalletTradeMain_ShowFLG]  DEFAULT ((1)) FOR [ShowFLG]
GO

ALTER TABLE [dbo].[TB_WalletTradeMain] ADD  CONSTRAINT [DF_TB_WalletTradeMain_RetryTimes]  DEFAULT ((0)) FOR [RetryTimes]
GO


