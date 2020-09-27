/****************************************************************
** Name: [dbo].[usp_BookingCancel]
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
** EXEC @Error=[dbo].[usp_BookingCancel]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/18 下午 02:32:02 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/18 下午 02:32:02    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BookingCancel]
	@IDNO                   VARCHAR(10)           ,
	@OrderNo				BIGINT                ,
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
--目前各類型預約數
DECLARE @NormalRentBookingNowCount      TINYINT;
DECLARE @AnyRentBookingNowCount			TINYINT;
DECLARE @MotorRentBookingNowCount		TINYINT;
DECLARE @RentNowActiveType              TINYINT;
DECLARE @NowActiveOrderNum				BIGINT;
--目前各類型預約數結束
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BookingCancel';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token    =ISNULL (@Token    ,'');
SET @Descript=N'使用者操作【取消訂單】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @NormalRentBookingNowCount  =0;
SET @AnyRentBookingNowCount		=0;
SET @MotorRentBookingNowCount	=0;
SET @RentNowActiveType=5;
SET @NowActiveOrderNum=0;

		BEGIN TRY
		 IF @Token='' OR @IDNO=''  OR @OrderNo=0
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
				 --0.再次檢核token
		 IF @Error=0
		 BEGIN
		 	SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
			ELSE
			BEGIN
			    SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
				IF @hasData=0
				BEGIN
				   SET @Error=1;
				   SET @ErrorCode='ERR101';
				END
			END
		 END
		 IF @Error=0
		 BEGIN
			SELECT @NormalRentBookingNowCount=ISNULL([NormalRentBookingNowCount],0)
			            ,@AnyRentBookingNowCount=ISNULL([AnyRentBookingNowCount],0)
						,@MotorRentBookingNowCount=ISNULL([MotorRentBookingNowCount],0)
						,@RentNowActiveType=ISNULL(RentNowActiveType,5)
						,@NowActiveOrderNum=ISNULL(NowActiveOrderNum,0)
			FROM [dbo].[TB_BookingStatusOfUser]
			WHERE IDNO=@IDNO;
		 END
		 --判斷訂單狀態
		 IF @Error=0
		 BEGIN
			BEGIN TRAN
				SET @hasData=0
				SELECT @hasData=COUNT(order_number)  FROM TB_OrderMain WHERE IDNO=@IDNO AND order_number=@OrderNo AND (car_mgt_status<=3 AND cancel_status=0 AND booking_status<3);
				IF @hasData>0
				BEGIN
					UPDATE TB_OrderMain SET cancel_status=3 WHERE IDNO=@IDNO AND order_number=@OrderNo
					IF @@ROWCOUNT=1
					BEGIN
						COMMIT TRAN;
							SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status,@CarNo=CarNo,@ProjType=ProjType
							FROM TB_OrderMain
							WHERE order_number=@OrderNo;
							INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);
							
							UPDATE TB_CarInfo SET RentCount=RentCount-1 WHERE CarNo=@CarNo AND RentCount>0;

							--更新總表
							IF @ProjType=0
							BEGIN
								UPDATE [TB_BookingStatusOfUser] SET  [NormalRentBookingNowCount]=[NormalRentBookingNowCount]-1,NormalRentBookingCancelCount=NormalRentBookingCancelCount+1 WHERE IDNO=@IDNO AND NormalRentBookingNowCount>0;
							END
							ELSE IF @ProjType=3
							BEGIN
								UPDATE [TB_BookingStatusOfUser] SET  [AnyRentBookingNowCount]=[AnyRentBookingNowCount]-1,AnyRentBookingCancelCount=AnyRentBookingCancelCount+1 WHERE IDNO=@IDNO AND [AnyRentBookingNowCount]>0;
							END
							ELSE IF @ProjType=4
							BEGIN
								UPDATE [TB_BookingStatusOfUser] SET  MotorRentBookingNowCount=MotorRentBookingNowCount-1,MotorRentBookingCancelCount=MotorRentBookingCancelCount+1 WHERE IDNO=@IDNO AND MotorRentBookingNowCount>0;
							END

					END
					ELSE
					BEGIN
						SET @Error=1;
						SET @ErrorCode='ERR169';
						ROLLBACK TRAN;
					END
				END
				ELSE
				BEGIN
					ROLLBACK TRAN;
					SET @Error=1;
					SET @ErrorCode='ERR168';
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取消訂單', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingCancel';