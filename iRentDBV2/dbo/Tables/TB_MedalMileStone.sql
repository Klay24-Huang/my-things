CREATE TABLE [dbo].[TB_MedalMileStone](
	[IDNO] [varchar](10) NOT NULL,
	[Class] [varchar](10) NOT NULL,
	[Series] [nvarchar](20) NOT NULL,
	[Action] [varchar](10) NOT NULL,
	[MileStone] [varchar](12) NOT NULL,
	[Norm] [int] NOT NULL,
	[Progress] [decimal](10, 2) NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NOT NULL,
	[ShowTime] [datetime] NULL,
	[GetMedalTime] [datetime] NULL,
 CONSTRAINT [PK_TB_MedalMileStone] PRIMARY KEY CLUSTERED 
(
	[IDNO] ASC,
	[MileStone] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MedalMileStone] ADD  CONSTRAINT [DF_TB_MedalMileStone_IDNO]  DEFAULT ('') FOR [IDNO]
GO

ALTER TABLE [dbo].[TB_MedalMileStone] ADD  CONSTRAINT [DF_TB_MedalMileStone_Class]  DEFAULT ('') FOR [Class]
GO

ALTER TABLE [dbo].[TB_MedalMileStone] ADD  CONSTRAINT [DF_TB_MedalMileStone_Series]  DEFAULT ('') FOR [Series]
GO

ALTER TABLE [dbo].[TB_MedalMileStone] ADD  CONSTRAINT [DF_TB_MedalMileStone_Action]  DEFAULT ('') FOR [Action]
GO

ALTER TABLE [dbo].[TB_MedalMileStone] ADD  CONSTRAINT [DF_TB_MedalMileStone_MileStone]  DEFAULT ('') FOR [MileStone]
GO

ALTER TABLE [dbo].[TB_MedalMileStone] ADD  CONSTRAINT [DF_TB_MedalMileStone_Norm]  DEFAULT ((0)) FOR [Norm]
GO

ALTER TABLE [dbo].[TB_MedalMileStone] ADD  CONSTRAINT [DF_TB_MedalMileStone_Progress]  DEFAULT ((0)) FOR [Progress]
GO

ALTER TABLE [dbo].[TB_MedalMileStone] ADD  CONSTRAINT [DF_TB_MedalMileStone_MKTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_MedalMileStone] ADD  CONSTRAINT [DF_TB_MedalMileStone_UPDTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [UPDTime]
GO

