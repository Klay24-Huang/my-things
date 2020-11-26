CREATE TABLE [dbo].[TB_iRentClearControl]
(
	 [CarNo]                  VARCHAR (50) DEFAULT ('') NOT NULL,
    [RentCount]              INT          DEFAULT ((0)) NOT NULL,
    [UnCleanCount]           INT          DEFAULT ((0)) NOT NULL,
    [LastRentOrderNo]        BIGINT       DEFAULT ((0)) NOT NULL,
    [LastRentTime]           DATETIME     NULL,
    [LastCleanOrderNo]       BIGINT       DEFAULT ((0)) NOT NULL,
    [LastCleanTime]          DATETIME     NULL,
    [LastMaintenanceOrderNo] INT          DEFAULT ((0)) NOT NULL,
    [LastMaintenanceMilage]  FLOAT (53)   DEFAULT ((0.0)) NOT NULL,
    [LastMaintenanceTime]    DATETIME     NULL,
    [MKTime]                 DATETIME     DEFAULT (getdate()) NOT NULL,
    [UPDTime]                DATETIME     DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_TB_iRentClearControl] PRIMARY KEY CLUSTERED ([CarNo] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_iRentClearControl_SearchMaintenance]
    ON [dbo].[TB_iRentClearControl]([LastMaintenanceMilage] ASC, [LastMaintenanceTime] ASC, [LastMaintenanceOrderNo] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TB_iRentClearControl_SearchClean]
    ON [dbo].[TB_iRentClearControl]([LastCleanTime] ASC, [UnCleanCount] ASC, [LastCleanOrderNo] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TB_iRentClearControl_SearchRent]
    ON [dbo].[TB_iRentClearControl]([LastRentTime] ASC, [RentCount] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後一次修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次定期保養時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'LastMaintenanceTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次定期保養里程', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'LastMaintenanceMilage';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次定期保養訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'LastMaintenanceOrderNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次清潔完成時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'LastCleanTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次清潔訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'LastCleanOrderNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次出租時間還車時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'LastRentTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次出租訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'LastRentOrderNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次清潔後未清潔次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'UnCleanCount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上次清潔後出租次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'RentCount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl', @level2type = N'COLUMN', @level2name = N'CarNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'整備人員車輛出租數記錄', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_iRentClearControl';

