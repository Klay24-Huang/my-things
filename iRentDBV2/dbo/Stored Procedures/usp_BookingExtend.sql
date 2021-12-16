/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_BookingExtend
* 系    統 : IRENT
* 程式功能 : 延長用車
* 作    者 : 
* 撰寫日期 : 20200924 ADD BY Eric
* 修改日期 : 
* Example  : 
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_BookingExtend]
	@IDNO                   VARCHAR(10)           ,	--帳號
	@OrderNo                BIGINT                ,	--訂單編號
	@Token                  VARCHAR(1024)         ,	--TOKEN
	@SD                     DATETIME              ,	--預計還車時間
	@ED                     DATETIME              ,	--延長用車時間
	@CarNo                  VARCHAR(10)           ,	--車號
	@LogID                  BIGINT                ,
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
DECLARE @car_mgt_status TINYINT;
DECLARE @cancel_status TINYINT;
DECLARE @booking_status TINYINT;
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;
DECLARE @ProjType INT;
DECLARE @tmpED DATETIME;
DECLARE @tmpCount TINYINT;
DECLARE @NextStartTime DATETIME;	--下筆訂單的起始時間
DECLARE @tmpFineTime DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BookingExtend';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @Descript=N'使用者操作【延長用車】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token=ISNULL (@Token,'');
SET @tmpCount=0;

BEGIN TRY
	IF @Token='' OR @IDNO='' OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	--0.再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END

	--先取得資料
	IF @Error=0
	BEGIN
		SELECT @CarNo=CarNo,
			@tmpED=stop_time,
			@tmpFineTime=fine_Time,
			@car_mgt_status=car_mgt_status,
			@cancel_status=cancel_status,
			@booking_status=booking_status,
			@ProjType=ProjType 
		FROM TB_OrderMain WITH(NOLOCK) 
		WHERE order_number=@OrderNo;
	END

	IF @Error=0
	BEGIN
		--延長時間最少要一小時
		DECLARE @DiffMinute int;
		SET @DiffMinute = DATEDIFF(MINUTE,@SD,@ED);
		
		IF @DiffMinute < 60
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR237';
		END

		IF @Error=0
		BEGIN
			----檢查延長時間是否有卡到其他訂單
			--SELECT @tmpCount=COUNT(IDNO) FROM TB_OrderMain WITH(NOLOCK)
			--WHERE (CarNo=@CarNo AND order_number<>@OrderNo AND (cancel_status=0 and car_mgt_status=0))AND  
			--(
			--	(start_time between @SD AND @ED) 
			--	OR (stop_time between @SD AND @ED)
			--	OR (@SD BETWEEN start_time AND stop_time)
			--	OR (@ED BETWEEN start_time AND stop_time)
			--	OR (DATEADD(MINUTE,-30,@SD) between start_time AND stop_time)
			--	OR (DATEADD(MINUTE,30,@ED) between start_time AND stop_time)
			--);
			--IF @tmpCount>0
			--BEGIN
			--	SET @Error=1;
			--	SET @ErrorCode='ERR181';
			--END

			SELECT top 1 @NextStartTime=ISNULL(start_time,'') FROM TB_OrderMain WITH(NOLOCK) 
			WHERE CarNo=@CarNo AND order_number<>@OrderNo AND (cancel_status=0 and car_mgt_status=0)
			AND start_time >= @SD
			order by start_time

			IF @NextStartTime <> ''
			BEGIN
				DECLARE @tempStartTime DATETIME;
				SET @tempStartTime = DATEADD(MINUTE,-30,@NextStartTime);
				IF @ED > @tempStartTime
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR181';
				END
			END
		END
		
		IF @Error=0
		BEGIN
			--延長用車時間重疊到之後的預約用車時間
			SET @tmpCount=0;
			SELECT @tmpCount=COUNT(IDNO) FROM TB_OrderMain  WITH(NOLOCK)
			WHERE IDNO=@IDNO AND order_number<>@OrderNo 
			AND (car_mgt_status<4 AND cancel_status=0)
			AND (start_time between @SD AND @ED) 
			AND (ProjType <> 4 AND ProjType=@ProjType); 
			IF @tmpCount>0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR182';
			END
		END
	END

	--開始做延長
	IF @Error=0
	BEGIN
		BEGIN TRAN

		INSERT TB_OrderExtendHistory (order_number,StopTime,ExtendStopTime,booking_status)
		SELECT @OrderNo,stop_time,@ED,booking_status
		FROM TB_OrderMain
		WITH(NOLOCK) WHERE order_number=@OrderNo

		IF @NowTime>@tmpED
		BEGIN
			--超過預計還車時間才延長用車，則要記錄逾時時間
			IF @tmpFineTime=''
			BEGIN
				SET @Descript=N'使用者操作【延長用車】逾時';

				UPDATE TB_OrderMain 
				SET stop_time=@ED,
					booking_status=3,
					fine_Time=@tmpED 
				WHERE order_number=@OrderNo 
				AND booking_status<4 
				AND (car_mgt_status>=4 AND car_mgt_status<15) 
				AND cancel_status<3
					
				INSERT INTO TB_OrderHistory(OrderNum,booking_status,car_mgt_status,cancel_status,Descript)
				SELECT @OrderNo,booking_status,car_mgt_status,cancel_status,@Descript FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo;
			END
			ELSE
			BEGIN
				UPDATE TB_OrderMain 
				SET stop_time=@ED,
					booking_status=3
				WHERE order_number=@OrderNo 
				AND booking_status<4 
				AND (car_mgt_status>=4 AND car_mgt_status<15) 
				AND cancel_status<3

				INSERT INTO TB_OrderHistory(OrderNum,booking_status,car_mgt_status,cancel_status,Descript)
				SELECT @OrderNo,booking_status,car_mgt_status,cancel_status,@Descript FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo;
			END
		END
		ELSE
		BEGIN
			UPDATE TB_OrderMain 
			SET stop_time=@ED,
				booking_status=3 
			WHERE order_number=@OrderNo 
			AND booking_status<4 
			AND (car_mgt_status>=4 AND car_mgt_status<15)
			AND cancel_status<3

			INSERT INTO TB_OrderHistory(OrderNum,booking_status,car_mgt_status,cancel_status,Descript)
			SELECT @OrderNo,booking_status,car_mgt_status,cancel_status,@Descript FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo;
		END
		COMMIT TRAN;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingExtend';



