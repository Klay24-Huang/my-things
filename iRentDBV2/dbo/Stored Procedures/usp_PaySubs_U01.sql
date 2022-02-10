/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_PaySubs_U01
* 系    統 : IRENT
* 程式功能 : 訂閱制付費-單筆
* 作    者 : eason
* 撰寫日期 : 20210329
* 修改日期 : 20220122 ADD BY AMBER REASON.新增PRGID參數&六兄弟
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_PaySubs_U01]
(   
	@IDNO			VARCHAR(20)       , --身分證號
    @LogID			BIGINT,
	@MonProjID      VARCHAR(10)       ,
	@MonProPeriod   INT               ,
	@ShortDays      INT               ,
	@PayTypeId      bigint = 0        ,--付款方式
	@InvoTypeId     bigint = 0        ,--發票設定
	@SetNow         DATETIME = null   ,		
	@MerchantTradeNo		VARCHAR(30),	--20210709 ADD BY ADAM REASON.補上台新IR編
	@TaishinTradeNo			VARCHAR(50),	--20210709 ADD BY ADAM REASON.補上台新IR編
	@PRGID                  VARCHAR(50),     --20220122 ADD BY AMBER	
	@xError                  INT             OUTPUT,
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
		select @IDNO[IDNO], @LogID[LogID], @MonProjID[MonProjID], @MonProPeriod[MonProPeriod], @ShortDays[ShortDays],@SetNow[SetNow],@PRGID[PRGID]
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),'{}')
	end

 	SET @xError = 0
    SET	@ErrorCode  = '0000'	
    SET	@ErrorMsg   = 'SUCCESS'	
    SET	@SQLExceptionCode = ''		
    SET	@SQLExceptionMsg = ''		
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_PaySubs_U01'
	DECLARE @NOW DATETIME = iif(@SetNow is null, DATEADD(HOUR, 8, GETDATE()),@SetNow)
	SET @PRGID=ISNULL(@PRGID,'')
	DECLARE @NowTime DATETIME = dbo.GET_TWDATE()

	IF @LogID IS NULL OR @LogID = ''
	BEGIN
		SET @xError=1
		set @ErrorMsg = 'LogID必填'
		SET @ErrorCode = 'ERR254'
	END	

	IF @IDNO = '' OR @MonProjID = '' OR @MonProPeriod = 0 OR @PayTypeId = 0 OR @InvoTypeId = 0  
	BEGIN
		SET @xError=1
		set @ErrorMsg = 'IDNO,MonProjID,MonProPeriod,PayTypeId,InvoTypeId必填'
		SET @ErrorCode = 'ERR257' --參數遺漏
	END

	BEGIN TRY 
		BEGIN TRAN   
	    
			declare @NowMonRentId bigint =0
			declare @MRSDetailID bigint = 0
			declare @SubsID bigint = 0

			select top 1 @SubsID = mr.SubsId, @NowMonRentId=mr.MonthlyRentId from SYN_MonthlyRent mr with(nolock)
			where mr.useFlag = 1 and mr.MonType = 2 and mr.IDNO = @IDNO
			and mr.ProjID = @MonProjID and mr.MonProPeriod = @MonProPeriod and mr.ShortDays = @ShortDays
			and @NOW between mr.StartDate and mr.EndDate

			if @SubsID = 0 or @NowMonRentId =0
			begin
			   set @xError = 1
			   set @ErrorMsg = '月租不存在'
			   set @ErrorCode = 'ERR258' --月租不存在
			end

			declare @MonRentCount int = 0
			declare @MRSDetailCount int =0
			if @xError = 0
			begin
			    select @MonRentCount = count(*) from SYN_MonthlyRent m with(nolock) where m.SubsId = @SubsID
			    select @MRSDetailCount = count(*) from TB_MonthlyRentSetDetail d with(nolock) where
				d.MonProjID = @MonProjID and d.MonProPeriod = @MonProPeriod and d.ShortDays = @ShortDays			

			if @MRSDetailCount = 0 
				begin
				   set @xError = 1
				   set @ErrorMsg = 'MRSDetail不存在'
				   set @ErrorCode = 'ERR261' --專案不存在
				end
			end

			if @xError = 0
			begin
			   if @MonRentCount <> @MRSDetailCount
			   begin
			       set @xError = 1
				   set @ErrorMsg = '月租與明細設定筆數不相等'
				   set @ErrorCode = 'ERR272' --月租與明細設定筆數不相等
			   end
			end

			if @xError = 0
			begin			 
			    ;with tmp as(
				select ROW_NUMBER() OVER(ORDER BY mr.StartDate ASC) AS rowId, 
				nowPeriod = case when mr.MonthlyRentId = @NowMonRentId then 1 else 0 end, mr.*
				from SYN_MonthlyRent mr with(nolock)
				where mr.SubsId = @SubsID)	
				select top 1  @MRSDetailID = d.MRSDetailID 
				from tmp t
				join TB_MonthlyRentSetDetail d on d.Period = t.rowId
				and d.MonProjID = @MonProjID and d.MonProPeriod = @MonProPeriod and d.ShortDays = @ShortDays
				where t.nowPeriod = 1
			end

			if @xError = 0
			begin
			   if @MRSDetailID = 0
			   begin
				   SET @xError=1
				   set @ErrorMsg = '月租設定明細不存在'
				   SET @ErrorCode = 'ERR271' --月租設定明細不存在
			   end
			end

			if @xError = 0
			begin
			   declare @MonthlyPayCk int =0
			   select @MonthlyPayCk = count(*) from TB_MonthlyPay p with(nolock) 
               where p.MonthlyRentId = @NowMonRentId and p.MRSDetailID = @MRSDetailID and p.IDNO = @IDNO
		   
			   if @MonthlyPayCk>0
			   begin
			       SET @xError=1
				   set @ErrorMsg = '資料已存在'
				   SET @ErrorCode = 'ERR273' --資料已存在
			   end
			end

			if @xError = 0
			begin
			   insert into TB_MonthlyPay (MonthlyRentId,MRSDetailID,IDNO,ActualPay,PayDate,PayTypeId,InvoTypeId
			   ,MerchantTradeNo,TaishinTradeNo		--20210709 ADD BY ADAM REASON.補上台新IR編
			   ,A_PRGID,A_USERID,MKTime,U_PRGID,U_USERID,UPDTime)
			   values (@NowMonRentId, @MRSDetailID, @IDNO,1,@NOW,@PayTypeId,@InvoTypeId
			   ,@MerchantTradeNo,@TaishinTradeNo		--20210709 ADD BY ADAM REASON.補上台新IR編
			   ,@PRGID,@IDNO,@NowTime,@PRGID,@IDNO,@NowTime
			   )
			end

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



