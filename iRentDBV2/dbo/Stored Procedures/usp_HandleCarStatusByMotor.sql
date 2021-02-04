/****************************************************************
** Name: [dbo].[usp_HandleCarStatusByMotor]
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
** EXEC @Error=[dbo].[usp_HandleCarStatusByMotor]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/23 下午 01:52:30 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/23 下午 01:52:30    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_HandleCarStatusByMotor]
    @deviceCID  VARCHAR(10),--
    @deviceType INT, --
    @deviceACCStatus   INT, --
    @deviceGPSStatus   INT, --
    @deviceGPRSStatus  INT, --
    @deviceGPSTime VARCHAR(20),
    @deviceSpeed  INT, --
    @deviceVolt      FLOAT,--
    @deviceLatitude  FLOAT,--
    @deviceLongitude FLOAT,--
    @deviceMillage   FLOAT,--
    @deviceCourse    FLOAT,--
    @deviceRPM   INT, --
    @deviceiSpeed   INT, --
    @device2TBA FLOAT,--
    @device3TBA FLOAT,--
    @deviceRSOC VARCHAR(10),
    @deviceRDistance VARCHAR(10),
    @deviceMBA FLOAT,--
    @deviceMBAA   FLOAT, --
    @deviceMBAT_Hi   FLOAT, --
    @deviceMBAT_Lo   FLOAT, --
    @deviceRBA FLOAT,
    @deviceRBAA   FLOAT, --
    @deviceRBAT_Hi   FLOAT, --
    @deviceRBAT_Lo   FLOAT, --
    @deviceLBA FLOAT,--
    @deviceLBAA   FLOAT, --
    @deviceLBAT_Hi   FLOAT, --
    @deviceLBAT_Lo   FLOAT, --
    @deviceTMP    FLOAT, --
    @deviceCur    FLOAT, --
    @deviceTPS    FLOAT, --
    @deviceiVOL   FLOAT, --
    @deviceErr   FLOAT, --
    @deviceALT   FLOAT, --
    @deviceGx FLOAT,--
    @deviceGy FLOAT,--
    @deviceGz FLOAT,--
    @deviceBLE_Login   INT, --
    @deviceBLE_BroadCast   INT, --
    @devicePwr_Mode   INT, --
    @deviceReversing   INT, --
    @devicePut_Down   INT, --
    @devicePwr_Relay   INT, --
    @deviceStart_OK   INT, --
    @deviceHard_ACC   INT, --
    @deviceEMG_Break   INT, --
    @deviceSharp_Turn   INT, --
    @deviceBat_Cover   INT, --
    @deviceLowVoltage   INT, --
    @extDeviceStatus1   INT, --
    @extDeviceData5 VARCHAR(128),--
    @extDeviceData6 VARCHAR(256),--
	@deviceName	VARCHAR(50),
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
DECLARE @CID VARCHAR(10);
DECLARE @deviceMBA_Cal FLOAT
DECLARE @deviceRBA_Cal FLOAT
DECLARE @deviceLBA_Cal FLOAT
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_HandleCarStatusByMotor';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo		  =ISNULL (@deviceName   ,'');
SET @ProjType=5;
SET @deviceCID    =ISNULL (@deviceCID    ,'');
SET @CID= '';
SET @deviceMBA_Cal = 0
SET @deviceRBA_Cal = 0
SET @deviceLBA_Cal = 0


		BEGIN TRY
		 IF @deviceCID='' OR @CarNo = ''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		--20201124 UPD BY Jerry	增加預估里程計算
		IF @deviceRDistance='NA'
		BEGIN
			SET @deviceMBA_Cal = IIF(@deviceMBA < 0, 0, @deviceMBA)
			SET @deviceRBA_Cal = IIF(@deviceRBA < 0, 0, @deviceRBA)
			SET @deviceLBA_Cal = IIF(@deviceLBA < 0, 0, @deviceLBA)
			IF @deviceMBA_Cal>0 or @deviceLBA_Cal>0 or @deviceRBA_Cal>0
				SET @deviceRDistance=Round((@deviceMBA_Cal*9600+@deviceLBA_Cal*12000+@deviceRBA_Cal*12000)/(CASE @deviceMBA_Cal WHEN 0 THEN 0 ELSE 9600 END+CASE @deviceLBA_Cal WHEN 0 THEN 0 ELSE 12000 END+CASE @deviceRBA_Cal WHEN 0 THEN 0 ELSE 12000 END)*0.45,1);
			ELSE
				SET @deviceRDistance=0;
		END

		IF @Error=0
		BEGIN
			--SELECT @CarNo=ISNULL(CarNo,'') FROM TB_CarInfo WITH(NOLOCK) WHERE CID=@deviceCID;
			--20201214 車機維修後，CID可能會變更
			SELECT @CID=ISNULL(CID,'') FROM TB_CarInfo WITH(NOLOCK) WHERE CarNo=@CarNo;
			IF @deviceCID <> @CID
			BEGIN
				UPDATE TB_CarInfo SET CID=@deviceCID WHERE CarNo=@CarNo;
			END
			--20210128 CID有多筆時，將未對應的車號CID清空
			SELECT @hasData=COUNT(1) FROM TB_CarInfo WITH(NOLOCK) WHERE CID=@deviceCID;
			IF @hasData>1
			BEGIN
				UPDATE TB_CarInfo SET CID='' WHERE CID=@deviceCID AND CarNo<>@CarNo;
			END
			SELECT @hasData=COUNT(1) FROM TB_CarStatus  WITH(NOLOCK) WHERE CarNo=@CarNo;
			IF @hasData=0
			BEGIN
				INSERT INTO TB_CarStatus([CarNo],[CID],[deviceType],[ACCStatus],[GPSStatus]
										,[GPSTime],[GPRSStatus],[Speed],[Volt],[Latitude]
										,[Longitude],[Millage] ,[deviceCourse],[deviceRPM],[device2TBA]
										,[device3TBA],[deviceRSOC],[deviceRDistance],[deviceMBA],[deviceMBAA]
										,[deviceMBAT_Hi],[deviceMBAT_Lo],[deviceRBA],[deviceRBAA],[deviceRBAT_Hi]
										,[deviceRBAT_Lo],[deviceLBA],[deviceLBAA],[deviceLBAT_Hi],[deviceLBAT_Lo]
										,[deviceTMP],[deviceCur],[deviceTPS],[deviceVOL],[deviceErr]
										,[deviceALT],[deviceGx],[deviceGy],[deviceGz],[deviceBLE_Login]
										,[deviceBLE_BroadCast],[devicePwr_Mode],[deviceReversing],[devicePut_Down],[devicePwr_Relay]
										,[deviceStart_OK],[deviceHard_ACC],[deviceEMG_Break],[deviceSharp_Turn],[deviceBat_Cover]
										,[deviceLowVoltage],[extDeviceStatus1],[extDeviceData5],[extDeviceData6],[deviceName]
								)VALUES(@CarNo,@deviceCID,@deviceType,@deviceACCStatus,@deviceGPSStatus
								       ,@deviceGPSTime,@deviceGPRSStatus,@deviceSpeed,@deviceVolt,@deviceLatitude
									   ,@deviceLongitude,@deviceMillage,@deviceCourse,@deviceRPM,@device2TBA
									   ,@device3TBA,@deviceRSOC,@deviceRDistance,@deviceMBA,@deviceMBAA
									   ,@deviceMBAT_Hi,@deviceMBAT_Lo,@deviceRBA,@deviceRBAA,@deviceRBAT_Hi
									   ,@deviceRBAT_Lo,@deviceLBA,@deviceLBAA,@deviceLBAT_Hi,@deviceLBAT_Lo
									   ,@deviceTMP,@deviceCur,@deviceTPS,@deviceiVOL,@deviceErr
									   ,@deviceALT,@deviceGx,@deviceGy,@deviceGz,@deviceBLE_Login
									   ,@deviceBLE_BroadCast,@devicePwr_Mode,@deviceReversing,@devicePut_Down,@devicePwr_Relay
									   ,@deviceStart_OK,@deviceHard_ACC,@deviceEMG_Break,@deviceSharp_Turn,@deviceBat_Cover
									   ,@deviceLowVoltage,@extDeviceStatus1,@extDeviceData5,@extDeviceData6,@deviceName
								)
			END
			ELSE
			BEGIN
				--20210128 CID多筆時，將未對應的車號刪除
				SELECT @hasData=COUNT(1) FROM TB_CarStatus WITH(NOLOCK) WHERE CID=@deviceCID;
				IF @hasData>1
				BEGIN
					DELETE FROM TB_CarStatus WHERE CID=@deviceCID AND CarNo<>@CarNo;
				END
				UPDATE TB_CarStatus
				SET  [CID]=@deviceCID,
						[ACCStatus]=@deviceACCStatus,
						[GPSStatus]=@deviceGPSStatus,
						[GPSTime]=@deviceGPSTime,
						[GPRSStatus]=@deviceGPRSStatus,
						[Speed]=@deviceSpeed,
						[Volt]=@deviceVolt,
						[Latitude]=@deviceLatitude,
						[Longitude]=@deviceLongitude,
						[Millage]= CASE WHEN @deviceMillage IS NOT NULL AND @deviceMillage > 0 THEN @deviceMillage ELSE Millage END,
						[deviceCourse]=@deviceCourse,
						[deviceRPM]= CASE WHEN @deviceRPM IS NOT NULL THEN @deviceRPM ELSE deviceRPM END,
						[device2TBA]= CASE WHEN @device2TBA IS NOT NULL AND @deviceMillage > 0 THEN @device2TBA ELSE device2TBA END,
						[device3TBA]= CASE WHEN @device3TBA IS NOT NULL AND @deviceMillage > 0 THEN @device3TBA ELSE device3TBA END,
						[deviceRSOC]= CASE WHEN @deviceRSOC IS NOT NULL AND @deviceMillage > 0 THEN @deviceRSOC ELSE deviceRSOC END,
						[deviceRDistance]= CASE WHEN @deviceRDistance IS NOT NULL THEN @deviceRDistance ELSE deviceRDistance END,
						[deviceMBA]= CASE WHEN @deviceMBA IS NOT NULL AND @deviceMillage > 0 THEN @deviceMBA ELSE deviceMBA END,
						[deviceMBAA]= CASE WHEN @deviceMBAA IS NOT NULL THEN @deviceMBAA ELSE deviceMBAA END,
						[deviceMBAT_Hi]= CASE WHEN @deviceMBAT_Hi IS NOT NULL THEN @deviceMBAT_Hi ELSE deviceMBAT_Hi END,
						[deviceMBAT_Lo]= CASE WHEN @deviceMBAT_Lo IS NOT NULL THEN @deviceMBAT_Lo ELSE deviceMBAT_Lo END,
						[deviceRBA]= CASE WHEN @deviceRBA IS NOT NULL AND @deviceMillage > 0 THEN @deviceRBA ELSE deviceRBA END,
						[deviceRBAA]= CASE WHEN @deviceRBAA IS NOT NULL THEN @deviceRBAA ELSE deviceRBAA END,
						[deviceRBAT_Hi]= CASE WHEN @deviceRBAT_Hi IS NOT NULL THEN @deviceRBAT_Hi ELSE deviceRBAT_Hi END,
						[deviceRBAT_Lo]= CASE WHEN @deviceRBAT_Lo IS NOT NULL THEN @deviceRBAT_Lo ELSE deviceRBAT_Lo END,
						[deviceLBA]= CASE WHEN @deviceLBA IS NOT NULL AND @deviceMillage > 0 THEN @deviceLBA ELSE deviceLBA END,
						[deviceLBAA]= CASE WHEN @deviceLBAA IS NOT NULL THEN @deviceLBAA ELSE deviceLBAA END,
						[deviceLBAT_Hi]= CASE WHEN @deviceLBAT_Hi IS NOT NULL THEN @deviceLBAT_Hi ELSE deviceLBAT_Hi END,
						[deviceLBAT_Lo]= CASE WHEN @deviceLBAT_Lo IS NOT NULL THEN @deviceLBAT_Lo ELSE deviceLBAT_Lo END,
						[deviceTMP]= CASE WHEN @deviceTMP IS NOT NULL THEN @deviceTMP ELSE deviceTMP END,
						[deviceCur]= CASE WHEN @deviceCur IS NOT NULL THEN @deviceCur ELSE deviceCur END,
						[deviceTPS]= CASE WHEN @deviceTPS IS NOT NULL THEN @deviceTPS ELSE deviceTPS END,
						[deviceVOL]= CASE WHEN @deviceiVOL IS NOT NULL THEN @deviceiVOL ELSE deviceVOL END,
						[deviceErr]= CASE WHEN @deviceErr IS NOT NULL THEN @deviceErr ELSE deviceErr END,
						[deviceALT]= CASE WHEN @deviceALT IS NOT NULL THEN @deviceALT ELSE deviceALT END,
						[deviceGx]= CASE WHEN @deviceGx IS NOT NULL THEN @deviceGx ELSE deviceGx END,
						[deviceGy]= CASE WHEN @deviceGy IS NOT NULL THEN @deviceGy ELSE deviceGy END,
						[deviceGz]= CASE WHEN @deviceGz IS NOT NULL THEN @deviceGz ELSE deviceGz END,
						[deviceBLE_Login]=@deviceBLE_Login,
						[deviceBLE_BroadCast]=@deviceBLE_BroadCast,
						[devicePwr_Mode]=@devicePwr_Mode,
						[deviceReversing]=@deviceReversing,
						[devicePut_Down]=@devicePut_Down,
						[devicePwr_Relay]=@devicePwr_Relay,
						[deviceStart_OK]=@deviceStart_OK,
						[deviceHard_ACC]=@deviceHard_ACC,
						[deviceEMG_Break]=@deviceEMG_Break,
						[deviceSharp_Turn]=@deviceSharp_Turn,
						[deviceBat_Cover]=@deviceBat_Cover,
						[deviceLowVoltage]=@deviceLowVoltage,
						[extDeviceStatus1]=@extDeviceStatus1,
					--,[extDeviceData5]=@extDeviceData5,[extDeviceData6]=@extDeviceData6	--ReportNow不會回傳[extDeviceData5],[extDeviceData6]，所以不作更新
						[deviceName]=@deviceName
					,UPDTime=@NowTime
				WHERE CarNo=@CarNo AND @deviceGPSTime>[GPSTime]
			END
			INSERT INTO TB_CarRawData([CarNo],[CID],[deviceType],[ACCStatus],[GPSStatus]
										,[GPSTime],[GPRSStatus],[Speed],[Volt],[Latitude]
										,[Longitude],[Millage] ,[deviceCourse],[deviceRPM],[device2TBA]
										,[device3TBA],[deviceRSOC],[deviceRDistance],[deviceMBA],[deviceMBAA]
										,[deviceMBAT_Hi],[deviceMBAT_Lo],[deviceRBA],[deviceRBAA],[deviceRBAT_Hi]
										,[deviceRBAT_Lo],[deviceLBA],[deviceLBAA],[deviceLBAT_Hi],[deviceLBAT_Lo]
										,[deviceTMP],[deviceCur],[deviceTPS],[deviceVOL],[deviceErr]
										,[deviceALT],[deviceGx],[deviceGy],[deviceGz],[deviceBLE_Login]
										,[deviceBLE_BroadCast],[devicePwr_Mode],[deviceReversing],[devicePut_Down],[devicePwr_Relay]
										,[deviceStart_OK],[deviceHard_ACC],[deviceEMG_Break],[deviceSharp_Turn],[deviceBat_Cover]
										,[deviceLowVoltage],[extDeviceStatus1],[extDeviceData5],[extDeviceData6],[deviceName]
								)VALUES(@CarNo,@deviceCID,@deviceType,@deviceACCStatus,@deviceGPSStatus
								       ,@deviceGPSTime,@deviceGPRSStatus,@deviceSpeed,@deviceVolt,@deviceLatitude
									   ,@deviceLongitude,@deviceMillage,@deviceCourse,@deviceRPM,@device2TBA
									   ,@device3TBA,@deviceRSOC,@deviceRDistance,@deviceMBA,@deviceMBAA
									   ,@deviceMBAT_Hi,@deviceMBAT_Lo,@deviceRBA,@deviceRBAA,@deviceRBAT_Hi
									   ,@deviceRBAT_Lo,@deviceLBA,@deviceLBAA,@deviceLBAT_Hi,@deviceLBAT_Lo
									   ,@deviceTMP,@deviceCur,@deviceTPS,@deviceiVOL,@deviceErr
									   ,@deviceALT,@deviceGx,@deviceGy,@deviceGz,@deviceBLE_Login
									   ,@deviceBLE_BroadCast,@devicePwr_Mode,@deviceReversing,@devicePut_Down,@devicePwr_Relay
									   ,@deviceStart_OK,@deviceHard_ACC,@deviceEMG_Break,@deviceSharp_Turn,@deviceBat_Cover
									   ,@deviceLowVoltage,@extDeviceStatus1,@extDeviceData5,@extDeviceData6,@deviceName
								)
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleCarStatusByMotor';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleCarStatusByMotor';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寫入遠傳車機定時回報', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleCarStatusByMotor';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleCarStatusByMotor';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleCarStatusByMotor';