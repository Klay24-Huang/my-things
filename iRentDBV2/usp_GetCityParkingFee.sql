/****************************************************************
** Name: [dbo].[usp_GetCityParkingFee]
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
** EXEC @Error=[dbo].[usp_GetProjectPriceBase]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Adam 
** Date:2021/04/29 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2021/04/29|  Adam	  |          First Release
**			 |			  |
*****************************************************************/
/* TESTING EXAMPLE
DECLARE @ParkingFee INT =0
EXEC usp_GetCityParkingFee 10053662,'2021-04-19 17:00:00','2021-04-21 17:00:00','A122364317',9999,@ParkingFee OUTPUT,'','','',''
SELECT @ParkingFee
*/
ALTER PROCEDURE [dbo].[usp_GetCityParkingFee]
	@OrderNo			BIGINT,					--案件編號
	@SD					DATETIME,				--
	@ED					DATETIME,				--
	@IDNO				VARCHAR(10)				,--
	@LogID				BIGINT                ,
	@ParkingFee			INT				OUTPUT, --停車費回傳
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
DECLARE @NowTime DATETIME;
DECLARE @Descript NVARCHAR(200);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetCityParkingFee';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'綁定CityParking停車費';
SET @NowTime=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
	
	IF @OrderNo =0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	--IF @Error=0
	--BEGIN
	--	--排除案件狀態
	--	IF NOT EXISTS(SELECT * FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo AND car_mgt_status=11	AND cancel_status=0)
	--	BEGIN
	--		SET @Error=1;
	--		SET @ErrorCode='ERR900'
	--	END

	--	--SELECT * FROM TB_OrderMain WITH(NOLOCK) WHERE 
	--END

	IF @Error=0
	BEGIN
		SELECT * INTO #TB_CityParking FROM TB_CityParking with(NOLOCK)	
		WHERE meter_started_at >= @SD AND meter_ended_at <= @ED
		AND CancelMark=''
		AND request_amount IS NOT NULL

		IF EXISTS(SELECT * FROM #TB_CityParking)
		BEGIN
			--先更新資料
			UPDATE TB_CityParking 
			SET OrderNo=@OrderNo
				,UpdateDate=@NowTime
				,UpdateUserID=@IDNO
				,UpdateEvent='GetPayDetail'
			WHERE ID IN (SELECT ID FROM #TB_CityParking)
			
			INSERT INTO TB_CityParking_LOG
			SELECT 'U',@FunName,@NowTime,* FROM TB_CityParking WITH(NOLOCK) WHERE ID IN (SELECT ID FROM #TB_CityParking)

			SELECT @ParkingFee = SUM(request_amount) 
			FROM TB_CityParking with(NOLOCK) 
			WHERE meter_started_at >= @SD AND meter_ended_at <= @ED
			
		END
		ELSE
		BEGIN
			SET @ParkingFee=0
		END

		DROP TABLE #TB_CityParking
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

RETURN 0
