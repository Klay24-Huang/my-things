/****************************************************************
** Name: [dbo].[usp_GetBookingStartTime]
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
** EXEC @Error=[dbo].[usp_GetBookingStartTime]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/24 下午 04:36:15 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/24 下午 04:36:15    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetBookingStartTime]
	@IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@Token                  VARCHAR(1024)         ,
	@LogID                  BIGINT                ,
	@SD                     DATETIME        OUTPUT,
	@ED                     DATETIME        OUTPUT,
	@Pure                   DATETIME        OUTPUT,
	@isOldBooking           TINYINT         OUTPUT,
	@CarNo                  VARCHAR(10)     OUTPUT,
	@CarType                VARCHAR(10)     OUTPUT,
	@ProjID                 VARCHAR(10)     OUTPUT,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData BIGINT;
DECLARE @car_mgt_status TINYINT;
DECLARE @cancel_status TINYINT;
DECLARE @booking_status TINYINT;
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;
DECLARE @ProjType INT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetBookingStartTime';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token    =ISNULL (@Token    ,'');

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
		 --1.開始判斷
		 IF @Error=0
		 BEGIN
		    BEGIN TRAN
			  SET @hasData=0;
			  SELECT @hasData=OrderMain.order_number,@SD=final_start_time,@ED=stop_time,@CarNo=OrderMain.CarNo,@CarType=CarType,@ProjID= OrderMain.ProjID 
			  FROM TB_OrderMain AS OrderMain 
			  JOIN TB_Project AS pr ON pr.PROJID = OrderMain.ProjID --eason 2020-11-06
			  LEFT JOIN TB_CarInfo AS CarInfo ON CarInfo.CarNo=OrderMain.CarNo 
			  LEFT JOIN TB_OrderDetail AS OrderDetail ON OrderMain.order_number=OrderDetail.order_number
			  --WHERE booking_status<=3 AND (car_mgt_status>=4 AND car_mgt_status<15) AND cancel_status<3 AND OrderMain.order_number=@OrderNo AND OrderMain.IDNO=@IDNO;
			  WHERE booking_status <=  ( --eason 2020-11-06
			     CASE WHEN  pr.PROJTYPE in (0) THEN 3 --同站可延長再延長
				 WHEN pr.PROJTYPE in (3,4) THEN 2 --路邊,機車不可延長
				 ELSE booking_status END
			  )				  
			  AND (car_mgt_status>=4 AND car_mgt_status<15) AND cancel_status<3 AND OrderMain.order_number=@OrderNo AND OrderMain.IDNO=@IDNO
 			  			  
			  IF @hasData=0
			  BEGIN
			    SET @Error=1;
				SET @ErrorCode='ERR180';
			  END
			  ELSE
			  BEGIN 
			    DECLARE @tmpDate DATETIME;
				SET @tmpDate='1911-01-01 00:00:00'
				SELECT top 1   @tmpDate=ISNULL([StopTime],'1911-01-01 00:00:00')  FROM TB_OrderExtendHistory   where order_number=@OrderNo  and booking_status=0  
				IF @tmpDate<>'1911-01-01 00:00:00'
				BEGIN
				   SET @Pure=@tmpDate;
				END

			  END
			--END
			COMMIT TRAN;
			SET @SD=ISNULL(@SD,'1911-01-01 00:00:00');
			SET @ED=ISNULL(@ED,'1911-01-01 00:00:00');
			SET @Pure=ISNULL(@Pure,@ED);
			SET @CarNo=ISNULL(@CarNo,'');
			SET @CarType=ISNULL(@CarType,'');
			SET @ProjID=ISNULL(@ProjID,'');			
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetBookingStartTime';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetBookingStartTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'延長用車前取得原始資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetBookingStartTime';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetBookingStartTime';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetBookingStartTime';