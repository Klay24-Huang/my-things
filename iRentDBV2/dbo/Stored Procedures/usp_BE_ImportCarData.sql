/****************************************************************
** Name: [dbo].[usp_BE_ImportCarData]
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
** EXEC @Error=[dbo].[usp_BE_ImportCarData]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/16 下午 04:35:35 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/16 下午 04:35:35    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_ImportCarData]
	@CarNo					VARCHAR(10)           ,
	@TSEQNO					INT                   ,
	@CarType				VARCHAR(6)            ,
	@Seat					TINYINT               ,
	@FactoryYear			VARCHAR(6)            ,
	@CarColor				NVARCHAR(3)           ,
	@EngineNO				VARCHAR(50)           ,
	@BodyNO					VARCHAR(50)           ,
	@CCNum					INT                   ,
	@IsMotor                INT                   ,
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
DECLARE @StationID VARCHAR(10);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_ImportCarData';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo		=ISNULL(@CarNo		,'');
SET @TSEQNO		=ISNULL(@TSEQNO		,0);
SET @CarType	=ISNULL(@CarType	,'');
SET @Seat		=ISNULL(@Seat		,0);
SET @FactoryYear=ISNULL(@FactoryYear,'');
SET @CarColor	=ISNULL(@CarColor	,'');
SET @EngineNO	=ISNULL(@EngineNO	,'');
SET @BodyNO		=ISNULL(@BodyNO		,'');
SET @CCNum		=ISNULL(@CCNum		,0);
SET @UserID     =ISNULL(@UserID     ,'');
SET @IsMotor=0;
SET @StationID='X0XX';


		BEGIN TRY
	
		 
		 IF @CarNo='' OR @CarType=''OR @FactoryYear=''OR @CarColor='' OR @EngineNO='' OR @BodyNO='' OR @UserID=''  OR @TSEQNO=0 OR @Seat=0 OR @CCNum=0
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
		 	SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Car WHERE CarNo=@CarNo;
			IF @hasData>0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR713';
			END
		 END
		 IF @Error=0
		 BEGIN
		 	SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_CarInfo WHERE CarNo=@CarNo;
			IF @hasData>0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR714';
			END
		 END
		 IF @Error=0
		 BEGIN
				INSERT INTO TB_Car(CarNo,TSEQNO,CarType,StationID,nowStationID,available,last_Opt,UPDTime)VALUES(@CarNo,@TSEQNO,@CarType,@StationID,@StationID,2,@UserID,@NowTime);
				INSERT INTO TB_CarInfo([CarNo],[TSEQNO],[CarType] ,[Seat],[FactoryYear],[CarColor],[EngineNO],[BodyNO],[CCNum],[IsMotor],last_Opt,UPDTime)VALUES(@CarNo,@TSEQNO,@CarType,@Seat,@FactoryYear,@CarColor,@EngineNO,@BodyNO,@CCNum,@IsMotor,@UserID,@NowTime);
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ImportCarData';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ImportCarData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'匯入汽車資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ImportCarData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ImportCarData';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ImportCarData';