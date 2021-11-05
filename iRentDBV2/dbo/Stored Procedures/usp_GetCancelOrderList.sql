/****** Object:  StoredProcedure [dbo].[usp_GetCancelOrderList]    Script Date: 2021/11/5 上午 09:32:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/********************************************************************************************************
* Server   :  SQYHI03AZT
* Database :  IRENT_V2T
* 系    統 :  IRENT測試API
* 程式名稱 :  usp_GetCancelOrderList 
* 程式功能 :  獲取已取消訂單
* 程式作者 :  2020/9/25 上午 10:17:48 ADD BY Eric
* 程式修改 :  20201006 UPDATE BY ADAM 排除已刪除的清單
*			  20211102 UPDATE BY PO YU 新增判斷取消訂單原因欄位
*			  20211103 UPD BY Po Yu  REASON.  Join TB_Code判斷取消訂單
*             20211105 UPD BY Po Yu  REASON. 更改cancel_status為文字輸出
********************************************************************************************************/
ALTER PROCEDURE [dbo].[usp_GetCancelOrderList]
	@IDNO                   VARCHAR(10)           ,
	@Token                  VARCHAR(1024)         ,
	@pageSize				INT					  ,	--每頁幾筆
	@pageNo					INT					  ,	--第幾頁
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
DECLARE @hasData BIGINT;
DECLARE @car_mgt_status TINYINT;
DECLARE @cancel_status TINYINT;
DECLARE @booking_status TINYINT;
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;
DECLARE @maxPage INT;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetCancelOrderList';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【查詢取消訂單】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO=ISNULL (@IDNO,'');
SET @Token=ISNULL (@Token,'');
SET @pageSize=ISNULL(@pageSize,10);		
SET @pageNo=ISNULL(@pageNo,1);
SET @maxPage=0;

BEGIN TRY
	IF @Token='' OR @IDNO='' 
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

	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(order_number) 	FROM TB_OrderMain AS OrderMain WITH(NOLOCK)  WHERE IDNO=@IDNO AND cancel_status>0
		IF @hasData>0
		BEGIN
			SET @maxPage=CEILING(@hasData/@pageSize);
			IF (@hasData%@pageSize>0)
			BEGIN
				SET @maxPage=@maxPage+1;
			END
			IF @pageNo>@maxPage
			BEGIN
				SET @pageNo=@maxPage;
			END
		END
	END

	IF @Error=0
	BEGIN
		;WITH T
		AS (
			SELECT ROW_NUMBER() OVER (ORDER BY start_time DESC) AS RowNo ,
				OrderMain.order_number,
				OrderMain.CarNo,
				OrderMain.init_price,
				OrderMain.ProjID,
				OrderMain.ProjType,
				OrderMain.start_time,
				OrderMain.stop_time ,
				VWFullData.Seat,
				VWFullData.CarBrend,
				VWFullData.Score,
				VWFullData.OperatorICon,
				VWFullData.CarTypeImg,
				VWFullData.CarTypeName,
				VWFullData.PRONAME ,
				InsurancePurePrice,
				init_TransDiscount --20201009 eason 計算預估總金額
				,ISNULL(Setting.MilageBase, IIF(OrderMain.ProjType=4, 0, -1)) AS MilageUnit ,
				CASE WHEN OrderMain.ProjType=0 THEN '同站' ELSE iRentStation.Area END CarOfArea ,
				CASE WHEN OrderMain.ProjType=0 THEN iRentStation.Location ELSE '' END AS StationName ,
				Car.IsMotor,
				Car.WeekdayPrice,
				Car.HoildayPrice,
				Car.WeekdayPriceByMinutes,
				Car.HoildayPriceByMinutes, --20201008 Eason
				CASE OrderMain.cancel_status WHEN 6 THEN '授權失敗已取消' ELSE '已取消' END AS cancel_status--20211102 Po Yu Reason:新增取消訂單的原因  20211105 UPD BY Po Yu  REASON. 更改cancel_status為文字輸出
			FROM TB_OrderMain AS OrderMain WITH(NOLOCK)
			LEFT JOIN TB_CarInfo AS Car WITH(NOLOCK) ON Car.CarNo=OrderMain.CarNo
			LEFT JOIN VW_GetFullProjectCollectionOfCarTypeGroup AS VWFullData WITH(NOLOCK) ON VWFullData.CARTYPE=Car.CarType
			AND VWFullData.StationID=OrderMain.lend_place
			AND VWFullData.PROJID=OrderMain.ProjID
			LEFT JOIN TB_MilageSetting AS Setting WITH(NOLOCK) ON Setting.ProjID=OrderMain.ProjID
			AND (OrderMain.start_time BETWEEN Setting.SDate AND Setting.EDate)
			LEFT JOIN TB_iRentStation AS iRentStation WITH(NOLOCK) ON iRentStation.StationID=OrderMain.lend_place
			left join TB_Code AS Code with(nolock)  on OrderMain.cancel_status=Code.MapCode and Code.TBMap='TB_OrderMain'
			WHERE IDNO=@IDNO
			AND Code.TypeFlag=1 --20211103 UPD BY Po Yu  REASON.Join  TB_Code判斷取消訂單
			AND isDelete=0		--20201006 ADD BY ADAM REASON.排除已刪除的清單
		),
		T2 AS (
			SELECT COUNT(1) TotalCount FROM T
		)
		SELECT *
		FROM T2, T
		WHERE RowNo BETWEEN (@pageNo - 1) * @pageSize  + 1 AND @pageNo * @pageSize;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetCancelOrderList';