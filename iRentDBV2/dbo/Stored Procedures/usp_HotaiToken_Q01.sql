/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_HotaiToken_Q01
* 系    統 : IRENT
* 程式功能 : 查詢和泰會員Token
* 作    者 : AMBER
* 撰寫日期 : 20211117 
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_HotaiToken_Q01]
(   
	@IDNO		            VARCHAR(10)			  ,	--身分證號
	@AccessToken  			NVARCHAR(600)	OUTPUT,
	@RefreshToken  			NVARCHAR(60)	OUTPUT,
	@OneID  			    VARCHAR (50)	OUTPUT,
	@ErrorCode 				VARCHAR (6)	    OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
)
AS
DECLARE @Error      INT = 0;
DECLARE @IsSystem   TINYINT = 1;
DECLARE @ErrorType  TINYINT = 4;
DECLARE @FunName    VARCHAR(50) = 'usp_HotaiToken_Q01';
DECLARE @LogID      BIGINT=0

SET @ErrorCode= '0000';
SET @ErrorMsg= 'SUCCESS';
SET @SQLExceptionCode= '';	
SET @SQLExceptionMsg= '';	
SET @IDNO = ISNULL(@IDNO,'');

BEGIN
	BEGIN TRY
		IF @IDNO=''
		BEGIN
			SET @Error=1
			SET @ErrorCode = 'ERR900'
		END

		IF @Error = 0
		BEGIN
			   SELECT @AccessToken=AccessToken,@RefreshToken=RefreshToken,@OneID=OneID FROM TB_MemberHotai WITH(NOLOCK) WHERE IDNO=@IDNO;
		       
			   IF @@ROWCOUNT =0
			   BEGIN
			      SET @Error=1
			      SET @ErrorCode = 'ERR953'
			   END			   
			   ELSE IF @AccessToken='' OR @RefreshToken=''
			   BEGIN
			      SET @Error=1
			      SET @ErrorCode = 'ERR941'
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

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH
	RETURN @Error
END


