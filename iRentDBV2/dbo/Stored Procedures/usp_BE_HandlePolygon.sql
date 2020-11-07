/****************************************************************
** Name: [dbo].[BE_HandlePolygon]
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
** EXEC @Error=[dbo].[BE_HandlePolygon]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/7 下午 04:53:29 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/7 下午 04:53:29    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_HandlePolygon]
	@StationID              VARCHAR(10)           ,
	@BlockID                BIGINT                ,
	@BlockName              NVARCHAR(100)         ,
	@MAPColor               VARCHAR(20)           ,
	@Longitude				VARCHAR(MAX)          ,
	@Latitude				VARCHAR(MAX)          ,
	@StartDate              DATETIME              ,
	@EndDate                DATETIME              ,
	@Mode                   INT                   ,
	@UserID                 NVARCHAR(10)          ,
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

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_HandlePolygon';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=DATEADD(HOUR,8,GETDATE());

SET @StationID	=ISNULL(@StationID,'');
SET @BlockID  	=ISNULL(@BlockID  ,0);
SET @BlockName	=ISNULL(@BlockName,'');
SET @MAPColor 	=ISNULL(@MAPColor ,'');
SET @Longitude	=ISNULL(@Longitude,'');
SET @Latitude	=ISNULL(@Latitude,'');
SET @UserID   	=ISNULL(@UserID   ,'');
SET @Mode       =ISNULL(@Mode,-1);
SET @StartDate =ISNULL(@StartDate,@NowTime);
SET @EndDate   =ISNULL(@EndDate  ,'2099-12-31 23:59:59');
		BEGIN TRY

		 
		 IF @UserID='' OR @StationID=''  OR @BlockName='' OR @Mode<0
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
			IF @BlockID=0
			BEGIN
				SET @hasData=0;
		 		SELECT @hasData=COUNT(1) FROM TB_Polygon WHERE StationID=@StationID AND BlockType=@Mode;
				IF @hasData>0
				BEGIN
					--先寫入舊資料到歷史
					INSERT INTO TB_PolygonHistory(StationID,BLOCK_ID,BlockName,MAPColor,Longitude,Latitude,BlockType,StartDate,EndDate,use_flag,MKTime,UPDTime,ADD_User,UPD_User)
					SELECT StationID,BLOCK_ID,BlockName,MAPColor,Longitude,Latitude,BlockType,StartDate,EndDate,use_flag,MKTime,@NowTime,ADD_User,@UserID FROM TB_Polygon WHERE StationID=@StationID AND BlockType=@Mode;

					
				END
				ELSE
				BEGIN
					INSERT INTO TB_Polygon(StationID,BlockName,MAPColor,Longitude,Latitude,BlockType,StartDate,EndDate,use_flag,ADD_User)
									VALUES(@StationID,@BlockName,@MAPColor,@Longitude,@Latitude,@Mode,@StartDate,@EndDate,1,@UserID)
				END
		   END
		   ELSE
		   BEGIN
				SET @hasData=0;
		 		SELECT @hasData=COUNT(1) FROM TB_Polygon WHERE StationID=@StationID AND BlockType=@Mode AND BLOCK_ID=@BlockID;
				IF @hasData=0
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR743'
				END
				ELSE
				BEGIN
					INSERT INTO TB_PolygonHistory(StationID,BLOCK_ID,BlockName,MAPColor,Longitude,Latitude,BlockType,StartDate,EndDate,use_flag,MKTime,UPDTime,ADD_User,UPD_User)
					SELECT StationID,BLOCK_ID,BlockName,MAPColor,Longitude,Latitude,BlockType,StartDate,EndDate,use_flag,MKTime,@NowTime,ADD_User,@UserID FROM TB_Polygon WHERE StationID=@StationID AND BlockType=@Mode AND BLOCK_ID=@BlockID;

					UPDATE TB_Polygon
					SET StationID=@StationID,BlockName=@BlockName,MAPColor=@MAPColor,Longitude=@Longitude,Latitude=@Latitude,BlockType=@Mode,StartDate=@StartDate,EndDate=@EndDate,UPDTime=@NowTime,UPD_User=@UserID
					WHERE StationID=@StationID AND BlockType=@Mode AND BLOCK_ID=@BlockID;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandlePolygon';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandlePolygon';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'設定電子柵欄', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandlePolygon';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandlePolygon';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandlePolygon';