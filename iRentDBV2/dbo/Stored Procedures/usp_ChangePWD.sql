/****************************************************************
** Name: [dbo].[usp_ChangePWD]
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
** EXEC @Error=[dbo].[usp_ChangePWD]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/6 上午 07:21:25 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/6 上午 07:21:25    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_ChangePWD]
	@IDNO                   VARCHAR(10)           ,
	@OldPWD                 VARCHAR(20)           , --舊密碼
	@NewPWD                 VARCHAR(20)           , --新密碼
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
DECLARE @tmpPWD VARCHAR(20);
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_ChangePWD';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @IDNO    =ISNULL (@IDNO    ,'');

SET @OldPWD=ISNULL (@OldPWD,'');
SET @NewPWD=ISNULL (@NewPWD,'');
SET @tmpPWD='';
		BEGIN TRY
		 IF  @IDNO='' OR @OldPWD='' OR @NewPWD='' 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR100'
 		 END
		 
		 IF @Error=0
		 BEGIN
			BEGIN TRAN
		       SELECT @hasData=COUNT(1) FROM TB_MemberData WHERE MEMIDNO=@IDNO ;
				IF @hasData=0
				BEGIN
					SET @Error=1;
				    SET @ErrorCode='ERR132';
				   COMMIT TRAN;
				END
				ELSE
				BEGIN
				   SELECT @tmpPWD=ISNULL(MEMPWD,'') FROM TB_MemberData WHERE MEMIDNO=@IDNO ;
				     IF @tmpPWD=@OldPWD
					 BEGIN
						UPDATE TB_MemberData 
						SET MEMPWD=@NewPWD
						WHERE  MEMIDNO=@IDNO;
					END
					ELSE
					BEGIN
						SET @Error=1;
						SET @ErrorCode='ERR146';
					END
				   COMMIT TRAN;
				END
		 END
		--寫入錯誤訊息
		    IF @Error=1
			BEGIN
			   INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
				 VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem)
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
				 VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem)
		END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ChangePWD';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ChangePWD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改密碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ChangePWD';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ChangePWD';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ChangePWD';