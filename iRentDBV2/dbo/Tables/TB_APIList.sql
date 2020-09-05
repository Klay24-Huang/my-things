CREATE TABLE [dbo].[TB_APIList] (
    [APIID]    INT           IDENTITY (1, 1) NOT NULL,
    [APIName]  VARCHAR (50)  DEFAULT ('') NOT NULL,
    [APICName] NVARCHAR (20) CONSTRAINT [DF_TB_APIList201610_APICName] DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_TB_APIList] PRIMARY KEY CLUSTERED ([APIID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_APIList_Search]
    ON [dbo].[TB_APIList]([APIName] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'API名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_APIList', @level2type = N'COLUMN', @level2name = N'APIName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'API列表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_APIList';

