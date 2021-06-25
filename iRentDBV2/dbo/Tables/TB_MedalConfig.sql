CREATE TABLE [dbo].[TB_MedalConfig](
	[KeyNo] [int] IDENTITY(1,1) NOT NULL,
	[Class] [varchar](10) NOT NULL,
	[ClassName] [nvarchar](20) NULL,
	[Series] [varchar](10) NULL,
	[SeriesName] [nvarchar](20) NULL,
	[Action] [varchar](10) NOT NULL,
	[ActionName] [nvarchar](20) NULL,
	[Stage] [int] NOT NULL,
	[MileStone] [varchar](12) NULL,
	[MileStoneName] [nvarchar](20) NULL,
	[Norm] [int] NOT NULL,
	[Unit] [varchar](5) NOT NULL,
	[MKTime] [datetime] NULL,
	[MKUser] [varchar](10) NULL,
	[UPDTime] [datetime] NULL,
	[UPDUser] [varchar](10) NULL,
	[Active] [varchar](3) NULL,
	[BackStageInsert] [int] NOT NULL,
	[Describe] [nvarchar](50) NULL,
	[iConName] [varchar](30) NULL,
 CONSTRAINT [PK_No] PRIMARY KEY CLUSTERED 
(
	[KeyNo] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MedalConfig] ADD  CONSTRAINT [DF_TB_MedalConfig_MileStoneName]  DEFAULT (N'') FOR [MileStoneName]
GO

ALTER TABLE [dbo].[TB_MedalConfig] ADD  CONSTRAINT [DF_TB_MedalConfig_BackStageInsert]  DEFAULT ((0)) FOR [BackStageInsert]
GO

