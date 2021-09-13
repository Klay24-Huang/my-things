CREATE TABLE [dbo].[TB_PersonNotificationRLog](
	[NotificationID] [bigint] NOT NULL,
	[MKTime] [datetime] NULL,
 CONSTRAINT [PK_TB_PersonNotificationRLog] PRIMARY KEY CLUSTERED 
(
	[NotificationID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'個人推播流水號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_PersonNotificationRLog', @level2type=N'COLUMN',@level2name=N'NotificationID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'讀取紀錄時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_PersonNotificationRLog', @level2type=N'COLUMN',@level2name=N'MKTime'
GO