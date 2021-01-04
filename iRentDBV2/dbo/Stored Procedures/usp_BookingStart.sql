/****************************************************************
** Name: [dbo].[usp_BookingStart]
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
** EXEC @Error=[dbo].[usp_BookingStart]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/24 上午 05:03:30 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/24 上午 05:03:30    |  Eric|          First Release
**			 |			  |
*****************************************************************/
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

DECLARE @RentNowActiveType              TINYINT;
DECLARE @NowActiveOrderNum				BIGINT;

DECLARE @IsMotor	 INT		--是否為機車
DECLARE @PrevOrderNo INT		--上一筆訂單編號
DECLARE @PrevIsMotor INT		--上一筆專案是否為機車
DECLARE @PrevRentPrice INT		--上一筆專案租金
DECLARE @PrevFinalStopTime DATETIME		--上一筆專案
DECLARE @TransferPrice INT=0

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BookingStart';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
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

	--檢核會員狀態 20210104 ADD BY ADAM
	IF @Error=0
	BEGIN
		--審核不通過不可取車
		IF EXISTS(SELECT Audit FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND Audit=2)
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR239';
		END
	END

	IF @Error=0
	BEGIN
		SELECT @RentNowActiveType=ISNULL(RentNowActiveType,5),@NowActiveOrderNum=ISNULL(NowActiveOrderNum,0)
		FROM [dbo].[TB_BookingStatusOfUser] WITH(NOLOCK)
		WHERE IDNO=@IDNO;
		IF @RentNowActiveType NOT IN(0,5) AND @NowActiveOrderNum>0
		BEGIN
			--20210104 ADD BY ADAM REASON.針對還在目前案件做判斷
			IF EXISTS(SELECT order_number FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@NowActiveOrderNum AND car_mgt_status=16)
			BEGIN
				--已還車要更新
				UPDATE TB_BookingStatusOfUser SET @NowActiveOrderNum=0 WHERE IDNO=@IDNO;
				SET @NowActiveOrderNum=0;
			END
			ELSE
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR172';
			END
		END
	END
	IF @Error=0
	BEGIN
		IF @ProjType=3 AND @StopTime<>''
		BEGIN
			SELECT @hasData=COUNT(1) FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo AND start_time>CONVERT(datetime,@StopTime);
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR175';
			END
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
				
		IF @hasData>0
		BEGIN
			--寫入記錄
			SELECT @booking_status=booking_status,
				   @cancel_status=cancel_status,
				   @car_mgt_status=car_mgt_status,
				   @CarNo=CarNo,
				   @ProjType=ProjType,
				   @IsMotor=CASE WHEN ProjType=4 THEN 1 ELSE 0 END
			FROM TB_OrderMain WITH(NOLOCK)
			WHERE order_number=@OrderNo;

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
			FROM TB_OrderMain A WITH(NOLOCK)
			JOIN TB_OrderDetail B WITH(NOLOCK) ON A.order_number=B.order_number
			WHERE A.order_number<@OrderNo		--上一筆訂單
			AND A.IDNO=@IDNO AND A.car_mgt_status>=16
			ORDER BY A.order_number DESC

			--運具轉換且時間在一個小時內轉乘
			IF @PrevOrderNo>0 AND @IsMotor<>@PrevIsMotor AND DATEADD(hour,1,@PrevFinalStopTime) > @NowTime
			BEGIN
				--設定可折抵金額
				SELECT @TransferPrice = CASE WHEN @PrevRentPrice>=46 THEN 46 ELSE @PrevRentPrice END
			END

			-- 20201220 by EDWARD 因車機有時候回傳錯誤的[取車里程]，所以暫時查詢上一筆合約的還車里程來取代[取車里程]
			declare @pre_end_mile float=0
			set @pre_end_mile = isnull((select top 1 D.end_mile 
										  from IRENT_V2..TB_OrderMain C with(nolock)
										  join IRENT_V2..TB_OrderDetail D with(nolock) ON D.order_number=C.order_number
										 where C.CarNo=@CarNo and D.final_start_time<@NowTime
										 order by D.final_start_time desc),0)
			--if (@NowMileage<>@pre_end_mile)
			if @pre_end_mile>0 and ABS(@NowMileage-@pre_end_mile)>3  -- 差距超過3公里時才用上一筆合約的還車里程來取代
			begin
				set @SQLExceptionMsg = @CarNo+' ('+cast(@OrderNo as varchar)+') 錯誤的[取車里程]:'+cast(@NowMileage as varchar) +'=>'+cast(@pre_end_mile as varchar)
				insert into TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
				values (@FunName,'ErrMil',@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
				-- select top 50 * from TB_ErrorLog with(nolock) where ErrorCode ='ErrMil' order by DTime desc
				set @NowMileage=@pre_end_mile
			end
			-- end of 錯誤的[取車里程]


			--寫入訂單明細
			INSERT INTO TB_OrderDetail(order_number,already_lend_car,final_start_time,start_mile
			)VALUES(@OrderNo,1,@NowTime,@NowMileage
			);

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
				WHERE order_number=@OrderNo	
			END

			--更新主控表
			UPDATE [dbo].[TB_BookingStatusOfUser]
			SET RentNowActiveType=@ProjType,
				NowActiveOrderNum=@OrderNo
			WHERE IDNO=@IDNO;

			--寫入歷程
			INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
			VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);

			--更新車輛狀態
			UPDATE TB_Car SET available=0,NowOrderNo=@OrderNo WHERE CarNo=@CarNo;

			--加入機車取車時的電池電量及經緯度
			IF @ProjType=4
			BEGIN
				INSERT INTO TB_OrderDataByMotor(OrderNo,P_lat,P_lon,P_LBA,P_RBA,P_MBA,P_TBA)
				SELECT @OrderNo,Latitude,Longitude,deviceLBA,deviceRBA,deviceMBA,device3TBA 
				FROM TB_CarStatus WITH(NOLOCK) WHERE CarNo=@CarNo
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
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingStart';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取車', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingStart';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingStart';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingStart';