/****************************************************************
** Name: [dbo].[usp_BE_HandleAuditImage]
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
** EXEC @Error=[dbo].[usp_BE_HandleAuditImage]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/4 上午 06:12:36 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/4 上午 06:12:36    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_HandleAuditImage]
	@IDNO                   VARCHAR(10)           ,
	@ID_1					TINYINT				  ,
    @ID_1_Image				VARCHAR(150)		  ,
    @ID_1_Audit				INT					  ,
    @ID_1_Reason			NVARCHAR(150)		  ,
    @ID_2					TINYINT				  ,
    @ID_2_Audit				INT					  ,
    @ID_2_Image				VARCHAR(150)		  ,
    @ID_2_Reason			NVARCHAR(150)		  ,
    @Car_1					TINYINT				  ,
    @Car_1_Image			VARCHAR(150)		  ,
    @Car_1_Audit			INT					  ,
    @Car_1_Reason			NVARCHAR(150)		  ,
    @Car_2					TINYINT				  ,
    @Car_2_Image			VARCHAR(150)		  ,
    @Car_2_Audit			INT					  ,
    @Car_2_Reason			NVARCHAR(150)		  ,
    @Motor_1				TINYINT				  ,
    @Motor_1_Image			VARCHAR(150)		  ,
    @Motor_1_Audit			INT					  ,
    @Motor_1_Reason			NVARCHAR(150)		  ,
    @Motor_2				TINYINT				  ,
    @Motor_2_Image			VARCHAR(150)		  ,
    @Motor_2_Audit			INT					  ,
    @Motor_2_Reason			NVARCHAR(150)		  ,
    @Self_1					TINYINT				  ,
    @Self_1_Image			VARCHAR(150)		  ,
    @Self_1_Audit			INT					  ,
    @Self_1_Reason			NVARCHAR(150)		  ,
    @F01					TINYINT				  ,
    @F01_Image				VARCHAR(150)		  ,
    @F01_Audit				INT					  ,
    @F01_Reason				VARCHAR(150)		  ,
    @Other_1				TINYINT				  ,
    @Other_1_Image			VARCHAR(150)		  ,
    @Other_1_Audit			INT					  ,
    @Other_1_Reason			NVARCHAR(150)		  ,
    @Business_1				TINYINT				  ,
    @Business_1_Image		VARCHAR(150)		  ,
    @Business_1_Audit		INT					  ,
    @Business_1_Reason		VARCHAR(150)		  ,
    @Signture_1				TINYINT				  ,
    @Signture_1_Image		VARCHAR(150)		  ,
    @Signture_1_Audit		INT					  ,
    @Signture_1_Reason		NVARCHAR(150)		  ,
    @UserID					NVARCHAR(10)		  ,
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
DECLARE @tmpFileName VARCHAR(150);
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_HandleAuditImage';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【取消訂單】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @UserID    =ISNULL (@UserID    ,'');

		BEGIN TRY

		 
		 IF @UserID='' OR @IDNO='' 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
			--身份證正面
			--IF @ID_1_Audit=1 --通過
			--BEGIN
			--	IF @ID_1_Image<>''
			--	BEGIN
			--		--更新證件主檔
			--		SELECT @hasData=COUNT(1) FROM TB_CrentialsPIC WITH(NOLOCK) WHERE IDNO=@IDNO AND CrentialsType=@ID_1;
			--		IF @hasData=0
			--		BEGIN
			--			INSERT INTO TB_CrentialsPIC(IDNO,CrentialsType,CrentialsFile)VALUES(@IDNO,@ID_1,@ID_1_Image)
			--		END
			--		ELSE
			--		BEGIN
			--			SELECT @tmpFileName=ISNULL(CrentialsFile,'') FROM TB_CrentialsPIC WITH(NOLOCK) WHERE IDNO=@IDNO AND CrentialsType=@ID_1;
			--			IF @tmpFileName<>@ID_1_Image
			--			BEGIN
			--				UPDATE TB_CrentialsPIC
			--				SET CrentialsFile=@ID_1_Image
			--				WHERE IDNO=@IDNO AND CrentialsType=@ID_1;
			--			END
			--		END
			--		--寫入照片審核歷程
			--		SET @hasData=0
			--		SELECT @hasData=COUNT(1) FROM TB_AuditCredentials WITH(NOLOCK) WHERE IDNO=@IDNO AND CrentialsType=@ID_1;
			--		IF @hasData=0
			--		BEGIN
			--			INSERT INTO TB_AuditCredentials(IDNO,CrentialsType,CrentialsFile)VALUES(@IDNO,@ID_1,@ID_1_Image)
			--		END
			--		ELSE
			--		BEGIN
			--				UPDATE TB_AuditCredentials
			--				SET CrentialsFile=@ID_1_Image
			--				WHERE IDNO=@IDNO AND CrentialsType=@ID_1;
			--		END
			--		--刪除待審核
			--		DELETE FROM TB_tmpCrentialsPIC WHERE IDNO=@IDNO AND CrentialsType=@ID_1;
			--	END
			--END
			--ELSE
			--BEGIN
			--	SELECT @hasData=COUNT(1) FROM TB_AuditCrentialsReject WITH(NOLOCK) WHERE IDNO=@IDNO AND CrentialsType=@ID_1;
			--	IF @hasData=0
			--	BEGIN
			--		INSERT INTO TB_AuditCrentialsReject(IDNO,CrentialsType,RejectReason)VALUES(@IDNO,@ID_1,@ID_1_Reason);
			--	END
			--	ELSE
			--	BEGIN
			--		UPDATE TB_AuditCrentialsReject
			--		SET RejectReason=@ID_1_Reason,UPDTime=@NowTime
			--		WHERE IDNO=@IDNO AND CrentialsType=@ID_1;
			--	END
			--	UPDATE TB_Credentials 
			--	SET ID_1=-2
			--	WHERE IDNO=@IDNO;
			--END
			EXEC @Error= [usp_HandleAuditImageData] @IDNO ,@ID_1 , @ID_1_Image, @ID_1_Audit , @ID_1_Reason,@LogID,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
			EXEC @Error= [usp_HandleAuditImageData] @IDNO ,@ID_2 , @ID_2_Image, @ID_2_Audit , @ID_2_Reason,@LogID,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
			EXEC @Error= [usp_HandleAuditImageData] @IDNO ,@Car_1 , @Car_1_Image, @Car_1_Audit , @Car_1_Reason,@LogID,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
			EXEC @Error= [usp_HandleAuditImageData] @IDNO ,@Car_2 , @Car_2_Image, @Car_2_Audit , @Car_2_Reason,@LogID,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
			EXEC @Error= [usp_HandleAuditImageData] @IDNO ,@Motor_1 , @Motor_1_Image, @Motor_1_Audit , @Motor_1_Reason,@LogID,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
			EXEC @Error= [usp_HandleAuditImageData] @IDNO ,@Motor_2 , @Motor_2_Image, @Motor_2_Audit , @Motor_2_Reason,@LogID,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
			EXEC @Error= [usp_HandleAuditImageData] @IDNO ,@Self_1 , @Self_1_Image, @Self_1_Audit , @Self_1_Reason,@LogID,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
			EXEC @Error= [usp_HandleAuditImageData] @IDNO ,@F01 , @F01_Image, @F01_Audit , @F01_Reason,@LogID,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
			EXEC @Error= [usp_HandleAuditImageData] @IDNO ,@Other_1 , @Other_1_Image,@Other_1_Audit ,@Other_1_Reason,@LogID,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
			EXEC @Error= [usp_HandleAuditImageData] @IDNO ,@Business_1 ,@Business_1_Image,@Business_1_Audit , @Business_1_Reason,@LogID,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
			EXEC @Error= [usp_HandleAuditImageData] @IDNO ,@Signture_1 , @Signture_1_Image,@Signture_1_Audit , @Signture_1_Reason,@LogID,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleAuditImage';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleAuditImage';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'審核照片', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleAuditImage';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleAuditImage';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleAuditImage';