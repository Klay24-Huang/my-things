/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetWalletStoreTradeTransHistory_Q1
* 系    統 : IRENT
* 程式功能 : 取得電子錢包歷史紀錄
* 作    者 : eason
* 撰寫日期 : 20210817
* 修改日期 : 20210903 UPD BY AMBER REASON: 調整F_INFNO改為抓取TaishinNO(台新交易IR編)欄位
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetWalletStoreTradeTransHistory_Q1]
(   
	@MSG			VARCHAR(10) OUTPUT,
	@IDNO			VARCHAR(20)       , --身分證號
    @LogID			BIGINT,
	@SD             DATETIME,
	@ED             DATETIME
)
AS
BEGIN
    SET NOCOUNT ON
	DECLARE @Error INT = 0
    DECLARE	@ErrorCode VARCHAR(6) = '0000'	
    DECLARE	@ErrorMsg  		   NVARCHAR(100) = 'SUCCESS'	
    DECLARE	@SQLExceptionCode  VARCHAR(10) = ''		
    DECLARE	@SQLExceptionMsg   NVARCHAR(1000) = ''	
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_GetWalletStoreTradeTransHistory_Q1'

	drop table if exists #Tmp_TradeMain

	BEGIN TRY
	    set @LogID = isnull(@LogID,'')
	    set @IDNO = isnull(@IDNO,'')

		IF @LogID = ''
		BEGIN
			SET @Error=1
			set @ErrorMsg = 'LogID必填'
			SET @ErrorCode = 'ERR254'
		END

		if @Error = 0
		begin
			IF @IDNO = ''
			BEGIN
				SET @Error=1
				set @ErrorMsg = 'IDNO必填'
				SET @ErrorCode = 'ERR256'
			END
		end

		if @Error = 0
		begin
		   if @SD is null or @ED is null
		   begin
		      set @Error = 1
			  set @ErrorMsg = 'SD, ED 必填'
			  set @ErrorCode = 'ERR257' --參數遺漏
		   end
		end

		if @Error = 0
		begin
		  	set @SD = DATEADD(second, DATEPART(SECOND, @SD)*-1,@SD)--去秒數
	        set @ED = DATEADD(second, DATEPART(SECOND, @ED)*-1,@ED)--去秒數

			 declare @days int = datediff(day,@SD,@ED)
			 if @days > 90
			 begin
			    set @Error = 1
				set @ErrorMsg = '最多查詢90天'
				set @ErrorCode = 'ERR914'
			 end
		end

		IF @Error = 0
		BEGIN
		   select distinct 
		   tm.ORGID, tm.IDNO, tm.SEQNO, tm.TaishinNO --key保留
		   ,tm.TradeType, tm.TradeKey 
		   ,tm.TradeDate
		   ,CardNo = isnull((select top 1  --卡號末4碼
			   SUBSTRING(b.CardNumber, len(b.CardNumber)-3,len(b.CardNumber)) 
			   from TB_MemberCardBinding b with(nolock)
			   where b.IDNO = tm.IDNO and b.IsValid = 1),'')
           ,c.Code0, c.CodeName, c.Negative
		   ,tm.TradeAMT, td.PayType		
		   into #Tmp_TradeMain
		   from TB_WalletTradeMain tm with(nolock)
		   left join TB_WalletTradeDetail td with(nolock) 
		   on td.ORGID = tm.ORGID and td.IDNO = tm.IDNO and td.SEQNO = tm.SEQNO and td.TaishinNO = tm.TaishinNO
		   join TB_WalletCodeTable c with(nolock) on c.CodeGroup = 'TradeType' and c.Code0 = tm.TradeType
		   where tm.ShowFLG = 1 and tm.IDNO = @IDNO 
		   and tm.TradeDate >= @SD and tm.TradeDate <= @ED

		   declare @TmpCount int = 0
		   select @TmpCount = COUNT(*) from #Tmp_TradeMain
		   if @TmpCount > 0
		   begin
		      select distinct
			   t.ORGID, t.IDNO, t.SEQNO, t.TaishinNO --key保留
			   ,t.TradeType, t.TradeKey 
			   ,t.TradeDate
			   ,t.CardNo  
			   ,t.Code0, t.CodeName, t.Negative
			   ,TradeAMT = (case 
			       when t.Negative = 1 then -1 * t.TradeAMT --Negative:1表負項，0表正項
			       else t.TradeAMT end)
			   ,TradeNote = (case 
			      when t.Code0 = 'Store_Credit' then '信用卡*' + CAST(t.CardNo as nvarchar) --卡號
			      when t.Code0 = 'Store_Account' then '虛擬帳號轉帳'
				  when t.Code0 = 'Store_Shop' then '超商繳款'
				  when t.Code0 = 'Store_Return' then '合約退款'
				  when t.Code0 = 'Store_Trans' then  ('轉贈人 '+
				    (select dbo.FN_BlockName((select top 1 me.MEMCNAME from TB_MemberData me where me.MEMIDNO = t.TradeKey),'O')) --姓名
				  )				  
				  when t.Code0 = 'Withdraw' then '電子錢包提領'
				  when t.Code0 = 'Pay_Car' then 'H'+ CAST(t.TradeKey as nvarchar)--單號
				  when t.Code0 = 'Pay_Motor' then 'H'+ CAST(t.TradeKey as nvarchar)--單號
				  when t.Code0 = 'Pay_Monthly' then(
					   select top 1 mr.ProjNM from SYN_MonthlyRent mr where MonthlyRentId = CAST(t.TradeKey as bigint))
				  when t.Code0 = 'Cancel' then '電子錢包付款取消'
				  when t.Code0 = 'Give_Trans' then ('電子錢包贈與 ' +
				    (select dbo.FN_BlockName((select top 1 me.MEMCNAME from TB_MemberData me where me.MEMIDNO = t.TradeKey),'O')) --姓名			     
				  )
				  else '其他' end     
			   )
			   from #Tmp_TradeMain t
			   order by t.SEQNO desc
		   end
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

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	--輸出系統訊息
	SELECT @ErrorCode[ErrorCode], @ErrorMsg[ErrorMsg], @SQLExceptionCode[SQLExceptionCode], @SQLExceptionMsg[SQLExceptionMsg], @Error[Error]

	drop table if exists #Tmp_TradeMain
END



