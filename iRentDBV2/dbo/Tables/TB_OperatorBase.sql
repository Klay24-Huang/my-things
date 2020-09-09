CREATE TABLE [dbo].[TB_OperatorBase]
(
	[OperatorID] INT NOT NULL IDENTITY, 
	[OperatorICon] NVARCHAR(300) NOT NULL DEFAULT N'',
    [OperatorName] NVARCHAR(100) NOT NULL DEFAULT N'', 
	[Score] FLOAT NOT NULL DEFAULT 5.0,
	[use_flag] [tinyint] NOT NULL DEFAULT (1),
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_OperatorBase] PRIMARY KEY ([OperatorID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'業者名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperatorBase',
    @level2type = N'COLUMN',
    @level2name = N'OperatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'業者主檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperatorBase',
    @level2type = NULL,
    @level2name = NULL
	GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OperatorBase', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OperatorBase', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OperatorBase', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

CREATE INDEX [IX_TB_OperatorBase_Search] ON [dbo].[TB_OperatorBase] ([OperatorID], [use_flag])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'累積分數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperatorBase',
    @level2type = N'COLUMN',
    @level2name = N'Score'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'業者圖示',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OperatorBase',
    @level2type = N'COLUMN',
    @level2name = N'OperatorICon'