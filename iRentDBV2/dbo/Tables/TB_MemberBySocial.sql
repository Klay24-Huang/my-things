CREATE TABLE [dbo].[TB_MemberBySocial] (
    [MEMIDNO]    VARCHAR (20) DEFAULT ('') NOT NULL,
    [SocialType] TINYINT      DEFAULT ((0)) NOT NULL,
    [SocialID]   VARCHAR (50) DEFAULT ('') NOT NULL,
    [MKTime]     DATETIME     DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]    DATETIME     DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    CONSTRAINT [PK_TB_MemberBySocial] PRIMARY KEY CLUSTERED ([SocialType] ASC, [MEMIDNO] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_MemberBySocial_Search]
    ON [dbo].[TB_MemberBySocial]([SocialID] ASC, [SocialType] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後一次修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberBySocial', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'綁定時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberBySocial', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'社群帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberBySocial', @level2type = N'COLUMN', @level2name = N'SocialID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'社群類別：0:Google;1:FB;2:APPLE ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberBySocial', @level2type = N'COLUMN', @level2name = N'SocialType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員ID對應會員資料表MEMIDNO', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberBySocial', @level2type = N'COLUMN', @level2name = N'MEMIDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員綁社群', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberBySocial';

