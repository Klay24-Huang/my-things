/****** Object:  StoredProcedure [dbo].[usp_BE_InsBlackList2_TEST]    Script Date: 2021/10/19 下午 05:12:15 ******/
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
2021/10/15 UPD BY 唐瑋祁
--20211021 ADD BY 唐瑋祁 REASON.上版

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
ALTER PROCEDURE [dbo].[usp_BE_InsBlackList2]	
	@Mode         int,
	@Mobile       VARCHAR(10),
	@USERID		  VARCHAR(20),
	@MEMO		  VARCHAR(20),		--20211021 ADD BY 唐瑋祁 REASON.上版
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

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_BE_InsBlackList2';
SET @IsSystem=0;
SET @ErrorType=1;
SET @Now=DATEADD(HOUR,8,GETDATE())

BEGIN TRY
	UPDATE TB_MemberData set HasCheckMobile=0,U_SYSDT=@Now--U_USERID=@USERID
	,MEMONEW2=''+@USERID+'於('+CONVERT(CHAR(8),@Now,112)+')嘗試使用黑名單手機號碼'+@Mobile+'驗證，請留意' where MEMIDNO=@USERID

	INSERT INTO TB_MemberData_Log
	SELECT 'U','',@Now,* FROM TB_MemberData WHERE MEMIDNO=@USERID;
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