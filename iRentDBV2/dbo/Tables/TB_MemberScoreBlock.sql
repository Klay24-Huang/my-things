CREATE TABLE [dbo].[TB_MemberScoreBlock](
	[A_PRGID] [varchar](50) NOT NULL,
	[A_USERID] [varchar](10) NOT NULL,
	[A_SYSDT] [datetime] NOT NULL,
	[U_PRGID] [varchar](50) NOT NULL,
	[U_USERID] [varchar](10) NOT NULL,
	[U_SYSDT] [datetime] NOT NULL,
	[SEQ] [int] IDENTITY(1,1) NOT NULL,
	[MEMIDNO] [varchar](10) NOT NULL,
	[START_DT] [datetime] NOT NULL,
	[END_DT] [datetime] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MemberScoreBlock] ADD  CONSTRAINT [DF__TB_Member__A_PRG__07197FBF]  DEFAULT ('') FOR [A_PRGID]
GO

ALTER TABLE [dbo].[TB_MemberScoreBlock] ADD  CONSTRAINT [DF__TB_Member__A_USE__080DA3F8]  DEFAULT ('') FOR [A_USERID]
GO

ALTER TABLE [dbo].[TB_MemberScoreBlock] ADD  CONSTRAINT [DF__TB_Member__A_SYS__0901C831]  DEFAULT (dateadd(hour,(8),getdate())) FOR [A_SYSDT]
GO

ALTER TABLE [dbo].[TB_MemberScoreBlock] ADD  CONSTRAINT [DF__TB_Member__U_PRG__09F5EC6A]  DEFAULT ('') FOR [U_PRGID]
GO

ALTER TABLE [dbo].[TB_MemberScoreBlock] ADD  CONSTRAINT [DF__TB_Member__U_USE__0AEA10A3]  DEFAULT ('') FOR [U_USERID]
GO

ALTER TABLE [dbo].[TB_MemberScoreBlock] ADD  CONSTRAINT [DF__TB_Member__U_SYS__0BDE34DC]  DEFAULT (dateadd(hour,(8),getdate())) FOR [U_SYSDT]
GO

ALTER TABLE [dbo].[TB_MemberScoreBlock] ADD  CONSTRAINT [DF_TB_MemberScoreBlock_MEMIDNO]  DEFAULT ('') FOR [MEMIDNO]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車型代碼主檔' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberScoreBlock'
GO

