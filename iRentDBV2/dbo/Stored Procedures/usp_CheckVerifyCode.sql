
/****************************************************************
* Serve    : sqyhi03vm
* Database : IRENT_V2
* 程式名稱 : usp_CheckVerifyCode
* 系    統 : IRENT
* 程式功能 : 判斷手機驗證碼是否正確
* 作    者 : Eric
* 撰寫日期 : 2020/8/5 上午 05:33:13 
* 修改日期 : 20211019 UPD BY 唐瑋祁.把驗證通過將其他同門號手機押為未認證的判斷移到usp_CheckMobileUse(上線後才會註解，我先寫珮綺帳號)

DECLARE @mail               VARCHAR(100);
DECLARE @Error               INT;
DECLARE @ErrorCode 			VARCHAR(6);		
DECLARE @ErrorMsg  			NVARCHAR(100);
DECLARE @SQLExceptionCode	VARCHAR(10);		
DECLARE @SQLExceptionMsg		NVARCHAR(1000);
EXEC @Error=[dbo].[usp_CheckMobileUse] '0928250058',908,@mail OUTPUT,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
SELECT @mail,@Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
*****************************************************************/
ALTER PROCEDURE [dbo].[usp_CheckVerifyCode]
	@IDNO                   VARCHAR(10)           , --帳號
	@DeviceID               VARCHAR(128)          , --機號
	@OrderNum               VARCHAR(20)           , --訂單編號
	@Mode                   TINYINT               , --0:註冊;1:忘記密碼;2:一次性開門;3:更換手機
	@VerifyCode             VARCHAR(6)            , --簡訊驗證碼
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @NowTime DATETIME;
DECLARE @tmpVerifyCode VARCHAR(6);
DECLARE @VerifyCodeID BIGINT;
DECLARE @DeadLine DATETIME;
DECLARE @MOBILE VARCHAR(10);
DECLARE @LogFlag VARCHAR(1);
DECLARE @ID2 VARCHAR(15);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_CheckVerifyCode';
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @IDNO=ISNULL (@IDNO,'');
SET @DeviceID=ISNULL (@DeviceID,'');
SET @OrderNum=ISNULL (@OrderNum,'');
SET @VerifyCode=ISNULL(@VerifyCode,'');
SET @Mode=ISNULL(@Mode,9);
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @tmpVerifyCode='';
SET @VerifyCodeID=0;
SET @DeadLine=GETDATE();
SET @LogFlag='';

BEGIN TRY

	IF @DeviceID='' OR @IDNO=''  OR @VerifyCode='' OR @Mode=9 OR (@OrderNum='' AND @Mode=2)
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		IF @Mode=0 OR @Mode=3
		BEGIN
			--模式(0:註冊時) 3:更換手機
			SELECT TOP 1 @VerifyCodeID=ISNULL(VerifyCodeID,0),
				@tmpVerifyCode=ISNULL(VerifyNum,''),
				@DeadLine=ISNULL(DeadLine,GETDATE()),
				@MOBILE=Mobile
			FROM TB_VerifyCode  WITH(NOLOCK)
			WHERE IDNO=@IDNO AND Mode=@Mode AND IsVerify=0 
			ORDER BY SendTime DESC;

			IF @VerifyCodeID>0 AND @tmpVerifyCode=@VerifyCode 
			BEGIN
				IF @NowTime<@DeadLine
				BEGIN
					UPDATE TB_VerifyCode
					SET IsVerify=1 
					WHERE VerifyCodeID=@VerifyCodeID;

					--確認無待審資料
					SELECT @hasData=Count(1) FROM [TB_MemberDataOfAutdit] WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND MEMTEL=@MOBILE AND HasAudit=0;
					IF @hasData=0
					BEGIN
						IF EXISTS(SELECT MEMIDNO FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO)
						BEGIN
							UPDATE [TB_MemberData] 
							SET MEMTEL=@MOBILE,
								HasCheckMobile=1,
								U_PRGID=12,
								U_USERID=@IDNO,
								U_SYSDT=@NowTime
							WHERE MEMIDNO=@IDNO;

							SET @LogFlag='U';
						END
						ELSE
						BEGIN
							INSERT INTO TB_MemberData(MEMIDNO,MEMTEL,HasCheckMobile,A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT)
							VALUES(@IDNO,@MOBILE,1,12,@IDNO,@NowTime,12,@IDNO,@NowTime);

							SET @LogFlag='A';
						END

						-- 20210225;新增LOG檔
						INSERT INTO TB_MemberData_Log
						SELECT @LogFlag,'12',@NowTime,* FROM TB_MemberData WHERE MEMIDNO=@IDNO;
					END
					ELSE
					BEGIN
						IF EXISTS(SELECT MEMIDNO FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO)
						BEGIN
							UPDATE [TB_MemberData] 
							SET MEMTEL=@MOBILE,
								HasCheckMobile=1,
								U_PRGID=12,
								U_USERID=@IDNO,
								U_SYSDT=@NowTime
							WHERE MEMIDNO=@IDNO;

							SET @LogFlag='U';
						END
						ELSE
						BEGIN
							INSERT INTO TB_MemberData(MEMIDNO,MEMTEL,HasCheckMobile,A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT)
							VALUES(@IDNO,@MOBILE,1,12,@IDNO,@NowTime,12,@IDNO,@NowTime);

							SET @LogFlag='A';
						END

						-- 20210225;新增LOG檔
						INSERT INTO TB_MemberData_Log
						SELECT @LogFlag,'12',@NowTime,* FROM TB_MemberData WHERE MEMIDNO=@IDNO;
					END

					--20210323 ADD BY JERRY 驗證通過將其他同門號手機押為未認證
					--IF EXISTS (SELECT * FROM TB_MemberData WITH(NOLOCK) WHERE MEMTEL=@MOBILE AND HasCheckMobile=1 AND MEMIDNO!=@IDNO) 
					--BEGIN
					--	SET @ID2 = (SELECT MEMIDNO FROM TB_MemberData WHERE MEMTEL=@MOBILE AND HasCheckMobile=1 AND MEMIDNO!=@IDNO)

					--	UPDATE TB_MemberData
					--	SET HasCheckMobile=0
					--		,U_SYSDT=@NowTime
					--		,U_USERID=@IDNO
					--		,U_PRGID=12
					--	WHERE MEMTEL=@MOBILE 
					--	AND HasCheckMobile=1 
					--	AND MEMIDNO!=@IDNO;
					--	--20210225唐加Log，沒Log玩個屁
					--	INSERT INTO TB_MemberData_Log
					--	SELECT 'U','12',@NowTime,* FROM TB_MemberData WHERE MEMIDNO=@ID2;
					--END	
				END
				ELSE
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR141';
				END
			END
			ELSE
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR131';
			END
		END
		ELSE IF @Mode=1
		BEGIN
			--模式(1:忘記密碼)
			SELECT TOP 1 @VerifyCodeID=ISNULL(VerifyCodeID,0),@tmpVerifyCode=ISNULL(VerifyNum,''),@DeadLine=ISNULL(DeadLine,GETDATE()) 
			FROM TB_VerifyCode WITH(NOLOCK) WHERE IDNO=@IDNO AND Mode=1  
			ORDER BY SendTime DESC;

			IF @VerifyCodeID>0 AND @tmpVerifyCode=@VerifyCode 
			BEGIN
				IF @NowTime<@DeadLine
				BEGIN
					UPDATE TB_VerifyCode
					SET IsVerify=1 
					WHERE VerifyCodeID=@VerifyCodeID;
				END
				ELSE
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR142';
				END
			END
			ELSE
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR131';
			END
		END
		ELSE IF @Mode=2
		BEGIN
			--模式(2:一次性開門)
			SELECT TOP 1 @VerifyCodeID=ISNULL(VerifyCodeID,0),@tmpVerifyCode=ISNULL(VerifyNum,''),@DeadLine=ISNULL(DeadLine,GETDATE()) 
			FROM TB_VerifyCode WITH(NOLOCK) WHERE IDNO=@IDNO AND Mode=2 AND OrderNum=@OrderNum 
			ORDER BY SendTime DESC;

			IF @VerifyCodeID>0 AND @tmpVerifyCode=@VerifyCode
			BEGIN
				IF @NowTime<@DeadLine
				BEGIN
					UPDATE TB_VerifyCode
					SET IsVerify=1 
					WHERE VerifyCodeID=@VerifyCodeID;
				END
				ELSE
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR531';
				END
			END
			ELSE
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR131';
			END
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckVerifyCode';