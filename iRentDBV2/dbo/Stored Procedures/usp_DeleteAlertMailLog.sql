/****** Object:  StoredProcedure [dbo].[usp_DeleteAlertMailLog]    Script Date: 2021/3/30 下午 05:04:55 ******/

/****************************************************************
** Name: [dbo].[usp_DeleteAlertMailLog]
** Desc: 
**
** Return values: 0 成功 else 錯誤
** Return Recordset: 
**
** Called by: 
**
** Parameters:
** Input
** -----------

** 
**
** Output
** -----------
		
	@ErrorCode 				VARCHAR(6)			
	@ErrorCodeDesc			NVARCHAR(100)	
	@SQLExceptionCode		VARCHAR(10)				
	@SqlExceptionMsg		NVARCHAR(1000)	
**
** 
** Example
**------------
** DECLARE @Error               INT;
** DECLARE @ErrorCode 			VARCHAR(6);		
** DECLARE @ErrorMsg  			NVARCHAR(100);
** DECLARE @SQLExceptionCode	VARCHAR(10);		
** DECLARE @SQLExceptionMsg		NVARCHAR(1000);
** EXEC @Error=[dbo].[usp_DeleteAlertMailLog]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:
** Date:
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2021/03/16 10:30:00 Jet Add
** 
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_DeleteAlertMailLog]
	@LogID				BIGINT					,
	@ErrorCode			VARCHAR(6)		OUTPUT	,--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT	 --回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @SendTime DATETIME;
DECLARE @NowDate Datetime;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_DeleteAlertMailLog';
SET @IsSystem=0;
SET @ErrorType=0;
SET @NowDate=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
	IF @Error=0
	BEGIN
		SELECT TOP 1 @SendTime=ISNULL(SendTime,@NowDate) FROM TB_AlertMailLog WITH(NOLOCK) WHERE HasSend=1 ORDER BY SendTime DESC;
		SET @SendTime = DATEADD(MINUTE,-5,@SendTime);

		DELETE FROM TB_AlertMailLog WHERE HasSend=0 AND MKTime<=@SendTime;
	END

	--寫入錯誤訊息
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
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
	INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_DeleteAlertMailLog';
GO

