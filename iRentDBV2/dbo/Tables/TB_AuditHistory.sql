CREATE TABLE [dbo].[TB_AuditHistory](
	[AuditHistoryID] [bigint] IDENTITY(1,1) NOT NULL,
	[IDNO] [varchar](10) NOT NULL,
	[AuditUser] [nvarchar](10) NOT NULL,
	[AuditDate] [datetime] NOT NULL,
	[HandleItem] [tinyint] NOT NULL,
	[HandleType] [tinyint] NOT NULL,
	[IsReject] [tinyint] NOT NULL,
	[RejectReason] [nvarchar](1024) NOT NULL,
	[RejectExplain] [nvarchar](1024) NOT NULL,
	[RentType] [tinyint] NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[AuditType] [int] NOT NULL,
 CONSTRAINT [PK_TB_AuditHistory] PRIMARY KEY CLUSTERED 
(
	[AuditHistoryID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_AuditHistory] ADD  CONSTRAINT [DF__TB_AuditHi__IDNO__080C0D4A]  DEFAULT ('') FOR [IDNO]
GO

ALTER TABLE [dbo].[TB_AuditHistory] ADD  CONSTRAINT [DF__TB_AuditH__Audit__09003183]  DEFAULT (N'') FOR [AuditUser]
GO

ALTER TABLE [dbo].[TB_AuditHistory] ADD  CONSTRAINT [DF__TB_AuditH__Audit__09F455BC]  DEFAULT (dateadd(hour,(8),getdate())) FOR [AuditDate]
GO

ALTER TABLE [dbo].[TB_AuditHistory] ADD  CONSTRAINT [DF__TB_AuditH__Handl__0AE879F5]  DEFAULT ((0)) FOR [HandleItem]
GO

ALTER TABLE [dbo].[TB_AuditHistory] ADD  CONSTRAINT [DF__TB_AuditH__Handl__0BDC9E2E]  DEFAULT ((0)) FOR [HandleType]
GO

ALTER TABLE [dbo].[TB_AuditHistory] ADD  CONSTRAINT [DF__TB_AuditH__IsRej__0CD0C267]  DEFAULT ((0)) FOR [IsReject]
GO

ALTER TABLE [dbo].[TB_AuditHistory] ADD  CONSTRAINT [DF__TB_AuditH__Rejec__0DC4E6A0]  DEFAULT (N'') FOR [RejectReason]
GO

ALTER TABLE [dbo].[TB_AuditHistory] ADD  CONSTRAINT [DF__TB_AuditH__Rejec__0EB90AD9]  DEFAULT (N'') FOR [RejectExplain]
GO

ALTER TABLE [dbo].[TB_AuditHistory] ADD  CONSTRAINT [DF_TB_AuditHistory_RentType]  DEFAULT ((0)) FOR [RentType]
GO

ALTER TABLE [dbo].[TB_AuditHistory] ADD  CONSTRAINT [DF__TB_AuditH__MKTim__0FAD2F12]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_AuditHistory] ADD  DEFAULT ((0)) FOR [AuditType]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'審核歷程' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_AuditHistory'
GO