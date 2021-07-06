CREATE TABLE [dbo].[TB_ScoreDef](
	[A_PRGID] [varchar](10) NULL,
	[A_USERID] [varchar](10) NULL,
	[A_SYSDT] [datetime] NULL,
	[U_PRGID] [varchar](10) NULL,
	[U_USERID] [varchar](10) NULL,
	[U_SYSDT] [datetime] NULL,
	[SEQ] [int] IDENTITY(1,1) NOT NULL,
	[SCTYPENO] [varchar](10) NOT NULL,
	[SCTYPE] [varchar](50) NOT NULL,
	[SCITEMNO] [varchar](10) NOT NULL,
	[SCITEM] [varchar](50) NOT NULL,
	[SCMITEMNO] [varchar](10) NOT NULL,
	[SCMITEM] [varchar](50) NOT NULL,
	[SCDITEMNO] [varchar](10) NOT NULL,
	[SCDITEM] [varchar](50) NOT NULL,
	[UIDESC] [varchar](50) NOT NULL,
	[SCORE] [int] NOT NULL,
	[UI_STATUS] [tinyint] NULL,
 CONSTRAINT [PK_TB_ScoreDef] PRIMARY KEY CLUSTERED 
(
	[SEQ] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [A_PRGID]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [A_USERID]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT (dateadd(hour,(8),getdate())) FOR [A_SYSDT]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [U_PRGID]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [U_USERID]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT (dateadd(hour,(8),getdate())) FOR [U_SYSDT]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [SCTYPENO]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [SCTYPE]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [SCITEMNO]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [SCITEM]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [SCMITEMNO]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [SCMITEM]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [SCDITEMNO]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [SCDITEM]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [UIDESC]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  DEFAULT ('') FOR [SCORE]
GO

ALTER TABLE [dbo].[TB_ScoreDef] ADD  CONSTRAINT [DF_TB_ScoreDef_UI_STATUS]  DEFAULT ((1)) FOR [UI_STATUS]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'積分維護主檔' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ScoreDef'
GO

