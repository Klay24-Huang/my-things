/****************************************************************
** Name: [dbo].[usp_BE_HandleExtendCar]
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
** EXEC @Error=[dbo].[usp_BE_HandleExtendCar]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/21 下午 02:54:39 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/21 下午 02:54:39    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_HandleExtendCar]
	@OrderNo                BIGINT                ,
	@ExtendTime             DATETIME              ,
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
DECLARE @tmpED    DATETIME;
DECLARE @CarNo                  VARCHAR(10) 
DECLARE @SD       DATETIME;
DECLARE @tmpFineTime DATETIME;
DECLARE @tmpOrderNum BIGINT;
DECLARE @tmpIDNo VARCHAR(20)
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @tmpIDNo    ='';
SET @FunName='usp_BE_HandleExtendCar';
SET @IsSystem=0;
SET @tmpOrderNum=0;
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @Descript=N'後台操作【強迫延長用車】';
SET @CarNo='';
SET @ExtendTime=ISNULL (@ExtendTime,'1900-01-01 00:00:00');
SET @OrderNo=ISNULL (@OrderNo,'');
SET @tmpFineTime='1900-01-01 00:00:00';
		BEGIN TRY
		 IF @ExtendTime='1900-01-01 00:00:00'  OR @OrderNo='' OR @UserID=''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR100'
 		 END
		 
		 IF @Error=0
		 BEGIN
		  BEGIN TRAN
		   SELECT @tmpOrderNum=order_number,@car_mgt_status=car_mgt_status,@cancel_status=cancel_status,@booking_status=booking_status,@CarNo=CarNo,@SD=stop_time,@tmpIDNo=IDNO,@tmpFineTime=ISNULL(fine_Time,'1900-01-01 00:00:00') 
		   FROM TB_OrderMain 
		   WHERE order_number=@OrderNo AND cancel_status=0 AND booking_status<5 AND car_mgt_status>0 AND car_mgt_status<15;
		   IF @tmpOrderNum>0
		   BEGIN
		      DECLARE @tmpCount2 INT;
			  SET @tmpCount2=0
			  SELECT @tmpCount2=COUNT(IDNO) FROM TB_OrderMain 
			  WHERE (CarNo=@CarNo AND order_number<>@OrderNo AND ((car_mgt_status>=4 AND car_mgt_status<15)AND booking_status<5 AND cancel_status=0)) AND  (start_time between @SD AND @ExtendTime OR stop_time between @SD AND @ExtendTime );
			   IF @tmpCount2>0
			   BEGIN
			     SET @Error=1;
			     SET @ErrorCode='ERR726';
				 ROLLBACK TRAN;
			   END
			   ELSE
			   BEGIN
			     SET @tmpCount2=0;
			     SELECT @tmpCount2=COUNT(IDNO) FROM TB_OrderMain 
			     WHERE (IDNO=@tmpIDNo AND order_number<>@OrderNo AND ((car_mgt_status>=4 AND car_mgt_status<15)AND booking_status<5 AND cancel_status=0)) AND  (start_time between @SD AND @ExtendTime OR stop_time between @SD AND @ExtendTime ); 
				 IF @tmpCount2>0
				 BEGIN
				   SET @Error=1;
			     SET @ErrorCode='ERR727';
				 ROLLBACK TRAN;
				 END
			   END
		   END
		   ELSE
		   BEGIN
		      SET @Error=1;
			  SET @ErrorCode='ERR728'
			  ROLLBACK TRAN;
		   END
		 END
		 IF @Error=0
		 BEGIN
		     INSERT TB_OrderExtendHistory(order_number,StopTime,ExtendStopTime,booking_status,isForce)
					SELECT @OrderNo,TB_OrderMain.stop_time,@ExtendTime, TB_OrderMain.booking_status,1
					FROM TB_OrderMain
					WHERE TB_OrderMain.order_number=@OrderNo;

					SELECT @tmpED=stop_time FROM TB_OrderMain WHERE order_number=@OrderNo;
					IF GETDATE()>@tmpED
					BEGIN
					   IF @tmpFineTime='1900-01-01 00:00:00'
					   BEGIN
				        UPDATE TB_OrderMain SET stop_time=@ExtendTime,booking_status=4,fine_time=@tmpED WHERE order_number=@OrderNo AND booking_status<=4 AND (car_mgt_status>=4 AND car_mgt_status<15) AND cancel_status=0
					    INSERT INTO TB_OrderHistory(OrderNum,booking_status,car_mgt_status,cancel_status,Descript)
				        SELECT @OrderNo,booking_status,car_mgt_status,cancel_status,@Descript FROM TB_OrderMain WHERE order_number=@OrderNo;
					   END
					   ELSE
					   BEGIN
					     UPDATE TB_OrderMain SET stop_time=@ExtendTime,booking_status=4 WHERE order_number=@OrderNo AND booking_status<=4 AND (car_mgt_status>=4 AND car_mgt_status<15) AND cancel_status=0
					     INSERT INTO TB_OrderHistory(OrderNum,booking_status,car_mgt_status,cancel_status,Descript)
				         SELECT @OrderNo,booking_status,car_mgt_status,cancel_status,@Descript FROM TB_OrderMain WHERE order_number=@OrderNo;
					   END
					END
					ELSE
					BEGIN
					     UPDATE TB_OrderMain SET stop_time=@ExtendTime,booking_status=4 WHERE order_number=@OrderNo AND booking_status<=4 AND (car_mgt_status>=4 AND car_mgt_status<15) AND cancel_status=0
					    INSERT INTO TB_OrderHistory(OrderNum,booking_status,car_mgt_status,cancel_status,Descript)
				        SELECT @OrderNo,booking_status,car_mgt_status,cancel_status,@Descript FROM TB_OrderMain WHERE order_number=@OrderNo;
					END
					COMMIT TRAN;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleExtendCar';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleExtendCar';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleExtendCar';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleExtendCar';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleExtendCar';