

/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_HotaiTranStep1_I01
* 系    統 : IRENT
* 程式功能 : 和泰Pay信用卡授權交易表寫入
* 作    者 : Umeko
* 撰寫日期 : 20211206
* 修改日期 :
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_HotaiTranStep1_I01]
	@OrderNo                BIGINT                , --訂單編號
	@CreditType             TINYINT               ,
	@MemberID               VARCHAR(20)           ,
	@CardToken              VARCHAR(128)          ,
	@amount                 INT                   ,
	@AutoClose             INT,                                         --是否自動關帳
	@AuthType              INT,
	@MerchantTradeNoLeft     Varchar(13), --交易編號前置
	@PrgName               varchar(50),
	@PrgUser                 varchar(20),
	@LogID                  BIGINT                ,
	@MerchantTradeNo		VARCHAR(19)  OUTPUT,          --訂單編號
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

SET @FunName='usp_HotaiTranStep1_I01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());

SET @OrderNo    =ISNULL (@OrderNo ,0);
SET @MerchantTradeNo =ISNULL(@MerchantTradeNo,'');
SET @CreditType      =ISNULL(@CreditType      ,0);
SET @CardToken       =ISNULL(@CardToken,'');
SET @amount          =ISNULL(@amount          ,0);
Set @PrgName = ISNULL(@PrgName,'')
Set @PrgUser = ISNULL(@PrgUser,'')

Declare @PRGID varchar(50) = '0'
If Exists(Select 1 From TB_APIList with(nolock) Where APIName = @PrgName)
Begin
	Select @PRGID = Convert(varchar(20),APIID) From TB_APIList with(nolock) Where APIName = @PrgName
End
Else
Begin
	Set @PRGID = Left(@PrgName,50)
End
if(@PrgUser = '')
	Set @PrgUser = @PRGID
Else
	Set @PrgUser = Left(@PrgUser,20)
		

	BEGIN TRY
		IF @CreditType in (0,6)	--租金和訂金有跟訂單
		BEGIN
			IF @OrderNo=0  OR @amount=0 OR @CardToken=''
			BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR900'
 			END
		END
		ELSE
		BEGIN
			IF  @amount=0 OR @CardToken='' OR @MemberID=''
			BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR900'
 			END
		 END

		 IF @Error=0
		 BEGIN
				Begin Tran
				 Declare @seq int =0
		 		 Insert into [dbo].[TB_HotaiTransaction] ([IDNO],[OrderNO],[Amount],[CardToken],[Step],[A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT])
				 Values(@MemberID,@OrderNo,@amount,@CardToken,1,@PRGID,@PrgUser,@NowTime,@PRGID,@PrgUser,@NowTime)
				 Set @seq = Scope_Identity()

				 Set @MerchantTradeNo = CONCAT(@MerchantTradeNoLeft,@seq)
				 
				 Update TB_HotaiTransaction Set TransactionNo = @MerchantTradeNo Where [SeqNo] = @seq
				 IF @@ROWCOUNT != 1
						ROLLBACK TRAN
				 Else
				 Begin
					INSERT INTO TB_Trade(OrderNo,MerchantTradeNo,CreditType,amount,MerchantMemberID,CardToken,AutoClose)
					VALUES(@OrderNo,@MerchantTradeNo,@CreditType,@amount,@MemberID,@CardToken,@AutoClose);
					Commit
				End
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsTradeForClose_I01';