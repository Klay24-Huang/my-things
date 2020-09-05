CREATE TABLE [dbo].[TB_ManagerStation]
(
[StationID] VARCHAR(10) NOT NULL DEFAULT '', 
[StationName] NVARCHAR(100) NOT NULL DEFAULT N'', 
	[MKTime] [datetime] NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())),
	[UPDTime] [datetime] NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())),
    [A_USER_ID] VARCHAR(50) NOT NULL DEFAULT '', 
    [U_USER_ID] VARCHAR(50) NOT NULL DEFAULT '', 
    CONSTRAINT [PK_TB_ManagerStation] PRIMARY KEY ([StationID])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管轄據點代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ManagerStation',
    @level2type = N'COLUMN',
    @level2name = N'StationID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管轄據點名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ManagerStation',
    @level2type = N'COLUMN',
    @level2name = N'StationName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ManagerStation',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最近一次修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ManagerStation',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ManagerStation',
    @level2type = N'COLUMN',
    @level2name = N'A_USER_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ManagerStation',
    @level2type = N'COLUMN',
    @level2name = N'U_USER_ID'
GO
