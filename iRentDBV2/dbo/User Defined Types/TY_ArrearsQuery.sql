CREATE TYPE [dbo].[TY_ArrearsQuery] AS TABLE(
	[CUSTID] [varchar](10) NULL,
	[ORDNO] [varchar](20) NULL,
	[CNTRNO] [varchar](20) NULL,
	[PAYMENTTYPE] [varchar](5) NULL,
	[SPAYMENTTYPE] [nvarchar](100) NULL,
	[DUEAMT] [int] NULL,
	[PAIDAMT] [int] NULL,
	[CARNO] [varchar](30) NULL,
	[POLNO] [varchar](50) NULL,
	[PAYTYPE] [varchar](5) NULL,
	[GIVEDATE] [varchar](16) NULL,
	[RNTDATE] [varchar](16) NULL,
	[INBRNHCD] [varchar](5) NULL,
	[IRENTORDNO] [varchar](20) NULL,
	[TAMT] [int] NULL
)
GO