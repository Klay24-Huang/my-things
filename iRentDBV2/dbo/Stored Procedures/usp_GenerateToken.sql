/****************************************************************
** Name: [dbo].[usp_GenerateToken]
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
** EXEC @Error=[dbo].[usp_GenerateToken]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/7/20 上午 07:29:22 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/7/20 上午 07:29:22    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GenerateToken]
	@MEMIDNO                VARCHAR(20)           ,
	@DeviceID				VARCHAR(256)          ,
	@Rxpires_in             INT                   , --Access DeadLine second
	@Refrash_Rxpires_in     INT                   , --Refrash DeadLine second
	@LogID                  BIGINT                ,
	@APPVersion              VARCHAR(10)		  , --app版號
	@APPKind                TINYINT               , --0:ANDROID;1:iOS
	@Access_Token  			VARCHAR(64)	    OUTPUT,	--驗證碼
	@Refrash_Token		    VARCHAR(64)	    OUTPUT,	--驗證碼
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
SET XACT_ABORT, NOCOUNT ON;
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
/*初始設定*/
DECLARE @Key CHAR(50);
DECLARE @Hash1th VARCHAR(40);
DECLARE @Hash2th VARCHAR(40);
DECLARE @RxpiresDate DATETIME;
DECLARE @RxpiresRefrashDate DATETIME;
DECLARE @hasData   INT;
DECLARE @tmpDeviceID VARCHAR(256);

SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_GenerateToken';
SET @IsSystem=0;
SET @ErrorType=1;
SET @MEMIDNO=ISNULL(@MEMIDNO,'');
SET @Rxpires_in=ISNULL(@Rxpires_in,1800); --預設30分鐘
SET @Refrash_Rxpires_in=ISNULL(@Refrash_Rxpires_in,3600);--預設30分鐘
SET @DeviceID=ISNULL(@DeviceID,'');
SET @LogID=ISNULL(@LogID,0);
SET @hasData=0;

BEGIN TRY
	IF @MEMIDNO='' OR @Rxpires_in=0 OR @DeviceID=''
	BEGIN
		SET @ErrorType=0;
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	IF @Error=0
	BEGIN
		SET @RxpiresDate=DATEADD(HOUR,8,DATEADD(SECOND,@Rxpires_in+30,GETDATE()));   --多給30秒當buffer
		SET @RxpiresRefrashDate=DATEADD(HOUR,8,DATEADD(SECOND,@Refrash_Rxpires_in+30,GETDATE()));   --多給30秒當buffer
		SET @Key = '5KMTE9JDGUJ52HE88T7PGGBF8WGU5T4YZB4TYRRSRVZWT9KGML';
		--20201203 ADD BY ADAM REASON.補上deviceid
		SET @Hash1th = UPPER(RIGHT(sys.fn_varbintohexstr(HASHBYTES('SHA1',UPPER(@MEMIDNO) + @DeviceID + @key + CONVERT(VARCHAR(20),@RxpiresDate,112))),40));
		SET @Hash2th = UPPER(RIGHT(sys.fn_varbintohexstr(HASHBYTES('SHA1',@hash1th)), 40));
		SET @Access_Token = LEFT(@hash1th + @hash2th,64);
		SET @Refrash_Token=RIGHT(@hash2th+@hash1th,64);

		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE MEMIDNO=@MEMIDNO;
		IF @hasData=0
		BEGIN
			INSERT INTO TB_Token(MEMIDNO,Access_Token,Refrash_Token,Rxpires_in,Refrash_Rxpires_in,DeviceID,APP,APPVersion)
			VALUES(@MEMIDNO,@Access_Token,@Refrash_Token,@RxpiresDate,@RxpiresRefrashDate,@DeviceID,@APPKind,@APPVersion);
		END
		ELSE
		BEGIN
			SELECT @tmpDeviceID=DeviceID FROM TB_Token WITH(NOLOCK) WHERE MEMIDNO=@MEMIDNO;

			IF @tmpDeviceID=@DeviceID
			BEGIN
				UPDATE TB_Token
				SET Access_Token=@Access_Token,
					Refrash_Token=@Refrash_Token,
					Rxpires_in=@RxpiresDate,
					Refrash_Rxpires_in=@RxpiresRefrashDate,
					APP=@APPKind,
					APPVersion=@APPVersion
				WHERE MEMIDNO=@MEMIDNO;
			END
			ELSE
			BEGIN
				--後面有人登入，要後踢前，產生新的token
				UPDATE TB_Token
				SET Access_Token=@Access_Token,
					Refrash_Token=@Refrash_Token,
					Rxpires_in=@RxpiresDate,
					Refrash_Rxpires_in=@RxpiresRefrashDate,
					DeviceID=@DeviceID,
					APP=@APPKind,
					APPVersion=@APPVersion
				WHERE MEMIDNO=@MEMIDNO;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GenerateToken';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GenerateToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GenerateToken';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GenerateToken';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GenerateToken';

