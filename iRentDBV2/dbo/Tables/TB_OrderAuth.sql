/****** Object:  Table [dbo].[TB_OrderAuth]    Script Date: 2021/2/23 下午 03:47:20 ******/
CREATE TABLE [dbo].[TB_OrderAuth](
	[A_PRGID] [varchar](20) NOT NULL,
	[A_USERID] [varchar](20) NOT NULL,
	[A_SYSDT] [datetime] NOT NULL,
	[U_PRGID] [varchar](20) NOT NULL,
	[U_USERID] [varchar](20) NOT NULL,
	[U_SYSDT] [datetime] NOT NULL,
	[authSeq] [bigint] IDENTITY(1,1) NOT NULL,
	[order_number] [bigint] NOT NULL,
	[final_price] [int] NOT NULL,
	[IDNO] [varchar](10) NOT NULL,
	[AuthFlg] [int] NOT NULL,
	[AuthCode] [varchar](50) NOT NULL,
	[AuthMessage] [nvarchar](120) NOT NULL,
	[transaction_no] [varchar](50) NOT NULL,
 CONSTRAINT [PK_TB_OrderAuth] PRIMARY KEY CLUSTERED 
(
	[authSeq] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_OrderAuth] ADD  CONSTRAINT [DF_TB_OrderAuth_A_PRGID]  DEFAULT ('') FOR [A_PRGID]
GO

ALTER TABLE [dbo].[TB_OrderAuth] ADD  CONSTRAINT [DF_TB_OrderAuth_A_USERID]  DEFAULT ('') FOR [A_USERID]
GO

ALTER TABLE [dbo].[TB_OrderAuth] ADD  CONSTRAINT [DF_TB_OrderAuth_A_SYSDT]  DEFAULT (getdate()) FOR [A_SYSDT]
GO

ALTER TABLE [dbo].[TB_OrderAuth] ADD  CONSTRAINT [DF_TB_OrderAuth_U_PRGID]  DEFAULT ('') FOR [U_PRGID]
GO

ALTER TABLE [dbo].[TB_OrderAuth] ADD  CONSTRAINT [DF_TB_OrderAuth_U_USERID]  DEFAULT ('') FOR [U_USERID]
GO

ALTER TABLE [dbo].[TB_OrderAuth] ADD  CONSTRAINT [DF_TB_OrderAuth_U_SYSDT]  DEFAULT (getdate()) FOR [U_SYSDT]
GO

ALTER TABLE [dbo].[TB_OrderAuth] ADD  CONSTRAINT [DF_TB_OrderAuth_final_price]  DEFAULT ((0)) FOR [final_price]
GO

ALTER TABLE [dbo].[TB_OrderAuth] ADD  CONSTRAINT [DF_TB_OrderAuth_IDNO]  DEFAULT ('') FOR [IDNO]
GO

ALTER TABLE [dbo].[TB_OrderAuth] ADD  CONSTRAINT [DF_TB_OrderAuth_AuthFlg]  DEFAULT ((0)) FOR [AuthFlg]
GO

ALTER TABLE [dbo].[TB_OrderAuth] ADD  CONSTRAINT [DF_TB_OrderAuth_AuthCode]  DEFAULT ('') FOR [AuthCode]
GO

ALTER TABLE [dbo].[TB_OrderAuth] ADD  CONSTRAINT [DF_TB_OrderAuth_transaction_no]  DEFAULT ('') FOR [transaction_no]
GO

