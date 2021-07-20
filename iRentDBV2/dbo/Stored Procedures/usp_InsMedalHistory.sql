/****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsMedalHistory]
	@MSG		VARCHAR(100)	OUTPUT	,
	@IDNO		VARCHAR(10)				,	--帳號
	@Event		VARCHAR(50)				,	--關聯事件
	@Action		VARCHAR(10)				,	--動作
	@Amt		INT						,	--計數
	@ActiveFLG	VARCHAR(1)				,	--A:待計算,B:已計算,C:已取消,N:不計算(金流未進帳)
	@MKTime		DATETIME				,	--寫入時間
	@MKUser		VARCHAR(10)				,	--寫入使用者
	@UPDTime	DATETIME				,	--更新時間
	@UPDUser	VARCHAR(10)					--更新使用者
AS
DECLARE @Error		INT;
DECLARE @ErrorCode	VARCHAR(6);
DECLARE @ErrorMsg	NVARCHAR(100);
DECLARE @ErrorType	TINYINT;
DECLARE @SQLExceptionCode VARCHAR(10);
DECLARE @SQLExceptionMsg NVARCHAR(1000);
DECLARE @IsSystem	TINYINT;
DECLARE @LogID		BIGINT;
DECLARE @FunName	VARCHAR(50);
DECLARE @hasData	TINYINT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @ErrorType=0;
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @IsSystem=0;
SET @LogID=123456;
SET @FunName='usp_InsMedalHistory';
SET @hasData=0;
SET @IDNO=ISNULL (@IDNO,'');

BEGIN TRY
	IF @IDNO=''
	BEGIN
		SET @Error=1;
		SET @MSG='IDNO EMPTY';
	END

	IF @Error=0
	BEGIN
		INSERT INTO TB_MedalHistory (IDNO,Event,Action,Amt,MKTime,MKUser,UPDTime,UPDUser,ActiveFLG)
		VALUES(@IDNO,@Event,@Action,@Amt,@MKTime,@MKUser,@UPDTime,@UPDUser,@ActiveFLG);
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

	SET @MSG = CONCAT(@IDNO,'|',@Event,'|',@Action,'|無法寫入');
END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsMedalHistory';
GO

