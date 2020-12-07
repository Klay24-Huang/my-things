CREATE TABLE [dbo].[TB_OrderParkingDetailByMachiOfSync]
(
	    [Id]               BIGINT       IDENTITY (1, 1) NOT NULL,
    [machi_id]         VARCHAR (50) NOT NULL,
    [order_number]     BIGINT       NOT NULL,
    [machi_parking_id] VARCHAR (50) NOT NULL,
    [Amount]           INT          NOT NULL,
    [Check_in]         DATETIME     NOT NULL,
    [Check_out]        DATETIME     NOT NULL,
    [CarNo]            VARCHAR (50)  DEFAULT ('') NOT NULL,
    [Conviction]       TINYINT       DEFAULT ((0)) NOT NULL,
    [paid_at]          DATETIME     NULL,
    [refund_amount]    FLOAT (53)    DEFAULT ((0)) NOT NULL,
    [MKTime]           DATETIME      DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_TB_OrderParkingDetailByMachiOfSync] PRIMARY KEY CLUSTERED ([Id] ASC, [machi_id] ASC)
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排程同步用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderParkingDetailByMachiOfSync',
    @level2type = NULL,
    @level2name = NULL