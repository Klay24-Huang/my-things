/****************************************************************
** Name: [dbo].[usp_BE_ChangeCar]
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
** EXEC @Error=[dbo].[usp_BE_ChangeCar]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/22 下午 04:06:52 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/22 下午 04:06:52    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_ChangeCar]
	@OrderNo                BIGINT                ,	--訂單編號
	@UserID                 NVARCHAR(10)          ,
	@LogID                  BIGINT                ,
	@NewCarNo               VARCHAR(10)     OUTPUT,
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
DECLARE @haveCar TINYINT;
DECLARE @car_mgt_status TINYINT;
DECLARE @cancel_status TINYINT;
DECLARE @booking_status TINYINT;
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;

/*換車要用參數*/
DECLARE @tmpOrderNum            BIGINT;
DECLARE @IDNO					VARCHAR(10);	--身份證
DECLARE @ProjID					VARCHAR(10);	--專案代碼
DECLARE @ProjType               TINYINT;		--專案類型(0:同站;3:路邊;4:機車)
DECLARE @StationID				VARCHAR(6);		--取車據點
DECLARE @CarType                VARCHAR(20);	--車款		20210715 UPD BY YEH REASON:因應CC把長度改為20
DECLARE @RStationID				VARCHAR(6);		--還車據點
DECLARE @SD						DATETIME;		--預計取車時間
DECLARE @ED						DATETIME;		--預計還車時間
DECLARE @StopPickTime           DATETIME;		--最後取車時間
DECLARE @Price					INT;			--預估租金
DECLARE @CarNo					VARCHAR(10);	--隨租隨還車號
DECLARE @Insurance              TINYINT    ;	--是否使用安心服務(0:否;1:是)
DECLARE @InsurancePurePrice     INT        ;	--安心服務預估金額
DECLARE @PayMode                TINYINT    ;	--計費模式：0:以時計費;1:以分計費
/*換車要用參數結束*/

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_ChangeCar';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @haveCar=0;
SET @Descript=N'';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @NewCarNo='';
SET @ProjType=5;
SET @UserID=ISNULL (@UserID,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @tmpOrderNum=0;

BEGIN TRY
	IF @UserID=''  OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		SET @Descript=CONCAT(N'後台操作【換車】，操作者【',@UserID,'】');
		SET @hasData=0;

		SELECT @tmpOrderNum=ISNULL(order_number,0),
			@CarNo=ISNULL(OrderMain.CarNo,''),
			@SD=ISNULL(start_time,'1911-01-01 00:00:00'),
			@ED=ISNULL(stop_time,'1911-01-01 00:00:00'),
			@StationID=ISNULL(lend_place,''),
			@IDNO=ISNULL(IDNO,''),
			@ProjID=ISNULL(OrderMain.ProjID,''),
			@CarType=ISNULL(VW.CarTypeGroupCode,'')
		FROM TB_OrderMain AS OrderMain WITH (NOLOCK)
		INNER JOIN TB_CarInfo AS CarInfo WITH(NOLOCK) ON CarInfo.CarNo=OrderMain.CarNo
		INNER JOIn VW_GetFullProjectCollectionOfCarTypeGroup AS VW WITH(NOLOCK) ON VW.PROJID=OrderMain.ProjID AND VW.CARTYPE=CarInfo.CarType
		WHERE order_number=@OrderNo AND car_mgt_status=0 AND cancel_status=0 AND booking_status=0;

		IF @tmpOrderNum=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR729';
		END

		IF @Error=0
		BEGIN
			BEGIN TRAN

			DECLARE @tmpCar TABLE(
				CarNo VARCHAR(10) NOT NULL PRIMARY KEY,
			    RentCount INT 
			);

			--將車號及出租次數先寫入暫存
			INSERT INTO @tmpCar
			SELECT Car.CarNo,CarInfo.RentCount
			FROM TB_Car AS Car WITH(NOLOCK)
			INNER JOIN TB_CarInfo AS CarInfo WITH(NOLOCK) ON CarInfo.CarNo=Car.CarNo AND Car.CarType IN (
				SELECT VW.CARTYPE FROM VW_GetFullProjectCollectionOfCarTypeGroup AS VW WITH(NOLOCK) 
				WHERE CarTypeGroupCode=@CarType AND VW.PROJID=@ProjID AND VW.StationID=@StationID
			)
			WHERE available<=1 AND nowStationID=@StationID AND CarInfo.CID<>'';

			--由暫存取出是否有符合的車輛
			SELECT TOP 1 @NewCarNo=CarNo FROM @tmpCar 
			WHERE CarNo NOT IN (
				SELECT CarNo FROM TB_OrderMain  WITH(NOLOCK)
				WHERE (booking_status<5 AND car_mgt_status<16 AND cancel_status=0)
				AND CarNo IN (SELECT CarNo FROM @tmpCar)
				AND (
					(start_time BETWEEN @SD AND @ED) OR 
					(stop_time BETWEEN @SD AND @ED) OR 
					(@SD BETWEEN start_time AND stop_time) OR 
					(@ED BETWEEN start_time AND stop_time) OR 
					(DATEADD(MINUTE,-30,@SD) BETWEEN start_time AND stop_time) OR 
					(DATEADD(MINUTE,30,@ED) BETWEEN start_time AND stop_time)
				)
			) 
			ORDER BY RentCount ASC;

			--判斷有沒有預約到車
			IF @NewCarNo='' 
			BEGIN
				SET @haveCar=0;
				ROLLBACK TRAN;
				SET @Error=1;
				SET @ErrorCode='ERR731';
			END
			ELSE
			BEGIN
				UPDATE TB_OrderMain SET CarNo=@NewCarNo WHERE order_number=@OrderNo;

				UPDATE TB_CarInfo SET RentCount=RentCount+1,UPDTime=@NowTime WHERE CarNo=@NewCarNo;

				UPDATE TB_CarInfo SET RentCount=RentCount-1,UPDTime=@NowTime WHERE CarNo=@CarNo AND (RentCount-1)>0;

				--寫入歷史記錄
				INSERT INTO TB_OrderHistory(OrderNum,Descript)VALUES(@OrderNo,@Descript);

				COMMIT TRAN;
			END
			--判斷有無預約到車結束
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ChangeCar';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ChangeCar';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台換車', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ChangeCar';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ChangeCar';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ChangeCar';