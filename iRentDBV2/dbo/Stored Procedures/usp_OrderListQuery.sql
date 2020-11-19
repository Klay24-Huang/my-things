﻿/****************************************************************
** Name: [dbo].[usp_OrderListQuery]
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
** EXEC @Error=[dbo].[usp_OrderListQuery]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/27 下午 05:30:36 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/27 下午 05:30:36 |	Eric	|          First Release
** 2020/10/3 下午 13:39:00 |	Adam	|  修改汽車的平日每小時價跟假日每小時價
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_OrderListQuery]
	@IDNO                   VARCHAR(10)           ,
	@Token                  VARCHAR(1024)         ,
	@LogID                  BIGINT                ,
	@OrderNo                BIGINT                ,
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
DECLARE @NowTime DATETIME;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_OrderListQuery';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL (@IDNO,'');
SET @Token=ISNULL (@Token,'');
SET @OrderNo=ISNULL (@OrderNo,0);

BEGIN TRY
	IF @Token='' OR @IDNO=''  
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

	--輸出訂單資訊
	IF @Error=0
		BEGIN
			
			SELECT lend_place AS StationID,StationName,Tel,ADDR,Latitude,Longitude,Content            --據點相關
                ,OperatorName,OperatorICon,Score                                                    --營運商相關
                ,CarNo,CarBrend,CarOfArea,CarTypeName,CarTypeImg,Seat,parkingSpace,IsMotor=ISNULL(IsMotor,0)            --車子相關, 20201006 eason ADD CarNo,IsMotor
                ,device3TBA,RemainingMilage                                                            --機車電力相關
                ,ProjType,PRONAME--,PRICE,PRICE_H                                                    --專案基本資料
                ,IIF(PayMode=0,PRICE/10,PRICE) as PRICE                                                --平日每小時價 20201003 ADD BY ADAM
                ,IIF(PayMode=0,PRICE_H/10,PRICE_H) as PRICE_H                                        --假日每小時價 20201003 ADD BY ADAM
                ,BaseMinutes
                ,BaseMinutesPrice
                ,MinuteOfPrice
                ,MaxPrice
                ,MaxPriceH                        --當ProjType=4才有值, 20201006 eason ADD MaxPriceH
                ,order_number
                ,start_time
                ,final_start_time
                ,final_stop_time
                ,stop_pick_time
                ,stop_time
                ,init_price
				,Insurance = CASE WHEN VW.ProjType=4 THEN 0 WHEN ISNULL(BU.InsuranceLevel,3) = 6 THEN 0 ELSE 1 END		--安心服務
				,InsurancePerHours = CASE WHEN VW.ProjType=4 THEN 0 WHEN K.InsuranceLevel IS NULL THEN II.InsurancePerHours WHEN K.InsuranceLevel < 6 THEN K.InsurancePerHours ELSE 0 END		--安心服務每小時價
                ,VW.InsurancePurePrice
                ,init_TransDiscount
                ,car_mgt_status
                ,booking_status
                ,cancel_status
                ,ISNULL(Setting.MilageBase
                ,IIF(VW.ProjType=4,0,-1)) AS MilageUnit
                ,already_lend_car
                ,IsReturnCar
				--20201026 ADD BY ADAM REASON.增加AppStatus
                ,AppStatus = CASE WHEN DATEADD(mi,-30,VW.start_time) > @NowTime AND car_mgt_status=0 THEN 1             --1:尚未到取車時間(取車時間半小時前)
                                  WHEN DATEADD(mi,-30,VW.start_time) < @NowTime AND @NowTime <= VW.start_time 
                                       AND VW.NowOrderNo>0 AND car_mgt_status=0 THEN 2                                  --2:立即換車(取車前半小時，前車尚未完成還車)
								  WHEN car_mgt_status=0 AND @NowTime > VW.stop_pick_time THEN 9							--9:未取車
                                  WHEN DATEADD(mi,-30,VW.start_time) < @NowTime AND @NowTime <= VW.start_time 
                                       AND car_mgt_status=0    THEN 3                                                   --3:開始使用(取車時間半小時前)
                                  WHEN VW.start_time < @NowTime AND @NowTime < VW.stop_pick_time 
                                       AND car_mgt_status=0    THEN 4                                                   --4:開始使用-提示最晚取車時間(取車時間後~最晚取車時間)
                                  WHEN car_mgt_status<=11 AND DATEADD(mi,-30,stop_time) > @NowTime 
										AND car_mgt_status >0	THEN 5													--5:操作車輛(取車後) 取車時間改實際取車時間
                                  WHEN car_mgt_status<=11 AND DATEADD(mi,-30,stop_time) < @NowTime THEN 6                 --6:操作車輛(準備還車)-
								  WHEN car_mgt_status<=11 AND stop_time < @NowTime THEN 6
								  WHEN car_mgt_status=16 AND DATEADD(mi,15,final_stop_time) > @NowTime 
										AND OD.nowStatus=0 THEN 7							--7:物品遺漏(再開一次車門)
								  WHEN car_mgt_status=16 AND OD.nowStatus=1 THEN 8	--8:鎖門並還車(一次性開門申請後)
                                  ELSE 0 END
				,StationPic1 = i1.StationPic
				,StationPic2 = i2.StationPic
				,StationPic3 = i3.StationPic
				,StationPic4 = i4.StationPic
				,[CarLatitude]
				,[CarLongitude]
				,Area
            FROM VW_GetOrderData AS VW WITH(NOLOCK)
            LEFT JOIN TB_MilageSetting AS Setting WITH(NOLOCK) ON Setting.ProjID=VW.ProjID AND (VW.start_time BETWEEN Setting.SDate AND Setting.EDate)

			LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=@IDNO
			LEFT JOIN (SELECT BU.InsuranceLevel,II.CarTypeGroupCode,II.InsurancePerHours
						FROM TB_BookingInsuranceOfUser BU WITH(NOLOCK)
						LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON BU.IDNO=@IDNO AND ISNULL(BU.InsuranceLevel,3)=II.InsuranceLevel
						WHERE II.useflg='Y') K ON VW.CarTypeGroupCode=K.CarTypeGroupCode
			LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=VW.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3		--預設專用
			

			LEFT JOIN (SELECT ROW_NUMBER() OVER(PARTITION BY StationID ORDER BY iRentStationInfoID) as SEQ,StationID,StationPic,PicDescription
						FROM TB_iRentStationInfo i1 WITH(NOLOCK) WHERE use_flag=1) AS I1 ON lend_place=I1.StationID AND I1.SEQ=1
			LEFT JOIN (SELECT ROW_NUMBER() OVER(PARTITION BY StationID ORDER BY iRentStationInfoID) as SEQ,StationID,StationPic,PicDescription
						FROM TB_iRentStationInfo i2 WITH(NOLOCK) WHERE use_flag=1) AS I2 ON lend_place=I2.StationID AND I2.SEQ=2
			LEFT JOIN (SELECT ROW_NUMBER() OVER(PARTITION BY StationID ORDER BY iRentStationInfoID) as SEQ,StationID,StationPic,PicDescription
						FROM TB_iRentStationInfo i3 WITH(NOLOCK) WHERE use_flag=1) AS I3 ON lend_place=I3.StationID AND I3.SEQ=3
			LEFT JOIN (SELECT ROW_NUMBER() OVER(PARTITION BY StationID ORDER BY iRentStationInfoID) as SEQ,StationID,StationPic,PicDescription
						FROM TB_iRentStationInfo i4 WITH(NOLOCK) WHERE use_flag=1) AS I4 ON lend_place=I4.StationID AND I4.SEQ=4
			LEFT JOIN TB_OpenDoor OD WITH(NOLOCK) ON OD.OrderNo=VW.order_number
            WHERE VW.IDNO=@IDNO AND cancel_status=0
            AND (car_mgt_status<16    --排除已還車的
				--針對汽機車已還車在15分鐘內的
				OR (car_mgt_status=16 AND final_stop_time is not null AND OD.nowStatus<2 AND DATEADD(mi,15,final_stop_time) > @NowTime)
				--針對機車已還車在15分鐘內尚未做一次性開門申請
				--OR (car_mgt_status=16 AND final_stop_time is not null AND OD.OpenDoorID IS null AND DATEADD(mi,15,final_stop_time) > @NowTime AND VW.ProjType='4')
				
				)	--還車後15分鐘內
            AND order_number = CASE WHEN @OrderNo=0 OR @OrderNo=-1 THEN order_number ELSE @OrderNo END

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_OrderListQuery';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_OrderListQuery';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_OrderListQuery';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_OrderListQuery';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_OrderListQuery';