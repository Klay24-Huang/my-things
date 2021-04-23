/****** Object:  Table [dbo].[TB_ChkVersion]    Script Date: 2021/3/23 上午 09:46:42 ******/
CREATE TABLE [dbo].[TB_ChkVersion](
	[C_Id] [int] IDENTITY(1,1) NOT NULL,
	[iRentVersion] [varchar](10) NOT NULL,
	[ChkTime] [datetime] NOT NULL,
	[UseFlag] [varchar](2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[C_Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

