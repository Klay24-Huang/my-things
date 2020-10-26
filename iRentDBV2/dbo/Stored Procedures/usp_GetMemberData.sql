﻿/****************************************************************
** Name: [dbo].[usp_GetMemberData]
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
** EXEC @Error=[dbo].[usp_GetMemberData]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Jet 
** Date:2020/10/13 下午 03:24:00 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/21 下午 03:24:00    |  ADAM   |          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetMemberData]
	@IDNO		            VARCHAR(10)           ,
	@Token                  VARCHAR(1024)         ,
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

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetMemberData';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=DATEADD(HOUR,8,GETDATE());

	BEGIN TRY
        IF @Token='' OR @IDNO=''
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR900'
		END
		        
        --0.再次檢核token
		IF @Error=0
		BEGIN
			SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND Rxpires_in>@NowTime;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
			ELSE
			BEGIN
			    SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND MEMIDNO=@IDNO;
				IF @hasData=0
				BEGIN
				   SET @Error=1;
				   SET @ErrorCode='ERR101';
				END
			END
		END

		--1.取得資料
		IF @Error=0
		BEGIN
			SELECT   [MEMIDNO]
					--,[MEMPWD]  --20201024 ADD BY ADAM REASON.安全考量移除
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
					,Case When [ID_1]>0 And [ID_2]>0 Then B.ID_1 Else 0 End ID_pic
					,Case When [CarDriver_1]>0 And [CarDriver_2]>0 Then B.CarDriver_1 Else 0 End DD_pic
					,Case When [MotorDriver_1]>0 And [MotorDriver_2]>0 Then B.MotorDriver_1 Else 0 End MOTOR_pic
					,ISNULL([Self_1],0) As AA_pic 
					,ISNULL([Law_Agent],0) As F01_pic
					,ISNULL([Signture],0) AS Signture_pic
					,ISNULL(CrentialsFile,'') AS SigntureCode
			FROM TB_MemberData A WITH(NOLOCK)
			Left Join TB_Credentials B WITH(NOLOCK) on B.IDNO=A.MEMIDNO
			LEFT JOIN TB_CrentialsPIC C WITH(NOLOCK) ON A.MEMIDNO=C.IDNO AND CrentialsType=11
			WHERE A.MEMIDNO=@IDNO

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberData';



GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberData';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberData';