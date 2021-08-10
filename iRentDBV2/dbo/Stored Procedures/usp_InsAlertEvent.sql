/****************************************************************
** Change History
*****************************************************************
** 用途：寫入告警事件
** 10:車機失聯1小時
** 11:超過15分鐘未完成還車作業
** 12:超過預約還車時間30分鐘未還車
** 13:取車1小時前沒有車
*****************************************************************
** 異動記錄：
** 2021/04/22 ADD BY YEH First Release，增加10/11/12
** 2021/07/08 UPD BY YEH REASON:增加13
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsAlertEvent]
	@LogID                  BIGINT					,
	@ErrorCode 				VARCHAR(6)		OUTPUT	,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT	,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT	,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT		--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;
DECLARE @OneHTime DATETIME;			-- 系統時間-1小時
DECLARE @EventType INT;				-- 事件類別
DECLARE @AddOneHourTime DATETIME;	-- 系統時間+1小時

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_InsAlertEvent';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @OneHTime=DATEADD(HOUR,-1,@NowTime);
SET @EventType=0;
SET @AddOneHourTime=DATEADD(HOUR,+1,@NowTime);

BEGIN TRY
	IF @Error=0
	BEGIN
		DROP TABLE IF EXISTS #CarList;
		DROP TABLE IF EXISTS #NoPayList;
		DROP TABLE IF EXISTS #NoReturnList;
		DROP TABLE IF EXISTS #NoCarList;

		--***************************************************************************************************************
		-- 10:車機失聯1小時
		SET @EventType=10;

		CREATE TABLE #CarList (
			CID		VARCHAR(10),
			CarNo	VARCHAR(10),
			Mail	VARCHAR(100),
			UPDTime DATETIME
		);
		
		-- 排除條件：1.下線車輛 2.據點(XXXX/X0XX)
		INSERT INTO #CarList
		SELECT A.CID,A.CarNo,CONCAT(ISNULL(C.AlertEmail,C.[ManageStationID]),'@hotaimotor.com.tw;'),A.UPDTime
		FROM TB_CarStatus A WITH(NOLOCK)
		INNER JOIN TB_Car B WITH(NOLOCK) ON B.CarNo=A.CarNo AND B.available<>2
		LEFT JOIN TB_iRentStation C WITH(NOLOCK) ON C.StationID=B.nowStationID
		WHERE A.UPDTime<=@OneHTime
		AND B.nowStationID NOT IN ('XXXX','X0XX');

		INSERT INTO TB_EventHandle(EventType,MachineNo,CarNo,MKTime,UPDTime)
		SELECT @EventType,CID,CarNo,UPDTime,@NowTime
		FROM #CarList;

		INSERT INTO TB_AlertMailLog([EventType],[Receiver],[Sender],[HasSend],CarNo,[MKTime],UPDTime)
		SELECT @EventType,Mail,'0',0,CarNo,UPDTime,@NowTime
		FROM #CarList;
		--***************************************************************************************************************
		
		--***************************************************************************************************************
		-- 11:超過15分鐘未完成還車作業
		SET @EventType=11;

		CREATE TABLE #NoPayList (
			OrderNo	BIGINT,
			CarNo	VARCHAR(10),
			CID		VARCHAR(10),
			Mail	VARCHAR(100)
		);

		-- 條件：car_mgt_status=11 AND 還車時間超過15分鐘
		INSERT INTO #NoPayList
		SELECT A.order_number,A.CarNo,C.CID,CONCAT(ISNULL(E.AlertEmail,E.[ManageStationID]),'@hotaimotor.com.tw;')
		FROM TB_OrderMain A WITH(NOLOCK)
		INNER JOIN TB_OrderDetail B WITH(NOLOCK) on B.order_number=A.order_number
		INNER JOIN TB_CarInfo C WITH(NOLOCK) on A.CarNo=C.CarNo
		INNER JOIN TB_Car D WITH(NOLOCK) on A.CarNo=D.CarNo
		INNER JOIN TB_iRentStation E WITH(NOLOCK) on E.StationID=D.nowStationID
		WHERE A.car_mgt_status=11 AND A.start_time>='2021/1/1'
		AND DATEDIFF(MINUTE,B.final_stop_time,@NowTime)>15;

		INSERT INTO TB_EventHandle(EventType,NowOrder,MachineNo,CarNo,MKTime,UPDTime)
		SELECT @EventType,OrderNo,CID,CarNo,@NowTime,@NowTime
		FROM #NoPayList;

		INSERT INTO TB_AlertMailLog([EventType],[Receiver],[Sender],[HasSend],CarNo,OrderNo,[MKTime],UPDTime)
		SELECT @EventType,Mail,'0',0,CarNo,OrderNo,@NowTime,@NowTime
		FROM #NoPayList;
		--***************************************************************************************************************
		
		--***************************************************************************************************************
		-- 12:超過預約還車時間30分鐘未還車
		SET @EventType=12;

		CREATE TABLE #NoReturnList (
			OrderNo	BIGINT,
			CarNo	VARCHAR(10),
			CID		VARCHAR(10),
			Mail	VARCHAR(100)
		);

		-- 條件：用車中 && 預計還車時間超過30分鐘
		INSERT INTO #NoReturnList
		SELECT A.order_number,A.CarNo,C.CID,CONCAT(ISNULL(E.AlertEmail,E.[ManageStationID]),'@hotaimotor.com.tw;')
		FROM TB_OrderMain A WITH(NOLOCK)
		INNER JOIN TB_OrderDetail B WITH(NOLOCK) on B.order_number=A.order_number
		INNER JOIN TB_CarInfo C WITH(NOLOCK) on A.CarNo=C.CarNo
		INNER JOIN TB_Car D WITH(NOLOCK) on A.CarNo=D.CarNo
		INNER JOIN TB_iRentStation E WITH(NOLOCK) on E.StationID=D.nowStationID
		WHERE (A.car_mgt_status>=4 AND A.car_mgt_status<11) AND A.cancel_status=0
		AND A.start_time>='2021/1/1'
		AND DATEDIFF(minute,A.stop_time,DATEADD(MINUTE,30,@NowTime))>30;

		INSERT INTO TB_EventHandle(EventType,NowOrder,MachineNo,CarNo,MKTime,UPDTime)
		SELECT @EventType,OrderNo,CID,CarNo,@NowTime,@NowTime
		FROM #NoReturnList;

		INSERT INTO TB_AlertMailLog([EventType],[Receiver],[Sender],[HasSend],CarNo,OrderNo,[MKTime],UPDTime)
		SELECT @EventType,Mail,'0',0,CarNo,OrderNo,@NowTime,@NowTime
		FROM #NoReturnList;
		--***************************************************************************************************************

		--***************************************************************************************************************
		-- 13:取車1小時前沒有車
		SET @EventType=13;

		CREATE TABLE #NoCarList (
			OrderNo	BIGINT,
			CarNo	VARCHAR(10),
			CID		VARCHAR(10),
			Mail	VARCHAR(100)
		);

		-- 條件：目前尚在用車，且系統時間+1小時有訂單
		INSERT INTO #NoCarList
		Select C.order_number,A.CarNo,D.CID,CONCAT(ISNULL(E.AlertEmail,E.[ManageStationID]),'@hotaimotor.com.tw;')
		From TB_Car A WITH(NOLOCK)
		INNER JOIN TB_OrderMain B WITH(NOLOCK) ON B.CarNo=A.CarNo AND (B.car_mgt_status>=4 AND B.car_mgt_status<16) AND B.cancel_status=0 AND B.start_time>='2021/1/1'
		LEFT JOIN (Select * FROM TB_OrderMain WITH(NOLOCK) 
					Where car_mgt_status=0 AND booking_status<>1 AND cancel_status=0 
					AND start_time BETWEEN @NowTime AND @AddOneHourTime ) C ON C.CarNo=A.CarNo
		INNER JOIN TB_CarInfo D WITH(NOLOCK) ON D.CarNo=A.CarNo
		INNER JOIN TB_iRentStation E WITH(NOLOCK) on E.StationID=A.nowStationID
		WHERE A.available < 2
		AND C.order_number IS NOT NULL;

		INSERT INTO TB_EventHandle(EventType,NowOrder,MachineNo,CarNo,MKTime,UPDTime)
		SELECT @EventType,OrderNo,CID,CarNo,@NowTime,@NowTime
		FROM #NoCarList;

		INSERT INTO TB_AlertMailLog([EventType],[Receiver],[Sender],[HasSend],CarNo,OrderNo,[MKTime],UPDTime)
		SELECT @EventType,Mail,'0',0,CarNo,OrderNo,@NowTime,@NowTime
		FROM #NoCarList;
		--***************************************************************************************************************

		DROP TABLE IF EXISTS #CarList;
		DROP TABLE IF EXISTS #NoPayList;
		DROP TABLE IF EXISTS #NoReturnList;
		DROP TABLE IF EXISTS #NoCarList;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsAlertEvent';
GO

