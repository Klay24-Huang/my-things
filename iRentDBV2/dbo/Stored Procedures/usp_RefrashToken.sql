﻿/****************************************************************
** Name: [dbo].[usp_RefrashToken]
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
** EXEC @Error=[dbo].[usp_RefrashToken]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/13 上午 11:27:10 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/13 上午 11:27:10    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_RefrashToken]
	@IDNO                   VARCHAR(10)           , --帳號
	@RefrashToken           VARCHAR(64)           , --Token
	@DeviceID               VARCHAR(128)          , --DeviceID
	@APPVersion             VARCHAR(10)			  , --app版號
	@APP                    TINYINT               , --0:ANDROID;1:iOS
	@Rxpires_in             INT                   , --Access DeadLine second
	@Refrash_Rxpires_in     INT                   , --Refrash DeadLine second
	@LogID                  BIGINT                ,
	@PushREGID				BIGINT				  , --推播註冊流水號
	@Access_Token  			VARCHAR(64)	    OUTPUT,	--驗證碼
	@Refrash_Token		    VARCHAR(64)	    OUTPUT,	--驗證碼
	@MandatoryUPD			INT				OUTPUT,	--強制更新 1=強更，0=否
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
DECLARE @iRentVersion VARCHAR(10);	--iRent版號
DECLARE @ChkTime DATETIME;			--強更時間
DECLARE @UseFlag VARCHAR(1);		--啟動區分: Y = 啟動, N = 不啟動

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_RefrashToken';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @IDNO=ISNULL(@IDNO,'');
SET @DeviceID=ISNULL (@DeviceID,'');
SET @APP=ISNULL(@APP,2);
SET @APPVersion=ISNULL(@APPVersion,'');
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @MandatoryUPD=0;

BEGIN TRY
	IF @DeviceID='' OR @IDNO='' 
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	IF @Error=0
	BEGIN
		SET @hasData=0;
		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND Refrash_Token=@RefrashToken AND APP=@APP AND APPVersion=@APPVersion AND DeviceID=@DeviceID AND Refrash_Rxpires_in>=@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
	END

	--2.產生Token
	IF @Error=0
	BEGIN
		EXEC @Error=usp_GenerateToken @IDNO,@DeviceID,@Rxpires_in,@Refrash_Rxpires_in,@LogID,@APPVersion,@APP,@Access_Token OUTPUT,@Refrash_Token OUTPUT,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg OUTPUT
	END

	--20210129 增加推播流水號儲存檢測
	IF @Error=0
	BEGIN
		DECLARE @OldPushREGID BIGINT
		SELECT @OldPushREGID=PushREGID FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

		IF @OldPushREGID <> @PushREGID
		BEGIN
			IF @PushREGID > 0
			BEGIN
				UPDATE TB_MemberData 
				SET PushREGID=@PushREGID,
					U_PRGID=19,
					U_USERID=@IDNO,
					U_SYSDT=@NowTime
				WHERE MEMIDNO=@IDNO

				-- 20210226;新增LOG檔
				INSERT INTO TB_MemberData_Log
				SELECT 'U','19',@NowTime,* FROM TB_MemberData WHERE MEMIDNO=@IDNO;
			END
		END
	END

	--20210322;新增比對版號更新機制
	IF @Error=0
	BEGIN
		SELECT TOP 1 @iRentVersion=iRentVersion,@UseFlag=UseFlag,@ChkTime=ChkTime
		FROM [TB_ChkVersion] where DATEADD(HOUR,8,GETDATE()) >= ChkTime order by ChkTime desc;

		IF @UseFlag='Y'
		BEGIN
			IF @APPVersion <> @iRentVersion AND @NowTime >= @ChkTime
			BEGIN
				SET @MandatoryUPD=1;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_RefrashToken';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_RefrashToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新Token', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_RefrashToken';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_RefrashToken';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_RefrashToken';