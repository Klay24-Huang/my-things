/********************************************************************************************************
* Server   :  SQYHI03AZ
* Database :  IRENT_V2
* 系    統 :  IRENT後台
* 程式名稱 :  exec usp_GetFinishOrderList 
exec usp_GetFinishOrderList 'S124413890','30058240DBEB4B3E09F23FF3F40D0106946AD846569DB940B1D931A7EFF7D2BE','2021','1','','','','','',''
* 程式功能 :  完成的訂單查詢
* 程式作者 :  2020/10/7 下午 02:09:16 ADD BY Eric
* 程式修改 :  2021/09/03 UPDATE BY PO YU 新增判斷是否為共同承租人訂單 以及新增共同承租人歷史訂單 

********************************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetFinishOrderList]
	@IDNO                   VARCHAR(10)           ,
	@Token                  VARCHAR(1024)         ,
	@ShowYear               INT                   , --0:不顯示整年;20XX表顯示該年
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

SET @FunName='usp_GetFinishOrderList';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【查詢已完成的訂單】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO=ISNULL (@IDNO,'');
SET @Token=ISNULL (@Token,'');

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
			IF @ShowYear=0
				BEGIN
					SET @ShowYear=YEAR(@NowTime);
				END

			;WITH T
			AS (
				SELECT ROW_NUMBER() OVER (ORDER BY a.final_start_time DESC) AS RowNo,*
				FROM
				(
					SELECT OrderMain.order_number AS OrderNo
					,OrderMain.CarNo,OrderMain.ProjType
					,OrderMain.unified_business_no AS UniCode
					,OrderDetail.final_start_time
					,OrderDetail.final_stop_time
					,OrderDetail.final_price
					,Car.CarOfArea,Station.[Location] AS StationName
					,VWFullData.CarTypeImg
					,YEAR(OrderDetail.final_start_time) AS RentYear
					,Station.Area
					,IIF(EXISTS(SELECT TOP 1 1 FROM TB_SavePassenger with(nolock) WHERE Order_number=OrderMain.order_number), 1, 0) as IsJointOrder

					FROM TB_SavePassenger AS SavePassenger WITH(NOLOCK)
					LEFT JOIN TB_OrderMain AS OrderMain WITH(NOLOCK) on SavePassenger.Order_number=OrderMain.order_number
					LEFT JOIN TB_Car AS Car WITH(NOLOCK) ON Car.CarNo=OrderMain.CarNo
					LEFT JOIN VW_GetFullProjectCollectionOfCarTypeGroup As VWFullData WITH(NOLOCK) ON VWFullData.CARTYPE=Car.CarType AND VWFullData.StationID=OrderMain.lend_place AND VWFullData.PROJID=OrderMain.ProjID
					LEFT JOIN TB_iRentStation AS Station WITH(NOLOCK) ON Station.StationID=OrderMain.lend_place
					INNER JOIN TB_OrderDetail AS OrderDetail WITH(NOLOCK) ON OrderDetail.order_number=OrderMain.order_number
					WHERE MEMIDNO=@IDNO AND isDelete=0 AND car_mgt_status=16 AND booking_status=5 
					--AND YEAR(OrderDetail.final_start_time)=@ShowYear
					AND OrderDetail.final_start_time > DATEADD(year,-1,dbo.GET_TWDATE())    --20210901 ADD BY PO YU.歷史訂單新增共同承租人訂單
						UNION
					SELECT OrderMain.order_number AS OrderNo
							,OrderMain.CarNo,OrderMain.ProjType
							,OrderMain.unified_business_no AS UniCode
							,OrderDetail.final_start_time
							,OrderDetail.final_stop_time
							,OrderDetail.final_price
							,Car.CarOfArea,Station.[Location] AS StationName
							,VWFullData.CarTypeImg
							,YEAR(OrderDetail.final_start_time) AS RentYear
							,Station.Area
							,IIF(EXISTS(SELECT TOP 1 1 FROM TB_SavePassenger with(nolock) WHERE Order_number=OrderMain.order_number), 1, 0) as IsJointOrder
					FROM TB_OrderMain AS OrderMain WITH(NOLOCK) 
					LEFT JOIN TB_Car AS Car WITH(NOLOCK) ON Car.CarNo=OrderMain.CarNo
					LEFT JOIN VW_GetFullProjectCollectionOfCarTypeGroup As VWFullData WITH(NOLOCK) ON VWFullData.CARTYPE=Car.CarType AND VWFullData.StationID=OrderMain.lend_place AND VWFullData.PROJID=OrderMain.ProjID
					LEFT JOIN TB_iRentStation AS Station WITH(NOLOCK) ON Station.StationID=OrderMain.lend_place
					INNER JOIN TB_OrderDetail AS OrderDetail WITH(NOLOCK) ON OrderDetail.order_number=OrderMain.order_number
					WHERE IDNO=@IDNO AND isDelete=0 AND car_mgt_status=16 AND booking_status=5 
					--AND YEAR(OrderDetail.final_start_time)=@ShowYear
					AND OrderDetail.final_start_time > DATEADD(year,-1,dbo.GET_TWDATE())   --20210101 ADD BY ADAM REASON.歷史訂單跟著跨年，先改為一年內訂單
					)  a
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetFinishOrderList';
GO