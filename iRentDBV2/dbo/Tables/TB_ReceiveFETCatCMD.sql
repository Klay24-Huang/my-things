CREATE TABLE [dbo].[TB_ReceiveFETCatCMD]
(
	[requestId] VARCHAR(100) NOT NULL, 
    [method] VARCHAR(50) NOT NULL, 
	[CmdReply]    VARCHAR(20) NOT NULL DEFAULT '', 
	[CID]         VARCHAR(10) NOT NULL DEFAULT '', 
	[deviceToken] VARCHAR(512) NOT NULL DEFAULT '', 
    [receiveRawData] VARCHAR(1000) NOT NULL DEFAULT '', 
    [MKTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    CONSTRAINT [PK_TB_ReceiveFETCatCMD] PRIMARY KEY ([requestId], [method])
)
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'接收的原始資料',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReceiveFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'receiveRawData'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'指令',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReceiveFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'method'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'key值',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReceiveFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'requestId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'對遠傳cat車機下指令',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReceiveFETCatCMD',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車機的deviceToken',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReceiveFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'deviceToken'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車機編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReceiveFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'CID'

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'回覆的狀態，Okay表成功',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReceiveFETCatCMD',
    @level2type = N'COLUMN',
    @level2name = N'CmdReply'