/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_WalletReturn_Q01
* 系    統 : IRENT
* 程式功能 : 撈取錢包退款清單
* 作    者 : Amber
* 撰寫日期 : 20220223
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_WalletReturn_Q01]
    @PRGName                VARCHAR(50)           ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息          
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT=0;
DECLARE @FunName VARCHAR(50)='usp_WalletReturn_Q01';
DECLARE @ErrorType TINYINT=0;
DECLARE @NowTime DATETIME=dbo.GET_TWDATE()

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @PRGName=ISNULL(@PRGName,'')

BEGIN TRY
    DECLARE @PRGID VARCHAR(20) = '0'
    IF EXISTS(SELECT 1 FROM TB_APIList WITH(NOLOCK) WHERE APIName = @PRGName)
    BEGIN
	 SELECT @PRGID = CONVERT(VARCHAR(20),APIID) FROM TB_APIList WITH(NOLOCK) WHERE APIName = @PRGName
	END
	ELSE
	BEGIN
	SET @PRGID = Left(@PRGName,20)
	END

	SELECT authSeq[AuthSeq],R.IDNO,R.order_number[Order_number],returnAmt[ReturnAmt],ori_transaction_no[Ori_transaction_no],transaction_no[Transaction_no],CardType,
	POSId,StoreId,SourceFrom,AccountId,StoreTransDate,StoreTransId,TransId,
    CASE WHEN cancel_status >0 THEN 'Cancel' ELSE 'PreAuth_Return' END AS TradeType
	INTO #WalletReturn
	FROM TB_OrderAuthReturn R  WITH(NOLOCK)
	JOIN TB_TaishinWalletPayTransactionLog T WITH(NOLOCK) ON R.ori_transaction_no=StoreTransId 
	JOIN TB_OrderMain O WITH(NOLOCK) ON O.order_number=R.order_number
    WHERE CardType=2 AND AuthFlg=0 AND T.TransType='T001' --抓原交易扣款相關資料
 	ORDER BY authSeq;


	UPDATE T
	SET T.AuthFlg= 9, --處理狀態(0:未處理,1:處理成功,-1:處理失敗;9:處理中)
	U_SYSDT=@NowTime,
	U_PRGID=@PRGID,
	U_USERID=@PRGID
	FROM  TB_OrderAuthReturn T
	JOIN #WalletReturn C ON T.authSeq=C.AuthSeq;

    SELECT * FROM #WalletReturn ;
	DROP TABLE IF EXISTS #WalletReturn;

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_WalletReturn_Q01';
GO


