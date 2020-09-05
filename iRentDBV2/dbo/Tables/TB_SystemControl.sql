CREATE TABLE [dbo].[TB_SystemControl] (
    [ControlID]        INT            IDENTITY (1, 1) NOT NULL,
    [StopServiceStart] DATETIME       NOT NULL,
    [StopServiceEnd]   DATETIME       NOT NULL,
    [StopMessage]      NVARCHAR (300) DEFAULT (N'') NOT NULL,
    [ControlType]      TINYINT        DEFAULT ((0)) NOT NULL,
    [AddUser]          VARCHAR (50)   DEFAULT ('') NOT NULL,
    [MKTime]           DATETIME       DEFAULT (getdate()) NOT NULL,
    [MaintainUser]     VARCHAR (50)   DEFAULT ('') NOT NULL,
    [UPDTime]          DATETIME       NULL,
    CONSTRAINT [PK_TB_SystemControl] PRIMARY KEY CLUSTERED ([ControlID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_SystemControl_Search]
    ON [dbo].[TB_SystemControl]([StopServiceStart] ASC, [StopServiceEnd] ASC, [ControlType] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後一次修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_SystemControl', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後一次修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_SystemControl', @level2type = N'COLUMN', @level2name = N'MaintainUser';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_SystemControl', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_SystemControl', @level2type = N'COLUMN', @level2name = N'AddUser';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'停止類型：0:禁止登入;1:禁止租車', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_SystemControl', @level2type = N'COLUMN', @level2name = N'ControlType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'露出訊息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_SystemControl', @level2type = N'COLUMN', @level2name = N'StopMessage';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'結束時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_SystemControl', @level2type = N'COLUMN', @level2name = N'StopServiceEnd';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'開始時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_SystemControl', @level2type = N'COLUMN', @level2name = N'StopServiceStart';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'系統設定', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_SystemControl';

