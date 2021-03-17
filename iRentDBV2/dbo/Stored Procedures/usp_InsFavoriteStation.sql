-- =============================================
-- Author:      eason
-- Create Date: 2021-03-17
-- Description: 設定常用站點
-- =============================================
CREATE PROCEDURE [dbo].[usp_InsFavoriteStation]
(   
	@MSG			VARCHAR(10) OUTPUT,
	@IDNO			VARCHAR(20)       , --身分證號
    @LogID			BIGINT,
    @FavoStations	TY_FavoriteStation  READONLY --站點List
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
	DECLARE @FunName VARCHAR(50) = 'usp_InsFavoriteStation'
	DECLARE @Now DATETIME = DATEADD(HOUR,8,GETDATE())

	BEGIN TRY
		BEGIN TRAN 		    

		SET @IDNO = ISNULL(@IDNO,'')

		IF @LogID IS NULL OR @LogID = ''
		BEGIN
			SET @Error=1
			SET @ErrorCode = 'spErr'
			SET @ErrorMsg = 'LogID必填'
		END

		IF  @Error = 0
		BEGIN

			DECLARE @FavoStations_add_count INT		    
			DECLARE @FavoStations_del_count INT
			
			SELECT @FavoStations_add_count = COUNT(*) FROM @FavoStations WHERE Mode = 1
			SELECT @FavoStations_del_count = COUNT(*) FROM @FavoStations WHERE Mode = 0

			--刪除最愛站點
			IF @FavoStations_del_count > 0
			BEGIN
			   DELETE FROM TB_FavoriteStation  
			   WHERE IDNO = @IDNO AND StationID IN (SELECT fs.StationID FROM @FavoStations fs WHERE fs.Mode=0)
			END

			--新增最愛站點
			IF @FavoStations_add_count > 0
			BEGIN		   
                WITH tmp AS(
			    SELECT infs.StationID, @IDNO[IDNO], @Now[MKTime]  FROM @FavoStations infs 
				LEFT JOIN TB_FavoriteStation tbfs ON tbfs.StationID = infs.StationID AND tbfs.IDNO = @IDNO
				WHERE infs.Mode = 1 AND ISNULL(tbfs.StationID,'') = '' 
				)
				INSERT INTO TB_FavoriteStation (IDNO, StationID, MKTime)
				SELECT t.IDNO, t.StationID, t.MKTime FROM tmp t
			END

			ELSE
			BEGIN
				SET @Error = 1
				SET @ErrorCode = 'spErr'
				SET @ErrorMsg = 'FavoStations 為必填' 
			END
		END

		COMMIT TRAN 
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
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


