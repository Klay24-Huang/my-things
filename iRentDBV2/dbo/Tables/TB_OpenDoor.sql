CREATE TABLE [dbo].[TB_OpenDoor]
(
	 [OpenDoorID] BIGINT   IDENTITY (1, 1) NOT NULL,
    [OrderNo]    BIGINT    DEFAULT ((0)) NOT NULL,
    [DeadLine]   DATETIME  NOT NULL,
    [VerifyCode] VARCHAR(8) DEFAULT '' NOT NULL,
    [IsVerify]   TINYINT  DEFAULT 0 NOT NULL,
    [nowStatus]  TINYINT   DEFAULT ((0)) NOT NULL,
    [MKTime]     DATETIME  DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]      DATETIME NULL,
    CONSTRAINT [PK_TB_OpenDoor] PRIMARY KEY CLUSTERED ([OpenDoorID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_OpenDoor_Sync]
    ON [dbo].[TB_OpenDoor]([nowStatus] ASC, [MKTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TB_OpenDoor_OrderNo]
    ON [dbo].[TB_OpenDoor]([OrderNo], [nowStatus], [DeadLine], [IsVerify]);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OpenDoor', @level2type = N'COLUMN', @level2name = 'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'目前狀態（0:開門;1:關門)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OpenDoor', @level2type = N'COLUMN', @level2name = N'nowStatus';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易序號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OpenDoor', @level2type = N'COLUMN', @level2name = N'OrderNo';

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'一次性開門',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OpenDoor',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'使用期限',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OpenDoor',
    @level2type = N'COLUMN',
    @level2name = N'DeadLine'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'驗證碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OpenDoor',
    @level2type = N'COLUMN',
    @level2name = N'VerifyCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否通過驗證',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OpenDoor',
    @level2type = N'COLUMN',
    @level2name = N'IsVerify'