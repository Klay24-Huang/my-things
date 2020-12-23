﻿/****************************************************************
** Name: [dbo].[usp_ReturnCar]
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
** EXEC @Error=[dbo].[usp_ReturnCar]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/28 上午 09:38:48 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/28 上午 09:38:48    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_ReturnCar]
	@IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@NowMileage             FLOAT                 ,
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

SET @FunName='usp_ReturnCar';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【記錄還車時間】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
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
		BEGIN TRAN;
		SELECT @hasData=COUNT(order_number) FROM TB_OrderMain WITH(NOLOCK) WHERE IDNO=@IDNO AND order_number=@OrderNo AND car_mgt_status>=4 AND car_mgt_status<15 AND cancel_status=0;
		IF @hasData=0
		BEGIN
			ROLLBACK TRAN;
			SET @Error=1;
			SET @ErrorCode='ERR185';
		END
		
		IF @Error=0
		BEGIN
			SELECT @hasData=COUNT(order_number) FROM TB_OrderDetail WITH(NOLOCK) WHERE  order_number=@OrderNo;
			IF @hasData=0
			BEGIN
				ROLLBACK TRAN;
				SET @Error=1;
				SET @ErrorCode='ERR185';
			END
			ELSE
			BEGIN
				SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status,@CarNo=CarNo,@ProjType=ProjType
				FROM TB_OrderMain WITH(NOLOCK)
				WHERE order_number=@OrderNo;
				
				--如果取不到里程，從tb取出
				IF @NowMileage=0
				BEGIN
					SELECT @NowMileage=Millage FROM TB_CarStatus WITH(NOLOCK) WHERE CarNo=@CarNo;
				END

				-- 因應車機訊號回應資料不穩，增加這段判斷 START
				DECLARE @start_mile float;	--起始里程
				DECLARE @temp_time Datetime = DATEADD(MINUTE,-10,@NowTime);
				DECLARE @CarRawData_Millage float;
				DECLARE @Start_Date Datetime;
				DECLARE @WMin int;	--平日時數
				DECLARE @HMin int;	--假日時數
				DECLARE @TotalDay int;	--總使用天數

				SELECT @start_mile=start_mile,@Start_Date=final_start_time FROM TB_OrderDetail WITH(NOLOCK) WHERE order_number=@OrderNo;

				-- 取TB_CarRawData的最後一筆資料 條件下：GPSTime介於(系統時間-10分)~系統時間 & 里程數>0 & (里程數 >= 起始里程)
				SET @CarRawData_Millage = ISNULL((SELECT top 1 Millage FROM [TB_CarRawData] WITH(NOLOCK)
													WHERE CarNo=@CarNo AND OBDStatus=1
													AND GPSTime <= @NowTime AND GPSTime >= @temp_time 
													AND Millage > 0 AND Millage >= @start_mile
													ORDER BY GPSTime DESC),0);

				select top 1 @WMin = g.w_mins, @HMin = g.h_mins from dbo.FN_GetRangeMins(@Start_Date, @NowTime, 60, 600, 'car', '') g
				SET @TotalDay = IIF(Round((@WMin + @HMin)/600,0)=0, 1, Round((@WMin + @HMin)/600,0));

				IF @NowMileage > @start_mile
				BEGIN
					-- (CarRawData取出來的里程數-起始里程) > 1天1000公里上限時，就把還車里程押上起始里程，暫時當異常狀況處理
					IF ABS(@CarRawData_Millage - @start_mile) > (@TotalDay * 1000)
					BEGIN
						SET @NowMileage = @start_mile;
					END
				END
				ELSE
				BEGIN
					-- CarRawData取出來的里程數 < 起始里程，就把還車里程押上起始里程，暫時當異常狀況處理
					IF @CarRawData_Millage < @start_mile
					BEGIN
						SET @NowMileage = @start_mile;
					END
				END
				-- 因應車機訊號回應資料不穩，增加這段判斷 END

				UPDATE TB_OrderDetail SET final_stop_time=@NowTime,end_mile=@NowMileage WHERE order_number=@OrderNo;

				UPDATE TB_OrderMain SET car_mgt_status=11 
				WHERE IDNO=@IDNO AND order_number=@OrderNo AND car_mgt_status>=4 AND car_mgt_status<15 AND cancel_status=0;

				--寫入歷程
				INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
				VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);

				COMMIT TRAN;
			END
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ReturnCar';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ReturnCar';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用者操作【還車前檢查並記錄還車時間】', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ReturnCar';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ReturnCar';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ReturnCar';