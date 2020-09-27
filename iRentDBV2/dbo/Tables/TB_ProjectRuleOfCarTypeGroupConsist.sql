CREATE TABLE [dbo].[TB_ProjectRuleOfCarTypeGroupConsist]
(
	[ProjectRuleOfCarTypeGroupConsistID] INT NOT NULL IDENTITY, 
    [ProjectRuleOfCarTypeGroupID] INT NOT NULL DEFAULT 0, 
    [CarType] VARCHAR(10) NOT NULL DEFAULT '' ,
	[use_flag] [tinyint] NOT NULL DEFAULT (1),
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案控制車型群組包含車型',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRuleOfCarTypeGroupConsist',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案控制車型群組包含車型群組，對應TB_ProjectRuleOfCarTypeGroup PK',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRuleOfCarTypeGroupConsist',
    @level2type = N'COLUMN',
    @level2name = N'ProjectRuleOfCarTypeGroupID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectRuleOfCarTypeGroupConsist',
    @level2type = N'COLUMN',
    @level2name = N'CarType'
		GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectRuleOfCarTypeGroupConsist', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectRuleOfCarTypeGroupConsist', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectRuleOfCarTypeGroupConsist', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO