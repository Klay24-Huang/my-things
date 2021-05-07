/****************************************************************
** Name: [dbo].[usp_BE_BookingCancel]
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
** EXEC @Error=[dbo].[usp_BE_BookingCancel]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/22 下午 04:01:14 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/22 下午 04:01:14    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_BookingCancelNew]
	@OrderNo                BIGINT                ,
	@UserID                 NVARCHAR(10)          ,
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
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;
DECLARE @tmpOrderNum TINYINT;
DECLARE @IDNO	VARCHAR(10)
DECLARE @TradeStatus INT;
DECLARE @TradeNo VARCHAR(128);
--目前各類型預約數
DECLARE @NormalRentBookingNowCount      TINYINT;
DECLARE @AnyRentBookingNowCount			TINYINT;
DECLARE @MotorRentBookingNowCount		TINYINT;
DECLARE @RentNowActiveType              TINYINT;
DECLARE @NowActiveOrderNum				BIGINT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_BookingCancelNew';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript='';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @tmpOrderNum=0;
SET @OrderNo=ISNULL (@OrderNo,0);
SET @UserID=ISNULL(@UserID,'');
SET @TradeNo='';
SET @TradeStatus=0;
BEGIN TRY
	IF @UserID=''  OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	--0.再次檢核token
	IF @Error=0
	BEGIN
		SET @Descript=CONCAT(N'後台操作【作廢訂單】，操作者【',@UserID,'】');
		BEGIN TRAN
	
		SELECT @tmpOrderNum=COUNT(order_number) 
		FROM TB_OrderMain WITH(NOLOCK) 
		WHERE order_number=@OrderNo AND (car_mgt_status>=4 AND car_mgt_status<15 AND cancel_status=0 );
		  
		IF @tmpOrderNum=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR129'
			ROLLBACK TRAN;
		END
		ELSE
		BEGIN
			IF @Error=0
			BEGIN
				SELECT @IDNO=IDNO,@ProjType=ProjType FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo

				SELECT @NormalRentBookingNowCount=ISNULL([NormalRentBookingNowCount],0)
					,@AnyRentBookingNowCount=ISNULL([AnyRentBookingNowCount],0)
					,@MotorRentBookingNowCount=ISNULL([MotorRentBookingNowCount],0)
					,@RentNowActiveType=ISNULL(RentNowActiveType,5)
					,@NowActiveOrderNum=ISNULL(NowActiveOrderNum,0)
				FROM [dbo].[TB_BookingStatusOfUser] WITH(NOLOCK)
				WHERE IDNO=@IDNO;

				SELECT @TradeNo=ISNULL(transaction_no,''),@TradeStatus=ISNULL(trade_status,0) FROM TB_OrderDetail WITH(NOLOCK) WHERE order_number=@OrderNo;
				IF @TradeNo<>'' AND @TradeStatus=1
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR774'
					ROLLBACK TRAN;
				END
			END
			IF @Error=0
			BEGIN
				UPDATE TB_OrderMain SET cancel_status=5,car_mgt_status=16,booking_status=5 WHERE  order_number=@OrderNo;

				IF @@ROWCOUNT=1
				BEGIN
					COMMIT TRAN;
					SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status,@CarNo=CarNo
					FROM TB_OrderMain WITH(NOLOCK)
					WHERE order_number=@OrderNo;

					INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
					VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);

					--  UPDATE Car_201607 SET available=1 WHERE car_id=@CarNo;
					-- SELECT @RstationID=return_place,@ProjID=premium FROM TB_BookingMain_201609 WHERE order_number=@OrderNum;

					UPDATE TB_CarInfo 
					SET RentCount=RentCount-1,
						UPDTime=@NowTime 
					WHERE CarNo=@CarNo AND RentCount>0;

					--更新總表
					IF @ProjType=0
					BEGIN
						UPDATE [TB_BookingStatusOfUser] 
						SET [NormalRentBookingNowCount]=[NormalRentBookingNowCount]-1,
							NormalRentBookingCancelCount=NormalRentBookingCancelCount+1,
							[RentNowActiveType]=5,[NowActiveOrderNum]=0,
							UPDTime=@NowTime
						WHERE IDNO=@IDNO AND NormalRentBookingNowCount>0;
					END
					ELSE IF @ProjType=3
					BEGIN
						UPDATE [TB_BookingStatusOfUser] 
						SET [AnyRentBookingNowCount]=0,
							AnyRentBookingCancelCount=AnyRentBookingCancelCount+1,
							[RentNowActiveType]=5,[NowActiveOrderNum]=0,
							UPDTime=@NowTime
						WHERE IDNO=@IDNO AND [AnyRentBookingNowCount]>0;
					END
					ELSE IF @ProjType=4
					BEGIN
						UPDATE [TB_BookingStatusOfUser] 
						SET MotorRentBookingNowCount=0,
							MotorRentBookingCancelCount=MotorRentBookingCancelCount+1,
							[RentNowActiveType]=5,[NowActiveOrderNum]=0,
							UPDTime=@NowTime 
						WHERE IDNO=@IDNO AND MotorRentBookingNowCount>0;
					END
				END
				ELSE
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR132'
					ROLLBACK TRAN;
				END
			END
			IF @Error=0
			BEGIN
				IF NOT EXISTS(SELECT IRENTORDNO FROM TB_ReturnCarControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo)
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
					SELECT PROCD='F',C.ORDNO,C.CNTRNO,A.order_number,C.CUSTID,C.CUSTNM, C.BIRTH,
						C.CUSTTYPE,C.ODCUSTID,C.CARTYPE,CASRNO=A.CarNo,C.TSEQNO,C.GIVEDATE,
						C.GIVETIME,dbo.FN_CalRntdays(B.final_start_time,B.final_stop_time),CAST(B.start_mile AS INT),C.OUTBRNHCD,CONVERT(VARCHAR,B.final_stop_time,112),REPLACE(CONVERT(VARCHAR(5),B.final_stop_time,108),':',''),
						CAST(B.end_mile AS INT),C.INBRNHCD,C.RPRICE,C.RINSU,C.DISRATE,B.fine_interval/600,
						fine_price,RNTAMT=(B.fine_price+B.mileage_price),pure_price,mileage_price,A.ProjID,C.REMARK,
						A.bill_option,A.unified_business_no,'',A.invoiceAddress,B.gift_point,B.gift_motor_point,
						CARDNO=ISNULL(Trade.CardNumber,''),PAYAMT=ISNULL(Trade.AUTHAMT,0),AUTHCODE=IIF(ISNULL(Trade.AuthIdResp,0)=0,'',CONVERT(VARCHAR(20),Trade.AuthIdResp)),isRetry=1,RetryTimes=0,B.Etag,
						A.CARRIERID,A.NPOBAN,B.Insurance_price,ISNULL(B.parkingFee,0) AS PARKINGAMT2,@NowTime,@NowTime
					FROM TB_OrderMain A WITH(NOLOCK)
					JOIN TB_OrderDetail B WITH(NOLOCK) ON A.order_number=B.order_number
					JOIN TB_lendCarControl C WITH(NOLOCK) ON A.order_number=C.IRENTORDNO
					LEFT JOIN TB_Trade AS Trade WITH(NOLOCK) ON Trade.MerchantTradeNo =B.transaction_no AND Trade.CreditType=0 AND IsSuccess=1 AND Trade.OrderNo=B.order_number
					WHERE A.order_number=@OrderNo
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_BookingCancelNew';
GO

