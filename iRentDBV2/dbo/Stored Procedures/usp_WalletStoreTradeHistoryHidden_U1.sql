/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_WalletStoreTradeHistoryHidden_U1
* 系    統 : IRENT
* 程式功能 : 錢包歷程-儲值交易紀錄隱藏
* 作    者 : eason
* 撰寫日期 : 20210818
* 修改日期 : 20210903 UPD BY AMBER REASON: 調整F_INFNO改為抓取TaishinNO(台新交易IR編)欄位
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_WalletStoreTradeHistoryHidden_U1]
(   
	@IDNO			   VARCHAR(20)       , --身分證號
    @LogID			   BIGINT,
	@ORGID             VARCHAR(5), --組織代號(公司代碼)
	@SEQNO             INT,  --帳款流水號(by會員)
	@TaishinNO           VARCHAR(30), --財務-上游批號(IR編)
	@xError                 INT              OUTPUT,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
)
AS
BEGIN
    SET NOCOUNT ON

	declare @spIn nvarchar(max), @SpOut nvarchar(max)--splog
	begin 		
		select @spIn = isnull((
		select @IDNO[IDNO], @LogID[LogID] , 
		@ORGID[ORGID], @SEQNO[SEQNO], @TaishinNO[TaishinNO]
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),'{}')
	end

 	SET @xError = 0
    SET	@ErrorCode  = '0000'	
    SET	@ErrorMsg   = 'SUCCESS'	
    SET	@SQLExceptionCode = ''		
    SET	@SQLExceptionMsg = ''		
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_WalletStoreTradeHistoryHidden_U1'

	set @IDNO = ISNULL(@IDNO,'')
	set @ORGID = ISNULL(@ORGID,'')
	set @TaishinNO = ISNULL(@TaishinNO,'')

	IF @LogID IS NULL OR @LogID = ''
	BEGIN
		SET @xError=1
		set @ErrorMsg = 'LogID必填'
		SET @ErrorCode = 'ERR254'--LogID必填
	END	

	if @IDNO = '' or @ORGID = '' or @TaishinNO = ''
	begin
	   set @xError = 1
	   set @ErrorMsg = 'IDNO, ORGID, TaishinNO 必填'
	   set @ErrorCode = 'ERR257' --參數遺漏
	end

	BEGIN TRY 
		BEGIN TRAN   
		   update w
		   set w.ShowFLG = 0 --APP上是否顯示：0:隱藏,1:顯示
		   from TB_WalletTradeMain w
		   where w.ORGID = @ORGID and w.IDNO = @IDNO
		   and w.SEQNO = @SEQNO and w.TaishinNO = @TaishinNO
		COMMIT TRAN 
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		SET @xError=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	if @xError <> 0 --sp錯誤log
		exec dbo.usp_SpSubsLog @FunName, @spIn, null,@xError, @ErrorCode, @ErrorMsg, @SQLExceptionCode, @SQLExceptionMsg

END



