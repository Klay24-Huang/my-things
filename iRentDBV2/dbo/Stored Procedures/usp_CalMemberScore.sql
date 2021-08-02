/****************************************************************
** 用　　途：加總會員積分
*****************************************************************
** Change History
*****************************************************************
** 20210531 ADD BY YEH
** 20210705 UPD BY YEH REASON:阿舒說已經是黑名單的只做積分加總，其餘欄位不動
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_CalMemberScore]
	@IDNO				VARCHAR(10)				,	--帳號
	@ErrorCode			VARCHAR(6)		OUTPUT	,	--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;
DECLARE @LogID BIGINT;
DECLARE @Detail INT;	--明細總積分
DECLARE @Default INT;	--預設總積分
DECLARE @Main INT;		--主檔總積分
DECLARE @BLOCK_CNT INT;	--黑名單次數
DECLARE @BLOCK_EDATE DATETIME;	--停權日期
DECLARE @ISBLOCK INT;	--是否為黑名單：0：不是黑名單，1：列為黑名單

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_CalMemberScore';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @LogID=963852;
SET @IDNO=ISNULL(@IDNO,'');
SET @Detail=0;
SET @Default=100;
SET @Main=0;
SET @BLOCK_CNT=0;
SET @BLOCK_EDATE=DATEADD(MONTH,1,@NowTime);	-- 預設系統日+1個月
SET @ISBLOCK=0;

BEGIN TRY
	IF @IDNO=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		DROP TABLE IF EXISTS #ScoreDetail;

		-- 將待加總資料寫入TEMP TABLE
		SELECT *
		INTO #ScoreDetail
		FROM TB_MemberScoreDetail WITH(NOLOCK)
		WHERE MEMIDNO=@IDNO AND ISPROCESSED=0;

		-- 取得要更新的資料筆數
		DECLARE @RowCount INT = 0;
		SET @RowCount = (SELECT COUNT(*) FROM #ScoreDetail);

		-- 有資料筆數才處理，避免SP CATCH
		IF @RowCount > 0
		BEGIN
			-- 取得明細加總
			SELECT @Detail=SUM(SCORE) FROM #ScoreDetail;

			-- 判斷主檔是否有資料，沒資料INSERT，有資料UPDATE
			SELECT @hasData=COUNT(*) FROM TB_MemberScoreMain WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
			IF @hasData = 0
			BEGIN
				SET @Main = @Default + @Detail;	-- 總積分 = 預設值 + 明細積分

				IF @Main > 50
				BEGIN
					INSERT INTO TB_MemberScoreMain (A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,
													MEMIDNO,MEMRFNBR,SCORE,BLOCK_CNT,ISBLOCK)
					SELECT @FunName,@IDNO,@NowTime,@FunName,@IDNO,@NowTime,
						MEMIDNO,MEMRFNBR,@Main,0,0
					FROM TB_MemberData WITH(NOLOCK)
					WHERE MEMIDNO=@IDNO;
				END
				ELSE	
				BEGIN
					-- 總積分 <= 50 要記錄黑名單次數及停權日期
					INSERT INTO TB_MemberScoreMain (A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,
													MEMIDNO,MEMRFNBR,SCORE,BLOCK_CNT,ISBLOCK,BLOCK_EDATE)
					SELECT @FunName,@IDNO,@NowTime,@FunName,@IDNO,@NowTime,
						MEMIDNO,MEMRFNBR,@Main,1,1,@BLOCK_EDATE
					FROM TB_MemberData WITH(NOLOCK)
					WHERE MEMIDNO=@IDNO;

					INSERT INTO TB_MemberScoreBlock (A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,MEMIDNO,START_DT,END_DT)
					VALUES(@FunName,@IDNO,@NowTime,@FunName,@IDNO,@NowTime,@IDNO,@NowTime,@BLOCK_EDATE);
				END
			END
			ELSE
			BEGIN
				-- 取得主檔的積分總分/黑名單次數/是否為黑名單
				SELECT @Main=SCORE,@BLOCK_CNT=BLOCK_CNT,@ISBLOCK=ISBLOCK FROM TB_MemberScoreMain WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

				SET @Main = @Main + @Detail;	-- 總積分 = 總積分 + 明細積分

				IF @Main > 50
				BEGIN
					UPDATE TB_MemberScoreMain
					SET SCORE=@Main,
						U_PRGID=@FunName,
						U_USERID=@IDNO,
						U_SYSDT=@NowTime
					WHERE MEMIDNO=@IDNO;
				END
				ELSE
				BEGIN
					IF @ISBLOCK = 0
					BEGIN
						SET @BLOCK_CNT = @BLOCK_CNT + 1;	-- 低於50分，黑名單次數+1

						IF @BLOCK_CNT >= 3	-- 3次就永久停權
						BEGIN
							SET @BLOCK_EDATE=CONVERT(datetime, '2099-12-31 23:59:59', 120);
						END

						UPDATE TB_MemberScoreMain
						SET SCORE=@Main,
							BLOCK_CNT=@BLOCK_CNT,
							ISBLOCK=1,
							BLOCK_EDATE=@BLOCK_EDATE,
							U_PRGID=@FunName,
							U_USERID=@IDNO,
							U_SYSDT=@NowTime
						WHERE MEMIDNO=@IDNO;

						INSERT INTO TB_MemberScoreBlock (A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,MEMIDNO,START_DT,END_DT)
						VALUES(@FunName,@IDNO,@NowTime,@FunName,@IDNO,@NowTime,@IDNO,@NowTime,@BLOCK_EDATE);
					END
					ELSE
					BEGIN
						-- 20210705 UPD BY YEH REASON:阿舒說已經是黑名單的只做積分加總，其餘欄位不動
						UPDATE TB_MemberScoreMain
						SET SCORE=@Main,
							U_PRGID=@FunName,
							U_USERID=@IDNO,
							U_SYSDT=@NowTime
						WHERE MEMIDNO=@IDNO;
					END
				END
			END

			-- 已加總的明細資料，狀態改為已加總
			UPDATE TB_MemberScoreDetail
			SET ISPROCESSED=1,
				U_PRGID=@FunName,
				U_USERID=@IDNO,
				U_SYSDT=@NowTime
			FROM TB_MemberScoreDetail A
			INNER JOIN #ScoreDetail B ON B.SEQ=A.SEQ;
		END

		DROP TABLE IF EXISTS #ScoreDetail;
	END

	-- 寫入錯誤訊息
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CalMemberScore';
GO

