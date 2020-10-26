CREATE TABLE [dbo].[TB_tmpFeedBack](
	[TB_tmpFeedBack_ID] [int] IDENTITY(1,1) NOT NULL,
	[OrderNo] [bigint] NOT NULL,
	[CarDesc] [nvarchar](100) NULL,
	[MKTime] [datetime] NOT NULL,
	[UPDTime] [datetime] NOT NULL,
 CONSTRAINT [PK_TB_tmpFeedBack] PRIMARY KEY CLUSTERED 
(
	[TB_tmpFeedBack_ID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_tmpFeedBack] ADD  CONSTRAINT [DF_TB_tmpFeedBack_MKTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [MKTime]
GO

ALTER TABLE [dbo].[TB_tmpFeedBack] ADD  CONSTRAINT [DF_TB_tmpFeedBack_UPDTime]  DEFAULT (dateadd(hour,(8),getdate())) FOR [UPDTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'流水號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_tmpFeedBack', @level2type=N'COLUMN',@level2name=N'TB_tmpFeedBack_ID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訂單編號, 為TB_tmpFeedBackPIC父系Id使用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_tmpFeedBack', @level2type=N'COLUMN',@level2name=N'OrderNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車況描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_tmpFeedBack', @level2type=N'COLUMN',@level2name=N'CarDesc'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上傳取車回饋-Master' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_tmpFeedBack'
GO
