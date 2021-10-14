/***********************************************************************************************
* Serve    : sqyhi03azt.database.windows.net
* Database : IRENT_V2T
* 程式名稱 : usp_WalletStoreShop_I02
* 系    統 : IRENT
* 程式功能 : 電子錢包超商條碼新增記錄
* 作    者 : AMBER
* 撰寫日期 : 20211007
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_WalletStoreShop_I02]
	@CTxSn			    varchar(64),   --用戶端交易代碼
	@TxSn			    varchar(26),   --交易序號
	@TxType		        varchar(1),    --交易類別 新增：i、刪除：d
	@PaymentId			varchar(20),   --銷帳編號
	@CvsType			int,           --超商類型 0:7-11,1:全家 ,2:萊爾富
	@CvsCode		    varchar(3),    --超商代收碼
	@PayAmount			int,           --繳費金額 
	@PayPeriod		    int,           --期數
	@DueDate		    varchar(8),    --繳費期限 YYYYMMDD
	@OverPaid		    varchar(1),    --是否允許溢繳
    @CustId		        varchar(10),   --繳費人客戶編號
	@CustMobile		    varchar(20),   --繳費人行動電話
	@CustEmail		    varchar(50),   --繳費人Email
	@Memo               nvarchar(50),  --備註
	@StatusCode			varchar(20),   --回應狀態代碼
    @StatusDesc			nvarchar(200), --狀態說明
	@LogID              BIGINT,         
	@ErrorCode          varchar(6)        OUTPUT,     --回傳錯誤代碼
	@ErrorMsg           nvarchar(100)     OUTPUT,     --回傳錯誤訊息
	@SQLExceptionCode   varchar(10)       OUTPUT,     --回傳sqlException代碼
	@SQLExceptionMsg    nvarchar(1000)    OUTPUT      --回傳sqlException訊
AS
BEGIN
SET NOCOUNT ON
	BEGIN TRY 
	SET	@ErrorCode  = '0000'	
	SET	@ErrorMsg   = 'SUCCESS'	
	SET	@SQLExceptionCode = ''		
	SET	@SQLExceptionMsg = ''		
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_WalletStoreShop_I02'
	DECLARE @NowTime DATETIME =dbo.GET_TWDATE()
	
	IF NOT EXISTS(SELECT 1 FROM TB_TaishinWalletStoreShopLog WITH(NOLOCK) WHERE TxSn=@Txsn)
	BEGIN
	INSERT INTO TB_TaishinWalletStoreShopLog
	(CTxSn,TxSn,TxType,PaymentId,CvsType,CvsCode,PayAmount,PayPeriod,DueDate,OverPaid,CustId,CustMobile,CustEmail,
     Memo,StatusCode,StatusDesc,MKTime,UPDTime)
	VALUES
	(
		@CTxSn,@TxSn,@TxType,@PaymentId,@CvsType,@CvsCode,@PayAmount,@PayPeriod,@DueDate,@OverPaid,@CustId,@CustMobile,@CustEmail,
		@Memo,@StatusCode,@StatusDesc,@NowTime,@NowTime
	)
	
	END
	
	END TRY	
	BEGIN CATCH
		ROLLBACK TRAN
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH
END



