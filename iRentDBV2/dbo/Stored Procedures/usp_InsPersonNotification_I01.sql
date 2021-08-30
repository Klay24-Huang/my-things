
/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_InsPersonNotification_I01
* 系    統 : IRENT
* 程式功能 : 新增個人推播訊息
* 作    者 : Umeko
* 撰寫日期 : 20210826
* 修改日期 :
Example :
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_InsPersonNotification_I01]
	@OrderNo                BIGINT                ,	--訂單編號
	@IDNO                   VARCHAR(10)           ,	--帳號
	@NType					Tinyint				  ,
	@STime                  DateTime              ,
	@Title					nvarchar(500)         ,
	@Message				nvarchar(500)         ,
	@url				    varchar(500)          ,
	@imageurl				varchar(500)          ,
	@LogID                  BIGINT                ,	--執行的api log
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
BEGIN
	DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_InsPersonNotification_I01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);

BEGIN TRY
	IF @IDNO=''
	Begin
		Set @Error = 1
		Set @ErrorCode = 'ERR922'
	End

	Declare @UserName nvarchar(20)
	Declare @UserToken varchar(1024)
	Declare @DataCount int = 0
	IF @Error=0
	BEGIN
		Select @UserName = MEMCNAME,@UserToken = PushREGID
		From TB_MemberData with(nolock)
		Where MEMIDNO = @IDNO

		Set @DataCount = @@ROWCOUNT
	END

	IF @DataCount = 0
	Begin
		Set @Error = 1
		Set @ErrorCode = 'ERR922'
	End

	IF @Error=0
	BEGIN
		--Insert into TB_PersonNotification(OrderNum,IDNO,NType,UserName,UserToken,STime,Title,[Message],[url],[imageUrl])
		--Values(@OrderNo,@IDNO,@NType,@UserName,@UserToken,@STime,@Title,@Message,@url,@imageurl)
		Insert into TB_PersonNotification(OrderNum,IDNO,NType,UserName,UserToken,STime,Title,[Message],[url])
		Values(@OrderNo,@IDNO,@NType,@UserName,@UserToken,@STime,@Title,@Message,@url)

		If @@ERROR <> 0 And @@ROWCOUNT = 0
		Begin
			Set @Error = 1
			Set @ErrorCode = 'ERR923'
		End
	End

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsPersonNotification_I01';
END