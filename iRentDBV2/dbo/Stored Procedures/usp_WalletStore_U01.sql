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
    @ORGID                  VARCHAR(5)            , --公司別
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
	@TaishinNO              VARCHAR(30)	          , --IR編or台新訂單編號
	@TradeType              VARCHAR(20)           , --交易類別名稱
	@PRGName                VARCHAR(50)           , --程式名稱
	@Mode                   TINYINT               , --交易類別代號(0:消費;1:儲值;2:轉贈給他人;3:受他人轉贈;4:退款;5:欠費繳交)
	@InputSource            TINYINT               , --輸入來源(1:APP;2:Web)
	@OrderNo                VARCHAR(20)           , --訂單編號
	@ShowFLG                TINYINT               , --APP上是否顯示：0:隱藏,1:顯示
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

SET @FunName='usp_WalletStore_U01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime         =dbo.GET_TWDATE();
SET @OrderNo         =ISNULL (@OrderNo,'0')
SET @IDNO            =ISNULL (@IDNO,'');
SET @WalletMemberID  =ISNULL (@WalletMemberID,'');
SET @WalletAccountID =ISNULL (@WalletAccountID,'');
SET @Token           =ISNULL (@Token,'');

BEGIN TRY
　IF @InputSource = 1
　BEGIN
 　　IF @Token='' OR @IDNO=''  OR @WalletAccountID='' OR @WalletMemberID=''
     BEGIN
   　  SET @Error=1;
  　   SET @ErrorCode='ERR900'
	 END
  END
  ELSE 
  BEGIN
    IF @IDNO=''  OR @WalletAccountID='' OR @WalletMemberID=''
    BEGIN
      SET @Error=1
  　  SET @ErrorCode='ERR900'
	END
  END
		 
  --0.再次檢核token
  IF @Error=0　And @InputSource = 1
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

		DECLARE @PRGID VARCHAR(10) = '0'
		IF @InputSource = 1
		BEGIN
			SELECT @PRGID = Convert(VARCHAR(10),APIID) FROM TB_APIList WITH(NOLOCK) WHERE  APIName = @PRGName
		END
		ELSE
		BEGIN
			SET @PRGID = Left(@PRGName,10)
		END

		INSERT INTO TB_WalletHistory(IDNO,WalletMemberID,WalletAccountID,Mode,Amount,TransDate,TransId,StoreTransId,OrderNo)
		VALUES(@IDNO,@WalletMemberID,@WalletAccountID,1,@StoreAmount,@LastTransDate,@LastTransId,@LastStoreTransId,Convert(bigint,@OrderNo));

		INSERT INTO TB_WalletTradeMain(ORGID,IDNO,TaishinNO,ORDNO,TradeType,TradeDate,TradeAMT,F_CONTNO,ShowFLG,UPDTime,UPDUser,UPDPRGID,MKTime,MKUser,MKPRGID)
		VALUES (@ORGID,@IDNO,@TaishinNO,@OrderNo,@TradeType,@LastTransDate,@StoreAmount,@WalletMemberID,@ShowFLG,@NowTime,@PRGID,@PRGID,@NowTime,@PRGID,@PRGID);


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



