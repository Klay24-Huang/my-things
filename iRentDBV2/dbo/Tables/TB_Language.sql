CREATE TABLE [dbo].[TB_Language] (
    [LanguageId]       INT           IDENTITY (1, 1) NOT NULL,
    [LanguageName]     NVARCHAR (20) DEFAULT (N'') NOT NULL,
    [LanguageShowName] NVARCHAR (50) DEFAULT (N'') NOT NULL,
    [isShow]           TINYINT       DEFAULT ((0)) NOT NULL,
    [AddUser]          NVARCHAR (20) DEFAULT (N'') NOT NULL,
    [MKTime]           DATETIME      DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [MaintainUser]     NVARCHAR (20) DEFAULT (N'') NOT NULL,
    [UPDTime]          DATETIME      DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    CONSTRAINT [PK_TB_Language] PRIMARY KEY CLUSTERED ([LanguageId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_Language_Search]
    ON [dbo].[TB_Language]([isShow] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後一次修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Language', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Language', @level2type = N'COLUMN', @level2name = N'MaintainUser';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Language', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Language', @level2type = N'COLUMN', @level2name = N'AddUser';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否顯示；0:不顯示（預設值）;1:顯示', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Language', @level2type = N'COLUMN', @level2name = N'isShow';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顯示的語系名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Language', @level2type = N'COLUMN', @level2name = N'LanguageShowName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'語系名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Language', @level2type = N'COLUMN', @level2name = N'LanguageName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'語系主檔', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Language';

