/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetOrderInfoForPreAuth_Q01
* 系    統 : IRENT
* 程式功能 : 取得訂單資訊(For計算預授權金額用)
* 作    者 : AMBER
* 撰寫日期 : 20211101
* 修改日期 : 
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetOrderInfoForPreAuth_Q01]
	@OrderNumber			BIGINT                , --訂單號碼
    @ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT=0;
DECLARE @IsSystem TINYINT=0;
DECLARE @FunName VARCHAR(50)='usp_GetOrderInfoForPreAuth_Q01';
DECLARE @ErrorType TINYINT=0;
DECLARE @LogID  INT=0;

/*初始設定*/
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

BEGIN TRY
WITH extendInfo
AS (
SELECT MIN(StopTime) AS extendStartTime,MAX(ExtendStopTime) AS extendStopTime,COUNT(order_number) AS extendTimes,order_number
FROM TB_OrderExtendHistory 	WITH(NOLOCK) WHERE order_number=@OrderNumber GROUP BY order_number),
monthly
AS (SELECT TOP 1 OrderNo,u.MonthlyRentId,u.Mode,WorkDayRateForCar,WorkDayRateForMoto,HoildayRateForCar,HoildayRateForMoto FROM TB_MonthlyRentUse u WITH(NOLOCK)
JOIN TB_SubsBookingMonth s WITH(NOLOCK) ON u.MonthlyRentId=s.MonthlyRentId WHERE u.useFlag=1 AND OrderNo=@OrderNumber)
SELECT VW.order_number AS OrderNo,	 
			   VW.ProjID,
			   VW.ProjType,
			   PRICE =CASE WHEN monthly.MonthlyRentId >0 AND VW.ProjType =4 THEN monthly.WorkDayRateForMoto
			          WHEN monthly.MonthlyRentId >0 AND VW.ProjType <4 THEN (monthly.WorkDayRateForCar * 10) --20211125訂閱制目前無假日費率，假日用原本專案費率
					  ELSE VW.PRICE END, 
			   VW.PRICE_H,
			   VW.start_time AS SD,
			   VW.stop_time AS ED,		
			   ISNULL(Setting.MilageBase, IIF(VW.ProjType=4, 0,-1)) AS MilageUnit ,			
			   VW.CarNo,
			   VW.CarTypeGroupCode,
			   VW.Insurance,
			   VW.WeekdayPrice, --汽車平日逾時原價
		       VW.HoildayPrice, --汽車假日逾時原價
		 	   InsurancePerHours = CASE WHEN VW.ProjType=4 THEN 0
										WHEN K.InsuranceLevel IS NULL THEN II.InsurancePerHours
										WHEN K.InsuranceLevel < 4 THEN K.InsurancePerHours
										ELSE 0 END,
		       (SELECT ISNULL(SUM(Auth.final_price),0) FROM TB_OrderAuthAmount Auth WITH(NOLOCK) WHERE VW.order_number=Auth.order_number AND AuthType IN (1,2,3,4)) AS PreAuthAmt,	--1:預約 2.訂金 3:取車 4:延長用車		
		       ISNULL(extendInfo.extendTimes,0) AS ExtendTimes,
			   ISNULL(extendInfo.extendStartTime,'1911-01-01 00:00:00') AS ExtendStartTime,
			   ISNULL(extendInfo.extendStopTime,'1911-01-01 00:00:00') AS ExtendStopTime,
			   VW.StationID
		FROM VW_GetOrderData AS VW 	WITH(NOLOCK)
		LEFT JOIN TB_MilageSetting AS Setting WITH(NOLOCK) ON Setting.ProjID=VW.ProjID AND (VW.start_time BETWEEN Setting.SDate AND Setting.EDate) 
		LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=VW.IDNO
		LEFT JOIN TB_InsuranceInfo K WITH(NOLOCK) ON K.CarTypeGroupCode=VW.CarTypeGroupCode AND K.useflg='Y' AND BU.InsuranceLevel=K.InsuranceLevel	
		LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=VW.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3--預設專用
		LEFT JOIN extendInfo ON VW.order_number=extendInfo.order_number
		LEFT JOIN monthly ON VW.order_number=monthly.OrderNo
		WHERE VW.order_number=@OrderNumber;
END TRY
BEGIN CATCH
		SET @Error=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
RETURN @Error



