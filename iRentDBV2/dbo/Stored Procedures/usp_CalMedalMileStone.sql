/****************************************************************
** 用　　途：加總會員達成的里程數
*****************************************************************
** Change History
*****************************************************************
** 20210524 ADD BY YEH
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_CalMedalMileStone]
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
SET @LogID=123456;
SET @FunName='usp_CalMedalMileStone';
SET @hasData=0;
SET @NowDate=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
	IF @Error=0
	BEGIN
		DROP TABLE IF EXISTS #History;
		DROP TABLE IF EXISTS #TMP_CONFIG;
		DROP TABLE IF EXISTS #TMP_UPDATE;
		DROP TABLE IF EXISTS #TMP_INSERT;
		DROP TABLE IF EXISTS #TMP_StatusUPDATE;
		DROP TABLE IF EXISTS #TMP_StatusINSERT;

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
		WHERE [ActiveFLG]='A'
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

			-- 將在TB_MedalProgressStatus的資料從#History取出存至#TMP_UPDATE
			SELECT A.IDNO,A.Action,A.Amt
			INTO #TMP_UPDATE
			FROM #History A
			INNER JOIN TB_MedalProgressStatus B WITH(NOLOCK) ON A.IDNO=B.IDNO AND A.Action=B.Action;

			-- 將#History存至#TMP_INSERT
			SELECT A.IDNO,A.Action,A.Amt
			INTO #TMP_INSERT
			FROM #History A;

			-- 將#TMP_INSERT刪掉存在#TMP_UPDATE的資料
			DELETE A FROM #TMP_INSERT A 
			INNER JOIN #TMP_UPDATE B ON A.IDNO=B.IDNO AND A.Action=B.Action;

			-- 將#TMP_UPDATE更新至TB_MedalProgressStatus
			UPDATE TB_MedalProgressStatus
			SET Amt=A.Amt + B.Amt,
				UPDTime=@NowDate
			FROM TB_MedalProgressStatus A
			INNER JOIN #TMP_UPDATE B ON A.IDNO=B.IDNO AND A.Action=B.Action;

			-- 將#TMP_INSERT寫入至TB_MedalProgressStatus
			INSERT INTO TB_MedalProgressStatus([IDNO],[Class],[Series],[Action],[Amt],[MKTime],[UPDTime])
			SELECT A.IDNO,B.Class,B.Series,A.Action,A.Amt,@NowDate,@NowDate
			FROM #TMP_INSERT A
			INNER JOIN #TMP_CONFIG B ON A.Action=B.Action;

			-- 將TB_MedalHistory的狀態改成已計算
			UPDATE TB_MedalHistory
			SET ActiveFLG='B',
				UPDTime=@NowDate,
				UPDUser=@FunName
			FROM TB_MedalHistory A
			INNER JOIN #History B ON A.IDNO=B.IDNO AND A.Action=B.Action;

			-- 將TB_MedalProgressStatus且存在TB_MedalMileStone的資料寫入#TMP_StatusUPDATE
			SELECT A.IDNO,A.Class,A.Series,A.Action,A.Amt
			INTO #TMP_StatusUPDATE
			FROM TB_MedalProgressStatus A WITH(NOLOCK)
			INNER JOIN TB_MedalMileStone B ON A.IDNO=B.IDNO AND A.Class=B.Class AND A.Series=B.Series AND A.Action=B.Action;

			-- 將TB_MedalProgressStatus資料寫入#TMP_StatusINSERT
			SELECT A.IDNO,A.Class,A.Series,A.Action,A.Amt
			INTO #TMP_StatusINSERT
			FROM TB_MedalProgressStatus A WITH(NOLOCK)
			LEFT JOIN TB_MedalMileStone B ON A.IDNO=B.IDNO AND A.Class=B.Class AND A.Series=B.Series AND A.Action=B.Action;

			-- 將#TMP_StatusINSERT刪除存在#TMP_StatusUPDATE的資料
			DELETE A FROM #TMP_StatusINSERT A 
			INNER JOIN #TMP_StatusUPDATE B ON A.IDNO=B.IDNO AND A.Class=B.Class AND A.Series=B.Series AND A.Action=B.Action;

			-- 將#TMP_StatusUPDATE更新至TB_MedalMileStone
			UPDATE TB_MedalMileStone
			SET Progress=CASE WHEN A.Amt >= B.Norm THEN B.Norm ELSE A.Amt END,
				GetMedalTime=CASE WHEN A.Amt >= B.Norm THEN @NowDate ELSE NULL END,
				UPDTime=@NowDate
			FROM #TMP_StatusUPDATE A
			INNER JOIN TB_MedalMileStone B ON A.IDNO=B.IDNO AND A.Class=B.Class AND A.Series=B.Series AND A.Action=B.Action AND B.Norm<>B.Progress;

			-- 將#TMP_StatusINSERT寫入至TB_MedalMileStone
			INSERT INTO TB_MedalMileStone([IDNO],[Class],[Series],[Action],[MileStone],
				[Norm],[Progress],
				[GetMedalTime],[MKTime],[UPDTime])
			SELECT A.IDNO,B.Class,B.Series,B.Action,B.MileStone,
				B.Norm,CASE WHEN A.Amt >= B.Norm THEN B.Norm ELSE A.Amt END,
				CASE WHEN A.Amt >= B.Norm THEN @NowDate ELSE NULL END,@NowDate,@NowDate
			FROM #TMP_StatusINSERT A
			INNER JOIN TB_MedalConfig B ON A.Class=B.Class AND A.Series=B.Series AND A.Action=B.Action;
		END

		DROP TABLE IF EXISTS #History;
		DROP TABLE IF EXISTS #TMP_CONFIG;
		DROP TABLE IF EXISTS #TMP_UPDATE;
		DROP TABLE IF EXISTS #TMP_INSERT;
		DROP TABLE IF EXISTS #TMP_StatusUPDATE;
		DROP TABLE IF EXISTS #TMP_StatusINSERT;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CalMedalMileStone';
GO

