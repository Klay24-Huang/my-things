CREATE TABLE [dbo].[TB_MedalProgressStatus](
	[IDNO] [varchar](10) NOT NULL,
	[Class] [varchar](10) NOT NULL,
	[Series] [nvarchar](20) NOT NULL,
	[Action] [varchar](10) NOT NULL,
	[Amt] [decimal](10, 2) NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NOT NULL,
 CONSTRAINT [PK_TB_MedalProgressStatus] PRIMARY KEY CLUSTERED 
(
	[IDNO] ASC,
	[Action] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MedalProgressStatus] ADD  CONSTRAINT [DF_TB_MedalProgressStatus_IDNO]  DEFAULT ('') FOR [IDNO]
GO

ALTER TABLE [dbo].[TB_MedalProgressStatus] ADD  CONSTRAINT [DF_TB_MedalProgressStatus_Class]  DEFAULT ('') FOR [Class]
GO

ALTER TABLE [dbo].[TB_MedalProgressStatus] ADD  CONSTRAINT [DF_TB_MedalProgressStatus_Series]  DEFAULT ('') FOR [Series]
GO

ALTER TABLE [dbo].[TB_MedalProgressStatus] ADD  CONSTRAINT [DF_TB_MedalProgressStatus_Action]  DEFAULT ('') FOR [Action]
GO

ALTER TABLE [dbo].[TB_MedalProgressStatus] ADD  CONSTRAINT [DF_TB_MedalProgressStatus_Amt]  DEFAULT ((0)) FOR [Amt]
GO

ALTER TABLE [dbo].[TB_MedalProgressStatus] ADD  CONSTRAINT [DF_TB_MedalProgressStatus_MKTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_MedalProgressStatus] ADD  CONSTRAINT [DF_TB_MedalProgressStatus_UPDTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [UPDTime]
GO

