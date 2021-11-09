
/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_PreCreditAuthList_Q01
* 系    統 : IRENT
* 程式功能 : 取出預約及逾期訂單
* 作    者 : Umeko
* 撰寫日期 : 20211101
* 修改日期 : 

* Example  : 
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetPreCreditAuthList_Q01]
	@NHour int,
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

SET @FunName='usp_GetPreCreditAuthList_Q01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=DATEADD(HOUR,8,GETDATE());


BEGIN TRY

Declare @PreOrderAuth as table ( order_number bigint,IDNO varchar(10),booking_date datetime,start_time datetime,stop_time datetime,Seqno bigint, pre_final_Price int,AuthType int,status int)

--預約訂單 6小時前
Insert into @PreOrderAuth (order_number,IDNO,booking_date,start_time,stop_time,Seqno,status,pre_final_Price,AuthType)
Select main.order_number,main.IDNO,booking_date,start_time,stop_time,Amount.Seqno,Amount.status,Amount.final_price  pre_final_price
--,start_time a_start_time, booking_date a_booking_date,stop_time a_stop_time
--,Convert(varchar(10),start_time,111) getCarDate,DateAdd(day,-2,DateAdd(hour,20,Convert(varchar(10),start_time,111))) BookingNDaysAgo
--,DateAdd(hour,-6,start_time) NowBookingbefore 
--,DateAdd(hour,6,dbo.GET_TWDATE()) 
--,DateAdd(hour,-2,dbo.GET_TWDATE())  TimeOver
--,datediff(hour,stop_time,dbo.GET_TWDATE()) OverTimes
, 1 AuthType
From TB_OrderMain main with(nolock)
Join TB_OrderAuthAmount Amount with(nolock) On main.order_number = Amount.order_number
Where main.booking_status = 0 And car_mgt_status = 0 And cancel_status = 0 And  Amount.Status = 0
And start_time > DateAdd(hour,@NHour,dbo.GET_TWDATE()) 

--Select * From @PreOrderAuth


update TB_OrderAuthAmount 
Set Status = 1
--Select *
From TB_OrderAuthAmount A 
Where Exists(Select 1 From @PreOrderAuth Where Seqno = A.Seqno) And Status = 0

--逾時訂單
Insert into @PreOrderAuth (order_number,IDNO,booking_date,start_time,stop_time,Seqno,status,pre_final_Price,AuthType)
Select main.order_number,main.IDNO,booking_date,start_time,stop_time,0 Seqno,0 status,0  pre_final_price
--,start_time a_start_time, booking_date a_booking_date,stop_time a_stop_time
--,Convert(varchar(10),start_time,111) getCarDate,DateAdd(day,-2,DateAdd(hour,20,Convert(varchar(10),start_time,111))) BookingNDaysAgo
--,DateAdd(hour,-6,start_time) NowBookingbefore 
--,DateAdd(hour,-2,dbo.GET_TWDATE())  TimeOver
--,datediff(hour,stop_time,dbo.GET_TWDATE()) OverTimes
, 5 AuthType
From TB_OrderMain main 
Where car_mgt_status >=4 And car_mgt_status <16 And booking_status < 5 And cancel_status = 0 And ProjType in (0,3)
--And Exists(Select 1 From TB_OrderAuthAmount Amount with(nolock) Where Amount.order_number = main.order_number And Amount.AuthType = 1)
And Not Exists(Select 1 From TB_OrderAuth OrderAuth  with(nolock) Where order_number = main.order_number  And OrderAuth.AuthType = 5)
And start_time >= '2021/01/01'
And stop_time < DateAdd(hour,-2,dbo.GET_TWDATE()) 

Select * From @PreOrderAuth

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

END
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetPreCreditAuthList_Q01';