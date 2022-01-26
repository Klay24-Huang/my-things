/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_HotaiGetDefaultCard_Q01
* 系    統 : IRENT
* 程式功能 : 和泰預設卡片失效
* 作    者 : Umeko
* 撰寫日期 :  20211124
* 修改日期 :  20211125 UPD BY AMBER REASON: 預設付費方式改為信用卡 PayMode(0:信用卡 4:和泰PAY)             
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_HotaiDefaultCardUnbind_U01]
	@IDNO varchar(20),
	@HotaiCardID			INT	,	
	@LogID                  BIGINT ,
	@U_FuncName             VARCHAR(50),
	@U_USERID           VARCHAR(20),
	@ErrorCode 			  VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  			 NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
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

SET @FunName='usp_HotaiDefaultCardUnbind_U01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime =dbo.GET_TWDATE();
SET @IDNO=ISNULL (@IDNO,'');

BEGIN TRY
	IF @IDNO=''
		BEGIN
			SET @Error=1
			SET @ErrorCode = 'ERR900'
		END
		IF @HotaiCardID = 0
		Begin
			SET @Error=1
			SET @ErrorCode = 'ERR900'
		End

		Declare @U_PRGID varchar(50) = '0'
		IF @U_PRGID = 1
		Begin
			Select @U_PRGID = Convert(varchar(10),APIID) From TB_APIList with(nolock) Where APIName = @U_FuncName
		End
		Else
		Begin
			Set @U_PRGID = Left(@U_FuncName,50)
		End

		IF @U_USERID = ''
		Begin
			Set @U_USERID = @U_PRGID
		End

		IF @Error = 0 And Exists(Select 1 From TB_MemberHotaiCard Where HotaiCardID = @HotaiCardID And IDNO = @IDNO And isCancel = 0 )
		Begin
			

			Update TB_MemberHotaiCard
			Set isCancel = 1,U_PRGID = @U_PRGID,@U_USERID = @U_USERID,U_SYSDT =@NowTime
			Where HotaiCardID = @HotaiCardID And IDNO = @IDNO And isCancel = 0 

			--20211125 UPD BY AMBER REASON: 預設付費方式改為信用卡 PayMode(0:信用卡 4:和泰PAY) 
	        Update TB_MemberData set PayMode=0,U_PRGID=0,U_USERID=@U_USERID,U_SYSDT=@NowTime WHERE MEMIDNO=@IDNO AND PayMode=4
		End
		
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HotaiDefaultCardUnbind_U01';
END