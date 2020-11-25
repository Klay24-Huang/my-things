/****************************************************************
** Name: [dbo].[usp_BE_ReturnControlSuccess]
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
** EXEC @Error=[dbo].[usp_BE_ReturnControlSuccess]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/19 上午 10:52:36 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/19 上午 10:52:36    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_ReturnControlSuccess]
    @OrderNo                BIGINT                ,
	@IsSuccess              INT                   ,
	@HasPaid                INT                   , --是否有付款：0:否;1:是
	@INVNO                  VARCHAR(50)           , --發票號碼
	@INVDATE                VARCHAR(20)           , --發票日期
	@INVAMT                 INT                   , --發票金額
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
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_ReturnControlSuccess';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @OrderNo   =ISNULL(@OrderNo  ,0); 
SET @IsSuccess =ISNULL(@IsSuccess,-1);
SET @INVNO  =ISNULL(@INVNO  ,'');
SET @INVDATE=ISNULL(@INVDATE,'');
SET @INVAMT =ISNULL(@INVAMT ,-1);
		BEGIN TRY

		 
		 IF  @OrderNo=0 OR @IsSuccess=-1
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
		 	IF @IsSuccess=1 
			BEGIN
			    IF @HasPaid=1
				BEGIN
					IF @INVNO='' OR @INVDATE='' OR @INVAMT=-1
					BEGIN
						SET @Error=1;
						SET @ErrorCode='ERR900'					
					END
					ELSE
					BEGIN
						UPDATE TB_ReturnCarControl
						SET UPDTime=@NowTime,isRetry=0
						WHERE IRENTORDNO=@OrderNo;

						UPDATE TB_OrderMain
						SET invoice_date=@INVDATE,invoice_price=@INVAMT,invoiceCode=@INVNO
						WHERE order_number=@OrderNo
					END
				END
				ELSE
				BEGIN
					UPDATE TB_ReturnCarControl
					SET UPDTime=@NowTime,isRetry=0
					WHERE IRENTORDNO=@OrderNo;
				END
			END
			ELSE
			BEGIN
				UPDATE TB_ReturnCarControl
				SET UPDTime=@NowTime,isRetry=1,RetryTimes=RetryTimes+1
				WHERE IRENTORDNO=@OrderNo;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ReturnControlSuccess';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ReturnControlSuccess';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'儲存130結果', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ReturnControlSuccess';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ReturnControlSuccess';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ReturnControlSuccess';