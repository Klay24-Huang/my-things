/****************************************************************
* Serve    : sqyhi03vm
* Database : IRENT_V2
* 程式吊稱 : usp_CheckMobileUse
* 系    統 : IRENT
* 程式功能 : 判斷手機號碼是否已被用過


* 作    者 : 唐瑋祁


* 撰寫日期 : 20210928
* 修改日期 : 20211019 UPD BY 唐瑋祁.發送MAIL判斷修改

			 20211130 ADD BY ADAM REASON.調整邏輯

			 20211207 UPD BY YEH REASON:加上HasVaildEMail=1的條件

			 20211207 UPD BY YEH REASON:BUG修正



DECLARE @mail               VARCHAR(100);
DECLARE @Error               INT;
DECLARE @ErrorCode 			VARCHAR(6);		
DECLARE @ErrorMsg  			NVARCHAR(100);
DECLARE @SQLExceptionCode	VARCHAR(10);		
DECLARE @SQLExceptionMsg		NVARCHAR(1000);
EXEC @Error=[dbo].[usp_CheckMobileUse] '88422',908,@mail OUTPUT,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
SELECT @mail,@Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_CheckMobileUse]
	@Mobile				VARCHAR(10)           , --身分證字號


	@LogID				BIGINT                ,
	@mail				VARCHAR(100)    OUTPUT,	
	@ErrorCode			VARCHAR(6)		OUTPUT,	--回傳錯誤代碼


	@ErrorMsg			NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息


	@SQLExceptionCode	VARCHAR(10)		OUTPUT,	--回傳sqlException代碼


	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息


AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @ID VARCHAR(10);
DECLARE @NowDate DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_CheckMobileUse';
SET @IsSystem=0;
SET @ErrorType=0;
SET @ID=ISNULL(@Mobile,'');--前端傳進來的ID重新命吊

SET @NowDate=DATEADD(HOUR,8,GETDATE());
SET @mail='';

BEGIN TRY
	IF @ID=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900';
	END

	IF @Error=0
	BEGIN
		-- 取得此帳號驗證的手機號碼

		SET @Mobile=ISNULL((SELECT TOP 1 Mobile FROM TB_VerifyCode WITH(NOLOCK) WHERE IDNO=@ID AND IsVerify=1 ORDER BY SendTime DESC),'');

		-- 取得此手機號碼的最後一筆EMAIL
		-- 20211207 UPD BY YEH REASON:加上HasVaildEMail=1的條件

		SELECT TOP 1 @mail=MEMEMAIL FROM TB_MemberData WITH(NOLOCK) WHERE MEMTEL=@Mobile AND HasCheckMobile=1 AND HasVaildEMail=1 AND MEMIDNO!=@ID ORDER BY U_SYSDT DESC;

		--20211019唐加

		IF EXISTS (SELECT * FROM TB_MemberData WITH(NOLOCK) WHERE MEMTEL=@Mobile AND HasCheckMobile=1 AND MEMIDNO!=@ID)
		BEGIN
			DROP TABLE IF EXISTS #ID2;

			--20211130 ADD BY ADAM REASON.調整邏輯

			SELECT MEMIDNO INTO #ID2 FROM TB_MemberData WITH(NOLOCK) WHERE MEMTEL=@Mobile AND HasCheckMobile=1 AND MEMIDNO!=@ID;

			UPDATE TB_MemberData
			SET HasCheckMobile=0
				,U_SYSDT=@NowDate
				,U_USERID=@ID
				,U_PRGID=12
			WHERE MEMTEL=@MOBILE 
			AND HasCheckMobile=1
			AND MEMIDNO IN (SELECT MEMIDNO FROM #ID2);	--20211130 ADD BY ADAM REASON.調整邏輯	20211207 UPD BY YEH REASON:BUG修正


			INSERT INTO TB_MemberData_Log
			SELECT 'U','12',@NowDate,*
			FROM TB_MemberData WITH(NOLOCK)
			WHERE MEMIDNO IN (SELECT MEMIDNO FROM #ID2)	--20211130 ADD BY ADAM REASON.調整邏輯


			DROP TABLE IF EXISTS #ID2;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckMobileUse';
GO