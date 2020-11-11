/****************************************************************
** Name: [dbo].[usp_InsReceiveCMD]
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
** EXEC @Error=[dbo].[usp_InsReceiveCMD]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/9/17 下午 02:44:50 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/9/17 下午 02:44:50    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsReceiveCMD]
	@requestId				VARCHAR(100) ,
	@method					VARCHAR(50) , 
	@CmdReply				VARCHAR(20) ,
	@receiveRawData         VARCHAR(1000),
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
DECLARE @CID	VARCHAR(10);
DECLARE @deviceToken	VARCHAR(512);
DECLARE @extDeviceData5 VARCHAR(128);
DECLARE @extDeviceData6 VARCHAR(256);
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_InsReceiveCMD';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @requestId    =ISNULL (@requestId    ,'');
SET @method       =ISNULL (@method,'');
SET @CID		 ='';
SET @deviceToken ='';
SET @extDeviceData5 = '';
SET @extDeviceData6 = '';
SET @receiveRawData	 =ISNULL(@receiveRawData	,'');
SET @CmdReply	 =ISNULL(@CmdReply	,'');

		BEGIN TRY
		 IF @requestId='' OR @method='' OR @CmdReply=''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		 IF @Error=0
		 BEGIN
		    SELECT @deviceToken=deviceToken,@CID=CID FROM TB_SendFETCatCMD WITH(NOLOCK) WHERE requestId=@requestId;
			INSERT INTO TB_ReceiveFETCatCMD(requestId,method,CmdReply,receiveRawData,CID,deviceToken)VALUES(@requestId,@method,@CmdReply,@receiveRawData,@CID,@deviceToken);

			--method=[SetMotorcycleRent]，需更新[TB_CarStatus]的[extDeviceData5],[extDeviceData6]
			IF @method = 'SetMotorcycleRent'
			BEGIN
				BEGIN TRY
				SELECT @extDeviceData5 = extDeviceData5, @extDeviceData6 = extDeviceData6
				FROM OPENJSON(@receiveRawData)
				  WITH (
					extDeviceData5 VARCHAR(128) '$.extDeviceData5',
					extDeviceData6 VARCHAR(256) '$.extDeviceData6'
				);
				END TRY
				BEGIN CATCH
				END CATCH
				UPDATE TB_CarStatus
				SET extDeviceData5 = @extDeviceData5, extDeviceData6 = @extDeviceData6
				WHERE CID = @CID
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsReceiveCMD';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsReceiveCMD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'接收FET CAT 回傳命令執行結果', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsReceiveCMD';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsReceiveCMD';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsReceiveCMD';