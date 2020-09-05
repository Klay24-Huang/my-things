CREATE TABLE [dbo].[TB_APILog] (
    [APILogID]  BIGINT         IDENTITY (1, 1) NOT NULL,
    [APIID]     INT            DEFAULT ((0)) NOT NULL,
	[CLIENTIP]  VARCHAR(64)    DEFAULT ('') NOT NULL, 
    [ORDNO]     VARCHAR (50)   DEFAULT ('') NOT NULL,
    [APIInput]  VARCHAR (MAX) DEFAULT ('') NOT NULL,
    [MKTime]    DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    CONSTRAINT [PK_TB_APILog] PRIMARY KEY CLUSTERED ([APILogID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_APILog_Search]
    ON [dbo].[TB_APILog]([APIID] ASC, [MKTime] ASC, [ORDNO] ASC);


GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'呼叫api時的輸入值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_APILog', @level2type = N'COLUMN', @level2name = N'APIInput';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'執行哪筆訂單編號，若非有訂單則以空字串或身份證代入', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_APILog', @level2type = N'COLUMN', @level2name = N'ORDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'執行的功能，對應TB_APIList pk', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_APILog', @level2type = N'COLUMN', @level2name = N'APIID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'API呼叫記錄', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_APILog';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'執行者ip',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_APILog',
    @level2type = N'COLUMN',
    @level2name = N'CLIENTIP'