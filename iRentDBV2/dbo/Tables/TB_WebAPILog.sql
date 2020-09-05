CREATE TABLE [dbo].[TB_WebAPILog]
(
	[WebAPILogId] [bigint] IDENTITY(1,1) NOT NULL,
	[WebAPIId] [int] NOT NULL DEFAULT 0,
	[WebAPIInput] [varchar](4096) NOT NULL DEFAULT '',
	[WebAPIOutput] [nvarchar](max) NOT NULL DEFAULT N'',
	[DTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NOT NULL, 
    CONSTRAINT [PK_TB_WebAPILog] PRIMARY KEY ([WebAPILogId]),
)

GO

CREATE INDEX [IX_TB_WebAPILog_Search] ON [dbo].[TB_WebAPILog] ([WebAPIId], [DTime], [WebAPIInput])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'收到回傳時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WebAPILog',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'送出時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WebAPILog',
    @level2type = N'COLUMN',
    @level2name = N'DTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'回傳值',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WebAPILog',
    @level2type = N'COLUMN',
    @level2name = N'WebAPIOutput'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'送出值',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WebAPILog',
    @level2type = N'COLUMN',
    @level2name = N'WebAPIInput'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'對應TB_WebAPIList pk',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WebAPILog',
    @level2type = N'COLUMN',
    @level2name = N'WebAPIId'