CREATE TABLE [dbo].[TB_BookingStatusOfUser]
(
	[IDNO] VARCHAR(20) NOT NULL, 
    [NormalRentBookingNowCount]    TINYINT NOT NULL DEFAULT 0 ,
	[AnyRentBookingNowCount]       TINYINT NOT NULL DEFAULT 0 ,
	[MotorRentBookingNowCount]     TINYINT NOT NULL DEFAULT 0 ,
	[RentNowActiveType]            TINYINT NOT NULL DEFAULT 5 ,
	[NowActiveOrderNum]            BIGINT NOT NULL DEFAULT 0,
	[NormalRentBookingCancelCount] INT NOT NULL DEFAULT 0 ,
	[AnyRentBookingCancelCount]    INT NOT NULL DEFAULT 0 ,
	[MotorRentBookingCancelCount]  INT NOT NULL DEFAULT 0 ,
	[NormalRentBookingFinishCount] INT NOT NULL DEFAULT 0 ,
	[AnyRentBookingFinishCount]    INT NOT NULL DEFAULT 0 ,
	[MotorRentBookingFinishCount]  INT NOT NULL DEFAULT 0 ,
	[NormalRentBookingTotalCount] INT NOT NULL DEFAULT 0 ,
	[AnyRentBookingTotalCount] INT NOT NULL DEFAULT 0 ,
	[MotorRentBookingTotalCount] INT NOT NULL DEFAULT 0, 
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_BookingStatusOfUser] PRIMARY KEY ([IDNO]) ,

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
    @value = N'同站累計取消總數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'NormalRentBookingCancelCount'
	GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'路邊累計取消總數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'AnyRentBookingCancelCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機車累計取消總數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'MotorRentBookingCancelCount'
	GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同站累計完成總數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'NormalRentBookingFinishCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'路邊完成總數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'AnyRentBookingFinishCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機車累計完成總數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'MotorRentBookingFinishCount'
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
GO


EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_BookingStatusOfUser', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_BookingStatusOfUser', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'目前進行中的類型(0:同站;3:路邊;4:機車;5:無)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = N'COLUMN',
    @level2name = N'RentNowActiveType'
GO

CREATE UNIQUE INDEX [IX_TB_BookingStatusOfUser_Search] ON [dbo].[TB_BookingStatusOfUser] ([IDNO], [NowActiveOrderNum])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預約管制表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingStatusOfUser',
    @level2type = NULL,
    @level2name = NULL