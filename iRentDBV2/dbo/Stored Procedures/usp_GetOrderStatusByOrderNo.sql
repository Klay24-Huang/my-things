/****************************************************************
** Name: [dbo].[usp_GetOrderStatusByOrderNo]
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
** EXEC @Error=[dbo].[usp_GetOrderStatusByOrderNo]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/5 下午 01:29:01 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/5 下午 01:29:01    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetOrderStatusByOrderNo]
	@IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
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

DECLARE @NowTime DATETIME;
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetOrderStatusByOrderNo';
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
		 IF @Error=0
		 BEGIN
	 SELECT order_number AS OrderNo,lend_place AS StationID,StationName,Tel,ADDR,Latitude,Longitude,Content --據點相關
			      ,OperatorName,OperatorICon,Score										   --營運商相關
				  ,CarBrend,CarOfArea,CarTypeName,CarTypeImg,Seat,parkingSpace             --車子相關
				  ,WeekdayPrice, HoildayPrice                                              --汽車租金牌價 20201117 eason
				  ,device3TBA,RemainingMilage											   --機車電力相關
				  ,ProjType,PRONAME--,PRICE,PRICE_H										   --專案基本資料
				  ,IIF(PayMode=0,PRICE/10,PRICE) as PRICE								--平日每小時價 20201003 ADD BY ADAM
				  ,IIF(PayMode=0,PRICE_H/10,PRICE_H) as PRICE_H							--假日每小時價 20201003 ADD BY ADAM
				  ,BaseMinutes,BaseMinutesPrice,MinuteOfPrice,MaxPrice					   --當ProjType=4才有值
				  ,start_time,final_start_time,stop_pick_time,stop_time,final_stop_time,ISNULL(fine_Time,'') AS fine_Time
				  ,init_price,Insurance,InsurancePurePrice,init_TransDiscount,car_mgt_status,booking_status,cancel_status
				  ,ISNULL(Setting.MilageBase,IIF(VW.ProjType=4,0,-1)) AS MilageUnit
				  ,already_lend_car,IsReturnCar,CarNo,final_price,start_mile,end_mile
				  ,InsurancePerHours = CASE WHEN VW.ProjType=4 THEN 0 WHEN K.InsuranceLevel IS NULL THEN II.InsurancePerHours WHEN K.InsuranceLevel < 6 THEN K.InsurancePerHours ELSE 0 END
				  ,VW.Insurance				--是否有安心服務
			FROM VW_GetOrderData AS VW 	WITH(NOLOCK)
			LEFT JOIN TB_MilageSetting AS Setting WITH(NOLOCK) ON Setting.ProjID=VW.ProjID AND (VW.start_time BETWEEN Setting.SDate AND Setting.EDate)
			LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=VW.IDNO
			LEFT JOIN (SELECT BU.InsuranceLevel,II.CarTypeGroupCode,II.InsurancePerHours
							FROM TB_BookingInsuranceOfUser BU WITH(NOLOCK)
							LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON BU.IDNO=@IDNO AND ISNULL(BU.InsuranceLevel,3)=II.InsuranceLevel
							WHERE II.useflg='Y') K ON VW.CarTypeGroupCode=K.CarTypeGroupCode
			LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=VW.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3		--預設專用
			
		     WHERE VW.IDNO=@IDNO AND order_number=@OrderNo AND cancel_status=0
			ORDER BY start_time ASC 
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderStatusByOrderNo';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderStatusByOrderNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用訂單編號取得此訂單資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderStatusByOrderNo';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderStatusByOrderNo';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderStatusByOrderNo';