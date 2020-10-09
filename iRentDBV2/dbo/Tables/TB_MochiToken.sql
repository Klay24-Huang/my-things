CREATE TABLE [dbo].[TB_MochiToken]
(
   [token]     VARCHAR (512) NOT NULL,
    [StartDate] DATETIME      NOT NULL,
    [EndDate]   DATETIME      NOT NULL,
    [MKTime]    DATETIME      DEFAULT DATEADD(HOUR,8,GETDATE()),
    [UPDTime]   DATETIME     DEFAULT DATEADD(HOUR,8,GETDATE()),
)
GO
CREATE NONCLUSTERED INDEX [IX_TB_MochiToken_Search]
    ON [dbo].[TB_MochiToken]([StartDate] ASC, [EndDate] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最近一次更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MochiToken', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MochiToken', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'有效迄日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MochiToken', @level2type = N'COLUMN', @level2name = N'EndDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'有效起日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MochiToken', @level2type = N'COLUMN', @level2name = N'StartDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車麻吉TOKEN', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MochiToken', @level2type = N'COLUMN', @level2name = N'token';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車麻吉token管制表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MochiToken';
