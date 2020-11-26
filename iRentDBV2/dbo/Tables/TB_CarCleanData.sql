CREATE TABLE [dbo].[TB_CarCleanData]
(
	 [CarNo]         VARCHAR (50)  DEFAULT ('') NOT NULL,
    [lastCleanTime] DATETIME      DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [lastRentTimes] INT           DEFAULT ((0)) NOT NULL,
    [lastOpt]       NVARCHAR (50) DEFAULT (N'') NOT NULL,
    [MKTime]        DATETIME      DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]       DATETIME      NULL,
    CONSTRAINT [PK_TB_CarCleanData] PRIMARY KEY CLUSTERED ([CarNo] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanData', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後清潔者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanData', @level2type = N'COLUMN', @level2name = N'lastOpt';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'清潔後此車出租次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanData', @level2type = N'COLUMN', @level2name = N'lastRentTimes';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'清潔完成時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanData', @level2type = N'COLUMN', @level2name = N'lastCleanTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車號，對應carinfo的車號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanData', @level2type = N'COLUMN', @level2name = N'CarNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'清潔總表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarCleanData';

