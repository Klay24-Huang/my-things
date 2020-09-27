CREATE TABLE [dbo].[TB_OrderExtendHistory]
(
	[HistoryID]        INT          IDENTITY (1, 1) NOT NULL,
    [order_number]     BIGINT       DEFAULT ((0)) NOT NULL,
    [StopTime]         DATETIME     NOT NULL,
    [ExtendStopTime]   DATETIME     NOT NULL,
    [booking_status]   TINYINT      DEFAULT ((6)) NOT NULL,
    [isForce]          TINYINT       DEFAULT ((0)) NOT NULL,
    [MKTime]           DATETIME     DEFAULT  (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    CONSTRAINT [PK_TB_OrderExtendHistory] PRIMARY KEY ([HistoryID]),
)
GO
CREATE NONCLUSTERED INDEX [IX_TB_OrderExtendHistory_order_number]
    ON [dbo].[TB_OrderExtendHistory]([order_number] ASC);
    GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寫入時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderExtendHistory', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後端強制延長(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderExtendHistory', @level2type = N'COLUMN', @level2name = N'isForce';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'延長後預計還車時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderExtendHistory', @level2type = N'COLUMN', @level2name = N'StopTime';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'延長用車記錄',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderExtendHistory',
    @level2type = NULL,
    @level2name = NULL