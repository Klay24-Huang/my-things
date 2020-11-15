/****************************************************************
** Name: [dbo].[usp_CheckVerifyCode]
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
** EXEC @Error=[dbo].[usp_CheckVerifyCode]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/5 上午 05:33:13 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/5 上午 05:33:13    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_CheckVerifyCode]
	@IDNO                   VARCHAR(10)           ,
	@DeviceID               VARCHAR(128)          ,
	@OrderNum               VARCHAR(20)           ,
	@Mode                   TINYINT               , --0:註冊;1:一次性開門
	@VerifyCode             VARCHAR(6)            , 
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @NowTime DATETIME;
DECLARE @tmpVerifyCode VARCHAR(6);
DECLARE @VerifyCodeID BIGINT;
DECLARE @DeadLine DATETIME;
DECLARE @MOBILE VARCHAR(10);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_CheckVerifyCode';
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @IDNO=ISNULL (@IDNO,'');
SET @DeviceID=ISNULL (@DeviceID,'');
SET @OrderNum=ISNULL (@OrderNum,'');
SET @VerifyCode=ISNULL(@VerifyCode,'');
SET @Mode=ISNULL(@Mode,3);
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @tmpVerifyCode='';
SET @VerifyCodeID=0;
SET @DeadLine=GETDATE();

BEGIN TRY
	IF @DeviceID='' OR @IDNO=''  OR @VerifyCode='' OR @Mode=3 OR (@OrderNum='' AND @Mode=2)
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	IF @Error=0
	BEGIN
		IF @Mode=0
		BEGIN
			--模式(0:註冊時)
			SELECT TOP 1 @VerifyCodeID=ISNULL(VerifyCodeID,0),
				@tmpVerifyCode=ISNULL(VerifyNum,''),
				@DeadLine=ISNULL(DeadLine,GETDATE()),
				@MOBILE=Mobile
			FROM TB_VerifyCode 
			WHERE IDNO=@IDNO AND Mode=0 AND IsVerify=0 
			ORDER BY SendTime DESC;

			IF @VerifyCodeID>0 AND @tmpVerifyCode=@VerifyCode 
			BEGIN
				IF @NowTime<@DeadLine
				BEGIN
					UPDATE TB_VerifyCode
					SET IsVerify=1 
					WHERE VerifyCodeID=@VerifyCodeID;

					SELECT @hasData=Count(1) FROM [TB_MemberDataOfAutdit] WHERE MEMIDNO=@IDNO AND MEMTEL=@MOBILE AND HasAudit=0;
					IF @hasData=0
					BEGIN
						INSERT INTO TB_MemberData(MEMIDNO,MEMTEL,HasCheckMobile,A_USERID,A_SYSDT)
						VALUES(@IDNO,@MOBILE,1,@IDNO,@NowTime);
					END
					ELSE
					BEGIN
						UPDATE [TB_MemberData] 
						SET MEMTEL=@MOBILE,
							HasCheckMobile=1,
							U_USERID=@IDNO,
							U_SYSDT=@NowTime
						WHERE MEMIDNO=@IDNO;
					END
				END
				ELSE
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR141';
				END
			END
			ELSE
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR131';
			END
		END
		ELSE IF @Mode=1
		BEGIN
			--模式(1:忘記密碼)
			SELECT TOP 1 @VerifyCodeID=ISNULL(VerifyCodeID,0),@tmpVerifyCode=ISNULL(VerifyNum,''),@DeadLine=ISNULL(DeadLine,GETDATE()) 
			FROM TB_VerifyCode WHERE IDNO=@IDNO AND Mode=1  
			ORDER BY SendTime DESC;

			IF @VerifyCodeID>0 AND @tmpVerifyCode=@VerifyCode 
			BEGIN
				IF @NowTime<@DeadLine
				BEGIN
					UPDATE TB_VerifyCode
					SET IsVerify=1 
					WHERE VerifyCodeID=@VerifyCodeID;
				END
				ELSE
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR142';
				END
			END
			ELSE
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR131';
			END
		END
		ELSE
		BEGIN
			--模式(2:一次性開門)
			SELECT TOP 1 @VerifyCodeID=ISNULL(VerifyCodeID,0),@tmpVerifyCode=ISNULL(VerifyNum,''),@DeadLine=ISNULL(DeadLine,GETDATE()) 
			FROM TB_VerifyCode WHERE IDNO=@IDNO AND Mode=2 AND OrderNum=@OrderNum 
			ORDER BY SendTime DESC;

			IF @VerifyCodeID>0 AND @tmpVerifyCode=@VerifyCode
			BEGIN
				IF @NowTime<@DeadLine
				BEGIN
					UPDATE TB_VerifyCode
					SET IsVerify=1 
					WHERE VerifyCodeID=@VerifyCodeID;
				END
				ELSE
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR531';
				END
			END
			ELSE
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR131';
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckVerifyCode';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckVerifyCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'確認簡訊驗證碼是否正確', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckVerifyCode';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckVerifyCode';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckVerifyCode';