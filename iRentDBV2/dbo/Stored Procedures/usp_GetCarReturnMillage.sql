/****** Object:  StoredProcedure [dbo].[usp_GetCarReturnMillage]    Script Date: 2021/2/19 下午 03:57:32 ******/

/****************************************************************
** Name: [dbo].[usp_GetCarReturnMillage]
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
** EXEC @Error=[dbo].[usp_GetCarReturnMillage]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:
** Date:
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetCarReturnMillage]
	@IDNO				VARCHAR(10)           , --帳號
	@OrderNo			BIGINT                , --訂單編號
	@LogID				BIGINT                , --LOGID
	@Millage			INT             OUTPUT, --里程數
	@ErrorCode			VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
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
DECLARE @CarNo VARCHAR(10);	--車號
DECLARE @NowTime DATETIME;	--系統時間
DECLARE @StatrDate DATETIME;	--取車時間
DECLARE @EndDate DATETIME;	--還車時間
DECLARE @StatrDateString VARCHAR(8);	--取車時間
DECLARE @EndDateString VARCHAR(8);	--還車時間
DECLARE @NowDateString VARCHAR(8);	--系統時間

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_GetCarReturnMillage';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @IDNO=ISNULL(@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Descript=N'後台強還【取還車里程】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @CarNo='';
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @StatrDateString='';
SET @EndDateString='';
SET @NowDateString=CONVERT(VARCHAR,@NowTime,112);
SET @Millage=0;

BEGIN TRY
	IF @IDNO='' OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		SELECT @booking_status=A.booking_status,
			@cancel_status=A.cancel_status,
			@car_mgt_status=A.car_mgt_status,
			@CarNo=A.CarNo,
			@StatrDate=ISNULL(B.final_start_time,@NowTime),
			@EndDate=ISNULL(B.final_stop_time,@NowTime),
			@StatrDateString=ISNULL(CONVERT(VARCHAR, B.final_start_time, 112),''),
			@EndDateString=ISNULL(CONVERT(VARCHAR, B.final_stop_time, 112),''),
			@Millage=B.end_mile
		FROM TB_OrderMain A WITH(NOLOCK)
		INNER JOIN TB_OrderDetail B WITH(NOLOCK) ON B.order_number=A.order_number
		WHERE A.order_number=@OrderNo;

		IF @Millage = 0
		BEGIN
			--照理來說有還車時間就有還車里程，以防資料有問題，這段邏輯還是先寫著備用，不然正常狀況都走ELSE
			IF @EndDateString <> ''	--有還車時間
			BEGIN
				IF @EndDateString = @NowDateString
				BEGIN
					--還車時間=系統時間，里程數抓TB_CarStatus
					SELECT @Millage=Millage FROM TB_CarStatus WITH(NOLOCK) WHERE CarNo=@CarNo;
				END
				ELSE
				BEGIN
					--不同天則抓該車下筆訂單的起始里程
					SELECT TOP 1 @Millage=B.start_mile FROM TB_OrderMain A WITH(NOLOCK)
					INNER JOIN TB_OrderDetail B WITH(NOLOCK) ON B.order_number=A.order_number
					WHERE A.CarNo=@CarNo AND A.order_number<>@OrderNo AND B.final_start_time>=@EndDate
					ORDER BY B.final_start_time
				END
			END
			ELSE
			BEGIN
				IF @StatrDateString = @NowDateString
				BEGIN
					--取車時間=系統時間，里程數抓TB_CarStatus
					SELECT @Millage=Millage FROM TB_CarStatus WITH(NOLOCK) WHERE CarNo=@CarNo;
				END
				ELSE
				BEGIN
					--不同天則抓該車下筆訂單的起始里程
					SELECT TOP 1 @Millage=B.start_mile FROM TB_OrderMain A WITH(NOLOCK)
					INNER JOIN TB_OrderDetail B WITH(NOLOCK) ON B.order_number=A.order_number
					WHERE A.CarNo=@CarNo AND A.order_number<>@OrderNo AND B.final_start_time>=@StatrDate
					ORDER BY B.final_start_time
				END
			END
		END

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetCarReturnMillage';
GO

