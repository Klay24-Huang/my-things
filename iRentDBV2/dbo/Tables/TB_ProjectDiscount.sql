CREATE TABLE [dbo].[TB_ProjectDiscount](
	[ProjID] [varchar](10) NOT NULL,
	[CARTYPE] [varchar](10) NOT NULL,
	[CUSTOMIZE] [char](1) NOT NULL,
	[CUSDAY] [smallint] NOT NULL,
	[DISTYPE] [tinyint] NOT NULL,
	[DISRATE] [float] NOT NULL,
	[PRICE] [float] NOT NULL,
	[PRICE_H] [float] NOT NULL,
	[DISCOUNT] [float] NOT NULL,
	[PHOURS] [float] NOT NULL,
	[FirstFreeMins] [float] NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NULL,
 CONSTRAINT [PK_TB_ProjectDiscount] PRIMARY KEY CLUSTERED 
(
	[ProjID] ASC,
	[CARTYPE] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF__TB_Projec__ProjI__6B44E613]  DEFAULT ('') FOR [ProjID]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF__TB_Projec__CARTY__6C390A4C]  DEFAULT ('') FOR [CARTYPE]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF__TB_Projec__CUSTO__6D2D2E85]  DEFAULT ('N') FOR [CUSTOMIZE]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF__TB_Projec__CUSDA__6E2152BE]  DEFAULT ((0)) FOR [CUSDAY]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF__TB_Projec__DISTY__6F1576F7]  DEFAULT ((2)) FOR [DISTYPE]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF__TB_Projec__DISRA__70099B30]  DEFAULT ((0.0)) FOR [DISRATE]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF__TB_Projec__PRICE__70FDBF69]  DEFAULT ((0.0)) FOR [PRICE]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF__TB_Projec__PRICE__71F1E3A2]  DEFAULT ((0.0)) FOR [PRICE_H]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF__TB_Projec__DISCO__72E607DB]  DEFAULT ((0.0)) FOR [DISCOUNT]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF__TB_Projec__PHOUR__73DA2C14]  DEFAULT ((0.0)) FOR [PHOURS]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF_TB_ProjectDiscount_FirstFreeMins]  DEFAULT ((0)) FOR [FirstFreeMins]
GO

ALTER TABLE [dbo].[TB_ProjectDiscount] ADD  CONSTRAINT [DF__TB_Projec__MKTim__74CE504D]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'專案代碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'ProjID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'CARTYPE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否自訂 Y:自訂;N:非自訂' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'CUSTOMIZE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'自訂天數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'CUSDAY'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'優惠類型 1:折扣;2:優惠價;3:折價;' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'DISTYPE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'折扣費率' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'DISRATE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'優惠價(平日)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'PRICE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'優惠價(假日)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'PRICE_H'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'折扣金額' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'DISCOUNT'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'PHOURS'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'前n分鐘0元' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'FirstFreeMins'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'專案優惠價' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount'
GO

