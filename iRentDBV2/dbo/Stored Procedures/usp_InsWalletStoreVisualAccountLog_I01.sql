/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_InsWalletStoreVisualAccountLog_I01
* 系    統 : IRENT
* 程式功能 : 電子錢包虛擬帳號產生紀錄檔
* 作    者 : AMBER
* 撰寫日期 : 20210928
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_InsWalletStoreVisualAccountLog_I01]
	@Amt				VARCHAR(15),   --交易金額
	@TrnActNo			VARCHAR(16),   --虛擬帳號
	@DueDate			VARCHAR(16),   --繳費期限
	@Token              VARCHAR(1024), --JWT TOKEN
    @IDNO               VARCHAR(10)  , --操作的會員帳號
	@LogID              BIGINT,         
	@ErrorCode          VARCHAR(6)        OUTPUT,     --回傳錯誤代碼
	@ErrorMsg           NVARCHAR(100)     OUTPUT,     --回傳錯誤訊息
	@SQLExceptionCode   VARCHAR(10)       OUTPUT,     --回傳sqlException代碼
	@SQLExceptionMsg    NVARCHAR(1000)    OUTPUT      --回傳sqlException訊
AS

DECLARE @Error INT
DECLARE @IsSystem TINYINT = 1
DECLARE @ErrorType TINYINT = 4
DECLARE @hasData INT
DECLARE @FunName VARCHAR(50) = 'usp_InsWalletStoreVisualAccountLog_I01'
DECLARE @NowTime DATETIME =dbo.Get_TWDATE()

/*初始設定*/
SET @Error=0;
SET	@ErrorCode  = '0000'	
SET	@ErrorMsg   = 'SUCCESS'	
SET	@SQLExceptionCode = ''		
SET	@SQLExceptionMsg = ''
SET @hasData=0
SET @IDNO       =ISNULL (@IDNO,'');

BEGIN TRY 	

IF @Token='' OR @IDNO='' OR @TrnActNo='' OR @DueDate='' OR @Amt=''
  BEGIN
  SET @Error=1;
  SET @ErrorCode='ERR900'
  END
		 
  --再次檢核token
 IF @Error=0
  BEGIN
    SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
    IF @hasData=0
    BEGIN
    SET @Error=1;
    SET @ErrorCode='ERR101';
    END
    ELSE
	BEGIN
	SET @hasData=0;
	SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
	END 
    IF @hasData=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR101';
	END
  END

 IF @Error=0
 BEGIN
	IF NOT EXISTS(SELECT 1 FROM TB_WalletStoreVisualAccountLog WITH(NOLOCK) WHERE TrnActNo=@TrnActNo)
	BEGIN
	INSERT INTO TB_WalletStoreVisualAccountLog (TrnActNo,IDNO,Amt,DueDate,ReCallFLG,MKTime,UPDTime) 
	  VALUES (@TrnActNo,@IDNO,@Amt,@DueDate,0,@NowTime,@NowTime)
	END
	ELSE
	BEGIN
	   SET @Error=1;
       SET @ErrorCode='ERR935'
	END
 END	

 IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
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
			INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
			VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
RETURN @Error
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsWalletStoreVisualAccountLog_I01';



