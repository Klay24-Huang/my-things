/****** Object:  Table [dbo].[TB_MemberDataBlockMobile]    Script Date: 2021/2/22 下午 02:45:45 ******/

CREATE TABLE [dbo].[TB_MemberDataBlockMobile](
	[Mobile] [varchar](10) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TB_MemberDataBlockMobile] PRIMARY KEY CLUSTERED 
(
	[Mobile] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MemberDataBlockMobile] ADD  CONSTRAINT [DF_TB_MemberDataBlockMobile_CreateDate]  DEFAULT (dateadd(hour,(8),getdate())) FOR [CreateDate]
GO

