/****************************************************************
** Name: [dbo].[usp_BE_ContactFinish]
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
** EXEC @Error=[dbo].[usp_BE_ContactFinish]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/30 上午 05:54:10 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/30 上午 05:54:10    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_ContactFinish]
    @IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@UserID                 NVARCHAR(10)          ,
	@transaction_no         NVARCHAR(100)         , --金流交易序號，免付費使用Free
	@ReturnDate             DATETIME              , --強還時間
	--20201130 UPD BY JERRY 增加發票寄送方式相關欄位
	@bill_option            INT					  ,	--發票寄送方式：1:捐贈;2:email;3:二聯;4:三聯;5:手機條碼;6:自然人憑證
	@unified_business_no    VARCHAR(8)			  ,	--統一編號
	@CARRIERID              VARCHAR(20)           ,	--手機條碼載具,自然人憑證載具
	@NPOBAN                 VARCHAR(20)           ,	--愛心碼
	@ParkingSpace           NVARCHAR(256)         ,	--停車格
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
--DECLARE @ParkingSpace NVARCHAR(256);
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_ContactFinish';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'後台強還，設定狀態為已還車';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @UserID=ISNULL (@UserID,'');
SET @ParkingSpace='';

BEGIN TRY
	IF @UserID='' OR @IDNO=''  OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
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
			SELECT [OrderNo],[ParkingImage],
			--20201214 改由前端客服維護
				[ParkingSpace]=CASE WHEN @ParkingSpace='' THEN [ParkingSpace] ELSE @ParkingSpace END
			FROM TB_ParkingSpaceTmp WITH(NOLOCK) WHERE OrderNo=@OrderNo;

			DELETE FROM TB_ParkingSpaceTmp WHERE OrderNo=@OrderNo;
			--SELECT @ParkingSpace=ISNULL([ParkingSpace],'') FROM [TB_ParkingSpace] WITH(NOLOCK) WHERE OrderNo=@OrderNo;
		END

		--寫入歷程
		INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
		VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);
		
		--20210220 ADD BY ADAM REASON.發票資訊需要整理過在寫入
		--資料梳理
		IF @bill_option=1	--捐贈
		BEGIN
			SET @unified_business_no=''
			SET @CARRIERID=''
		END
		IF @bill_option=2 OR @bill_option=3		--EMAIL or 二聯
		BEGIN
			SET @NPOBAN=''
			SET @unified_business_no=''
			SET @CARRIERID=''
		END
		IF @bill_option=4	--三聯
		BEGIN
			SET @NPOBAN=''
			SET @CARRIERID=''
		END	
		IF @bill_option=5 OR @bill_option=6		--載具 or 自然人憑證
		BEGIN
			SET @NPOBAN=''
			SET @unified_business_no=''
		END
		

		--更新訂單主檔
		UPDATE TB_OrderMain
		SET booking_status=5,
			car_mgt_status=16,
			modified_status=2,
			--20201130 UPD BY JERRY 增加發票寄送方式相關欄位
			bill_option=@bill_option,
			unified_business_no=@unified_business_no,
			CARRIERID=@CARRIERID,
			NPOBAN=@NPOBAN
		WHERE order_number=@OrderNo;
					
		--更新訂單明細
		IF @transaction_no='Free'
		BEGIN
			UPDATE A
			SET transaction_no=@transaction_no,
				trade_status=1,
				[already_return_car]=1,
				[already_payment]=1,
				final_stop_time=@ReturnDate,
				parkingSpace=@ParkingSpace
				--end_mile=C.Millage	--20210224 ADD BY ADAM REASON.強還先不壓里程
			FROM TB_OrderDetail A
			LEFT JOIN TB_OrderMain B ON A.order_number=B.order_number
			--LEFT JOIN TB_CarStatus C ON B.CarNo=C.CarNo
			WHERE A.order_number=@OrderNo;
		END
		ELSE
		BEGIN
			UPDATE A
			--SET [already_return_car]=1,[already_payment]=1
			--20201110 UPD BY JERRY 不管何種狀態都要更新final_stop_time
			SET [already_return_car]=1,
				[already_payment]=1,
				final_stop_time=@ReturnDate,
				parkingSpace=@ParkingSpace
				--end_mile=C.Millage	--20210224 ADD BY ADAM REASON.強還先不壓里程
			FROM TB_OrderDetail A
			LEFT JOIN TB_OrderMain B ON A.order_number=B.order_number
			--LEFT JOIN TB_CarStatus C ON B.CarNo=C.CarNo
			WHERE A.order_number=@OrderNo;
		END
		--20201010 ADD BY ADAM REASON.還車改為只針對個人訂單狀態去個別處理
		--更新個人訂單控制
		IF @ProjType=4
		BEGIN
			UPDATE [TB_BookingStatusOfUser]
			SET [MotorRentBookingNowCount]=CASE WHEN [MotorRentBookingNowCount]-1<0 THEN 0 ELSE [MotorRentBookingNowCount]-1 END,
				RentNowActiveType=5,
				NowActiveOrderNum=0,
				[MotorRentBookingFinishCount]=[MotorRentBookingFinishCount]+1,
				UPDTime=@NowTime
			WHERE IDNO=@IDNO;

			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM [TB_OrderDataByMotor] WHERE OrderNo=@OrderNo;
			IF @hasData=0
			BEGIN
				INSERT INTO TB_OrderDataByMotor(OrderNo,R_lat,R_lon,R_LBA,R_RBA,R_MBA,R_TBA)
				SELECT @OrderNo,Latitude,Longitude,deviceLBA,deviceRBA,deviceMBA,device3TBA 
				FROM TB_CarStatus WITH(NOLOCK) WHERE CarNo=@CarNo;
			END
			ELSE
			BEGIN
				UPDATE TB_OrderDataByMotor
				SET R_LBA=deviceLBA,
					R_RBA=deviceRBA,
					R_TBA=device3TBA,
					R_MBA=deviceMBA,
					R_lon=Longitude,
					R_lat=Latitude,
					UPDTime=@NowTime
				FROM TB_CarStatus 
				WHERE OrderNo=@OrderNo;
			END		
		END
		ELSE IF @ProjType=0
		BEGIN
			UPDATE [TB_BookingStatusOfUser]
			SET [NormalRentBookingNowCount]=CASE WHEN [NormalRentBookingNowCount]-1<0 THEN 0 ELSE [NormalRentBookingNowCount]-1 END,
				RentNowActiveType=5,NowActiveOrderNum=0,
				[NormalRentBookingFinishCount]=[NormalRentBookingFinishCount]+1,
				UPDTime=@NowTime
			WHERE IDNO=@IDNO;
		END
		ELSE
		BEGIN
			UPDATE [TB_BookingStatusOfUser]
			SET [AnyRentBookingNowCount]=CASE WHEN [AnyRentBookingNowCount]-1<0 THEN 0 ELSE [AnyRentBookingNowCount]-1 END,
				RentNowActiveType=5,
				NowActiveOrderNum=0,
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

		--更新車輛出租次數與未清潔次數
		UPDATE TB_CarInfo 
		SET RentCount=RentCount+1,
			UncleanCount=UncleanCount+1,
			UPDTime=@NowTime,
			last_Opt=@UserID
	    WHERE CarNo=@CarNo;

		--寫入歷程
		SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status
		FROM TB_OrderMain WITH(NOLOCK)
		WHERE order_number=@OrderNo;

		SET @Descript=N'後台強還，完成還車';
		INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
		VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);

		--寫入一次性開門的deadline
		--INSERT INTO TB_OpenDoor(OrderNo,DeadLine)VALUES(@OrderNo,DATEADD(MINUTE,15,@NowTime));

		--準備傳送合約
		IF NOT EXISTS(SELECT IRENTORDNO FROM TB_ReturnCarControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo)
		BEGIN
			INSERT INTO TB_ReturnCarControl
			(
				[PROCD], [ORDNO], [CNTRNO], [IRENTORDNO], [CUSTID], [CUSTNM], [BIRTH], 
				[CUSTTYPE], [ODCUSTID], [CARTYPE], [CARNO], [TSEQNO], [GIVEDATE], 
				[GIVETIME], [RENTDAYS], [GIVEKM], [OUTBRNHCD], [RNTDATE], [RNTTIME], 
				[RNTKM], [INBRNHCD], [RPRICE], [RINSU], [DISRATE], [OVERHOURS], 
				[OVERAMT2], [RNTAMT], 
				[RENTAMT], 
				[LOSSAMT2], [PROJID], [REMARK], 
				[INVKIND], [UNIMNO], [INVTITLE], [INVADDR], [GIFT], [GIFT_MOTO], 
				[CARDNO], [PAYAMT], [AUTHCODE], [isRetry], [RetryTimes], [eTag], 
				[CARRIERID], [NPOBAN], [NOCAMT], [PARKINGAMT2], [MKTime], [UPDTime]
			)
			SELECT PROCD='A',C.ORDNO,C.CNTRNO,A.order_number,C.CUSTID,C.CUSTNM, C.BIRTH,
				C.CUSTTYPE,C.ODCUSTID,C.CARTYPE,CASRNO=A.CarNo,C.TSEQNO,C.GIVEDATE,
				C.GIVETIME,dbo.FN_CalRntdays(B.final_start_time,B.final_stop_time),CAST(B.start_mile AS INT),C.OUTBRNHCD,CONVERT(VARCHAR,B.final_stop_time,112),REPLACE(CONVERT(VARCHAR(5),B.final_stop_time,108),':',''),
				CAST(B.end_mile AS INT),C.INBRNHCD,C.RPRICE,C.RINSU,C.DISRATE,B.fine_interval/600,
				fine_price,RNTAMT=(B.fine_price+B.mileage_price),
				RENTAMT=CASE WHEN (pure_price- CASE WHEN TransDiscount>0 THEN TransDiscount ELSE 0 END) > 0 THEN (pure_price- CASE WHEN TransDiscount>0 THEN TransDiscount ELSE 0 END) ELSE 0 END,
				mileage_price,A.ProjID,C.REMARK,
				A.bill_option,A.unified_business_no,'',A.invoiceAddress,B.gift_point,B.gift_motor_point,
				CARDNO=ISNULL(Trade.CardNumber,''),PAYAMT=ISNULL(Trade.AUTHAMT,0),AUTHCODE=IIF(ISNULL(Trade.AuthIdResp,0)=0,'',CONVERT(VARCHAR(20),Trade.AuthIdResp)),isRetry=1,RetryTimes=0,B.Etag,
				A.CARRIERID,A.NPOBAN,B.Insurance_price,ISNULL(B.parkingFee,0) AS PARKINGAMT2,@NowTime,@NowTime	--20210506;UPD BY YEH REASON.PARKINGAMT2改抓OrderDetail的parkingFee
			FROM TB_OrderMain A WITH(NOLOCK)
			JOIN TB_OrderDetail B WITH(NOLOCK) ON A.order_number=B.order_number
			JOIN TB_lendCarControl C WITH(NOLOCK) ON A.order_number=C.IRENTORDNO
			LEFT JOIN TB_Trade AS Trade WITH(NOLOCK) ON Trade.MerchantTradeNo =B.transaction_no AND Trade.CreditType=0 AND IsSuccess=1 AND Trade.OrderNo=B.order_number
			WHERE A.order_number=@OrderNo
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ContactFinish';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ContactFinish';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款完成', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ContactFinish';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ContactFinish';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ContactFinish';