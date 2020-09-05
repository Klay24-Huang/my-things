/****************************************************************
** Name: [dbo].[usp_UploadCredentials]
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
** EXEC @Error=[dbo].[usp_UploadCredentials]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/9 下午 05:11:34 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/9 下午 05:11:34    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_UploadCredentials]
	@IDNO                   VARCHAR(10)           ,
	@DeviceID               VARCHAR(128)          ,
	@Mode                   TINYINT               , --模式：0:新增;1:修改
	@CrentialsType         TINYINT               , --證件照類型: 1:身份證正面;2:身份證反面;3:汽車駕照正面;4:汽車駕照反面;5:機車駕證正面;6:機車駕證反面;7:自拍照;8:法定代理人;9:其他（如台大專案）;10:企業用戶
	@CrentialsFile	        VARCHAR(8000)         , --證件照
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

SET @FunName='usp_UploadCredentials';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @DeviceID=ISNULL (@DeviceID,'');
SET @CrentialsFile=ISNULL(@CrentialsFile,'')
SET @Mode=ISNULL(@Mode,2)
SET @CrentialsType=ISNULL(@CrentialsType,0)
SET @NowTime=DATEADD(HOUR,8,GETDATE());
		BEGIN TRY
		 IF @DeviceID='' OR @IDNO='' OR @CrentialsFile='' OR (@CrentialsType<1 OR @CrentialsType>10) OR (@Mode>1) 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		 IF @Error=0
		 BEGIN
		    SELECT @hasData=COUNT(1) FROM TB_MemberData WHERE MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
					SET @Error=1;
				    SET @ErrorCode='ERR136';
			END
			ELSE
			BEGIN
					SET @hasData=0;
					SELECT @hasData=COUNT(1) FROM TB_MemberData WHERE MEMIDNO=@IDNO AND HasCheckMobile=1;
					IF @hasData=0
					BEGIN
					   SET @Error=1;
					   SET @ErrorCode='ERR137';
					END
			END
		 END
		 IF @Error=0
		 BEGIN
				SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_Credentials WHERE IDNO=@IDNO;
				IF @hasData=0
				BEGIN
				  -- INSERT INTO TB_Credentials(IDNO,Signture)VALUES(@IDNO,1);
				  --1:身份證正面;2:身份證反面;3:汽車駕照正面;4:汽車駕照反面;5:機車駕證正面;6:機車駕證反面;7:自拍照;8:法定代理人;9:其他（如台大專案）;10:企業用戶
				  --證件類別開始
				   IF @CrentialsType=1
				   BEGIN
					INSERT INTO  TB_Credentials(IDNO,ID_1)VALUES(@IDNO,1);
				   END
				   ELSE IF @CrentialsType=2
				   BEGIN
					INSERT INTO  TB_Credentials(IDNO,ID_2)VALUES(@IDNO,1);
				   END
				   ELSE IF @CrentialsType=3
				   BEGIN
					INSERT INTO  TB_Credentials(IDNO,CarDriver_1)VALUES(@IDNO,1);
				   END
				   ELSE IF @CrentialsType=4
				   BEGIN
					INSERT INTO  TB_Credentials(IDNO,CarDriver_2)VALUES(@IDNO,1);
				   END
				   ELSE IF @CrentialsType=5
				   BEGIN
					INSERT INTO  TB_Credentials(IDNO,MotorDriver_1)VALUES(@IDNO,1);
				   END
				   ELSE IF @CrentialsType=6
				   BEGIN
					INSERT INTO  TB_Credentials(IDNO,MotorDriver_2)VALUES(@IDNO,1);
				   END
				   ELSE IF @CrentialsType=7
				   BEGIN
					INSERT INTO  TB_Credentials(IDNO,Self_1)VALUES(@IDNO,1);
				   END
				   ELSE IF @CrentialsType=8
				   BEGIN
					INSERT INTO  TB_Credentials(IDNO,Law_Agent)VALUES(@IDNO,1);
				   END
				   ELSE IF @CrentialsType=9
				   BEGIN
					INSERT INTO  TB_Credentials(IDNO,Other_1)VALUES(@IDNO,1);
				   END
				   ELSE IF @CrentialsType=10
				   BEGIN
					INSERT INTO  TB_Credentials(IDNO,Business_1)VALUES(@IDNO,1);
				   END
				   --證件類別結束
				   SET @hasData=0;
				   SELECT @hasData=COUNT(1) FROM TB_CrentialsPIC WHERE IDNO=@IDNO;
				     IF @hasData=0
				     BEGIN
						INSERT INTO TB_CrentialsPIC(IDNO,CrentialsFile,CrentialsType)VALUES(@IDNO,@CrentialsFile,@CrentialsType);
					 END
					 ELSE
					 BEGIN
						UPDATE TB_CrentialsPIC
						SET CrentialsFile=@CrentialsFile,UPDTime=@NowTime
						WHERE IDNO=@IDNO AND CrentialsType=@CrentialsFile;
					 END
				END
				ELSE
				BEGIN
					--UPDATE TB_Credentials SET Signture=1 WHERE IDNO=@IDNO;
					 --證件類別開始
				   IF @CrentialsType=1
				   BEGIN
					UPDATE  TB_Credentials SET ID_1=1,UPDTime=@NowTime WHERE IDNO= @IDNO;
				   END
				   ELSE IF @CrentialsType=2
				   BEGIN
					UPDATE  TB_Credentials SET ID_2=1,UPDTime=@NowTime WHERE IDNO= @IDNO;
				   END
				   ELSE IF @CrentialsType=3
				   BEGIN
					UPDATE  TB_Credentials SET CarDriver_1=1,UPDTime=@NowTime WHERE IDNO= @IDNO;
				   END
				   ELSE IF @CrentialsType=4
				   BEGIN
					UPDATE  TB_Credentials SET CarDriver_2=1,UPDTime=@NowTime WHERE IDNO= @IDNO;
				   END
				   ELSE IF @CrentialsType=5
				   BEGIN
					UPDATE  TB_Credentials SET MotorDriver_1=1,UPDTime=@NowTime WHERE IDNO= @IDNO;
				   END
				   ELSE IF @CrentialsType=6
				   BEGIN
					UPDATE  TB_Credentials SET MotorDriver_2=1,UPDTime=@NowTime WHERE IDNO= @IDNO;
				   END
				   ELSE IF @CrentialsType=7
				   BEGIN
					UPDATE  TB_Credentials SET Self_1=1,UPDTime=@NowTime WHERE IDNO= @IDNO;
				   END
				   ELSE IF @CrentialsType=8
				   BEGIN
					UPDATE  TB_Credentials SET Law_Agent=1,UPDTime=@NowTime WHERE IDNO= @IDNO;
				   END
				   ELSE IF @CrentialsType=9
				   BEGIN
					UPDATE  TB_Credentials SET Other_1=1,UPDTime=@NowTime WHERE IDNO= @IDNO;
				   END
				   ELSE IF @CrentialsType=10
				   BEGIN
					UPDATE  TB_Credentials SET Business_1=1,UPDTime=@NowTime WHERE IDNO= @IDNO;
				   END
				   --證件類別結束
					SET @hasData=0;
				    SELECT @hasData=COUNT(1) FROM TB_CrentialsPIC WHERE IDNO=@IDNO AND CrentialsType=11;
					IF @hasData=0
				     BEGIN
						INSERT INTO TB_CrentialsPIC(IDNO,CrentialsFile,CrentialsType)VALUES(@IDNO,@CrentialsFile,@CrentialsType);
					 END
					 ELSE
					 BEGIN
						UPDATE TB_CrentialsPIC
						SET CrentialsFile=@CrentialsFile,UPDTime=@NowTime
						WHERE IDNO=@IDNO AND CrentialsType=@CrentialsType;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UploadCredentials';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UploadCredentials';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'上傳證件照', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UploadCredentials';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UploadCredentials';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UploadCredentials';