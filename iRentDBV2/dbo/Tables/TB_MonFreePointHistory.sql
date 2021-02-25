CREATE TABLE [dbo].[TB_MonFreePointHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[MonFreePointId] [bigint] NOT NULL,
	[OrderNo] [bigint] NOT NULL,
	[IDNO] [varchar](20) NOT NULL,
	[UseMotoTotalFreeMins] [float] NOT NULL,
	[UseMotoWorkDayFreeMins] [float] NOT NULL,
	[UseMotoHolidyDayFreeMins] [float] NOT NULL,
	[UseCarTotalFreeHours] [float] NOT NULL,
	[UseCarWorkDayFreeHours] [float] NOT NULL,
	[UseCarHolidayFreeHours] [float] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[MKTime] [datetime] NOT NULL,
 CONSTRAINT [PK_TB_MonFreePointHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MonFreePointHistory] ADD  CONSTRAINT [DF_TB_MonFreePointHistory_OrderNo]  DEFAULT ((0)) FOR [OrderNo]
GO

ALTER TABLE [dbo].[TB_MonFreePointHistory] ADD  CONSTRAINT [DF_TB_MonFreePointHistory_IDNO]  DEFAULT ('') FOR [IDNO]
GO

ALTER TABLE [dbo].[TB_MonFreePointHistory] ADD  CONSTRAINT [DF_TB_MonFreePointHistory_UseMotoTotalFreeMins]  DEFAULT ((0)) FOR [UseMotoTotalFreeMins]
GO

ALTER TABLE [dbo].[TB_MonFreePointHistory] ADD  CONSTRAINT [DF_TB_MonFreePointHistory_MotoWorkDayFreeMins]  DEFAULT ((0)) FOR [UseMotoWorkDayFreeMins]
GO

ALTER TABLE [dbo].[TB_MonFreePointHistory] ADD  CONSTRAINT [DF_TB_MonFreePointHistory_MotoHolidyDayFreeMins]  DEFAULT ((0)) FOR [UseMotoHolidyDayFreeMins]
GO

ALTER TABLE [dbo].[TB_MonFreePointHistory] ADD  CONSTRAINT [DF_TB_MonFreePointHistory_UseCarTotalFreeHours]  DEFAULT ((0)) FOR [UseCarTotalFreeHours]
GO

ALTER TABLE [dbo].[TB_MonFreePointHistory] ADD  CONSTRAINT [DF_TB_MonFreePointHistory_UseCarWorkDayFreeHours]  DEFAULT ((0)) FOR [UseCarWorkDayFreeHours]
GO

ALTER TABLE [dbo].[TB_MonFreePointHistory] ADD  CONSTRAINT [DF_TB_MonFreePointHistory_UseCarHolidayFreeHours]  DEFAULT ((0)) FOR [UseCarHolidayFreeHours]
GO

ALTER TABLE [dbo].[TB_MonFreePointHistory] ADD  CONSTRAINT [DF_TB_MonFreePointHistory_MKTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_MonFreePointHistory]  WITH CHECK ADD  CONSTRAINT [FK_TB_MonFreePointHistory_TB_MonFreePoint] FOREIGN KEY([MonFreePointId])
REFERENCES [dbo].[TB_MonFreePoint] ([MonFreePointId])
GO

ALTER TABLE [dbo].[TB_MonFreePointHistory] CHECK CONSTRAINT [FK_TB_MonFreePointHistory_TB_MonFreePoint]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流水號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'月租免費點數主表Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'MonFreePointId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訂單編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'OrderNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'身份證' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'IDNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車不分平假日使用免費點數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'UseMotoTotalFreeMins'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車平日使用免費點數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'UseMotoWorkDayFreeMins'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車假日使用免費點數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'UseMotoHolidyDayFreeMins'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車不分平假日使用免費點數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'UseCarTotalFreeHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車平日使用免費點數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'UseCarWorkDayFreeHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車假日使用免費點數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'UseCarHolidayFreeHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'起始日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'StartDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'結束日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePointHistory', @level2type=N'COLUMN',@level2name=N'EndDate'
GO