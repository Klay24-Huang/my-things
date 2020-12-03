/****** Object:  StoredProcedure [dbo].[usp_CheckCarIButton]    Script Date: 2020/12/3 下午 01:32:28 ******/

/****************************************************************
** Name: [dbo].[usp_CheckCarIButton]
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
** EXEC @Error=[dbo].[usp_CheckCarIButton]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
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
**
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_CheckCarIButton]
	@IDNO                   VARCHAR(10)           ,	--帳號
	@Token                  VARCHAR(1024)         ,	--JWT Token
	@OrderNo				BIGINT                ,	--訂單編號
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
DECLARE @Descript NVARCHAR(200);
DECLARE @CarNo VARCHAR(10);
DECLARE @DeviceStatus INT;			--iButton扣壓，是為1，否為0
DECLARE @DeviceData VARCHAR(50);	--iButton編號
DECLARE @HasIButton TINYINT;		--是否有iButton;0:否1:有
DECLARE @iButtonKey VARCHAR(100);	--iButtonKey

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_CheckCarIButton';
SET @IsSystem=0;
SET @ErrorType=0;
SET @Descript=N'還車前檢查【iButton】';
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @Token=ISNULL(@Token,'');
SET @OrderNo=ISNULL(@OrderNo,0);
SET @CarNo='';

BEGIN TRY
	IF @Token='' OR @IDNO='' OR @OrderNo=0
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

	IF @Error=0
	BEGIN
		SELECT @CarNo=CarNo FROM TB_OrderMain WHERE order_number=@OrderNo;
		IF @CarNo<>''
		BEGIN
			SELECT @DeviceStatus=extDeviceStatus2,@DeviceData=extDeviceData3 FROM TB_CarStatus WHERE CarNo=@CarNo;

			SELECT @HasIButton=HasIButton,@iButtonKey=iButtonKey FROM TB_CarInfo WHERE CarNo=@CarNo;

			IF @HasIButton=1
			BEGIN
				IF @DeviceStatus <> 1 OR @DeviceData <> @iButtonKey
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR231';
				END
			END
		END
		ELSE
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR223';
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckCarIButton';
GO

