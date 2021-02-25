CREATE TABLE [dbo].[TB_MonFreePoint](
	[MonFreePointId] [bigint] IDENTITY(1,1) NOT NULL,
	[MonthlyRentId] [bigint] NOT NULL,
	[MotoFreeType] [int] NOT NULL,
	[MotoTotalFreeMins] [float] NOT NULL,
	[MotoWorkDayFreeMins] [float] NOT NULL,
	[MotoHolidyDayFreeMins] [float] NOT NULL,
	[CarFreeType] [int] NOT NULL,
	[CarTotalFreeHours] [float] NOT NULL,
	[CarWorkDayFreeHours] [float] NOT NULL,
	[CarHolidayFreeHours] [float] NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NULL,
 CONSTRAINT [PK_TB_MonFreePoint] PRIMARY KEY CLUSTERED 
(
	[MonFreePointId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MonFreePoint] ADD  CONSTRAINT [DF_TB_MonFreePoint_MotoFreeType]  DEFAULT ((0)) FOR [MotoFreeType]
GO

ALTER TABLE [dbo].[TB_MonFreePoint] ADD  CONSTRAINT [DF_TB_MonFreePoint_MotoTotalFreeMins]  DEFAULT ((0)) FOR [MotoTotalFreeMins]
GO

ALTER TABLE [dbo].[TB_MonFreePoint] ADD  CONSTRAINT [DF_TB_MonFreePoint_MotoWorkDayFreeMins]  DEFAULT ((0)) FOR [MotoWorkDayFreeMins]
GO

ALTER TABLE [dbo].[TB_MonFreePoint] ADD  CONSTRAINT [DF_TB_MonFreePoint_MotoHolidyDayFreeMins]  DEFAULT ((0)) FOR [MotoHolidyDayFreeMins]
GO

ALTER TABLE [dbo].[TB_MonFreePoint] ADD  CONSTRAINT [DF_TB_MonFreePoint_CarFreeType]  DEFAULT ((0)) FOR [CarFreeType]
GO

ALTER TABLE [dbo].[TB_MonFreePoint] ADD  CONSTRAINT [DF_TB_MonFreePoint_CarTotalFreeHours]  DEFAULT ((0)) FOR [CarTotalFreeHours]
GO

ALTER TABLE [dbo].[TB_MonFreePoint] ADD  CONSTRAINT [DF_TB_MonFreePoint_CarWorkDayFreeHours]  DEFAULT ((0)) FOR [CarWorkDayFreeHours]
GO

ALTER TABLE [dbo].[TB_MonFreePoint] ADD  CONSTRAINT [DF_TB_MonFreePoint_CarHolidayFreeHours]  DEFAULT ((0)) FOR [CarHolidayFreeHours]
GO

ALTER TABLE [dbo].[TB_MonFreePoint] ADD  CONSTRAINT [DF_TB_MonFreePoint_MKTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_MonFreePoint]  WITH CHECK ADD  CONSTRAINT [FK_TB_MonFreePoint_TB_MonthlyRent] FOREIGN KEY([MonthlyRentId])
REFERENCES [dbo].[TB_MonthlyRent] ([MonthlyRentId])
GO

ALTER TABLE [dbo].[TB_MonFreePoint] CHECK CONSTRAINT [FK_TB_MonFreePoint_TB_MonthlyRent]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'月租IId' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePoint', @level2type=N'COLUMN',@level2name=N'MonthlyRentId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車免費時段: 0無,1平日,2假日,3不分平假日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePoint', @level2type=N'COLUMN',@level2name=N'MotoFreeType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車不分平假日免費分鐘數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePoint', @level2type=N'COLUMN',@level2name=N'MotoTotalFreeMins'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車平日免費分鐘數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePoint', @level2type=N'COLUMN',@level2name=N'MotoWorkDayFreeMins'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'機車假日免費分鐘數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePoint', @level2type=N'COLUMN',@level2name=N'MotoHolidyDayFreeMins'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車免費時段: 0無,1平日,2假日,3不分平假日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePoint', @level2type=N'COLUMN',@level2name=N'CarFreeType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車不分平假日免費時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePoint', @level2type=N'COLUMN',@level2name=N'CarTotalFreeHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車平日免費時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePoint', @level2type=N'COLUMN',@level2name=N'CarWorkDayFreeHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'汽車假日免費時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePoint', @level2type=N'COLUMN',@level2name=N'CarHolidayFreeHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'月租免費點數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MonFreePoint'
GO
