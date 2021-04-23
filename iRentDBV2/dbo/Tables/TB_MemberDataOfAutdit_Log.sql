/****** Object:  Table [dbo].[TB_MemberDataOfAutdit_Log]    Script Date: 2021/3/23 上午 09:47:20 ******/
CREATE TABLE [dbo].[TB_MemberDataOfAutdit_Log](
	[PROCD] [varchar](5) NOT NULL,
	[UPD_PRG] [varchar](10) NOT NULL,
	[UPD_TIME] [datetime] NOT NULL,
	[AuditID] [bigint] NOT NULL,
	[MEMIDNO] [varchar](10) NOT NULL,
	[MEMCNAME] [nvarchar](60) NOT NULL,
	[MEMTEL] [varchar](20) NOT NULL,
	[MEMHTEL] [varchar](20) NOT NULL,
	[MEMBIRTH] [datetime] NULL,
	[MEMCOUNTRY] [int] NOT NULL,
	[MEMCITY] [int] NOT NULL,
	[MEMADDR] [nvarchar](500) NOT NULL,
	[MEMEMAIL] [varchar](200) NOT NULL,
	[MEMCOMTEL] [varchar](20) NOT NULL,
	[MEMCONTRACT] [nvarchar](10) NOT NULL,
	[MEMCONTEL] [varchar](20) NOT NULL,
	[MEMMSG] [varchar](1) NOT NULL,
	[CARDNO] [varchar](20) NOT NULL,
	[UNIMNO] [varchar](10) NOT NULL,
	[MEMSENDCD] [tinyint] NOT NULL,
	[CARRIERID] [varchar](20) NOT NULL,
	[NPOBAN] [varchar](20) NOT NULL,
	[AuditKind] [tinyint] NOT NULL,
	[HasAudit] [tinyint] NOT NULL,
	[IsNew] [tinyint] NOT NULL,
	[SPECSTATUS] [varchar](2) NOT NULL,
	[SPSD] [varchar](8) NOT NULL,
	[SPED] [varchar](8) NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [PROCD]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [UPD_PRG]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT (dateadd(hour,(8),getdate())) FOR [UPD_TIME]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [MEMIDNO]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT (N'') FOR [MEMCNAME]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [MEMTEL]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [MEMHTEL]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ((0)) FOR [MEMCOUNTRY]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ((0)) FOR [MEMCITY]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT (N'') FOR [MEMADDR]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT (N'') FOR [MEMEMAIL]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [MEMCOMTEL]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT (N'') FOR [MEMCONTRACT]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [MEMCONTEL]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('Y') FOR [MEMMSG]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [CARDNO]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [UNIMNO]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ((2)) FOR [MEMSENDCD]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [CARRIERID]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [NPOBAN]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ((0)) FOR [AuditKind]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ((0)) FOR [HasAudit]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ((0)) FOR [IsNew]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('00') FOR [SPECSTATUS]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [SPSD]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT ('') FOR [SPED]
GO

ALTER TABLE [dbo].[TB_MemberDataOfAutdit_Log] ADD  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'待審核會員資料' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberDataOfAutdit_Log'
GO

