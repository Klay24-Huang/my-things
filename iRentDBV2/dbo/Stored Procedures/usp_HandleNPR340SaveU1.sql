﻿/****************************************************************
** Name: [dbo].[usp_HandleNPR340SaveU1]
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
** EXEC @Error=[dbo].[usp_HandleNPR340SaveU1]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/23 下午 04:07:47 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/23 下午 04:07:47    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_HandleNPR340SaveU1]
	@CUSTID 				VARCHAR(20)			  , 
    @ORDNO 					VARCHAR(50)			  , 
    @CNTRNO 				VARCHAR(50)			  , 
    @PAYMENTTYPE 			VARCHAR(10)			  , 
    @CARNO 					VARCHAR(10)			  ,
    @NORDNO 				VARCHAR(50)			  , 
    @PAYDATE 				VARCHAR(30)			  , 
    @AUTH_CODE 				VARCHAR(10)			  , 
    @AMOUNT 				VARCHAR(20)			  , 
    @CDTMAN 				NVARCHAR(10)		  , 
    @CARDNO 				VARCHAR(30)			  , 
    @POLNO 					VARCHAR(30)			  , 
	@MerchantTradeNo 		VARCHAR(40)			  ,
    @ServerTradeNo  		VARCHAR(50)			  ,
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

SET @FunName='usp_HandleNPR340SaveU1';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());



		BEGIN TRY

		 
		 IF @CUSTID='' OR @PAYMENTTYPE='' OR @MerchantTradeNo='' OR @ServerTradeNo=''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
		 	SET @hasData=0;
			SELECT @hasData=COUNT(1)
			FROM TB_NPR340
			WHERE CUSTID=@CUSTID AND ORDNO=@ORDNO AND CNTRNO=@CNTRNO AND PAYMENTTYPE=@PAYMENTTYPE AND AMOUNT=@AMOUNT AND CARNO=@CARNO   AND POLNO=@POLNO

			IF @hasData=0
			BEGIN
				INSERT INTO TB_NPR340(CUSTID,ORDNO,CNTRNO,PAYMENTTYPE,NORDNO,CDTMAN,POLNO,MerchantTradeNo,ServerTradeNo,PAYDATE,AUTH_CODE,CARDNO,AMOUNT,CARNO)VALUES(@CUSTID,@ORDNO,@CNTRNO,@PAYMENTTYPE,@NORDNO,@CDTMAN,@POLNO,@MerchantTradeNo,@ServerTradeNo,@PAYDATE,@AUTH_CODE,@CARDNO,@AMOUNT,@CARNO);
			END
			ELSE
			BEGIN
				SET @hasData=0;
				SELECT @hasData=COUNT(1)
				FROM TB_NPR340
				WHERE CUSTID=@CUSTID AND ORDNO=@ORDNO AND CNTRNO=@CNTRNO AND PAYMENTTYPE=@PAYMENTTYPE  AND POLNO=@POLNO AND MerchantTradeNo='' AND ServerTradeNo='';
				IF @hasData=1
				BEGIN
					UPDATE TB_NPR340
					SET MerchantTradeNo=@MerchantTradeNo,ServerTradeNo=@ServerTradeNo,CDTMAN=@CDTMAN,NORDNO=@NORDNO,PAYDATE=@PAYDATE,AUTH_CODE=@AUTH_CODE,CARDNO=@CARDNO,AMOUNT=@AMOUNT,UPDTime=@NowTime
					WHERE CUSTID=@CUSTID AND ORDNO=@ORDNO AND CNTRNO=@CNTRNO AND PAYMENTTYPE=@PAYMENTTYPE  AND POLNO=@POLNO AND MerchantTradeNo='' AND ServerTradeNo=''
				END
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleNPR340SaveU1';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleNPR340SaveU1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'欠款查詢（已刷卡成功）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleNPR340SaveU1';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleNPR340SaveU1';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HandleNPR340SaveU1';