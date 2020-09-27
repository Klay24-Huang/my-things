CREATE TABLE [dbo].[TB_BookingControlProjectGroup]
(
	[BookingControlProjectGroupID] INT NOT NULL, 
    [BookingControlProjectGroupName] NVARCHAR(100) NOT NULL DEFAULT (N''), 
	[BookingStartTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [BookingStopTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,9,GETDATE()), 
	[AllowBooking] TINYINT NOT NULL DEFAULT(1),
    [UseMinHours] FLOAT NOT NULL DEFAULT 1, 
    [UseMaxHours] FLOAT NOT NULL DEFAULT 70, 
	[RuleMessageOfBooking] NVARCHAR(100) NOT NULL DEFAULT(N''),
	[RuleMessageOfMinHours] NVARCHAR(100) NOT NULL DEFAULT(N''),
	[RuleMessageOfMaxHours] NVARCHAR(100) NOT NULL DEFAULT(N''),
	[use_flag] [tinyint] NOT NULL DEFAULT (1),
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_BookingControlProjectGroup] PRIMARY KEY ([BookingControlProjectGroupID]) 
)
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否允許預約(0:不允許;1:允許;2:僅允許取還車在BookingStartTime與BookingStopTime區間內）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroup',
    @level2type = N'COLUMN',
    @level2name = N'AllowBooking'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預計取車',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroup',
    @level2type = N'COLUMN',
    @level2name = N'BookingStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預計還車',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroup',
    @level2type = N'COLUMN',
    @level2name = N'BookingStopTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最低使用時數，一天以10小時計',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroup',
    @level2type = N'COLUMN',
    @level2name = 'UseMinHours'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最大使用時數，一天以10小時計',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroup',
    @level2type = N'COLUMN',
    @level2name = N'UseMaxHours'
		GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_BookingControlProjectGroup', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_BookingControlProjectGroup', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_BookingControlProjectGroup', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'禁止預約時的錯誤訊息',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroup',
    @level2type = N'COLUMN',
    @level2name = N'RuleMessageOfBooking'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'小於最低預約時數時的錯誤訊息',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroup',
    @level2type = N'COLUMN',
    @level2name = N'RuleMessageOfMinHours'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'大於最大允許時間時的錯誤訊息',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroup',
    @level2type = N'COLUMN',
    @level2name = N'RuleMessageOfMaxHours'
GO



CREATE INDEX [IX_TB_BookingControlProjectGroup_Search] ON [dbo].[TB_BookingControlProjectGroup] (BookingControlProjectGroupID, [BookingStartTime], [BookingStopTime], [AllowBooking], [use_flag])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預約控制專案群組',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControlProjectGroup',
    @level2type = NULL,
    @level2name = NULL