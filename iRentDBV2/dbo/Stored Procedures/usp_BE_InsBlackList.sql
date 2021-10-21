/****** Object:  StoredProcedure [dbo].[usp_BE_InsBlackList_TEST]    Script Date: 2021/10/19 下午 05:12:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****************************************************************
* Serve    : sqyhi03vm
* Database : IRENT_V2
* 程式名稱 : usp_BE_InsBlackList
* 系    統 : IRENT
* 程式功能 : 手機黑名單新增
* 作    者 : 唐瑋祁
* 撰寫日期 : 2021/9/15
* 修改日期 : 
2021/10/14 UPD BY 唐瑋祁

DECLARE @Error INT;
DECLARE
	@ErrorCode 				VARCHAR(6)		,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)		--回傳sqlException訊息
EXEC @Error=usp_BE_InsBlackList '0','0917997857','80345' ,@ErrorCode  OUTPUT ,@ErrorMsg  OUTPUT ,@SQLExceptionCode  OUTPUT ,@SQLExceptionMsg  OUTPUT
select @Error,@ErrorCode,@SQLExceptionMsg,@SQLExceptionCode,@ErrorMsg 

DECLARE @Error INT;
DECLARE
	@ErrorCode 				VARCHAR(6)		,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)		--回傳sqlException訊息
EXEC @Error=usp_BE_InsBlackList '1','0917997857','80345' ,@ErrorCode  OUTPUT ,@ErrorMsg  OUTPUT ,@SQLExceptionCode  OUTPUT ,@SQLExceptionMsg  OUTPUT
select @Error,@ErrorCode,@SQLExceptionMsg,@SQLExceptionCode,@ErrorMsg 
*****************************************************************/
ALTER PROCEDURE [dbo].[usp_BE_InsBlackList]	
	@Mode         int,
	@Mobile       VARCHAR(10),
	@USERID		  VARCHAR(20),
	@MEMO		  VARCHAR(20),
	@ErrorCode 			 VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  			 NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode	 VARCHAR(10)	OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg	 NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @Now datetime;
DECLARE @ID VARCHAR(10);
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_BE_InsBlackList';
SET @IsSystem=0;
SET @ErrorType=1;
SET @Now=DATEADD(HOUR,8,GETDATE())
SET @ID=(SELECT MEMIDNO FROM TB_MemberData WITH(NOLOCK) WHERE MEMTEL=@Mobile AND HasCheckMobile=1)

BEGIN TRY
	IF NOT EXISTS(SELECT 1 FROM TB_MemberData WITH(NOLOCK) WHERE MEMTEL=@Mobile) AND @Mode=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
		SET @ErrorMsg=@Mobile+'手機號碼不存在於會員資料庫中';
 	END

	IF NOT EXISTS(SELECT 1 FROM TB_MemberDataBlockMobile WITH(NOLOCK) WHERE Mobile=@Mobile) AND @Mode=1
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
		SET @ErrorMsg=@Mobile+'手機號碼不存在於黑名單';
 	END

	IF EXISTS(SELECT 1 FROM TB_MemberDataBlockMobile WITH(NOLOCK) WHERE Mobile=@Mobile) AND @Mode=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
		SET @ErrorMsg=@Mobile+'手機號碼已存在於黑名單';
 	END

	IF @Error=0
	BEGIN
		IF (@Mode=0)--新增
		BEGIN
			INSERT INTO TB_MemberDataBlockMobile(Mobile,CreateDate,MEMO)
			values(@Mobile,@Now,@MEMO)

			INSERT INTO TB_MemberDataBlockMobile_Log(A_SYSDT,Mobile,CreateDate,Valid,USERID,MEMO)
			values(@Now,@Mobile,@Now,'Y',@USERID,@MEMO)

			UPDATE TB_MemberData set HasCheckMobile=0,U_SYSDT=@Now,U_USERID=@USERID
			where MEMTEL=@Mobile AND HasCheckMobile=1

			INSERT INTO TB_MemberData_Log
			SELECT 'U','',@Now,* FROM TB_MemberData WHERE MEMIDNO=@ID;
		END
		ELSE IF(@Mode=1)--刪除
		BEGIN
			INSERT INTO TB_MemberDataBlockMobile_Log(A_SYSDT,Mobile,CreateDate,Valid,USERID,MEMO)
			values(@Now,@Mobile,(SELECT CreateDate FROM TB_MemberDataBlockMobile WHERE Mobile=@Mobile),'N',@USERID,@MEMO)

			DELETE TB_MemberDataBlockMobile WHERE Mobile=@Mobile
		END
		ELSE IF(@Mode=2)--修改
		BEGIN
			UPDATE TB_MemberDataBlockMobile SET MEMO=@MEMO WHERE Mobile=@Mobile

			INSERT INTO TB_MemberDataBlockMobile_Log(A_SYSDT,Mobile,CreateDate,Valid,USERID,MEMO)
			values(@Now,@Mobile,(SELECT CreateDate FROM TB_MemberDataBlockMobile WHERE Mobile=@Mobile),'N',@USERID,@MEMO)		
		END
	END
	--寫入錯誤訊息
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,'',@IsSystem);
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
	SET @ErrorType=1;
	INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,'',@IsSystem);
END CATCH

RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_InsBlackList';