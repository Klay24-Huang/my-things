/****************************************************************
** Name: [dbo].[usp_RegisterMemberData]
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
** Auth:Eric 
** Date:2020/8/7 上午 05:47:47 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/7 上午 05:47:47    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_RegisterMemberData]
	@IDNO                   VARCHAR(10)           ,
	@DeviceID               VARCHAR(128)          ,
	@MEMCNAME				NVARCHAR(10)          , --姓名
	@MEMBIRTH               DATETIME              , --生日
	@MEMCITY                INT                   , --行政區id
	@MEMADDR                NVARCHAR(500)		  , --地址
	@MEMEMAIL				VARCHAR(200)		  , --EMail
	@Signture		        VARCHAR(8000)        , --簽名檔
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
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_RegisterMemberData';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @DeviceID=ISNULL (@DeviceID,'');
SET @MEMCNAME    =ISNULL (@MEMCNAME    ,'');
SET @MEMBIRTH=ISNULL(@MEMBIRTH,'');
SET @MEMCITY =ISNULL(@MEMCITY ,0);
SET @MEMADDR =ISNULL(@MEMADDR ,'');
SET @MEMEMAIL=ISNULL(@MEMEMAIL,'');
SET @Signture=ISNULL(@Signture,'');



		BEGIN TRY
		 IF @DeviceID='' OR @IDNO='' OR @MEMBIRTH='' OR @MEMCITY=0 OR @MEMADDR='' OR @MEMEMAIL='' OR @Signture='' OR @MEMCNAME=''
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
		 		UPDATE TB_MemberData
				SET MEMADDR=@MEMADDR,MEMBIRTH=@MEMBIRTH,MEMCITY=@MEMCITY,MEMCNAME=@MEMCNAME,MEMEMAIL=@MEMEMAIL,IrFlag=1
				WHERE MEMIDNO=@IDNO;

				SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_Credentials WHERE IDNO=@IDNO;
				IF @hasData=0
				BEGIN
				   INSERT INTO TB_Credentials(IDNO,Signture)VALUES(@IDNO,1);

				   SET @hasData=0;
				   SELECT @hasData=COUNT(1) FROM TB_tmpCrentialsPIC WHERE IDNO=@IDNO AND CrentialsType=11;	--改為查TB_tmpCrentialsPIC
				     IF @hasData=0
				     BEGIN
						INSERT INTO TB_tmpCrentialsPIC(IDNO,CrentialsType,CrentialsFile)VALUES(@IDNO,11,@Signture);
					 END
					 ELSE
					 BEGIN
						UPDATE TB_tmpCrentialsPIC
						SET CrentialsFile=@Signture
						WHERE IDNO=@IDNO AND CrentialsType=11;
					 END
				END
				ELSE
				BEGIN
					UPDATE TB_Credentials SET Signture=1 WHERE IDNO=@IDNO;
					
					SET @hasData=0;
				    SELECT @hasData=COUNT(1) FROM TB_tmpCrentialsPIC WHERE IDNO=@IDNO AND CrentialsType=11;
					IF @hasData=0
				     BEGIN
						INSERT INTO TB_tmpCrentialsPIC(IDNO,CrentialsType,CrentialsFile)VALUES(@IDNO,11,@Signture);
					 END
					 ELSE
					 BEGIN
						UPDATE TB_tmpCrentialsPIC
						SET CrentialsFile=@Signture
						WHERE IDNO=@IDNO AND CrentialsType=11;
					 END
				END
		 END
		 IF @Error=0
		 BEGIN
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
						[CARRIERID],[NPOBAN],2,0,1  
			   FROM TB_MemberData WHERE MEMIDNO=@IDNO;
		
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_RegisterMemberData';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_RegisterMemberData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'註冊寫入基本資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_RegisterMemberData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_RegisterMemberData';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_RegisterMemberData';