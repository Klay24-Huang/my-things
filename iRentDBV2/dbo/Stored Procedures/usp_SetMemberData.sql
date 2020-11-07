/****** Object:  StoredProcedure [dbo].[usp_SetMemberData]    Script Date: 2020/10/27 ‰∏äÂ 10:38:05 ******/
/****************************************************************
** Name: [dbo].[usp_SetMemberData]
** Desc: 
**
** Return values: 0 êÂ else ØË™§
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
	@IDNO                   VARCHAR(10)           , 
	@DeviceID               VARCHAR(128)          , 
	@MEMCNAME				NVARCHAR(10)          , 
	@MEMBIRTH               DATETIME              , 
	@MEMCITY                INT                   , 
	@MEMADDR                NVARCHAR(500)		  , 
	@MEMHTEL				VARCHAR(20)			  , 
	@MEMCOMTEL				VARCHAR(20)			  , 
	@MEMCONTRACT			NVARCHAR(10)		  , 
	@MEMCONTEL				VARCHAR(20)			  , 
	@MEMMSG					VARCHAR(1)			  , 
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime	DATETIME;

DECLARE @OMEMCNAME				NVARCHAR(10)    
DECLARE @OMEMBIRTH               DATETIME        
DECLARE @OMEMCITY                INT             
DECLARE @OMEMADDR                NVARCHAR(500)	
DECLARE @OSignture		        VARCHAR(8000)   
DECLARE @OMEMHTEL				VARCHAR(20)		
DECLARE @AuditKind              TINYINT;
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_SetMemberData';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL (@IDNO,'');
SET @DeviceID=ISNULL (@DeviceID,'');
SET @MEMCNAME=ISNULL (@MEMCNAME,'');
SET @MEMBIRTH=ISNULL(@MEMBIRTH,'');
SET @MEMCITY =ISNULL(@MEMCITY ,0);
SET @MEMADDR =ISNULL(@MEMADDR ,'');
SET @MEMHTEL  =ISNULL(@MEMHTEL,'');
SET @MEMCOMTEL=ISNULL(@MEMCOMTEL,'');
SET @MEMCONTRACT=ISNULL(@MEMCONTRACT,'');
SET @MEMCONTEL=ISNULL(@MEMCONTEL,'');
SET @MEMMSG  =ISNULL(@MEMMSG,'Y');
SET @AuditKind=0;

BEGIN TRY
	IF @IDNO='' OR @DeviceID='' OR @MEMCNAME='' OR @MEMBIRTH='' OR @MEMCITY=0 OR @MEMADDR=''
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
			SET @ErrorCode='ERR130';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_MemberData WHERE MEMIDNO=@IDNO AND HasCheckMobile=1;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR135';
			END
		END
	END
	IF @Error=0
	BEGIN
		SELECT @OMEMCNAME=ISNULL(MEMCNAME,''),
			   @OMEMBIRTH=ISNULL(MEMBIRTH,'1911-01-01 00:00:00'),
			   @OMEMCITY=ISNULL(MEMCITY,0),
			   @OMEMADDR=ISNULL(MEMADDR,''),
			   @OMEMHTEL=ISNULL(MEMHTEL,'')
		FROM TB_MemberData WHERE MEMIDNO=@IDNO;

		SELECT @OSignture=ISNULL(CrentialsFile,'') FROM TB_CrentialsPIC WHERE IDNO=@IDNO AND CrentialsType=11
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
			U_SYSDT=@NowTime
		WHERE MEMIDNO=@IDNO;
	END
	IF @Error=0
	BEGIN
		IF @MEMCNAME<>@OMEMCNAME OR @MEMBIRTH<>@OMEMBIRTH OR @MEMCITY<>@OMEMCITY OR @MEMADDR<>@OMEMADDR 
		BEGIN
			IF @MEMCNAME=@OMEMCNAME AND @MEMBIRTH=@OMEMBIRTH AND @MEMCITY=@OMEMCITY AND @MEMADDR=@OMEMADDR
			BEGIN
				SET @AuditKind=1;
			END
			ELSE
			BEGIN
				SET @AuditKind=2;
			END
		
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_MemberDataOfAutdit WHERE [MEMIDNO]=@IDNO;
			IF @hasData>0
			BEGIN
				DELETE FROM TB_MemberDataOfAutdit WHERE [MEMIDNO]=@IDNO;
			END

			INSERT INTO TB_MemberDataOfAutdit([MEMIDNO],[MEMCNAME],[MEMTEL],[MEMBIRTH],[MEMCOUNTRY],     
											  [MEMCITY],[MEMADDR],[MEMEMAIL],[MEMCOMTEL],[MEMCONTRACT], 	 
											  [MEMCONTEL],[MEMMSG],[CARDNO],[UNIMNO],[MEMSENDCD],
											  [CARRIERID],[NPOBAN],[AuditKind],[HasAudit],[IsNew])
			SELECT 	[MEMIDNO],[MEMCNAME],[MEMTEL],[MEMBIRTH],[MEMCOUNTRY],     
					[MEMCITY],[MEMADDR],[MEMEMAIL],[MEMCOMTEL],[MEMCONTRACT], 	 
					[MEMCONTEL],[MEMMSG],[CARDNO],[UNIMNO],[MEMSENDCD],
					[CARRIERID],[NPOBAN],@AuditKind,0,0  
			FROM TB_MemberData WHERE MEMIDNO=@IDNO;

	END
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END
END TRY
BEGIN CATCH
	SET @Error=-1;
	SET @ErrorCode='ERR999';
	SET @ErrorMsg='®t≤Œµo•Õ≤ß±`';
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