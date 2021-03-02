CREATE TYPE [dbo].[TY_MonthlyRent] AS TABLE(
	[IDNO]				varchar(10)		NOT NULL,
	[WorkDayHours]		float			NOT NULL,
	[HolidayHours]		float			NOT NULL,
	[MotorTotalHours]	float			NOT NULL,
	[StartDate]			datetime		NOT NULL,
	[EndDate]			datetime		NOT NULL,
	[SEQNO]				int				NOT NULL,
	[ProjID]			varchar(20)		NOT NULL,
	[ProjNM]			nvarchar(50)	NOT NULL,
	[FavHFee]			float			NOT NULL
)
GO