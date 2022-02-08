/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_SaveSubsInvno_U01
* 系    統 : IRENT
* 程式功能 : 訂閱發票儲存
* 作    者 : ADAM
* 撰寫日期 : 20210713
* 修改日期 : 20220122 ADD BY AMBER REASON.新增PRGID參數&六兄弟
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_SaveSubsInvno_U01]
(
	@IDNO			VARCHAR(20),
	@LogID			BIGINT,
	@MonProjID		VARCHAR(10),
	@MonProPeriod	INT,
	@ShortDays		INT,
	@NowPeriod		SMALLINT,
	@PayTypeId		BIGINT = 0,
	@InvoTypeId		BIGINT = 0,
	@InvoiceType	VARCHAR(5),
	@CARRIERID		VARCHAR(20),
	@UNIMNO			VARCHAR(10),
	@NPOBAN			VARCHAR(10),
	@Invno			VARCHAR(10),
	@InvoicePrice	INT,
	@InvoiceDate	VARCHAR(10),
	@PRGID       	VARCHAR(50),                    --20220122 ADD BY AMBER
	@xError                 INT             OUTPUT,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
)
AS
BEGIN
	SET NOCOUNT ON

	SET @xError = 0
    SET	@ErrorCode  = '0000'	
    SET	@ErrorMsg   = 'SUCCESS'	
    SET	@SQLExceptionCode = ''		
    SET	@SQLExceptionMsg = ''	

	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_SaveSubsInvno_U01'
	DECLARE @NOW DATETIME = dbo.GET_TWDATE()
	DECLARE @IsMotor TINYINT
	DECLARE @MonthlyRentID INT
	declare @spIn nvarchar(max), @SpNote nvarchar(max) = ''
	DECLARE @NowTime DATETIME =dbo.GET_TWDATE()

	SET @PRGID=ISNULL(@PRGID,'')
	
	BEGIN TRY
		select @spIn = isnull((
		select @IDNO[IDNO], @LogID[LogID], @MonProjID[MonProjID], @MonProPeriod[MonProPeriod], @ShortDays[ShortDays],
		@NowPeriod[NowPeriod], @PayTypeId[PayTypeId], @InvoTypeId[InvoTypeId],
		@InvoiceType[InvoiceType], @CARRIERID[CARRIERID], @UNIMNO[UNIMNO],
		@NPOBAN[NPOBAN],@Invno[Invno],@InvoicePrice[InvoicePrice],@InvoiceDate[InvoiceDate],@PRGID[PRGID]
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),'{}')

		IF @LogID IS NULL OR @LogID = ''
		BEGIN
			SET @xError = 1
			SET @ErrorMsg = 'LogID必填'
			SET @ErrorCode = 'ERR254'
		END

		IF @IDNO = '' OR @MonProjID = '' OR @MonProPeriod = 0 OR @NowPeriod = 0
		BEGIN
			SET @xError=1
			SET @ErrorMsg = '參數遺漏'
			SET @ErrorCode = 'ERR257' 
		END

		IF @xError = 0
		BEGIN
			declare @match_SubsId bigint = 0
			select top 1 @match_SubsId = m.SubsId from SYN_MonthlyRent m with(nolock)
			where m.useFlag = 1 and m.MonType = 2 and m.IDNO = @IDNO and @NOW between m.StartDate and m.EndDate
			and m.ProjID = @MonProjID and m.MonProPeriod = @MonProPeriod and m.ShortDays = @ShortDays 

			IF @match_SubsId = 0
			BEGIN
				SET @xError = 1
				SET @ErrorMsg = '無待升月租檔案'
				SET @ErrorCode = 'ERR258'
			END

			IF @xError = 0
			BEGIN
				select ROW_NUMBER() OVER(ORDER BY m.StartDate ASC) as mPeriod,
				IsNowPeriod = case when @NOW between m.StartDate and m.EndDate then 1 else 0 end,
				m.SubsId, m.MonthlyRentId, m.ProjID, m.MonProPeriod, m.ShortDays,
				m.WorkDayHours, m.HolidayHours, m.MotoTotalHours,
				m.StartDate, m.EndDate
				into #Tmp_MonRent
				from SYN_MonthlyRent m with(nolock)
				where m.SubsId = @match_SubsId 
				order by m.StartDate asc

				--20210821 ADD BY ADAM REASON.現階段改由用期數去判斷
				select top 1 @MonthlyRentID=MonthlyRentId from #Tmp_MonRent t where t.mPeriod = @NowPeriod		

				--寫入發票檔
				INSERT INTO TB_InvoiceHist
				(
					[IDNO], [TradeType], [TradeNo], [TradeCode], [bill_option], [title], [unified_business_no], 
					[invoiceAddress], [invoiceCode], [CARRIERID], [NPOBAN], [invoice_price], [invoice_date], 
					[REFNO], [InvoiceType], [useFlag], [isShow], [MKTime], [UPDTime],A_PRGID,A_USERID,U_PRGID,U_USERID
				)
				VALUES
				(
					@IDNO, '14', @MonthlyRentID, '', @InvoiceType, '', @UNIMNO, 
					'', @Invno, @CARRIERID, @NPOBAN, @Invoiceprice, @InvoiceDate,
					'', @InvoTypeId, 1, 1,@NowTime, @NowTime,@PRGID,@IDNO,@PRGID,@IDNO
				)

				IF @@ERROR <> 0 
				BEGIN
					SET @xError = 1
					SET @ErrorMsg = '寫入失敗'
					SET @ErrorCode = 'ERR999'
				END

				if @NowPeriod > 1
				begin 
				    Set @PRGID = Left(@PRGID,20)
					UPDATE TB_OrderAuthMonthly set InvJob=1,U_PRGID=@PRGID,U_USERID=@IDNO,U_SYSDT=@NowTime where MonthlyRentId=@MonthlyRentID

					INSERT INTO TB_OrderAuthMonthly_LOG 
					SELECT 'U',* FROM TB_OrderAuthMonthly WITH(NOLOCK) WHERE MonthlyRentId=@MonthlyRentID
				end
			END
		END
		
	END TRY
	BEGIN CATCH
		--ROLLBACK TRAN
		SET @xError=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH	

	if @xError <> 0 --sp錯誤log
		exec dbo.usp_SpSubsLog @FunName, @spIn, null,@xError, @ErrorCode, @ErrorMsg, @SQLExceptionCode, @SQLExceptionMsg, @SpNote
END



