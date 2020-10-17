CREATE TABLE [dbo].[TB_FeedBackKind]
(
	[FeedBackKindId] INT NOT NULL, 
    [Star] INT NOT NULL DEFAULT 1, 
    [Descript] NVARCHAR(50) NOT NULL DEFAULT N'' ,
    [use_flag] [tinyint] NOT NULL DEFAULT (1),
    [last_Opt] [NVARCHAR](10) NOT NULL DEFAULT 'SYS',
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_FeedBackKind] PRIMARY KEY ([FeedBackKindId]), 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'星星數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FeedBackKind',
    @level2type = N'COLUMN',
    @level2name = N'Star'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'描述',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FeedBackKind',
    @level2type = N'COLUMN',
    @level2name = N'Descript'
    GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_FeedBackKind', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_FeedBackKind', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_FeedBackKind', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FeedBackKind',
    @level2type = N'COLUMN',
    @level2name = N'last_Opt'
GO

CREATE INDEX [IX_TB_FeedBackKind_Search] ON [dbo].[TB_FeedBackKind] ([Star], [use_flag])
