/****** Object:  StoredProcedure [dbo].[usp_GetOrderDetail_20210517]    Script Date: 2021/9/2 下午 02:37:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/********************************************************************************************************
* Server   :  SQYHI03AZ
* Database :  IRENT_V2
* 系    統 :  IRENT後台
* 程式名稱 :  exec usp_GetOrderDetail 
* 程式功能 :  訂單明細查詢
* 程式作者 :  2020/10/11 下午 04:44:22 ADD BY Eric
* 程式修改 :  2021/09/03 UPDATE BY PO YU 新增判斷是否為共同承租人訂單欄位
********************************************************************************************************/
ALTER PROCEDURE [dbo].[usp_GetOrderDetail_20210517]
	@IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@Token                  VARCHAR(1024)         ,
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
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetOrderDetail';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【取消訂單】';

SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token=ISNULL (@Token,'');

BEGIN TRY
	IF @Token='' OR @IDNO=''  OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	--0.再次檢核token
	IF @Error=0
	BEGIN

	   declare @final_price int =  0
	   declare @sd datetime
	   declare @ed datetime
	   declare @fsd datetime
	   declare @fed datetime
	   declare @OrdPrice int = 0
	   declare @UseOrdPrice int = 0
	   declare @ReturnOrderPrice int = 0

	   select top 1  
	   @final_price = d.final_price,
	   @sd = o.start_time, 
	   @ed = o.stop_time, 
	   @fsd = d.final_start_time, 
	   @fed = d.final_stop_time
	   from TB_OrderMain o	   
	   join TB_OrderDetail d  on o.order_number = d.order_number
	   where d.order_number = @OrderNo	     

	    select top 1 @OrdPrice = p.PAYAMT from TB_NYPayList p where p.order_number = @OrderNo
	    set @UseOrdPrice  = ISNULL(@OrdPrice,0) - cast(FLOOR(dbo.FN_UnUseOrderPrice(@UseOrdPrice,@sd,@ed,@fsd,@fed)) as int)--使用訂金
	    select top 1 @ReturnOrderPrice = p.RETURNAMT from TB_NYPayList p where p.order_number = @OrderNo

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

	IF @Error=0
	BEGIN
		DECLARE @RenterType int;
		IF EXISTS(SELECT TOP 1 1 FROM VW_GetOrderData with(nolock) Where order_number = @OrderNo)
			BEGIN 
				SET @RenterType=1;
			END
		ELSE IF EXISTS(SELECT TOP 1 1 FROM TB_SavePassenger with(nolock) Where order_number = @OrderNo)
			BEGIN 
				SET @RenterType=2;
			END
		ELSE
			BEGIN 
				SET @RenterType=0;
			END

		SELECT VW.[order_number] AS OrderNo,
			   [OperatorICon] AS Operator,
			   [CarTypeImg] AS CarTypePic,
			   VW.[CarNo] ,
			   [Seat],
			   [CarBrend],
			   [CarTypeName] ,
			   [StationName],
			   [Score] AS OperatorScore,
			   --[PRONAME] AS ProjName,
			   --20210720 ADD BY ADAM REASON.補上訂閱制專案名稱
			   CASE WHEN mb.OrderNo IS NOT NULL THEN mrs.MonProjNM ELSE [PRONAME] END AS ProjName,
			   VW.[pure_price] ,
			   VW.[gift_point] AS GiftPoint,
			   VW.[gift_motor_point] As GiftMotorPoint,
			   (VW.monthly_workday + VW.monthly_holiday) AS MonthlyHours ,
			   VW.[ProjType],
			   VW.[final_start_time] AS StartTime,
			   VW.[final_stop_time] AS EndTime ,
			   OrderPrice = @OrdPrice, --全部訂金
			   UseOrderPrice =  iif(@UseOrdPrice > 0,@UseOrdPrice,0 ),--使用訂金
			   ReturnOrderPrice = iif(@ReturnOrderPrice > 0, @ReturnOrderPrice,0),--退還訂金
			   LastOrderPrice = 0,--剩餘訂金
			   OrderPrice = ISNULL(NYP.PAYAMT,0),		--春節訂金
			   VW.[mileage_price],
			   VW.[Insurance_price],
			   VW.[Etag],
			   VW.[fine_price] ,
			   VW.[final_price] ,
			   VW.[TransDiscount],
			   VW.[parkingFee] ,
			   invoice_date=RTRIM(VW.invoice_date),
			   VW.invoice_price,
			   VW.invoiceCode,
			   VW.bill_option AS InvoiceType ,
			   VW.[CARRIERID],
			   VW.[NPOBAN],
			   VW.unified_business_no,
			   ISNULL(Love.LoveName, '') AS NPOBAN_Name,
			   VW.Millage,
			   VW.Area,
			   VW.start_mile,
			   VW.end_mile,
			   0 AS DiscountAmount,
			   '' As DiscountName,
			   ISNULL(Fee.CarDispatch,0) AS CarDispatch,
			   ISNULL(Fee.CleanFee,0) AS CleanFee,
			   ISNULL(Fee.DestroyFee,0) AS DestroyFee,
			   ISNULL(Fee.ParkingFee,0) AS ParkingFee2,
			   ISNULL(Fee.DraggingFee,0) AS DraggingFee,
			   ISNULL(Fee.OtherFee,0) AS OtherFee,
			   --20210517 ADD BY ADAM REASON.新換電獎勵需求
			   ChangePoint = ISNULL(MCBH.ChgGift,0),
			   ChangeTimes = ISNULL(MCBH.ChgTimes,0),
			   RSOC_S = ISNULL(MCBH.RSOC_S,0),
			   RSOC_E = ISNULL(MCBH.RSOC_E,0),
			   RewardPoint = ISNULL(MCBH.RewardGift,0),
			   TotalRewardPoint = ISNULL(MCBH.TotalGift,0),
			   RenterType = @RenterType 
		FROM [dbo].[VW_GetOrderData] AS VW WITH(NOLOCK)
		LEFT JOIN TB_LoveCode AS Love WITH(NOLOCK) ON Love.LoveCode=VW.NPOBAN
		LEFT JOIN TB_OrderOtherFee AS Fee WITH(NOLOCK) ON Fee.OrderNo=VW.order_number
		LEFT JOIN TB_NYPayList NYP WITH(NOLOCK) ON VW.order_number=NYP.order_number
		--20210517 ADD BY ADAM REASON.新換電獎勵需求
		LEFT JOIN TB_MotorChangeBattHis MCBH WITH(NOLOCK) ON VW.order_number=MCBH.order_number

		--20210720 ADD BY ADAM REASON.訂閱制專案名稱修改
		LEFT JOIN TB_SubsBookingMonth mb with(nolock) ON mb.OrderNo = VW.order_number
		LEFT JOIN SYN_MonthlyRent mr with(nolock) ON mr.MonthlyRentId=mb.MonthlyRentId
		LEFT JOIN TB_MonthlyRentSet mrs with(nolock) ON mr.ProjID=mrs.MonProjID AND mr.MonProPeriod=mrs.MonProPeriod AND mr.ShortDays=mrs.ShortDays


		WHERE VW.order_number=@OrderNo;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderDetail';
