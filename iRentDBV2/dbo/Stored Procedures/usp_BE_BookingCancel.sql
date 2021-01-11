/****************************************************************
** Name: [dbo].[usp_BE_BookingCancel]
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
** EXEC @Error=[dbo].[usp_BE_BookingCancel]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/22 下午 04:01:14 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/22 下午 04:01:14    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_BookingCancel]
	@OrderNo                BIGINT                ,
	@UserID                 NVARCHAR(10)          ,
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
DECLARE @tmpOrderNum TINYINT;
DECLARE @IDNO	VARCHAR(10)
--目前各類型預約數
DECLARE @NormalRentBookingNowCount      TINYINT;
DECLARE @AnyRentBookingNowCount			TINYINT;
DECLARE @MotorRentBookingNowCount		TINYINT;
DECLARE @RentNowActiveType              TINYINT;
DECLARE @NowActiveOrderNum				BIGINT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_BookingCancel';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript='';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @tmpOrderNum=0;
SET @OrderNo=ISNULL (@OrderNo,0);
SET @UserID=ISNULL(@UserID,'');

BEGIN TRY
	IF @UserID=''  OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	--0.再次檢核token
	IF @Error=0
	BEGIN
		SET @Descript=CONCAT(N'後台操作【取消訂單】，操作者【',@UserID,'】');
		BEGIN TRAN
	
		SELECT @tmpOrderNum=COUNT(order_number) 
		FROM TB_OrderMain WITH(NOLOCK) 
		WHERE order_number=@OrderNo AND (car_mgt_status=0 AND cancel_status=0 AND booking_status<3);
		  
		IF @tmpOrderNum=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR129'
			ROLLBACK TRAN;
		END
		ELSE
		BEGIN
			IF @Error=0
			BEGIN
				SELECT @IDNO=IDNO,@ProjType=ProjType FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo

				SELECT @NormalRentBookingNowCount=ISNULL([NormalRentBookingNowCount],0)
					,@AnyRentBookingNowCount=ISNULL([AnyRentBookingNowCount],0)
					,@MotorRentBookingNowCount=ISNULL([MotorRentBookingNowCount],0)
					,@RentNowActiveType=ISNULL(RentNowActiveType,5)
					,@NowActiveOrderNum=ISNULL(NowActiveOrderNum,0)
				FROM [dbo].[TB_BookingStatusOfUser] WITH(NOLOCK)
				WHERE IDNO=@IDNO;
			END

			UPDATE TB_OrderMain SET cancel_status=5 WHERE  order_number=@OrderNo;

			IF @@ROWCOUNT=1
			BEGIN
				COMMIT TRAN;
				SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status,@CarNo=CarNo
				FROM TB_OrderMain WITH(NOLOCK)
				WHERE order_number=@OrderNo;

				INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)
				VALUES(@OrderNo,@cancel_status,@car_mgt_status,@booking_status,@Descript);

				--  UPDATE Car_201607 SET available=1 WHERE car_id=@CarNo;
				-- SELECT @RstationID=return_place,@ProjID=premium FROM TB_BookingMain_201609 WHERE order_number=@OrderNum;

				UPDATE TB_CarInfo 
				SET RentCount=RentCount-1,
					UPDTime=@NowTime 
				WHERE CarNo=@CarNo AND RentCount>0;

				--更新總表
				IF @ProjType=0
				BEGIN
					UPDATE [TB_BookingStatusOfUser] 
					SET [NormalRentBookingNowCount]=[NormalRentBookingNowCount]-1,
						NormalRentBookingCancelCount=NormalRentBookingCancelCount+1,
						UPDTime=@NowTime
					WHERE IDNO=@IDNO AND NormalRentBookingNowCount>0;
				END
				ELSE IF @ProjType=3
				BEGIN
					UPDATE [TB_BookingStatusOfUser] 
					SET [AnyRentBookingNowCount]=0,
						AnyRentBookingCancelCount=AnyRentBookingCancelCount+1,
						UPDTime=@NowTime
					WHERE IDNO=@IDNO AND [AnyRentBookingNowCount]>0;
				END
				ELSE IF @ProjType=4
				BEGIN
					UPDATE [TB_BookingStatusOfUser] 
					SET MotorRentBookingNowCount=0,
						MotorRentBookingCancelCount=MotorRentBookingCancelCount+1,
						UPDTime=@NowTime 
					WHERE IDNO=@IDNO AND MotorRentBookingNowCount>0;
				END
			END
			ELSE
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR132'
				ROLLBACK TRAN;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取消訂單', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_BookingCancel';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_BookingCancel';