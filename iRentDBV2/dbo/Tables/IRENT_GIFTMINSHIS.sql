CREATE TABLE [dbo].[IRENT_GIFTMINSHIS] (
    [A_PRGID]  VARCHAR (MAX) NOT NULL,
    [A_USERID] VARCHAR (10)  NOT NULL,
    [A_SYSDT]  DATETIME      NOT NULL,
    [U_PRGID]  VARCHAR (10)  NOT NULL,
    [U_USERID] VARCHAR (10)  NOT NULL,
    [U_SYSDT]  DATETIME      NOT NULL,
    [SEQNO]    INT           NOT NULL,
    [MEMIDNO]  VARCHAR (20)  NOT NULL,
    [CNTRNO]   VARCHAR (20)  NOT NULL,
    [MINS]     INT           NOT NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用分鐘數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSHIS', @level2type = N'COLUMN', @level2name = N'MINS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用合約', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSHIS', @level2type = N'COLUMN', @level2name = N'CNTRNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'贈與者身分證編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSHIS', @level2type = N'COLUMN', @level2name = N'MEMIDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSHIS', @level2type = N'COLUMN', @level2name = N'SEQNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSHIS', @level2type = N'COLUMN', @level2name = N'U_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSHIS', @level2type = N'COLUMN', @level2name = N'U_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSHIS', @level2type = N'COLUMN', @level2name = N'U_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSHIS', @level2type = N'COLUMN', @level2name = N'A_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSHIS', @level2type = N'COLUMN', @level2name = N'A_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSHIS', @level2type = N'COLUMN', @level2name = N'A_PRGID';

