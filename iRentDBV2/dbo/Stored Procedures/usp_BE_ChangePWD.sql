﻿/****************************************************************
** Name: [dbo].[usp_BE_ChangePWD]
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
** EXEC @Error=[dbo].[usp_BE_ChangePWD]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/12 下午 02:25:31 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/12 下午 02:25:31    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_ChangePWD]
	@Account                VARCHAR(50)           ,
	@UserPwd                VARCHAR(50)           ,
	@NewPwd                 VARCHAR(50)           ,
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
DECLARE @car_mgt_status TINYINT;
DECLARE @cancel_status TINYINT;
DECLARE @booking_status TINYINT;
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_ChangePWD';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【取消訂單】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @Account  =ISNULL (@Account ,'');
SET @UserPwd  =ISNULL (@UserPwd ,'');
SET @NewPwd   =ISNULL (@NewPwd   ,'');

		BEGIN TRY

		 
		 IF @Account='' OR @UserPwd=''  OR @NewPwd=''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		
		 IF @Error=0
		 BEGIN
		 	SELECT @hasData=COUNT(1) FROM TB_Manager WHERE  Account=@Account  AND UserPwd=HASHBYTES('sha1',@UserPwd);
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR701';
			END
			ELSE
			BEGIN
			   UPDATE TB_Manager
			   SET UPDTime=@NowTime,UserPwd=HASHBYTES('sha1',@NewPwd)
			   WHERE  Account=@Account;

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ChangePWD';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ChangePWD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'管理平台更改密碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ChangePWD';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ChangePWD';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ChangePWD';