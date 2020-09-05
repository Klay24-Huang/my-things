CREATE TABLE [dbo].[TB_WebAPIList]
(
		[WebAPIId] [int] IDENTITY(1,1) NOT NULL,
	[WebAPIURL] [varchar](512) NOT NULL DEFAULT '',
	[WebAPIName] [varchar](50) NOT NULL DEFAULT '',
	[WebAPICName] [nvarchar](50) NOT NULL DEFAULT N'',
	[WebAPIDescript] [nvarchar](256) NOT NULL DEFAULT N'',
	 [MKTime]    DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    CONSTRAINT [PK_TB_WebAPIList] PRIMARY KEY ([WebAPIId]),
)

GO

CREATE INDEX [IX_TB_WebAPIList_Search] ON [dbo].[TB_WebAPIList] ([WebAPIId], [WebAPIURL])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'連結網址',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WebAPIList',
    @level2type = N'COLUMN',
    @level2name = N'WebAPIURL'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WebAPIList',
    @level2type = N'COLUMN',
    @level2name = N'WebAPIName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能中文名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WebAPIList',
    @level2type = N'COLUMN',
    @level2name = N'WebAPICName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'功能描述',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WebAPIList',
    @level2type = N'COLUMN',
    @level2name = N'WebAPIDescript'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WebAPIList',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'第三方WebAPI的功能列表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_WebAPIList',
    @level2type = NULL,
    @level2name = NULL