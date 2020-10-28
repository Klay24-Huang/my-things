CREATE TABLE [dbo].[TB_NPR330](
	[TB_NPR330_ID] [int] IDENTITY(1,1) NOT NULL,
	[CarNo] [varchar](10) NULL,
	[Amount] [int] NULL,
	[ArrearsKind] [varchar](5) NULL,
	[StartDate] [varchar](16) NULL,
	[EndDate] [varchar](16) NULL,
	[OrderNo] [varchar](20) NULL,
	[ShortOrderNo] [varchar](20) NULL,
	[StationID] [varchar](10) NULL,
	[CarType] [varchar](20) NULL,
	[IsMotor] [tinyint] NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NULL,
 CONSTRAINT [PK_TB_NPR330] PRIMARY KEY CLUSTERED 
(
	[TB_NPR330_ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_NPR330] ADD  CONSTRAINT [DF_TB_NPR330_MKTime]  DEFAULT (getdate()) FOR [MKTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流水號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'TB_NPR330_ID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'待繳金額' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'Amount'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'欠費種類 1:租金,2:罰單,3:停車費,4:ETAG' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'ArrearsKind'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'實際取車時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'StartDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'實際還車時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'EndDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訂單編號,因api可能回傳非bigint,此處改字串' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'OrderNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'短租合約編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'ShortOrderNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'據點ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'StationID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車型代碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'CarType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否為機車（0:否;1:是)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'IsMotor'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'欠費查詢紀錄' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330'
GO


