CREATE TABLE [dbo].[TB_BookingControlProjectGroupConsist]
(
	[BookingControlProjectGroupConsistID] INT NOT NULL, 
    [BookingControlProjectGroupID] INT NOT NULL DEFAULT 0, 
    [ProjID] VARCHAR(10) NOT NULL DEFAULT '', 
		[use_flag] [tinyint] NOT NULL DEFAULT (1),
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_BookingControlProjectGroupConsist] PRIMARY KEY ([BookingControlProjectGroupConsistID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroupConsist',
    @level2type = N'COLUMN',
    @level2name = N'ProjID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'群組代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroupConsist',
    @level2type = N'COLUMN',
    @level2name = N'BookingControlProjectGroupID'
			GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_BookingControlProjectGroupConsist', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_BookingControlProjectGroupConsist', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_BookingControlProjectGroupConsist', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

CREATE INDEX [IX_TB_BookingControlProjectGroupConsist_Search] ON [dbo].[TB_BookingControlProjectGroupConsist] ([BookingControlProjectGroupID], [use_flag])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預約控制群組內含專案',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroupConsist',
    @level2type = NULL,
    @level2name = NULL