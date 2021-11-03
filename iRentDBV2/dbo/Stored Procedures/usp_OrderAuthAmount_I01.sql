/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_OrderAuthAmount_I01
* 系    統 : IRENT
* 程式功能 : 寫入預授權金額檔
* 作    者 : AMBER
* 撰寫日期 : 20211027
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_OrderAuthAmount_I01]
    @MerchantTradNo         VARCHAR(50)           , --廠商訂單編號
    @BankTradeNo            VARCHAR(50)           , --銀行交易序號
    @IDNO                   VARCHAR(10)           , --操作的會員帳號
	@CardType               INT                   , --信用卡類別(0:和泰 ; 1:台新)
	@AuthType               INT                   , --授權類別 (1:預約; 2:訂金; 3:取車; 4:延長用車; 5:逾時; 6:欠費; 7:還車)
    @final_price            INT                   , --授權金額
	@PRGName                VARCHAR(50)           , --程式名稱
	@OrderNo                INT                   , --訂單編號
    @Status                 INT                   , --處理狀態
	@Token                  VARCHAR(1024)         , --JWT TOKEN
	@LogID                  BIGINT                , --執行的api log
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT
DECLARE @IsSystem TINYINT
DECLARE @FunName VARCHAR(50)
DECLARE @ErrorType TINYINT
DECLARE @hasData INT
DECLARE @NowTime DATETIME

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_OrderAuthAmount_I01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime         =dbo.GET_TWDATE();
SET @OrderNo         =ISNULL (@OrderNo,'0')
SET @IDNO            =ISNULL (@IDNO,'');
SET @MerchantTradNo  =ISNULL (@MerchantTradNo,'');
SET @BankTradeNo     =ISNULL (@BankTradeNo,'');
SET @Token           =ISNULL (@Token,'');

BEGIN TRY

  IF @IDNO=''
  BEGIN
   SET @Error=1
   SET @ErrorCode='ERR900'
  END
  		 
  --0.再次檢核token
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
   DECLARE @PRGID VARCHAR(10) = '0'
   SELECT @PRGID = Convert(VARCHAR(10),APIID) FROM TB_APIList WITH(NOLOCK) WHERE  APIName = @PRGName

   BEGIN
   INSERT INTO TB_OrderAuthAmount(order_number,MerchantTradeNo,BankTradeNo,IDNO,CardType,AuthType,final_price,[Status],A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT)	
   VALUES(@OrderNo,@MerchantTradNo,@BankTradeNo,@IDNO,@CardType,@AuthType,@final_price,@Status,@PRGID,@PRGID,@NowTime,@PRGID,@PRGID,@NowTime);
   END       
  END

--寫入錯誤訊息
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
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_OrderAuthAmount_I01';



