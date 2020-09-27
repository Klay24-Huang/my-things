CREATE TABLE [dbo].[TB_CarMachine]
(
	[CarMachineID] INT          IDENTITY (1, 1) NOT NULL,
    [MachineNo]    VARCHAR (10)  DEFAULT ('') NOT NULL,
    [MobileID]     INT           DEFAULT ((0)) NOT NULL,
    [UUID]         VARCHAR (30)   DEFAULT ('') NULL,
    [NFCIsOpen]    INT            DEFAULT ((0)) NOT NULL, 
    [deviceToken] VARCHAR(1024) NOT NULL DEFAULT '', 
    CONSTRAINT [PK_TB_CarMachine] PRIMARY KEY ([CarMachineID]),
)

GO

CREATE INDEX [IX_TB_CarMachine_SearchMachineNo] ON [dbo].[TB_CarMachine] ([MachineNo])

GO

CREATE INDEX [IX_TB_CarMachine_SearchDeviceToken] ON [dbo].[TB_CarMachine] ([MachineNo], [deviceToken])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'遠傳deviceToken',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarMachine',
    @level2type = N'COLUMN',
    @level2name = N'deviceToken'
GO

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'讀卡機電源是否啟動(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarMachine', @level2type = N'COLUMN', @level2name = N'NFCIsOpen';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手機id', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarMachine', @level2type = N'COLUMN', @level2name = N'MobileID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車機機碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarMachine', @level2type = N'COLUMN', @level2name = N'MachineNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'PK', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CarMachine', @level2type = N'COLUMN', @level2name = N'CarMachineID';