/****************************************************************
** Name: [dbo].[usp_MemberLogin]
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
** EXEC @Error=[dbo].[usp_MemberLogin]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/3 上午 06:04:28 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/3 上午 06:04:28    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_MemberLogin]
	@MEMIDNO                VARCHAR(10)           , --帳號
	@PWD                    VARCHAR(20)           , --密碼
	@DeviceID               VARCHAR(128)          , --DeviceID
	@APPVersion             VARCHAR(10)			  , --app版號
	@APP                    TINYINT               , --0:ANDROID;1:iOS
	@Rxpires_in             INT                   , --Access DeadLine second
	@Refrash_Rxpires_in     INT                   , --Refrash DeadLine second
	@LogID                  BIGINT                ,
	@Access_Token  			VARCHAR(64)	    OUTPUT,	--驗證碼
	@Refrash_Token		    VARCHAR(64)	    OUTPUT,	--驗證碼
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @tmpIDNo     VARCHAR(10);
DECLARE @tmpOrderNum VARCHAR(20);
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
SET @FunName='usp_MemberLogin';
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @MEMIDNO    =ISNULL(@MEMIDNO    ,'');
SET @DeviceID   =ISNULL(@DeviceID,'');
--SET @APPVerion  =ISNULL(@APPVerion,'');
	BEGIN TRY
		--0.基本防呆
		IF @DeviceID='' OR @MEMIDNO='' --OR @APPVerion='' 
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR900'
		END
		--1.驗證帳密
		IF @Error=0
		BEGIN
			SELECT @hasData=COUNT(1) FROM TB_MemberData	WHERE MEMIDNO=@MEMIDNO AND MEMPWD=@PWD;
			IF @hasData<>1
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR100';
				SET @ErrorType=1;
			END
		END
		--2.產生Token
		IF @Error=0
		BEGIN
			EXEC @Error=usp_GenerateToken @MEMIDNO ,	@DeviceID,@Rxpires_in,@Refrash_Rxpires_in,@LogID,@APPVersion,@APP,@Access_Token OUTPUT,@Refrash_Token OUTPUT,@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg OUTPUT
		END
		--3.回傳基本資料
		IF @Error=0
		BEGIN

			SELECT   [MEMIDNO]
					--,[MEMPWD]	--20201024 ADD BY ADAM REASON.安全考量移除
					,[MEMCNAME]
					,[MEMTEL]
					,CASE WHEN MEMBIRTH IS NULL THEN '' ELSE CONVERT(VARCHAR(10),MEMBIRTH,120) END AS [MEMBIRTH]
					,[MEMCITY] AS MEMAREAID
					,[MEMADDR]
					,[MEMEMAIL]
					,[CARDNO]
					,[UNIMNO]
					,[MEMSENDCD]
					,[CARRIERID]
					,[NPOBAN]
					,[HasCheckMobile]
					,[NeedChangePWD]
					,[HasBindSocial]
					,[IrFlag]
					,[PayMode]
					,[HasVaildEMail]
					,[Audit]
					,[RentType]
					,Case When [ID_1]=1 And [ID_2] =1 Then B.ID_1 Else 0 End ID_pic
					,Case When [CarDriver_1]=1 And [CarDriver_2]=1 Then B.CarDriver_1 Else 0 End DD_pic
					,Case When [MotorDriver_1]=1 And [MotorDriver_2]=1 Then B.MotorDriver_1 Else 0 End MOTOR_pic
					,ISNULL([Self_1],0) As AA_pic 
					,ISNULL([Law_Agent],0) As F01_pic
					,ISNULL([Signture],0) AS Signture_pic
					,ISNULL(CrentialsFile,'') AS SigntureCode
			FROM TB_MemberData A WITH(NOLOCK)
			Left Join TB_Credentials B WITH(NOLOCK) on B.IDNO=A.MEMIDNO
			LEFT JOIN TB_CrentialsPIC C WITH(NOLOCK) ON A.MEMIDNO=C.IDNO AND CrentialsType=11
			WHERE A.MEMIDNO=@MEMIDNO AND  MEMPWD=@PWD;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MemberLogin';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MemberLogin';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員登入', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MemberLogin';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MemberLogin';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MemberLogin';