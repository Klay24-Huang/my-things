/****** Object:  Table [dbo].[TB_MemberDataBlockMobile]    Script Date: 2021/2/22 下午 02:45:45 ******/

CREATE TABLE [dbo].[TB_MemberDataBlockMobile](
	[Mobile] [varchar](10) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[MEMO] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_TB_MemberDataBlockMobile] PRIMARY KEY CLUSTERED 
(
	[Mobile] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MemberDataBlockMobile] ADD  CONSTRAINT [DF_TB_MemberDataBlockMobile_CreateDate]  DEFAULT (dateadd(hour,(8),getdate())) FOR [CreateDate]
GO

ALTER TABLE [dbo].[TB_MemberDataBlockMobile] ADD  DEFAULT ('') FOR [MEMO]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'手機號碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberDataBlockMobile', @level2type=N'COLUMN',@level2name=N'Mobile'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberDataBlockMobile', @level2type=N'COLUMN',@level2name=N'CreateDate'
GO