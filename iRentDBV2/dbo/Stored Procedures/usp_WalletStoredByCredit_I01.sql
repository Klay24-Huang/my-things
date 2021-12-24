/***********************************************************************************************
* Serve    : sqyhi03azt.database.windows.net
* Database : IRENT_V2T
* 程式名稱 : usp_WalletStoredByCredit_I01
* 系    統 : IRENT
* 程式功能 : 寫入台新開戶儲值錯誤紀錄檔
* 作    者 : AMBER
* 撰寫日期 : 20211018
* 修改日期 : 20211224 UPD BY AMBER 新增input參數
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_WalletStoredByCredit_I01]
    @IDNO                   VARCHAR(10),  --身分證
	@MemberId               VARCHAR(20),  --商店會員編號
	@Name                   NVARCHAR(10), --會員姓名
	@PhoneNo                VARCHAR(10),  --手機號碼
    @Email                  VARCHAR(50),  --Email
	@AccountType			VARCHAR(1),   --帳戶類別(1:個人一類;2:個人二類(預設);3:個人三類;4:法人二類;5:法人三類)
	@CreateType				VARCHAR(1),   --會員虛擬帳號建立來源(1:使用商店會員編號;2:由系統產生)
	@AmountType				VARCHAR(1),   --金額類別(1:現金;2:信用卡;3:收款金額)
	@Amount					INT,          --交易金額
	@Bonus					INT,          --紅利點數
	@BonusExpiredate		VARCHAR(8),   --紅利到期日(yyyyMMdd)
	@SourceFrom				VARCHAR(1),   --交易來源
	@StoreValueReleaseDate	VARCHAR(8),   --履保起日(YYYYMMDD)交易來源=B實體禮物卡轉存時，此欄位才需帶值
	@GiftCardBarCode		VARCHAR(16),  --禮物卡條碼
	@PRGName                VARCHAR(50),  --程式名稱
	@ReturnCode             VARCHAR(4),   --回傳代碼
	@Message                NVARCHAR(200),  --回傳訊息
	@ExceptionData          NVARCHAR(500),  --回傳異常錯誤訊息
	@TradeType              VARCHAR(50),    --交易類別名稱 對應(TB_WalletCodeTable:CodeGroup)
	@MerchantTradeNo        VARCHAR(30),    --特店訂單編號
    @BankTradeNo            VARCHAR(50),    --銀行交易編號
    @CardNumber             VARCHAR(20),    --信用卡號
	@LogID					BIGINT,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS

DECLARE @Error INT
DECLARE @IsSystem TINYINT = 1
DECLARE @ErrorType TINYINT = 4
DECLARE @FunName VARCHAR(50) = 'usp_WalletStoredByCredit_I01'
DECLARE @NowTime DATETIME =[dbo].[GET_TWDATE]()

/*初始設定*/
SET @Error=0;
SET	@ErrorCode  = '0000'	
SET	@ErrorMsg   = 'SUCCESS'	
SET	@SQLExceptionCode = ''		
SET	@SQLExceptionMsg = ''
SET @IDNO =ISNULL (@IDNO,'');
SET @LogID=ISNULL (@LogID,0);
	
BEGIN TRY 	   
   BEGIN
      IF @Amount=0 OR @BankTradeNo=''
	  BEGIN
	    SET @Error=1
		SET @ErrorCode='ERR900'
	  END
      IF NOT EXISTS (SELECT 1 FROM TB_TaishinWalletStoreValueErrorLog WHERE BankTradeNo=@BankTradeNo)
	  INSERT INTO TB_TaishinWalletStoreValueErrorLog(ID,MemberId,[Name],PhoneNo,Email,AccountType,CreateType,AmountType,Amount,Bonus,BonusExpiredate,SourceFrom,StoreValueReleaseDate,GiftCardBarCode,ProcessStatus,ReturnCode,[Message],ExceptionData,MKTime,MKUser,MKPRGID,UPDTime,UPDUser,UPDPRGID,TradeType,MerchantTradeNo,BankTradeNo,CardNumber) 
      VALUES(@IDNO,@MemberId,@Name,@PhoneNo,@Email,@AccountType,@CreateType,@AmountType,@Amount,@Bonus,@BonusExpiredate,@SourceFrom,@StoreValueReleaseDate,@GiftCardBarCode,0,@ReturnCode,@Message,@ExceptionData,@NowTime,@PRGName,@PRGName,@NowTime,@PRGName,@PRGName,@TradeType,@MerchantTradeNo,@BankTradeNo,@CardNumber);
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
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_WalletStoredByCredit_I01';


