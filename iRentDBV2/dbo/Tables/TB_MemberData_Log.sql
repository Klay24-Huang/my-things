/****** Object:  Table [dbo].[TB_MemberData_Log]    Script Date: 2021/3/23 上午 09:47:01 ******/
CREATE TABLE [dbo].[TB_MemberData_Log](
	[PROCD] [varchar](5) NOT NULL,
	[UPD_PRG] [varchar](10) NOT NULL,
	[UPD_TIME] [datetime] NOT NULL,
	[A_PRGID] [int] NULL,
	[A_USERID] [varchar](10) NULL,
	[A_SYSDT] [datetime] NOT NULL,
	[U_PRGID] [int] NULL,
	[U_USERID] [varchar](10) NULL,
	[U_SYSDT] [datetime] NOT NULL,
	[MEMIDNO] [varchar](10) NOT NULL,
	[MEMCNAME] [nvarchar](60) NOT NULL,
	[MEMPWD] [varchar](100) NOT NULL,
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
	[CARDNO] [varchar](30) NOT NULL,
	[UNIMNO] [varchar](10) NOT NULL,
	[MEMSENDCD] [tinyint] NOT NULL,
	[CARRIERID] [varchar](20) NOT NULL,
	[NPOBAN] [varchar](20) NOT NULL,
	[HasVaildEMail] [tinyint] NOT NULL,
	[HasCheckMobile] [tinyint] NOT NULL,
	[NeedChangePWD] [tinyint] NOT NULL,
	[HasBindSocial] [tinyint] NOT NULL,
	[Audit] [int] NOT NULL,
	[AuditMessage] [nvarchar](1024) NOT NULL,
	[IrFlag] [int] NOT NULL,
	[PayMode] [tinyint] NOT NULL,
	[RentType] [tinyint] NOT NULL,
	[SPECSTATUS] [varchar](2) NOT NULL,
	[SPSD] [varchar](8) NOT NULL,
	[SPED] [varchar](8) NOT NULL,
	[PushREGID] [bigint] NOT NULL,
	[MEMRFNBR] [int] NOT NULL,
	[MEMONEW2] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [PROCD]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT (dateadd(hour,(8),getdate())) FOR [A_SYSDT]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT (dateadd(hour,(8),getdate())) FOR [U_SYSDT]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [MEMIDNO]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT (N'') FOR [MEMCNAME]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [MEMPWD]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [MEMTEL]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [MEMHTEL]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((0)) FOR [MEMCOUNTRY]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((0)) FOR [MEMCITY]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT (N'') FOR [MEMADDR]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT (N'') FOR [MEMEMAIL]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [MEMCOMTEL]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT (N'') FOR [MEMCONTRACT]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [MEMCONTEL]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('Y') FOR [MEMMSG]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [CARDNO]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [UNIMNO]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((2)) FOR [MEMSENDCD]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [CARRIERID]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [NPOBAN]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((0)) FOR [HasVaildEMail]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((0)) FOR [HasCheckMobile]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((0)) FOR [NeedChangePWD]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((0)) FOR [HasBindSocial]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((0)) FOR [Audit]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT (N'') FOR [AuditMessage]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((-1)) FOR [IrFlag]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((0)) FOR [PayMode]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((0)) FOR [RentType]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('00') FOR [SPECSTATUS]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [SPSD]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [SPED]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((0)) FOR [PushREGID]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ((0)) FOR [MEMRFNBR]
GO

ALTER TABLE [dbo].[TB_MemberData_Log] ADD  DEFAULT ('') FOR [MEMONEW2]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'會員資料表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberData_Log'
GO

