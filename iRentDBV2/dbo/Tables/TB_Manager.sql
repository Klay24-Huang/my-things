CREATE TABLE [dbo].[TB_Manager]
(
	 [Account]   VARCHAR (50)  DEFAULT ('') NOT NULL,
    [UserName]  NVARCHAR (50) DEFAULT (N'') NOT NULL,
    [UserPwd]   VARCHAR(50)   DEFAULT ('') NOT NULL,
    [Operator] INT DEFAULT (0) NOT NULL,
    [UserGroup] varchar(50) NOT NULL,
    [UserGroupID] INT  DEFAULT 0 NOT NULL,
     [SD] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [ED] DATETIME NOT NULL DEFAULT '2099-12-31 23:59:59', 
    [ClientIP]  VARCHAR (256) DEFAULT ('') NOT NULL,
    [MKTime]    DATETIME      DEFAULT (getdate()) NOT NULL,
    [UPDTime]   DATETIME      NULL,
    CONSTRAINT [PK_TB_Manager1] PRIMARY KEY CLUSTERED ([Account] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IX_TB_Manager_Search]
    ON [dbo].[TB_Manager]([UserGroupID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最近一次更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Manager', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Manager', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登入ip', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Manager', @level2type = N'COLUMN', @level2name = N'ClientIP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'群組', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Manager', @level2type = N'COLUMN', @level2name = 'UserGroupID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'姓名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Manager', @level2type = N'COLUMN', @level2name = N'UserName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Manager', @level2type = N'COLUMN', @level2name = N'Account';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台使用者資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Manager';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'密碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Manager',
    @level2type = N'COLUMN',
    @level2name = N'UserPwd'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'業者id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Manager',
    @level2type = N'COLUMN',
    @level2name = N'Operator'