/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_WalletStore_U01
* 系    統 : IRENT
* 程式功能 : 開戶及儲值錢包
* 作    者 : AMBER
* 撰寫日期 : 20210914
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_WalletStore_U01]
	@IDNO                   VARCHAR(10)           , --操作的會員帳號
	@WalletMemberID         VARCHAR(20)           , --錢包會員ID
	@WalletAccountID		VARCHAR(20)			  , --錢包虛擬ID
    @Status					INT      			  , --錢包狀態
    @Email				    VARCHAR(200)		  , --開戶時申請的mail
    @PhoneNo				VARCHAR(20)			  , --開戶時申請的電話
    @StoreAmount			INT					  , --儲值金額
	@WalletBalance          INT                   , --錢包金額
    @CreateDate				DATETIME			  , --開戶日期
    @LastTransDate			DATETIME			  , --最近一次交易日期
    @LastStoreTransId		VARCHAR(50)			  , --最近一次訂單編號
    @LastTransId			VARCHAR(50)			  , --最近一次台新訂單編號
	@TaishinNO              VARCHAR(30)	          , --台新IR編
	@TradeType              VARCHAR(20)           , --交易類別名稱
	@PRGID                  VARCHAR(20)           , --程式ID
	@Mode                   TINYINT               , --交易類別代號(0:消費;1:儲值;2:轉贈給他人;3:受他人轉贈;4:退款)
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
DECLARE @ORGID  VARCHAR(5)='01'
DECLARE @TradeKey VARCHAR(50)=''
DECLARE @ShowFLG  TINYINT=1

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_WalletStore_U01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime         =dbo.GET_TWDATE();
SET @IDNO            =ISNULL (@IDNO,'');
SET @WalletMemberID  =ISNULL (@WalletMemberID,'');
SET @WalletAccountID =ISNULL (@WalletAccountID,'');
SET @Token           =ISNULL (@Token,'');

BEGIN TRY

  IF @Token='' OR @IDNO=''  OR @WalletAccountID='' OR @WalletMemberID=''
  BEGIN
  SET @Error=1;
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
	--1.檢核是否有開通錢包
	SELECT @hasData=COUNT(1) FROM TB_UserWallet WITH(NOLOCK) WHERE IDNO=@IDNO;
	
	IF @hasData=0
	BEGIN
		INSERT INTO TB_UserWallet(IDNO,WalletMemberID,WalletAccountID,CreateDate,[Status],Email,PhoneNo,LastStoreTransId,LastTransDate,LastTransId,StoreAmount,WalletBalance)
		VALUES(@IDNO,@WalletMemberID,@WalletAccountID,@CreateDate,@Status,@Email,@PhoneNo,@LastStoreTransId,@LastTransDate,@LastTransId,@StoreAmount,@WalletBalance);		
	END
	ELSE
	BEGIN
		UPDATE TB_UserWallet
		SET WalletBalance=@WalletBalance,StoreAmount=StoreAmount+@StoreAmount,LastStoreTransId=@LastStoreTransId,LastTransDate=@LastTransDate,LastTransId=@LastTransId,[Status]=@Status
		WHERE IDNO=@IDNO AND  WalletMemberID=@WalletMemberID AND WalletAccountID=@WalletAccountID;			
	END

    INSERT INTO TB_WalletHistory(IDNO,WalletMemberID,WalletAccountID,Mode,Amount,TransDate,TransId,StoreTransId)
    VALUES(@IDNO,@WalletMemberID,@WalletAccountID,1,@StoreAmount,@LastTransDate,@LastTransId,@LastStoreTransId);

    INSERT INTO TB_WalletTradeMain(ORGID,IDNO,TaishinNO,TradeType,TradeKey,TradeDate,TradeAMT,F_CONTNO,ShowFLG,UPDTime,UPDUser,UPDPRGID,MKTime,MKUser,MKPRGID)
    VALUES (@ORGID,@IDNO,@TaishinNO,@TradeType,@TradeKey,@LastTransDate,@StoreAmount,@WalletMemberID,@ShowFLG,@NowTime,@PRGID,@PRGID,@NowTime,@PRGID,@PRGID);

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_WalletStore_U01';



