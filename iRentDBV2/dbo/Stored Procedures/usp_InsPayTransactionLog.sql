CREATE PROCEDURE [dbo].[usp_InsPayTransactionLog]
	 @GUID                 VARCHAR(32),
      @MerchantId           VARCHAR(16),
      @AccountId            VARCHAR(20),
      @BarCode              VARCHAR(25),
      @POSId                VARCHAR(20),
      @StoreId              VARCHAR(20),
      @StoreTransDate       VARCHAR(14),
      @StoreTransId         VARCHAR(20),
      @TransmittalDate      VARCHAR(8),
      @TransDate            VARCHAR(14),
      @TransId              VARCHAR(30),
      @SourceTransId        VARCHAR(30),
      @TransType            VARCHAR(4),
      @BonusFlag            VARCHAR(1),
      @PriceCustody         VARCHAR(1),
      @SmokeLiqueurFlag     VARCHAR(1),
      @Amount               int,
      @ActualAmount         int,
      @Bonus                int,
      @SourceFrom           VARCHAR(1),
      @AccountingStatus     VARCHAR(1),
      @SmokeAmount          int,
      @ActualGiftCardAmount int,
      @LogID                BIGINT,
      @ErrorCode            VARCHAR(6)        OUTPUT,     --回傳錯誤代碼
      @ErrorMsg             NVARCHAR(100)     OUTPUT,     --回傳錯誤訊息
      @SQLExceptionCode     VARCHAR(10)       OUTPUT,     --回傳sqlException代碼
      @SQLExceptionMsg      NVARCHAR(1000)    OUTPUT      --回傳sqlException訊息
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
		DECLARE @FunName VARCHAR(50) = 'usp_InsPayTransactionLog'
	

		IF NOT EXISTS(SELECT 1 FROM TB_TaishinWalletPayTransactionLog WITH(NOLOCK) WHERE GUID=@GUID)
		BEGIN
			INSERT INTO TB_TaishinWalletPayTransactionLog
			(
				GUID, MerchantId, AccountId, BarCode, POSId, StoreId, StoreTransDate, StoreTransId, TransmittalDate,
				TransDate, TransId, SourceTransId, TransType, BonusFlag, PriceCustody, SmokeLiqueurFlag, Amount, 
				ActualAmount, Bonus, SourceFrom, AccountingStatus, SmokeAmount, ActualGiftCardAmount, 
				MKTime
			)
			VALUES
			(
				@GUID, @MerchantId, @AccountId, @BarCode, @POSId, @StoreId, @StoreTransDate, @StoreTransId, @TransmittalDate,
				@TransDate, @TransId, @SourceTransId, @TransType, @BonusFlag, @PriceCustody, @SmokeLiqueurFlag, @Amount,
				@ActualAmount, @Bonus, @SourceFrom, @AccountingStatus, @SmokeAmount, @ActualGiftCardAmount,
				dbo.Get_TWDATE()
			)

		END
	END TRY
	
	BEGIN CATCH
		ROLLBACK TRAN
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH
END