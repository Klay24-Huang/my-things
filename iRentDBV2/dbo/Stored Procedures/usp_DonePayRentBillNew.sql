/****** Object:  StoredProcedure [dbo].[usp_DonePayRentBillNew]    Script Date: 2021/2/18 下午 04:10:52 ******/

/****************************************************************
** Name: [dbo].[usp_DonePayRentBill]
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
** EXEC @Error=[dbo].[usp_DonePayRentBill]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/6 下午 05:07:23 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/6 下午 05:07:23    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_DonePayRentBillNew]
	@IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@transaction_no         NVARCHAR(100)         , --金流交易序號，免付費使用Free
	@Token                  VARCHAR(1024)         ,
	@LogID                  BIGINT                ,
	@Reward					INT				OUTPUT,	--換電獎勵
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
DECLARE @ParkingSpace NVARCHAR(128);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_DonePayRentBillNew';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【完成付款金流】NEW';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token=ISNULL (@Token,'');
SET @ParkingSpace='';

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

	IF @Error=0
	BEGIN
		BEGIN TRAN

		SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status,@CarNo=CarNo,@ProjType=ProjType
		FROM TB_OrderMain WITH(NOLOCK)
		WHERE order_number=@OrderNo;

		SELECT @hasData=COUNT(1) FROM TB_ParkingSpaceTmp WITH(NOLOCK) WHERE OrderNo=@OrderNo;
		IF @hasData>0
		BEGIN
			INSERT INTO [dbo].[TB_ParkingSpace]([OrderNo],[ParkingImage],[ParkingSpace])
			SELECT [OrderNo],[ParkingImage],[ParkingSpace] FROM TB_ParkingSpaceTmp WITH(NOLOCK) WHERE OrderNo=@OrderNo;

			DELETE FROM TB_ParkingSpaceTmp WHERE OrderNo=@OrderNo;
			--取出停車位置
			--SELECT @ParkingSpace=ISNULL([ParkingSpace],'') FROM [TB_ParkingSpace] WITH(NOLOCK) WHERE OrderNo=@OrderNo AND SEQNO=1;
		END

		--寫入歷程
		INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
		VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);
					
		--更新訂單主檔
		UPDATE TB_OrderMain
		SET booking_status=5,
			car_mgt_status=16
		WHERE order_number=@OrderNo;
					
		--更新訂單明細
		UPDATE TB_OrderDetail
		SET transaction_no=@transaction_no,
			trade_status=1,
			[already_return_car]=1,
			[already_payment]=1
		WHERE order_number=@OrderNo;

		--20201010 ADD BY ADAM REASON.還車改為只針對個人訂單狀態去個別處理
		--更新個人訂單控制
		IF @ProjType=4
		BEGIN
			UPDATE [TB_BookingStatusOfUser]
			SET [MotorRentBookingNowCount]=[MotorRentBookingNowCount]-1,
				RentNowActiveType=5,NowActiveOrderNum=0,
				[MotorRentBookingFinishCount]=[MotorRentBookingFinishCount]+1,
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
			END
		END
		ELSE IF @ProjType=0
		BEGIN
			UPDATE [TB_BookingStatusOfUser]
			SET [NormalRentBookingNowCount]=[NormalRentBookingNowCount]-1,
				RentNowActiveType=5,NowActiveOrderNum=0,
				[NormalRentBookingFinishCount]=[NormalRentBookingFinishCount]+1,
				UPDTime=@NowTime
			WHERE IDNO=@IDNO;
		END
		ELSE
		BEGIN
			UPDATE [TB_BookingStatusOfUser]
			SET [AnyRentBookingNowCount]=[AnyRentBookingNowCount]-1,
				RentNowActiveType=5,NowActiveOrderNum=0,
				[AnyRentBookingFinishCount]=[AnyRentBookingFinishCount]+1,
				UPDTime=@NowTime
			WHERE IDNO=@IDNO;
		END

		--更新車輛
		UPDATE TB_Car
		SET [NowOrderNo]=0,
			[LastOrderNo]=@OrderNo,
			available=1,
			UPDTime=@NowTime
		WHERE CarNo=@CarNo;

		--20210122 ADD BY ADAM REASON.更新未清潔次數
		UPDATE TB_CarInfo 
		SET RentCount=RentCount+1,
		UncleanCount=UncleanCount+1,
		UPDTime=@NowTime
		--last_Opt=@UserID
		WHERE CarNo=@CarNo;

		--寫入歷程
		SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status
		FROM TB_OrderMain WITH(NOLOCK)
		WHERE order_number=@OrderNo;

		SET @Descript=N'完成還車';
		INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
		VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);

		--寫入一次性開門的deadline
		INSERT INTO TB_OpenDoor(OrderNo,DeadLine)VALUES(@OrderNo,DATEADD(MINUTE,15,@NowTime));

		--準備傳送合約
		IF NOT EXISTS(SELECT IRENTORDNO FROM TB_ReturnCarControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo)
		BEGIN
			IF EXISTS (SELECT final_price FROM VW_GetOrderData WITH(NOLOCK) WHERE IDNO=@IDNO AND order_number=@OrderNo AND cancel_status=0 AND final_price=0)
			BEGIN
				INSERT INTO TB_ReturnCarControl
				(
					[PROCD], [ORDNO], [CNTRNO], [IRENTORDNO], [CUSTID], [CUSTNM], [BIRTH], 
					[CUSTTYPE], [ODCUSTID], [CARTYPE], [CARNO], [TSEQNO], [GIVEDATE], 
					[GIVETIME], [RENTDAYS], [GIVEKM], [OUTBRNHCD], [RNTDATE], [RNTTIME], 
					[RNTKM], [INBRNHCD], [RPRICE], [RINSU], [DISRATE], [OVERHOURS], 
					[OVERAMT2], [RNTAMT], [RENTAMT], [LOSSAMT2], [PROJID], [REMARK], 
					[INVKIND], [UNIMNO], [INVTITLE], [INVADDR], [GIFT], [GIFT_MOTO], 
					[CARDNO], [PAYAMT], [AUTHCODE], [isRetry], [RetryTimes], [eTag], 
					[CARRIERID], [NPOBAN], [NOCAMT], [PARKINGAMT2], [MKTime], [UPDTime]
				)
				SELECT PROCD='A',C.ORDNO,C.CNTRNO,A.order_number,C.CUSTID,C.CUSTNM, C.BIRTH,
					C.CUSTTYPE,C.ODCUSTID,C.CARTYPE,CASRNO=A.CarNo,C.TSEQNO,C.GIVEDATE,
					C.GIVETIME,dbo.FN_CalRntdays(B.final_start_time,B.final_stop_time),CAST(B.start_mile AS INT),C.OUTBRNHCD,CONVERT(VARCHAR,B.final_stop_time,112),REPLACE(CONVERT(VARCHAR(5),B.final_stop_time,108),':',''),
					CAST(B.end_mile AS INT),C.INBRNHCD,C.RPRICE,C.RINSU,C.DISRATE,B.fine_interval/600,
					fine_price,RNTAMT=(B.fine_price+B.mileage_price),RENTAMT=CASE WHEN (pure_price- TransDiscount) > 0 THEN (pure_price- TransDiscount) ELSE 0 END,mileage_price,A.ProjID,C.REMARK,	--20201229 租金要扣掉轉乘優惠
					A.bill_option,A.unified_business_no,'',A.invoiceAddress,B.gift_point,B.gift_motor_point,
					CARDNO=ISNULL(Trade.CardNumber,''),PAYAMT=ISNULL(Trade.AUTHAMT,0),AUTHCODE=IIF(ISNULL(Trade.AuthIdResp,0)=0,'',CONVERT(VARCHAR(20),Trade.AuthIdResp)),isRetry=1,RetryTimes=0,B.Etag,
					C.CARRIERID,C.NPOBAN,B.Insurance_price,ISNULL(Machi.Amount,0) AS PARKINGAMT2,@NowTime,@NowTime
				FROM TB_OrderMain A WITH(NOLOCK)
				JOIN TB_OrderDetail B WITH(NOLOCK) ON A.order_number=B.order_number
				JOIN TB_lendCarControl C WITH(NOLOCK) ON A.order_number=C.IRENTORDNO
				LEFT JOIN TB_Trade AS Trade WITH(NOLOCK) ON Trade.MerchantTradeNo =B.transaction_no AND Trade.CreditType=0 AND IsSuccess=1 AND Trade.OrderNo=B.order_number
				LEFT JOIN TB_OrderParkingFeeByMachi AS Machi WITH(NOLOCK) ON Machi.OrderNo=B.order_number
				WHERE A.order_number=@OrderNo
			END
			ELSE
			BEGIN
				IF NOT EXISTS (SELECT order_number FROM TB_OrderAuth WITH(NOLOCK) WHERE order_number=@OrderNo)
				BEGIN
					INSERT INTO TB_OrderAuth
					(A_PRGID, A_USERID, A_SYSDT, U_PRGID, U_USERID, U_SYSDT, order_number, final_price, 
								AuthFlg, AuthMessage,IDNO)
					SELECT A_PRGID='DonePayRentBill'
						, A_USERID=@IDNO
						, A_SYSDT=@NowTime
						, U_PRGID='DonePayRentBill'
						, U_USERID=@IDNO
						, U_SYSDT=@NowTime
						, order_number
						, final_price
						, AuthFlg=0
						, AuthMessage =''
						, IDNO
					FROM VW_GetOrderData WITH(NOLOCK) WHERE IDNO=@IDNO AND order_number=@OrderNo 
				END
			END
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_DonePayRentBill';
GO

