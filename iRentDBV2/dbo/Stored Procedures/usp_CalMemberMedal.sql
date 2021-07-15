/****************************************************************
** 用　　途：加總會員達成的里程數
*****************************************************************
** Change History
*****************************************************************
** 20210531 ADD BY YEH
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_CalMemberMedal]
	@IDNO				VARCHAR(10)				,	--帳號
	@ErrorCode			VARCHAR(6)		OUTPUT	,	--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		--回傳sqlException訊息
AS
DECLARE @Error		INT;
DECLARE @ErrorType	TINYINT;
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
SET @LogID=112233;
SET @FunName='usp_CalMemberMedal';
SET @hasData=0;
SET @NowDate=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
	IF @Error=0
	BEGIN
		DROP TABLE IF EXISTS #History;
		DROP TABLE IF EXISTS #TMP_CONFIG;
		DROP TABLE IF EXISTS #ProgressStatus;
		DROP TABLE IF EXISTS #MileStone;

		CREATE TABLE #History(
			IDNO	VARCHAR(10),
			Action	VARCHAR(50),
			Amt		FLOAT
		);

		-- 將待計算資料取出GROUP BY存至#History
		INSERT INTO #History
		SELECT [IDNO],
			[Action],
			SUM([Amt]) AS [Amt]
		FROM TB_MedalHistory WITH(NOLOCK)
		WHERE IDNO=@IDNO
		AND [ActiveFLG]='A'
		GROUP BY [IDNO],[Action];

		-- 取得要更新的資料筆數
		DECLARE @RowCount INT = 0;
		SET @RowCount = (SELECT COUNT(*) FROM #History);

		-- 有資料筆數才處理，避免SP CATCH
		IF @RowCount > 0
		BEGIN
			-- 取出設定檔
			SELECT Class,Series,Action 
			INTO #TMP_CONFIG 
			FROM TB_MedalConfig WITH(NOLOCK) GROUP BY Class,Series,Action;

			-- 將TB_MedalProgressStatus的資料JOIN #History取出存至#ProgressStatus
			SELECT A.IDNO,A.Action,A.Amt
			INTO #ProgressStatus
			FROM #History A
			INNER JOIN TB_MedalProgressStatus B ON A.IDNO=B.IDNO AND A.Action=B.Action;

			-- 將#ProgressStatus更新至TB_MedalProgressStatus
			UPDATE TB_MedalProgressStatus
			SET Amt=A.Amt + B.Amt,
				UPDTime=@NowDate
			FROM TB_MedalProgressStatus A
			INNER JOIN #ProgressStatus B ON A.IDNO=B.IDNO AND A.Action=B.Action;

			-- 將不存在#ProgressStatus的寫入至TB_MedalProgressStatus
			INSERT INTO TB_MedalProgressStatus([IDNO],[Class],[Series],[Action],[Amt],[MKTime],[UPDTime])
			SELECT A.IDNO,B.Class,B.Series,A.Action,A.Amt,@NowDate,@NowDate
			FROM #History A
			INNER JOIN #TMP_CONFIG B ON A.Action=B.Action
			WHERE A.Action NOT IN (SELECT Action FROM #ProgressStatus);

			-- 將TB_MedalHistory的狀態改成已計算
			UPDATE TB_MedalHistory
			SET ActiveFLG='B',
				UPDTime=@NowDate,
				UPDUser=@FunName
			FROM TB_MedalHistory A
			INNER JOIN #History B ON A.IDNO=B.IDNO AND A.Action=B.Action;

			-- 將TB_MedalProgressStatus且存在TB_MedalMileStone的資料寫入#MileStone
			SELECT A.IDNO,A.Class,A.Series,A.Action,A.Amt,ISNULL(B.MileStone,'') AS MileStone
			INTO #MileStone
			FROM TB_MedalProgressStatus A
			LEFT JOIN TB_MedalMileStone B ON A.IDNO=B.IDNO AND A.Class=B.Class AND A.Series=B.Series AND A.Action=B.Action
			WHERE A.IDNO=@IDNO;

			-- 將#MileStone更新至TB_MedalMileStone
			UPDATE TB_MedalMileStone
			SET Progress=CASE WHEN A.Amt >= B.Norm THEN B.Norm ELSE A.Amt END,
				GetMedalTime=CASE WHEN A.Amt >= B.Norm THEN @NowDate ELSE NULL END,
				UPDTime=@NowDate
			FROM #MileStone A
			INNER JOIN TB_MedalMileStone B ON A.IDNO=B.IDNO AND A.Class=B.Class AND A.Series=B.Series AND A.Action=B.Action AND B.Norm<>B.Progress;
		
			-- 將MileStone=''寫入至TB_MedalMileStone
			INSERT INTO TB_MedalMileStone([IDNO],[Class],[Series],[Action],[MileStone],
				[Norm],[Progress],
				[GetMedalTime],[MKTime],[UPDTime])
			SELECT A.IDNO,B.Class,B.Series,B.Action,B.MileStone,
				B.Norm,CASE WHEN A.Amt >= B.Norm THEN B.Norm ELSE A.Amt END,
				CASE WHEN A.Amt >= B.Norm THEN @NowDate ELSE NULL END,@NowDate,@NowDate
			FROM #MileStone A
			INNER JOIN TB_MedalConfig B ON A.Class=B.Class AND A.Series=B.Series AND A.Action=B.Action
			WHERE A.MileStone='';
		END

		DROP TABLE IF EXISTS #History;
		DROP TABLE IF EXISTS #TMP_CONFIG;
		DROP TABLE IF EXISTS #ProgressStatus;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CalMemberMedal';
GO

