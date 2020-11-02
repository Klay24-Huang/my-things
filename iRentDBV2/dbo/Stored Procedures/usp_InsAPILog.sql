/****************************************************************
** Name: [dbo].[usp_InsAPILog]
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
** EXEC @Error=[dbo].[usp_InsAPILog]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/3 下午 02:48:04 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/3 下午 02:48:04    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsAPILog]
	@APIName                VARCHAR(50)           ,
	@ClientIP               VARCHAR(45)           ,
	@APIInput               NVARCHAR(4000)        ,
	@ORDNO		            VARCHAR(50)           ,
	@LogID                  BIGINT          OUTPUT, 
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @APIID INT;
DECLARE @ErrorType TINYINT;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_InsAPILog';
SET @IsSystem=0;
SET @LogID=0;
SET @APIID=0;
SET @APIName    =ISNULL (@APIName    ,'');
SET @ClientIP=ISNULL (@ClientIP,'');
SET @APIInput=ISNULL (@APIInput,'');
SET @ORDNO=ISNULL (@ORDNO,'');
SET @ErrorType=1;
		BEGIN TRY
		 IF @APIName='' OR @ClientIP='' OR @APIInput='' 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
			IF @Error=0
			BEGIN
			   SELECT @APIID=APIID FROM TB_APIList WHERE APIName=@APIName;
			   --20201029不知道的APIName也要記錄，設定@APIID=999
			   IF @APIID=0
			   BEGIN
					SET @APIID = 999
					SET @ORDNO = @APIName
			   END
			   IF @APIID>0
			   BEGIN
					INSERT INTO TB_APILog(APIID,CLIENTIP,APIInput,ORDNO)VALUES(@APIID,@ClientIP,@APIInput,@ORDNO);
					IF @@ROWCOUNT=0
					BEGIN
					   SET @Error=1;
					   SET @ErrorCode='ERR903';
					END
					ELSE
					BEGIN
					   SET @LogID=@@IDENTITY;
					END
			   END
			   ELSE
			   BEGIN
			       SET @Error=1;
				   SET @ErrorCode='ERR904';
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
			SET @ErrorType=1;
			   INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
				 VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
		END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsAPILog';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsAPILog';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寫入API LOG', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsAPILog';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsAPILog';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsAPILog';