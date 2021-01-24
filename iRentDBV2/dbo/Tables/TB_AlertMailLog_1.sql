CREATE TABLE [dbo].[TB_AlertMailLog](
	[AlertID] [bigint] IDENTITY(1,1) NOT NULL,
	[EventType] [int] NOT NULL,
	[Receiver] [varchar](1024) NOT NULL,
	[Sender] [varchar](256) NOT NULL,
	[HasSend] [tinyint] NOT NULL,
	[CarNo] [varchar](20) NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_AlertMailLog] PRIMARY KEY ([AlertID]),
) 
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

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'告警類別' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_AlertMailLog', @level2type=N'COLUMN',@level2name=N'EventType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'收件者' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_AlertMailLog', @level2type=N'COLUMN',@level2name=N'Receiver'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'寄件者' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_AlertMailLog', @level2type=N'COLUMN',@level2name=N'Sender'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否發送：0:否;1:是;2:失敗' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_AlertMailLog', @level2type=N'COLUMN',@level2name=N'HasSend'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_AlertMailLog', @level2type=N'COLUMN',@level2name=N'CarNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_AlertMailLog', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_AlertMailLog', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'告警信' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_AlertMailLog'
GO
