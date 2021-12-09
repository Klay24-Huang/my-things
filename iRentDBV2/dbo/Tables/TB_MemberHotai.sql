CREATE TABLE [dbo].[TB_MemberHotai] (
    [SeqNo]        INT            IDENTITY (1, 1) NOT NULL,
    [IDNO]         VARCHAR (20)   CONSTRAINT [DF_TB_MemberHotai_IDNO] DEFAULT ('') NOT NULL,
    [OneID]        VARCHAR (50)   CONSTRAINT [DF_TB_MemberHotai_OneID] DEFAULT ('') NOT NULL,
    [RefreshToken] VARCHAR (60)   CONSTRAINT [DF_TB_MemberHotai_RefreshToken] DEFAULT ('') NULL,
    [AccessToken]  VARCHAR (1000) CONSTRAINT [DF_TB_MemberHotai_AccessToken] DEFAULT ('') NULL,
    [isCancel]     TINYINT        CONSTRAINT [DF_TB_MemberHotai_isCancel] DEFAULT ((0)) NOT NULL,
    [CancelTime]   DATETIME       NULL,
    [A_PRGID]      VARCHAR (50)   CONSTRAINT [DF_TB_MemberHotai_A_PRGID] DEFAULT ('') NOT NULL,
    [A_USERID]     VARCHAR (20)   CONSTRAINT [DF_TB_MemberHotai_A_USERID] DEFAULT ('') NOT NULL,
    [A_SYSDT]      DATETIME       CONSTRAINT [DF_TB_MemberHotai_A_SYSDT] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [U_PRGID]      VARCHAR (50)   CONSTRAINT [DF_TB_MemberHotai_U_PRGID] DEFAULT ('') NOT NULL,
    [U_USERID]     VARCHAR (20)   CONSTRAINT [DF_TB_MemberHotai_U_USERID] DEFAULT ('') NOT NULL,
    [U_SYSDT]      DATETIME       CONSTRAINT [DF_TB_MemberHotai_U_SYSDT] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    CONSTRAINT [PK_TB_MemberHotai] PRIMARY KEY NONCLUSTERED ([SeqNo] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_TB_MemberHotai]
    ON [dbo].[TB_MemberHotai]([OneID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'U_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'U_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'U_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'A_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'A_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'A_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'解除綁定時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'CancelTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'解除綁定 0:未解除,1:已解除綁定', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'isCancel';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'和泰AccessToken', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'AccessToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'和泰RefreshToken', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'RefreshToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'和泰One ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'OneID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'身分證號(MemberData.MEMIDNO)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'IDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'和泰會員綁定紀錄檔流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotai', @level2type = N'COLUMN', @level2name = N'SeqNo';

