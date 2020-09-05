/****************************************************************
** Name: [dbo].[usp_InsErrorLog]
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
	@FunName                VARCHAR(50)   
	@ErrCode                VARCHAR(6)    
	@ErrType				TINYINT			
	@SQLErrorCode           INT           
	@SQLErrorDesc 			NVARCHAR(1000)
	@LogID					BIGINT			
	@IsSystem 				TINYINT       
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
** EXEC @Error=[dbo].[usp_InsErrorLog]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/7/22 上午 06:52:30 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/7/22 上午 06:52:30    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsErrorLog]
	@FunName                VARCHAR(50)           ,
	@ErrCode                VARCHAR(6)            ,
	@ErrType				TINYINT				  ,
	@SQLErrorCode           INT                   ,
	@SQLErrorDesc 			NVARCHAR(1000)	      ,	
	@LogID					BIGINT				  ,
	@IsSystem 				TINYINT               ,	--回傳錯誤訊息
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;

SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName      =ISNULL(@FunName ,'');
SET @ErrCode      =ISNULL(@ErrCode,'');
SET @SQLErrorCode =ISNULL(@SQLErrorCode ,0);
SET @SQLErrorDesc =ISNULL(@SQLErrorDesc ,N'');
SET @IsSystem 	  =ISNULL(@IsSystem ,99);
SET @ErrType	  =ISNULL(@ErrType,0);
SET @LogID		  =ISNULL(@LogID,0);

		BEGIN TRY
		      IF @FunName='' OR @IsSystem=99
			  BEGIN
			     SET @ErrCode=1;
				 SET @ErrorCode='ERR801'
			  END
			  IF @Error=0
			  BEGIN
			     IF @FunName<>'' AND @IsSystem<>99 AND (@ErrCode='' AND @SQLErrorCode=0 AND @SQLErrorDesc=N'')
				 BEGIN
				   SET @ErrCode=1;
				   SET @ErrorCode='ERR801'
				 END
			  END
			  IF @Error=0
			  BEGIN
			     INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
				 VALUES (@FunName,@ErrorCode,@ErrType,@SQLErrorCode,@SQLErrorDesc,@LogID,@IsSystem);

				 IF @@ROWCOUNT=0
				 BEGIN
				    SET @Error=1;
					SET @ErrorCode='ERR803'
				 END
			  END
		END TRY
		BEGIN CATCH
			SET @Error=-1;
			SET @ErrorCode='ERR999';
			SET @ErrorMsg='我要寫錯誤訊息';
			SET @SQLExceptionCode=ERROR_NUMBER();
			SET @SQLExceptionMsg=ERROR_MESSAGE();
			SET @IsSystem=1;
			IF @@TRANCOUNT > 0
			BEGIN
				print 'rolling back transaction' /* <- this is never printed */
				ROLLBACK TRAN
			END
		END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsErrorLog';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsErrorLog';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寫入錯誤log', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsErrorLog';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsErrorLog';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsErrorLog';

