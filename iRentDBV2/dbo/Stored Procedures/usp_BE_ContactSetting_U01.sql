/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_BE_ContactSetting_U01
* 系    統 : IRENT
* 程式功能 : 後台強還存檔
* 作    者 : YEH
* 撰寫日期 : 20211109
* 修改日期 : 20211202 UPD BY YEH REASON:整筆退才寫TB_OrderAuthReturn
			 20211216 ADD BY ADAM REASON.增加isnull判斷
			 20211217 UPD BY YEH REASON:增加信用卡類別(0:和泰;1:台新)
			 20211229 UPD BY YEH REASON:信用卡類別用付款方式區分
			 20220216 UPD BY YEH REASON:增加更新A_PRGID,U_PRGID
			 20220223 UPD BY YEH REASON:增加錢包退款
			 20220223 UPD BY YEH REASON:PAYAMT = 關帳檔總金額 + 錢包扣款總金額 - 錢包退款總金額
* Example  :
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_BE_ContactSetting_U01]
	@Msg					VARCHAR(100)	OUTPUT	,	-- 回傳錯誤訊息
	@IDNO					VARCHAR(10)				,	-- 帳號
	@OrderNo				BIGINT					,	-- 訂單編號
	@UserID					NVARCHAR(10)			,	-- 操作者
	@ReturnDate				DATETIME				,	-- 強還時間
	@bill_option			INT						,	-- 發票寄送方式：1:捐贈;2:email;3:二聯;4:三聯;5:手機條碼;6:自然人憑證
	@unified_business_no	VARCHAR(8)				,	-- 統一編號
	@CARRIERID				VARCHAR(20)				,	-- 手機條碼載具,自然人憑證載具
	@NPOBAN					VARCHAR(20)				,	-- 愛心碼
	@ParkingSpace           NVARCHAR(256)			,	-- 停車格
	@Mode					INT						,	-- 強還動作(3:系統操作異常/4:逾時未還/5:營運範圍外無法還車/6:車輛沒電/7:其他)
	@LogID                  BIGINT					,	-- LogID
	@TradeClose				TY_TradeClose	READONLY	-- TradeClose清單
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @Error INT;
	DECLARE @ErrorCode VARCHAR(6);				--回傳錯誤代碼
	DECLARE	@ErrorMsg NVARCHAR(100);
	DECLARE @SQLExceptionCode VARCHAR(10);		--回傳sqlException代碼
	DECLARE @SQLExceptionMsg NVARCHAR(1000);	--回傳sqlException訊息

	DECLARE @IsSystem TINYINT;
	DECLARE @FunName VARCHAR(50);
	DECLARE @ErrorType TINYINT;
	DECLARE @hasData TINYINT;
	DECLARE @NowTime DATETIME;

	DECLARE @car_mgt_status TINYINT;
	DECLARE @cancel_status TINYINT;
	DECLARE @booking_status TINYINT;
	DECLARE @Descript NVARCHAR(200);
	DECLARE @CarNo VARCHAR(10);
	DECLARE @ProjType INT;
	DECLARE @ProjID VARCHAR(10);
	DECLARE @RSOC_S FLOAT
			,@RSOC_E FLOAT
			,@RSOC1 FLOAT
			,@CHANGETIME INT
			,@RewardGift INT=0
			,@ChgGift INT
			,@CHKSEQNO INT
			,@CHKCarNo VARCHAR(10)
			,@CHKDT DATETIME
			,@LBA FLOAT
			,@RBA FLOAT
	DECLARE @Reward INT;		-- 換電獎勵
	DECLARE @DiffAmount INT;	-- 尾款
	DECLARE @final_price INT;	-- 總計
	DECLARE @NoPreAuth INT;		-- 預授權不處理專案 (0:要處理 1:不處理)
	DECLARE @PayMode INT;		-- 付款方式
	DECLARE @CardType INT;		-- 信用卡類別(0:和泰; 1:台新)

	/*初始設定*/
	SET @Error=0;
	SET @ErrorCode='0000';
	SET @ErrorMsg='SUCCESS'; 
	SET @SQLExceptionCode='';
	SET @SQLExceptionMsg='';
	SET @IsSystem=0;
	SET @FunName='usp_BE_ContactSetting_U01';
	SET @ErrorType=0;
	SET @hasData=0;
	SET @NowTime=DATEADD(HOUR,8,GETDATE());

	SET @Descript=N'後台強還，設定狀態為已還車';
	SET @car_mgt_status=0;
	SET @cancel_status =0;
	SET @booking_status=0;
	SET @CarNo='';
	SET @ProjType=5;
	SET @IDNO=ISNULL (@IDNO,'');
	SET @OrderNo=ISNULL (@OrderNo,0);
	SET @ParkingSpace=ISNULL(@ParkingSpace,'');
	SET @Reward=0;
	SET @DiffAmount=0;
	SET @final_price=0;
	SET @NoPreAuth=0;
	SET @CardType=1;	-- 信用卡類別(預設台新信用卡)

	BEGIN TRY
		IF @IDNO='' OR @OrderNo=0 OR @UserID=''
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR900';
		END

		IF @Error=0
		BEGIN
			BEGIN TRAN

			SELECT @booking_status=booking_status,
				@cancel_status=cancel_status,
				@car_mgt_status=car_mgt_status,
				@CarNo=CarNo,
				@ProjType=ProjType,
				@ProjID=ProjID
			FROM TB_OrderMain WITH(NOLOCK)
			WHERE order_number=@OrderNo;

			SELECT @final_price=final_price FROM TB_OrderDetail WITH(NOLOCK) WHERE order_number=@OrderNo;

			-- 專案代碼是否不處理預授權
			SELECT @NoPreAuth=ISNULL(1,0) FROM TB_Code WITH(NOLOCK) WHERE CodeGroup='PreAuth' AND MapCode=@ProjID AND UseFlag=1;

			SELECT @hasData=COUNT(1) FROM TB_ParkingSpaceTmp WITH(NOLOCK) WHERE OrderNo=@OrderNo;
			IF @hasData>0
			BEGIN
				-- 20201214 改由前端客服維護
				INSERT INTO TB_ParkingSpace (OrderNo,ParkingImage,ParkingSpace)
				SELECT OrderNo,ParkingImage,ParkingSpace=CASE WHEN @ParkingSpace='' THEN ParkingSpace ELSE @ParkingSpace END
				FROM TB_ParkingSpaceTmp WITH(NOLOCK) WHERE OrderNo=@OrderNo;

				DELETE FROM TB_ParkingSpaceTmp WHERE OrderNo=@OrderNo;
			END

			SET @Descript=CONCAT('使用者',@UserID,'操作：',@Descript);

			--寫入歷程
			INSERT INTO TB_OrderHistory (OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
			VALUES (@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);

			--20210220 ADD BY ADAM REASON.發票資訊需要整理過在寫入
			--資料梳理
			IF @bill_option=1	--捐贈
			BEGIN
				SET @unified_business_no='';
				SET @CARRIERID='';
			END
			IF @bill_option=2 OR @bill_option=3		--EMAIL or 二聯
			BEGIN
				SET @NPOBAN='';
				SET @unified_business_no='';
				SET @CARRIERID='';
			END
			IF @bill_option=4	--三聯
			BEGIN
				SET @NPOBAN='';
				SET @CARRIERID='';
			END	
			IF @bill_option=5 OR @bill_option=6		--載具 or 自然人憑證
			BEGIN
				SET @NPOBAN='';
				SET @unified_business_no='';
			END
					
			--更新訂單主檔
			UPDATE TB_OrderMain
			SET car_mgt_status=16,
				booking_status=5,
				modified_status=2,
				--20201130 UPD BY JERRY 增加發票寄送方式相關欄位
				bill_option=@bill_option,
				unified_business_no=@unified_business_no,
				CARRIERID=@CARRIERID,
				NPOBAN=@NPOBAN
			WHERE order_number=@OrderNo;
				
			-- 更新訂單明細
			-- 20201110 UPD BY JERRY 不管何種狀態都要更新final_stop_time
			UPDATE A
			SET already_return_car=1,
				already_payment=1,
				final_stop_time=@ReturnDate,
				parkingSpace=@ParkingSpace
			FROM TB_OrderDetail A
			LEFT JOIN TB_OrderMain B ON A.order_number=B.order_number
			WHERE A.order_number=@OrderNo;
			
			--20201010 ADD BY ADAM REASON.還車改為只針對個人訂單狀態去個別處理
			--更新個人訂單控制
			IF @ProjType=4	-- 路邊機車
			BEGIN
				UPDATE TB_BookingStatusOfUser
				SET MotorRentBookingNowCount=CASE WHEN MotorRentBookingNowCount -1 < 0 THEN 0 ELSE MotorRentBookingNowCount -1 END,
					RentNowActiveType=5,
					NowActiveOrderNum=0,
					MotorRentBookingFinishCount=MotorRentBookingFinishCount + 1,
					UPDTime=@NowTime
				WHERE IDNO=@IDNO;

				--20201201 ADD BY ADAM REASON.換電獎勵處理
				IF EXISTS(SELECT OrderNo FROM TB_OrderDataByMotor WITH(NOLOCK) WHERE OrderNo=@OrderNo)
				BEGIN
					--寫入機車還車時的資訊 20201030 ADD BY ERIC
					UPDATE TB_OrderDataByMotor
					SET R_lat=B.Latitude,
						R_lon=B.Longitude,
						R_LBA=deviceLBA,
						R_RBA=deviceRBA,
						R_MBA=deviceMBA,
						R_TBA=device3TBA,
						Reward=CASE WHEN device3TBA-P_TBA>=99 THEN 0
									WHEN device3TBA-P_TBA>=40 THEN 20
									WHEN device3TBA-P_TBA>=20 THEN 10
									ELSE 0 END,
						UPDTime=@NowTime
					FROM TB_CarStatus B WITH(NOLOCK)
					WHERE B.CarNo=@CarNo AND TB_OrderDataByMotor.OrderNo=@OrderNo;
				
					--取出換電獎勵
					SELECT @Reward=Reward FROM TB_OrderDataByMotor WITH(NOLOCK) WHERE OrderNo=@OrderNo;

					--有可能寫入失敗
					IF NOT EXISTS(SELECT SEQNO FROM TB_MotorChangeBattLOG WITH(NOLOCK) WHERE order_number=@OrderNo AND EventCD='1')
					BEGIN
						--20211216 ADD BY ADAM REASON.增加isnull判斷
						INSERT INTO TB_MotorChangeBattLOG(order_number,CarNo,EventCD,RSOC,LBA,RBA,MKTime,CHKCD,REFNO)
						SELECT @OrderNo,@CarNo,'1',P_TBA,ISNULL(P_LBA,0),ISNULL(P_RBA,0),MKTime,0,0 FROM TB_OrderDataByMotor WITH(NOLOCK) WHERE OrderNo=@OrderNo
					END

					DROP TABLE IF EXISTS #TB_MotorChangeBattLog;

					SELECT * 
					INTO #TB_MotorChangeBattLog
					FROM TB_MotorChangeBattLOG WITH(NOLOCK) WHERE order_number=@OrderNo;

					--整理換電LOG，補上沒查過儀表板電量的資料
					DECLARE ChangeBattCur CURSOR FOR
					SELECT SEQNO,CarNo,MKTime
					FROM #TB_MotorChangeBattLog WHERE CHKCD=0 and EventCD IN (1,2,3,4);

					OPEN ChangeBattCur

					FETCH NEXT FROM ChangeBattCur INTO @CHKSEQNO,@CHKCarNo,@CHKDT
					WHILE @@FETCH_STATUS <> -1
					BEGIN
						--20211216 ADD BY ADAM REASON.增加isnull判斷
						SELECT TOP 1 @RSOC1= deviceRSOC,@RBA=ISNULL(deviceRBA,0),@LBA=ISNULL(deviceLBA,0) FROM TB_CarRawData WITH(NOLOCK) WHERE CarNo=@CHKCarNo AND GPSTime > @CHKDT AND deviceRSOC is not null and deviceRSOC<>'' AND deviceRSOC<>'NA';

						--20210616 ADD BY ADAM REASON.取不到值就以3TBA為主
						IF @RSOC1 IS NULL
						BEGIN
							SELECT TOP 1 @RSOC1=device3TBA FROM TB_CarRawData WITH(NOLOCK) WHERE CarNo=@CHKCarNo AND GPSTime > @CHKDT AND device3TBA>0;
						END

						--20211216 ADD BY ADAM REASON.增加isnull判斷
						UPDATE TB_MotorChangeBattLOG SET RSOC=ISNULL(@RSOC1,0),LBA=ISNULL(@LBA,0),RBA=ISNULL(@RBA,0),CHKCD=1 WHERE SEQNO=@CHKSEQNO;
						UPDATE #TB_MotorChangeBattLOG SET RSOC=ISNULL(@RSOC1,0),LBA=ISNULL(@LBA,0),RBA=ISNULL(@RBA,0),CHKCD=1 WHERE SEQNO=@CHKSEQNO;

						FETCH NEXT FROM ChangeBattCur INTO @CHKSEQNO,@CHKCarNo,@CHKDT
					END

					CLOSE ChangeBattCur
					DEALLOCATE ChangeBattCur
					--PRINT 'ChangeBattCur END'

					--取車電量
					SELECT TOP 1 @RSOC_S=RSOC FROM #TB_MotorChangeBattLOG WITH(NOLOCK) WHERE order_number=@OrderNo AND EventCD=1 ORDER BY MKTime;
					--還車電量
					SELECT TOP 1 @RSOC_E=RSOC FROM #TB_MotorChangeBattLOG WITH(NOLOCK) WHERE order_number=@OrderNo AND EventCD=2 ORDER BY MKTime DESC;

					--換電次數，每邊換電>80%就+5
					SELECT @CHANGETIME=SUM(CASE WHEN B.LBA-A.LBA > 80 AND B.RBA-A.RBA > 80 THEN 2
						WHEN B.LBA-A.LBA > 80 OR B.RBA-A.RBA > 80 THEN 1
						ELSE 0 END)
					FROM #TB_MotorChangeBattLOG A with(nolock)
					JOIN #TB_MotorChangeBattLOG B with(nolock) ON A.SEQNO=B.REFNO AND A.EventCD in (3,4);
		
					SET @RewardGift = 0;

					--統計結果
					--取還車電量差40% +10
					IF (@RSOC_E-@RSOC_S) >= 40
					BEGIN
						SET @RewardGift = @RewardGift + 10;
					END

					--還車電量>85% +5
					IF @RSOC_E >= 85 AND ISNULL(@CHANGETIME,0) > 0
					BEGIN
						SET @RewardGift = @RewardGift + 5;
					END

					--換電次數*5
					SET @ChgGift  = ISNULL(@CHANGETIME,0) * 5;
					--PRINT ISNULL(@CHANGETIME,0)
					--PRINT @ChgGift
					--PRINT @RewardGift

					SET @Reward = @ChgGift+@RewardGift;

					INSERT INTO TB_MotorChangeBattHis(order_number, ChgTimes, RSOC_S, RSOC_E, ChgGift, RewardGift, TotalGift, MKTime)
					values(@OrderNo, ISNULL(@CHANGETIME,0), ISNULL(@RSOC_S,0), ISNULL(@RSOC_E,0), ISNULL(@ChgGift,0) , ISNULL(@RewardGift,0) , ISNULL(@Reward,0) , dbo.GET_TWDATE());

					DROP TABLE IF EXISTS #TB_MotorChangeBattLog;
				END
			END
			ELSE IF @ProjType=0	-- 同站
			BEGIN
				UPDATE TB_BookingStatusOfUser
				SET NormalRentBookingNowCount=CASE WHEN NormalRentBookingNowCount -1 < 0 THEN 0 ELSE NormalRentBookingNowCount -1 END,
					RentNowActiveType = 5,
					NowActiveOrderNum = 0,
					NormalRentBookingFinishCount = NormalRentBookingFinishCount + 1,
					UPDTime = @NowTime
				WHERE IDNO=@IDNO;
			END
			ELSE	-- 路邊汽車
			BEGIN
				UPDATE TB_BookingStatusOfUser
				SET AnyRentBookingNowCount=CASE WHEN AnyRentBookingNowCount-1 < 0 THEN 0 ELSE AnyRentBookingNowCount - 1 END,
					RentNowActiveType=5,
					NowActiveOrderNum=0,
					AnyRentBookingFinishCount=AnyRentBookingFinishCount+1,
					UPDTime=@NowTime
				WHERE IDNO=@IDNO;
			END

			--更新車輛
			UPDATE TB_Car
			SET NowOrderNo=0,
				LastOrderNo=@OrderNo,
				available=1,
				UPDTime=@NowTime
			WHERE CarNo=@CarNo;

			--20210122 ADD BY ADAM REASON.更新未清潔次數
			UPDATE TB_CarInfo 
			SET RentCount=RentCount+1,
				UncleanCount=UncleanCount+1,
				UPDTime=@NowTime
			WHERE CarNo=@CarNo;

			--寫入歷程
			SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status
			FROM TB_OrderMain WITH(NOLOCK)
			WHERE order_number=@OrderNo;

			SET @Descript=N'後台強還，完成還車';
			SET @Descript=CONCAT('使用者',@UserID,'操作：',@Descript);

			INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
			VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);

			--寫入一次性開門的deadline
			--INSERT INTO TB_OpenDoor(OrderNo,DeadLine)VALUES(@OrderNo,DATEADD(MINUTE,15,@NowTime));

			-- 取得付款方式
			SELECT @PayMode=PayMode FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

			-- 20211229 UPD BY YEH REASON:信用卡類別用付款方式區分
			IF @PayMode = 0	-- 台新信用卡
			BEGIN
				SET @CardType = 1;
			END
			ELSE IF @PayMode = 4	-- 和泰PAY
			BEGIN
				SET @CardType = 0;
			END
			ELSE IF @PayMode = 1	-- 錢包
			BEGIN
				SET @CardType = 2;
			END

			-- 取得尾款金額，(>0:補授權 ; =0:僅作關帳 ; <0:調整授權金
			SELECT @DiffAmount=DiffAmount FROM TB_OrderExtinfo WITH(NOLOCK) WHERE order_number=@OrderNo;

			DECLARE @Count int = 0;
			SELECT @Count = COUNT(*) FROM @TradeClose;

			IF @Count > 0
			BEGIN
				-- 更新關帳檔
				UPDATE TB_TradeClose
				SET ChkClose=A.ChkClose,
					CloseAmout=A.CloseAmout,
					U_PRGID=@FunName,
					U_USERID=@IDNO,
					U_SYSDT=@NowTime
				FROM @TradeClose A
				INNER JOIN TB_TradeClose B ON B.CloseID=A.CloseID
				WHERE A.CardType <> 2;	-- 20220223 UPD BY YEH REASON:只有信用卡才更新

				-- 有退款金額寫TB_OrderAuthReturn
				-- 退款要拆成信用卡(台新/中信)/錢包兩種來看，信用卡整筆退款才寫入TB_OrderAuthReturn，錢包則是有退款就寫入TB_OrderAuthReturn

				-- 有退款金額寫TB_OrderAuthReturn
				-- 20211217 UPD BY YEH REASON:增加信用卡類別(0:和泰;1:台新)
				INSERT INTO TB_OrderAuthReturn (A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,order_number,returnAmt,IDNO,AuthFlg,AuthCode,AuthMessage,transaction_no,ori_transaction_no,CardType)
				SELECT @FunName,@IDNO,@NowTime,@FunName,@IDNO,@NowTime,@OrderNo,A.RefundAmount,@IDNO,0,'','','',B.MerchantTradeNo,B.CardType
				FROM @TradeClose A
				INNER JOIN TB_TradeClose B ON B.CloseID=A.CloseID
				WHERE A.RefundAmount > 0
				AND A.ChkClose = 2		-- 20211202 UPD BY YEH REASON:整筆退才寫TB_OrderAuthReturn
				AND A.CardType <> 2;	-- 20220223 UPD BY YEH REASON:信用卡

				-- 20220223 UPD BY YEH REASON:增加錢包退款
				INSERT INTO TB_OrderAuthReturn (A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT,order_number,returnAmt,IDNO,AuthFlg,AuthCode,AuthMessage,transaction_no,ori_transaction_no,CardType)
				SELECT @FunName,@IDNO,@NowTime,@FunName,@IDNO,@NowTime,@OrderNo,A.RefundAmount,@IDNO,0,'','','',B.StoreTransId,A.CardType
				FROM @TradeClose A
				INNER JOIN TB_WalletHistory B ON B.HistoryID=A.CloseID
				WHERE A.RefundAmount > 0
				AND A.CardType = 2;
			END

			--準備傳送合約
			IF NOT EXISTS(SELECT IRENTORDNO FROM TB_ReturnCarControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo)
			BEGIN
				IF (@NoPreAuth = 1 OR @final_price = 0 OR @DiffAmount <= 0)
				BEGIN
					DECLARE @TradeCloseAmount INT = 0;	-- 關帳檔總金額
					DECLARE @WalletAmount INT = 0;		-- 錢包扣款總金額
					DECLARE @WalletReturn INT = 0;		-- 錢包退款總金額

					SELECT @TradeCloseAmount=ISNULL(SUM(CloseAmout),0) FROM TB_TradeClose WITH(NOLOCK) WHERE OrderNo=@OrderNo AND ChkClose=1;
					SELECT @WalletAmount=ISNULL(SUM(Amount),0) FROM TB_WalletHistory WITH(NOLOCK) WHERE OrderNo=@OrderNo AND Mode=0;
					SELECT @WalletReturn=ISNULL(SUM(returnAmt),0) FROM TB_OrderAuthReturn WITH(NOLOCK) WHERE order_number=@OrderNo AND CardType=2;

					-- 20220216 UPD BY YEH REASON:增加更新A_PRGID,U_PRGID
					INSERT INTO TB_ReturnCarControl
					(
						PROCD, ORDNO, CNTRNO, IRENTORDNO, CUSTID, CUSTNM, BIRTH, 
						CUSTTYPE, ODCUSTID, CARTYPE, CARNO, TSEQNO, GIVEDATE, 
						GIVETIME, RENTDAYS, GIVEKM, OUTBRNHCD, RNTDATE, RNTTIME, 
						RNTKM, INBRNHCD, RPRICE, RINSU, DISRATE, OVERHOURS, 
						OVERAMT2, RNTAMT, 
						RENTAMT, 
						LOSSAMT2, PROJID, REMARK, 
						INVKIND, UNIMNO, INVTITLE, INVADDR, GIFT, GIFT_MOTO, CARDNO,
						PAYAMT, 
						AUTHCODE, isRetry, RetryTimes, eTag, 
						CARRIERID, NPOBAN, NOCAMT, PARKINGAMT2, 
						MKTime, UPDTime, A_PRGID, U_PRGID
					)
					SELECT PROCD='A', C.ORDNO, C.CNTRNO, A.order_number, C.CUSTID, C.CUSTNM, C.BIRTH,
						C.CUSTTYPE, C.ODCUSTID, C.CARTYPE, CASRNO=A.CarNo, C.TSEQNO, C.GIVEDATE,
						C.GIVETIME, dbo.FN_CalRntdays(B.final_start_time,B.final_stop_time), CAST(B.start_mile AS INT), C.OUTBRNHCD, CONVERT(VARCHAR,B.final_stop_time,112), REPLACE(CONVERT(VARCHAR(5),B.final_stop_time,108),':',''),
						CAST(B.end_mile AS INT), C.INBRNHCD, C.RPRICE, C.RINSU, C.DISRATE, B.fine_interval/600,
						B.fine_price, RNTAMT=(B.fine_price+B.mileage_price),
						RENTAMT=CASE WHEN (pure_price- CASE WHEN TransDiscount>0 THEN TransDiscount ELSE 0 END) > 0 THEN (pure_price- CASE WHEN TransDiscount>0 THEN TransDiscount ELSE 0 END) ELSE 0 END,	--20201229 租金要扣掉轉乘優惠
						mileage_price, A.ProjID, C.REMARK,
						A.bill_option, A.unified_business_no, '', A.invoiceAddress, B.gift_point, B.gift_motor_point, CARDNO='', 
						PAYAMT = (@TradeCloseAmount + @WalletAmount - @WalletReturn),		-- 20220223 UPD BY YEH REASON:PAYAMT = 關帳檔總金額 + 錢包扣款總金額 - 錢包退款總金額
						AUTHCODE='', isRetry=1, RetryTimes=0, B.Etag,
						C.CARRIERID, C.NPOBAN, B.Insurance_price, ISNULL(B.parkingFee,0) AS PARKINGAMT2,	--20210506;UPD BY YEH REASON.PARKINGAMT2改抓OrderDetail的parkingFee
						@NowTime, @NowTime, @FunName, @FunName
					FROM TB_OrderMain A WITH(NOLOCK)
					INNER JOIN TB_OrderDetail B WITH(NOLOCK) ON A.order_number=B.order_number
					INNER JOIN TB_lendCarControl C WITH(NOLOCK) ON A.order_number=C.IRENTORDNO
					WHERE A.order_number=@OrderNo;

					-- 20210707;ADD BY YEH REASON:計算徽章成就
					EXEC usp_CalOrderMedal @OrderNo,'A',@FunName,@LogID,'','','','';

					-- 20210707 ADD BY YEH REASON:計算會員積分
					EXEC usp_CalOrderScore @OrderNo,'C',0,0,@FunName,@LogID,'','','','';
				END
				ELSE
				BEGIN
					-- 預授權上線後，TB_OrderAuth會有多筆資料，因此用AuthType=7判斷是否寫入
					IF NOT EXISTS (SELECT order_number FROM TB_OrderAuth WITH(NOLOCK) WHERE order_number=@OrderNo AND AuthType=7)
					BEGIN
						INSERT INTO TB_OrderAuth (A_PRGID, A_USERID, A_SYSDT, U_PRGID, U_USERID, U_SYSDT, order_number, IDNO , final_price, AuthFlg, AuthMessage, CardType, AuthType, AutoClose)
						SELECT @FunName, @IDNO, @NowTime, @FunName, @IDNO, @NowTime, order_number, IDNO, @DiffAmount, 0, '', @CardType, 7, 1
						FROM VW_GetOrderData WITH(NOLOCK) WHERE IDNO=@IDNO AND order_number=@OrderNo;
					END
				END
			END

			-- 回傳結果：換電獎勵
			SELECT @Reward Reward;

			COMMIT TRAN
		END

		--寫入錯誤訊息
		IF @Error=1
		BEGIN
			INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
			VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
		END
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		SET @Error=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

		SET @IsSystem=1;
		SET @ErrorType=4;
		INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	--輸出系統訊息
	SELECT @ErrorCode ErrorCode, @ErrorMsg ErrorMsg, @SQLExceptionCode SQLExceptionCode, @SQLExceptionMsg SQLExceptionMsg, @Error Error
END
GO