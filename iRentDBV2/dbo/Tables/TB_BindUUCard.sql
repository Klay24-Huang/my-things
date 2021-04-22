/****** Object:  Table [dbo].[TB_BindUUCard]    Script Date: 2021/4/15 上午 09:22:03 ******/
CREATE TABLE [dbo].[TB_BindUUCard](
	[OrderNumber] [bigint] NOT NULL,
	[IDNO] [varchar](10) NOT NULL,
	[CID] [varchar](10) NOT NULL,
	[DeviceToken] [varchar](256) NOT NULL,
	[IsCens] [tinyint] NOT NULL,
	[OldCardNo] [varchar](30) NOT NULL,
	[NewCardNo] [varchar](30) NOT NULL,
	[Result] [int] NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[UPTime] [datetime] NOT NULL,
 CONSTRAINT [PK_TB_BindUUCard] PRIMARY KEY CLUSTERED 
(
	[OrderNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_BindUUCard] ADD  CONSTRAINT [DF_TB_BindUUCard_IDNO]  DEFAULT ('') FOR [IDNO]
GO

ALTER TABLE [dbo].[TB_BindUUCard] ADD  CONSTRAINT [DF_TB_BindUUCard_CID]  DEFAULT ('') FOR [CID]
GO

ALTER TABLE [dbo].[TB_BindUUCard] ADD  CONSTRAINT [DF_TB_BindUUCard_deviceToken]  DEFAULT ('') FOR [DeviceToken]
GO

ALTER TABLE [dbo].[TB_BindUUCard] ADD  CONSTRAINT [DF_TB_BindUUCard_IsCens]  DEFAULT ((0)) FOR [IsCens]
GO

ALTER TABLE [dbo].[TB_BindUUCard] ADD  CONSTRAINT [DF_TB_BindUUCard_OldCardNo]  DEFAULT ('') FOR [OldCardNo]
GO

ALTER TABLE [dbo].[TB_BindUUCard] ADD  CONSTRAINT [DF_TB_BindUUCard_NewCardNo]  DEFAULT ('') FOR [NewCardNo]
GO

ALTER TABLE [dbo].[TB_BindUUCard] ADD  CONSTRAINT [DF_TB_BindUUCard_Result]  DEFAULT ((0)) FOR [Result]
GO

ALTER TABLE [dbo].[TB_BindUUCard] ADD  CONSTRAINT [DF_TB_BindUUCard_MKTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_BindUUCard] ADD  CONSTRAINT [DF_TB_BindUUCard_UPTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [UPTime]
GO

