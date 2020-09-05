CREATE TABLE [dbo].[TB_OrderDetail]
(
	[order_number] [int] NOT NULL,
	[already_lend_car] [tinyint] NOT NULL DEFAULT 0,
	[already_return_car] [tinyint] NOT NULL DEFAULT 0,
	[extend_stop_time] [datetime] NULL,
	[force_extend_stop_time] [datetime] NULL,
	[final_start_time] [datetime] NULL,
	[final_stop_time] [datetime] NULL,
	[start_door_time] [datetime] NULL,
	[end_door_time] [datetime] NULL,
	[transaction_no] [nvarchar](100) NOT NULL DEFAULT N'',
	[final_price] [int] NOT NULL DEFAULT 0,
	[pure_price] [int] NOT NULL DEFAULT 0,
	[mileage_price] [int] NOT NULL DEFAULT 0,
	[Insurance_price][int] NOT NULL DEFAULT 0,
	[fine_price] [int] NOT NULL DEFAULT 0,
	[fine_interval] [int] NOT NULL DEFAULT 0,
	[fine_rate] [int] NOT NULL DEFAULT 0,
	[gift_point] [int] NOT NULL DEFAULT 0,
	[Etag] [int] NOT NULL DEFAULT 0,
	[already_payment] [int] NOT NULL DEFAULT 0,
	[start_mile] [float] NOT NULL DEFAULT 0,
	[end_mile] [float] NOT NULL DEFAULT 0,
	[trade_status] [int] NOT NULL DEFAULT 0,
	[parkingFee] [int] NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_TB_OrderDetail] PRIMARY KEY ([order_number]),
	
)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'預約延長時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'extend_stop_time'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'預約強制延長時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'force_extend_stop_time'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'實際出車時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'final_start_time'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'實際還車時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'final_stop_time'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'第一次開門時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'start_door_time'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'第一次關門時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'end_door_time'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'金流交易序號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'transaction_no'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'實際租金(租金小計+出車里程油資+延遲還車費用)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'final_price'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'租金小計' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'pure_price'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出車里程油資' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'mileage_price'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'延遲還車費用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'fine_price'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'延遲還車時間(秒)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'fine_interval'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'延遲還車計用計算依據(原始一日車輛租金)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'fine_rate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用會員點數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'gift_point'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'實際付款金額' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'already_payment'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'取車里程' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'start_mile'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'還車里程' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'end_mile'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'交易狀態' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderDetail', @level2type=N'COLUMN',@level2name=N'trade_status'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否出車(0:否;1:是)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderDetail',
    @level2type = N'COLUMN',
    @level2name = N'already_lend_car'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否還車(0:否;1:是)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderDetail',
    @level2type = N'COLUMN',
    @level2name = N'already_return_car'
GO

CREATE INDEX [IX_TB_OrderDetail_Search_PayStatus] ON [dbo].[TB_OrderDetail] ([trade_status], [transaction_no])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'安心服務費用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderDetail',
    @level2type = N'COLUMN',
    @level2name = N'Insurance_price'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單明細檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderDetail',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE INDEX [IX_TB_OrderDetail_Search_StartEnd] ON [dbo].[TB_OrderDetail] ([final_start_time], [final_stop_time])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'ETag費用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderDetail',
    @level2type = N'COLUMN',
    @level2name = N'Etag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'特約停車場費用（車麻吉）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderDetail',
    @level2type = N'COLUMN',
    @level2name = N'parkingFee'