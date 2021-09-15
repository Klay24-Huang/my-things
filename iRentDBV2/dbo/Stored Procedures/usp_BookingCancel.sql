/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_BookingCancel
* 系    統 : IRENT
* 程式功能 : 取消訂單
* 作    者 : ERIC
* 撰寫日期 : 20200918
* 修改日期 : 20210107 ADD BY ADAM REASON.增加春節預約取消判斷，並存入返還金額
			 20210707 ADD BY YEH REASON:計算積分
			 20210914 UPD BY YEH REASON:將共同承租人狀態是已接受/邀請中的改為已取消

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_BookingCancel]
	@IDNO                   VARCHAR(10)           ,	--帳號
	@OrderNo				BIGINT                ,	--訂單編號
	@Token                  VARCHAR(1024)         ,	--Token
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
--目前各類型預約數
DECLARE @NormalRentBookingNowCount      TINYINT;
DECLARE @AnyRentBookingNowCount			TINYINT;
DECLARE @MotorRentBookingNowCount		TINYINT;
DECLARE @RentNowActiveType              TINYINT;
DECLARE @NowActiveOrderNum				BIGINT;
--目前各類型預約數結束

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BookingCancel';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @IDNO=ISNULL(@IDNO,'');
SET @OrderNo=ISNULL(@OrderNo,0);
SET @Token=ISNULL(@Token,'');
SET @Descript=N'使用者操作【取消訂單】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @NormalRentBookingNowCount=0;
SET @AnyRentBookingNowCount=0;
SET @MotorRentBookingNowCount=0;
SET @RentNowActiveType=5;
SET @NowActiveOrderNum=0;

BEGIN TRY
	IF @Token='' OR @IDNO=''  OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900';
	END
		 
	--0.再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token with(nolock) WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token with(nolock) WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END

	IF @Error=0
	BEGIN
		SELECT @NormalRentBookingNowCount=ISNULL([NormalRentBookingNowCount],0)
			,@AnyRentBookingNowCount=ISNULL([AnyRentBookingNowCount],0)
			,@MotorRentBookingNowCount=ISNULL([MotorRentBookingNowCount],0)
			,@RentNowActiveType=ISNULL(RentNowActiveType,5)
			,@NowActiveOrderNum=ISNULL(NowActiveOrderNum,0)
		FROM [dbo].[TB_BookingStatusOfUser] WITH(NOLOCK)
		WHERE IDNO=@IDNO;
	END

	--20210107 ADD BY ADAM REASON.增加春節預約取消判斷，並存入返還金額
	DECLARE @PROJID VARCHAR(10)='';
	SELECT @PROJID=ProjID FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo;

	IF @Error=0 AND @PROJID='R129'
	BEGIN
		DECLARE @PAYFLG VARCHAR(1)='N'
				,@ORDAMT	INT
				,@REFUNDAMT INT
				,@DIFFDAY	INT;
		--取消預約需要判斷是否需要退錢
		IF EXISTS(SELECT order_number FROM TB_NYPayList WITH(NOLOCK) WHERE order_number=@OrderNo)
		BEGIN
			SET @PAYFLG='Y';
		END
		--計算預約租金，差異天數
		SELECT @ORDAMT=round(init_price*0.3,0)	--訂金
			,@DIFFDAY=DATEDIFF(day,booking_date,start_time)	--差異天數
		FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo;

		--計算退款金額
		SELECT @REFUNDAMT=CASE WHEN @DIFFDAY>10 THEN @ORDAMT				--大於10天全額退費
								WHEN @DIFFDAY>7 THEN ROUND(@ORDAMT*0.5,0)	--大於7天退50%
								WHEN @DIFFDAY>4 THEN ROUND(@ORDAMT*0.4,0)	--大於4天退40%
								WHEN @DIFFDAY>2 THEN ROUND(@ORDAMT*0.3,0)	--大於2天退30%
								WHEN @DIFFDAY>1 THEN ROUND(@ORDAMT*0.2,0)	--大於1天退20%
								ELSE 0 END;
		--寫入資料進去
		INSERT INTO TB_NYRefund (order_number,IDNO,DiffDay,ChkFLG,PayFLG,OrderAmt,RefundAmt,MKTime,UPDTime)
		SELECT @OrderNo,@IDNO,@DIFFDAY,@PAYFLG,@PAYFLG,@ORDAMT,@REFUNDAMT,dbo.GET_TWDATE(),dbo.GET_TWDATE();

		--把預約作廢
		IF EXISTS(SELECT * FROM TB_BookingControl WITH(NOLOCK) WHERE order_number=@OrderNo)
		BEGIN
			--預約作廢沒拋的就不轉了
			UPDATE TB_BookingControl SET PROCD='F',isRetry=case when ORDNO='' THEN 0 ELSE 1 END WHERE order_number=@OrderNo;
		END
	END

	--判斷訂單狀態
	IF @Error=0
	BEGIN
		BEGIN TRAN
		SET @hasData=0;
		SELECT @hasData=COUNT(order_number) FROM TB_OrderMain WITH(NOLOCK) WHERE IDNO=@IDNO AND order_number=@OrderNo AND (car_mgt_status<=3 AND cancel_status=0 AND booking_status<3);
		IF @hasData>0
		BEGIN
			UPDATE TB_OrderMain SET cancel_status=3 WHERE IDNO=@IDNO AND order_number=@OrderNo;

			IF @@ROWCOUNT=1
			BEGIN
				COMMIT TRAN;

				SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status,@CarNo=CarNo,@ProjType=ProjType
				FROM TB_OrderMain WITH(NOLOCK)
				WHERE order_number=@OrderNo;

				INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
				VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);
							
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
						UPDTime=@NowTime
					WHERE IDNO=@IDNO AND NormalRentBookingNowCount>0;

					-- 20210707 ADD BY YEH REASON:計算積分
					EXEC [usp_CalOrderScore] @OrderNo,'A',0,0,@FunName,@LogID,'','','','';
				END
				ELSE IF @ProjType=3
				BEGIN
					UPDATE [TB_BookingStatusOfUser] 
					SET [AnyRentBookingNowCount]=0,
						AnyRentBookingCancelCount=AnyRentBookingCancelCount+1,
						UPDTime=@NowTime
					WHERE IDNO=@IDNO AND [AnyRentBookingNowCount]>0;
				END
				ELSE IF @ProjType=4
				BEGIN
					UPDATE [TB_BookingStatusOfUser] 
					SET MotorRentBookingNowCount=0,
						MotorRentBookingCancelCount=MotorRentBookingCancelCount+1,
						UPDTime=@NowTime
					WHERE IDNO=@IDNO AND MotorRentBookingNowCount>0;
				END

				-- 20210914 UPD BY YEH REASON:將共同承租人狀態是已接受/邀請中的改為已取消
				IF EXISTS(SELECT * FROM TB_TogetherPassenger WITH(NOLOCK) WHERE Order_number=@OrderNo)
				BEGIN
					-- 將被取消的人資料撈出來回傳給API要送推播
					SELECT A.Order_number,A.MEMIDNO,dbo.FN_BlockName(C.MEMCNAME,'●') AS MEMCNAME
					FROM TB_TogetherPassenger A WITH(NOLOCK) 
					INNER JOIN TB_OrderMain B WITH(NOLOCK) ON B.order_number=A.Order_number
					INNER JOIN TB_MemberData C WITH(NOLOCK) ON C.MEMIDNO=B.IDNO
					WHERE A.Order_number=@OrderNo AND A.ChkType IN ('Y','S');

					UPDATE TB_TogetherPassenger
					SET ChkType='N',
						UPTime=@NowTime
					WHERE Order_number=@OrderNo
					AND ChkType IN ('Y','S');
				END
			END
			ELSE
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR169';
				ROLLBACK TRAN;
			END
		END
		ELSE
		BEGIN
			ROLLBACK TRAN;
			SET @Error=1;
			SET @ErrorCode='ERR168';
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingCancel';
GO