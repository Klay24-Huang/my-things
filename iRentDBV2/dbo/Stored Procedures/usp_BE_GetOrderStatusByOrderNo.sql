/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_BE_GetOrderStatusByOrderNo
* 系    統 : IRENT
* 程式功能 : 後台-使用訂單編號取出該訂單基本資訊
* 作    者 : ERIC
* 撰寫日期 : 20201030
* 修改日期 : 20210907 UPD BY YEH REASON:增加副承租人每小時費率總和
			 20211109 UPD BY YEH REASON:增加預授權金額
			 20220222 UPD BY YEH REASON:預授權金額=信用卡(台新/中信)+錢包

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_BE_GetOrderStatusByOrderNo]
	@IDNO                   VARCHAR(10)           ,
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
DECLARE @NowTime DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_BE_GetOrderStatusByOrderNo';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @UserID=ISNULL (@UserID,'');

BEGIN TRY
	IF @UserID='' OR @IDNO='' OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		-- 20220222 UPD BY YEH REASON:預授權金額=信用卡(台新/中信)+錢包
		DECLARE @CreditAmount INT = 0;	-- 信用卡預授權金額
		DECLARE @WalletAmount INT = 0;	-- 錢包預授權金額

		SELECT @CreditAmount=ISNULL(SUM(CloseAmout),0) FROM TB_TradeClose WITH(NOLOCK) WHERE OrderNo=@OrderNo;
		SELECT @WalletAmount=ISNULL(SUM(Amount),0) FROM TB_WalletHistory WITH(NOLOCK) WHERE OrderNo=@OrderNo AND Mode=0;

		SELECT VW.order_number AS OrderNo,
			   VW.lend_place AS StationID,
			   VW.StationName,
			   VW.Tel,
			   VW.ADDR,
			   VW.Latitude,
			   VW.Longitude,
			   VW.Content,		--據點相關
			   VW.OperatorName,
			   VW.OperatorICon,
			   VW.Score,		--營運商相關
			   VW.CarBrend,
			   VW.CarOfArea,
			   VW.CarTypeName,
			   VW.CarTypeImg,
			   VW.Seat,
			   VW.parkingSpace, --車子相關
			   VW.CarTypeGroupCode,
			   VW.device3TBA,
			   VW.RemainingMilage, --機車電力相關
			   VW.ProjID,
			   VW.ProjType,
			   VW.PRONAME,	--專案基本資料
			   IIF(VW.PayMode=0, VW.PRICE/10, VW.PRICE) AS PRICE, --平日每小時價 20201003 ADD BY ADAM
			   IIF(VW.PayMode=0, VW.PRICE_H/10, VW.PRICE_H) AS PRICE_H, --假日每小時價 20201003 ADD BY ADAM
			   OrderPrice = 0, --ISNULL(NYP.PAYAMT,0),	--春節訂金
			   UseOrderPrice = 0, --ISNULL(NYP.PAYAMT,0) - dbo.FN_UnUseOrderPrice(ISNULL(NYP.PAYAMT,0),VW.start_time,VW.stop_time,VW.final_start_time,VW.final_stop_time),	--使用訂金
			   LastOrderPrice = 0,	--剩餘訂金
			   VW.BaseMinutes,
			   VW.BaseMinutesPrice,
			   VW.MinuteOfPrice,
			   VW.MinuteOfPriceH,
			   VW.MaxPrice, --當ProjType=4才有值
			   VW.start_time,
			   VW.final_start_time,
			   VW.stop_pick_time,
			   VW.stop_time,
			   VW.final_stop_time,
			   ISNULL(VW.fine_Time,'') AS fine_Time,
			   VW.init_price,
			   VW.Insurance,
			   VW.InsurancePurePrice,
			   VW.init_TransDiscount,
			   VW.car_mgt_status,
			   VW.booking_status,
			   VW.cancel_status ,
			   ISNULL(Setting.MilageBase, IIF(VW.ProjType=4, 0, -1)) AS MilageUnit,
			   VW.already_lend_car,
			   VW.IsReturnCar,
			   VW.CarNo,
			   VW.final_price,
			   VW.start_mile,
			   VW.end_mile ,
			   InsurancePerHours = CASE WHEN VW.ProjType=4 THEN 0
									   WHEN K.InsuranceLevel IS NULL THEN II.InsurancePerHours
									   WHEN K.InsuranceLevel < 4 THEN K.InsurancePerHours
									   ELSE 0 END, --安心服務每小時價
			   VW.WeekdayPrice,
			   VW.HoildayPrice, --汽車租金牌價 20201117 eason
			   VW.FirstFreeMins, --前n分鐘免費
			   JointInsurancePerHour = ISNULL((SELECT SUM(InsurancePerHours) FROM TB_SavePassenger SP WITH(NOLOCK) WHERE SP.Order_number=VW.order_number),0),	-- 20210903 UPD BY YEH REASON:增加副承租人每小時費率總和
			   PreAmount = @CreditAmount + @WalletAmount	-- 20211109 UPD BY YEH REASON:增加預授權金額
		FROM VW_GetOrderData AS VW WITH(NOLOCK)
		LEFT JOIN TB_MilageSetting AS Setting WITH(NOLOCK) ON Setting.ProjID=VW.ProjID AND (VW.start_time BETWEEN Setting.SDate AND Setting.EDate)
		LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=VW.IDNO
		LEFT JOIN TB_InsuranceInfo K WITH(NOLOCK) ON K.CarTypeGroupCode=VW.CarTypeGroupCode AND K.useflg='Y' AND BU.InsuranceLevel=K.InsuranceLevel
		LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=VW.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3 --預設專用
		--LEFT JOIN TB_NYPayList NYP WITH(NOLOCK) ON VW.order_number=NYP.order_number
		WHERE VW.IDNO=@IDNO
		  AND VW.order_number=@OrderNo
		  AND cancel_status=0
		ORDER BY start_time ASC;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetOrderStatusByOrderNo';
GO