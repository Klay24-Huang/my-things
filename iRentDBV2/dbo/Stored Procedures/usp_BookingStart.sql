/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_BookingStart
* 系    統 : IRENT
* 程式功能 : 汽車取車
* 作    者 : ERIC
* 撰寫日期 : 2020924
* 修改日期 : 20210217;和天霖確認轉乘優惠規則：
						1.實際租金=0，就沒有轉乘優惠
						2.不論月租/折抵的狀況如何，車輛租金要>0才可計算轉乘優惠，用(車輛租金-轉乘優惠金額)來判斷
			 20210720 ADD BY ADAM REASON.7/21後轉乘優惠改為60元
			 20210831 UPD BY YEH REASON:增加共同承租人判斷
			 20210901 UPD BY YEH REASON:調整共同承租人判斷

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_BookingStart]
	@IDNO                   VARCHAR(10)           ,	--帳號
	@OrderNo                BIGINT                ,	--訂單編號
	@Token                  VARCHAR(1024)         ,	--JWT TOKEN
	@StopTime               VARCHAR(20)           , --路邊租還才能更改結束日       
	@NowMileage             FLOAT                 ,	--取車里程
	@LogID                  BIGINT                ,	--執行的api log
	@Insurance				INT					  , --加購安心服務(0:否;1:有)
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
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;
DECLARE @RentNowActiveType TINYINT;
DECLARE @NowActiveOrderNum BIGINT;
DECLARE @IsMotor INT;					-- 是否為機車
DECLARE @PrevOrderNo INT;				-- 上一筆訂單編號
DECLARE @PrevIsMotor INT;				-- 上一筆是否為機車
DECLARE @PrevRentPrice INT;				-- 上一筆租金
DECLARE @PrevFinalStopTime DATETIME;	-- 上一筆還車時間
DECLARE @PrevFinalPrice INT;			-- 上一筆實際費用
DECLARE @PrevTransDiscount INT;			-- 上一筆轉乘優惠金額
DECLARE @TransferPrice INT;				-- 轉乘優惠金額
DECLARE @CarTypeGroupCode VARCHAR(20);	-- 車型簡碼
DECLARE @MainInsurancePerHour INT;		-- 主承租人每小時安心服務價格
DECLARE @JointInsurancePerHour INT;		-- 共同承租人每小時安心服務價格
DECLARE @JointCount INT;				-- 共同承租人數
DECLARE @JointInsurancePrice INT;		-- 共同承租人費用
DECLARE @UseInsurance INT;				-- 使用安心服務(1:可;0:不可)
DECLARE @MainInsuLV INT;				-- 主承租人安心服務等級
DECLARE @TotalInsurancePerHour INT;		-- 每小時安心服務總金額
DECLARE @TotalInsurancePrice INT;		-- 安心服務預計金額
DECLARE @StartDate DATETIME;			-- 預計取車時間
DECLARE @StopDate DATETIME;				-- 預計還車時間

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BookingStart';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @Descript=N'使用者操作【取車】';
SET @RentNowActiveType=5;
SET @NowActiveOrderNum=0;
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token=ISNULL (@Token,'');
SET @StopTime =ISNULL(@StopTime,'');
SET @NowMileage=ISNULL(@NowMileage,0);
SET @Insurance=ISNULL(@Insurance,0);
SET @PrevOrderNo=0;
SET @PrevRentPrice=0;
SET @PrevTransDiscount=0;
SET @PrevFinalPrice=0;
SET @TransferPrice=0;
SET @CarTypeGroupCode='';
SET @MainInsurancePerHour=0;
SET @JointInsurancePerHour=0;
SET @JointCount=0;
SET @JointInsurancePrice=0;
SET @UseInsurance=0;
SET @MainInsuLV=0;
SET @TotalInsurancePerHour=0;
SET @TotalInsurancePrice=0;

BEGIN TRY
	IF @Token='' OR @IDNO='' OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	--0.再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token  AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END

	--檢核會員狀態
	IF @Error=0
	BEGIN
		--審核不通過不可取車
		IF EXISTS(SELECT Audit FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND (Audit=2 OR HasCheckMobile=0))
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR239';
		END
	END

	--取訂單資訊
	IF @Error=0
	BEGIN
		SELECT @booking_status=booking_status,
				@cancel_status=cancel_status,
				@car_mgt_status=car_mgt_status,
				@CarNo=CarNo,
				@ProjType=ProjType,
				@IsMotor=CASE WHEN ProjType=4 THEN 1 ELSE 0 END
		FROM TB_OrderMain WITH(NOLOCK)
		WHERE order_number=@OrderNo;
	END

	IF @Error=0
	BEGIN
		SELECT @RentNowActiveType=ISNULL(RentNowActiveType,5),@NowActiveOrderNum=ISNULL(NowActiveOrderNum,0)
		FROM [dbo].[TB_BookingStatusOfUser] WITH(NOLOCK)
		WHERE IDNO=@IDNO;

		IF @RentNowActiveType NOT IN (0,5) AND @NowActiveOrderNum>0
		BEGIN
			--20210104 ADD BY ADAM REASON.針對還在目前案件做判斷
			IF EXISTS(SELECT order_number FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@NowActiveOrderNum AND car_mgt_status=16)
			BEGIN
				--已還車要更新
				UPDATE TB_BookingStatusOfUser SET @NowActiveOrderNum=0,UPDTime=@NowTime WHERE IDNO=@IDNO;
				SET @NowActiveOrderNum=0;
			END
			ELSE
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR172';
			END
		END
	END

	--IF @Error=0
	--BEGIN
	--	IF @ProjType=3 AND @StopTime<>''
	--	BEGIN
	--		SELECT @hasData=COUNT(1) FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo AND start_time>CONVERT(datetime,@StopTime);
	--		IF @hasData=0
	--		BEGIN
	--			SET @Error=1;
	--			SET @ErrorCode='ERR175';
	--		END
	--	END
	--END

	-- 檢查是否有前車未還
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

	IF @Error=0
	BEGIN
		BEGIN TRAN
		SET @hasData=0
		SELECT @hasData=COUNT(order_number) FROM TB_OrderMain WITH(NOLOCK)
		WHERE IDNO=@IDNO AND order_number=@OrderNo 
		AND (car_mgt_status<=3 AND cancel_status=0 AND booking_status<3) 
		AND stop_pick_time>@NowTime 
		AND ((ProjType=0 AND start_time <= DATEADD(MINUTE,30,@NowTime)) --同站可提早30分鐘取車
			 OR (start_time<=@NowTime)	--路邊通常都是預約後才取車
			);
		
		--寫入記錄
		IF @hasData>0
		BEGIN
			--如果取不到里程，從tb取出
			IF @NowMileage=0
			BEGIN
				SELECT @NowMileage=Millage FROM TB_CarStatus WITH(NOLOCK) WHERE CarNo=@CarNo;
			END

			--轉乘優惠判斷，先找上一筆訂單
			SELECT TOP 1 @PrevOrderNo=A.order_number
						,@PrevIsMotor=CASE WHEN A.ProjType=4 THEN 1 ELSE 0 END
						,@PrevFinalStopTime=B.final_stop_time
						,@PrevRentPrice=B.pure_price
						,@PrevFinalPrice=B.final_price
						,@PrevTransDiscount=B.TransDiscount
			FROM TB_OrderMain A WITH(NOLOCK)
			JOIN TB_OrderDetail B WITH(NOLOCK) ON A.order_number=B.order_number
			WHERE A.IDNO=@IDNO AND A.car_mgt_status=16 AND A.booking_status=5
			ORDER BY B.final_stop_time DESC

			--運具轉換且時間在一個小時內轉乘
			IF @PrevOrderNo>0 AND @IsMotor<>@PrevIsMotor AND DATEDIFF(minute,@PrevFinalStopTime,@NowTime) <=60
			BEGIN
				--20210217;和天霖確認轉乘優惠規則：
				--1.實際租金=0，就沒有轉乘優惠
				--2.不論月租/折抵的狀況如何，車輛租金要>0才可計算轉乘優惠，用(車輛租金-轉乘優惠金額)來判斷
				--20210720 ADD BY ADAM REASON.7/21後轉乘優惠改為60元
				DECLARE @TransferPriceMax INT=46
				IF dbo.GET_TWDATE()>'2021-07-21'
				BEGIN
					SET @TransferPriceMax = 60
				END

				IF @PrevFinalPrice > 0
				BEGIN
					IF @PrevRentPrice > 0
					BEGIN
						IF (@PrevRentPrice-@PrevTransDiscount) >= @TransferPriceMax
						BEGIN
							SET @TransferPrice = @TransferPriceMax;
						END
						ELSE
						BEGIN
							SET @TransferPrice = (@PrevRentPrice-@PrevTransDiscount);
						END
					END
				END
			END

			--寫入訂單明細
			INSERT INTO TB_OrderDetail(order_number,already_lend_car,final_start_time,start_mile)
			VALUES(@OrderNo,1,@NowTime,@NowMileage);

			--更新訂單主檔
			IF @ProjType=3 AND @StopTime<>''
			BEGIN
				UPDATE TB_OrderMain 
				SET stop_time=@stopTime,
					car_mgt_status=4,
					Insurance=@Insurance,
					init_TransDiscount=@TransferPrice	--20201201 ADD BY ADAM REASON增加轉乘優惠
				WHERE order_number=@OrderNo AND start_time<CONVERT(datetime,@StopTime);
			END
			ELSE
			BEGIN
				UPDATE TB_OrderMain 
				SET car_mgt_status=4,
					Insurance=@Insurance,
					init_TransDiscount=@TransferPrice	--20201201 ADD BY ADAM REASON增加轉乘優惠
				WHERE order_number=@OrderNo;
			END

			--更新主控表
			UPDATE TB_BookingStatusOfUser
			SET RentNowActiveType=@ProjType,
				NowActiveOrderNum=@OrderNo,
				UPDTime=@NowTime
			WHERE IDNO=@IDNO;

			--寫入歷程
			INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
			VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);

			--更新車輛狀態
			UPDATE TB_Car 
			SET available=0,
				NowOrderNo=@OrderNo,
				UPDTime=@NowTime 
			WHERE CarNo=@CarNo;

			--加入機車取車時的電池電量及經緯度
			IF @ProjType=4
			BEGIN
				INSERT INTO TB_OrderDataByMotor(OrderNo,P_lat,P_lon,P_LBA,P_RBA,P_MBA,P_TBA)
				SELECT @OrderNo,Latitude,Longitude,deviceLBA,deviceRBA,deviceMBA,device3TBA 
				FROM TB_CarStatus WITH(NOLOCK) WHERE CarNo=@CarNo
			END

			-- 20210831 UPD BY YEH REASON:增加共同承租人判斷
			SET @hasData=0;
			SELECT @hasData=COUNT(*) FROM TB_TogetherPassenger WITH(NOLOCK) WHERE Order_number=@OrderNo;
			IF @hasData > 0
			BEGIN
				-- 將尚未回應的邀請人狀態改為取消
				UPDATE TB_TogetherPassenger
				SET ChkType='N',
					UPTime=@NowTime
				WHERE Order_number=@OrderNo
				AND ChkType='S';

				-- 非機車才可以用安心服務
				IF @ProjType <> 4
				BEGIN
					-- 使用者選要使用安心服務，則先設定為使用，後續再判斷訂單是否可使用
					IF @Insurance = 1
					BEGIN
						SET @UseInsurance=1;
					END

					-- 取得主承租人安心服務等級
					SELECT @MainInsuLV=InsuranceLevel FROM TB_BookingInsuranceOfUser WITH(NOLOCK) WHERE IDNO=@IDNO;

					-- 判斷主承租人是否可使用安心服務，LEVEL>=4就不能用
					IF @MainInsuLV >= 4
					BEGIN
						SET @UseInsurance=0;
					END
					ELSE
					BEGIN
						-- 判斷副承租人是否可使用安心服務，有人LEVEL>=4就不能用
						SET @hasData=0;
						SELECT @hasData=COUNT(*) FROM TB_TogetherPassenger A WITH(NOLOCK)
						INNER JOIN TB_BookingInsuranceOfUser B WITH(NOLOCK) ON B.IDNO=A.MEMIDNO
						WHERE A.Order_number=@OrderNo AND A.ChkType='Y' AND B.InsuranceLevel >= 4;

						IF @hasData > 0
						BEGIN
							SET @UseInsurance=0;
						END
					END

					-- 使用安心服務，才取各項金額來加總計算
					IF @UseInsurance = 1
					BEGIN
						-- 取車型簡碼
						SELECT @CarTypeGroupCode=D.CarTypeGroupCode FROM TB_Car A WITH(NOLOCK)
						INNER JOIN TB_CarType B WITH(NOLOCK) ON B.CarType=A.CarType
						INNER JOIN TB_CarTypeGroupConsist C WITH(NOLOCK) ON C.CarType=B.CarType
						INNER JOIN TB_CarTypeGroup D WITH(NOLOCK) ON D.CarTypeGroupID=C.CarTypeGroupID
						WHERE CarNo=@CarNo;

						-- 取主承租人每小時安心服務價格
						SELECT @MainInsurancePerHour=InsurancePerHours FROM TB_InsuranceInfo WITH(NOLOCK) WHERE InsuranceLevel=@MainInsuLV AND CarTypeGroupCode=@CarTypeGroupCode;

						-- 取共同承租人每小時安心服務價格
						SELECT @JointInsurancePerHour=InsurancePerHours FROM TB_InsuranceInfoOther WITH(NOLOCK) WHERE CarTypeGroupCode=@CarTypeGroupCode;
		
						-- 取有同意的共同承租人數
						SELECT @JointCount=COUNT(*) FROM TB_TogetherPassenger WITH(NOLOCK) WHERE Order_number=@OrderNo AND ChkType='Y';

						-- 每小時安心服務總金額 = 主承租人每小時安心服務價格 + 共同承租人數 * 共同承租人每小時安心服務價格
						SET @TotalInsurancePerHour = @MainInsurancePerHour + (@JointCount * @JointInsurancePerHour);

						-- 取得訂單起訖時間
						SELECT @StartDate=start_time,@StopDate=stop_time FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo;
				
						-- 計算安心服務預估金額
						SELECT @TotalInsurancePrice=dbo.FN_CarRentCompute(@StartDate, @StopDate, @TotalInsurancePerHour * 10, @TotalInsurancePerHour * 10, 10, 0);
					END
				END
				
				-- 寫入共同承租人檔
				INSERT INTO TB_SavePassenger (Order_number,MEMIDNO,MEMCNAME,MEMTEL,InsurancePerHours,HistFlg,MKTime,UPTime)
				SELECT Order_number,MEMIDNO,MEMCNAME,MEMTEL,@JointInsurancePerHour,1,@NowTime,@NowTime
				FROM TB_TogetherPassenger WITH(NOLOCK) 
				WHERE Order_number=@OrderNo AND ChkType='Y';

				-- 更新回訂單檔
				UPDATE TB_OrderMain 
				SET Insurance=@UseInsurance,
					InsurancePurePrice=@TotalInsurancePrice
				WHERE order_number=@OrderNo;
			END

			COMMIT TRAN;
		END
		ELSE
		BEGIN
			ROLLBACK TRAN;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingStart';
GO