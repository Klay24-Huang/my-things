CREATE TABLE [dbo].[TB_ErrorLog] (
    [ERRID]        BIGINT          IDENTITY (1, 1) NOT NULL,
    [FunName]      VARCHAR (50)    DEFAULT ('') NOT NULL,
    [ErrorCode]    VARCHAR (30)    DEFAULT ('') NOT NULL,
    [ErrType]      TINYINT         DEFAULT ((0)) NOT NULL,
    [SQLErrorCode] INT             DEFAULT ((0)) NOT NULL,
    [SQLErrorDesc] NVARCHAR (1000) DEFAULT (N'') NOT NULL,
    [LogID]        BIGINT          DEFAULT ((0)) NOT NULL,
    [IsSystem]     TINYINT         DEFAULT ((0)) NOT NULL,
    [DTime]        DATETIME        DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    CONSTRAINT [PK_TB_ErrorLog] PRIMARY KEY CLUSTERED ([ERRID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_ErrorLog_Search_By_Date]
    ON [dbo].[TB_ErrorLog]([DTime] ASC, [ErrType] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TB_ErrorLog_Search]
    ON [dbo].[TB_ErrorLog]([LogID] ASC, [ErrType] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否為系統錯誤：0:否;1:是', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorLog', @level2type = N'COLUMN', @level2name = N'IsSystem';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'對應TB_APILOG的PK', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorLog', @level2type = N'COLUMN', @level2name = N'LogID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'SQL Exception Message', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorLog', @level2type = N'COLUMN', @level2name = N'SQLErrorDesc';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'SQL Exception CODE', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorLog', @level2type = N'COLUMN', @level2name = N'SQLErrorCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'錯誤類型：0:一般訊息;1:金流回傳;2:短租回傳;3:車機回傳;4:執行SQL發生錯誤', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorLog', @level2type = N'COLUMN', @level2name = N'ErrType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'錯誤代碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorLog', @level2type = N'COLUMN', @level2name = N'ErrorCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'執行的功能名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorLog', @level2type = N'COLUMN', @level2name = N'FunName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'錯誤Log', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorLog';

