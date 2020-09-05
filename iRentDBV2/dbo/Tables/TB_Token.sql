CREATE TABLE [dbo].[TB_Token] (
    [MEMIDNO]       VARCHAR (20)  DEFAULT ('') NOT NULL,
    [Access_Token]  VARCHAR (64)  DEFAULT ('') NOT NULL,
    [Refrash_Token] VARCHAR (64)  DEFAULT ('') NOT NULL,
    [DeviceID]      VARCHAR (256) DEFAULT ('') NOT NULL,
    [APP]           TINYINT       DEFAULT ((0)) NOT NULL,
    [APPVersion]    VARCHAR (10)  DEFAULT ('') NOT NULL,
    [Rxpires_in]    DATETIME      DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [Refrash_Rxpires_in] DATETIME NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())), 
    CONSTRAINT [PK_TB_Token] PRIMARY KEY CLUSTERED ([MEMIDNO] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_Token_Refrash]
    ON [dbo].[TB_Token]([Refrash_Token] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TB_Token_Search]
    ON [dbo].[TB_Token]([Access_Token] ASC, [Rxpires_in] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'存取token有效日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Token', @level2type = N'COLUMN', @level2name = N'Rxpires_in';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'APP版號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Token', @level2type = N'COLUMN', @level2name = N'APPVersion';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0:Android;1:iOS', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Token', @level2type = N'COLUMN', @level2name = N'APP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手機機碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Token', @level2type = N'COLUMN', @level2name = N'DeviceID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新token', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Token', @level2type = N'COLUMN', @level2name = N'Refrash_Token';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'存取token', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Token', @level2type = N'COLUMN', @level2name = N'Access_Token';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Token', @level2type = N'COLUMN', @level2name = N'MEMIDNO';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Refrash Token的有效期限',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Token',
    @level2type = N'COLUMN',
    @level2name = N'Refrash_Rxpires_in'