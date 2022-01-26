/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_ArrearsPaySubs_U01
* 系    統 : IRENT
* 程式功能 : 月租欠費繳交
* 作    者 : eason
* 撰寫日期 : 20210525
* 修改日期 : 20220122 ADD BY AMBER REASON.新增PRGID參數&六兄弟
Example :
begin tran
declare @xError INT=0,@ErrorCode VARCHAR(6)='',@ErrorMsg NVARCHAR(200)=''
exec usp_ArrearsPaySubs_U1 'A122364317',99999,'2491,2561',0,0,null,'','',@xError output,@ErrorCode output,@ErrorMsg output,'',''
select @xError,@ErrorCode,@ErrorMsg
select * FROM TB_MonthlyPay WITH(NOLOCK) WHERE IDNO='A122364317'
rollback tran
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_ArrearsPaySubs_U01]
(   
	@IDNO			   VARCHAR(20)       , --身分證號
    @LogID			   BIGINT,
    @MonthlyRentIds    VARCHAR(MAX) = '' ,  
	@PayTypeId      bigint = 0        ,--付款方式
	@InvoTypeId     bigint = 0        ,--發票設定
	@SetNow            DATETIME = null   ,
	@MerchantTradeNo		VARCHAR(30),	--20210709 ADD BY ADAM REASON.補上台新IR編
	@TaishinTradeNo			VARCHAR(50),	--20210709 ADD BY ADAM REASON.補上台新IR編
	@PRGID			        VARCHAR(50),	--20220122 ADD BY AMBER
	@xError                 INT              OUTPUT,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息	
)
AS
BEGIN
    SET NOCOUNT ON

	drop table if exists #Tb_MRent

	declare @spIn nvarchar(max), @SpOut nvarchar(max)--splog
	begin 		
		select @spIn = isnull((
		select @IDNO[IDNO], @LogID[LogID], @MonthlyRentIds[MonthlyRentIds],
		@PayTypeId[PayTypeId], @InvoTypeId[InvoTypeId], @SetNow[SetNow],
		@MerchantTradeNo[MerchantTradeNo], @TaishinTradeNo[TaishinTradeNo],@PRGID[PRGID]
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),'{}')
	end

 	SET @xError = 0
    SET	@ErrorCode  = '0000'	
    SET	@ErrorMsg   = 'SUCCESS'	
    SET	@SQLExceptionCode = ''		
    SET	@SQLExceptionMsg = ''		
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_ArrearsPaySubs_U1'
	DECLARE @NOW DATETIME = iif(@SetNow is null, DATEADD(HOUR, 8, GETDATE()),@SetNow)

	set @MonthlyRentIds = isnull(@MonthlyRentIds,'')
	SET @PRGID=ISNULL(@PRGID,'')
	DECLARE @NowTime DATETIME= dbo.GET_TWDATE()
	declare @ckCount int = 0

	declare @Tb_MonIds table(MonId bigint)
	declare @Tb_MonIds_count int = 0

	IF @LogID IS NULL OR @LogID = ''
	BEGIN
		SET @xError=1
		set @ErrorMsg = 'LogID必填'
		SET @ErrorCode = 'ERR254'--LogID必填
	END	

	if @xError = 0
	begin
		IF @IDNO = '' 
		BEGIN
			SET @xError=1
			set @ErrorMsg = 'IDNO必填'
			SET @ErrorCode = 'ERR257' --參數遺漏
		END
	end
	
	if @xError = 0
	begin
		IF @MonthlyRentIds = ''
		BEGIN
			SET @xError=1
			SET @ErrorMsg = 'MonthlyRentId必填'
			SET @ErrorCode = 'ERR916'
		END
	end
	
	if @xError = 0
	begin  
	   begin try
		   insert into @Tb_MonIds 
		   select cast(m.sValue as bigint)	   
		   from dbo.FN_StrListToTb(@MonthlyRentIds) m

		   select @Tb_MonIds_count = count(*) from @Tb_MonIds 
	   end try
	   begin catch
	       set @xError = 1
		   set @ErrorMsg = 'MonthlyRentIds內容錯誤'
		   set @ErrorCode = 'ERR916'
	   end catch
	end

	declare @MonthRentCount int = 0   
	if @xError = 0
	begin
		select m.*
		into #Tb_MRent
		from SYN_MonthlyRent m with(nolock)	 
		where m.useFlag = 1 and m.MonType = 2 and m.IDNO = @IDNO and 
		m.MonthlyRentId = case when @Tb_MonIds_count > 0 
		  then (select top 1 MonId from @Tb_MonIds where MonId = m.MonthlyRentId) 
		  else m.MonthlyRentId end

		select @MonthRentCount = count(*) from #Tb_MRent

		if @MonthRentCount = 0
		begin
			set @xError = 1
			set @ErrorMsg = 'MonthlyRent不存在'
			set @ErrorCode = 'ERR909'
	    end
	end	

	if @xError = 0
	begin
	   set @ckCount = 0
	   ;with tmp as(
	      select  ROW_NUMBER() over(partition by m.SubsId order by m.StartDate) rw, m.* 
		  from SYN_MonthlyRent m with(nolock)	
		  where m.SubsId in (select t0.SubsId from #Tb_MRent t0)
	   )
	   select @ckCount = count(*)
	   from tmp t
	   left join TB_MonthlyRentSetDetail d with(nolock)	
	   on d.MonProjID = t.ProjID and d.MonProPeriod = t.MonProPeriod 
	   and d.ShortDays = t.ShortDays and d.Period = t.rw
	   where isnull(d.MRSDetailID,0) = 0 

	   if @ckCount > 0
	   begin
	      set @ErrorCode = 1
		  set @ErrorMsg = 'MRSetDetail數量不同'
		  set @ErrorCode = 'ERR914'
	   end
	end

	BEGIN TRY 
	  BEGIN TRAN   	 
	  
	    if @xError = 0
		begin
            ;with tmp as (
				select ROW_NUMBER() over(partition by m.SubsId order by m.StartDate) rw, m.* 
				from SYN_MonthlyRent m with(nolock)	
				where m.SubsId in (select m0.SubsId from #Tb_MRent m0))
            insert into TB_MonthlyPay 
			(MonthlyRentId,MRSDetailID,IDNO,ActualPay,PayDate,PayTypeId,InvoTypeId,
			MerchantTradeNo,TaishinTradeNo,A_PRGID,A_USERID,MKTime,U_PRGID,U_USERID,UPDTime)
			select distinct
			t.MonthlyRentId, d.MRSDetailID, @IDNO, 1,@NowTime, @PayTypeId, @InvoTypeId,
			@MerchantTradeNo, @TaishinTradeNo,@PRGID,@IDNO,@NowTime,@PRGID,@IDNO,@NowTime
			from tmp t
			join TB_MonthlyRentSetDetail d
			on d.MonProjID = t.ProjID and d.MonProPeriod = t.MonProPeriod 
			and d.ShortDays = t.ShortDays and d.Period = t.rw		
			and t.MonthlyRentId = case when @Tb_MonIds_count > 0 
			    then (select top 1 MonId from @Tb_MonIds where MonId = t.MonthlyRentId) 
				else t.MonthlyRentId end
			left join TB_MonthlyPay p
			on p.MonthlyRentId = t.MonthlyRentId and p.MRSDetailID = d.MRSDetailID
			and p.IDNO = @IDNO
			where isnull(p.ActualPay,0) = 0 
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

	drop table if exists #Tb_MRent

END


