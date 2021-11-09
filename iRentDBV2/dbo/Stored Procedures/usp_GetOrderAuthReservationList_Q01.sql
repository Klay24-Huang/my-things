


/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetOrderAuthReservationList_Q01
* 系    統 : IRENT
* 程式功能 : 取得授權清單
* 作    者 : Umeko
* 撰寫日期 : 20211108
* 修改日期 : 

* Example  : 
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetOrderAuthReservationList_Q01]
	@GateNo                INT,      
	@Retry                 INT,
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
DECLARE @AuthFlg DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetOrderAuthReservationList_Q01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=DATEADD(HOUR,8,GETDATE());

if @Retry = 0 
	Set @AuthFlg = 0

BEGIN TRY
	SELECT TOP 20
		A.authSeq,
		A.order_number,
		A.final_price,
		A.IDNO,
		transaction_no='',--B.MerchantTradeNo
		A.CardType,
		A.AuthType,
		A.AutoClose,
		A.isRetry,
		A.AuthFlg
	INTO #TB_OrderAuth
	FROM TB_OrderAuthReservation A WITH(NOLOCK) 
	Where AuthFlg = @AuthFlg And AppointmentTime >=@NowTime
	ORDER BY A.authSeq ASC
	
	--檢查訂單狀態
	Select order_number,cancel_status ,start_time into #TB_OrderMain
	From TB_OrderMain with(nolock)
	Where order_number in (Select order_number From #TB_OrderAuth)

	Update #TB_OrderAuth
	Set AuthFlg = -2
	From #TB_OrderAuth A 
	Where Exists(Select 1 From #TB_OrderMain Where order_number = A.order_number And  cancel_status <> 0 )

	--取出手機，失敗要發簡訊用
	Select IDNO = MEMIDNO,Mobile = MEMTEL into #MemberData
	From TB_MemberData with(nolock)
	Where MEMIDNO in (Select distinct IDNO From #TB_OrderAuth)
	And HasCheckMobile = 1

	--SELECT A.IDNO,CardToken=SUBSTRING(MAX(CONVERT(VARCHAR,A.MKTime,120)+A.CardToken),20,8000)
	--INTO #TB_MemberCardBinding
	--FROM TB_MemberCardBinding A WITH(NOLOCK)
	--JOIN #TB_OrderAuth B WITH(NOLOCK) ON A.IDNO=B.IDNO
	--WHERE A.IsValid=1
	--GROUP BY A.IDNO

	SELECT 
		A.authSeq,
		A.order_number,
		A.final_price,
		A.IDNO,
		A.transaction_no,
		A.CardType,
		A.AuthType,
		A.AutoClose,
		A.isRetry,
		A.AuthFlg,
		CardToken='',
		C.Mobile
	INTO #TB_OrderAuthList
	FROM #TB_OrderAuth A
	--LEFT JOIN #TB_MemberCardBinding B ON A.IDNO=B.IDNO
	Left Join #MemberData C ON A.IDNO = C.IDNO

	--信用卡且台新付款又不綁卡直接押失敗
	--Update #TB_OrderAuthList
	--Set AuthFlg = -1
	--Where CardType = 1 And CardToken = '' And AuthFlg = 0


	UPDATE A
	SET U_SYSDT=CASE WHEN A.AuthFlg=9 THEN U_SYSDT ELSE @NowTime END,
		AuthFlg=CASE WHEN B.AuthFlg < 0 THEN B.AuthFlg ELSE 9 END,
		AuthMessage=CASE WHEN B.AuthFlg = -1 THEN '查無生效綁卡資料'  
											 WHEN B.AuthFlg = -2 THEN '授權前手動取消訂單'  
		ELSE A.AuthMessage END,
		U_PRGID='AuthReservationList',
		U_USERID='AuthReservationList'
	FROM TB_OrderAuthReservation A
	JOIN #TB_OrderAuthList B ON A.authSeq=B.authSeq
	
	--針對押失敗的依狀態進行取消或發通知或重發
	--declare @authSeq varchar(14),@isRetry int,@order_number int,@IDNO varchar(10),@final_price int
	--,@CardType int,@AuthType int

	--DECLARE batch_OrderAuth CURSOR FOR
	--	Select authSeq, isRetry,order_number,IDNO,final_price
	--	from #TB_OrderAuthList Where  AuthFlg = -1 And AuthType = 1
	--OPEN batch_OrderAuth
	--FETCH NEXT FROM batch_OrderAuth into @authSeq, @isRetry ,@order_number,@IDNO,@final_price
	--WHILE @@FETCH_STATUS =0
	--BEGIN
	--	Declare @iError int, @iErrorCode  VARCHAR(6)	,@iErrorMsg NVARCHAR(100),@iSQLExceptionCode VARCHAR(10),@iSQLExceptionMsg NVARCHAR(1000)
	--	,@Token VARCHAR(1024) 

	--	if(@isRetry = 0)
	--	Begin
	--		Declare @StartTime  datetime,@AppointmentTime datetime			
	--		Select @StartTime = start_time From #TB_OrderMain Where order_number = @order_number
	--		Set @AppointmentTime = DATEADD(hour,-4,@StartTime)

	--		--寫入2次授權預約
	--		EXEC @iError =  usp_InsOrderAuthReservation_I01 @order_number,@IDNO,@final_price
	--		,1,1,0,1,0,'AuthReservationList','AuthReservationList',@AppointmentTime
	--		,@iErrorCode output,@iErrorMsg output,@iSQLExceptionCode output,@iSQLExceptionMsg output
	--	End

	--	if(@isRetry = 1)
	--	Begin
	--	     --寫入取消訂單
	--		Exec @iError = usp_BookingCancel_U01 @IDNO,@order_number,@Token,0,'授權失敗取消訂單',6,
	--		@iErrorCode output,@iErrorMsg output,@iSQLExceptionCode output,@iSQLExceptionMsg output
	--	End
	--	FETCH NEXT FROM batch_OrderAuth into @authSeq, @isRetry ,@order_number,@IDNO,@final_price
	--END
	--CLOSE batch_OrderAuth
	--DEALLOCATE batch_OrderAuth

	SELECT A.authSeq,
		A.order_number,
		A.final_price,
		A.IDNO,
		A.transaction_no,
		A.CardType,
		A.AuthType,
		AutoClosed  = A.AutoClose,
		A.isRetry,
		A.CardToken,
		A.Mobile
	FROM #TB_OrderAuthList A WHERE AuthFlg = 0

	DROP TABLE #TB_OrderAuthList
	DROP TABLE #TB_OrderAuth
	--DROP TABLE #TB_MemberCardBinding
	Drop Table #TB_OrderMain
	Drop Table #MemberData

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderAuthReservationList_Q01';