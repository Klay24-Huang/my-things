CREATE TABLE [dbo].[TB_iRentStation]
(
	[StationID] VARCHAR(10) NOT NULL DEFAULT '', 
    [ManageStationID] VARCHAR(10) NOT NULL DEFAULT '', 
	[BlockGroupID] [int] NOT NULL DEFAULT 0,
	[Location] [nvarchar](128) NOT NULL DEFAULT N'',
	[Tel] [varchar](20) NOT NULL DEFAULT '',
	[ADDR] [nvarchar](100) NOT NULL DEFAULT '',
	[Latitude] [DECIMAL](9,6) NOT NULL DEFAULT 23.973875,
	[Longitude] [DECIMAL](9,6) NOT NULL DEFAULT 120.982025,
	[Content] [nvarchar](500) NOT NULL DEFAULT '',
    [ContentForAPP] [nvarchar](500) NOT NULL DEFAULT '',
	[UNICode] [VARCHAR](10) NOT NULL DEFAULT '',
    [CityID] [tinyint] NOT NULL DEFAULT 0,
    [AreaID] [int] NOT NULL DEFAULT 0,
    [IsRequiredForReturn] [TINYINT] NOT NULL DEFAULT 0,
    [CommonLendStation] VARCHAR(10) NOT NULL DEFAULT 'X088',
    [FCODE] VARCHAR(100) NOT NULL DEFAULT '' ,
    [SDate] DATETIME NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())),
    [EDate] DATETIME NOT NULL DEFAULT '2099-12-31 23:59:59',
	[IsNormalStation] [tinyint] NOT NULL DEFAULT 0,
    [AllowParkingNum] INT not null default 0,
    [NowOnlineNum] INT NOT NULL DEFAULT 0,
	[use_flag] [tinyint] NOT NULL DEFAULT 2,
    [Area] [varchar](10) NOT NULL DEFAULT '',
    [AlertEmail] [varchar](100) NULL,
	[MKTime] [datetime] NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())),
	[UPDTime] [datetime] NOT NULL DEFAULT (DATEADD(HOUR,8,GETDATE())),
    [A_USER_ID] VARCHAR(50) NOT NULL DEFAULT '', 
    [U_USER_ID] VARCHAR(50) NOT NULL DEFAULT '', 
    CONSTRAINT [PK_TB_iRentStation] PRIMARY KEY ([StationID], [ManageStationID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'據點代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'StationID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理據點代碼，對應TB_ManagerStation',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'ManageStationID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'對應的電子柵欄群組id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'BlockGroupID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'顯示名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'Location'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'電話',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'Tel'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'地址',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'ADDR'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'緯度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'Latitude'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'經度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'Longitude'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'其他備註',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'Content'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'縣市代碼（對應TB_City)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'CityID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否啟用(0:停用;1:啟用;2:僅測試身份可視)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'use_flag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最近一次修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'A_USER_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'U_USER_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'iRent據點',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE INDEX [IX_TB_iRentStation_Search] ON [dbo].[TB_iRentStation] ([Latitude], [Longitude], [use_flag], [IsNormalStation])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否為同站租還據點(0:否;1:是)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'IsNormalStation'
GO
CREATE TRIGGER TR_MainTain_iRentStation
   ON  [dbo].[TB_iRentStation]
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
	   INSERT INTO TB_UPDDataWatchTable(NormalRent)VALUES(@NowTime)
	END
	ELSE
	BEGIN
	   UPDATE TB_UPDDataWatchTable SET NormalRent=@NowTime;
	END

END
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車位數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'AllowParkingNum'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'統一編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'UNICode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'據點地塊行政區代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'AreaID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'目前上線數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'NowOnlineNum'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'迄日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'EDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'起日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'SDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'財務-部門代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'FCODE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'共同出車庫位',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'CommonLendStation'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'還車位置資訊必填',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'IsRequiredForReturn'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'據點描述（app顯示）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_iRentStation',
    @level2type = N'COLUMN',
    @level2name = N'ContentForAPP'