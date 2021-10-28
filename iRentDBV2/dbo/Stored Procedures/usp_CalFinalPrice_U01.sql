/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_CalFinalPrice_U01
* 系    統 : IRENT
* 程式功能 : 更新計算租金結果
* 作    者 : eason
* 撰寫日期 : 20210107
* 修改日期 : 20211026 UPD BY YEH REASON:差額存檔

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_CalFinalPrice_U01]
	@IDNO				VARCHAR(10)				,	--帳號
	@OrderNo			BIGINT					,	--訂單編號
	@final_price		INT						,	--總價
	@pure_price			INT						,	--車輛租金
	@mileage_price		INT						,	--里程費
	@Insurance_price	INT						,	--安心服務費
	@fine_price			INT						,	--逾時費用
	@gift_point			INT						,	--使用時數(汽車)
	@gift_motor_point	INT						,	--使用時數(機車)
	@monthly_workday	FLOAT					,	--使用的月租平日時數
	@monthly_holiday	FLOAT					,	--使用的月租假日時數
    @Etag				INT						,	--ETAG費用
	@parkingFee			INT						,	--停車費
	@TransDiscount		INT						,	--轉乘優惠
	@Token				VARCHAR(1024)			,
	@DiffAmount			INT						,	--差額
	@APIName			VARCHAR(50)				,	--API名稱
	@LogID				BIGINT					,
	@ErrorCode			VARCHAR(6)		OUTPUT	,	--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		--回傳sqlException訊息
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
DECLARE @ProjType INT;
DECLARE @APIID INT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_CalFinalPrice_U01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @Descript=N'使用者操作【計算租金】';
SET @car_mgt_status=0;
SET @cancel_status=0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @ProjType=5;
SET @IDNO=ISNULL(@IDNO,'');
SET @OrderNo=ISNULL(@OrderNo,0);
SET @Token=ISNULL(@Token,'');
SET @APIID=0;

BEGIN TRY
	IF @Token='' OR @IDNO='' OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900';
	END

	-- 檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND Rxpires_in>@NowTime;
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

	-- 更新資料
	IF @Error=0
	BEGIN
		SELECT @booking_status=booking_status,
				@cancel_status=cancel_status,
				@car_mgt_status=car_mgt_status,
				@ProjType=ProjType
		FROM TB_OrderMain WITH(NOLOCK)
		WHERE order_number=@OrderNo;

		SELECT @APIID=ISNULL(APIID,0) FROM TB_APIList WITH(NOLOCK) WHERE APIName=@APIName;

		UPDATE TB_OrderDetail
		SET final_price=@final_price,
			pure_price=@pure_price,
			mileage_price=@mileage_price,
			Insurance_price=@Insurance_price,
			fine_price=@fine_price,
			gift_point=@gift_point,
			gift_motor_point=@gift_motor_point,
			monthly_workday = @monthly_workday,
			monthly_holiday = @monthly_holiday,
			Etag =@Etag,
			parkingFee=@parkingFee,
			TransDiscount=@TransDiscount  
		WHERE order_number=@OrderNo;
		
		-- 20211026 UPD BY YEH REASON:差額存檔
		IF NOT EXISTS(SELECT * FROM TB_OrderExtinfo WITH(NOLOCK) WHERE order_number=@OrderNo)
		BEGIN
			INSERT INTO TB_OrderExtinfo (order_number,DiffAmount,MKTime,MKUser,MKPRGID,UPDTime,UPDUser,UPDPRGID)
			VALUES(@OrderNo,@DiffAmount,@NowTime,@IDNO,@APIID,@NowTime,@IDNO,@APIID);
		END
		ELSE
		BEGIN
			UPDATE TB_OrderExtinfo
			SET DiffAmount=@DiffAmount,
				UPDTime=@NowTime,
				UPDUser=@IDNO,
				UPDPRGID=@APIID
			WHERE order_number=@OrderNo;
		END

		--寫入歷程
		INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
		VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CalFinalPrice';
GO