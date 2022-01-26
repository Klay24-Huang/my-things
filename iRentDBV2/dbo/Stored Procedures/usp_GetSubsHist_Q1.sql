/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetSubsHist_Q1
* 系    統 : IRENT
* 程式功能 : 取得訂閱制歷史紀錄
* 作    者 : eason
* 撰寫日期 : 20210426
* 修改日期 : 20210525 ADD BY ADAM REASON.增加城市車手
             20220125 ADD BY AMBER REASON.調整invoice_date輸出格式
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetSubsHist_Q1]
(   
	@MSG			VARCHAR(10) OUTPUT,
	@IDNO			VARCHAR(20)       , --身分證號
    @LogID			BIGINT,
	@SetNow         DATETIME = null   
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
	DECLARE @FunName VARCHAR(50) = 'usp_GetSubsHist_Q1'
	DECLARE @NOW DATETIME = iif(@SetNow is null, DATEADD(HOUR, 8, GETDATE()),@SetNow)
	DECLARE @str_now varchar(10) = convert(varchar, @Now, 112) 

	BEGIN TRY
	    set @LogID = isnull(@LogID,'')
	    set @IDNO = isnull(@IDNO,'')

		IF @LogID = ''
		BEGIN
			SET @Error=1
			set @ErrorMsg = 'LogID必填'
			SET @ErrorCode = 'ERR254'
		END

		IF @IDNO = ''
		BEGIN
			SET @Error=1
			set @ErrorMsg = 'IDNO必填'
			SET @ErrorCode = 'ERR256'
		END

		IF @Error = 0
		BEGIN

		   --過期訂閱
			select distinct 
			s.MonProjID, s.MonProPeriod, s.ShortDays, s.MonProjNM
			--,s.PeriodPrice,
			,PeriodPrice = sd.PeriodPayPrice,	--20210720 ADD BY ADAM REASON.為解決兩期一付問題，首期金額改由DETAIL去找
			--s.CarWDHours, s.CarHDHours, s.MotoTotalMins,
			CarWDHours = case when s.CarWDHours > 0 then s.CarWDHours else -999 end,
			CarHDHours = case when s.CarHDHours > 0 then s.CarHDHours else -999 end,
			MotoTotalMins = case when s.MotoTotalMins > 0 then s.MotoTotalMins else -999 end,
			s.IsMoto, m.StartDate, m.EndDate,
			m.MonthlyRentId,
			m.WorkDayRateForCar[WDRateForCar],
			m.HoildayRateForCar[HDRateForCar], 
			m.WorkDayRateForMoto[WDRateForMoto], 
			m.HoildayRateForMoto[HDRateForMoto], 
			p.PayDate, 
			PerNo = (select cast(tmp.rw as int) from(
				select ROW_NUMBER() OVER(order by m5.StartDate asc) as rw,m5.StartDate, m5.EndDate 
				from SYN_MonthlyRent m5 with(nolock) where m5.SubsId = m.SubsId) tmp
				where tmp.StartDate = m.StartDate and tmp.EndDate = m.EndDate
			),
			isnull(h.isShow,0)[isShow], h.TradeNo, --TradeNo對應MonthlyRentId
			InvType = (select top 1 c.CodeNm from TB_Code c with(nolock) where c.CodeId = h.InvoiceType), 
			h.unified_business_no, h.invoiceCode, convert(varchar(10),convert(date,h.invoice_date),111) as invoice_date, h.invoice_price ,　            --20220125 ADD BY AMBER REASON.invoice_date轉換成 yyyy/MM/dd格式
			IsMix = case when ((s.CarWDHours > 0 or s.CarHDHours > 0) and (s.MotoTotalMins > 0 or s.HDRateForMoto<2)) then 1 else 0 end	--20210525 ADD BY ADAM REASON.增加城市車手
			,h.CARRIERID ,h.NPOBAN,InvoiceType=C.MapCode, NPOBAN_Name=ISNULL(Love.LoveName, '')		--20210619 ADD BY ADAM REASON.補上載具條碼跟捐贈碼
			from SYN_MonthlyRent m with(nolock)
			join TB_MonthlyRentSet s on
			s.MonProjID = m.ProjID and s.MonProPeriod = m.MonProPeriod and s.ShortDays = m.ShortDays
			join TB_SubsMain sm on sm.SubsId = m.SubsId	
			left join TB_MonthlyPay p on p.MonthlyRentId = m.MonthlyRentId
			left join TB_InvoiceHist h on h.TradeNo = m.MonthlyRentId and h.TradeType = 14 and h.useFlag = 1 and isnull(h.isShow,0) = 1				 
			LEFT JOIN TB_LoveCode AS Love WITH(NOLOCK) ON Love.LoveCode=h.NPOBAN		--20210619 ADD BY ADAM REASON.補上載具條碼跟捐贈碼
			LEFT JOIN TB_Code AS C  WITH(NOLOCK) ON C.CodeId=h.InvoiceType
			 --20210720 ADD BY ADAM REASON.為解決兩期一付問題，首期金額改由DETAIL去找
			JOIN TB_MonthlyRentSetDetail sd WITH(NOLOCK) ON s.MonProjID=sd.MonProjID AND s.MonProPeriod=sd.MonProPeriod
			AND s.ShortDays=sd.ShortDays AND sd.Period=1
			where m.useFlag = 1 and m.IDNO = @IDNO and isnull(h.isShow,0) = 1		
			--and @NOW > m.EndDate  

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

END



