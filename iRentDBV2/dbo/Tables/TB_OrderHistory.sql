CREATE TABLE [dbo].[TB_OrderHistory]
(
	[LogID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [OrderNum]       BIGINT         DEFAULT ((0)) NOT NULL,
    [booking_status] TINYINT        DEFAULT ((0)) NOT NULL,
    [car_mgt_status] TINYINT        DEFAULT ((0)) NOT NULL,
    [cancel_status]  TINYINT        DEFAULT ((0)) NOT NULL,
	[modify_status]  TINYINT        DEFAULT 0 NOT NULL,
    [Descript]       NVARCHAR (100) DEFAULT (N'') NOT NULL,
    [MKTime]         DATETIME       DEFAULT DATEADD(HOUR,8,GETDATE()) NOT NULL,
    CONSTRAINT [PK_TB_OrderHistory] PRIMARY KEY ([LogID]),
)

GO

CREATE INDEX [IX_TB_OrderHistory_Search] ON [dbo].[TB_OrderHistory] ([OrderNum], [MKTime])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderHistory',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'狀態描述',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderHistory',
    @level2type = N'COLUMN',
    @level2name = N'Descript'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderHistory',
    @level2type = N'COLUMN',
    @level2name = N'OrderNum'
GO
	EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改後的取消狀態', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderHistory', @level2type = N'COLUMN', @level2name = N'cancel_status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改後的取還車狀態', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderHistory', @level2type = N'COLUMN', @level2name = N'car_mgt_status';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改後的預約單狀態', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderHistory', @level2type = N'COLUMN', @level2name = N'booking_status';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單歷史記錄',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderHistory',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE INDEX [IX_TB_OrderHistory_USearch] ON [dbo].[TB_OrderHistory] ([OrderNum], [car_mgt_status], [cancel_status], [booking_status], [modify_status])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改合約',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderHistory',
    @level2type = N'COLUMN',
    @level2name = N'modify_status'