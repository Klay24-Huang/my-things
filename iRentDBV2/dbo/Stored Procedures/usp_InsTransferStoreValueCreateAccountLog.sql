
CREATE PROCEDURE [dbo].[usp_InsTransferStoreValueCreateAccountLog]
	@GUID				varchar(32),
	@MerchantId			varchar(16),
	@AccountId			varchar(20),
	@BarCode			varchar(21),
	@POSId				varchar(20),
	@StoreId			varchar(20),
	@StoreTransDate		varchar(14),
	@StoreTransId		varchar(20),
	@TransmittalDate	varchar(8),
	@TransDate			varchar(14),
	@TransId			varchar(30),
	@Amount				int,
	@ActualAmount		int,
	@TransAccountId		varchar(420),
	@SourceFrom			varchar(1),
	@AmountType			varchar(1),
	@LogID                BIGINT,
	@ErrorCode            VARCHAR(6)        OUTPUT,     --回傳錯誤代碼
	@ErrorMsg             NVARCHAR(100)     OUTPUT,     --回傳錯誤訊息
	@SQLExceptionCode     VARCHAR(10)       OUTPUT,     --回傳sqlException代碼
	@SQLExceptionMsg      NVARCHAR(1000)    OUTPUT      --回傳sqlException訊
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
		DECLARE @FunName VARCHAR(50) = 'usp_InsTransferStoreValueCreateAccountLog'
	

		IF NOT EXISTS(SELECT 1 FROM TB_TaishinWalletTransferStoreValueLog WITH(NOLOCK) WHERE GUID=@GUID)
		BEGIN
			INSERT INTO TB_TaishinWalletTransferStoreValueLog
			(
				GUID, MerchantId, AccountId, BarCode, POSId, StoreId, StoreTransDate, StoreTransId, TransmittalDate, 
				TransDate, TransId, Amount, ActualAmount, TransAccountId, SourceFrom, AmountType, 
				MKTime
			)
			VALUES
			(
				@GUID, @MerchantId, @AccountId, @BarCode, @POSId, @StoreId, @StoreTransDate, @StoreTransId, @TransmittalDate, 
				@TransDate, @TransId, @Amount, @ActualAmount, @TransAccountId, @SourceFrom, @AmountType, 
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