/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_WalletStoredErrorHandle_U01
* 系    統 : IRENT
* 程式功能 : 更新錢包儲值失敗處理狀態
* 作    者 : Amber
* 撰寫日期 : 20211224
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_WalletStoredErrorHandle_U01]
    @ProcessStatus          INT,
	@TransId                VARCHAR(30),    --台新交易編號
	@PRGName                VARCHAR(50),    --程式名稱
	@ReturnCode             VARCHAR(4),     --回傳代碼
	@Message                NVARCHAR(200),  --回傳訊息
	@ExceptionData          NVARCHAR(500),  --回傳異常錯誤訊息
    @LogID			        BIGINT,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS

DECLARE @Error INT
DECLARE @IsSystem TINYINT = 1
DECLARE @ErrorType TINYINT = 4
DECLARE @FunName VARCHAR(50) = 'usp_WalletStoredErrorHandle_U01'
DECLARE @NowTime DATETIME =[dbo].[GET_TWDATE]()

/*初始設定*/
SET @Error=0;
SET	@ErrorCode  = '0000'	
SET	@ErrorMsg   = 'SUCCESS'	
SET	@SQLExceptionCode = ''		
SET	@SQLExceptionMsg = ''
SET @LogID=ISNULL(@LogID,0)

BEGIN TRY 	
	
	UPDATE TB_TaishinWalletStoreValueErrorLog 
	SET ProcessStatus=@ProcessStatus,UPDPRGID=@PRGName,UPDTime=@NowTime,[Message]=@Message,ExceptionData=@ExceptionData,
	ReturnCode=@ReturnCode
	WHERE TransId=@TransId;
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
			INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
			VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
RETURN @Error
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_WalletStoredErrorHandle_U01';



