/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_InsTradeForClose_I01
* 系    統 : IRENT
* 程式功能 : 錢包付款，寫入錢包扣款紀錄
* 作    者 : Umeko
* 撰寫日期 : 20210922
* 修改日期 :
Example :
***********************************************************************************************/
Create PROCEDURE [dbo].[usp_InsTradeForClose_I01]
	@OrderNo                BIGINT                , --訂單編號
	@MerchantTradeNo		VARCHAR(30)			  ,
	@CreditType             TINYINT               ,
	@MemberID               VARCHAR(20)           ,
	@CardToken              VARCHAR(128)          ,
	@amount                 INT                   ,
	@AutoClose             INT,                                         --是否自動關帳
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

SET @FunName='usp_InsTrade';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());

SET @OrderNo    =ISNULL (@OrderNo    ,0);
SET @MerchantTradeNo =ISNULL(@MerchantTradeNo,'');
SET @CreditType      =ISNULL(@CreditType      ,0);
SET @CardToken       =ISNULL(@CardToken,'');
SET @amount          =ISNULL(@amount          ,0);


		BEGIN TRY

		     IF @CreditType=0			--還車才會有訂單編號，其餘的都是認@MerchantTradeNo
			 BEGIN
			 	  IF @OrderNo=0 OR @MerchantTradeNo=''  OR @amount=0 OR @CardToken=''
				  BEGIN
				    SET @Error=1;
				    SET @ErrorCode='ERR900'
 				  END
			 END
			 ELSE
			 BEGIN
				  IF @MerchantTradeNo=''  OR @amount=0 OR @CardToken='' OR @MemberID=''
				  BEGIN
				    SET @Error=1;
				    SET @ErrorCode='ERR900'
 				  END
			 END
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
		     IF @CreditType=0
			 BEGIN
				   SELECT @MemberID=ISNULL(IDNO,'') FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo
				   SELECT @hasData=COUNT(1) FROM TB_Trade WITH(NOLOCK) WHERE OrderNo=@OrderNo AND CreditType=@CreditType;
					IF @hasData=0
					BEGIN
						INSERT INTO TB_Trade(OrderNo,MerchantTradeNo,CreditType,amount,MerchantMemberID,CardToken,AutoClose)
						VALUES(@OrderNo,@MerchantTradeNo,@CreditType,@amount,@MemberID,@CardToken,@AutoClose);
					END
					ELSE
					BEGIN
					   SET @hasData=0;
						SELECT @hasData=COUNT(1) FROM TB_Trade WITH(NOLOCK) WHERE OrderNo=@OrderNo AND IsSuccess=1 AND CreditType=@CreditType;
						IF @hasData=0
						BEGIN
							UPDATE TB_Trade 
							SET  MerchantTradeNo=@MerchantTradeNo,CreditType=@CreditType,amount=@amount,CardToken=@CardToken
							WHERE OrderNo=@OrderNo AND CreditType=@CreditType;
						END
						ELSE
						BEGIN
							SET @Error=1;
							SET @ErrorCode='ERR763'
						END
					END
			 END
			 ELSE
			 BEGIN
				INSERT INTO TB_Trade(OrderNo,MerchantTradeNo,CreditType,amount,MerchantMemberID,CardToken,AutoClose)
				VALUES(@OrderNo,@MerchantTradeNo,@CreditType,@amount,@MemberID,@CardToken,@AutoClose);
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsTrade';