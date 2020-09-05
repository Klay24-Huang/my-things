CREATE TABLE [dbo].[TB_ProjectDiscount]
(
	[ProjID] [VARCHAR](10) NOT NULL DEFAULT '',
	[CARTYPE] [VARCHAR](10) NOT NULL DEFAULT '',
	[CUSTOMIZE] [CHAR](1) NOT NULL DEFAULT 'N',
	[CUSDAY] [SMALLINT] NOT NULL DEFAULT 0,
	[DISTYPE] [TINYINT] NOT NULL DEFAULT 2,
	[DISRATE] [FLOAT] NOT NULL DEFAULT 0.0,
	[PRICE] [FLOAT] NOT NULL DEFAULT 0.0,
	[PRICE_H] [FLOAT] NOT NULL DEFAULT 0.0,
	[DISCOUNT] [FLOAT] NOT NULL DEFAULT 0.0,
	[PHOURS] [FLOAT] NOT NULL DEFAULT 0.0,
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_ProjectDiscount] PRIMARY KEY ([ProjID], [CARTYPE]),
)
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

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'優惠價(平日)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name='PRICE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'優惠價(假日)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name='PRICE_H'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'折扣金額' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'DISCOUNT'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'時數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'PHOURS'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectDiscount', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案優惠價',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectDiscount',
    @level2type = NULL,
    @level2name = NULL