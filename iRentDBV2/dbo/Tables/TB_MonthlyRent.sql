CREATE TABLE [dbo].[TB_MonthlyRent]
(
	[MonthlyRentId]  BIGINT NOT NULL IDENTITY ,
    [ProjID]		 VARCHAR(20) NOT NULL DEFAULT '',
	[ProjNM]		 NVARCHAR(50) NOT NULL DEFAULT N'',
    [SEQNO]			 BIGINT NOT NULL,
    [Mode]           INT          DEFAULT (-1) NOT NULL,
	[IDNO]           VARCHAR (20) DEFAULT ('') NOT NULL,
    [WorkDayHours]   FLOAT DEFAULT ((0.000000)) NOT NULL, 
    [HolidayHours]   FLOAT DEFAULT ((0.000000)) NOT NULL, 
    [MotoTotalHours] FLOAT DEFAULT ((0.000000)) NOT NULL, 
    [WorkDayRateForCar] FLOAT NOT NULL DEFAULT 99, 
    [HoildayRateForCar] FLOAT NOT NULL DEFAULT 168, 
    [WorkDayRateForMoto] FLOAT NOT NULL DEFAULT 1.5, 
    [HoildayRateForMoto] FLOAT NOT NULL DEFAULT 1.5, 
    [StartDate]      DATETIME     NOT NULL,
    [EndDate]        DATETIME     NOT NULL,
    [MKTime]         DATETIME     DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]        DATETIME     NULL, 
    CONSTRAINT [PK_TB_MonthlyRent] PRIMARY KEY ([MonthlyRentId]),
)

GO

CREATE INDEX [IX_TB_MonthlyRent_Search] ON [dbo].[TB_MonthlyRent] ([IDNO], [SEQNO], [Mode])

GO

CREATE INDEX [IX_TB_MonthlyRent_SearchByDate] ON [dbo].[TB_MonthlyRent] ([StartDate], [EndDate], [IDNO], [Mode])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租專案代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'ProjID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租專案名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'ProjNM'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租流水號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'SEQNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'模式：0:汽車月租;1:機車月租',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'Mode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'身份證',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'IDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汽車平日時數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'WorkDayHours'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汽車假日時數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'HolidayHours'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機車時數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'MotoTotalHours'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汽車平日費率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'WorkDayRateForCar'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汽車假日費率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'HoildayRateForCar'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機車平日費率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'WorkDayRateForMoto'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機車假日費率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'HoildayRateForMoto'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'起始日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'StartDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'結束日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = N'COLUMN',
    @level2name = N'EndDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'月租時數及費率表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MonthlyRent',
    @level2type = NULL,
    @level2name = NULL