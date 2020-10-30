/****************************************************************
** Name: [dbo].[usp_DonePayBack]
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
** EXEC @Error=[dbo].[usp_VerifyEMail]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:eason 
** Date:2020/10/07 上午 09:24:42 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/29 下午 03:15:42    |  eason|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_DonePayBack]
	@NPR330Save_ID          INT                   ,
	@IDNO                   VARCHAR(10)           ,
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

SET @FunName='usp_DonePayBack';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

	BEGIN TRY      

		 IF @Token='' OR @IDNO='' OR @NPR330Save_ID = 0 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
		 	SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
			ELSE
			BEGIN
			    SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
				IF @hasData=0
				BEGIN
				   SET @Error=1;
				   SET @ErrorCode='ERR101';
				END
			END
		 END
	
	     IF @Error = 0
		 BEGIN
		   
		   UPDATE s SET 
		   s.IsPay = 1,
		   s.UPDTime = DATEADD(HOUR,8,GETDATE())
		   FROM TB_NPR330Save s
		   WHERE s.useFlag = 1 AND s.NPR330Save_ID = @NPR330Save_ID

		   UPDATE d SET
		   d.IsPay = 1,
		   d.UPDTime = DATEADD(HOUR,8,GETDATE())
		   FROM TB_NPR330Detail d
           WHERE d.useFlag = 1 AND d.NPR330Save_ID = @NPR330Save_ID

		   --同IDNO的NPR330Save_ID
		   DECLARE @tb_NPR330Save_ID TABLE(sId INT)		   
		   INSERT INTO @tb_NPR330Save_ID
		   SELECT s.NPR330Save_ID FROM TB_NPR330Save s
		   WHERE s.NPR330Save_ID <> @NPR330Save_ID 
		   AND s.IDNO = @IDNO		   

		   IF (SELECT COUNT(*) FROM　@tb_NPR330Save_ID) > 0　
		   BEGIN		   
			   UPDATE s SET 
			   s.useFlag = 0,
			   s.UPDTime = DATEADD(HOUR,8,GETDATE())
			   FROM TB_NPR330Save s
			   WHERE s.NPR330Save_ID IN (SELECT sId FROM @tb_NPR330Save_ID) 		    

			   UPDATE d SET
			   d.useFlag = 0,
			   d.UPDTime = DATEADD(HOUR,8,GETDATE())
			   FROM TB_NPR330Detail d
			   WHERE d.NPR330Save_ID IN (SELECT sId FROM @tb_NPR330Save_ID) 
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_DonePayBack';
GO


