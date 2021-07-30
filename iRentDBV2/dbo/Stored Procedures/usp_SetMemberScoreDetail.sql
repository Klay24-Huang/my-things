/****************************************************************
** 用　　途：會員積分記錄刪除
*****************************************************************
** Change History
*****************************************************************
** 20210519 ADD BY YEH
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_SetMemberScoreDetail]
	@IDNO				VARCHAR(10)				,	--帳號
	@SEQ                INT						,	--序號
	@PRGID				INT						,	--程式代號
	@LogID				BIGINT					,
	@ErrorCode			VARCHAR(6)		OUTPUT	,	--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		--回傳sqlException訊息
AS
DECLARE @Error		INT;
DECLARE @IsSystem	TINYINT;
DECLARE @FunName	VARCHAR(50);
DECLARE @ErrorType	TINYINT;
DECLARE @hasData	TINYINT;
DECLARE @NowTime	DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_SetMemberScoreDetail';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @SEQ=ISNULL(@SEQ,0);

BEGIN TRY
	IF @IDNO='' OR @SEQ=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		IF EXISTS(SELECT * FROM TB_MemberScoreDetail WITH(NOLOCK) WHERE SEQ=@SEQ AND MEMIDNO=@IDNO)
		BEGIN
			UPDATE TB_MemberScoreDetail
			SET UIDISABLE=1,
				UIDISABLE_DT=@NowTime,
				U_PRGID=@PRGID,
				U_USERID=@IDNO,
				U_SYSDT=@NowTime
			WHERE SEQ=@SEQ AND MEMIDNO=@IDNO;
		END
		ELSE
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR915';
		END
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SetMemberScoreDetail';
GO