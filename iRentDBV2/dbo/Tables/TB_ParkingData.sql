CREATE TABLE [dbo].[TB_ParkingData](
	[ParkingID] [int] IDENTITY(1,1) NOT NULL,
	[ParkingType] [TINYINT] NOT NULL DEFAULT (0),
	[ParkingName] [nvarchar](100) NOT NULL DEFAULT (N'') ,
	[ParkingAddress] [nvarchar](150) NOT NULL DEFAULT (N'') ,
	[ParkingLng] [decimal](9, 6) NOT NULL DEFAULT (0.000000) ,
	[ParkingLat] [decimal](9, 6) NOT NULL DEFAULT (0.000000),
	[OpenTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[CloseTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[use_flag] [tinyint] NOT NULL DEFAULT (1),
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL,
	[last_opt] [nvarchar](10) NOT NULL DEFAULT (N'') ,
 CONSTRAINT [PK_TB_ParkingData] PRIMARY KEY CLUSTERED 
(
	[ParkingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'停車場地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ParkingData', @level2type=N'COLUMN',@level2name=N'ParkingAddress'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'經度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ParkingData', @level2type=N'COLUMN',@level2name=N'ParkingLng'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'緯度' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ParkingData', @level2type=N'COLUMN',@level2name=N'ParkingLat'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'開放時間（起）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ParkingData', @level2type=N'COLUMN',@level2name=N'OpenTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'開放時間(迄)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ParkingData', @level2type=N'COLUMN',@level2name=N'CloseTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ParkingData', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ParkingData', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ParkingData', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後建立（修改）者' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ParkingData', @level2type=N'COLUMN',@level2name=N'last_opt'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'停車場資料' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_ParkingData'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'停車場業者別',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ParkingData',
    @level2type = N'COLUMN',
    @level2name = N'ParkingType'
	GO
CREATE INDEX [IX_TB_ParkingData_Search_OF_APP] ON [dbo].[TB_ParkingData] ([ParkingLat], [ParkingLng], [use_flag])

GO

CREATE INDEX [IX_TB_ParkingData_Search_OF_BE] ON [dbo].[TB_ParkingData] ([ParkingName])

GO
CREATE TRIGGER TR_MainTain_ParkingData
   ON  [dbo].[TB_ParkingData]
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
