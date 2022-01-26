CREATE TABLE [dbo].[TB_MemberCMK](
	[MEMIDNO] [varchar](10) NOT NULL,
	[MEMRFNBR] [int] NOT NULL,
	[VerType] [varchar](20) NOT NULL,
	[Version] [varchar](20) NOT NULL,
	[Source] [varchar](1) NOT NULL,
	[AgreeDate] [datetime] NOT NULL,
	[TEL] [varchar](1) NOT NULL,
	[SMS] [varchar](1) NOT NULL,
	[EMAIL] [varchar](1) NOT NULL,
	[POST] [varchar](1) NOT NULL,
	[A_PRGID] [varchar](50) NOT NULL,
	[A_USERID] [varchar](10) NOT NULL,
	[A_SYSDT] [datetime] NOT NULL,
	[U_PRGID] [varchar](50) NOT NULL,
	[U_USERID] [varchar](10) NOT NULL,
	[U_SYSDT] [datetime] NOT NULL,
 CONSTRAINT [PK_TB_MemberCMK] PRIMARY KEY CLUSTERED 
(
	[MEMIDNO] ASC,
	[VerType] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_MEMIDNO]  DEFAULT ('') FOR [MEMIDNO]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_MEMRFNBR]  DEFAULT ((0)) FOR [MEMRFNBR]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_Table_1_VERTYPE]  DEFAULT ('') FOR [VerType]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_Table_1_Version]  DEFAULT ('') FOR [Version]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_Source]  DEFAULT ('') FOR [Source]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_AgreeDate_1]  DEFAULT (dateadd(hour,(8),getdate())) FOR [AgreeDate]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_TEL]  DEFAULT ('') FOR [TEL]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_SMS]  DEFAULT ('') FOR [SMS]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_EMAIL]  DEFAULT ('') FOR [EMAIL]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_POST]  DEFAULT ('') FOR [POST]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_A_PRGID]  DEFAULT ('') FOR [A_PRGID]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_A_USERID]  DEFAULT ('') FOR [A_USERID]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_A_SYSDT]  DEFAULT (dateadd(hour,(8),getdate())) FOR [A_SYSDT]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_U_PRGID]  DEFAULT ('') FOR [U_PRGID]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_U_USERID]  DEFAULT ('') FOR [U_USERID]
GO

ALTER TABLE [dbo].[TB_MemberCMK] ADD  CONSTRAINT [DF_TB_MemberCMK_U_SYSDT]  DEFAULT (dateadd(hour,(8),getdate())) FOR [U_SYSDT]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'身分證字號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'MEMIDNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'會員流水號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'MEMRFNBR'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'同意書版本類型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'VerType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'同意書版本號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'Version'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'同意來源管道 (I:IRENT，W:官網)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'Source'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'同意時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'AgreeDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'電話通知狀態 (N:不通知、Y:通知)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'TEL'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'簡訊通知狀態 (N:不通知、Y:通知)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'SMS'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'EMAIL通知 (N:不通知、Y:通知)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'EMAIL'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'郵寄通知 (N:不通知、Y:通知)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'POST'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登錄程式代號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'A_PRGID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登錄者' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'A_USERID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登錄時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'A_SYSDT'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改程式代號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'U_PRGID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改者' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'U_USERID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberCMK', @level2type=N'COLUMN',@level2name=N'U_SYSDT'
GO