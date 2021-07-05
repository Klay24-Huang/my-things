CREATE TABLE [dbo].[TB_MedalHistory](
	[IDNO] [varchar](10) NOT NULL,
	[Event] [nvarchar](50) NOT NULL,
	[SubEvent] [varchar](20) NOT NULL,
	[Action] [varchar](10) NOT NULL,
	[Amt] [decimal](10, 2) NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[MKUser] [varchar](50) NOT NULL,
	[UPDTime] [datetime] NOT NULL,
	[UPDUser] [varchar](50) NOT NULL,
	[ActiveFLG] [varchar](1) NOT NULL,
 CONSTRAINT [PK_TB_MedalHistory] PRIMARY KEY CLUSTERED 
(
	[IDNO] ASC,
	[Event] ASC,
	[Action] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MedalHistory] ADD  CONSTRAINT [DF_TB_MedalHistory_IDNO]  DEFAULT ('') FOR [IDNO]
GO

ALTER TABLE [dbo].[TB_MedalHistory] ADD  CONSTRAINT [DF_TB_MedalHistory_Event]  DEFAULT ('') FOR [Event]
GO

ALTER TABLE [dbo].[TB_MedalHistory] ADD  CONSTRAINT [DF_TB_MedalHistory_SubEvent]  DEFAULT ('') FOR [SubEvent]
GO

ALTER TABLE [dbo].[TB_MedalHistory] ADD  CONSTRAINT [DF_TB_MedalHistory_Action]  DEFAULT ('') FOR [Action]
GO

ALTER TABLE [dbo].[TB_MedalHistory] ADD  CONSTRAINT [DF_TB_MedalHistory_Amt]  DEFAULT ((0)) FOR [Amt]
GO

ALTER TABLE [dbo].[TB_MedalHistory] ADD  CONSTRAINT [DF_TB_MedalHistory_MKTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_MedalHistory] ADD  CONSTRAINT [DF_TB_MedalHistory_MKUser]  DEFAULT ('') FOR [MKUser]
GO

ALTER TABLE [dbo].[TB_MedalHistory] ADD  CONSTRAINT [DF_TB_MedalHistory_UPDTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [UPDTime]
GO

ALTER TABLE [dbo].[TB_MedalHistory] ADD  CONSTRAINT [DF_TB_MedalHistory_UPDUser]  DEFAULT ('') FOR [UPDUser]
GO

ALTER TABLE [dbo].[TB_MedalHistory] ADD  CONSTRAINT [DF_TB_MedalHistory_ActiveFLG]  DEFAULT ('') FOR [ActiveFLG]
GO

