CREATE TABLE [dbo].[TB_CensCMDLog](
	[LogId] [bigint] IDENTITY(1,1) NOT NULL,
	[Method] [varchar](50) NOT NULL,
	[CID] [varchar](10) NOT NULL,
	[SendParams] [varchar](1000) NULL,
	[ReceiveRawData] [varchar](1000) NULL,
	[MKTime] [datetime] NULL,
	[UPDTime] [datetime] NULL,
 CONSTRAINT [PK_TB_CensCMDLog] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車機指令' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CensCMDLog', @level2type=N'COLUMN',@level2name=N'Method'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車機CID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CensCMDLog', @level2type=N'COLUMN',@level2name=N'CID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'傳送內容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CensCMDLog', @level2type=N'COLUMN',@level2name=N'SendParams'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收內容' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CensCMDLog', @level2type=N'COLUMN',@level2name=N'ReceiveRawData'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'傳送時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CensCMDLog', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'接收時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CensCMDLog', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO


