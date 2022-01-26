

/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_HotaiTranStep3_U01
* 系    統 : IRENT
* 程式功能 : 和泰Pay信用卡授權交易表寫入
* 作    者 : Umeko
* 撰寫日期 : 20211207
* 修改日期 :
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_HotaiTranStep3_U01]
	@MerchantTradeNo		VARCHAR(19) ,          --訂單編號
	@PageTitle               nvarchar(200),
	@PageContent        nvarchar(max),
	@PrgName               varchar(50),
	@PrgUser                 varchar(20),
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

SET @FunName='usp_HotaiTranStep3_U01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=dbo.GET_TWDATE();

SET @MerchantTradeNo =ISNULL(@MerchantTradeNo,'');
SET @PageTitle = ISNULL(@PageTitle,'')
SET @PageContent = IsNull(@PageContent,'');
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
			IF  @MerchantTradeNo = '' OR @PageContent=''
					BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR900'
			END
				 IF @Error=0
				 BEGIN
						Update TB_HotaiTransaction
						Set  [PageTitle]= @PageTitle,[PageContent] = @PageContent
								,Step = 3,U_PRGID = @PRGID,[U_USERID] = @PrgUser,U_SYSDT = @NowTime
						Where [TransactionNo] = @MerchantTradeNo And Step = 2
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_HotaiTranStep3_U01';