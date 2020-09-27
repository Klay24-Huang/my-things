CREATE TABLE [dbo].[TB_Maintain_User]
(
	[Maintain_User_ID] INT         IDENTITY (1, 1) NOT NULL,
    [Account]          VARCHAR (50)   DEFAULT ('') NOT NULL,
    [Password]         VARCHAR (256)  DEFAULT ('') NOT NULL,
    [UserName]         NVARCHAR (20)  DEFAULT ('') NOT NULL,
    [ServerAccount]    TINYINT        DEFAULT ((0)) NOT NULL,
    [use_flag]         TINYINT        DEFAULT ((0)) NOT NULL,
    [MKTime]           DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]          DATETIME      NULL, 
    CONSTRAINT [PK_TB_Maintain_User] PRIMARY KEY ([Maintain_User_ID]),
)

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最近一次更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Maintain_User', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Maintain_User', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用(0:停用;1:啟用)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Maintain_User', @level2type = N'COLUMN', @level2name = N'use_flag';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否為Server專用帳號(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Maintain_User', @level2type = N'COLUMN', @level2name = N'ServerAccount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者姓名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Maintain_User', @level2type = N'COLUMN', @level2name = N'UserName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'密碼，以sha256加密', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Maintain_User', @level2type = N'COLUMN', @level2name = N'Password';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Maintain_User', @level2type = N'COLUMN', @level2name = N'Account';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'PK', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Maintain_User', @level2type = N'COLUMN', @level2name = N'Maintain_User_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'維護人員', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Maintain_User';

