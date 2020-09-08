CREATE TABLE [dbo].[TB_CarTypeGroupConsist]
(
	[ConsistID] INT NOT NULL IDENTITY, 
    [CarTypeGroupID] INT NOT NULL DEFAULT 0, 
    [CarType] VARCHAR(10) NOT NULL DEFAULT '' ,
	[use_flag] [tinyint] NOT NULL DEFAULT (1),
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_CarTypeGroupConsist] PRIMARY KEY ([ConsistID]), 
)

GO

CREATE INDEX [IX_TB_CarTypeGroupConsist_Search] ON [dbo].[TB_CarTypeGroupConsist] ([use_flag], [CarTypeGroupID])

GO

CREATE INDEX [IX_TB_CarTypeGroupConsist_SearchCarType] ON [dbo].[TB_CarTypeGroupConsist] ([CarType], [CarTypeGroupID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'群組ID對應TB_CarTypeGroup PK',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarTypeGroupConsist',
    @level2type = N'COLUMN',
    @level2name = N'CarTypeGroupID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarTypeGroupConsist',
    @level2type = N'COLUMN',
    @level2name = N'CarType'
	GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarTypeGroupConsist', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarTypeGroupConsist', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_CarTypeGroupConsist', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型群組內容',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_CarTypeGroupConsist',
    @level2type = NULL,
    @level2name = NULL