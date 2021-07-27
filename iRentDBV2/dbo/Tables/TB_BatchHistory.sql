CREATE TABLE [dbo].[TB_BatchHistory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FunctionName] [varchar](50) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Status] [int] NOT NULL,
	[NextDate] [datetime] NULL,
	[MKTime] [datetime] NOT NULL,
	[MKUser] [varchar](50) NOT NULL,
	[UPDTime] [datetime] NOT NULL,
	[UPDUser] [varchar](50) NOT NULL,
 CONSTRAINT [PK_TB_BatchHistory_1] PRIMARY KEY CLUSTERED 
(
	[FunctionName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_BatchHistory] ADD  CONSTRAINT [DF_TB_BatchHistory_FunctionName]  DEFAULT ('') FOR [FunctionName]
GO

ALTER TABLE [dbo].[TB_BatchHistory] ADD  CONSTRAINT [DF_TB_BatchHistory_Status]  DEFAULT ((0)) FOR [Status]
GO

ALTER TABLE [dbo].[TB_BatchHistory] ADD  CONSTRAINT [DF_TB_BatchHistory_MKTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_BatchHistory] ADD  CONSTRAINT [DF_Table_1_MKEvent]  DEFAULT ('') FOR [MKUser]
GO

ALTER TABLE [dbo].[TB_BatchHistory] ADD  CONSTRAINT [DF_Table_1_UPTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [UPDTime]
GO

ALTER TABLE [dbo].[TB_BatchHistory] ADD  CONSTRAINT [DF_Table_1_UPDEvent]  DEFAULT ('') FOR [UPDUser]
GO

