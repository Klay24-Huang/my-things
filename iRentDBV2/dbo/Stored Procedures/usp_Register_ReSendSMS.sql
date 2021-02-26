/****************************************************************
** Name: [dbo].[usp_Register_ReSendSMS]
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
** EXEC @Error=[dbo].[usp_Register_ReSendSMS]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/6 上午 06:55:41 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/6 上午 06:55:41    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_Register_ReSendSMS]
	@IDNO                   VARCHAR(10)				,	--帳號
	@Mobile                 VARCHAR(20)				,	--手機
	@DeviceID               VARCHAR(128)			,	--機碼
	@VerifyCode             VARCHAR(6)				,	--驗證碼
	@Mode					INT						,	--模式(0:註冊;1:忘記密碼;2:一次性開門;3:更換手機)
	@LogID                  BIGINT					,
	@ErrorCode 				VARCHAR(6)		OUTPUT	,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT	,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT	,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT		--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @DeadLine DATETIME;
DECLARE @NowDate DATETIME;
DECLARE @LogFlag VARCHAR(1);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_Register_ReSendSMS';
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @IDNO=ISNULL (@IDNO,'');
SET @DeviceID=ISNULL (@DeviceID,'');
SET @Mobile=ISNULL (@Mobile,'');
SET @Mode=ISNULL(@Mode,0);
SET @DeadLine=DATEADD(HOUR,8,GETDATE());
SET @DeadLine=DATEADD(MINUTE,15,@DeadLine);
SET @NowDate=DATEADD(HOUR,8,GETDATE());
SET @LogFlag='';

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
		SELECT @hasData=COUNT(1) FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
		IF @hasData=0
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_VerifyCode WITH(NOLOCK) WHERE IDNO=@IDNO AND Mode=@Mode;
			IF @hasData=0
			BEGIN
				--沒資料就寫一筆新的
				INSERT INTO TB_VerifyCode(IDNO,Mobile,Mode,VerifyNum,DeadLine)
				VALUES(@IDNO,@Mobile,@Mode,@VerifyCode,@DeadLine);
			END
			ELSE
			BEGIN
				SET @hasData=0
				SELECT @hasData=COUNT(1) FROM TB_VerifyCode WITH(NOLOCK) WHERE IDNO=@IDNO AND Mode=@Mode AND IsVerify=1;
				IF @hasData >= 1
				BEGIN
					--有驗證過，寫一筆新的
					INSERT INTO TB_VerifyCode(IDNO,Mobile,Mode,VerifyNum,DeadLine)
					VALUES(@IDNO,@Mobile,@Mode,@VerifyCode,DATEADD(MINUTE,15,@NowDate));
				END
				ELSE
				BEGIN
					--尚未驗證通過，更新原資料
					UPDATE TB_VerifyCode
					SET VerifyNum=@VerifyCode,
						IsVerify=0,
						DeadLine=@DeadLine,
						SendTime=@NowDate
					WHERE IDNO=@IDNO AND Mode=@Mode AND IsVerify=0;
				END
			END
			COMMIT TRAN;
		END
		ELSE
		BEGIN
			SET @hasData=0;
			--判斷是否審核通過
			SELECT @hasData=COUNT(1) FROM [TB_MemberDataOfAutdit] WITH(NOLOCK) WHERE MEMIDNO=@IDNO;  --20201114 ADD BY ADAM REASON.改為待審只有一筆
			IF @hasData=0
			BEGIN
				--會員資料存在，則從[TB_MemberData]取相關資料存至待審檔
				INSERT INTO [TB_MemberDataOfAutdit] (MEMIDNO,MEMCNAME,MEMTEL,MEMBIRTH,MEMCOUNTRY,
													 MEMCITY,MEMADDR,MEMEMAIL,MEMCOMTEL,MEMCONTRACT,
													 MEMCONTEL,MEMMSG,CARDNO,UNIMNO,MEMSENDCD,
													 CARRIERID,NPOBAN,AuditKind,HasAudit,IsNew,
													 MKTime,UPDTime)
				SELECT MEMIDNO,MEMCNAME,@Mobile,MEMBIRTH,MEMCOUNTRY,
					   MEMCITY,MEMADDR,MEMEMAIL,MEMCOMTEL,MEMCONTRACT,
					   MEMCONTEL,MEMMSG,CARDNO,UNIMNO,MEMSENDCD,
					   CARRIERID,NPOBAN,0,0,1,
					   @NowDate,@NowDate
				FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

				SET @LogFlag='A';
			END
			ELSE
			BEGIN
				UPDATE [TB_MemberDataOfAutdit] 
				SET MEMTEL=@Mobile,
					UPDTime=@NowDate 
				WHERE MEMIDNO=@IDNO;	--20201114 ADD BY ADAM REASON.改為待審只有一筆

				SET @LogFlag='U';
			END

			-- 20210225;新增LOG檔
			INSERT INTO TB_MemberDataOfAutdit_Log
			SELECT @LogFlag,'5',@NowDate,* FROM TB_MemberDataOfAutdit WHERE MEMIDNO=@IDNO;

			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_VerifyCode WITH(NOLOCK) WHERE IDNO=@IDNO AND Mode=@Mode;
			IF @hasData=0
			BEGIN
				--沒資料就寫一筆新的
				INSERT INTO TB_VerifyCode(IDNO,Mobile,Mode,VerifyNum,DeadLine)
				VALUES(@IDNO,@Mobile,@Mode,@VerifyCode,@DeadLine);
			END
			ELSE
			BEGIN
				SET @hasData=0
				SELECT @hasData=COUNT(1) FROM TB_VerifyCode WITH(NOLOCK) WHERE IDNO=@IDNO AND Mode=@Mode AND IsVerify=1;
				IF @hasData >= 1
				BEGIN
					--有驗證過，寫一筆新的
					INSERT INTO TB_VerifyCode(IDNO,Mobile,Mode,VerifyNum,DeadLine)
					VALUES(@IDNO,@Mobile,@Mode,@VerifyCode,DATEADD(MINUTE,15,@NowDate));
				END
				ELSE
				BEGIN
					--尚未驗證通過，更新原資料
					UPDATE TB_VerifyCode
					SET VerifyNum=@VerifyCode,
						IsVerify=0,
						DeadLine=@DeadLine,
						SendTime=@NowDate
					WHERE IDNO=@IDNO AND Mode=@Mode AND IsVerify=0;
				END
			END

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Register_ReSendSMS';
GO