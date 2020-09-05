CREATE TABLE [dbo].[TB_BookingStatusOfUser]
(
	[IDNO] VARCHAR(20) NOT NULL, 
    [NormalRentBookingNowCount] TINYINT NOT NULL DEFAULT 0 ,
	[AnyRentBookingNowCount] TINYINT NOT NULL DEFAULT 0 ,
	[MotorRentBookingNowCount] TINYINT NOT NULL DEFAULT 0 ,
	[NormalRentBookingTotalCount] INT NOT NULL DEFAULT 0 ,
	[AnyRentBookingTotalCount] INT NOT NULL DEFAULT 0 ,
	[MotorRentBookingTotalCount] INT NOT NULL DEFAULT 0 ,
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同站目前預約數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'NormalRentBookingNowCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'路邊目前預約數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'AnyRentBookingNowCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機車目前預約數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'MotorRentBookingNowCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同站累計總數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'NormalRentBookingTotalCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'路邊累計總數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'AnyRentBookingTotalCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機車累計總數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'MotorRentBookingTotalCount'