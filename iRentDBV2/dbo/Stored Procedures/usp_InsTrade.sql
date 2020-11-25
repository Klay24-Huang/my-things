/****************************************************************
** Name: [dbo].[usp_InsTrade]
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
** EXEC @Error=[dbo].[usp_InsTrade]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/18 下午 01:44:41 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/18 下午 01:44:41    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsTrade]
	@OrderNo                BIGINT                , --訂單編號
	@MerchantTradeNo		VARCHAR(30)			  ,
	@CreditType             TINYINT               ,
	@MemberID               VARCHAR(20)           ,
	@CardToken              VARCHAR(128)          ,
	@amount                 INT                   ,
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
				   SELECT @hasData=COUNT(1) FROM TB_Trade WHERE OrderNo=@OrderNo AND CreditType=@CreditType;
					IF @hasData=0
					BEGIN
						INSERT INTO TB_Trade(OrderNo,MerchantTradeNo,CreditType,amount,MerchantMemberID,CardToken)VALUES(@OrderNo,@MerchantTradeNo,@CreditType,@amount,@MemberID,@CardToken);
					END
					ELSE
					BEGIN
					   SET @hasData=0;
						SELECT @hasData=COUNT(1) FROM TB_Trade WHERE OrderNo=@OrderNo AND IsSuccess=1 AND CreditType=@CreditType;
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
				INSERT INTO TB_Trade(OrderNo,MerchantTradeNo,CreditType,amount,MerchantMemberID,CardToken)VALUES(@OrderNo,@MerchantTradeNo,@CreditType,@amount,@MemberID,@CardToken);
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


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsTrade';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'刷卡前寫入記錄', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsTrade';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsTrade';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsTrade';