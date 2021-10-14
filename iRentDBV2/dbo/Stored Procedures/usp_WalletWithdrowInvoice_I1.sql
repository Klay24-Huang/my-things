/****** Object:  StoredProcedure [dbo].[usp_WalletWithdrowInvoice_I1]    Script Date: 2021/9/30 下午 03:52:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_WalletWithdrowInvoice_I1
* 系    統 : IRENT
* 程式功能 : 寫入財務回拋手續費發票資訊
* 作    者 : YANKEY
* 撰寫日期 : 20210930
* 修改日期 : 
			
Example :

EXEC usp_WalletWithdrowInvoice_I1 '',1,95,5,100,'51994648','吾良店家','台北市中山區XX路X號X樓','/WSDA5CF','1999','8562','0104235','0009853648526957','孫中山','999'

select TOP 10 * from TB_WalletWithdrawInvoiceInfo WITH(NOLOCK) ORDER BY MKTime DESC
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_WalletWithdrowInvoice_I1]
(   
	@MSG			VARCHAR(10) OUTPUT,
	@SEQNO			BIGINT,		--對應TB_WalletyTradeMain.SEQNO(Mapping用)
	@SALAMT			INT,		--銷售金額(發票不含稅)
	@TAXAMT			INT,		--營業稅額(發票稅額)
	@FEEAMT			INT,		--手續費總額(發票總額)
	@INV_CUSTID		VARCHAR(11), --客戶統編
	@INV_CUSTNM		NVARCHAR(30), --客戶名稱
	@INV_ADDR		NVARCHAR(152), --地址
	@INVCARRIER		VARCHAR(10), --載具號碼
	@NPOBAN			VARCHAR(10), --愛心碼
	@RNDCODE		VARCHAR(4), --發票隨機碼
	@RVBANK			VARCHAR(7), --匯款銀行代號
	@RVACNT			VARCHAR(16), --匯款銀行帳號
	@RV_NAME		NVARCHAR(60), --匯款戶名
    @LogID			BIGINT
)
AS
BEGIN
    SET NOCOUNT ON
		
	DECLARE @NOW DATETIME = DATEADD(HOUR, 8, GETDATE())
	DECLARE @Error INT = 0
    DECLARE	@ErrorCode VARCHAR(6) = '0000'	
    DECLARE	@ErrorMsg  		   NVARCHAR(100) = 'SUCCESS'	
    DECLARE	@SQLExceptionCode  VARCHAR(10) = ''		
    DECLARE	@SQLExceptionMsg   NVARCHAR(1000) = ''	
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_WalletWithdrowInvoice_U1'

	BEGIN TRY
	    set @LogID = isnull(@LogID,'')
		

		IF @LogID = ''
		BEGIN
			SET @Error=1
			set @ErrorMsg = 'LogID必填'
			SET @ErrorCode = 'ERR254'
		END

		if @Error = 0
		begin
			SET @Error=1
			SET @ErrorCode = 'ERR256'
				 IF isNull(@SEQNO,'')		= ''  SET @ErrorMsg += '錢包主檔識別碼未填寫,'
			ELSE IF isNull(@SALAMT,'')		= ''  SET @ErrorMsg += '銷售金額(發票不含稅)未填寫,'
			ELSE IF isNull(@TAXAMT,'')		= ''  SET @ErrorMsg += '營業稅額(發票稅額)未填寫,'
			ELSE IF isNull(@FEEAMT,'')		= ''  SET @ErrorMsg += '手續費總額(發票總額)未填寫,'
			--ELSE IF isNull(@INV_CUSTID,'')	= ''	SET @ErrorMsg += '客戶統編未填寫,'
			--ELSE IF isNull(@INV_CUSTNM,'')	= ''	SET @ErrorMsg += '客戶名稱未填寫,'
			--ELSE IF isNull(@INV_ADDR,'')		= ''	SET @ErrorMsg += '發票寄送地址未填寫,'
			--ELSE IF isNull(@INVCARRIER,'')	= ''	SET @ErrorMsg += '載具號碼未填寫,'
			--ELSE IF isNull(@NPOBAN,'')		= ''	SET @ErrorMsg += '愛心碼未填寫,'
			--ELSE IF isNull(@RNDCODE,'')		= ''	SET @ErrorMsg += '發票隨機碼未填寫,'
			ELSE IF isNull(@RVBANK,'')		= ''  SET @ErrorMsg += '匯款銀行代號未填寫,'
			ELSE IF isNull(@RVACNT,'')		= ''  SET @ErrorMsg += '匯款銀行帳號未填寫,'
			--ELSE IF isNull(@RV_NAME,'')		= ''  SET @ErrorMsg += '匯款戶名未填寫,'
			ELSE 
				BEGIN
					SET @Error=0
					SET @ErrorCode = '0000'
				END
		end


		IF @Error = 0
		BEGIN
			insert into dbo.TB_WalletWithdrawInvoiceInfo (SEQNO,SALAMT,TAXAMT,FEEAMT,INV_CUSTID,INV_CUSTNM,INV_ADDR,INVCARRIER,NPOBAN,RNDCODE,RVBANK,RVACNT,RV_NAME,UPDTime,UPDUser,UPDPRGID,MKTime,MKUser,MKPRGID) 
			SELECT SEQNO = @SEQNO , SALAMT = @SALAMT ,TAXAMT = @TAXAMT , FEEAMT = @FEEAMT , INV_CUSTID = @INV_CUSTID , INV_CUSTNM = @INV_CUSTNM , INV_ADDR =@INV_ADDR , INVCARRIER = @INVCARRIER , NPOBAN = @NPOBAN , RNDCODE = @RNDCODE , RVBANK = @RVBANK , RVACNT = @RVACNT , RV_NAME = @RV_NAME 
			, UPDTime = @NOW , UPDUser = 'SYS' , UPDPRGID = 'WithdrowI1' , MKTime = @NOW , MKUser = 'SYS' , MKPRGID = 'WithdrowI1'
		END

		--寫入錯誤訊息
		IF @Error=1
		BEGIN
			INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
			VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
		END
	END TRY
	BEGIN CATCH
		SET @Error=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	--輸出系統訊息
	SELECT @ErrorCode[ErrorCode], @ErrorMsg[ErrorMsg], @SQLExceptionCode[SQLExceptionCode], @SQLExceptionMsg[SQLExceptionMsg], @Error[Error]


END

GO


