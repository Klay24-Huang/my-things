/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_SetMemberCMK
* 系    統 : IRENT
* 程式功能 : 更新會員條款
* 作    者 : YEH
* 撰寫日期 : 20210810
* 修改日期 : 20210817;UPD BY YEH REASON:更新會員主檔(活動及優惠訊息通知)
			 20210823;UPD BY YEH REASON:MemberData有資料才更新
			 20211105 UPD BY YEH REASON:增加JOIN設定檔，只回傳要拋短租的資料

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_SetMemberCMK]
	@IDNO				VARCHAR(10)				,	-- 帳號
	@VerType			VARCHAR(20)				,	-- 同意書版本類型
	@Version			VARCHAR(20)				,	-- 同意書版本號
	@SeqNo				INT						,	-- 流水號
	@Source				VARCHAR(1)				,	-- 同意來源管道 (I:IRENT，W:官網)
	@AgreeDate			DATETIME				,	-- 同意時間
	@TEL				VARCHAR(1)				,	-- 電話通知狀態 (N:不通知、Y:通知)
	@SMS				VARCHAR(1)				,	-- 簡訊通知狀態 (N:不通知、Y:通知)
	@EMAIL				VARCHAR(1)				,	-- EMAIL通知 (N:不通知、Y:通知)
	@POST				VARCHAR(1)				,	-- 郵寄通知 (N:不通知、Y:通知)
	@APIName			VARCHAR(50)				,	-- 來源程式
	@LogID				BIGINT					,
	@ErrorCode			VARCHAR(6)		OUTPUT	,	-- 回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	-- 回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	-- 回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		-- 回傳sqlException訊息
AS
DECLARE @Error		INT;
DECLARE @IsSystem	TINYINT;
DECLARE @FunName	VARCHAR(50);
DECLARE @ErrorType	TINYINT;
DECLARE @hasData	TINYINT;
DECLARE @NowTime	DATETIME;
DECLARE @MEMRFNBR	INT;	-- 短租會員流水號
DECLARE @APIID		INT;
DECLARE @MEMMSG		VARCHAR(1);	-- 活動及優惠訊息通知(Y:是 N:否)

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_SetMemberCMK';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @AgreeDate=ISNULL(@AgreeDate,@NowTime);
SET @VerType=ISNULL(@VerType,'');
SET @Version=ISNULL(@Version,'');
SET @SeqNo=ISNULL(@SeqNo,0);

BEGIN TRY
	IF @IDNO='' OR @SeqNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900';
	END

	IF @Error=0
	BEGIN
		-- 取得同意書版本類型
		IF @VerType = ''
		BEGIN
			SELECT @VerType=VerType FROM TB_CMKDef WITH(NOLOCK) WHERE SeqNo=@SeqNo;
		END
		-- 取得同意書版本號
		IF @Version = ''
		BEGIN
			SELECT TOP 1 @Version=Version FROM TB_CMKDef WITH(NOLOCK) WHERE VerType=@VerType AND @NowTime >= SDATE ORDER BY Version DESC;
		END

		-- 20210823;UPD BY YEH REASON:MemberData有資料才更新
		IF EXISTS(SELECT * FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO)
		BEGIN
			-- 取得短租會員流水號、活動及優惠訊息通知
			SELECT @MEMRFNBR=MEMRFNBR,@MEMMSG=MEMMSG FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

			-- 取得APIID
			SELECT @APIID=APIID FROM TB_APIList WITH(NOLOCK) WHERE APIName=@APIName;

			-- 更新行銷主檔
			IF EXISTS(SELECT * FROM TB_MemberCMK WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND VerType=@VerType)
			BEGIN
				UPDATE TB_MemberCMK
				SET Version=@Version,
					Source=@Source,
					AgreeDate=@AgreeDate,
					TEL=@TEL,
					SMS=@SMS,
					EMAIL=@EMAIL,
					POST=@POST,
					U_PRGID=@APIID,
					U_USERID=@IDNO,
					U_SYSDT=@NowTime
				WHERE MEMIDNO=@IDNO
				AND VerType=@VerType;
			END
			ELSE
			BEGIN
				INSERT INTO TB_MemberCMK (MEMIDNO,MEMRFNBR,VerType,Version,Source,AgreeDate,
										TEL,SMS,EMAIL,POST,
										A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT)
				VALUES(@IDNO,@MEMRFNBR,@VerType,@Version,@Source,@AgreeDate,
						@TEL,@SMS,@EMAIL,@POST,
						@APIID,@IDNO,@NowTime,@APIID,@IDNO,@NowTime);
			END

			-- 寫LOG
			INSERT INTO TB_MemberCMK_LOG
			SELECT * FROM TB_MemberCMK WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND VerType=@VerType;

			-- 20210817;UPD BY YEH REASON:更新會員主檔(活動及優惠訊息通知)
			IF @MEMMSG <> @EMAIL
			BEGIN
				UPDATE TB_MemberData
				SET MEMMSG=@EMAIL,
					U_PRGID=@APIID,
					U_USERID=@IDNO,
					U_SYSDT=@NowTime
				WHERE MEMIDNO=@IDNO;

				-- 寫LOG
				INSERT INTO TB_MemberData_Log
				SELECT 'U',@APIID,@NowTime,* FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
			END
		END

		-- 取得要回傳的資料
		-- 20211105 UPD BY YEH REASON:增加JOIN設定檔，只回傳要拋短租的資料
		SELECT A.MEMIDNO,A.VerType,A.Version,A.Source,A.AgreeDate,A.TEL,A.SMS,A.EMAIL,A.POST 
		FROM TB_MemberCMK A WITH(NOLOCK)
		INNER JOIN TB_CMKDef B WITH(NOLOCK) ON B.VerType=A.VerType AND B.Version=A.Version
		WHERE A.MEMIDNO=@IDNO AND A.VerType=@VerType AND B.Trans=1;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SetMemberCMK';
GO