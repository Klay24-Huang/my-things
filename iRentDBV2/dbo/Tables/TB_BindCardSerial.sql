CREATE TABLE [dbo].[TB_BindCardSerial]
(
	[IDNO] VARCHAR(20) NOT NULL, 
    [NowCount] INT NOT NULL DEFAULT 0, 
    [BindType] TINYINT NOT NULL DEFAULT 0, 
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_BindCardSerial] PRIMARY KEY ([IDNO], [BindType], [NowCount]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'綁定模式：0:信用卡;1:銀行帳戶;',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BindCardSerial',
    @level2type = N'COLUMN',
    @level2name = N'BindType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'此帳號目前綁定數量',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BindCardSerial',
    @level2type = N'COLUMN',
    @level2name = N'NowCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'身份證或居留證',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BindCardSerial',
    @level2type = N'COLUMN',
    @level2name = N'IDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BindCardSerial',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最近一次更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BindCardSerial',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO

CREATE INDEX [IX_TB_BindCardSerial_SearchByTime] ON [dbo].[TB_BindCardSerial] ([MKTime])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'帳號綁定信用卡數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BindCardSerial',
    @level2type = NULL,
    @level2name = NULL