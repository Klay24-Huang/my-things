/****************************************************************
** Name: [dbo].[usp_BE_ContactFinish]
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
** EXEC @Error=[dbo].[usp_BE_ContactFinish]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/30 上午 05:54:10 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/30 上午 05:54:10    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_ContactFinish]
    @IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@UserID                 NVARCHAR(10)          ,
	@transaction_no         NVARCHAR(100)         , --金流交易序號，免付費使用Free
	@ReturnDate             DATETIME              , --強還時間
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
DECLARE @ParkingSpace NVARCHAR(256);
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_ContactFinish';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'後台強還，設定狀態為已還車';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @UserID    =ISNULL (@UserID    ,'');
SET @ParkingSpace='';

		BEGIN TRY
		
		 
		 IF @UserID='' OR @IDNO=''  OR @OrderNo=0
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 

		 IF @Error=0
		 BEGIN
		            SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status,@CarNo=CarNo,@ProjType=ProjType
					FROM TB_OrderMain
					WHERE order_number=@OrderNo;

					SELECT @hasData=COUNT(1) FROM TB_ParkingSpaceTmp WHERE OrderNo=@OrderNo;
					IF @hasData>0
					BEGIN
					   INSERT INTO [dbo].[TB_ParkingSpace]([OrderNo],[ParkingImage],[ParkingSpace])
					   SELECT [OrderNo],[ParkingImage],[ParkingSpace] FROM TB_ParkingSpaceTmp WHERE OrderNo=@OrderNo;

					   DELETE FROM TB_ParkingSpaceTmp WHERE OrderNo=@OrderNo;
					   SELECT @ParkingSpace=ISNULL([ParkingSpace],'') FROM [TB_ParkingSpace] WHERE OrderNo=@OrderNo;
					END

					--寫入歷程
					INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);
					
					--更新訂單主檔
					UPDATE TB_OrderMain
					SET booking_status=5,car_mgt_status=16,modified_status=2
					WHERE order_number=@OrderNo;
					
					--更新訂單明細
					IF @transaction_no='Free'
					BEGIN
						UPDATE TB_OrderDetail
						SET transaction_no=@transaction_no,trade_status=1,[already_return_car]=1,[already_payment]=1,final_stop_time=@ReturnDate
						WHERE order_number=@OrderNo;
					END
					ELSE
					BEGIN
						UPDATE TB_OrderDetail
						--SET [already_return_car]=1,[already_payment]=1
						--20201110 UPD BY JERRY 不管何種狀態都要更新final_stop_time
						SET [already_return_car]=1,[already_payment]=1,final_stop_time=@ReturnDate
						WHERE order_number=@OrderNo;
					END
					--20201010 ADD BY ADAM REASON.還車改為只針對個人訂單狀態去個別處理
					--更新個人訂單控制
					IF @ProjType=4
					BEGIN
						UPDATE [TB_BookingStatusOfUser]
						SET [MotorRentBookingNowCount]=[MotorRentBookingNowCount]-1,RentNowActiveType=5,NowActiveOrderNum=0,[MotorRentBookingFinishCount]=[MotorRentBookingFinishCount]+1
						WHERE IDNO=@IDNO;

						INSERT INTO TB_OrderDataByMotor(OrderNo,R_lat,R_lon,R_LBA,R_RBA,R_MBA,R_TBA)
						SELECT @OrderNo,Latitude,Longitude,deviceLBA,deviceRBA,deviceMBA,device3TBA FROM TB_CarStatus WHERE CarNo=@CarNo;

					END
					ELSE IF @ProjType=0
					BEGIN
						UPDATE [TB_BookingStatusOfUser]
						SET [NormalRentBookingNowCount]=[NormalRentBookingNowCount]-1,RentNowActiveType=5,NowActiveOrderNum=0,[NormalRentBookingFinishCount]=[NormalRentBookingFinishCount]+1
						WHERE IDNO=@IDNO;
					END
					ELSE
					BEGIN
						UPDATE [TB_BookingStatusOfUser]
						SET [AnyRentBookingNowCount]=[AnyRentBookingNowCount]-1,RentNowActiveType=5,NowActiveOrderNum=0,[AnyRentBookingFinishCount]=[AnyRentBookingFinishCount]+1
						WHERE IDNO=@IDNO;
					END

					--更新車輛
					UPDATE TB_Car
					SET [NowOrderNo]=0,[LastOrderNo]=@OrderNo,available=1
					WHERE CarNo=@CarNo;
					--寫入歷程
					SET @Descript=N'後台強還，完成還車';
					INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);

					--寫入一次性開門的deadline
					--INSERT INTO TB_OpenDoor(OrderNo,DeadLine)VALUES(@OrderNo,DATEADD(MINUTE,15,@NowTime));

					

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ContactFinish';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ContactFinish';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款完成', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ContactFinish';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ContactFinish';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ContactFinish';