CREATE TABLE [dbo].[TB_OrderParkingDetailByMachi]
(
	    [Id]               BIGINT       IDENTITY (1, 1) NOT NULL,
    [machi_id]         VARCHAR (50)  DEFAULT ('') NOT NULL,
    [order_number]     BIGINT        DEFAULT ((0)) NOT NULL,
    [machi_parking_id] VARCHAR (50) NOT NULL,
    [Amount]           INT           DEFAULT ((0)) NOT NULL,
    [Check_in]         DATETIME     NOT NULL,
    [Check_out]        DATETIME     NOT NULL,
    [MKTime]           DATETIME     DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL, 
    CONSTRAINT [PK_TB_OrderParkingDetailByMachi] PRIMARY KEY ([Id]),
)
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingDetailByMachi', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'出場時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingDetailByMachi', @level2type = N'COLUMN', @level2name = N'Check_out';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'入場時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingDetailByMachi', @level2type = N'COLUMN', @level2name = N'Check_in';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代付停車費', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingDetailByMachi', @level2type = N'COLUMN', @level2name = N'Amount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車麻吉停車場編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingDetailByMachi', @level2type = N'COLUMN', @level2name = N'machi_parking_id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'iRent訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingDetailByMachi', @level2type = N'COLUMN', @level2name = N'order_number';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車麻吉訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingDetailByMachi', @level2type = N'COLUMN', @level2name = N'machi_id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車麻吉訂單', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingDetailByMachi';


