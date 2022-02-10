
/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_InsOrderAuthReservation_I01
* 系    統 : IRENT
* 程式功能 : 寫入授權
* 作    者 : Umeko
* 撰寫日期 : 20211102
* 修改日期 : 

* Example  : 
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_InsOrderAuthReservation_I01]
	@order_number      bigint,
	@IDNO					    varchar(10),
	@final_price             int,
	@CardType               int,
	@AuthType               int,
	@GateNO                  int,
	@isRetry                    int,
	@AutoClose              int,
	@PrgName               varchar(50),
	@PrgUser                 varchar(20),
	@AppointmentTime       datetime,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息        
AS


DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;

DECLARE @NowTime DATETIME;

Declare @ProID varchar(20)

IF Exists(Select 1 From [dbo].[TB_APIList] with(nolock) Where APIName = @PrgName )
Begin
	Select @ProID = APIID From [dbo].[TB_APIList] with(nolock) Where APIName = @PrgName
End
else
Begin
	Set @ProID = left(@PrgName,20)
End

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_InsOrderAuthReservation_I01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=dbo.GET_TWDATE()

BEGIN TRY
	 INSERT INTO TB_OrderAuthReservation
					(A_PRGID, A_USERID, A_SYSDT, U_PRGID, U_USERID, U_SYSDT, order_number, final_price, 
								AuthFlg, AuthMessage,IDNO,
								CardType,AuthType,GateNO,isRetry,AutoClose,AppointmentTime)
	SELECT A_PRGID = @ProID
		, A_USERID= @PrgUser
		, A_SYSDT= @NowTime
		, U_PRGID= @ProID
		, U_USERID= @PrgUser
		, U_SYSDT=@NowTime
		, order_number = @order_number
		, final_price = @final_price
		, AuthFlg=0
		, AuthMessage =''
		,IDNO = @IDNO
		,CardType = @CardType
		,AuthType = @AuthType
		,GateNO = @GateNO
		,isRetry = @isRetry
		,AutoClose = @AutoClose
		,AppointmentTime = @AppointmentTime

	--寫入錯誤訊息
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,0,@IsSystem);
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
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,0,@IsSystem);
END CATCH
RETURN @Error


EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsOrderAuthReservation_I01';