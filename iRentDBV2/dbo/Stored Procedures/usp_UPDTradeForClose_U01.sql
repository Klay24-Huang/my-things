/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_InsTradeForClose_I01
* 系    統 : IRENT
* 程式功能 :更新信用卡授權結果與關帳檔
* 作    者 : Umeko
* 撰寫日期 : 20211026
* 修改日期 :
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_UPDTradeForClose_U01]
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
	@ChkClose            INT,                                         --可否關帳
	@CardType               INT,
	@AuthType              Int,
	@ProName              varchar(50),                                         --程式名稱
	@UserID                   varchar(20),
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

SET @FunName='usp_UPDTradeForClose_U01';
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
				Declare @TradeID bigint = 0
				Select @TradeID = TradeID
				From TB_Trade with(nolock)  
				Where OrderNo=@OrderNo AND MerchantTradeNo=@MerchantTradeNo

				Declare @PRGID varchar(20) = '0'
				If Exists(Select 1 From TB_APIList with(nolock) Where APIName = @ProName)
				Begin
					Select @PRGID = Convert(varchar(20),APIID) From TB_APIList with(nolock) Where APIName = @ProName
				End
				Else
				Begin
					Set @PRGID = Left(@ProName,20)
				End
				if(@UserID = '')
					Set @UserID = @PRGID

				Begin tran
					UPDATE TB_Trade
					SET IsSuccess=@IsSuccess,RetCode=@RetCode,RetMsg=@RetMsg,TaishinTradeNo=@TaishinTradeNo,CardNumber=@CardNumber,process_date=@process_date,AUTHAMT=@AUTHAMT,AuthIdResp=@AuthIdResp,UPDTime=@NowTime,MerchantMemberID=@MerchantMemberID
					WHERE OrderNo=@OrderNo AND MerchantTradeNo=@MerchantTradeNo
					
					Insert into [dbo].[TB_TradeClose]([TradeID], [OrderNo], [MerchantTradeNo], [CardType], [AuthType], [ChkClose], [CloseAmout],  [A_PRGID], [A_USERID], [U_PRGID], [U_USERID])
					values(@TradeID,@OrderNo,@MerchantTradeNo,@CardType,@AuthType,@ChkClose,@AUTHAMT,@PRGID,@UserID,@PRGID,@UserID)
				Commit
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_UPDTradeForClose_U01';