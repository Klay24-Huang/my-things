/****************************************************************
** Name: [dbo].[usp_ForgetPWD]
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
** EXEC @Error=[dbo].[usp_ForgetPWD]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/6 上午 07:11:09 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/6  |   Eric     |          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_ForgetPWD]
	@IDNO                   VARCHAR(10)           ,
	@DeviceID               VARCHAR(128)          ,
	@VerifyCode             VARCHAR(10)           , --驗證碼
	@LogID                  BIGINT                ,
	@Mobile                 VARCHAR(20)     OUTPUT,
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
DECLARE @NowDate DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_ForgetPWD';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @IDNO=ISNULL(@IDNO,'');
SET @DeviceID=ISNULL (@DeviceID,'');
SET @VerifyCode=ISNULL(@VerifyCode,'');
SET @NowDate=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
	IF @DeviceID='' OR @IDNO='' OR @VerifyCode=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	IF @Error=0
	BEGIN
		BEGIN TRAN
		SELECT @hasData=COUNT(1) FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR132';
			COMMIT TRAN;
		END
		ELSE
		BEGIN
			SELECT @Mobile=MEMTEL FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

			SET @hasData=0
			SELECT @hasData=COUNT(1) FROM TB_VerifyCode WITH(NOLOCK) WHERE IDNO=@IDNO AND Mode=1;
			IF @hasData=0
			BEGIN
				--沒資料就寫一筆新的
				INSERT INTO TB_VerifyCode(IDNO,Mobile,Mode,VerifyNum,DeadLine)
				VALUES(@IDNO,@Mobile,1,@VerifyCode,DATEADD(MINUTE,15,@NowDate));
			END
			ELSE
			BEGIN
				SET @hasData=0
				SELECT @hasData=COUNT(1) FROM TB_VerifyCode WITH(NOLOCK) WHERE IDNO=@IDNO AND Mode=1 AND IsVerify=1;
				IF @hasData >= 1
				BEGIN
					--有驗證過，寫一筆新的
					INSERT INTO TB_VerifyCode(IDNO,Mobile,Mode,VerifyNum,DeadLine)
					VALUES(@IDNO,@Mobile,1,@VerifyCode,DATEADD(MINUTE,15,@NowDate));
				END
				ELSE
				BEGIN
					--尚未驗證通過，更新原資料
					UPDATE TB_VerifyCode 
					SET VerifyNum=@VerifyCode,
						IsVerify=0,
						DeadLine=DATEADD(MINUTE,15,@NowDate),
						SendTime=@NowDate
					WHERE IDNO=@IDNO AND Mode=1 AND IsVerify=0;
				END
			END

			UPDATE TB_MemberData 
			SET NeedChangePWD=1,
				U_PRGID=7,
				U_USERID=@IDNO,
				U_SYSDT=@NowDate
			WHERE MEMIDNO=@IDNO;

			-- 20210225;新增LOG檔
			INSERT INTO TB_MemberData_Log
			SELECT 'U','7',@NowDate,* FROM TB_MemberData WHERE MEMIDNO=@IDNO;

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ForgetPWD';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ForgetPWD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'忘記密碼取出手機', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ForgetPWD';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ForgetPWD';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ForgetPWD';