/****************************************************************
** Name: [dbo].[usp_HandleAuditImageData]
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
** EXEC @Error=[dbo].[usp_HandleAuditImageData]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/4 上午 11:16:58 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/4 上午 11:16:58    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_HandleAuditImageData]
	@IDNO                   VARCHAR(10)           ,
	@ImageType				TINYINT				  ,
    @Image	    			VARCHAR(150)		  ,
    @Audit		    		INT					  ,
    @Reason			        NVARCHAR(150)		  ,
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

 DECLARE @tmpFileName VARCHAR(150);
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @hasData=0;
SET @tmpFileName='';
SET @FunName='usp_HandleAuditImageData';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());

SET @IDNO    =ISNULL (@IDNO    ,'');


		BEGIN TRY

		 
		 IF  @IDNO='' 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
		 	--身份證正面
			IF @Audit=2 --通過
			BEGIN
				IF @Image<>''
				BEGIN
					--20201113 ADD BY ADAM REASON.不管怎麼樣先更新資料
					UPDATE TB_Credentials
					SET  ID_1=CASE WHEN @ImageType=1 THEN 2 ELSE ID_1 END
						,ID_2=CASE WHEN @ImageType=2 THEN 2 ELSE ID_2 END
						,CarDriver_1=CASE WHEN @ImageType=3 THEN 2 ELSE CarDriver_1 END
						,CarDriver_2=CASE WHEN @ImageType=4 THEN 2 ELSE CarDriver_2 END
						,MotorDriver_1=CASE WHEN @ImageType=5 THEN 2 ELSE MotorDriver_1 END
						,MotorDriver_2=CASE WHEN @ImageType=6 THEN 2 ELSE MotorDriver_2 END
						,Self_1=CASE WHEN @ImageType=7 THEN 2 ELSE Self_1 END
						,Other_1=CASE WHEN @ImageType=8 THEN 2 ELSE Other_1 END
						,Law_Agent=CASE WHEN @ImageType=9 THEN 2 ELSE Law_Agent END
						,Business_1=CASE WHEN @ImageType=10 THEN 2 ELSE Business_1 END
						,Signture=CASE WHEN @ImageType=11 THEN 2 ELSE Signture END
						,UPDTime=@NowTime
					WHERE IDNO=@IDNO

					--更新證件主檔
					SELECT @hasData=COUNT(1) FROM TB_CrentialsPIC WITH(NOLOCK) WHERE IDNO=@IDNO AND CrentialsType=@ImageType;
					IF @hasData=0
					BEGIN
						INSERT INTO TB_CrentialsPIC(IDNO,CrentialsType,CrentialsFile)VALUES(@IDNO,@ImageType,@Image)
					END
					ELSE
					BEGIN
						SELECT @tmpFileName=ISNULL(CrentialsFile,'') FROM TB_CrentialsPIC WITH(NOLOCK) WHERE IDNO=@IDNO AND CrentialsType=@ImageType;
						IF @tmpFileName<>@Image
						BEGIN
							UPDATE TB_CrentialsPIC
							SET CrentialsFile=@Image,UPDTime=@NowTime
							WHERE IDNO=@IDNO AND CrentialsType=@ImageType;

							/*
							IF @ImageType=1
							BEGIN
								UPDATE TB_Credentials 
								SET ID_1=2,UPDTime=@NowTime
								WHERE IDNO=@IDNO;
							END
							ELSE IF @ImageType=2
							BEGIN
								UPDATE TB_Credentials 
								SET ID_2=2,UPDTime=@NowTime
								WHERE IDNO=@IDNO;
							END
							ELSE IF @ImageType=3
							BEGIN
								UPDATE TB_Credentials 
								SET CarDriver_1=2,UPDTime=@NowTime
								WHERE IDNO=@IDNO;
							END
							ELSE IF @ImageType=4
							BEGIN
								UPDATE TB_Credentials 
								SET CarDriver_2=2,UPDTime=@NowTime
								WHERE IDNO=@IDNO;
							END
							ELSE IF @ImageType=5
							BEGIN
								UPDATE TB_Credentials 
								SET MotorDriver_1=2,UPDTime=@NowTime
								WHERE IDNO=@IDNO;
							END
							ELSE IF @ImageType=6
							BEGIN
								UPDATE TB_Credentials 
								SET MotorDriver_1=2,UPDTime=@NowTime
								WHERE IDNO=@IDNO;
							END
							ELSE IF @ImageType=7
							BEGIN
								UPDATE TB_Credentials 
								SET Self_1=2,UPDTime=@NowTime
								WHERE IDNO=@IDNO;
							END
							ELSE IF @ImageType=8
							BEGIN
								UPDATE TB_Credentials 
								SET Other_1=2,UPDTime=@NowTime
								WHERE IDNO=@IDNO;
							END
							ELSE IF @ImageType=9
							BEGIN
								UPDATE TB_Credentials 
								SET Law_Agent=2,UPDTime=@NowTime
								WHERE IDNO=@IDNO;
							END
							ELSE IF @ImageType=10
							BEGIN
								UPDATE TB_Credentials 
								SET Business_1=2,UPDTime=@NowTime
								WHERE IDNO=@IDNO;
							END
							ELSE IF @ImageType=11
							BEGIN
								UPDATE TB_Credentials 
								SET Signture=2,UPDTime=@NowTime
								WHERE IDNO=@IDNO;
							END */
						END
					END
					--寫入照片審核歷程
					SET @hasData=0
					SELECT @hasData=COUNT(1) FROM TB_AuditCredentials WITH(NOLOCK) WHERE IDNO=@IDNO AND CrentialsType=@ImageType;
					IF @hasData=0
					BEGIN
						INSERT INTO TB_AuditCredentials(IDNO,CrentialsType,CrentialsFile)VALUES(@IDNO,@ImageType,@Image)
					END
					ELSE
					BEGIN
							UPDATE TB_AuditCredentials
							SET CrentialsFile=@Image,UPDTime=@NowTime
							WHERE IDNO=@IDNO AND CrentialsType=@ImageType;
					END
				
					--刪除待審核
					DELETE FROM TB_tmpCrentialsPIC WHERE IDNO=@IDNO AND CrentialsType=@ImageType;
				END
			END
			ELSE
			BEGIN
				SELECT @hasData=COUNT(1) FROM TB_AuditCrentialsReject WITH(NOLOCK) WHERE IDNO=@IDNO AND CrentialsType=@ImageType;
				IF @hasData=0
				BEGIN
					INSERT INTO TB_AuditCrentialsReject(IDNO,CrentialsType,RejectReason,UPDTime)VALUES(@IDNO,@ImageType,@Reason,@NowTime);
				END
				ELSE
				BEGIN
					UPDATE TB_AuditCrentialsReject
					SET RejectReason=@Reason,UPDTime=@NowTime
					WHERE IDNO=@IDNO AND CrentialsType=@ImageType;
				END
				IF @ImageType=1
				BEGIN
					UPDATE TB_Credentials 
					SET ID_1=-1,UPDTime=@NowTime
					WHERE IDNO=@IDNO;
				END
				ELSE IF @ImageType=2
				BEGIN
					UPDATE TB_Credentials 
					SET ID_2=-1,UPDTime=@NowTime
					WHERE IDNO=@IDNO;
				END
				ELSE IF @ImageType=3
				BEGIN
					UPDATE TB_Credentials 
					SET CarDriver_1=-1,UPDTime=@NowTime
					WHERE IDNO=@IDNO;
				END
				ELSE IF @ImageType=4
				BEGIN
					UPDATE TB_Credentials 
					SET CarDriver_2=-1,UPDTime=@NowTime
					WHERE IDNO=@IDNO;
				END
				ELSE IF @ImageType=5
				BEGIN
					UPDATE TB_Credentials 
					SET MotorDriver_1=-1,UPDTime=@NowTime
					WHERE IDNO=@IDNO;
				END
				ELSE IF @ImageType=6
				BEGIN
					UPDATE TB_Credentials 
					SET MotorDriver_1=-1,UPDTime=@NowTime
					WHERE IDNO=@IDNO;
				END
				ELSE IF @ImageType=7
				BEGIN
					UPDATE TB_Credentials 
					SET Self_1=-1,UPDTime=@NowTime
					WHERE IDNO=@IDNO;
				END
				ELSE IF @ImageType=8
				BEGIN
					UPDATE TB_Credentials 
					SET Other_1=-1,UPDTime=@NowTime
					WHERE IDNO=@IDNO;
				END
				ELSE IF @ImageType=9
				BEGIN
					UPDATE TB_Credentials 
					SET Law_Agent=-1,UPDTime=@NowTime
					WHERE IDNO=@IDNO;
				END
				ELSE IF @ImageType=10
				BEGIN
					UPDATE TB_Credentials 
					SET Business_1=-1,UPDTime=@NowTime
					WHERE IDNO=@IDNO;
				END
				ELSE IF @ImageType=11
				BEGIN
					UPDATE TB_Credentials 
					SET Signture=-1,UPDTime=@NowTime
					WHERE IDNO=@IDNO;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleAuditImageData';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleAuditImageData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleAuditImageData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleAuditImageData';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleAuditImageData';