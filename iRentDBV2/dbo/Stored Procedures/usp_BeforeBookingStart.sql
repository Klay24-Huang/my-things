/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_BeforeBookingStart
* 系    統 : IRENT
* 程式功能 : 取車前檢查
* 作    者 : 
* 撰寫日期 : 
* 修改日期 : 20210915 UPD BY YEH REASON:增加會員積分黑名單不可租車檢查 (邏輯先寫好MARK，待上線再打開)
			 20210929 UPD BY YEH REASON:打開檢查
			 20211216 UPD BY AMBER REASON:增加檢核點
* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_BeforeBookingStart]
	@IDNO                   VARCHAR(10)           ,	--帳號
	@OrderNo				BIGINT                ,	--訂單編號
	@Token                  VARCHAR(1024)         ,
	@LogID                  BIGINT                ,
	@CID                    VARCHAR(10)     OUTPUT, --車機編號
	@IsCens                 INT             OUTPUT, --是否為興聯車機
	@IsMotor                INT             OUTPUT, --是否為機車車機
	@deviceToken            VARCHAR(256)    OUTPUT, --遠傳車機token
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
DECLARE @NowTime DATETIME;
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;
DECLARE @car_mgt_status TINYINT;
DECLARE @cancel_status TINYINT;
DECLARE @booking_status TINYINT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BeforeBookingStart';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token=ISNULL (@Token,'');
SET @CID='';
SET @IsCens='';
SET @deviceToken='';
SET @Descript=N'使用者操作【取車前判斷訂單狀態】';
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;

BEGIN TRY
	IF @Token='' OR @IDNO=''  OR @OrderNo=0
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

	--跨年交通管制，X0IU、X0IF、X0LL、X1Q9這四站12/31 12:00-1/1 05:00，這區間內限制無法取車
	--IF @Error=0
	--BEGIN
	--	DECLARE @StationID VARCHAR(6);
	--	DECLARE @SDATE DATETIME;
	--	DECLARE @EDATE DATETIME;

	--	SELECT @StationID=lend_place,@SDATE=start_time,@EDATE=stop_time FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo;

	--	IF (@SDATE>=CAST('2020-12-31 12:00:00' AS DATETIME) AND @SDATE<=CAST('2021-01-01 05:00:00' AS DATETIME)) OR
	--		(@EDATE>=CAST('2020-12-31 12:00:00' AS DATETIME) AND @EDATE<=CAST('2021-01-01 05:00:00' AS DATETIME))
	--	BEGIN
	--		IF @StationID='X0IU' OR @StationID='X0IF' OR @StationID='X0LL' OR @StationID='X1Q9'
	--		BEGIN
	--			SET @Error=1
	--			SET @ErrorCode='ERR171'
	--		END
	--	END
	--END

	-- 20210915 UPD BY YEH REASON:增加會員積分黑名單不可租車檢查
	-- 20210929 UPD BY YEH REASON:打開檢查
	IF @Error = 0
	BEGIN
		IF EXISTS(SELECT * FROM TB_MemberScoreMain WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND ISBLOCK=1)
		BEGIN
			SET @Error = 1;
			SET @ErrorCode = 'ERR287';
		END
	END
		 
	IF @Error=0
	BEGIN
		BEGIN TRAN
		SET @hasData=0
		SELECT @hasData=COUNT(order_number)  FROM TB_OrderMain WITH(NOLOCK) 
		WHERE IDNO=@IDNO AND order_number=@OrderNo AND (car_mgt_status<=3 AND cancel_status=0 AND booking_status<3) AND stop_pick_time>@NowTime;
		IF @hasData>0
		BEGIN
			--寫入記錄
			SELECT @booking_status=booking_status,
				@cancel_status=cancel_status,
				@car_mgt_status=car_mgt_status,
				@CarNo=CarNo,
				@ProjType=ProjType
			FROM TB_OrderMain WITH(NOLOCK)
			WHERE order_number=@OrderNo;
					
			INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
			VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);

			COMMIT TRAN;

			-- 回傳資料
			SELECT @deviceToken=ISNULL(deviceToken,''),
				@CID=CID,
				@IsCens=IsCens,
				@IsMotor=IsMotor 
			FROM TB_CarInfo WITH(NOLOCK) WHERE CarNo=@CarNo; 
		END
		ELSE
		BEGIN
			ROLLBACK TRAN;
			SET @Error=1;
			SET @ErrorCode='ERR171';
		END
	END

	-- 20211216 ADD BY AMBER REASON:檢核會員狀態
	IF @Error=0
	BEGIN
		--審核不通過不可取車
		IF EXISTS(SELECT Audit FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND (Audit=2 OR HasCheckMobile=0))
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR239';
		END
	END

	-- 20211216 ADD BY AMBER REASON:檢查是否有前車未還 
	IF @Error=0
	BEGIN
		DECLARE @BeforeDate DATETIME;
		SET @BeforeDate = DATEADD(Month,-1,@NowTime);
		SET @hasData=0;
		SELECT @hasData=COUNT(1) FROM TB_OrderMain WITH(NOLOCK) WHERE CarNo=@CarNo AND start_time>@BeforeDate
		AND (car_mgt_status>=4 and car_mgt_status<16) AND cancel_status=0;
		IF @hasData>0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR240';
		END
	END

	-- 20211216 ADD BY AMBER REASON:超過取車時間或此訂單已失效 
	IF @Error=0 
	BEGIN
	  SET @hasData=0;
	  SELECT @hasData=COUNT(order_number) FROM TB_OrderMain WITH(NOLOCK)
		WHERE IDNO=@IDNO AND order_number=@OrderNo 
		AND (car_mgt_status<=3 AND cancel_status=0 AND booking_status<3) 
		AND stop_pick_time>@NowTime 
		AND ((ProjType=0 AND start_time <= DATEADD(MINUTE,30,@NowTime)) --同站可提早30分鐘取車
			 --OR (start_time<=@NowTime)	--路邊通常都是預約後才取車
			 OR start_time <= DATEADD(MINUTE,10,@NowTime));	--20211213 ADD BY ADAM REASON.
	  IF @hasData=0
	  BEGIN
	   SET @Error=1;
	   SET @ErrorCode='ERR171';
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BeforeBookingStart';



