-- =============================================
-- Author:      eason
-- Create Date: 2020-10-22
-- Description: 欠費查詢
-- =============================================
CREATE PROCEDURE [dbo].[usp_ArrearsQuery_U1]
(   
	@MSG					VARCHAR(10) OUTPUT,
    @LogID                  BIGINT,
    @ArrearsQuery  TY_ArrearsQuery  READONLY
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
	DECLARE @FunName VARCHAR(50) = 'usp_ArrearsQuery_U1'

	BEGIN TRY
       /*
		declare @t TY_ArrearsQuery 
		insert into @t 
		values('1','2','3','4','1',6,7,'8','9','1','11','12','13','14',15)
		exec usp_ArrearsQuery_U1 1, @t	   
	   */		
	    
	   IF @LogID IS NULL OR @LogID = ''
	   BEGIN
		   SET @Error=1
		   SET @ErrorCode = 'spErr'
		   SET @ErrorMsg = 'LogID必填'
	   END

	   IF @Error = 0
	   BEGIN
			DECLARE @ArrearsQuery_count INT 
			SELECT @ArrearsQuery_count = COUNT(*) FROM @ArrearsQuery
			IF @ArrearsQuery_count > 0
			BEGIN
					SELECT 
					a.TAMT AS Amount, --待繳金額
					a.PAYMENTTYPE AS ArrearsKind, --欠費種類 1:租金,2:罰單,3:停車費,4:ETAG
					a.RNTDATE AS EndDate, --實際還車時間
					a.GIVEDATE AS StartDate, --實際取車時間
					OrderNo = CASE WHEN a.IRENTORDNO IS NULL OR a.IRENTORDNO = '' THEN '-'
					          ELSE 'H' + RIGHT(REPLICATE('0', 7) + CAST(a.IRENTORDNO as NVARCHAR), 7) END,
					a.CNTRNO AS ShortOrderNo, --短租合約編號
					StationName = ISNULL((SELECT TOP 1 s.StationName FROM TB_ManagerStation s WHERE s.StationID = a.INBRNHCD),'-'), --取車據點
					CarType = ISNULL(( --車型代碼
						SELECT TOP 1 ct.CarTypeName FROM TB_CarType ct
						WHERE ct.CarType = (
							SELECT TOP 1 c.CarType FROM TB_Car c WHERE c.CarNo = a.CARNO)
					),''),
					IsMotor = ISNULL((SELECT TOP 1 c.IsMotor FROM TB_CarInfo c WHERE c.CarNo = a.CARNO),0) ---是否是機車0否,1是
					FROM  @ArrearsQuery a			    
			END
			ELSE
			BEGIN
				SET @Error = 1
				SET @ErrorCode = 'spErr'
				SET @ErrorMsg = 'ArrearsQuery 為必填' 
			END
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


