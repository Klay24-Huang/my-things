/****************************************************************
** 用　　途：加總會員積分
*****************************************************************
** Change History
*****************************************************************
** 20210603 ADD BY YEH
** 20210705 UPD BY YEH REASON:阿舒說已經是黑名單的只做積分加總，其餘欄位不動
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_CalMemberScore_U1]
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
DECLARE @BLOCK_EDATE DATETIME;		--停權日期
DECLARE @Block_Forever DATETIME;	--永久停權日期

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @ErrorType=0;
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @IsSystem=0;
SET @LogID=774411;
SET @FunName='usp_CalMemberScore_U1';
SET @hasData=0;
SET @NowDate=DATEADD(HOUR,8,GETDATE());
SET @BLOCK_EDATE=DATEADD(MONTH,1,@NowDate);	-- 預設系統日+1個月
SET @Block_Forever=CONVERT(datetime, '2099-12-31 23:59:59', 120);

BEGIN TRY
	IF @Error=0
	BEGIN
		DROP TABLE IF EXISTS #ScoreDetail;
		DROP TABLE IF EXISTS #ScoreTotal;
		DROP TABLE IF EXISTS #ScoreUpdate;

		-- 取出待計算的明細資料
		SELECT *
		INTO #ScoreDetail
		FROM TB_MemberScoreDetail WITH(NOLOCK)
		WHERE ISPROCESSED=0;

		-- 取得要更新的資料筆數
		DECLARE @RowCount INT = 0;
		SET @RowCount = (SELECT COUNT(*) FROM #ScoreDetail);

		-- 有資料筆數才處理，避免SP CATCH
		IF @RowCount > 0
		BEGIN
			-- 用帳號GROUP BY並加總出總分
			SELECT MEMIDNO,SUM(SCORE) AS Total
			INTO #ScoreTotal
			FROM #ScoreDetail
			GROUP BY MEMIDNO;

			-- 將已存在TB_MemberScoreMain的資料取出並計算
			-- 20210705 UPD BY YEH REASON:阿舒說已經是黑名單的只做積分加總，其餘欄位不動
			SELECT A.MEMIDNO,
				(A.SCORE + B.Total) AS SCORE,
				CASE WHEN A.ISBLOCK = 0 AND (A.SCORE + B.Total) <= 50 THEN A.BLOCK_CNT + 1	-- 不是黑名單，這次加總後變黑名單，次數+1
					 ELSE A.BLOCK_CNT END AS BLOCK_CNT,
				CASE WHEN A.ISBLOCK = 1 THEN 1	-- 原本是黑名單就不動
					 WHEN A.ISBLOCK = 0 AND (A.SCORE + B.Total) <= 50 THEN 1	-- 這次加總後變黑名單 
					 ELSE 0 END AS ISBLOCK,
				CASE WHEN A.ISBLOCK = 1 THEN A.BLOCK_EDATE	-- 原本是黑名單就不動日期
					 WHEN A.ISBLOCK = 0 AND (A.SCORE + B.Total) <= 50 THEN @BLOCK_EDATE	-- 這次加總後變黑名單就給新日期
					 ELSE NULL END AS BLOCK_EDATE,
				A.ISBLOCK AS OriginalISBLOCK
			INTO #ScoreUpdate
			FROM TB_MemberScoreMain A WITH(NOLOCK)
			INNER JOIN #ScoreTotal B ON A.MEMIDNO=B.MEMIDNO;

			-- 更新回TB_MemberScoreMain
			UPDATE TB_MemberScoreMain
			SET SCORE = B.SCORE,
				BLOCK_CNT = B.BLOCK_CNT,
				ISBLOCK = B.ISBLOCK,
				BLOCK_EDATE = CASE WHEN B.BLOCK_CNT >= 3 THEN @Block_Forever ELSE B.BLOCK_EDATE END,
				U_PRGID=@FunName,
				U_USERID='SYSTEM',
				U_SYSDT=@NowDate
			FROM TB_MemberScoreMain A
			INNER JOIN #ScoreUpdate B ON A.MEMIDNO=B.MEMIDNO;

			-- 低於50者要寫黑名單記錄檔
			INSERT INTO TB_MemberScoreBlock (A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,MEMIDNO,START_DT,END_DT)
			SELECT @FunName,'SYSTEM',@NowDate,@FunName,'SYSTEM',@NowDate,MEMIDNO,@NowDate,CASE WHEN BLOCK_CNT >= 3 THEN @Block_Forever ELSE @BLOCK_EDATE END
			FROM #ScoreUpdate
			WHERE SCORE <= 50 AND OriginalISBLOCK=0;

			-- 將不存在TB_MemberScoreMain的資料寫入
			INSERT INTO TB_MemberScoreMain (A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,
											MEMIDNO,MEMRFNBR,SCORE,BLOCK_CNT,ISBLOCK,BLOCK_EDATE)
			SELECT @FunName,'SYSTEM',@NowDate,@FunName,'SYSTEM',@NowDate,
				A.MEMIDNO,B.MEMRFNBR,
				(100 + A.Total),
				CASE WHEN (100 + A.Total) <= 50 THEN 1 ELSE 0 END,
				CASE WHEN (100 + A.Total) <= 50 THEN 1 ELSE 0 END,
				CASE WHEN (100 + A.Total) <= 50 THEN @BLOCK_EDATE ELSE NULL END
			FROM #ScoreTotal A
			INNER JOIN TB_MemberData B ON A.MEMIDNO=B.MEMIDNO
			WHERE A.MEMIDNO NOT IN (SELECT MEMIDNO FROM #ScoreUpdate);

			-- 低於50者要寫黑名單記錄檔
			INSERT INTO TB_MemberScoreBlock (A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,MEMIDNO,START_DT,END_DT)
			SELECT @FunName,'SYSTEM',@NowDate,@FunName,'SYSTEM',@NowDate,MEMIDNO,@NowDate,@BLOCK_EDATE
			FROM #ScoreTotal
			WHERE (100 + Total) <= 50
			AND MEMIDNO NOT IN (SELECT MEMIDNO FROM #ScoreUpdate);

			-- 加總後將明細資料狀態改為已處理
			UPDATE TB_MemberScoreDetail
			SET ISPROCESSED=1,
				U_PRGID=@FunName,
				U_USERID='SYSTEM',
				U_SYSDT=@NowDate
			FROM TB_MemberScoreDetail A
			INNER JOIN #ScoreDetail B ON B.SEQ=A.SEQ;
		END

		DROP TABLE IF EXISTS #ScoreDetail;
		DROP TABLE IF EXISTS #ScoreTotal;
		DROP TABLE IF EXISTS #ScoreUpdate;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CalMemberScore_U1';
GO

