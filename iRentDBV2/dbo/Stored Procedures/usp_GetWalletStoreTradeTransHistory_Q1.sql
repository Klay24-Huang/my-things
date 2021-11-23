/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetWalletStoreTradeTransHistory_Q1
* 系    統 : IRENT
* 程式功能 : 取得電子錢包歷史紀錄
* 作    者 : eason
* 撰寫日期 : 20210817
* 修改日期 : 20210903 UPD BY AMBER REASON: 調整F_INFNO改為抓取TaishinNO(台新交易IR編)欄位
			 20210910 UPD BY YANKEY REASON: 調整Key值欄位、TradeNote輸出文字
			 20211117 UPD BY YEH REASON:整理程式碼、增加Token檢核機制
			 20211118 UPD BY YEH REASON:增加和泰PAY
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetWalletStoreTradeTransHistory_Q1]
(   
	@MSG		VARCHAR(10)		OUTPUT	,
	@IDNO		VARCHAR(20)				,	-- 帳號
    @Token		VARCHAR(1024)			,	-- Token
	@SD			DATETIME				,	-- 查詢起日
	@ED			DATETIME				,	-- 查詢迄日
	@LogID		BIGINT
)
AS
BEGIN
    SET NOCOUNT ON

	DECLARE @Error INT = 0;
    DECLARE	@ErrorCode VARCHAR(6) = '0000';
    DECLARE	@ErrorMsg NVARCHAR(100) = 'SUCCESS';
    DECLARE	@SQLExceptionCode VARCHAR(10) = '';
    DECLARE	@SQLExceptionMsg NVARCHAR(1000) = '';
	DECLARE @IsSystem TINYINT = 1;
	DECLARE @ErrorType TINYINT = 4;
	DECLARE @FunName VARCHAR(50) = 'usp_GetWalletStoreTradeTransHistory_Q1';
	DECLARE @hasData TINYINT = 0;
	DECLARE @NowTime DATETIME = DATEADD(HOUR,8,GETDATE());

	BEGIN TRY
	    SET @IDNO = ISNULL(@IDNO,'');
		SET @Token = ISNULL(@Token,'');
		SET @LogID = ISNULL(@LogID,0);

		IF @IDNO = '' OR @Token ='' OR @LogID = 0 OR @SD is null OR @ED is null
		BEGIN
			SET @Error=1;
			SET @ErrorMsg = 'LogID必填';
			SET @ErrorCode = 'ERR900';
		END

		IF @Error = 0
		BEGIN
			SET @SD = DATEADD(second, DATEPART(SECOND, @SD)*-1,@SD);	--去秒數
			SET @ED = DATEADD(second, DATEPART(SECOND, @ED)*-1,@ED);	--去秒數

			DECLARE @days int = datediff(day,@SD,@ED);
			IF @days > 90
			BEGIN
				SET @Error = 1;
				SET @ErrorMsg = '最多查詢90天';
				SET @ErrorCode = 'ERR914';
			END
		END

		IF @Error=0
		BEGIN
			SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND Rxpires_in>@NowTime;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
			ELSE
			BEGIN
				SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND MEMIDNO=@IDNO;
				IF @hasData=0
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR101';
				END
			END
		END

		IF @Error = 0
		BEGIN
			SELECT tm.SEQNO
				,tm.TradeType
				,tm.TradeKey 
				,tm.TradeDate
				,c.Code0
				,c.CodeName
				,c.Negative
				,TradeAMT = (CASE WHEN c.Negative = 1 THEN -1 * tm.TradeAMT ELSE tm.TradeAMT END)	--Negative:1表負項，0表正項
				,TradeNote = (CASE WHEN c.Code0 = 'Store_Credit' THEN '信用卡 ' + tm.TradeKey --卡號
								WHEN c.Code0 = 'Store_Account' THEN '虛擬帳號轉帳'
								WHEN c.Code0 = 'Store_Shop' THEN '超商繳款'
								WHEN c.Code0 = 'Store_Return' THEN '合約退款'
								WHEN c.Code0 = 'Store_Trans' THEN '轉入 ' + (SELECT dbo.FN_BlockName((SELECT me.MEMCNAME FROM TB_MemberData me WITH(NOLOCK) WHERE me.MEMIDNO=tm.TradeKey),'O')) 	--姓名
								WHEN c.Code0 = 'Withdraw' THEN '餘額提領'
								WHEN c.Code0 = 'Pay_Car' THEN 'H'+ tm.TradeKey	--單號
								WHEN c.Code0 = 'Pay_Motor' THEN 'H'+ tm.TradeKey	--單號
								WHEN c.Code0 = 'Pay_Monthly' THEN (SELECT mr.ProjNM FROM SYN_MonthlyRent mr WITH(NOLOCK) WHERE MonthlyRentId = CAST(tm.TradeKey as bigint))
								WHEN c.Code0 = 'Cancel' THEN '付款取消'
								WHEN c.Code0 = 'Give_Trans' THEN '轉出 ' + (SELECT dbo.FN_BlockName((SELECT me.MEMCNAME FROM TB_MemberData me WITH(NOLOCK) WHERE me.MEMIDNO=tm.TradeKey),'O'))	--姓名
								WHEN c.Code0 = 'Store_HotaiPay' THEN '和泰PAY ' + tm.TradeKey	-- 20211118 UPD BY YEH REASON:增加和泰PAY
								ELSE '其他' END
							)
				,tm.ShowFLG
			FROM TB_WalletTradeMain tm WITH(NOLOCK)
			INNER JOIN TB_WalletCodeTable c WITH(NOLOCK) ON c.CodeGroup = 'TradeType' AND c.Code0 = tm.TradeType
			WHERE tm.IDNO = @IDNO
			AND tm.TradeDate >= @SD AND tm.TradeDate <= @ED
			AND tm.ShowFLG = 1
			ORDER BY tm.SEQNO DESC;
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
	SELECT @ErrorCode [ErrorCode], @ErrorMsg [ErrorMsg], @SQLExceptionCode [SQLExceptionCode], @SQLExceptionMsg [SQLExceptionMsg], @Error [Error]
END
GO