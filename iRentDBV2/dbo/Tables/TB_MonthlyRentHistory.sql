CREATE TABLE [dbo].[TB_MonthlyRentHistory]
(
	[HistoryID]         BIGINT       IDENTITY (1, 1) NOT NULL,
    [OrderNo]          BIGINT          DEFAULT ((0)) NOT NULL,
    [IDNO]              VARCHAR (20)   DEFAULT ('') NOT NULL,
    [StartDate]         DATETIME     NOT NULL,
    [EndDate]           DATETIME     NOT NULL,
    [UseWorkDayHours]   FLOAT (53)   DEFAULT ((0)) NOT NULL,
    [UseHolidayHours]   FLOAT (53)   DEFAULT ((0)) NOT NULL,
    [UseMotoTotalHours] FLOAT (53)   DEFAULT ((0)) NOT NULL,
    [isSend]            TINYINT      DEFAULT ((0)) NOT NULL,
    [MonthlyRentId]    BIGINT       DEFAULT ((0)) NOT NULL,
    [MKTime]            DATETIME    DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    CONSTRAINT [PK_TB_MonthlyRentHistory] PRIMARY KEY CLUSTERED ([HistoryID] ASC)
)
GO
CREATE NONCLUSTERED INDEX [IX_TB_MonthlyRentHistory_SearchOrder]
    ON [dbo].[TB_MonthlyRentHistory]([IDNO] ASC, [isSend] ASC, [OrderNo] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TB_MonthlyRentHistory_SearchByPK]
    ON [dbo].[TB_MonthlyRentHistory]([MonthlyRentId] ASC);
    
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MonthlyRentHistory', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0:未送出;1:已發送', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MonthlyRentHistory', @level2type = N'COLUMN', @level2name = N'isSend';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'機車總時數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MonthlyRentHistory', @level2type = N'COLUMN', @level2name = N'UseMotoTotalHours';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'汽車假日時數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MonthlyRentHistory', @level2type = N'COLUMN', @level2name = N'UseHolidayHours';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'汽車平日時數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MonthlyRentHistory', @level2type = N'COLUMN', @level2name = N'UseWorkDayHours';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂閱迄', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MonthlyRentHistory', @level2type = N'COLUMN', @level2name = N'EndDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂閱起始', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MonthlyRentHistory', @level2type = N'COLUMN', @level2name = N'StartDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'身份證', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MonthlyRentHistory', @level2type = N'COLUMN', @level2name = N'IDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MonthlyRentHistory', @level2type = N'COLUMN', @level2name = N'OrderNo';

