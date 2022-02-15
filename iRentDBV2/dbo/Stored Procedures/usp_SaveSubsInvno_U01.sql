/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_SaveSubsInvno_U01
* 系    統 : IRENT
* 程式功能 : 訂閱發票儲存
* 作    者 : ADAM
* 撰寫日期 : 20210713
* 修改日期 : 20220122 ADD BY AMBER REASON.新增PRGID參數&六兄弟
			 20220127 UPD BY YEH REASON:INPUT參數調整，調整存檔邏輯
			 20220210 ADD BY AMBER REASON:若重開成功,一併更新發票錯誤記錄檔
Example :
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_SaveSubsInvno_U01]
(
	@IDNO				VARCHAR(20)				,	-- 帳號
	@MonthlyRentID		BIGINT					,	-- MonthlyRentID
	@NowPeriod			INT						,	-- 目前期數
	@PayTypeId			BIGINT					,	-- 付款方式
	@InvoTypeId			BIGINT					,	-- 發票設定
	@InvoiceType		VARCHAR(5)				,	-- 發票寄送方式
	@CARRIERID			VARCHAR(20)				,	-- 載具條碼
	@UNIMNO				VARCHAR(10)				,	-- 統編
	@NPOBAN				VARCHAR(10)				,	-- 捐贈碼
	@Invno				VARCHAR(10)				,	-- 發票號碼
	@InvoicePrice		INT						,	-- 發票金額
	@InvoiceDate		VARCHAR(10)				,	-- 發票日期
	@PRGID       		VARCHAR(50)				,	-- 來源程式 20220122 ADD BY AMBER
	@LogID				BIGINT					,	-- 
	@xError				INT             OUTPUT	,
	@ErrorCode			VARCHAR(6)		OUTPUT	,	--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		--回傳sqlException訊息
)
AS
BEGIN
	SET NOCOUNT ON

	SET @xError = 0;
    SET	@ErrorCode  = '0000';
    SET	@ErrorMsg   = 'SUCCESS';
    SET	@SQLExceptionCode = '';
    SET	@SQLExceptionMsg = '';

	DECLARE @IsSystem TINYINT = 1;
	DECLARE @ErrorType TINYINT = 4;
	DECLARE @FunName VARCHAR(50) = 'usp_SaveSubsInvno_U01';
	DECLARE @spIn NVARCHAR(MAX), @SpNote NVARCHAR(MAX) = '';
	DECLARE @NowTime DATETIME = dbo.GET_TWDATE();

	SET @PRGID=ISNULL(@PRGID,'');
	SET @LogID=ISNULL(@LogID,0);
	
	BEGIN TRY
		SELECT @spIn = ISNULL((SELECT @IDNO IDNO, @MonthlyRentID MonthlyRentID, @NowPeriod NowPeriod, @PayTypeId PayTypeId,
								@InvoTypeId InvoTypeId, @InvoiceType InvoiceType, @CARRIERID CARRIERID, @UNIMNO UNIMNO, @NPOBAN NPOBAN,
								@Invno Invno, @InvoicePrice InvoicePrice, @InvoiceDate InvoiceDate, @PRGID PRGID, @LogID LogID
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),'{}');

		-- 20220127 UPD BY YEH REASON:INPUT增加@MonthlyRentID，調整存檔邏輯
		--寫入發票檔
		INSERT INTO TB_InvoiceHist
		(
			IDNO, TradeType, TradeNo, TradeCode, bill_option, title, unified_business_no, 
			invoiceAddress, invoiceCode, CARRIERID, NPOBAN, invoice_price, invoice_date, 
			REFNO, InvoiceType, useFlag, isShow, MKTime, UPDTime, A_PRGID, A_USERID, U_PRGID, U_USERID
		)
		VALUES
		(
			@IDNO, '14', @MonthlyRentID, '', @InvoiceType, '', @UNIMNO, 
			'', @Invno, @CARRIERID, @NPOBAN, @Invoiceprice, @InvoiceDate,
			'', @InvoTypeId, 1, 1,@NowTime, @NowTime, @PRGID, @IDNO, @PRGID, @IDNO
		);

		IF @NowPeriod > 1
		BEGIN 
			SET @PRGID = Left(@PRGID,20);
			UPDATE TB_OrderAuthMonthly SET InvJob=1,U_PRGID=@PRGID,U_USERID=@IDNO,U_SYSDT=@NowTime WHERE MonthlyRentId=@MonthlyRentID;

			INSERT INTO TB_OrderAuthMonthly_LOG 
			SELECT 'U',* FROM TB_OrderAuthMonthly WITH(NOLOCK) WHERE MonthlyRentId=@MonthlyRentID;
		END

		-- 20220210 ADD BY AMBER REASON:若重開成功,一併更新發票錯誤記錄檔
		IF EXISTS (SELECT 1 FROM TB_MonthlyInvErrLog WITH(NOLOCK) WHERE MonthlyRentID=@MonthlyRentID AND TransFlg='N')
		BEGIN
		  UPDATE TB_MonthlyInvErrLog SET TransFlg='Y',TransCount=TransCount+1,UPDTime=@NowTime,U_PRGID=@PRGID,U_USERID=@IDNO
		  WHERE MonthlyRentID=@MonthlyRentID;
		END

	END TRY
	BEGIN CATCH
		--ROLLBACK TRAN
		SET @xError=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH	

	IF @xError <> 0 --sp錯誤log
	BEGIN
		EXEC dbo.usp_SpSubsLog @FunName, @spIn, null,@xError, @ErrorCode, @ErrorMsg, @SQLExceptionCode, @SQLExceptionMsg, @SpNote
	END
END