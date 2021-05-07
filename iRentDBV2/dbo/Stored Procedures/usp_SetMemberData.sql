/****** Object:  StoredProcedure [dbo].[usp_SetMemberData]    Script Date: 2020/10/27 上午 10:38:05 ******/
/****************************************************************
** Name: [dbo].[usp_SetMemberData]
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
** EXEC @Error=[dbo].[usp_RegisterMemberData]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:
** Date:
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_SetMemberData]
	@IDNO                   VARCHAR(10)           , --帳號(身份證)
	@DeviceID               VARCHAR(128)          , --機碼
	@MEMCNAME				NVARCHAR(10)          , --姓名
	@MEMBIRTH               DATETIME              , --生日
	@MEMCITY                INT                   , --行政區id
	@MEMADDR                NVARCHAR(500)		  , --地址
	@MEMHTEL				VARCHAR(20)			  , --連絡電話(住家)
	@MEMCOMTEL				VARCHAR(20)			  , --公司電話
	@MEMCONTRACT			NVARCHAR(10)		  , --緊急連絡人
	@MEMCONTEL				VARCHAR(20)			  , --緊急連絡人電話
	@MEMMSG					VARCHAR(1)			  , --活動及優惠訊息通知
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error		INT;
DECLARE @IsSystem	TINYINT;
DECLARE @FunName	VARCHAR(50);
DECLARE @ErrorType	TINYINT;
DECLARE @hasData	TINYINT;
DECLARE @NowTime	DATETIME;

DECLARE @OMEMCNAME	NVARCHAR(10)    
DECLARE @OMEMBIRTH	DATETIME        
DECLARE @OMEMCITY	INT             
DECLARE @OMEMADDR	NVARCHAR(500)
DECLARE @OMEMHTEL	VARCHAR(20)		
DECLARE @AuditKind	TINYINT;
DECLARE @Audit		INT;	--是否通過審核(0:未審;1:已審;2:審核不通過)
DECLARE @IsNew		INT;	--是否為新加入(0:否;1:是)
DECLARE @LogFlag VARCHAR(1);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_SetMemberData';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @DeviceID=ISNULL(@DeviceID,'');
SET @MEMCNAME=ISNULL(@MEMCNAME,'');
SET @MEMBIRTH=ISNULL(@MEMBIRTH,'');
SET @MEMCITY=ISNULL(@MEMCITY ,0);
SET @MEMADDR=ISNULL(@MEMADDR ,'');
SET @MEMHTEL=ISNULL(@MEMHTEL,'');
SET @MEMCOMTEL=ISNULL(@MEMCOMTEL,'');
SET @MEMCONTRACT=ISNULL(@MEMCONTRACT,'');
SET @MEMCONTEL=ISNULL(@MEMCONTEL,'');
SET @MEMMSG=ISNULL(@MEMMSG,'Y');
SET @AuditKind=0;
SET @Audit=0;
SET @IsNew=1;
SET @LogFlag='';

BEGIN TRY
	IF @IDNO='' OR @DeviceID='' OR @MEMCNAME='' OR @MEMBIRTH='' OR @MEMCITY=0 OR @MEMADDR=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR130';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND HasCheckMobile=1;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR135';
			END
		END
	END

	--先取出未修改前的資料
	IF @Error=0
	BEGIN
		SELECT @OMEMCNAME=ISNULL(MEMCNAME,''),
			   @OMEMBIRTH=ISNULL(MEMBIRTH,'1911-01-01 00:00:00'),
			   @OMEMCITY=ISNULL(MEMCITY,0),
			   @OMEMADDR=ISNULL(MEMADDR,''),
			   @OMEMHTEL=ISNULL(MEMHTEL,''),
			   @Audit=Audit
		FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
	END

	IF @Error=0
	BEGIN
		UPDATE TB_MemberData
		SET MEMCNAME=@MEMCNAME,
			MEMBIRTH=@MEMBIRTH,
			MEMCITY=@MEMCITY,
			MEMADDR=@MEMADDR,
			MEMHTEL=@MEMHTEL,
			MEMCOMTEL=@MEMCOMTEL,
			MEMCONTRACT=@MEMCONTRACT,
			MEMCONTEL=@MEMCONTEL,
			MEMMSG=@MEMMSG,
			MEMUPDT=@NowTime,
			U_PRGID=131,
			U_USERID=@IDNO,
			U_SYSDT=@NowTime
		WHERE MEMIDNO=@IDNO;

		-- 20210225;新增LOG檔
		INSERT INTO TB_MemberData_Log
		SELECT 'U','131',@NowTime,* FROM TB_MemberData WHERE MEMIDNO=@IDNO;
	END

	--審核的判斷
	IF @Error=0
	BEGIN
		IF @MEMCNAME<>@OMEMCNAME OR @MEMBIRTH<>@OMEMBIRTH
		BEGIN	
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_AuditHistory WITH(NOLOCK) WHERE HandleItem=1 and IsReject=0 and IDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM MEMBER_NEW WITH(NOLOCK) WHERE IRENTFLG='Y' AND MEMIDNO=@IDNO;
				IF @hasData=0
				BEGIN
					--都不存在就是新會員
					SET @IsNew=1;
				END
				ELSE
				BEGIN
					--iRent1.0有審核過
					SET @IsNew=0;
				END
			END
			ELSE
			BEGIN
				--審核通過
				SET @IsNew=0;
			END

			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_MemberDataOfAutdit WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				INSERT INTO TB_MemberDataOfAutdit([MEMIDNO],[MEMCNAME],[MEMTEL],[MEMBIRTH],[MEMCOUNTRY],     
												  [MEMCITY],[MEMADDR],[MEMEMAIL],[MEMCOMTEL],[MEMCONTRACT], 	 
												  [MEMCONTEL],[MEMMSG],[CARDNO],[UNIMNO],[MEMSENDCD],
												  [CARRIERID],[NPOBAN],[AuditKind],[HasAudit],[IsNew],
												  MKTime,UPDTime)
				SELECT 	[MEMIDNO],[MEMCNAME],[MEMTEL],[MEMBIRTH],[MEMCOUNTRY],     
						[MEMCITY],[MEMADDR],[MEMEMAIL],[MEMCOMTEL],[MEMCONTRACT], 	 
						[MEMCONTEL],[MEMMSG],[CARDNO],[UNIMNO],[MEMSENDCD],
						[CARRIERID],[NPOBAN],@AuditKind,0,@IsNew,
						@NowTime,@NowTime
				FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

				SET @LogFlag='A';
			END
			ELSE
			BEGIN
				UPDATE TB_MemberDataOfAutdit
				SET MEMCNAME=@MEMCNAME,
					MEMBIRTH=@MEMBIRTH,
					MEMCITY=@MEMCITY,
					MEMADDR=@MEMADDR,
					MEMHTEL=@MEMHTEL,
					MEMCOMTEL=@MEMCOMTEL,
					MEMCONTRACT=@MEMCONTRACT,
					MEMCONTEL=@MEMCONTEL,
					MEMMSG=@MEMMSG,
					UPDTime=@NowTime
				WHERE MEMIDNO=@IDNO;

				SET @LogFlag='U';
			END

			-- 20210225;新增LOG檔
			INSERT INTO TB_MemberDataOfAutdit_Log
			SELECT @LogFlag,'131',@NowTime,* FROM TB_MemberDataOfAutdit WHERE MEMIDNO=@IDNO;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SetMemberData';
GO