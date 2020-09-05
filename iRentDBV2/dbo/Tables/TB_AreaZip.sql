CREATE TABLE [dbo].[TB_AreaZip] (
    [AreaID]   INT           IDENTITY (1, 1) NOT NULL,
    [CityID]   TINYINT       DEFAULT ((0)) NOT NULL,
    [AreaName] NVARCHAR (50) DEFAULT (N'') NOT NULL,
    [ZIPCode]  VARCHAR (3)   DEFAULT ('') NOT NULL,
    CONSTRAINT [PK_TB_AreaZip] PRIMARY KEY CLUSTERED ([AreaID] ASC)
);



GO

CREATE INDEX [IX_TB_AreaZip_Search] ON [dbo].[TB_AreaZip] ([CityID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'縣市代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AreaZip',
    @level2type = N'COLUMN',
    @level2name = N'CityID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'行政區',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AreaZip',
    @level2type = N'COLUMN',
    @level2name = N'AreaName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'郵遞區號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AreaZip',
    @level2type = N'COLUMN',
    @level2name = N'ZIPCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'行政區',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_AreaZip',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE TRIGGER TR_MainTain_AreaZipData
   ON  [dbo].[TB_AreaZip]
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
	   INSERT INTO TB_UPDDataWatchTable(AreaList)VALUES(@NowTime)
	END
	ELSE
	BEGIN
	   UPDATE TB_UPDDataWatchTable SET AreaList=@NowTime;
	END

END