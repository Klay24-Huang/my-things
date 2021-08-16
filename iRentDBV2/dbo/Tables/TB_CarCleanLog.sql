CREATE TABLE [dbo].[TB_CarCleanLog]
(
	[OrderNum]      BIGINT          NOT NULL,
    [UserID]        VARCHAR (20)     DEFAULT ('') NOT NULL,
    [outsideClean]  TINYINT          DEFAULT ((0)) NOT NULL,
    [insideClean]   TINYINT          DEFAULT ((0)) NOT NULL,
    [rescue]        TINYINT          DEFAULT ((0)) NOT NULL,
    [dispatch]      TINYINT          DEFAULT ((0)) NOT NULL,
    [Anydispatch]   TINYINT          DEFAULT ((0)) NOT NULL,
    [Maintenance]   TINYINT          DEFAULT ((0)) NOT NULL,
    [OrderStatus]   TINYINT          DEFAULT ((0)) NOT NULL,
    [remark]        NVARCHAR (1024)  DEFAULT ('') NOT NULL,
    [incarPic]      VARCHAR (MAX)    DEFAULT ('') NOT NULL,
    [incarPicType]  VARCHAR (50)     DEFAULT ('') NOT NULL,
    [outcarPic]     VARCHAR (MAX)    DEFAULT ('') NOT NULL,
    [outcarPicType] VARCHAR (50)     DEFAULT ('') NULL,
    [bookingStart]  DATETIME        NULL,
    [bookingEnd]    DATETIME        NULL,
    [lastCleanTime] DATETIME        NULL,
    [lastRentTimes] INT             DEFAULT ((0)) NOT NULL,
    [MKTime]        DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPTime] DATETIME NULL, 
    CONSTRAINT [PK_TB_CarCleanLog] PRIMARY KEY CLUSTERED ([OrderNum] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_CarCleanLog_Search]
    ON [dbo].[TB_CarCleanLog]([UserID] ASC, [MKTime] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'image type', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'outcarPicType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車外照', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'outcarPic';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'image type', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'incarPicType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車內照', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'incarPic';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'remark';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0:未取車;1:已取車;2:已還車;3:已取消;4:已逾時(取車)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'OrderStatus';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'路邊租還調度0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'Anydispatch';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'非路邊租還調度0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'dispatch';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車輛救援0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'rescue';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'內裝清潔0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'insideClean';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車外清潔(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'outsideClean';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'UserID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂單編號，對應TB_BookingMain', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog', @level2type = N'COLUMN', @level2name = N'OrderNum';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車輛清潔記錄檔', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanLog';

