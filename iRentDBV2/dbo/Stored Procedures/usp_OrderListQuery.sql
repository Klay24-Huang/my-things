/****************************************************************
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
		--抓近1個月內未完成的訂單
		SELECT *
		INTO #tmpOrderMain
		FROM TB_OrderMain WITH(NOLOCK) 
		WHERE start_time > DATEADD(MONTH,-1,@NowTime) AND (car_mgt_status >= 4 and car_mgt_status < 16) AND cancel_status=0
		AND stop_time < @NowTime;

		SELECT VW.lend_place AS StationID
			,VW.StationName
			,VW.Tel
			,CT.CityName + AZ.AreaName + VW.ADDR AS ADDR
			,VW.Latitude
			,VW.Longitude
			,VW.Content
			,VW.ContentForAPP
			,VW.IsRequiredForReturn	--據點相關
            ,VW.OperatorName
			,VW.OperatorICon
			,VW.Score	--營運商相關
            ,VW.CarNo
			,VW.CarBrend
			,VW.CarOfArea
			,VW.CarTypeName
			,VW.CarTypeImg
			,VW.Seat
			,IsMotor=ISNULL(VW.IsMotor,0)	--車子相關, 20201006 eason ADD CarNo,IsMotor
            ,VW.device3TBA
			,VW.RemainingMilage						--機車電力相關
            ,VW.ProjType
			,VW.PRONAME								--專案基本資料
            ,IIF(VW.PayMode=0,VW.PRICE/10,VW.PRICE) as PRICE			--平日每小時價 20201003 ADD BY ADAM
            ,IIF(VW.PayMode=0,VW.PRICE_H/10,VW.PRICE_H) as PRICE_H	--假日每小時價 20201003 ADD BY ADAM
            ,VW.BaseMinutes
            ,VW.BaseMinutesPrice
            ,VW.MinuteOfPrice
            ,VW.MaxPrice
            ,VW.MaxPriceH			--當ProjType=4才有值, 20201006 eason ADD MaxPriceH
            ,VW.order_number
            ,VW.start_time
            ,VW.final_start_time
            ,VW.final_stop_time
            ,VW.stop_pick_time
            ,VW.stop_time
            ,VW.init_price
			,Insurance = CASE WHEN VW.ProjType=4 THEN 0 WHEN ISNULL(BU.InsuranceLevel,3) >= 4 THEN 0 ELSE 1 END		--安心服務   20201206改為等級4就是停權
			,InsurancePerHours = CASE WHEN VW.ProjType=4 THEN 0 
									  WHEN K.InsuranceLevel IS NULL THEN II.InsurancePerHours 
									  WHEN K.InsuranceLevel < 4 THEN K.InsurancePerHours 
									  ELSE 0 END		--安心服務每小時價
            ,VW.InsurancePurePrice
            ,VW.init_TransDiscount
            ,VW.car_mgt_status
            ,VW.booking_status
            ,VW.cancel_status
            ,ISNULL(Setting.MilageBase,IIF(VW.ProjType=4,0,-1)) AS MilageUnit
            ,VW.already_lend_car
            ,VW.IsReturnCar
			--20201026 ADD BY ADAM REASON.增加AppStatus
            ,AppStatus = CASE WHEN DATEADD(mi,-30,VW.start_time) > @NowTime AND VW.car_mgt_status=0 THEN 1	--1:尚未到取車時間(取車時間半小時前)
                              WHEN (ISNULL(TOM.CarNo,'') <> '' AND ISNULL(TOM.order_number,0) <> VW.order_number) OR 
								   (DATEADD(mi,-30,VW.start_time) < @NowTime AND @NowTime <= VW.start_time 
                                   AND VW.NowOrderNo>0 AND VW.car_mgt_status=0) THEN 2						--2:立即換車(取車前半小時，前車尚未完成還車)
							  WHEN VW.car_mgt_status=0 AND @NowTime > VW.stop_pick_time THEN 9				--9:未取車
                              WHEN DATEADD(mi,-30,VW.start_time) < @NowTime AND @NowTime <= VW.start_time 
                                   AND VW.car_mgt_status=0    THEN 3										--3:開始使用(取車時間半小時前)
                              WHEN VW.start_time < @NowTime AND @NowTime < VW.stop_pick_time 
                                   AND VW.car_mgt_status=0    THEN 4										--4:開始使用-提示最晚取車時間(取車時間後~最晚取車時間)
                              WHEN VW.car_mgt_status<=11 AND DATEADD(mi,-30,VW.stop_time) > @NowTime 
								   AND VW.car_mgt_status >0	THEN 5											--5:操作車輛(取車後) 取車時間改實際取車時間
                              WHEN VW.car_mgt_status<=11 AND DATEADD(mi,-30,VW.stop_time) < @NowTime THEN 6	--6:操作車輛(準備還車)-
							  WHEN VW.car_mgt_status<=11 AND VW.stop_time < @NowTime THEN 6
							  WHEN VW.car_mgt_status=16 AND DATEADD(mi,15,VW.final_stop_time) > @NowTime 
								   AND OD.nowStatus=0 THEN 7												--7:物品遺漏(再開一次車門)
							  WHEN VW.car_mgt_status=16 AND OD.nowStatus=1 THEN 8							--8:鎖門並還車(一次性開門申請後)
                              ELSE 0 END
			,VW.CarLatitude
			,VW.CarLongitude
			,VW.Area
			,StationPicJson = ISNULL((SELECT [StationPic],[PicDescription] FROM [TB_iRentStationInfo] SI WITH(NOLOCK) WHERE SI.use_flag=1 AND SI.StationID=VW.lend_place FOR JSON PATH),'[]')
			,OD.DeadLine AS OpenDoorDeadLine
			,LOD.parkingSpace AS parkingSpace
        FROM VW_GetOrderData AS VW WITH(NOLOCK)
        LEFT JOIN TB_MilageSetting AS Setting WITH(NOLOCK) ON Setting.ProjID=VW.ProjID AND (VW.start_time BETWEEN Setting.SDate AND Setting.EDate)
		LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=VW.IDNO
		LEFT JOIN TB_InsuranceInfo K WITH(NOLOCK) ON K.CarTypeGroupCode=VW.CarTypeGroupCode AND K.useflg='Y' AND BU.InsuranceLevel=K.InsuranceLevel	
		LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=VW.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3		--預設專用
		LEFT JOIN TB_OpenDoor OD WITH(NOLOCK) ON OD.OrderNo=VW.order_number
		LEFT JOIN TB_City CT WITH(NOLOCK) ON CT.CityID=VW.CityID
		LEFT JOIN TB_AreaZip AZ WITH(NOLOCK) ON AZ.AreaID=VW.AreaID
		LEFT JOIN TB_OrderDetail LOD WITH(NOLOCK) ON LOD.order_number=VW.LastOrderNo
		LEFT JOIN #tmpOrderMain TOM WITH(NOLOCK) ON TOM.CarNo=VW.CarNo
        WHERE VW.IDNO=@IDNO AND VW.cancel_status=0
        AND (VW.car_mgt_status<16    --排除已還車的
			--針對汽機車已還車在15分鐘內的
			OR (VW.car_mgt_status=16 AND VW.final_stop_time is not null AND OD.nowStatus<2 AND DATEADD(mi,15,VW.final_stop_time) > @NowTime)
			)
        AND VW.order_number = CASE WHEN @OrderNo=0 OR @OrderNo=-1 THEN VW.order_number ELSE @OrderNo END
		--20210104 UPD BY JERRY 只查一年內的資料
		AND VW.start_time > DATEADD(MONTH,-3,@NowTime)
        ORDER BY VW.start_time ASC

		DROP TABLE #tmpOrderMain
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