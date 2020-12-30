/****************************************************************
** Name: [dbo].[usp_UPDTrade]
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
** EXEC @Error=[dbo].[usp_UPDTrade]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/18 下午 01:57:41 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/18 下午 01:57:41    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_UPDTrade]
	@OrderNo                BIGINT                , --訂單編號
	@MerchantTradeNo		VARCHAR(30)			  ,
	@MerchantMemberID       VARCHAR(20)           ,
	@RetCode       			VARCHAR(10)           ,
	@RetMsg        			NVARCHAR (400)        ,
	@TaishinTradeNo			VARCHAR(50)           ,
	@CardNumber    			VARCHAR(20)           ,
	@process_date  			DATETIME              ,
	@AUTHAMT       			INT					  ,
	@AuthIdResp    			INT					  ,
	@IsSuccess              INT					  ,
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

SET @FunName='usp_UPDTrade';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());

SET @OrderNo    =ISNULL (@OrderNo    ,0);
SET @MerchantTradeNo=ISNULL(@MerchantTradeNo,'');
SET @RetCode       	=ISNULL(@RetCode       	,'');
SET @RetMsg        	=ISNULL(@RetMsg        	,'');
SET @TaishinTradeNo	=ISNULL(@TaishinTradeNo	,'');
SET @CardNumber    	=ISNULL(@CardNumber    	,'');
SET @process_date  	=ISNULL(@process_date  	,'1911-01-01 00:00:00');
SET @AUTHAMT       	=ISNULL(@AUTHAMT       	,-1);
SET @AuthIdResp    	=ISNULL(@AuthIdResp    	,-1);
SET @IsSuccess      =ISNULL(@IsSuccess      ,-2);

		BEGIN TRY

		 
		 IF @IsSuccess>-1
		 BEGIN
			IF  @MerchantTradeNo='' OR @RetCode='' OR @RetMsg='' OR @TaishinTradeNo='' OR @CardNumber='' OR @process_date='1911-01-01 00:00:00' OR @AUTHAMT=-1 OR @AuthIdResp=-1 OR @IsSuccess=-2
			BEGIN
			  SET @Error=1;
			  SET @ErrorCode='ERR900'
 			END
			ELSE
			BEGIN
				UPDATE TB_Trade
				SET IsSuccess=@IsSuccess,RetCode=@RetCode,RetMsg=@RetMsg,TaishinTradeNo=@TaishinTradeNo,CardNumber=@CardNumber,process_date=@process_date,AUTHAMT=@AUTHAMT,AuthIdResp=@AuthIdResp,UPDTime=@NowTime,MerchantMemberID=@MerchantMemberID
				WHERE OrderNo=@OrderNo AND MerchantTradeNo=@MerchantTradeNo
			END
		END
		ELSE
		BEGIN
			IF  @MerchantTradeNo='' 
			BEGIN
			  SET @Error=1;
			  SET @ErrorCode='ERR900'
			END
			ELSE
			BEGIN
				UPDATE TB_Trade
				SET IsSuccess=@IsSuccess,RetCode=@RetCode,RetMsg=@RetMsg,UPDTime=@NowTime
				WHERE OrderNo=@OrderNo AND MerchantTradeNo=@MerchantTradeNo
			END
		END
		 
		 --20201230 ADD BY ADAM REASON.交易編號有機率漏掉，遇到空的要補資料
		IF EXISTS(SELECT * FROM TB_OrderDetail WITH(NOLOCK) WHERE order_number=@OrderNo AND transaction_no='')
		BEGIN
			UPDATE TB_OrderDetail SET transaction_no=@MerchantTradeNo WHERE order_number=@OrderNo
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UPDTrade';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UPDTrade';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新刷卡結果', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UPDTrade';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UPDTrade';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UPDTrade';