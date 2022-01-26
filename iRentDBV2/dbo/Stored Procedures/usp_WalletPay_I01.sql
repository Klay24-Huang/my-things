/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_WalletPay_I01
* 系    統 : IRENT
* 程式功能 : 錢包付款，寫入錢包扣款紀錄
* 作    者 : Umeko
* 撰寫日期 : 20210922
* 修改日期 :
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_WalletPay_I01]
		@IDNO								VARCHAR(10)           , --操作的會員帳號
		@OrderNO						bigint                         , --訂單編號
		@WalletMemberID         VARCHAR(20)			, --錢包會員ID
		@WalletAccountID			VARCHAR(20)			, --錢包虛擬ID
		@Amount							INT								, --付款金額
		@WalletBalance				INT								, --錢包餘額
		@TransDate						DATETIME					,  --交易日期
		@StoreTransId					VARCHAR(50)			, --訂單編號
		@TransId							VARCHAR(50)			, --台新訂單編號
		@TradeType						VARCHAR(20)           , --交易類別名稱
		@PRGName						VARCHAR(50)           , --程式Name
		@Mode								TINYINT						, --交易類別代號(0:消費;1:儲值;2:轉贈給他人;3:受他人轉贈;4:退款;5:欠費繳交)
		@InputSource                  TINYINT                      ,--輸入來源(1:APP;2:Web)
		@Token								VARCHAR(1024)       , --JWT TOKEN
		@LogID								BIGINT						, --執行的api log
		@ErrorCode 						VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
		@ErrorMsg  						NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
		@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
		@SQLExceptionMsg			NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
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

SET @FunName='usp_WalletPay_I01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime   =dbo.GET_TWDATE();
SET @IDNO  =ISNULL (@IDNO,'');
SET @OrderNO =ISNULL (@OrderNO,0);
SET @WalletMemberID  =ISNULL (@WalletMemberID,'');
SET @WalletAccountID =ISNULL (@WalletAccountID,'');
SET @Token =ISNULL (@Token,'');

BEGIN TRY
  IF @InputSource = 1
  Begin
	  IF @Token='' OR @IDNO=''  OR @WalletAccountID='' OR @WalletMemberID='' OR @OrderNO = '0'
	  BEGIN
	  SET @Error=1;
	  SET @ErrorCode='ERR900'
	  END
  End
  Else
  Begin
	  IF @IDNO=''  OR @WalletAccountID='' OR @WalletMemberID='' OR @OrderNO = '0'
	  BEGIN
	  SET @Error=1;
	  SET @ErrorCode='ERR900'
	  END
  End
  --0.再次檢核token
 
  IF @Error=0 And @InputSource = 1
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
		Declare @PRGID varchar(10) = '0'
		IF @InputSource = 1
		Begin
			Select @PRGID = Convert(varchar(10),APIID) From TB_APIList with(nolock) Where APIName = @PRGName
		End
		Else
		Begin
			Set @PRGID = Left(@PRGName,10)
		End
		--寫入錢包歷程
		INSERT INTO TB_WalletHistory(IDNO,WalletMemberID,WalletAccountID,Mode,Amount,TransDate,TransId,StoreTransId)
		VALUES(@IDNO,@WalletMemberID,@WalletAccountID,@Mode,@Amount,@TransDate,@TransId,@StoreTransId);
		--寫入錢包交易主檔
		INSERT INTO TB_WalletTradeMain(ORGID,IDNO,TaishinNO,TradeType,TradeKey,TradeDate,TradeAMT,F_CONTNO,ShowFLG,UPDTime,UPDUser,UPDPRGID,MKTime,MKUser,MKPRGID)
		VALUES (@ORGID,@IDNO,@TransId,@TradeType,@OrderNO,@TransDate,@Amount,@WalletMemberID,@ShowFLG,@NowTime,@PRGID,@PRGID,@NowTime,@PRGID,@PRGID);

		Declare @ReceivedAmount int = 0,@StoreAmount int = 0, @UseReceivedAmount int = 0,@UseStoreAmount int,@LessAmount int = @Amount
		--計算扣款類型
		Select @ReceivedAmount =[ReceivedAmount] ,@StoreAmount = [StoreAmount]
		From TB_UserWallet
		Where IDNO=@IDNO AND  WalletMemberID=@WalletMemberID AND WalletAccountID=@WalletAccountID

		if (@ReceivedAmount > 0)
		begin
			if @ReceivedAmount >= @Amount
			Begin
				Set @UseReceivedAmount = @Amount				
			End
			Else
			Begin
				Set @UseReceivedAmount = @ReceivedAmount
			End
			Set @LessAmount = @LessAmount - @UseReceivedAmount
		End

		Set @UseStoreAmount = @LessAmount
		--更新錢包餘額
		Update TB_UserWallet
		SET WalletBalance=@WalletBalance
		,StoreAmount = StoreAmount - @UseStoreAmount
		,ReceivedAmount = ReceivedAmount - @UseReceivedAmount
		,LastStoreTransId=@StoreTransId,LastTransDate=@TransDate,LastTransId=@TransId
		WHERE IDNO=@IDNO AND  WalletMemberID=@WalletMemberID AND WalletAccountID=@WalletAccountID
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_WalletPay_I01';