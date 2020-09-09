CREATE TABLE [dbo].[TB_CarType]
(
	[CarType] VARCHAR(20) NOT NULL DEFAULT '', 
    [CarBrend] [varchar](20) NOT NULL DEFAULT '',
	[CarTypeName] [varchar](50) NOT NULL DEFAULT '',
	[Operator] INT NOT NULL DEFAULT 1,
	[isMoto] [tinyint] NOT NULL DEFAULT 0,
	[use_flag] [tinyint] NOT NULL DEFAULT 2,
	[MKTime] [datetime] NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())),
	[UPDTime] [datetime] NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())),
    [A_USER_ID] VARCHAR(50) NOT NULL DEFAULT '', 
    [U_USER_ID] VARCHAR(50) NOT NULL DEFAULT '', 
    CONSTRAINT [PK_TB_CarType] PRIMARY KEY ([CarType],[Operator]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarType',
    @level2type = N'COLUMN',
    @level2name = N'CarType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'廠牌',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarType',
    @level2type = N'COLUMN',
    @level2name = N'CarBrend'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarType',
    @level2type = N'COLUMN',
    @level2name = N'CarTypeName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否為機車(0:否;1:是)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarType',
    @level2type = N'COLUMN',
    @level2name = N'isMoto'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否啟用(0:停用;1:啟用;2:僅測試身份可視)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarType',
    @level2type = N'COLUMN',
    @level2name = N'use_flag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarType',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最近一次修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarType',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarType',
    @level2type = N'COLUMN',
    @level2name = N'A_USER_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarType',
    @level2type = N'COLUMN',
    @level2name = N'U_USER_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarType',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'業者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarType',
    @level2type = N'COLUMN',
    @level2name = N'Operator'