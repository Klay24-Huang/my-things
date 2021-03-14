CREATE TABLE [dbo].[Tb_TraceLog2](
	[traceId] [bigint] IDENTITY(1,1) NOT NULL,
	[CodeVersion] [varchar](50) NULL,
	[OrderNo] [bigint] NOT NULL,
	[ApiId] [int] NOT NULL,
	[ApiNm] [nvarchar](200) NOT NULL,
	[ApiMsg] [nvarchar](max) NOT NULL,
	[FlowStep] [nvarchar](max) NOT NULL,
	[TraceType] [varchar](100) NOT NULL,
	[DTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Tb_TraceLog2] PRIMARY KEY CLUSTERED 
(
	[traceId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Tb_TraceLog2] ADD  CONSTRAINT [DF_Tb_TraceLog2_CodeVersion]  DEFAULT ('') FOR [CodeVersion]
GO

ALTER TABLE [dbo].[Tb_TraceLog2] ADD  CONSTRAINT [DF_Tb_TraceLog2_OrderNo]  DEFAULT ((0)) FOR [OrderNo]
GO

ALTER TABLE [dbo].[Tb_TraceLog2] ADD  CONSTRAINT [DF_Tb_TraceLog2_ApiId]  DEFAULT ((0)) FOR [ApiId]
GO

ALTER TABLE [dbo].[Tb_TraceLog2] ADD  CONSTRAINT [DF_Tb_TraceLog2_ApiNm]  DEFAULT ('') FOR [ApiNm]
GO

ALTER TABLE [dbo].[Tb_TraceLog2] ADD  CONSTRAINT [DF_Tb_TraceLog2_ApiMsg]  DEFAULT ('') FOR [ApiMsg]
GO

ALTER TABLE [dbo].[Tb_TraceLog2] ADD  CONSTRAINT [DF_Tb_TraceLog2_FlowStep]  DEFAULT ('') FOR [FlowStep]
GO

ALTER TABLE [dbo].[Tb_TraceLog2] ADD  CONSTRAINT [DF_Tb_TraceLog2_TraceType]  DEFAULT ('') FOR [TraceType]
GO

ALTER TABLE [dbo].[Tb_TraceLog2] ADD  CONSTRAINT [DF_Tb_TraceLog2_DTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [DTime]
GO
