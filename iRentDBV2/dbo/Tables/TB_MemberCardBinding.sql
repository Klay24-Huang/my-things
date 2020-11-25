CREATE TABLE [dbo].[TB_MemberCardBinding] (
    [BindCardId]      BIGINT        IDENTITY (1, 1) NOT NULL,
    [IDNO]            VARCHAR (20)  CONSTRAINT [DF_TB_MemberCardBinding_IDNO] DEFAULT ('') NOT NULL,
    [BankNo]          VARCHAR (50)  CONSTRAINT [DF_Table_1_CardToken3] DEFAULT ('') NOT NULL,
    [CardNumber]      VARCHAR (50)  CONSTRAINT [DF_Table_1_CardToken2] DEFAULT ('') NOT NULL,
    [CardName]        NVARCHAR (60) CONSTRAINT [DF_Table_1_CardToken1] DEFAULT ('') NOT NULL,
    [AvailableAmount] VARCHAR (50)  CONSTRAINT [DF_Table_1_CardToken1_1] DEFAULT ('') NOT NULL,
    [CardToken]       VARCHAR (60)  CONSTRAINT [DF_TB_MemberCardBinding_CardToken] DEFAULT ('') NOT NULL,
    [IsValid]         TINYINT       CONSTRAINT [DF_Table_1_IsSuccess] DEFAULT ((0)) NOT NULL,
    [MKTime]          DATETIME      CONSTRAINT [DF_TB_MemberCardBinding_MKTime] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [UPDTime]         DATETIME      NULL,
    CONSTRAINT [PK_TB_MemberCardBinding] PRIMARY KEY CLUSTERED ([BindCardId] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最近一次更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCardBinding', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'新增時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCardBinding', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否生效', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCardBinding', @level2type = N'COLUMN', @level2name = N'IsValid';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'信用卡密鑰', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCardBinding', @level2type = N'COLUMN', @level2name = N'CardToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'可用額度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCardBinding', @level2type = N'COLUMN', @level2name = N'AvailableAmount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'信用卡名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCardBinding', @level2type = N'COLUMN', @level2name = N'CardName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'卡號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCardBinding', @level2type = N'COLUMN', @level2name = N'CardNumber';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'銀行代碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCardBinding', @level2type = N'COLUMN', @level2name = N'BankNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'身分證號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCardBinding', @level2type = N'COLUMN', @level2name = N'IDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N' 綁卡序號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCardBinding', @level2type = N'COLUMN', @level2name = N'BindCardId';

