/***********************************************************************************************
* Serve    : sqyhi03azt.database.windows.net
* Database : IRENT_V2T
* 程式名稱 : usp_WalletStoreShop_U01
* 系    統 : IRENT
* 程式功能 : 電子錢包超商條碼更新記錄
* 作    者 : AMBER
* 撰寫日期 : 20211007
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_WalletStoreShop_U01]
	@PaymentId			varchar(20),   --銷帳編號
	@Code1	            varchar(200),  --第一段條碼文字
    @Code2	            varchar(200),  --第二段銷帳編號
	@Code3	            varchar(200),  --第三段條碼文字
	@StatusCode			varchar(20),   --回應狀態代碼
    @StatusDesc			nvarchar(200), --狀態說明
	@Barcode64          varchar(MAX),  --BarCode
    @Url                varchar(2000), --Url
	@LogID              bigint,         
	@ErrorCode          varchar(6)        OUTPUT,     --回傳錯誤代碼
	@ErrorMsg           nvarchar(100)     OUTPUT,     --回傳錯誤訊息
	@SQLExceptionCode   varchar(10)       OUTPUT,     --回傳sqlException代碼
	@SQLExceptionMsg    nvarchar(1000)    OUTPUT      --回傳sqlException訊
AS
BEGIN
SET NOCOUNT ON
	BEGIN TRY 
	SET	@ErrorCode  = '0000'	
	SET	@ErrorMsg   = 'SUCCESS'	
	SET	@SQLExceptionCode = ''		
	SET	@SQLExceptionMsg = ''		
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_WalletStoreShop_U01'
	DECLARE @NowTime DATETIME =dbo.GET_TWDATE()
	DECLARE @TxType	 VARCHAR(1)='i'
	SET @Url=ISNULL(@Url,'')

	IF  EXISTS(SELECT 1 FROM TB_TaishinWalletStoreShopLog WITH(NOLOCK) WHERE PaymentId=@PaymentId AND TxType=@TxType)
	BEGIN
	UPDATE TB_TaishinWalletStoreShopLog 
	SET Code1=@Code1,Code2=@Code2,Code3=@Code3,Barcode64=@Barcode64,[Url]=@Url,StatusCode=@StatusCode,StatusDesc=@StatusDesc,UPDTime=@NowTime
	WHERE PaymentId=@PaymentId AND TxType=@TxType;	
	END
	
	END TRY	
	BEGIN CATCH
		ROLLBACK TRAN
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH
END


