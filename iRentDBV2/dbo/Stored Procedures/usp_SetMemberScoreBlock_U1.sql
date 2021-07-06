/****************************************************************
** 用　　途：停權會員恢復
*****************************************************************
** Change History
*****************************************************************
** 20210603 ADD BY YEH
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_SetMemberScoreBlock_U1]
	@MSG		VARCHAR(100)	OUTPUT
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
DECLARE @NowDate	DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @ErrorType=0;
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @IsSystem=0;
SET @LogID=774411;
SET @FunName='usp_SetMemberScoreBlock_U1';
SET @hasData=0;
SET @NowDate=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
	IF @Error=0
	BEGIN
		DROP TABLE IF EXISTS #ScoreMain;

		-- 取出黑名單次數 < 3 & 停權日期 <= 系統日 的資料
		SELECT * 
		INTO #ScoreMain
		FROM TB_MemberScoreMain WITH(NOLOCK) 
		WHERE BLOCK_CNT < 3 
		AND ISBLOCK > 0 
		AND BLOCK_EDATE IS NOT NULL 
		AND BLOCK_EDATE <= @NowDate;

		-- 將停權會員恢復
		UPDATE TB_MemberScoreMain
		SET ISBLOCK = 0,
			BLOCK_EDATE = NULL,
			U_PRGID=@FunName,
			U_USERID='SYSTEM',
			U_SYSDT=@NowDate
		FROM TB_MemberScoreMain A
		INNER JOIN #ScoreMain B ON A.MEMIDNO=B.MEMIDNO;

		DROP TABLE IF EXISTS #ScoreMain;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SetMemberScoreBlock_U1';
GO

