/****** Object:  Table [dbo].[TB_MemberDataWhiteList]    Script Date: 2020/12/29 下午 04:44:31 ******/

CREATE TABLE [dbo].[TB_MemberDataWhiteList](
	[IDNO] [varchar](10) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[MEMO] [varchar](50) NOT NULL,
	[MKTime] [datetime] NULL,
 CONSTRAINT [PK_TB_MemberDataWhiteList] PRIMARY KEY CLUSTERED 
(
	[IDNO] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_MemberDataWhiteList] ADD  CONSTRAINT [DF_TB_MemberDataWhiteList_IDNO]  DEFAULT ('') FOR [IDNO]
GO

ALTER TABLE [dbo].[TB_MemberDataWhiteList] ADD  CONSTRAINT [DF_TB_MemberDataWhiteList_MEMO]  DEFAULT ('') FOR [MEMO]
GO

