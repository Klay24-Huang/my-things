/****** Object:  UserDefinedTableType [dbo].[TY_AlertMailLog]    Script Date: 2021/9/10 上午 09:32:00 ******/
CREATE TYPE [dbo].[TY_AlertMailLog] AS TABLE(
	[AlertID] [bigint] NOT NULL,
	[EventType] [int] NOT NULL
)
GO

