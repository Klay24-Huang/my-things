CREATE TABLE [dbo].[TB_PersonNotification]
(
    [NotificationID] BIGINT          IDENTITY (1, 1) NOT NULL,
    [OrderNum]       BIGINT            DEFAULT ((0)) NOT NULL,
    [IDNO]           VARCHAR (20)      DEFAULT ('') NOT NULL,
    [NType]          TINYINT           DEFAULT ((4)) NOT NULL,
    [UserName]       NVARCHAR (20)     DEFAULT (N'') NOT NULL,
    [UserToken]      VARCHAR (1024)    DEFAULT ('') NOT NULL,
    [STime]          DATETIME        NULL,
    [PushTime]       DATETIME        NULL,
    [Title]          NVARCHAR (500)   DEFAULT (N'') NOT NULL,
    [Message]        NVARCHAR (1500)  DEFAULT (N'') NOT NULL,
    [url]            VARCHAR (500)    DEFAULT ('') NOT NULL,
    [isSend]         TINYINT          DEFAULT ((2)) NOT NULL,
    [MKTime]         DATETIME         DEFAULT  (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [NewsID]         INT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_TB_PersonNotification] PRIMARY KEY CLUSTERED ([NotificationID] ASC)
)
GO
CREATE NONCLUSTERED INDEX [IX_PersonNotification_NewSearch]
    ON [dbo].TB_PersonNotification([PushTime] ASC, [STime] ASC, [isSend] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SendNotification_New]
    ON [dbo].TB_PersonNotification([OrderNum] ASC, [NType] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TB_PersonNotification_201611_NewsID]
    ON [dbo].TB_PersonNotification([NewsID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TB_PersonNotification_201611_IDNO]
    ON [dbo].TB_PersonNotification([IDNO] ASC, [isSend] ASC, [NType] ASC, [OrderNum] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0:未送出;1:已送出', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PersonNotification', @level2type = N'COLUMN', @level2name = N'isSend';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0 公告
9 前預約未還車
3 逾期未取車
1 取車通知
4 逾時通知
5 結帳15分鐘未還車
2 還車通知
16 大燈未關
15 電源未關
18 好友推薦', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PersonNotification', @level2type = N'COLUMN', @level2name = N'NType';


