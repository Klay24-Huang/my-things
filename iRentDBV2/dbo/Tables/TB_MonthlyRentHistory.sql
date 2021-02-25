CREATE TABLE [dbo].[TB_MonthlyRentHistory](
	[HistoryID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderNo] [bigint] NOT NULL,
	[IDNO] [varchar](20) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[UseCarFreeType] [int] NOT NULL,
	[UseCarTotalHours] [float] NOT NULL,
	[UseWorkDayHours] [float] NOT NULL,
	[UseHolidayHours] [float] NOT NULL,
	[UseMotoFreeType] [int] NOT NULL,
	[UseMotoTotalHours] [float] NOT NULL,
	[UseMotoWorkDayMins] [float] NOT NULL,
	[UseMotoHolidayMins] [float] NOT NULL,
	[isSend] [tinyint] NOT NULL,
	[MonthlyRentId] [bigint] NOT NULL,
	[MKTime] [datetime] NOT NULL,
 CONSTRAINT [PK_TB_MonthlyRentHistory] PRIMARY KEY CLUSTERED 
(
	[HistoryID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF__TB_Monthl__Order__2374309D]  DEFAULT ((0)) FOR [OrderNo]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF__TB_Monthly__IDNO__246854D6]  DEFAULT ('') FOR [IDNO]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF_TB_MonthlyRentHistory_UseCarFreeType]  DEFAULT ((0)) FOR [UseCarFreeType]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF_TB_MonthlyRentHistory_UseCarTotalHours]  DEFAULT ((0.000000)) FOR [UseCarTotalHours]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF__TB_Monthl__UseWo__2FDA0782]  DEFAULT ((0.000000)) FOR [UseWorkDayHours]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF__TB_Monthl__UseHo__30CE2BBB]  DEFAULT ((0.000000)) FOR [UseHolidayHours]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF_TB_MonthlyRentHistory_UseMotoFreeType]  DEFAULT ((0)) FOR [UseMotoFreeType]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF__TB_Monthl__UseMo__31C24FF4]  DEFAULT ((0.000000)) FOR [UseMotoTotalHours]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF_TB_MonthlyRentHistory_UseMotoWorkDayMins]  DEFAULT ((0.000000)) FOR [UseMotoWorkDayMins]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF_TB_MonthlyRentHistory_UseMotoHolidayMins]  DEFAULT ((0.000000)) FOR [UseMotoHolidayMins]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF__TB_Monthl__isSen__2838E5BA]  DEFAULT ((0)) FOR [isSend]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF__TB_Monthl__SubSc__292D09F3]  DEFAULT ((0)) FOR [MonthlyRentId]
GO

ALTER TABLE [dbo].[TB_MonthlyRentHistory] ADD  CONSTRAINT [DF__TB_Monthl__MKTim__2A212E2C]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訂單編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'OrderNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'身份證' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'IDNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訂閱起始' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'StartDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訂閱迄' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'EndDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用汽車免費時段: 0無,1平日,2假日,3不分平假日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'UseCarFreeType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車總時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'UseCarTotalHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車平日時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'UseWorkDayHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車假日時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'UseHolidayHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用機車免費時段: 0無,1平日,2假日,3不分平假日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'UseMotoFreeType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車總時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'UseMotoTotalHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車平日時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'UseMotoWorkDayMins'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車假日時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'UseMotoHolidayMins'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0:未送出;1:已發送' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'isSend'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRentHistory', @level2type=N'COLUMN',@level2name=N'MKTime'
GO