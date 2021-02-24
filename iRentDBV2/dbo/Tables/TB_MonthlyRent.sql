CREATE TABLE [dbo].[TB_MonthlyRent](
	[MonthlyRentId] [bigint] IDENTITY(1,1) NOT NULL,
	[ProjID] [varchar](20) NOT NULL,
	[ProjNM] [nvarchar](50) NOT NULL,
	[SEQNO] [bigint] NOT NULL,
	[Mode] [int] NOT NULL,
	[IDNO] [varchar](20) NOT NULL,
	[CarTotalHours] [float] NOT NULL,
	[WorkDayHours] [float] NOT NULL,
	[HolidayHours] [float] NOT NULL,
	[MotoTotalHours] [float] NOT NULL,
	[MotoWorkDayMins] [float] NOT NULL,
	[MotoHolidayMins] [float] NOT NULL,
	[WorkDayRateForCar] [float] NOT NULL,
	[HoildayRateForCar] [float] NOT NULL,
	[WorkDayRateForMoto] [float] NOT NULL,
	[HoildayRateForMoto] [float] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NULL,
 CONSTRAINT [PK_TB_MonthlyRent] PRIMARY KEY CLUSTERED 
(
	[MonthlyRentId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthl__ProjI__13D39108]  DEFAULT ('') FOR [ProjID]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthl__ProjN__14C7B541]  DEFAULT (N'') FOR [ProjNM]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthly__Mode__15BBD97A]  DEFAULT ((-1)) FOR [Mode]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthly__IDNO__16AFFDB3]  DEFAULT ('') FOR [IDNO]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF_TB_MonthlyRent_CarTotalHours]  DEFAULT ((0.000000)) FOR [CarTotalHours]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthl__WorkD__17A421EC]  DEFAULT ((0.000000)) FOR [WorkDayHours]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthl__Holid__18984625]  DEFAULT ((0.000000)) FOR [HolidayHours]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthl__MotoT__198C6A5E]  DEFAULT ((0.000000)) FOR [MotoTotalHours]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF_TB_MonthlyRent_MotoWorkDayMins]  DEFAULT ((0.000000)) FOR [MotoWorkDayMins]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF_TB_MonthlyRent_MotoHolidayMins]  DEFAULT ((0.000000)) FOR [MotoHolidayMins]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthl__WorkD__1A808E97]  DEFAULT ((99)) FOR [WorkDayRateForCar]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthl__Hoild__1B74B2D0]  DEFAULT ((168)) FOR [HoildayRateForCar]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthl__WorkD__1C68D709]  DEFAULT ((1.5)) FOR [WorkDayRateForMoto]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthl__Hoild__1D5CFB42]  DEFAULT ((1.5)) FOR [HoildayRateForMoto]
GO

ALTER TABLE [dbo].[TB_MonthlyRent] ADD  CONSTRAINT [DF__TB_Monthl__MKTim__1E511F7B]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'短租專案代碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'ProjID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'短租專案名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'ProjNM'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'短租流水號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'SEQNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'模式：0:汽車月租;1:機車月租' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'Mode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'身份證' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'IDNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車不區分平假日時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'CarTotalHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車平日時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'WorkDayHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車假日時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'HolidayHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'MotoTotalHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車平日分鐘' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'MotoWorkDayMins'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車假日分鐘' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'MotoHolidayMins'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車平日費率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'WorkDayRateForCar'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車假日費率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'HoildayRateForCar'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車平日費率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'WorkDayRateForMoto'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車假日費率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'HoildayRateForMoto'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'起始日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'StartDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'結束日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent', @level2type=N'COLUMN',@level2name=N'EndDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'月租時數及費率表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonthlyRent'
GO