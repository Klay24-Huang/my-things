CREATE TABLE [dbo].[TB_NPR330Save](
	[NPR330Save_ID] [int] IDENTITY(1,1) NOT NULL,
	[IDNO] [varchar](20) NOT NULL,
	[IsPay] [int] NOT NULL,
	[useFlag] [int] NOT NULL,
	[PayMode] [int] NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NOT NULL,
	[MerchantTradeNo] [varchar](30) NOT NULL,
	[TaishinTradeNo] [varchar](50) NOT NULL,
    CONSTRAINT [PK_TB_NPR330Save] PRIMARY KEY CLUSTERED 
(
	[NPR330Save_ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_NPR330Save] ADD  CONSTRAINT [DF_TB_NPR330Save_PayMode]  DEFAULT ((0)) FOR [PayMode]
GO

ALTER TABLE [dbo].[TB_NPR330Save] ADD  CONSTRAINT [DF_TB_NPR330Save_MKTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_NPR330Save] ADD  CONSTRAINT [DF_TB_NPR330Save_UPDTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [UPDTime]
GO

ALTER TABLE [dbo].[TB_NPR330Save] ADD  CONSTRAINT [DF_TB_NPR330Save_MerchantTradeNo]  DEFAULT ('') FOR [MerchantTradeNo]
GO

ALTER TABLE [dbo].[TB_NPR330Save] ADD  CONSTRAINT [DF_TB_NPR330Save_TaishinTradeNo]  DEFAULT ('') FOR [TaishinTradeNo]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流水號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'NPR330Save_ID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'身分證號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'IDNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否付款' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'IsPay'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'有效區分' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'useFlag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'付款方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'PayMode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'產生時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交易序號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'MerchantTradeNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'台新交易序號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'TaishinTradeNo'
GO