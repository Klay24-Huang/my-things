/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_WalletStoredErrorHandle_Q01
* 系    統 : IRENT
* 程式功能 : 撈取錢包儲值失敗清單
* 作    者 : Amber
* 撰寫日期 : 20211224
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_WalletStoredErrorHandle_Q01]
    @PRGName                VARCHAR(50)           ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息       
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @NowTime DATETIME;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_WalletStoredErrorHandle_Q01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @NowTime=dbo.GET_TWDATE()
SET @PRGName=ISNULL(@PRGName,'')

BEGIN TRY
  SELECT * INTO #TB_ErrList
  FROM  TB_TaishinWalletStoreValueErrorLog WITH(NOLOCK)
  WHERE ProcessStatus=0;

  UPDATE E
  SET ProcessStatus=4, --處理中
  UPDPRGID=@PRGName,
  UPDUser=@PRGName,
  UPDTime=@NowTime
  FROM TB_TaishinWalletStoreValueErrorLog E
  JOIN #TB_ErrList L ON E.SEQNO=L.SEQNO;

  SELECT * FROM #TB_ErrList ;
  DROP TABLE #TB_ErrList;

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
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,0,@IsSystem);
END CATCH
RETURN @Error
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_WalletStoredErrorHandle_Q01';

