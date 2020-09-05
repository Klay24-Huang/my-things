CREATE TABLE [dbo].[TB_ErrorMessageList] (
    [ErrCode]        VARCHAR (30)    DEFAULT ('') NOT NULL,
    [ErrMsg]         NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [ErrType]        TINYINT         DEFAULT ((0)) NOT NULL,
    [needSendMail]   TINYINT         DEFAULT ((0)) NOT NULL,
    [ErrDescription] NVARCHAR (1000) DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_TB_ErrorMessageList] PRIMARY KEY CLUSTERED ([ErrCode] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_ErrorMessageList_needSendMail]
    ON [dbo].[TB_ErrorMessageList]([ErrType] ASC, [needSendMail] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顯示此錯誤訊息情境描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorMessageList', @level2type = N'COLUMN', @level2name = N'ErrDescription';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否需發信;0:否;1:是', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorMessageList', @level2type = N'COLUMN', @level2name = N'needSendMail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'錯誤類型：0:一般訊息;1:金流回傳;2:短租回傳;3:車機回傳;4:執行SQL發生錯誤', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorMessageList', @level2type = N'COLUMN', @level2name = N'ErrType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'錯誤代碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorMessageList', @level2type = N'COLUMN', @level2name = N'ErrCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'錯誤訊息列表(主表）預設為正體中文', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorMessageList';

