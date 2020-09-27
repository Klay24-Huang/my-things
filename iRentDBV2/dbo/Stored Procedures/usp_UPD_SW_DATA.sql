/****************************************************************
** Name: [dbo].[usp_UPD_SW_DATA]
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
** EXEC @Error=[dbo].[usp_UPD_SW_DATA]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/25 下午 04:24:29 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/25 下午 04:24:29    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_UPD_SW_DATA]
    @Station				VARCHAR(75),
	@Addr					NVARCHAR(100),
	@lon					decimal(9,6),
	@lat					decimal(9,6),
	@TotalCnt				INT,
	@FullCnt				INT,
	@EmptyCnt				INT,
	@UpdateTime             DATETIME,
	@CheckKey				VARCHAR(40),
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
DECLARE @Key	 VARCHAR(40);
DECLARE @EffectiveDate VARCHAR(10);
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_UPD_SW_DATA';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Key='';
SET @EffectiveDate=FORMAT(DATEADD(HOUR,8,GetDate()), 'yyyy-MM-dd');
		BEGIN TRY
		 IF @CheckKey=''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='003'
 		 END
		 IF @Error=0
		 BEGIN
		      SELECT @Key=ISNULL(EncryedKey,'') FROM TB_KeyStore WHERE EffectiveDate=@EffectiveDate AND EncryedKey=@CheckKey ;
				IF @Key=''
				BEGIN
				   SET @Error=1;
				   SET @ErrorCode='002'
				END
				ELSE
				BEGIN
				    
					 SET @hasData=0;
				     SELECT @hasData=COUNT(*)  FROM [TB_BAT_Station] WHERE Station=@Station ;
				     IF @hasData=0
					 BEGIN
						    INSERT INTO [TB_BAT_Station](Station,Addr,lon,lat,TotalCnt,FullCnt,EmptyCnt,MKTime,UPDTime)VALUES(@Station,@Addr,@lon,@lat,@TotalCnt,@FullCnt,@EmptyCnt,DATEADD(HOUR,8,GetDate()),@UpdateTime);
					 END
					 ELSE
					 BEGIN
					       UPDATE [TB_BAT_Station]
						   SET Station=@Station,Addr=@Addr,lon=@lon,lat=@lat,TotalCnt=@TotalCnt,FullCnt=@FullCnt,EmptyCnt=@EmptyCnt,UPDTime=@UpdateTime
						   WHERE Station=@Station;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UPD_SW_DATA';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UPD_SW_DATA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UPD_SW_DATA';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UPD_SW_DATA';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UPD_SW_DATA';