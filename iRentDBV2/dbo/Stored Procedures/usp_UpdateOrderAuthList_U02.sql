/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_UpdateOrderAuthList_U02
* 系    統 : IRENT
* 程式功能 : 授權結果存檔
* 作    者 : Umeko
* 撰寫日期 : 20211101
* 修改日期 : 

* Example  : 
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_UpdateOrderAuthList_U02]
	@authSeq				BIGINT,   
	@AuthFlg				INT,     
	@AuthCode				VARCHAR(50), 
	@AuthMessage			NVARCHAR(120),        
	@OrderNo				BIGINT,          
	@transaction_no			VARCHAR(50),
	@AuthType             int,
	@isRetry                  int,
    @IDNO                          varchar(10),
	@AutoClosed				int,
	@final_price                 int,
	@ProName              	VARCHAR(50),
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
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_UpdateOrderAuthList_U02';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=dbo.GET_TWDATE();

BEGIN TRY
	UPDATE TB_OrderAuth
	SET AuthFlg=@AuthFlg,
		AuthCode=@AuthCode,
		AuthMessage=@AuthMessage,
		transaction_no=@transaction_no,
		U_USERID=@ProName,
		U_PRGID=@ProName,
		U_SYSDT=@NowTime
	WHERE authSeq=@authSeq
	
	Declare @iError int, @iErrorCode  VARCHAR(6)	,@iErrorMsg NVARCHAR(100),@iSQLExceptionCode VARCHAR(10),@iSQLExceptionMsg NVARCHAR(1000)

	IF @AuthFlg=1 
	BEGIN
		IF @AuthType  IN (1,5 )
		Begin
			--推播需要欄位
			Declare @Title nvarchar(500)
				,@Message nvarchar(500)=''
				,@url varchar(500) =''
				,@imageurl varchar(500) = ''
				,@ActionName nvarchar(10)
				,@STime datetime

				Set @ActionName = Case @AuthType When 1 then N'預約取授權' When 5 Then N'逾時取授權' Else '' End

				Set @STime = DateAdd(SECOND,10,@NowTime)
				Set @Title =  '取授權成功通知'
				Set  @Message =  CONCAT(N'已於',Format(@NowTime,'MM-dd hh:mm','zh-TW'),@ActionName,N'成功，N金額',@final_price,N'，謝謝!')

				Exec @iError = usp_InsPersonNotification_I01 
									  @OrderNo
									, @IDNO
									, 19				
									, @STime
									, @Title
									, @Message
									, @url
									, @imageurl
									, 123456
									, @iErrorCode output
									, @iErrorMsg output
									, @iSQLExceptionCode output
									, @iSQLExceptionMsg output

			End
			--授權成功且為最後一筆結案訂單準備傳送合約
		IF @AutoClosed = 1 And @AuthType =7
		Begin
		EXEC usp_SendReturnCarControl_I01 @OrderNo,'','','',''
		End
	END

	IF @AuthFlg = -1
	Begin
		Declare @Token VARCHAR(1024) 

		 IF @isRetry = 0 And @AuthType =1
		Begin
			Declare @StartTime  datetime,@AppointmentTime datetime			
			Select @StartTime = start_time From TB_OrderMain with(nolock) Where order_number = @OrderNo
			Set @AppointmentTime = DATEADD(hour,-4,@StartTime)

			--寫入2次授權預約
			EXEC @iError =  usp_InsOrderAuthReservation_I01 @OrderNo,@IDNO,@final_price
			,1,1,0,1,0,'UpdateOrderAuthList','UpdateOrderAuthList',@AppointmentTime
			,@iErrorCode output,@iErrorMsg output,@iSQLExceptionCode output,@iSQLExceptionMsg output
		End

		if(@isRetry = 1  And @AuthType =1)
		Begin
		     --寫入取消訂單
			Exec @iError = usp_BookingCancel_U01 @IDNO,@OrderNo,@Token,0,'授權失敗取消訂單',6,
			@iErrorCode output,@iErrorMsg output,@iSQLExceptionCode output,@iSQLExceptionMsg output
		End
	End

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UpdateOrderAuthList_U02';