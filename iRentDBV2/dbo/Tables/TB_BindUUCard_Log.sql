/****** Object:  Table [dbo].[TB_BindUUCard_Log]    Script Date: 2021/4/15 上午 09:22:13 ******/
CREATE TABLE [dbo].[TB_BindUUCard_Log](
	[LogTime] [datetime] NOT NULL,
	[OrderNumber] [bigint] NOT NULL,
	[IDNO] [varchar](10) NOT NULL,
	[CID] [varchar](10) NOT NULL,
	[DeviceToken] [varchar](256) NOT NULL,
	[IsCens] [tinyint] NOT NULL,
	[OldCardNo] [varchar](30) NOT NULL,
	[NewCardNo] [varchar](30) NOT NULL,
	[Result] [int] NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[UPTime] [datetime] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_BindUUCard_Log] ADD  CONSTRAINT [DF_TB_BindUUCard_Log_UPTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [LogTime]
GO

ALTER TABLE [dbo].[TB_BindUUCard_Log] ADD  CONSTRAINT [DF_TB_BindUUCard_Log_IDNO]  DEFAULT ('') FOR [IDNO]
GO

ALTER TABLE [dbo].[TB_BindUUCard_Log] ADD  CONSTRAINT [DF_TB_BindUUCard_Log_CID]  DEFAULT ('') FOR [CID]
GO

ALTER TABLE [dbo].[TB_BindUUCard_Log] ADD  CONSTRAINT [DF_TB_BindUUCard_Log_deviceToken]  DEFAULT ('') FOR [DeviceToken]
GO

ALTER TABLE [dbo].[TB_BindUUCard_Log] ADD  CONSTRAINT [DF_TB_BindUUCard_Log_IsCens]  DEFAULT ((0)) FOR [IsCens]
GO

ALTER TABLE [dbo].[TB_BindUUCard_Log] ADD  CONSTRAINT [DF_TB_BindUUCard_Log_OldCardNo]  DEFAULT ('') FOR [OldCardNo]
GO

ALTER TABLE [dbo].[TB_BindUUCard_Log] ADD  CONSTRAINT [DF_TB_BindUUCard_Log_NewCardNo]  DEFAULT ('') FOR [NewCardNo]
GO

ALTER TABLE [dbo].[TB_BindUUCard_Log] ADD  CONSTRAINT [DF_TB_BindUUCard_Log_Result]  DEFAULT ((0)) FOR [Result]
GO

ALTER TABLE [dbo].[TB_BindUUCard_Log] ADD  CONSTRAINT [DF_TB_BindUUCard_Log_MKTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_BindUUCard_Log] ADD  CONSTRAINT [DF_TB_BindUUCard_Log_UPTime_1]  DEFAULT (dateadd(hour,(8),getdate())) FOR [UPTime]
GO

