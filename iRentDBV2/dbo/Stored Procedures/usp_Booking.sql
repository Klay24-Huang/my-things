/****************************************************************
** Name: [dbo].[usp_Booking]
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
** EXEC @Error=[dbo].[usp_Booking]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/16 上午 11:03:46 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/16 上午 11:03:46    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_Booking]
	@IDNO					VARCHAR(10),            --身份證
    @ProjID					VARCHAR(10),            --專案代碼
	@ProjType               TINYINT,                --專案類型(0:同站;3:路邊;4:機車)
    @StationID				VARCHAR(6),			    --取車據點
	@CarType                VARCHAR(10),            --車款
    @RStationID				VARCHAR(6),			    --還車據點
    @SD						DATETIME,			    --預計取車時間
    @ED						DATETIME,			    --預計還車時間
	@StopPickTime           DATETIME,               --最後取車時間
    @Price					INT,					--預估租金
	@CarNo					VARCHAR(10),            --隨租隨還車號
	@Token                  VARCHAR(1024),          --ACCESS TOKEN
	@Insurance              TINYINT      ,          --是否使用安心服務(0:否;1:是)
	@InsurancePurePrice     INT          ,          --安心服務預估金額
	@PayMode                TINYINT      ,          --計費模式：0:以時計費;1:以分計費
	@LogID                  BIGINT                ,
	@haveCar                TINYINT         OUTPUT, --是否有車(0:否;1:是)
	@OrderNum               BIGINT     OUTPUT,
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
DECLARE @tmpCarNo VARCHAR(10);
--增加判斷是否有符合轉乘優惠
DECLARE @LastCarType			TINYINT;      --前筆訂單類型(1:汽車;2:機車)
DECLARE @LastOrderNumber		BIGINT;	      --前筆訂單編號
DECLARE @LastOrderPure			INT;		  --純租金
DECLARE @LastOrderOver			INT;		  --純逾時
DECLARE @LastOrderFinal			INT;		  --前筆訂單支付金額(含里程及逾時）
DECLARE @LastProjType			INT;          --前次專案類型
DECLARE @OrderDistanceOfMinutes INT;		  --距離前筆訂單多久
DECLARE @MaxDiscountOfPrice     INT;
--增加判斷是否有符合轉乘優惠結束
--目前各類型預約數
DECLARE @NormalRentBookingNowCount      TINYINT;
DECLARE @AnyRentBookingNowCount			TINYINT;
DECLARE @MotorRentBookingNowCount		TINYINT;
DECLARE @RentNowActiveType              TINYINT;
DECLARE @NowActiveOrderNum				BIGINT;
--目前各類型預約數結束
DECLARE  @INVKIND		TINYINT		--發票寄送方式 1:捐贈;2:email;3:二聯;4:三聯;5:手機條碼;6:自然人憑證
		,@CARRIERID		VARCHAR(20)	--手機條碼
		,@NPOBAN		VARCHAR(20)	--愛心碼
		,@UNIMNO		VARCHAR(10)	--統編

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_Booking';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @tmpCarNo='';
SET @hasData=0;
SET @IDNO=ISNULL (@IDNO,'');
SET @Token=ISNULL (@Token,'');
SET @NowTime=DATEADD(HOUR,8,GETDATE());
--增加判斷是否有符合轉乘優惠
SET @LastCarType=0;
SET @LastOrderNumber=0;
SET @LastOrderPure=-1;
SET @LastOrderOver=-1;
SET @LastOrderFinal=-1;
SET @LastProjType=-1;
SET @OrderDistanceOfMinutes=-1;
SET @MaxDiscountOfPrice=0;
--增加判斷是否有符合轉乘優惠結束
SET @NormalRentBookingNowCount=0;
SET @AnyRentBookingNowCount=0;
SET @MotorRentBookingNowCount=0;
SET @RentNowActiveType=5;
SET @NowActiveOrderNum=0;
SET @Insurance=ISNULL(@Insurance,0);
SET @InsurancePurePrice=ISNULL(@InsurancePurePrice,0);
SET @PayMode=ISNULL(@PayMode,0);

BEGIN TRY
	IF @Token='' OR @IDNO='' 
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
	--0.再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END
	--1.取出目前各類型預約數
	IF @Error=0
	BEGIN
		SELECT @NormalRentBookingNowCount=ISNULL([NormalRentBookingNowCount], 0) ,
			   @AnyRentBookingNowCount=ISNULL([AnyRentBookingNowCount], 0) ,
			   @MotorRentBookingNowCount=ISNULL([MotorRentBookingNowCount], 0) ,
			   @RentNowActiveType=ISNULL(RentNowActiveType, 5) ,
			   @NowActiveOrderNum=ISNULL(NowActiveOrderNum, 0)
		FROM [dbo].[TB_BookingStatusOfUser]
		WHERE IDNO=@IDNO;

		--1.1三個相加>=5
		IF (@NormalRentBookingNowCount+@AnyRentBookingNowCount+@MotorRentBookingNowCount)>=5
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR156'
		END
		--1.2判斷是否是同站且已到達3
		IF @Error=0
		BEGIN
			IF @ProjType=0 AND @NormalRentBookingNowCount>=3
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR157';
			END
		END
		--1.3判斷是否是路邊且已到達1
		IF @Error=0
		BEGIN
			IF @ProjType=3 AND @AnyRentBookingNowCount>=1
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR158';
			END
		END
		--1.4判斷是否是機且已經到達1
		IF @Error=0
		BEGIN
			IF @ProjType=4 AND @MotorRentBookingNowCount>=1
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR159';
			END
		END
	END
	--2.判斷有沒有同站有沒有重疊
	IF @Error=0
	BEGIN
		IF @ProjType=0
		BEGIN
			SELECT @hasData=COUNT(1)
			FROM TB_OrderMain
			WHERE (booking_status<5 AND car_mgt_status<16 AND cancel_status=0)
			  AND IDNO=@IDNO
			  AND ProjType=@ProjType
			  AND ((start_time BETWEEN @SD AND @ED)
				   OR (stop_time BETWEEN @SD AND @ED)
				   OR (@SD BETWEEN start_time AND stop_time)
				   OR (@ED BETWEEN start_time AND stop_time)
				   OR (DATEADD(MINUTE, -30, @SD) BETWEEN start_time AND stop_time)
				   OR (DATEADD(MINUTE, 30, @ED) BETWEEN start_time AND stop_time))

			IF @hasData=1
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR160'
			END
		END
	END

	--2.5 春節卡預約
	IF (@SD>=CAST('2021-02-09' AS DATETIME) AND @SD<= CAST('2021-02-16' AS DATETIME))
	BEGIN
		SET @Error=1
		SET @ErrorCode='ERR235'
	END

	--維修鎖定
	--IF (@SD>=CAST('2020-12-24 03:00:00' AS DATETIME) AND @SD<=CAST('2020-12-24 05:00:00' AS DATETIME)) OR
	--(@ED>=CAST('2020-12-24 03:00:00' AS DATETIME) AND @ED<=CAST('2020-12-24 05:00:00' AS DATETIME))
	--BEGIN
	--	SET @Error=1
	--	SET @ErrorCode='ERR906'
	--END

	--跨年交通管制，X0IU、X0IF、X0LL、X1Q9這四站12/31 12:00-1/1 05:00，這區間內限制無法預約
	IF @Error=0
	BEGIN
		IF (@SD>=CAST('2020-12-31 12:00:00' AS DATETIME) AND @SD<=CAST('2021-01-01 05:00:00' AS DATETIME)) OR
			(@ED>=CAST('2020-12-31 12:00:00' AS DATETIME) AND @ED<=CAST('2021-01-01 05:00:00' AS DATETIME))
		BEGIN
			IF @StationID='X0IU' OR @StationID='X0IF' OR @StationID='X0LL' OR @StationID='X1Q9'
			BEGIN
				SET @Error=1
				SET @ErrorCode='ERR161'
			END
		END
	END
	--2.6 卡會員狀態 20210104 ADD BY ADAM REASON.審核不通過不可預約
	IF @Error=0
	BEGIN
		IF EXISTS(SELECT Audit FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND Audit=2)
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
	END
	--3.判斷有沒有車可預約
	IF @Error=0
	BEGIN
		IF @ProjType=0	--專案類型(0:同站)
		BEGIN
			BEGIN TRAN
			DECLARE @tmpCar TABLE(
			    CarNO VARCHAR(10) NOT NULL PRIMARY KEY,
			    RentCount INT 
			);
			--將車號及出租次數先寫入暫存
			INSERT INTO @tmpCar
			SELECT  Car.CarNo,CarInfo.RentCount
			FROM  [TB_Car] AS Car
			INNER JOIN  [TB_CarInfo] AS CarInfo ON CarInfo.CarNo=Car.CarNo AND Car.CarType IN (
				SELECT VW.CARTYPE FROM [dbo].[VW_GetFullProjectCollectionOfCarTypeGroup] AS VW 
				WHERE CarTypeGroupCode =UPPER(@CarType) AND VW.PROJID=@ProjID AND VW.StationID=@StationID
			)
			WHERE available<=1   AND nowStationID=@StationID AND CarInfo.CID<>'';

			--由暫存取出是否有符合的車輛
			SELECT TOP 1 @tmpCarNo=CarNo
			FROM @tmpCar
			WHERE CarNO NOT IN
				(SELECT CarNo
				 FROM TB_OrderMain
				 WHERE (booking_status<5 AND car_mgt_status<16 AND cancel_status=0)
				   AND CarNo IN (SELECT CarNo FROM @tmpCar)
				   AND ((start_time BETWEEN @SD AND @ED)
						OR (stop_time BETWEEN @SD AND @ED)
						OR (@SD BETWEEN start_time AND stop_time)
						OR (@ED BETWEEN start_time AND stop_time)
						OR (DATEADD(MINUTE, -30, @SD) BETWEEN start_time AND stop_time)
						OR (DATEADD(MINUTE, 30, @ED) BETWEEN start_time AND stop_time)) )
			ORDER BY RentCount ASC;

			--判斷有沒有預約到車
			IF @tmpCarNo='' 
			BEGIN
				SET @haveCar=0;
				ROLLBACK TRAN;
			END
			ELSE
			BEGIN
				--20201124 ADD BY ADAM REASON.由會員檔取出發票資料
				SELECT @INVKIND=MEMSENDCD,@CARRIERID=CARRIERID,@NPOBAN=NPOBAN,@UNIMNO=UNIMNO FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO
				--資料梳理
				IF @INVKIND=1
				BEGIN
					SET @UNIMNO=''
					SET @CARRIERID=''
				END
				IF @INVKIND=2 OR @INVKIND=3
				BEGIN
					SET @NPOBAN=''
					SET @UNIMNO=''
					SET @CARRIERID=''
				END
				IF @INVKIND=4
				BEGIN
					SET @NPOBAN=''
					SET @CARRIERID=''
				END
				IF @INVKIND=5
				BEGIN
					SET @NPOBAN=''
					SET @UNIMNO=''
				END

				INSERT INTO TB_OrderMain
				(
					IDNO,CarNo,ProjID,lend_place,return_place,start_time,stop_time,
					init_price,Insurance,InsurancePurePrice,ProjType,PayMode,stop_pick_time,
					bill_option, title, unified_business_no, invoiceAddress, CARRIERID, NPOBAN
				)
				VALUES(
					@IDNO,@tmpCarNo,@ProjID,@StationID,@StationID,@SD,@ED,
					@Price,@Insurance,@InsurancePurePrice,@ProjType,@PayMode,@StopPickTime,
					@INVKIND, '', @UNIMNO, '', @CARRIERID, @NPOBAN			--發票抬頭跟發票地址沒有設定
				);

				IF @@ROWCOUNT=1
				BEGIN
					SET @OrderNum=@@IDENTITY;
					SET @haveCar=1;

					--更新出租次數（取消需減回來）
					UPDATE TB_CarInfo 
					SET RentCount=RentCount+1,
						UPDTime=@NowTime
					WHERE CarNo=@tmpCarNo;

					--更新預約列表
					SET @hasData=0;
					SELECT @hasData=COUNT(1) FROM TB_BookingStatusOfUser WHERE IDNO=@IDNO;
					IF @hasData=0
					BEGIN								
						SET @NormalRentBookingNowCount=1;
						INSERT INTO TB_BookingStatusOfUser(IDNO,NormalRentBookingNowCount,NormalRentBookingTotalCount)
						VALUES(@IDNO,@NormalRentBookingNowCount,@NormalRentBookingNowCount);
					END
					ELSE
					BEGIN
						UPDATE TB_BookingStatusOfUser
						SET NormalRentBookingNowCount=@NormalRentBookingNowCount+1,
							NormalRentBookingTotalCount=NormalRentBookingTotalCount+1,
							UPDTime=@NowTime
						WHERE IDNO=@IDNO;
					END
					--寫入歷史記錄
					INSERT INTO TB_OrderHistory(OrderNum)VALUES(@OrderNum);
					COMMIT TRAN;
				END
				ELSE
				BEGIN
					ROLLBACK TRAN;
					SET @Error=1;
					SET @ErrorCode='ERR161';
				END
			END
			--判斷有無預約到車結束  
		END
		ELSE	--專案類型(3:路邊;4:機車)
		BEGIN
			BEGIN TRAN
			SET @hasData=0;
			SELECT @hasData=COUNT(CarNo)
			FROM TB_OrderMain
			WHERE (booking_status<5 AND car_mgt_status<16 AND cancel_status=0)
			  AND CarNo=@CarNo
			  AND ((start_time BETWEEN @SD AND @ED)
				   OR (stop_time BETWEEN @SD AND @ED)
				   OR (@SD BETWEEN start_time AND stop_time)
				   OR (@ED BETWEEN start_time AND stop_time)
				   OR (DATEADD(MINUTE, -30, @SD) BETWEEN start_time AND stop_time)
				   OR (DATEADD(MINUTE, 30, @ED) BETWEEN start_time AND stop_time))

			--print 'HASDATA=' + CAST(@hasData AS VARCHAR)
			IF @hasData=0
			BEGIN
				SELECT @INVKIND=MEMSENDCD,@CARRIERID=CARRIERID,@NPOBAN=NPOBAN,@UNIMNO=UNIMNO FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO
				--資料梳理
				IF @INVKIND=1
				BEGIN
					SET @UNIMNO=''
					SET @CARRIERID=''
				END
				IF @INVKIND=2 OR @INVKIND=3
				BEGIN
					SET @NPOBAN=''
					SET @UNIMNO=''
					SET @CARRIERID=''
				END
				IF @INVKIND=4
				BEGIN
					SET @NPOBAN=''
					SET @CARRIERID=''
				END
				IF @INVKIND=5
				BEGIN
					SET @NPOBAN=''
					SET @UNIMNO=''
				END

				INSERT INTO TB_OrderMain
				(
					IDNO,CarNo,ProjID,lend_place,return_place,
					start_time,stop_time,init_price,Insurance,InsurancePurePrice,
					ProjType,PayMode,stop_pick_time,
					bill_option, title, unified_business_no, invoiceAddress, CARRIERID, NPOBAN
				)
				VALUES
				(
					@IDNO,@CarNo,@ProjID,@StationID,@StationID,
					@SD,@ED,@Price,@Insurance,@InsurancePurePrice,
					@ProjType,@PayMode,@StopPickTime,
					@INVKIND, '', @UNIMNO, '', @CARRIERID, @NPOBAN	--發票抬頭跟發票地址沒有設定
				);
				
				IF @@ROWCOUNT=1
				BEGIN
					PRINT 'ROWCOUNT='
					SET @OrderNum=@@IDENTITY;
					SET @haveCar=1;

					--更新出租次數（取消需減回來）
					UPDATE TB_CarInfo 
					SET RentCount=RentCount+1,
						UPDTime=@NowTime
					WHERE CarNo=@CarNo;

					--更新預約列表
					SET @hasData=0;
					SELECT @hasData=COUNT(1) FROM TB_BookingStatusOfUser WHERE IDNO=@IDNO;
					IF @hasData=0
					BEGIN	
						IF @ProjType=3
						BEGIN
							SET @AnyRentBookingNowCount=1;
							INSERT INTO TB_BookingStatusOfUser(IDNO,AnyRentBookingNowCount,AnyRentBookingTotalCount)
							VALUES(@IDNO,@AnyRentBookingNowCount,@AnyRentBookingNowCount);
						END
						ELSE
						BEGIN
							SET @MotorRentBookingNowCount=1;
							INSERT INTO TB_BookingStatusOfUser(IDNO,MotorRentBookingNowCount,MotorRentBookingTotalCount)
							VALUES(@IDNO,@MotorRentBookingNowCount,@MotorRentBookingNowCount);
						END
					END
					ELSE
					BEGIN
						IF @ProjType=3
						BEGIN
							UPDATE TB_BookingStatusOfUser
							SET AnyRentBookingNowCount=AnyRentBookingNowCount+1,
								AnyRentBookingTotalCount=MotorRentBookingTotalCount+1,
								UPDTime=@NowTime
							WHERE IDNO=@IDNO;
						END
						ELSE
						BEGIN
							UPDATE TB_BookingStatusOfUser
							SET MotorRentBookingNowCount=MotorRentBookingNowCount+1,
								MotorRentBookingTotalCount=MotorRentBookingTotalCount+1,
								UPDTime=@NowTime
							WHERE IDNO=@IDNO;
						END
					END
					--寫入歷史記錄
					INSERT INTO TB_OrderHistory(OrderNum)VALUES(@OrderNum);
					COMMIT TRAN;
				END
				ELSE
				BEGIN
					ROLLBACK TRAN;
					SET @haveCar=0;
					IF @ProjType=3
					BEGIN
						SET @Error=1;
						SET @ErrorCode='ERR162';
					END
					BEGIN
						SET @Error=1;
						SET @ErrorCode='ERR163';
					END
				END
			END
			ELSE
			BEGIN
				ROLLBACK TRAN;
				SET @haveCar=0;
				IF @ProjType=3
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR162';
				END
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR163';
				END
			END
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Booking';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Booking';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Booking';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Booking';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Booking';