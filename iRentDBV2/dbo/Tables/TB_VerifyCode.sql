CREATE TABLE [dbo].[TB_VerifyCode]
(
	[VerifyCodeID] [bigint] IDENTITY(1,1) NOT NULL,
	[IDNO] [varchar](10) NOT NULL DEFAULT '',
	[VerifyNum] [varchar](10) NOT NULL DEFAULT '',
	[Mobile] [varchar](10) NOT NULL DEFAULT '',
	[IsVerify] [tinyint] NOT NULL DEFAULT 0,
	[Mode] [tinyint] NOT NULL DEFAULT 2,
	[DeadLine] [datetime] NULL,
	[OrderNum] [bigint] NOT NULL DEFAULT 0,
	[SendTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()) , 
    CONSTRAINT [PK_TB_VerifyCode] PRIMARY KEY ([VerifyCodeID]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'帳號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_VerifyCode',
    @level2type = N'COLUMN',
    @level2name = N'IDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'驗證碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_VerifyCode',
    @level2type = N'COLUMN',
    @level2name = N'VerifyNum'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'手機',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_VerifyCode',
    @level2type = N'COLUMN',
    @level2name = N'Mobile'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否驗證 (0:未驗證;1:驗證)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_VerifyCode',
    @level2type = N'COLUMN',
    @level2name = N'IsVerify'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'模式(0:註冊時;1:忘記密碼;2:一次性開門)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_VerifyCode',
    @level2type = N'COLUMN',
    @level2name = N'Mode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'一次性開門允許時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_VerifyCode',
    @level2type = N'COLUMN',
    @level2name = N'DeadLine'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號，一次性開門才有值',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_VerifyCode',
    @level2type = N'COLUMN',
    @level2name = N'OrderNum'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'上次發送時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_VerifyCode',
    @level2type = N'COLUMN',
    @level2name = N'SendTime'
GO

CREATE INDEX [IX_TB_VerifyCode_Search] ON [dbo].[TB_VerifyCode] ([IDNO], [VerifyNum], [Mode], [OrderNum])
