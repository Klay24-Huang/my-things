CREATE TABLE [dbo].[TB_MemberHotaiCard] (
    [HotaiCardID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [OneID]       VARCHAR (50)  CONSTRAINT [DF_TB_MemberHotaiCard_OneID] DEFAULT ('') NOT NULL,
    [IDNO]        VARCHAR (20)  CONSTRAINT [DF_TB_MemberHotaiCard_IDNO] DEFAULT ('') NOT NULL,
    [CardType]    NVARCHAR (20) CONSTRAINT [DF_TB_MemberHotaiCard_CardType] DEFAULT ('') NOT NULL,
    [BankDesc]    NVARCHAR (20) CONSTRAINT [DF_TB_MemberHotaiCard_BankDesc] DEFAULT ('') NOT NULL,
    [CardNo]      VARCHAR (50)  CONSTRAINT [DF_TB_MemberHotaiCard_CardNo] DEFAULT ('') NOT NULL,
    [CardToken]   VARCHAR (60)  CONSTRAINT [DF_TB_MemberHotaiCard_CardToken] DEFAULT ('') NOT NULL,
    [isCancel]    TINYINT       CONSTRAINT [DF_TB_MemberHotaiCard_isCancel] DEFAULT ((0)) NOT NULL,
    [CancelTime]  DATETIME      NULL,
    [A_PRGID]     VARCHAR (50)  CONSTRAINT [DF_TB_MemberHotaiCard_A_PRGID] DEFAULT ('') NOT NULL,
    [A_USERID]    VARCHAR (20)  CONSTRAINT [DF_TB_MemberHotaiCard_A_USERID] DEFAULT ('') NOT NULL,
    [A_SYSDT]     DATETIME      CONSTRAINT [DF_TB_MemberHotaiCard_A_SYSDT] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [U_PRGID]     VARCHAR (50)  CONSTRAINT [DF_TB_MemberHotaiCard_U_PRGID] DEFAULT ('') NOT NULL,
    [U_USERID]    VARCHAR (20)  CONSTRAINT [DF_TB_MemberHotaiCard_U_USERID] DEFAULT ('') NOT NULL,
    [U_SYSDT]     DATETIME      CONSTRAINT [DF_TB_MemberHotaiCard_U_SYSDT] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    CONSTRAINT [PK_TB_MemberHotaiCard] PRIMARY KEY NONCLUSTERED ([HotaiCardID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_TB_MemberHotaiCard]
    ON [dbo].[TB_MemberHotaiCard]([CardToken] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'U_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'U_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'U_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'A_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'A_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'A_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'解除綁定時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'CancelTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'解除綁定 0:未解除,1:已解除綁定', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'isCancel';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'信用卡密鑰', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'CardToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'隱碼卡號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'CardNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'發卡銀行，如中國信託', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'BankDesc';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'發卡機構，如visa', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'CardType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'身分證號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'IDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'和泰One ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'OneID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'和泰會員信用卡綁定紀錄檔流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberHotaiCard', @level2type = N'COLUMN', @level2name = N'HotaiCardID';

