/****** Object:  Table [dbo].[TB_WalletStoreVisualAccountLog]    Script Date: 2021/10/15 �U�� 03:44:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TB_WalletStoreVisualAccountLog](
	[SEQNO] [int] IDENTITY(1,1) NOT NULL,
	[IDNO] [varchar](10) NOT NULL,
	[TrnActNo] [varchar](16) NOT NULL,
	[Amt] [varchar](15) NOT NULL,
	[DueDate] [date] NOT NULL,
	[ReCallFLG] [int] NULL,
	[ReCallTime] [datetime] NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NOT NULL
) ON [PRIMARY]
GO


