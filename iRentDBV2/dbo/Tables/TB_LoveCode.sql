CREATE TABLE [dbo].[TB_LoveCode]
(
	[Id] INT NOT NULL IDENTITY, 
    [LoveName] NVARCHAR(50) NOT NULL DEFAULT N'', 
    [LoveCode] VARCHAR(20) NOT NULL DEFAULT '', 
    [LoveShortName] NVARCHAR(50) NOT NULL DEFAULT N'', 
    [UNICode] VARCHAR(10) NOT NULL DEFAULT '', 
		 [MKTime]    DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
	  [UPDTime]    DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL, 
    CONSTRAINT [PK_TB_LoveCode] PRIMARY KEY ([Id]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'愛心捐贈碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_LoveCode',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE TRIGGER TR_MainTain_LoveCode
   ON  [dbo].[TB_LoveCode]
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
	   INSERT INTO TB_UPDDataWatchTable(LoveCode)VALUES(@NowTime)
	END
	ELSE
	BEGIN
	   UPDATE TB_UPDDataWatchTable SET LoveCode=@NowTime;
	END

END
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機關名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_LoveCode',
    @level2type = N'COLUMN',
    @level2name = N'LoveName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'捐贈碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_LoveCode',
    @level2type = N'COLUMN',
    @level2name = N'LoveCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機關簡稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_LoveCode',
    @level2type = N'COLUMN',
    @level2name = N'LoveShortName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機關統編',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_LoveCode',
    @level2type = N'COLUMN',
    @level2name = N'UNICode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_LoveCode',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最近一次更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_LoveCode',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'