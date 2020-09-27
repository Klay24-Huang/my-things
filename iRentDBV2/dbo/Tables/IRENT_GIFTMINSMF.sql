CREATE TABLE [dbo].[IRENT_GIFTMINSMF] (
    [A_PRGID]  VARCHAR (10)  NOT NULL,
    [A_USERID] VARCHAR (10)  NOT NULL,
    [A_SYSDT]  DATETIME      NOT NULL,
    [U_PRGID]  VARCHAR (10)  NOT NULL,
    [U_USERID] VARCHAR (10)  NOT NULL,
    [U_SYSDT]  DATETIME      NOT NULL,
    [SEQNO]    INT           IDENTITY (1, 1) NOT NULL,
    [MEMIDNO]  VARCHAR (20)  CONSTRAINT [DF_IRENT_GIFTMINSMF_MEMIDNO] DEFAULT ('') NOT NULL,
    [MEMRFNBR] INT           NOT NULL,
    [GIFTNAME] NVARCHAR (30) CONSTRAINT [DF_IRENT_GIFTMINSMF_GIFTNAME] DEFAULT ('') NOT NULL,
    [GIFTMINS] INT           CONSTRAINT [DF_IRENT_GIFTMINSMF_GIFTMINS] DEFAULT ((0)) NOT NULL,
    [SDATE]    DATETIME      NOT NULL,
    [EDATE]    DATETIME      NOT NULL,
    [RCVFLG]   VARCHAR (1)   DEFAULT ('N') NULL,
    [COUPONID] VARCHAR (20)  CONSTRAINT [DF_IRENT_GIFTMINSMF_COUPONID] DEFAULT ('') NOT NULL,
    [GIFTTYPE] VARCHAR (5)   DEFAULT ('') NOT NULL,
    CONSTRAINT [PK_IRENT_GIFTMINSMF] PRIMARY KEY CLUSTERED ([SEQNO] ASC) WITH (FILLFACTOR = 90)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'有效日期(迄)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'EDATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'有效日期(起)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'SDATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'贈送分鐘數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'GIFTMINS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'贈送名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'GIFTNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'MEMRFNBR';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'贈與者身分證編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'MEMIDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'SEQNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'U_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'U_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'U_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'A_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'A_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'IRENT_GIFTMINSMF', @level2type = N'COLUMN', @level2name = N'A_PRGID';

