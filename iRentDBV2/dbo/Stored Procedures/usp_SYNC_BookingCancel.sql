/****************************************************************
** Name: [dbo].[usp_SYNC_BookingCancel]
** Desc: 
**
** Return values: 0 成功 else 錯誤
** Return Recordset: 
**
** Called by: 
**
** Parameters:
** Input
** -----------

** 
**
** Output
** -----------
		
	@ErrorCode 				VARCHAR(6)			
	@ErrorCodeDesc			NVARCHAR(100)	
	@SQLExceptionCode		VARCHAR(10)				
	@SqlExceptionMsg		NVARCHAR(1000)	
**
** 
** Example
**------------
** DECLARE @Error               INT;
** DECLARE @ErrorCode 			VARCHAR(6);		
** DECLARE @ErrorMsg  			NVARCHAR(100);
** DECLARE @SQLExceptionCode	VARCHAR(10);		
** DECLARE @SQLExceptionMsg		NVARCHAR(1000);
** EXEC @Error=[dbo].[usp_SYNC_BookingCancel]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/23 下午 05:52:28 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/23 下午 05:52:28    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_SYNC_BookingCancel]
	@NowTime                DATETIME              ,
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
DECLARE @Descript NVARCHAR(200);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_SYNC_BookingCancel';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
	--訂單資訊共用表
	-- 20210528;UPD BY YEH REASON.增加系統取消積分扣分欄位
	DECLARE @sync_order_data TABLE (
		OrderNum int,
		IDNO VARCHAR(10),
		citizen_id varchar(20),
		assigned_car_id varchar(20),
		Title varchar(50),
		StartTime datetime,
		StopTime datetime,
		FineTime datetime,
		FinalStartTime datetime,
		FinalStopTime datetime,
		CardNo varchar(50),
		car_mgt_status int,
		booking_status int,
		cancel_status int,
		MachineNo varchar(10),
		DeviceToken varchar(300),
		RID int,
		CarNo VARCHAR(20),
		ProjType INT,
		UseTimeDiff	INT
	);

	/*過15分鐘未取車，系統自動取消*/
	--寫入暫存表
	-- 20210528;UPD BY YEH REASON.增加系統取消積分扣分用欄位
	INSERT INTO @sync_order_data (OrderNum,citizen_id,Title,DeviceToken,cancel_status,car_mgt_status,booking_status,CarNo,ProjType
									,IDNO,StartTime,StopTime,UseTimeDiff)
	SELECT DISTINCT OrderNum,IDNO,ISNULL(Title,''),ISNULL(DeviceToken,''),cancel_status,car_mgt_status,booking_status,CarNo,PROJTYPE	--20201218 ADD BY ADAM 
		,IDNO,StartTime,StopTime,DATEDIFF(HOUR,StartTime,StopTime)
	FROM VW_SYNC_GetSyncData WITH(NOLOCK)
	WHERE car_mgt_status=0 AND booking_status=0  AND cancel_status=0 
	AND stop_pick_time<=dbo.GET_TWDATE()
	AND stop_pick_time>StartTime;

	--寫入訂單紀錄
	INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
	SELECT OrderNum,cancel_status,car_mgt_status,booking_status,N'排程【逾時未取車】'
	FROM @sync_order_data;

	--會員的資料要處理狀態
	UPDATE TB_CarInfo 
	SET RentCount=RentCount-1,
		UPDTime=@NowTime
	FROM TB_CarInfo C
	JOIN @sync_order_data D ON C.CarNo=D.CardNo
	WHERE C.RentCount>0;

	--更新總表
	UPDATE TB_BookingStatusOfUser
	SET NormalRentBookingNowCount	= CASE WHEN D.ProjType=0 AND NormalRentBookingNowCount>0 THEN NormalRentBookingNowCount-1 ELSE NormalRentBookingNowCount END,
		NormalRentBookingCancelCount= CASE WHEN D.ProjType=0 THEN NormalRentBookingCancelCount+1 ELSE NormalRentBookingCancelCount END,
		AnyRentBookingNowCount = CASE WHEN D.ProjType=3 AND AnyRentBookingNowCount>0 THEN 0 ELSE AnyRentBookingNowCount END,
		AnyRentBookingCancelCount = CASE WHEN D.ProjType=3 THEN AnyRentBookingCancelCount+1 ELSE AnyRentBookingCancelCount END,
		MotorRentBookingNowCount	= CASE WHEN D.ProjType=4 AND MotorRentBookingNowCount>0 THEN 0 ELSE MotorRentBookingNowCount END,
		MotorRentBookingCancelCount = CASE WHEN D.ProjType=4 THEN MotorRentBookingCancelCount+1 ELSE MotorRentBookingCancelCount END,
		UPDTime=@NowTime
	FROM TB_BookingStatusOfUser A
	JOIN @sync_order_data D ON A.IDNO=D.citizen_id;

	--寫入推播通知 ，推播先關掉等機制完成後再上
	--INSERT INTO TB_PersonNotification(OrderNum,IDNO,NType,UserName,UserToken,STime,Message)
	--SELECT OrderNum,citizen_id,3,Title,DeviceToken,@NowTime,N'請注意！您已超過預約取車時間，系統已自動取消訂單。提醒您，若臨時變更行程請於APP取消預約，以免影響您的租車權益。'
	--FROM @sync_order_data;

	--更新給短租預約單
	UPDATE TB_BookingControl 
	set PROCD='F',
		isRetry=0,
		UPDTime=@NowTime
	where order_number in (SELECT order_number FROM TB_BookingControl WITH(NOLOCK) WHERE order_number IN (SELECT OrderNum FROM @sync_order_data) AND PROCD = 'A' AND isRetry = 1);

	update TB_BookingControl 
	SET PROCD='F',
		isRetry=1,
		UPDTime=@NowTime
	WHERE order_number in (SELECT order_number FROM TB_BookingControl WITH(NOLOCK) WHERE order_number IN (select OrderNum from @sync_order_data) AND PROCD = 'A' AND isRetry = 0);

	--更新主訂單狀態
	UPDATE TB_OrderMain 
	set cancel_status=3 
	where order_number in (SELECT OrderNum FROM @sync_order_data);

	DROP TABLE IF EXISTS #CarList;

	-- 20210707 ADD BY YEH REASON:取出還在用車中的車輛清單
	SELECT order_number,IDNO,CarNo
	INTO #CarList 
	FROM TB_OrderMain WITH(NOLOCK) 
	WHERE (car_mgt_status>=4 AND car_mgt_status<16) AND cancel_status=0 AND ProjType=0 
	AND start_time>='2021/1/1 00:00:00';

	-- 20210707 ADD BY YEH REASON.同站 預約用車時間>=10小時積分扣-25分
	INSERT INTO TB_MemberScoreDetail(A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,
										MEMIDNO,ORDERNO,DEF_SEQ,SCORE,ORI_SCORE,UIDISABLE,ISPROCESSED)
	SELECT @FunName,IDNO,@NowTime,@FunName,IDNO,@NowTime,
		IDNO,OrderNum,5,-25,-25,0,0
	FROM @sync_order_data
	WHERE ProjType=0
	AND UseTimeDiff >= 10
	AND CarNo NOT IN (SELECT CarNo FROM #CarList)	-- 20210602 ADD BY YEH REASON:正在用車被系統取消不扣分
	-- 20210726 UPD BY YEH REASON:增加判斷不存在TB_MemberScoreDetail才寫入
	AND OrderNum NOT IN (SELECT ORDERNO FROM TB_MemberScoreDetail WITH(NOLOCK) WHERE DEF_SEQ=5 AND ORDERNO NOT IN (SELECT OrderNum FROM @sync_order_data) );

	-- 20210707 ADD BY YEH REASON.同站 預約用車時間<10小時積分扣-20分
	INSERT INTO TB_MemberScoreDetail(A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,
										MEMIDNO,ORDERNO,DEF_SEQ,SCORE,ORI_SCORE,UIDISABLE,ISPROCESSED)
	SELECT @FunName,IDNO,@NowTime,@FunName,IDNO,@NowTime,
		IDNO,OrderNum,4,-20,-20,0,0
	FROM @sync_order_data
	WHERE ProjType=0
	AND UseTimeDiff < 10
	AND CarNo NOT IN (SELECT CarNo FROM #CarList)	-- 20210602 ADD BY YEH REASON:正在用車被系統取消不扣分
	-- 20210726 UPD BY YEH REASON:增加判斷不存在TB_MemberScoreDetail才寫入
	AND OrderNum NOT IN (SELECT ORDERNO FROM TB_MemberScoreDetail WITH(NOLOCK) WHERE DEF_SEQ=4 AND ORDERNO NOT IN (SELECT OrderNum FROM @sync_order_data) );	

	-- 20210726 UPD BY YEH REASON:前位使用者逾時用車導致訂單被系統取消時，要扣前位使用者(影響後續用戶租用)的分數
	INSERT INTO TB_MemberScoreDetail(A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,
									 MEMIDNO,ORDERNO,DEF_SEQ,SCORE,ORI_SCORE,UIDISABLE,ISPROCESSED)
	SELECT @FunName,IDNO,@NowTime,@FunName,IDNO,@NowTime,
		IDNO,order_number,14,-20,-20,0,-1
	FROM #CarList
	WHERE CarNo IN (SELECT CarNo FROM @sync_order_data)
	AND order_number NOT IN (SELECT ORDERNO FROM TB_MemberScoreDetail WITH(NOLOCK) WHERE DEF_SEQ=14 AND ORDERNO NOT IN (SELECT OrderNum FROM @sync_order_data) );

	DROP TABLE IF EXISTS #CarList;

	--清除共用表
	delete from @sync_order_data

	/*還車前30分鐘通知*/
	--宣告固定時間參數
	declare @hourSub2return datetime;
	set @hourSub2return = DATEADD(HOUR,-2, @NowTime);

	--寫入暫存表
	insert into @sync_order_data (OrderNum,citizen_id,Title,DeviceToken,StopTime)
	SELECT distinct OrderNum,IDNO,ISNULL(Title,''),ISNULL(DeviceToken,''),StopTime
	FROM VW_SYNC_GetSyncData WITH(NOLOCK)
	WHERE (car_mgt_status>=4 AND car_mgt_status<15) 
	AND booking_status<5 
	AND cancel_status=0 
	AND @NowTime>= DATEADD(MINUTE,-30,StopTime) 
	AND StopTime>@NowTime AND OrderNum NOT IN ( SELECT OrderNum FROM TB_PersonNotification WHERE NType in (2,4) );

	--寫入推播通知，推播先關掉等機制完成後再上
	--INSERT INTO TB_PersonNotification(OrderNum,IDNO,NType,UserName,UserToken,STime,Message)
	--select distinct OrderNum,citizen_id,2,Title,DeviceToken,@NowTime,N'您好，您目前租用的車輛需於30分鐘內還車，若需延後還車，請於APP申請延長用車。(注意！逾時還車租金將以定價收費，無法享有優惠價)'
	--from @sync_order_data;

	--清除共用表
	delete from @sync_order_data
	/*還車前30分鐘通知END*/

	/*逾時通知*/
	--宣告固定時間參數
	declare @hourSub2delay datetime;
	set @hourSub2delay = DATEADD(HOUR,-2, @NowTime);

	--寫入暫存表
	INSERT INTO @sync_order_data (OrderNum,citizen_id,Title,DeviceToken,cancel_status,car_mgt_status,booking_status,
									IDNO,CarNo,StopTime,ProjType,FinalStartTime)
	SELECT distinct OrderNum,IDNO,ISNULL(Title,''),ISNULL(DeviceToken,''),cancel_status,car_mgt_status,booking_status,
		IDNO,CarNo,StopTime,PROJTYPE,FinalStartTime
	FROM VW_SYNC_GetSyncData WITH(NOLOCK)
	WHERE (car_mgt_status>=4 AND car_mgt_status<16) AND booking_status<5 AND cancel_status=0 
	AND StopTime<=@NowTime AND FineTime = '1911-01-01 00:00:00.000'
	AND OrderNum NOT IN (SELECT OrderNum FROM TB_PersonNotification WHERE NType=4);

	--寫入訂單歷程記錄
	INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
	select OrderNum,cancel_status,car_mgt_status,booking_status,N'排程【寫入逾時時間】'
	from @sync_order_data

	--寫入推播通知，推播先關掉等機制完成後再上
	--INSERT INTO TB_PersonNotification(OrderNum,IDNO,NType,UserName,UserToken,STime,Message)
	--select distinct OrderNum,citizen_id,4,Title,DeviceToken,@NowTime,N'請注意！您目前租用的車輛已超過預計還車時間，逾時租金將無法享有優惠價(以車款定價計算)。請立即還車以免影響您的租車權益及造成下一位使用者不便。'
	--from @sync_order_data;

	--更新訂單主表
	declare @delayCount int;
	set @delayCount = (select count(*) from @sync_order_data);

	if @delayCount > 0
	begin
		UPDATE TB_OrderMain 
		set fine_Time = stop_time 
		where order_number in ( select OrderNum from @sync_order_data );
	end

	-- 20210707 ADD BY YEH REASON:逾時且40分鐘後有訂單，先預扣分數
	INSERT INTO TB_MemberScoreDetail(A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,
										MEMIDNO,ORDERNO,DEF_SEQ,SCORE,ORI_SCORE,UIDISABLE,ISPROCESSED)
	SELECT @FunName,B.IDNO,@NowTime,@FunName,B.IDNO,@NowTime,
		B.IDNO,B.OrderNum,14,-20,-20,0,-1
	FROM TB_OrderMain A WITH(NOLOCK)
	INNER JOIN @sync_order_data B ON A.CarNo=B.CarNo AND B.ProjType=0
	WHERE A.ProjType=0
	AND A.car_mgt_status=0 AND A.cancel_status=0
	AND DATEDIFF(MINUTE, B.StopTime, A.start_time) <= 40
	AND B.FinalStartTime >= '2021/7/7 12:00:00'
	-- 20210726 UPD BY YEH REASON:增加判斷不存在TB_MemberScoreDetail才寫入
	AND B.OrderNum NOT IN (SELECT ORDERNO FROM TB_MemberScoreDetail WITH(NOLOCK) WHERE DEF_SEQ=14 AND ORDERNO IN (SELECT ORDERNO FROM @sync_order_data) );

	--清除共用表
	delete from @sync_order_data
	/*逾時通知END*/

	/*結帳後15分鐘未還車*/
	--宣告固定時間參數
	declare @hourSub2pay datetime;
	set @hourSub2pay = DATEADD(HOUR,-2, @NowTime);

	--寫入暫存表
	insert into @sync_order_data (OrderNum,citizen_id,Title,DeviceToken,MachineNo,assigned_car_id)
	SELECT distinct OrderNum,IDNO,ISNULL(Title,''),ISNULL(DeviceToken,''),CID,CarNo
	FROM VW_SYNC_GetSyncData WITH(NOLOCK)
	INNER JOIN TB_Trade WITH(NOLOCK) ON OrderNo=VW_SYNC_GetSyncData.OrderNum AND CreditType=0 AND IsSuccess=1 AND DATEADD(Minute,15,UPDTime) between @hourSub2pay and @NowTime 
	WHERE (car_mgt_status>=4 AND car_mgt_status<16) 
	AND booking_status<5 
	AND cancel_status=0 
	AND VW_SYNC_GetSyncData.OrderNum NOT IN (SELECT OrderNum FROM TB_PersonNotification WHERE NType=5);

	--寫入推播通知
	--INSERT INTO TB_PersonNotification(OrderNum,IDNO,NType,UserName,UserToken,STime,Message)
	--select distinct OrderNum,citizen_id,5,Title,DeviceToken,@NowTime,N'請注意！您尚未完成還車程序！請立即開啟APP點選「鎖車」按鈕。若有任何問題請聯繫24H客服人員0800-024550'
	--from @sync_order_data;

	--20210205 ADD BY ADAM REASON.補上清潔訂單自動取消
	DECLARE @sync_clearOrder_data TABLE (
		OrderNum int,
		CarNo varchar(50),
		car_mgt_status int,
		booking_status int,
		cancel_status int,
		spec_status int,
		StartTime datetime,
		StopTime datetime
	);

	DECLARE @sync_closeCleanOrder TABLE(OrderNum int);
	
	--取出全部未完成及未取消的清潔保修
	INSERT INTO @sync_clearOrder_data(OrderNum,CarNo,car_mgt_status,booking_status,cancel_status,spec_status,StartTime ,StopTime)
	SELECT order_number ,CarNo ,car_mgt_status ,booking_status ,cancel_status ,spec_status,start_time,stop_time
	FROM dbo.TB_OrderMain WITH(NOLOCK) WHERE booking_status>0 AND booking_status<3 AND cancel_status=0 AND spec_status>0 
	

	--執行整備人員逾期未取車
	INSERT INTO @sync_closeCleanOrder (OrderNum)
	SELECT order_number FROM TB_OrderMain WHERE order_number IN (
		SELECT OrderNum FROM @sync_clearOrder_data WHERE spec_status=4 AND DATEADD(minute,15,StartTime)<@NowTime  
		AND OrderNum IN (SELECT OrderNum FROM TB_CarCleanLog WHERE OrderStatus=0)
	)

	--更新訂單主檔
	UPDATE TB_OrderMain 
	SET cancel_status=5 
	WHERE order_number IN (
		SELECT OrderNum FROM @sync_clearOrder_data WHERE spec_status=4 AND DATEADD(minute,15,StartTime)<@NowTime 
		AND OrderNum IN (SELECT OrderNum FROM TB_CarCleanLog WHERE OrderStatus=0)
	)

	--更新整備人員記錄檔
	UPDATE TB_CarCleanLog 
	SET OrderStatus=4,
		UPTime=@NowTime
	WHERE OrderNum IN (
		SELECT OrderNum FROM @sync_closeCleanOrder
	)

	--清除共用表
	DELETE FROM @sync_closeCleanOrder
  
	--執行後台保清清潔若是已超過時間則自動釋出
	UPDATE TB_OrderMain 
	SET cancel_status=5 
	WHERE order_number IN (
		SELECT OrderNum FROM @sync_clearOrder_data WHERE spec_status <4 AND StopTime<@NowTime
	)

	--更新整備人員記錄檔
	UPDATE TB_CarCleanLog 
	SET OrderStatus=5,
		bookingEnd=@NowTime,
		UPTime=@NowTime
	WHERE OrderNum IN (
		SELECT OrderNum FROM @sync_clearOrder_data WHERE spec_status <4 AND StopTime<@NowTime
	)

	-- 20210317 ADD;將已取車逾時未還車的狀態更新
	--執行更新"已取車逾時未還車"狀態更新
	UPDATE TB_OrderMain 
	SET car_mgt_status=16 
	WHERE order_number IN (
		SELECT OrderNum FROM @sync_clearOrder_data WHERE car_mgt_status=0 AND StopTime < DATEADD(minute,-15,@NowTime)
		AND OrderNum IN (SELECT OrderNum FROM TB_CarCleanLog WHERE OrderStatus=1)
	)

	--更新整備人員記錄檔
	UPDATE TB_CarCleanLog 
	SET OrderStatus=5,
		UPTime=@NowTime
	WHERE OrderNum IN (
		SELECT OrderNum FROM @sync_clearOrder_data WHERE car_mgt_status=0 AND StopTime < DATEADD(minute,-15,@NowTime)
		AND OrderNum IN (SELECT OrderNum FROM TB_CarCleanLog WHERE OrderStatus=1)
	)

	--清除共用表
	DELETE FROM @sync_clearOrder_data


	--寫入錯誤訊息
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SYNC_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SYNC_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'排程取消', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SYNC_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SYNC_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SYNC_BookingCancel';