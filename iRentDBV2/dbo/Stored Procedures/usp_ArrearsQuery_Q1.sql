-- =============================================
-- Author:      adam
-- Create Date: 2020-12-13
-- Description: 欠費查詢
-- =============================================
CREATE PROCEDURE [dbo].[usp_ArrearsQuery_Q1]
(   
	@MSG					VARCHAR(10) OUTPUT,
	@NPR330SaveID           INT       ,
    @LogID                  BIGINT
)
AS
BEGIN
    SET NOCOUNT ON

	DECLARE @Error INT = 0
    DECLARE	@ErrorCode VARCHAR(6) = '0000'	
    DECLARE	@ErrorMsg  		   NVARCHAR(100) = 'SUCCESS'	
    DECLARE	@SQLExceptionCode  VARCHAR(10) = ''		
    DECLARE	@SQLExceptionMsg   NVARCHAR(1000) = ''	
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_ArrearsQuery_Q1'


	BEGIN TRY

		IF @LogID IS NULL OR @LogID = ''
		BEGIN
			SET @Error=1
			SET @ErrorCode = 'spErr'
			SET @ErrorMsg = 'LogID必填'
		END

		IF @Error=0
		BEGIN
			
			SELECT NPR330Save_ID
				,CarNo
				,Amount
				,IRENTORDNO
				,ORDNO
				,CNTRNO 
				,POLNO
				,ArrearsKind as PAYMENTTYPE 
			FROM TB_NPR330Detail WITH(NOLOCK) 
			WHERE NPR330Save_ID=@NPR330SaveID
			AND IsPay=0

		END


	END TRY
	BEGIN CATCH
		SET @Error=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();
		--IF @@TRANCOUNT > 0
  --      BEGIN
  --          print 'rolling back transaction' /* <- this is never printed */
  --          ROLLBACK TRAN
  --      END

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	--輸出系統訊息
	SELECT @ErrorCode[ErrorCode], @ErrorMsg[ErrorMsg], @SQLExceptionCode[SQLExceptionCode], @SQLExceptionMsg[SQLExceptionMsg], @Error[Error]

END
GO

