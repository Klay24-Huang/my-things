/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_WalletReturn_U01
* 系    統 : IRENT
* 程式功能 : 更新錢包退款
* 作    者 : Amber
* 撰寫日期 : 20220223
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_WalletReturn_U01]
 　 @IDNO						VARCHAR(10)         , --操作的會員帳號
	@OrderNo					BIGINT              , --訂單編號
	@WalletAccountID			VARCHAR(20)			, --錢包虛擬ID
	@AuthSeq                    BIGINT              , --TB_OrderAuthReturn識別欄位
	@Amount						INT					, --返還金額
	@WalletBalance				INT					, --錢包餘額
	@TransDate					DATETIME			, --交易日期
	@StoreTransId				VARCHAR(50)			, --商店訂單編號
	@TransId					VARCHAR(50)			, --台新訂單編號
	@TradeType					VARCHAR(20)         , --交易類別名稱
	@Mode						TINYINT				, --交易類別代號(0:消費;1:儲值;2:轉贈給他人;3:受他人轉贈;4:退款;5:欠費繳交)
    @PRGName					VARCHAR(50)         , --程式Name
	@IsDuplicate                INT                 , --交易是否重複
	@AuthFlg				    INT                 , --交易狀態
	@AuthCode				    VARCHAR(50)         , --交易回傳代碼
	@AuthMessage			    NVARCHAR(120)       , --交易回傳訊息
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息          
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT=0;
DECLARE @FunName VARCHAR(50)='usp_WalletReturn_U01';
DECLARE @ErrorType TINYINT=0;
DECLARE @ORGID  VARCHAR(5)='01'
DECLARE @ShowFLG  TINYINT=1
DECLARE @NowTime DATETIME

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @OrderNo         =ISNULL (@OrderNo,0);
SET @PRGName         =ISNULL(@PRGName,'')
SET @IDNO            =ISNULL (@IDNO,'');
SET @WalletAccountID =ISNULL (@WalletAccountID,'');
SET @NowTime         =dbo.GET_TWDATE();

BEGIN TRY

　　IF @OrderNo=0 or @IDNO='' or @WalletAccountID=''
	BEGIN
	SET @Error=1;
	SET @ErrorCode='ERR900'
	END

    DECLARE @PRGID VARCHAR(20) = '0'
    IF EXISTS(SELECT 1 FROM TB_APIList WITH(NOLOCK) WHERE APIName = @PRGName)
    BEGIN
	 SELECT @PRGID = CONVERT(VARCHAR(20),APIID) FROM TB_APIList WITH(NOLOCK) WHERE APIName = @PRGName
	END
	ELSE
	BEGIN
	SET @PRGID = Left(@PRGName,50)
	END
    	
	IF @Error=0
	BEGIN
	DECLARE @Balance_Before INT =0
	DECLARE @Balance_After INT =0
	DECLARE @WalletMemberID VARCHAR(20)=''
	
	--更新交易退款檔狀態	
	UPDATE TB_OrderAuthReturn SET AuthFlg=@AuthFlg, AuthCode=@AuthCode, AuthMessage=@AuthMessage,transaction_no=@TransId,
	U_USERID=@PRGID,U_PRGID=@PRGID,U_SYSDT=@NowTime
    WHERE authSeq=@AuthSeq;

    IF @AuthFlg=1 AND @IsDuplicate=0 --台新回交易成功&交易沒重複
	BEGIN	
	SELECT  @Balance_Before=WalletBalance,@WalletMemberID=WalletMemberID FROM  TB_UserWallet WITH(NOLOCK) WHERE IDNO=@IDNO AND WalletAccountID=@WalletAccountID
	SET @Balance_After=@Balance_Before+@Amount
		
	UPDATE TB_UserWallet
	SET WalletBalance=@WalletBalance
	,StoreAmount = StoreAmount +@Amount
	,LastStoreTransId=@StoreTransId,LastTransDate=@TransDate,LastTransId=@TransId
	WHERE IDNO=@IDNO AND  WalletAccountID=@WalletAccountID

	--寫入錢包歷程
　　INSERT INTO TB_WalletHistory(IDNO,WalletMemberID,WalletAccountID,Mode,Amount,Balance_Before,Balance_After,TransDate,TransId,StoreTransId,OrderNo)
	VALUES(@IDNO,@WalletMemberID,@WalletAccountID,@Mode,@Amount,@Balance_Before,@Balance_After,@TransDate,@TransId,@StoreTransId,@OrderNo);
	
	--寫入錢包交易主檔
	INSERT INTO TB_WalletTradeMain(ORGID,IDNO,TaishinNO,TradeType,TradeKey,TradeDate,TradeAMT,F_CONTNO,ShowFLG,UPDTime,UPDUser,UPDPRGID,MKTime,MKUser,MKPRGID,ORDNO,TransId,StoreTransId)
	VALUES (@ORGID,@IDNO,@TransId,@TradeType,@OrderNo,@TransDate,@Amount,@WalletAccountID,@ShowFLG,@NowTime,@PRGID,@PRGID,@NowTime,@PRGID,@PRGID,@OrderNo,@TransId,@StoreTransId);
	END
	END
		
	--寫入錯誤訊息
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,0,@IsSystem);
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
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,0,@IsSystem);
END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_WalletReturn_U01';


