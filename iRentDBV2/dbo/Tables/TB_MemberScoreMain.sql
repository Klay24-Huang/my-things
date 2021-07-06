CREATE TABLE [dbo].[TB_MemberScoreMain](
	[A_PRGID] [varchar](50) NOT NULL,
	[A_USERID] [varchar](10) NOT NULL,
	[A_SYSDT] [datetime] NOT NULL,
	[U_PRGID] [varchar](50) NOT NULL,
	[U_USERID] [varchar](10) NOT NULL,
	[U_SYSDT] [datetime] NOT NULL,
	[MEMIDNO] [varchar](10) NOT NULL,
	[MEMRFNBR] [int] NOT NULL,
	[SCORE] [int] NOT NULL,
	[BLOCK_CNT] [int] NOT NULL,
	[ISBLOCK] [int] NOT NULL,
	[BLOCK_EDATE] [datetime] NULL,
 CONSTRAINT [PK_TB_MemberScoreMain] PRIMARY KEY CLUSTERED 
(
	[MEMIDNO] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MemberScoreMain] ADD  CONSTRAINT [DF__TB_Member__A_PRG__05664177]  DEFAULT ('') FOR [A_PRGID]
GO

ALTER TABLE [dbo].[TB_MemberScoreMain] ADD  CONSTRAINT [DF__TB_Member__A_USE__065A65B0]  DEFAULT ('') FOR [A_USERID]
GO

ALTER TABLE [dbo].[TB_MemberScoreMain] ADD  CONSTRAINT [DF__TB_Member__A_SYS__074E89E9]  DEFAULT (dateadd(hour,(8),getdate())) FOR [A_SYSDT]
GO

ALTER TABLE [dbo].[TB_MemberScoreMain] ADD  CONSTRAINT [DF__TB_Member__U_PRG__0842AE22]  DEFAULT ('') FOR [U_PRGID]
GO

ALTER TABLE [dbo].[TB_MemberScoreMain] ADD  CONSTRAINT [DF__TB_Member__U_USE__0936D25B]  DEFAULT ('') FOR [U_USERID]
GO

ALTER TABLE [dbo].[TB_MemberScoreMain] ADD  CONSTRAINT [DF__TB_Member__U_SYS__0A2AF694]  DEFAULT (dateadd(hour,(8),getdate())) FOR [U_SYSDT]
GO

ALTER TABLE [dbo].[TB_MemberScoreMain] ADD  CONSTRAINT [DF__TB_Member__MEMID__0B1F1ACD]  DEFAULT ('') FOR [MEMIDNO]
GO

ALTER TABLE [dbo].[TB_MemberScoreMain] ADD  CONSTRAINT [DF__TB_Member__MEMRF__0C133F06]  DEFAULT ((0)) FOR [MEMRFNBR]
GO

ALTER TABLE [dbo].[TB_MemberScoreMain] ADD  CONSTRAINT [DF__TB_Member__SCORE__0D07633F]  DEFAULT ((0)) FOR [SCORE]
GO

ALTER TABLE [dbo].[TB_MemberScoreMain] ADD  CONSTRAINT [DF__TB_Member__BLOCK__0DFB8778]  DEFAULT ((0)) FOR [BLOCK_CNT]
GO

ALTER TABLE [dbo].[TB_MemberScoreMain] ADD  CONSTRAINT [DF__TB_Member__ISBLO__0EEFABB1]  DEFAULT ((0)) FOR [ISBLOCK]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'會員積分主檔' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberScoreMain'
GO

