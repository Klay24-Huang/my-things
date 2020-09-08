CREATE TABLE [dbo].[TB_CarTypeGroup]
(
	[CarTypeGroupID] INT NOT NULL IDENTITY, 
	[CarTypeGroupCode] VARCHAR(10) NOT NULL DEFAULT '',
    [CarTypeName] NVARCHAR(100) NOT NULL DEFAULT '', 
	[use_flag] [tinyint] NOT NULL DEFAULT (1),
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_CarTypeGroup] PRIMARY KEY ([CarTypeGroupID]),
)

GO

CREATE INDEX [IX_TB_CarTypeGroup_Search] ON [dbo].[TB_CarTypeGroup] ( [use_flag], [CarTypeName],[CarTypeGroupCode])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型群組名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarTypeGroup',
    @level2type = N'COLUMN',
    @level2name = N'CarTypeName'
GO

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarTypeGroup', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarTypeGroup', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarTypeGroup', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型群組',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarTypeGroup',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型簡碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarTypeGroup',
    @level2type = N'COLUMN',
    @level2name = N'CarTypeGroupCode'