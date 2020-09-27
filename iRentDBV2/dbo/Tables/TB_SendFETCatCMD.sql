CREATE TABLE [dbo].[TB_SendFETCatCMD]
(
	[requestId] VARCHAR(100) NOT NULL, 
    [method] VARCHAR(50) NOT NULL, 
	[CID]         VARCHAR(10) NOT NULL DEFAULT '', 
	[deviceToken] VARCHAR(512) NOT NULL DEFAULT '', 
    [SendParams] VARCHAR(1000) NOT NULL DEFAULT '', 
	[HttpStatus]  VARCHAR(50) NOT NULL DEFAULT '',
    [MKTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    CONSTRAINT [PK_TB_SendFETCatCMD] PRIMARY KEY ([requestId], [method])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'傳送的參數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SendFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'SendParams'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'指令',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SendFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'method'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'key值',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SendFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'requestId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'對遠傳cat車機下指令',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SendFETCatCMD',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車機的deviceToken',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SendFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'deviceToken'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車機編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SendFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'CID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'HTTP狀態',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_SendFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'HttpStatus'