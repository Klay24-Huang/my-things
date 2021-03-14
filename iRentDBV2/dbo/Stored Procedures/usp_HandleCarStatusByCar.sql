/****************************************************************
** Name: [dbo].[usp_HandleCarStatusByCar]
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
** EXEC @Error=[dbo].[usp_HandleCarStatusByCar]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/23 下午 12:44:06 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/23 下午 12:44:06    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_HandleCarStatusByCar]

    @deviceType					INT ,					--車機類型0:汽車;1:機車
    @deviceCID					VARCHAR(10),			--車機編號
    @deviceACCStatus			INT,					--車輛發動狀態，發動為1，熄火為0
    @deviceGPSStatus			INT,					--有效為1，無效為0
    @deviceGPSTime				DATETIME,				--GPS時間（已加八小時）
    @deviceOBDstatus			INT,					--OBD狀態
    @deviceGPRSStatus			INT,					--上線為1，離線為0
    @devicePowerONStatus		INT,					--電源狀態
    @devcieCentralLockStatus	INT,					--中控
    @deviceDoorStatus			VARCHAR(10),			--車門
    @deviceLockStatus			VARCHAR(10),			--車鎖
    @deviceIndoorLightStatus	INT,					--車內燈
    @deviceSecurityStatus		INT,					--防盜鎖
    @deviceSpeed				INT,					--時速
    @deviceVolt					FLOAT,					--電壓
    @deviceLatitude				FLOAT,					--緯度
    @deviceLongitude			FLOAT,					--經度
    @deviceMillage				INT,					--里程數
    @extDeviceStatus1			INT,					--租約狀態
    @extDeviceStatus2			INT,					--汽車專用，iButton扣壓，是為1，否為0
    @extDeviceData2				VARCHAR(512),			--GCP ID
    @extDeviceData3				VARCHAR(512),			--汽車專用，iButton編號
    @extDeviceData4				VARCHAR(512),			--卡號
    @extDeviceData7				VARCHAR(512)		  , --
	@deviceName					VARCHAR(50),
	@LogID						BIGINT                ,
	@ErrorCode 					VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  					NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode			VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg			NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;
DECLARE @CarNo VARCHAR(10);
DECLARE @CID VARCHAR(10);
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_HandleCarStatusByCar';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo		  =ISNULL (@deviceName   ,'');
SET @deviceCID    =ISNULL (@deviceCID    ,'');
SET @CID= '';


		BEGIN TRY
		 IF @deviceCID='' OR @CarNo = ''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
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
				INSERT INTO TB_CarStatus([CarNo],[CID],deviceType,[ACCStatus],[GPSStatus],[GPSTime]
										,[OBDStatus],[GPRSStatus],[PowerOnStatus],[CentralLockStatus],[DoorStatus]
										,[LockStatus],[IndoorLightStatus],[SecurityStatus],[Speed],[Volt]
										,[Latitude],[Longitude],[Millage],[extDeviceStatus1],[extDeviceStatus2]
										,[extDeviceData2],[extDeviceData3],[extDeviceData4],[deviceName]
								)VALUES(@CarNo,@deviceCID,@deviceType,@deviceACCStatus,@deviceGPSStatus,@deviceGPSTime
										,@deviceOBDstatus,@deviceGPRSStatus,@devicePowerONStatus,@devcieCentralLockStatus,@deviceDoorStatus
										,@deviceLockStatus,@deviceIndoorLightStatus,@deviceSecurityStatus,@deviceSpeed,@deviceVolt
										,@deviceLatitude,@deviceLongitude,@deviceMillage,@extDeviceStatus1,@extDeviceStatus2
										,@extDeviceData2,@extDeviceData3,@extDeviceData4,@deviceName
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
						[OBDStatus]=@deviceOBDstatus,
						[GPRSStatus]=@deviceGPRSStatus,
						[PowerOnStatus]=CASE WHEN @deviceOBDstatus = 1 THEN @devicePowerONStatus ELSE PowerOnStatus END,
						[CentralLockStatus]=@devcieCentralLockStatus,
						[DoorStatus]=@deviceDoorStatus,
						[LockStatus]=CASE WHEN @deviceOBDstatus = 1 THEN @deviceLockStatus ELSE LockStatus END,
						[IndoorLightStatus]=@deviceIndoorLightStatus,
						[SecurityStatus]=@deviceSecurityStatus,
						[Speed]=CASE WHEN @deviceOBDstatus = 1 THEN @deviceSpeed ELSE Speed END,
						[Volt]=@deviceVolt,
						[Latitude]=@deviceLatitude,
						[Longitude]=@deviceLongitude,
						[Millage]=CASE WHEN @deviceOBDstatus = 1 AND @deviceMillage > 0 THEN @deviceMillage ELSE Millage END,
						[extDeviceStatus1]=@extDeviceStatus1,
						[extDeviceStatus2]=CASE WHEN @deviceOBDstatus = 1 THEN @extDeviceStatus2 ELSE extDeviceStatus2 END,
						[extDeviceData2]=@extDeviceData2,
						[extDeviceData3]=CASE WHEN @deviceOBDstatus = 1 THEN @extDeviceData3 ELSE extDeviceData3 END,
						[extDeviceData4]=CASE WHEN @deviceOBDstatus = 1 THEN @extDeviceData4 ELSE extDeviceData4 END,
						[deviceName]=@deviceName,
						UPDTime=@NowTime
				WHERE CarNo=@CarNo AND @deviceGPSTime>[GPSTime]
			END
						INSERT INTO TB_CarRawData([CarNo],[CID],deviceType,[ACCStatus],[GPSStatus],[GPSTime]
										,[OBDStatus],[GPRSStatus],[PowerOnStatus],[CentralLockStatus],[DoorStatus]
										,[LockStatus],[IndoorLightStatus],[SecurityStatus],[Speed],[Volt]
										,[Latitude],[Longitude],[Millage],[extDeviceStatus1],[extDeviceStatus2]
										,[extDeviceData2],[extDeviceData3],[extDeviceData4],[deviceName]
								)VALUES(@CarNo,@deviceCID,@deviceType,@deviceACCStatus,@deviceGPSStatus,@deviceGPSTime
										,@deviceOBDstatus,@deviceGPRSStatus,@devicePowerONStatus,@devcieCentralLockStatus,@deviceDoorStatus
										,@deviceLockStatus,@deviceIndoorLightStatus,@deviceSecurityStatus,@deviceSpeed,@deviceVolt
										,@deviceLatitude,@deviceLongitude,@deviceMillage,@extDeviceStatus1,@extDeviceStatus2
										,@extDeviceData2,@extDeviceData3,@extDeviceData4,@deviceName
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleCarStatusByCar';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleCarStatusByCar';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寫入遠傳車機定時回報', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleCarStatusByCar';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleCarStatusByCar';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleCarStatusByCar';