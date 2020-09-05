CREATE TABLE [dbo].[TB_ErrorMessageMutiLanguage] (
    [ErrCode]    VARCHAR (30)   DEFAULT ('') NOT NULL,
    [ErrMsg]     NVARCHAR (100) DEFAULT (N'') NOT NULL,
    [LanguageId] TINYINT        DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_TB_ErrorMessageMutiLanguage] PRIMARY KEY CLUSTERED ([ErrCode] ASC, [LanguageId] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'語系id，對應TB_Language PK', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorMessageMutiLanguage', @level2type = N'COLUMN', @level2name = N'LanguageId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'錯誤代碼子表(多語系)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_ErrorMessageMutiLanguage';

