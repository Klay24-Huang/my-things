/****************************************************************
** Name: [dbo].[usp_BE_HandleStation]
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
** EXEC @Error=[dbo].[usp_BE_HandleStation]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/6 上午 06:14:51 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/6 上午 06:14:51    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_HandleStation]
    @StationType 			INT,
    @StationID   			VARCHAR(10),
    @StationName 			NVARCHAR(50),
    @ManagerStationID		VARCHAR(10),
    @UniCode 				VARCHAR(20),
    @CityID 				TINYINT,
    @AreaID 				INT,
    @Addr   				NVARCHAR(150),
    @TEL    				VARCHAR(50),
    @Longitude 				DECIMAL,
    @Latitude  				DECIMAL,
    @in_description			NVARCHAR(1024),
    @show_description		NVARCHAR(1024),
    @IsRequired				INT,
    @StationPick			INT,
    @FCode 					VARCHAR(50),
    @SDate 					DateTime,
    @EDate 					DateTime,
    @ParkingNum 			TINYINT,
    @OnlineNum 				INT,
    @Area 					NVARCHAR(10),
    @fileName1 				VARCHAR(150),
    @fileName2 				VARCHAR(150),
    @fileName3 				VARCHAR(150),
    @fileName4 				VARCHAR(150),
    @fileDescript1 			NVARCHAR(1024),
    @fileDescript2 			NVARCHAR(1024),
    @fileDescript3 			NVARCHAR(1024),
    @fileDescript4 			NVARCHAR(1024),
	@UserID					NVARCHAR(10),
    @Mode 					INT,
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
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_HandleStation';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @StationID    =ISNULL (@StationID    ,'');
SET @UserID    =ISNULL (@UserID    ,'');

		BEGIN TRY

		 
		 IF @StationID='' OR @UserID='' 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
				IF @Mode=1
				BEGIN
					SET @hasData=0;
					SELECT @hasData=COUNT(1) FROM TB_iRentStation WHERE StationID=@StationID;
					IF @hasData=0
					BEGIN
						SET @Error=1;
						SET @ErrorCode='ERR900';
					END
				END
				ELSE
				BEGIN
					SET @hasData=0;
					SELECT @hasData=COUNT(1) FROM TB_iRentStation WHERE StationID=@StationID;
					IF @hasData=1
					BEGIN
						SET @Error=1;
						SET @ErrorCode='ERR900';
					END
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'[dbo].[usp_BE_HandleStation]';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'[dbo].[usp_BE_HandleStation]';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'[dbo].[usp_BE_HandleStation]';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'[dbo].[usp_BE_HandleStation]';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'[dbo].[usp_BE_HandleStation]';