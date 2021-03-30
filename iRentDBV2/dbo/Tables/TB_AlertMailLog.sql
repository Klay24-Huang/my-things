/****** Object:  Table [dbo].[TB_AlertMailLog]    Script Date: 2021/3/30 下午 05:14:00 ******/

CREATE TABLE [dbo].[TB_AlertMailLog](
	[AlertID] [bigint] IDENTITY(1,1) NOT NULL,
	[EventType] [int] NOT NULL,
	[Receiver] [varchar](1024) NOT NULL,
	[Sender] [varchar](256) NOT NULL,
	[HasSend] [tinyint] NOT NULL,
	[CarNo] [varchar](20) NOT NULL,
	[SendTime] [datetime] NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NULL,
 CONSTRAINT [PK_TB_AlertMailLog] PRIMARY KEY CLUSTERED 
(
	[AlertID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_AlertMailLog] ADD  CONSTRAINT [DF__TB_AlertM__Event__4E00329D]  DEFAULT ((0)) FOR [EventType]
GO

ALTER TABLE [dbo].[TB_AlertMailLog] ADD  CONSTRAINT [DF__TB_AlertM__Recei__4EF456D6]  DEFAULT ('') FOR [Receiver]
GO

ALTER TABLE [dbo].[TB_AlertMailLog] ADD  CONSTRAINT [DF__TB_AlertM__Sende__4FE87B0F]  DEFAULT ('') FOR [Sender]
GO

ALTER TABLE [dbo].[TB_AlertMailLog] ADD  CONSTRAINT [DF__TB_AlertM__HasSe__50DC9F48]  DEFAULT ((0)) FOR [HasSend]
GO

ALTER TABLE [dbo].[TB_AlertMailLog] ADD  CONSTRAINT [DF_TB_AlertMailLog_CarNo]  DEFAULT ('') FOR [CarNo]
GO

ALTER TABLE [dbo].[TB_AlertMailLog] ADD  CONSTRAINT [DF__TB_AlertM__MKTim__51D0C381]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'告警信' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_AlertMailLog'
GO

