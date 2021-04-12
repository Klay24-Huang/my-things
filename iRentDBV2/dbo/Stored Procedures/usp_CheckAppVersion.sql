/****** Object:  StoredProcedure [dbo].[usp_CheckAppVersion]    Script Date: 2021/4/7 下午 02:35:00 ******/

/****************************************************************
** Name: [dbo].[usp_CheckAppVersion]
** Desc: 
**
** Return values: 0 成功 else 錯誤
** Return Recordset: 
**
** Called by: 
**
** Parameters:
** Input
** -----------

** 
**
** Output
** -----------
		
	@ErrorCode 				VARCHAR(6)			
	@ErrorCodeDesc			NVARCHAR(100)	
	@SQLExceptionCode		VARCHAR(10)				
	@SqlExceptionMsg		NVARCHAR(1000)	
**
** 
** Example
**------------
** DECLARE @Error               INT;
** DECLARE @ErrorCode 			VARCHAR(6);		
** DECLARE @ErrorMsg  			NVARCHAR(100);
** DECLARE @SQLExceptionCode	VARCHAR(10);		
** DECLARE @SQLExceptionMsg		NVARCHAR(1000);
** EXEC @Error=[dbo].[usp_CheckAppVersion]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Jet
** Date:2021/4/7 14:34:00 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2021/4/7 14:34:00    |  Jet|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_CheckAppVersion]
	@DeviceID               VARCHAR(128)          , --DeviceID
	@APPVersion             VARCHAR(10)			  , --app版號
	@APP                    TINYINT               , --0:ANDROID;1:iOS
	@LogID                  BIGINT                ,
	@MandatoryUPD			INT				OUTPUT,	--強制更新 1=強更，0=否
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;
DECLARE @iRentVersion VARCHAR(10);	--iRent版號
DECLARE @ChkTime DATETIME;			--強更時間
DECLARE @UseFlag VARCHAR(1);		--啟動區分: Y = 啟動, N = 不啟動

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_CheckAppVersion';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @DeviceID=ISNULL (@DeviceID,'');
SET @APP=ISNULL(@APP,2);
SET @APPVersion=ISNULL(@APPVersion,'');
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @MandatoryUPD=0;

BEGIN TRY
	IF @DeviceID='' OR @APPVersion='' 
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		SELECT TOP 1 @iRentVersion=iRentVersion,@UseFlag=UseFlag,@ChkTime=ChkTime
		FROM [dbo].[TB_ChkVersion] 
		WHERE DATEADD(HOUR,8,GETDATE()) >= ChkTime ORDER BY ChkTime DESC;

		IF @UseFlag='Y'
		BEGIN
			IF @APPVersion < @iRentVersion AND @NowTime >= @ChkTime
			BEGIN
				SET @MandatoryUPD=1;
			END
		END
	END

	--寫入錯誤訊息
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END
END TRY
BEGIN CATCH
	SET @Error=-1;
	SET @ErrorCode='ERR999';
	SET @ErrorMsg='我要寫錯誤訊息';
	SET @SQLExceptionCode=ERROR_NUMBER();
	SET @SQLExceptionMsg=ERROR_MESSAGE();
	IF @@TRANCOUNT > 0
	BEGIN
		print 'rolling back transaction' /* <- this is never printed */
		ROLLBACK TRAN
	END
	SET @IsSystem=1;
	SET @ErrorType=4;
	INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckAppVersion';
GO