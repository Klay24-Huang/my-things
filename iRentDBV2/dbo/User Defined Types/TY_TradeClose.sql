/****** Object:  UserDefinedTableType [dbo].[TY_TradeClose]    Script Date: 2021/11/3 上午 10:54:01 ******/
CREATE TYPE [dbo].[TY_TradeClose] AS TABLE(
	[CloseID] [int] NULL,
	[AuthType] [int] NULL,
	[ChkClose] [int] NULL,
	[CloseAmout] [int] NULL,
	[RefundAmount] [int] NULL
)
GO

