/****** Object:  Table [dbo].[TB_WalletCodeTable]    Script Date: 2021/9/28 ¤U¤È 01:18:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TB_WalletCodeTable](
	[CodeGroup] [varchar](30) NOT NULL,
	[Code0] [varchar](20) NOT NULL,
	[Code1] [varchar](20) NOT NULL,
	[Code2] [varchar](20) NULL,
	[Code3] [varchar](20) NULL,
	[CodeName] [nvarchar](20) NULL,
	[CodeDescribe] [nvarchar](50) NULL,
	[Negative] [int] NULL,
	[UseFlg] [int] NULL,
	[MKTime] [datetime] NULL,
	[UPDTime] [datetime] NULL,
 CONSTRAINT [PK_TB_WalletCodeTable_1] PRIMARY KEY CLUSTERED 
(
	[CodeGroup] ASC,
	[Code0] ASC,
	[Code1] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_WalletCodeTable] ADD  CONSTRAINT [DF_TB_WalletCodeTable_Negative]  DEFAULT ((0)) FOR [Negative]
GO


