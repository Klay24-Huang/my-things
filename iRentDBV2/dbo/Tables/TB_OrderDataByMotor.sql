CREATE TABLE [dbo].[TB_OrderDataByMotor]
(
    [OrderNo]      BIGINT          DEFAULT ((0)) NOT NULL,
    [P_LBA]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [P_RBA]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [P_TBA]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [P_MBA]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [P_lon]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [P_lat]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [R_LBA]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [R_RBA]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [R_TBA]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [R_MBA]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [R_lon]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [R_lat]        DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [Reward]       INT             DEFAULT ((0)) NULL,
    [MKTime]       DATETIME        DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL, 
    [UPDTime]      DATETIME       NULL, 
    CONSTRAINT [PK_TB_OrderDataByMotor] PRIMARY KEY ([OrderNo]),
)
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'還車時緯度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'R_lat';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'還車時經度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'R_lon';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'還車時核心電池電量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'R_MBA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'還車時平均電量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'R_TBA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'還車時右電池電量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'R_RBA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'還車時左電池電量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'R_LBA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取車時緯度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'P_lat';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取車時經度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'P_lon';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取車時核心電池電量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'P_MBA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取車時平均電量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'P_TBA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取車時右電池電量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'P_RBA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取車時左電池電量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'P_LBA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderDataByMotor', @level2type = N'COLUMN', @level2name = N'OrderNo';

