/****************************************************************
** Name: [dbo].[usp_InsCarStatus]
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
** EXEC @Error=[dbo].[usp_InsCarStatus]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/24 下午 02:10:15 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/24 下午 02:10:15    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsCarStatus]
    @MachineNo              VARCHAR(10)           ,
    @OBDStatus              TINYINT               ,
    @GPSStatus              TINYINT               ,
    @GPRSStatus             TINYINT               ,
    @AccON                  TINYINT               ,
    @PowON                  TINYINT               ,
    @LockStatus             VARCHAR(10)               ,
    @LightStatus            TINYINT               ,
    @SecurityStatus         TINYINT               ,
    @CentralLock            TINYINT               ,
    @LowPowStatus           TINYINT               ,
	@DoorStatus             VARCHAR(10)               ,
    @SPEED                  FLOAT                 ,
    @Milage                 FLOAT                 ,
    @Volt                   FLOAT                 ,
    @Lat                    FLOAT                 ,
    @Lng                    FLOAT                 ,
    @GPSTime                DATETIME              ,
	@iButton				TINYINT               ,
	@iButtonKey             VARCHAR(50)			  ,
	@OrderStatus            TINYINT               ,
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
DECLARE @NowTime DATETIME;
DECLARE @CarNo VARCHAR(10);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_InsCarStatus';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @CarNo='';
SET @MachineNo    =ISNULL (@MachineNo    ,'');
SET @NowTime=DATEADD(HOUR,8,GETDATE());
		BEGIN TRY
		  IF @MachineNo='' 
		  BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		 IF @Error=0
		 BEGIN
			SELECT @CarNo=ISNULL(CarNo,'') FROM TB_CarInfo WITH(NOLOCK) WHERE CID=@MachineNo;
			SELECT @hasData=COUNT(1) FROM TB_CarStatus  WITH(NOLOCK) WHERE CID=@MachineNo;

				IF @hasData=0
			BEGIN
				INSERT INTO TB_CarStatus([CarNo],[CID],deviceType,[ACCStatus],[GPSStatus],[GPSTime]
										,[OBDStatus],[GPRSStatus],[PowerOnStatus],[CentralLockStatus],[DoorStatus]
										,[LockStatus],[IndoorLightStatus],[SecurityStatus],[Speed],[Volt]
										,[Latitude],[Longitude],[Millage],[extDeviceStatus2],[extDeviceData3],[extDeviceStatus1]
										,UPDTime
								)VALUES(@CarNo,@MachineNo,0,@AccON,@GPSStatus,@GPSTime
										,@OBDStatus,@GPRSStatus,@PowON,@CentralLock,@DoorStatus
										,@LockStatus,@LightStatus,@SecurityStatus,@SPEED,@Volt
										,@Lat,@Lng,@Milage,@iButton,@iButtonKey,@OrderStatus
										,@NowTime
								)
			END
			ELSE
			BEGIN
				UPDATE TB_CarStatus
				SET  [ACCStatus]=@AccON,[GPSStatus]=@GPSStatus,[GPSTime]=@GPSTime
					,[OBDStatus]=@OBDStatus,[GPRSStatus]=@GPRSStatus,[PowerOnStatus]=@PowON,[CentralLockStatus]=@CentralLock,[DoorStatus]=@DoorStatus
					,[LockStatus]=@LockStatus,[IndoorLightStatus]=@LightStatus,[SecurityStatus]=@SecurityStatus,[Speed]=@SPEED,[Volt]=@Volt
					,[Latitude]=@Lat,[Longitude]=@Lng,[Millage]=@Milage,[extDeviceStatus2]=@iButton,[extDeviceData3]=@iButtonKey,UPDTime=@NowTime
					,[extDeviceStatus1]=@OrderStatus
					,[CarNo]=@CarNo
				WHERE CID=@MachineNo 
					  --AND @GPSTime>[GPSTime]	--20210107因GPSTime回傳資料有異常(2032年)，會造成後續不會更新，所以不比對GPSTime
			END
			--寫入rawdata
					INSERT INTO TB_CarRawData([CID],deviceType,[ACCStatus],[GPSStatus],[GPSTime]
										,[OBDStatus],[GPRSStatus],[PowerOnStatus],[CentralLockStatus],[DoorStatus]
										,[LockStatus],[IndoorLightStatus],[SecurityStatus],[Speed],[Volt]
										,[Latitude],[Longitude],[Millage],[extDeviceStatus2],[extDeviceData3],[extDeviceStatus1]
								)VALUES(@MachineNo,0,@AccON,@GPSStatus,@GPSTime
										,@OBDStatus,@GPRSStatus,@PowON,@CentralLock,@DoorStatus
										,@LockStatus,@LightStatus,@SecurityStatus,@SPEED,@Volt
										,@Lat,@Lng,@Milage,@iButton,@iButtonKey,@OrderStatus
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsCarStatus';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsCarStatus';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'興聯定時回報', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsCarStatus';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsCarStatus';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsCarStatus';