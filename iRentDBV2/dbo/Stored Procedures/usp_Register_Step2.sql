/****************************************************************
** Name: [dbo].[usp_Register_Step2]
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
** EXEC @Error=[dbo].[usp_Register_Step2]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/9 下午 03:35:20 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/9 下午 03:35:20    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_Register_Step2]
	@IDNO                   VARCHAR(10)           , --帳號
	@DeviceID               VARCHAR(128)          ,
	@PWD                    VARCHAR(100)          , --密碼
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
DECLARE @LogFlag VARCHAR(1);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_Register_Step2';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @IDNO=ISNULL(@IDNO,'');
SET @DeviceID=ISNULL (@DeviceID,'');
SET @PWD=ISNULL(@PWD,'')
SET @NowTime = DATEADD(hour,8,GETDATE())
SET @LogFlag='';

BEGIN TRY
	IF @DeviceID='' OR @IDNO=''  OR @PWD=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
		IF @hasData=0
		BEGIN
			SELECT @hasData=COUNT(1) FROM TB_MemberDataOfAutdit WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR133';
			END
			ELSE
			BEGIN
				INSERT INTO TB_MemberData
				(
					[A_PRGID], [A_USERID], [A_SYSDT], [U_PRGID], [U_USERID], [U_SYSDT],
					[MEMIDNO], [MEMCNAME], [MEMPWD], [MEMTEL], [MEMHTEL], [MEMBIRTH], 
					[MEMCOUNTRY], [MEMCITY], [MEMADDR], [MEMEMAIL], [MEMCOMTEL], 
					[MEMCONTRACT], [MEMCONTEL], [MEMMSG], [CARDNO], [UNIMNO], 
					[MEMSENDCD], [CARRIERID], [NPOBAN], [IrFlag]
				)
				SELECT 4, UPPER(@IDNO), @NowTime, 4, UPPER(@IDNO), @NowTime,
					[MEMIDNO], [MEMCNAME], @PWD, [MEMTEL], [MEMHTEL], [MEMBIRTH], 
					[MEMCOUNTRY], [MEMCITY], [MEMADDR], [MEMEMAIL], [MEMCOMTEL], 
					[MEMCONTRACT], [MEMCONTEL], [MEMMSG], [CARDNO], [UNIMNO], 
					[MEMSENDCD], [CARRIERID], [NPOBAN], 0
				FROM TB_MemberDataOfAutdit WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

				SET @LogFlag='A';
			END
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND HasCheckMobile=1;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR134';
			END
			ELSE
			BEGIN
				UPDATE TB_MemberData
				SET MEMPWD=@PWD,
					IrFlag=0,
					U_PRGID=4,
					U_USERID=@IDNO,
					U_SYSDT=@NowTime
				WHERE MEMIDNO=@IDNO;

				SET @LogFlag='U';
			END
		END

		-- 20210225;新增LOG檔
		INSERT INTO TB_MemberData_Log
		SELECT @LogFlag,'4',@NowTime,* FROM TB_MemberData WHERE MEMIDNO=@IDNO;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Register_Step2';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Register_Step2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'設定密碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Register_Step2';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Register_Step2';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_Register_Step2';