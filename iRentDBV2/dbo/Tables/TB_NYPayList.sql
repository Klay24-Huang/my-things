CREATE TABLE [dbo].[TB_NYPayList](
	[order_number] [int] NOT NULL,
	[PAYDATE] [varchar](8) NOT NULL,
	[PAYAMT] [int] NOT NULL,
	[RETURNAMT] [int] NOT NULL,
	[NORDNO] [varchar](50) NOT NULL,
	[MKTime] [datetime] NULL,
	[UPDTime] [datetime] NULL,
 CONSTRAINT [PK__TB_NYPay__730E34DE6EFC3DE0] PRIMARY KEY CLUSTERED 
(
	[order_number] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_NYPayList] ADD  CONSTRAINT [DF__TB_NYPayL__PAYDA__691E3D2D]  DEFAULT ('') FOR [PAYDATE]
GO

ALTER TABLE [dbo].[TB_NYPayList] ADD  CONSTRAINT [DF__TB_NYPayL__PAYAM__6A126166]  DEFAULT ((0)) FOR [PAYAMT]
GO

ALTER TABLE [dbo].[TB_NYPayList] ADD  CONSTRAINT [DF_TB_NYPayList_RETURNAMT]  DEFAULT ((0)) FOR [RETURNAMT]
GO

ALTER TABLE [dbo].[TB_NYPayList] ADD  CONSTRAINT [DF__TB_NYPayL__NORDN__6B06859F]  DEFAULT ('') FOR [NORDNO]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訂單編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NYPayList', @level2type=N'COLUMN',@level2name=N'order_number'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付費日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NYPayList', @level2type=N'COLUMN',@level2name=N'PAYDATE'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付費金額' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NYPayList', @level2type=N'COLUMN',@level2name=N'PAYAMT'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'網刷編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NYPayList', @level2type=N'COLUMN',@level2name=N'NORDNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NYPayList', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NYPayList', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO