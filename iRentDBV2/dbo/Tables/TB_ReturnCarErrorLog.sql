CREATE TABLE [dbo].[TB_ReturnCarErrorLog]
(
	[ErrorLogId] BIGINT NOT NULL, 
    [OrderNo] BIGINT NOT NULL, 
    [CarError] VARCHAR(50) NOT NULL DEFAULT '', 
    [OptUser] VARCHAR(50) NOT NULL DEFAULT '', 
    [MKTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    CONSTRAINT [PK_TB_ReturnCarErrorLog] PRIMARY KEY ([ErrorLogId]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarErrorLog',
    @level2type = N'COLUMN',
    @level2name = N'OptUser'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarErrorLog',
    @level2type = N'COLUMN',
    @level2name = N'OrderNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'記錄強還時車機發生錯誤',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarErrorLog',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'錯誤代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarErrorLog',
    @level2type = N'COLUMN',
    @level2name = N'CarError'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'發生時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarErrorLog',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'