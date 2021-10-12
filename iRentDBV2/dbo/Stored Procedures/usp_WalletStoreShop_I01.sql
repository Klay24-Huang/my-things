/***********************************************************************************************
* Serve    : sqyhi03azt.database.windows.net
* Database : IRENT_V2T
* 程式名稱 : usp_WalletStoreShop_I01
* 系    統 : IRENT
* 程式功能 : 電子錢包超商銷帳編號產生紀錄
* 作    者 : AMBER
* 撰寫日期 : 20211007
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_WalletStoreShop_I01]
	@IDNO				VARCHAR(10),   --會員帳號
	@CvsIdentifier      VARCHAR(5),    --業者識別代碼
	@Token              VARCHAR(1024), --JWT TOKEN
	@LogID              BIGINT,         
    @PaymentId          VARCHAR(16)       OUTPUT,     --銷帳編號
	@ErrorCode          VARCHAR(6)        OUTPUT,     --回傳錯誤代碼
	@ErrorMsg           NVARCHAR(100)     OUTPUT,     --回傳錯誤訊息
	@SQLExceptionCode   VARCHAR(10)       OUTPUT,     --回傳sqlException代碼
	@SQLExceptionMsg    NVARCHAR(1000)    OUTPUT      --回傳sqlException訊
AS

DECLARE @Error INT
DECLARE @IsSystem TINYINT = 1
DECLARE @ErrorType TINYINT = 4
DECLARE @hasData INT
DECLARE @FunName VARCHAR(50) = 'usp_WalletStoreShop_I01'
DECLARE @NowTime DATETIME =[dbo].[GET_TWDATE]()
DECLARE @SEQNO  INT

/*初始設定*/
SET @Error=0;
SET	@ErrorCode  = '0000'	
SET	@ErrorMsg   = 'SUCCESS'	
SET	@SQLExceptionCode = ''		
SET	@SQLExceptionMsg = ''
SET @hasData=0
SET @IDNO       =ISNULL (@IDNO,'');
SET @PaymentId  =''
SET @SEQNO      =0 
	
BEGIN TRY 	

IF @Token='' OR @IDNO='' OR @CvsIdentifier=''
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
	  INSERT INTO TB_WalletStoreCvsPaymentId (IDNO,CvsIdentifier,MKTime,UPDTime) VALUES (@IDNO,@CvsIdentifier,@NowTime,@NowTime)	  
	  SET @SEQNO=@@IDENTITY
	  UPDATE TB_WalletStoreCvsPaymentId SET PaymentId =CvsIdentifier+RIGHT('0000000000000'+CAST(@SEQNO AS varchar(13)),13),UPDTime=[dbo].[GET_TWDATE]() WHERE SEQNO=@SEQNO;	  
	  SET @PaymentId=@CvsIdentifier+RIGHT('0000000000000'+CAST(@SEQNO AS varchar(13)),13)	
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
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_WalletStoreShop_I01';


