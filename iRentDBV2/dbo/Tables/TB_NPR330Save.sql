CREATE TABLE [dbo].[TB_NPR330Save](
	[NPR330Save_ID] [int] IDENTITY(1,1) NOT NULL,
	[IDNO] [varchar](20) NOT NULL,
	[IsPay] [int] NOT NULL,
	[useFlag] [int] NOT NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NOT NULL,
 [MerchantTradeNo] VARCHAR(30) NOT NULL DEFAULT (''), 
    [TaishinTradeNo] VARCHAR(50) NULL DEFAULT (''), 
    CONSTRAINT [PK_TB_NPR330Save] PRIMARY KEY CLUSTERED 
(
	[NPR330Save_ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_NPR330Save] ADD  CONSTRAINT [DF_TB_NPR330Save_IsPay]  DEFAULT ((0)) FOR [IsPay]
GO

ALTER TABLE [dbo].[TB_NPR330Save] ADD  CONSTRAINT [DF_TB_NPR330Save_useFlag]  DEFAULT ((1)) FOR [useFlag]
GO

ALTER TABLE [dbo].[TB_NPR330Save] ADD  CONSTRAINT [DF_TB_NPR330Save_MKTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_NPR330Save] ADD  CONSTRAINT [DF_TB_NPR330Save_UPDTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [UPDTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'身分證號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'IDNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否已付款,0(未付款),1(已付款)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'IsPay'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用0(否),1(是)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_NPR330Save', @level2type=N'COLUMN',@level2name=N'useFlag'
GO


