/***********************************************************************************************
* Serve    : sqyhi03azt.database.windows.net
* Database : IRENT_V2T
* 程式名稱 : usp_InsHotaiMember_I01
* 系    統 : IRENT
* 程式功能 : 新增和泰客戶綁定
* 作    者 : Po Yu
* 撰寫日期 : 20211205
* 修改日期 : 
Example :
***********************************************************************************************/ 
Create Procedure [dbo].[usp_InsHotaiMember_I01]
@IDNO varchar(10),
@OneID varchar(50),
@RefreshToken varchar(50),
@AccessToken varchar(1000),
@LogID BIGINT,
@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT
DECLARE @IsSystem TINYINT
DECLARE @FunName VARCHAR(50)
DECLARE @ErrorType TINYINT
DECLARE @hasData INT
DECLARE @NowTime DATETIME

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_InsHotaiMember_I01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=DATEADD(hh,8,GETDATE());

BEGIN TRY
IF EXISTS(SELECT TOP 1 1 FROM TB_MemberHotai WITH(NOLOCK) WHERE IDNO=@IDNO)
	BEGIN
		BEGIN TRAN 
			UPDATE TB_MemberHotai SET RefreshToken=@RefreshToken,AccessToken=@AccessToken,U_PRGID='HotaiWebView',U_USERID='HotaiWebView',U_SYSDT=@NowTime WHERE IDNO=@IDNO
		COMMIT TRAN
	END
ELSE
	BEGIN 
		BEGIN TRAN 
			INSERT INTO TB_MemberHotai(IDNO,OneID,RefreshToken,AccessToken,isCancel,CancelTime,A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT)
			VALUES(@IDNO,@OneID,@RefreshToken,@AccessToken,0,NULL,'HotaiWebView','HotaiWebView',@NowTime,'HotaiWebView','HotaiWebView',@NowTime)
		COMMIT TRAN
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
			INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
			VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
RETURN @Error
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsHotaiMember_I01';