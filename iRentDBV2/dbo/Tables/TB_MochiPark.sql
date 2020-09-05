CREATE TABLE [dbo].[TB_MochiPark](
	[ParkId] [INT] IDENTITY(1,1) NOT NULL,
	[ParkingType] [TINYINT] NOT NULL DEFAULT (1),
	[Id] [VARCHAR](100) NOT NULL DEFAULT '',
	[Operator] [NVARCHAR](128) NOT NULL DEFAULT N'',
	[Name] [NVARCHAR](128) NOT NULL DEFAULT N'',
	[cooperation_state] [VARCHAR](50) NOT NULL DEFAULT '',
	[price] [INT] NOT NULL DEFAULT 0,
	[charge_mode] [VARCHAR](50) NOT NULL DEFAULT '',
	[lat] [DECIMAL](12, 6) NOT NULL DEFAULT 0.000000,
	[lng] [DECIMAL](12, 6) NOT NULL DEFAULT 0.000000,
	[open_status] [VARCHAR](50) NOT NULL DEFAULT '',
	[period] [NVARCHAR](50) NOT NULL DEFAULT '',
	[all_day_open] [TINYINT] NOT NULL DEFAULT 0,
	[detail] [NVARCHAR](128) NOT NULL DEFAULT N'',
	[city] [NVARCHAR](50) NOT NULL DEFAULT N'',
	[addr] [NVARCHAR](256) NOT NULL DEFAULT N'',
	[tel] [VARCHAR](20) NOT NULL DEFAULT '',
	[SettingPrice] [INT] NOT NULL DEFAULT 0,
	[use_flag] [TINYINT] NOT NULL DEFAULT 0,
	[StartTime] [DATETIME] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[CloseTime] [DATETIME] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[AddTime] [DATETIME] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[AddUser] [NVARCHAR](50) NOT NULL DEFAULT N'',
	[UpdateTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UpdateUser] [nvarchar](50) NOT NULL DEFAULT N'',
 CONSTRAINT [PK_TB_MochiPark] PRIMARY KEY CLUSTERED 
(
	[ParkId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車麻吉據點代碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'營運商' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'Operator'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'停車場名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'合作狀態: not_in_cooperated(非合作),coming_soon(即將合作麻吉付),in_cooperated(已合作麻吉付)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'cooperation_state'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'目前價格金額' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'price'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'計費方式，per_hour每小時' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'charge_mode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'緯度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'lat'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'經度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'lng'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'營業時間狀態 opening開放' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'open_status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'營業時間描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'period'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否全時段24小時營業(1:是;0:否)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'all_day_open'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'全時段的營業資訊' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'detail'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所屬縣市' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'city'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'addr'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'電話' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'tel'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'AddTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立者' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'AddUser'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'UpdateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新者' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark', @level2type=N'COLUMN',@level2name=N'UpdateUser'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車麻吉_特約停車場' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MochiPark'
GO


CREATE INDEX [IX_TB_MochiPark_Search] ON [dbo].[TB_MochiPark] ([Id], [Name], [open_status], [use_flag])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'停車場業者別',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MochiPark',
    @level2type = N'COLUMN',
    @level2name = N'parkingType'
GO
CREATE TRIGGER TR_MainTain_MochiPark
   ON  [dbo].[TB_MochiPark]
   AFTER  INSERT,DELETE,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here

	DECLARE @hasData TINYINT;
	DECLARE @NowTime DATETIME;
	SET @hasData=0;
	SET @NowTime=DATEADD(HOUR,8,GETDATE());
	SELECT @hasData =COUNT(1) FROM TB_UPDDataWatchTable;
	IF @hasData=0
	BEGIN
	   INSERT INTO TB_UPDDataWatchTable(Parking)VALUES(@NowTime)
	END
	ELSE
	BEGIN
	   UPDATE TB_UPDDataWatchTable SET Parking=@NowTime;
	END

END
GO