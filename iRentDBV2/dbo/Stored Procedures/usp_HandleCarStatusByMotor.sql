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

SET @FunName='usp_HandleCarStatusByMotor';
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
SET @deviceCID    =ISNULL (@deviceCID    ,'');


		BEGIN TRY
		 IF @deviceCID='' 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		IF @Error=0
		BEGIN
			SELECT @CarNo=ISNULL(CarNo,'') FROM TB_CarInfo WITH(NOLOCK) WHERE CID=@deviceCID;
			SELECT @hasData=COUNT(1) FROM TB_CarStatus  WITH(NOLOCK) WHERE CID=@deviceCID;
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
										,[deviceLowVoltage],[extDeviceStatus1],[extDeviceData5],[extDeviceData6]
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
									   ,@deviceLowVoltage,@extDeviceStatus1,@extDeviceData5,@extDeviceData6
								)
			END
			ELSE
			BEGIN
				UPDATE TB_CarStatus
				SET  [ACCStatus]=@deviceACCStatus,[GPSStatus]=@deviceGPSStatus,[GPSTime]=@deviceGPSTime,[GPRSStatus]=@deviceGPRSStatus,[Speed]=@deviceSpeed
					,[Volt]=@deviceVolt,[Latitude]=@deviceLatitude,[Longitude]=@deviceLongitude,[Millage]=@deviceMillage,[deviceCourse]=@deviceCourse
					,[deviceRPM]=@deviceRPM,[device2TBA]=@device2TBA,[device3TBA]=@device3TBA,[deviceRSOC]=@deviceRSOC,[deviceRDistance]=@deviceRDistance
					,[deviceMBA]=@deviceMBA,[deviceMBAA]=@deviceMBAA,[deviceMBAT_Hi]=@deviceMBAT_Hi,[deviceMBAT_Lo]=@deviceMBAT_Lo,[deviceRBA]=@deviceRBA
					,[deviceRBAA]=@deviceRBAA,[deviceRBAT_Hi]=@deviceRBAT_Hi,[deviceRBAT_Lo]=@deviceRBAT_Lo,[deviceLBA]=@deviceLBA,[deviceLBAA]=@deviceLBAA
					,[deviceLBAT_Hi]=@deviceLBAT_Hi,[deviceLBAT_Lo]=@deviceLBAT_Lo,[deviceTMP]=@deviceTMP,[deviceCur]=@deviceCur,[deviceTPS]=@deviceTPS
					,[deviceVOL]=@deviceiVOL,[deviceErr]=@deviceErr,[deviceALT]=@deviceALT,[deviceGx]=@deviceGx,[deviceGy]=@deviceGy
					,[deviceGz]=@deviceGz,[deviceBLE_Login]=@deviceBLE_Login,[deviceBLE_BroadCast]=@deviceBLE_BroadCast,[devicePwr_Mode]=@devicePwr_Mode,[deviceReversing]=@deviceReversing
					,[devicePut_Down]=@devicePut_Down,[devicePwr_Relay]=@devicePwr_Relay,[deviceStart_OK]=@deviceStart_OK,[deviceHard_ACC]=@deviceHard_ACC,[deviceEMG_Break]=@deviceEMG_Break
					,[deviceSharp_Turn]=@deviceSharp_Turn,[deviceBat_Cover]=@deviceBat_Cover,[deviceLowVoltage]=@deviceLowVoltage,[extDeviceStatus1]=@extDeviceStatus1
					--,[extDeviceData5]=@extDeviceData5,[extDeviceData6]=@extDeviceData6	--ReportNow不會回傳[extDeviceData5],[extDeviceData6]，所以不作更新
					,UPDTime=@NowTime
				WHERE CID=@deviceCID AND @deviceGPSTime>[GPSTime]
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
										,[deviceLowVoltage],[extDeviceStatus1],[extDeviceData5],[extDeviceData6]
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
									   ,@deviceLowVoltage,@extDeviceStatus1,@extDeviceData5,@extDeviceData6
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