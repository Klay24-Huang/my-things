/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_AutoStoreSetting_U01
* 系    統 : IRENT
* 程式功能 : 自動儲值設定
* 作    者 : YEH
* 撰寫日期 : 20210928
* 修改日期 : 

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_AutoStoreSetting_U01]
	@IDNO				VARCHAR(10)				,	-- 帳號
	@Token				VARCHAR(1024)			,	-- TOKEN
	@AutoStored			INT						,	-- 是否同意自動儲值 (0:不同意 1:同意)
	@ApiName			VARCHAR(100)			,	-- APIName
	@LogID				BIGINT					,	-- LogID
	@ErrorCode			VARCHAR(6)		OUTPUT	,	-- 回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	-- 回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	-- 回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		-- 回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;
DECLARE @ApiID INT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_AutoStoreSetting_U01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @Token=ISNULL(@Token,'');
SET @ApiID=0;

BEGIN TRY
	IF @Token='' OR @IDNO=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900';
	END

	-- 再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END

	IF @Error=0
	BEGIN
		SELECT @ApiID=ISNULL(APIID,999) FROM TB_APIList WITH(NOLOCK) WHERE APIName=@ApiName;

		SET @hasData=0;
		SELECT @hasData=COUNT(1) FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR213';
		END
		ELSE
		BEGIN
			UPDATE TB_MemberData
			SET AutoStored=@AutoStored,
				U_PRGID=@ApiID,
				U_USERID=@IDNO,
				U_SYSDT=@NowTime
			WHERE MEMIDNO=@IDNO;

			-- LOG檔
			INSERT INTO TB_MemberData_Log
			SELECT 'U',@ApiID,@NowTime,* FROM TB_MemberData WHERE MEMIDNO=@IDNO;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SetDefPayMode';
GO