/****************************************************************
** Name: [dbo].[usp_Register_step1]
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
** EXEC @Error=[dbo].[usp_Register_step1]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/5 上午 05:19:09 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/5 上午 05:19:09    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_Register_step1]
	@IDNO                   VARCHAR(10)           , --帳號
	@Mobile                 VARCHAR(20)           , --手機
	@DeviceID               VARCHAR(128)          ,
	@VerifyCode             VARCHAR(6)            , --驗證碼
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
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_Register_step1';
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @DeviceID=ISNULL (@DeviceID,'');
SET @Mobile  =ISNULL (@Mobile,'');
		BEGIN TRY
		 IF @DeviceID='' OR @IDNO='' OR @Mobile=''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		 IF @Error=0
		 BEGIN
			--再次確認身份證是否存在
			BEGIN TRAN
				SELECT @hasData=COUNT(1) FROM TB_MemberData WHERE MEMIDNO=@IDNO AND MEMPWD='' AND IrFlag>-1;
				IF @hasData=0
				BEGIN
					SET @hasData=0
					SELECT @hasData=COUNT(1) FROM TB_MemberData WHERE MEMIDNO=@IDNO;
					IF @hasData=0
					BEGIN
					   INSERT INTO TB_MemberData(MEMIDNO,MEMTEL)VALUES(@IDNO,@Mobile);
					  
					END
					ELSE
					BEGIN
						UPDATE TB_MemberData SET MEMTEL=@Mobile WHERE MEMIDNO=@IDNO;
						
					END

					SET @hasData=0
					SELECT  @hasData=COUNT(1) FROM TB_VerifyCode WHERE IDNO=@IDNO  AND Mode=0;
					IF @hasData=0
					BEGIN
						INSERT INTO TB_VerifyCode(IDNO,Mobile,Mode,VerifyNum,DeadLine)VALUES(@IDNO,@Mobile,0,@VerifyCode,DATEADD(MINUTE,15,DATEADD(HOUR,8,GETDATE())));
					END
					ELSE
					BEGIN
						UPDATE TB_VerifyCode SET Mobile=@Mobile,VerifyNum=@VerifyCode,IsVerify=0,DeadLine=DATEADD(MINUTE,15,DATEADD(HOUR,8,GETDATE())),SendTime=DATEADD(HOUR,8,GETDATE()) WHERE IDNO=@IDNO AND Mode=0;
					END
				 COMMIT TRAN;
				END
				ELSE
				BEGIN
				    SET @Error=1;
				    SET @ErrorCode='ERR130';
					COMMIT TRAN;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Register_step1';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Register_step1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立帳號及密碼並發送簡訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Register_step1';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Register_step1';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Register_step1';