/****************************************************************
** Change History
*****************************************************************
** 2021/05/21 ADD BY YEH
** 
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetMapMedal]
	@IDNO				VARCHAR(10)				, --帳號
	@Token				VARCHAR(1024)			,
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
DECLARE @hasData TINYINT;
DECLARE @NowDate Datetime;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_GetMapMedal';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowDate=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @Token=ISNULL(@Token,'');

BEGIN TRY
	IF @Token='' OR @IDNO=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	--再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND Rxpires_in>@NowDate;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END

	IF @Error=0
	BEGIN
		DROP TABLE IF EXISTS #MileStone;

		CREATE TABLE #MileStone(
			IDNO		VARCHAR(10),
			MileStone	VARCHAR(50)
		);

		INSERT INTO #MileStone
		SELECT IDNO,MileStone
		FROM TB_MedalMileStone WITH(NOLOCK)
		WHERE IDNO=@IDNO
		AND GetMedalTime IS NOT NULL
		AND ShowTime IS NULL;

		SELECT B.MileStone
			,B.MileStoneName
		FROM #MileStone A
		LEFT JOIN TB_MedalConfig B WITH(NOLOCK) ON A.MileStone=B.MileStone;

		UPDATE TB_MedalMileStone
		SET ShowTime=@NowDate 
		FROM TB_MedalMileStone A
		INNER JOIN #MileStone B ON A.MileStone=B.MileStone AND A.IDNO=B.IDNO;

		DROP TABLE IF EXISTS #MileStone;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMapMedal';
GO