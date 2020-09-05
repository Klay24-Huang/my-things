CREATE TABLE [dbo].[TB_ProjectStation]
(
	[PROJID] [varchar](10) NOT NULL DEFAULT (''), 
	[IOType] [char](1) NOT NULL,
	[StationID] [varchar](10) NOT NULL,
		[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL,
    CONSTRAINT [PK_TB_ProjectStation] PRIMARY KEY ([PROJID],[IOType],[StationID]) ,
)
GO


EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'優惠專案ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectStation', @level2type=N'COLUMN',@level2name=N'ProjID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出還車代碼(I:還車;O:出車)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectStation', @level2type=N'COLUMN',@level2name=N'IOType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'據點ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectStation', @level2type=N'COLUMN',@level2name=N'StationID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectStation', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ProjectStation', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO


CREATE INDEX [IX_TB_ProjectStation_Search] ON [dbo].[TB_ProjectStation] ([IOType], [PROJID], [StationID])
GO
CREATE NONCLUSTERED INDEX IX_SearchForVW_GetAllMotorAnyRentData
ON [dbo].[TB_ProjectStation] ([IOType],[StationID])

GO
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案可出還站',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ProjectStation',
    @level2type = NULL,
    @level2name = NULL