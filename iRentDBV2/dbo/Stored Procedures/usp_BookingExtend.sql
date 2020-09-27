/****************************************************************
** Name: [dbo].[usp_BookingExtend]
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
** EXEC @Error=[dbo].[usp_BookingExtend]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/24 下午 04:36:35 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/24 下午 04:36:35    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BookingExtend]
	@IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@Token                  VARCHAR(1024)         ,
	@SD                     DATETIME              ,
	@ED                     DATETIME              ,
	@CarNo                  VARCHAR(10)           ,
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
DECLARE @ProjType INT;
DECLARE @tmpIDNo          VARCHAR(10);
DECLARE @tmpOrderNum      BIGINT;
DECLARE @tmpED    DATETIME;
DECLARE @tmpFineTime DATETIME;
DECLARE @tmpCount INT;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BookingExtend';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【延長用車】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @tmpIDNo    ='';
SET @tmpOrderNum=0;
SET @tmpFineTime='1900-01-01 00:00:00';
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token    =ISNULL (@Token    ,'');
SET @SD=ISNULL(@SD,'1900-01-01 00:00:00');
SET @ED=ISNULL(@ED,'1900-01-01 00:00:00');
SET @tmpCount=0;

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
		 --先取得資料
		 IF @Error=0
		 BEGIN

			SELECT @tmpOrderNum=order_number,@tmpIDNo=IDNO,@car_mgt_status=car_mgt_status,@cancel_status=cancel_status,@booking_status=booking_status,@tmpFineTime=ISNULL(fine_Time,'1900-01-01 00:00:00'),@ProjType=ProjType FROM TB_OrderMain WHERE order_number=@OrderNo;
		 END
		 --開始做延長
		  IF @Error=0
		 BEGIN
		   BEGIN TRAN
		   SET @tmpCount=0
			  SELECT @tmpCount=COUNT(IDNO) FROM TB_OrderMain 
			  WHERE (CarNo=@CarNo AND order_number<>@OrderNo AND (cancel_status=0 and car_mgt_status=0))AND  
			  (
			                (start_time between @SD AND @ED) 
						    OR (stop_time between @SD AND @ED)
							OR (@SD BETWEEN start_time AND stop_time)
							OR (@ED BETWEEN start_time AND stop_time)
						    OR (DATEADD(MINUTE,-30,@SD) between start_time AND stop_time)
						    OR (DATEADD(MINUTE,30,@ED) between start_time AND stop_time)
			  );
			  --WHERE (assigned_car_id=@CarNo AND order_number<>@OrderNum AND ((car_mgt_status>=4 AND car_mgt_status<15)AND booking_status<5 AND cancel_status=0)) AND  (start_time between @SD AND @ED OR stop_time between @SD AND @ED );
			   IF @tmpCount>0
			   BEGIN
			     SET @Error=1;
			     SET @ErrorCode='ERR181';
				 ROLLBACK TRAN;
			   END
			   ELSE
			   BEGIN
			     SET @tmpCount=0;
			     SELECT @tmpCount=COUNT(IDNO) FROM TB_OrderMain 
			     WHERE (IDNO=@IDNO AND order_number<>@OrderNo AND ((car_mgt_status>=4 AND car_mgt_status<15)AND booking_status<5 AND cancel_status<2)) AND  (start_time between @SD AND @ED OR stop_time between @SD AND @ED ) AND (ProjType<>4 AND ProjType=@ProjType); 
				 IF @tmpCount>0
				 BEGIN
				   SET @Error=1;
			       SET @ErrorCode='ERR182';
				   ROLLBACK TRAN;
				 END
			   END

           	IF @Error=0
		    BEGIN
		    INSERT TB_OrderExtendHistory (order_number,StopTime,ExtendStopTime,booking_status)
					SELECT @OrderNo,stop_time,@ED,booking_status
					FROM TB_OrderMain
					WITH(NOLOCK) WHERE order_number=@OrderNo
					

					SELECT @tmpED=stop_time FROM TB_OrderMain WHERE order_number=@OrderNo;
					IF @NowTime>@tmpED
					BEGIN
						IF @tmpFineTime='1900-01-01 00:00:00'
						BEGIN
						  UPDATE TB_OrderMain SET stop_time=@ED,booking_status=3,fine_time=@tmpED WHERE order_number=@OrderNo AND booking_status<4 AND (car_mgt_status>=4 AND car_mgt_status<15) AND cancel_status<3
						   INSERT INTO TB_OrderHistory(OrderNum,booking_status,car_mgt_status,cancel_status,Descript)
						   SELECT @OrderNo,booking_status,car_mgt_status,cancel_status,@Descript FROM TB_OrderMain WHERE order_number=@OrderNo;
						END
						ELSE
						BEGIN
						   UPDATE TB_OrderMain SET stop_time=@ED,booking_status=3  WHERE order_number=@OrderNo AND booking_status<4 AND (car_mgt_status>=4 AND car_mgt_status<15) AND cancel_status<3
						   INSERT INTO TB_OrderHistory(OrderNum,booking_status,car_mgt_status,cancel_status,Descript)
						   SELECT @OrderNo,booking_status,car_mgt_status,cancel_status,@Descript FROM TB_OrderMain WHERE order_number=@OrderNo;
						END
					END
					ELSE
					BEGIN
					     UPDATE TB_OrderMain SET stop_time=@ED,booking_status=3 WHERE order_number=@OrderNo AND booking_status<4 AND (car_mgt_status>=4 AND car_mgt_status<15) AND cancel_status<3
							INSERT INTO TB_OrderHistory(OrderNum,booking_status,car_mgt_status,cancel_status,Descript)
							SELECT @OrderNo,booking_status,car_mgt_status,cancel_status,@Descript FROM TB_OrderMain WHERE order_number=@OrderNo;
					END
					COMMIT TRAN;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingExtend';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingExtend';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'延長用車', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingExtend';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingExtend';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingExtend';