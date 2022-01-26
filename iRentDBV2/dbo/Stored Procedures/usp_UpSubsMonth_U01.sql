/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_UpSubsMonth_U01
* 系    統 : IRENT
* 程式功能 : 訂閱制升轉
* 作    者 : eason
* 撰寫日期 : 2021-03-25
* 修改日期 : 20210709 ADD BY ADAM REASON.補上台新IR編
             20211005 ADD BY ADAM REASON.把升轉後之前的資料移除，並且更新TB_MonthlyPay的目前ID
			 20220122 ADD BY AMBER REASON.新增PRGID參數、補上更新三兄弟、排除已展期資料
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_UpSubsMonth_U01]
(   
	@IDNO			VARCHAR(20)       , --身分證號
    @LogID			BIGINT            ,
	@MonProjID      VARCHAR(10)       , 
	@MonProPeriod   INT               ,
	@ShortDays      INT               ,
	@UP_MonProjID      VARCHAR(10)    ,
	@UP_MonProPeriod   INT            ,
	@UP_ShortDays      INT            ,
	@PayTypeId      bigint = 0        ,--付款方式
	@InvoTypeId     bigint = 0        ,--發票設定
	@SetNow         DATETIME = null   ,
	@MerchantTradeNo		VARCHAR(30),	--20210709 ADD BY ADAM REASON.補上台新IR編
	@TaishinTradeNo			VARCHAR(50),	--20210709 ADD BY ADAM REASON.補上台新IR編
	@PRGID			        VARCHAR(50),	--20220122 ADD BY AMBER
	@xError                  INT             OUTPUT,
	@MonthlyRentId			BIGINT			OUTPUT, --20210714 ADD BY ADAM 
	@UPDPeriod				INT				OUTPUT, --20210715 ADD BY ADAM
	@OriSDATE				DATETIME		OUTPUT, --20210715 ADD BY ADAM
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
)
AS
BEGIN
    SET NOCOUNT ON

	declare @spIn nvarchar(max), @SpOut nvarchar(max), @SpNote nvarchar(max)=''--splog
	begin 		
		select @spIn = isnull((
		select @IDNO[IDNO], @LogID[LogID], @MonProjID[MonProjID], @MonProPeriod[MonProPeriod], @ShortDays[ShortDays],
		@UP_MonProjID[UP_MonProjID], @UP_MonProPeriod[UP_MonProPeriod], @UP_ShortDays[UP_ShortDays],
		@SetNow[SetNow],
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
	DECLARE @FunName VARCHAR(50) = 'usp_UpSubsMonth_U01'
	DECLARE @NOW DATETIME = iif(@SetNow is null, DATEADD(HOUR, 8, GETDATE()),@SetNow)
	declare @str_now varchar(10) = convert(varchar, @NOW, 112) 
	DECLARE @NowTime DATETIME = dbo.GET_TWDATE()

	SET @IDNO = ISNULL(@IDNO,'')
	SET @MonProjID = ISNULL(@MonProjID,'')
	SET @UP_MonProjID = ISNULL(@UP_MonProjID,'')
	SET @PRGID = ISNULL(@PRGID,'')

	drop table if exists #Tmp_MonRent

	BEGIN TRY
		BEGIN TRAN  tr_UpSubsMonth_U1   

		IF @LogID IS NULL OR @LogID = ''
		BEGIN
			SET @xError=1
			set @ErrorMsg = 'LogID必填'
			SET @ErrorCode = 'ERR254'
		END

		if @xError = 0
		begin
			IF @IDNO = '' OR @MonProjID = '' OR @MonProPeriod = 0 OR @UP_MonProjID = '' OR @UP_MonProPeriod = 0 
			BEGIN
				SET @xError=1
				SET @ErrorMsg = '參數遺漏'
				SET @ErrorCode = 'ERR257' 
			END
		end

		if @xError = 0
		begin
		   if @MonProPeriod <> @UP_MonProPeriod
			BEGIN
				SET @xError=1
				SET @ErrorMsg = '升轉期數需相同'
				SET @ErrorCode = 'ERR275' 
			END
		end

		declare @NowMonSetID int = 0 --待升專案ID
		declare @NxtMonSetID int = 0 --升級專案ID

		if @xError = 0
		begin
		   
		   select top 1 @NowMonSetID = s.MonSetID  from TB_MonthlyRentSet s with(nolock)
		   where s.MonProjID = @MonProjID and s.MonProPeriod = @MonProPeriod and s.ShortDays = @ShortDays

		   select top 1 @NxtMonSetID = s.MonSetID  from TB_MonthlyRentSet s with(nolock)
		   where s.MonProjID = @UP_MonProjID and s.MonProPeriod = @UP_MonProPeriod and s.ShortDays = @UP_ShortDays

		   if @NowMonSetID = 0 or @NxtMonSetID =0
			BEGIN
				SET @xError=1
				SET @ErrorMsg  = '待升,升級專案不存在'
				SET @ErrorCode = 'ERR909' 
			END
		end

		IF @xError = 0
		BEGIN	  

		  declare @match_SubsId bigint = 0
		  select top 1 @match_SubsId = m.SubsId from SYN_MonthlyRent m with(nolock)
		  where m.useFlag = 1 and m.MonType = 2 and m.IDNO = @IDNO and @NOW between m.StartDate and m.EndDate
		  and m.ProjID = @MonProjID and m.MonProPeriod = @MonProPeriod and m.ShortDays = @ShortDays 

		  if @match_SubsId = 0
		  begin
			SET @xError=1
			SET @ErrorMsg = '無待升月租檔案'
			SET @ErrorCode = 'ERR258' 
		  end

		  declare @Tmp_MonRent_count int = 0 		 
		  declare @nowPeriod int = 0  --目前期數

		  if @xError = 0
		  begin
			  select ROW_NUMBER() OVER(ORDER BY m.StartDate ASC) as mPeriod,
			  IsNowPeriod = case when @NOW between m.StartDate and m.EndDate then 1 else 0 end,
			  m.SubsId, m.MonthlyRentId, m.ProjID, m.MonProPeriod, m.ShortDays,
			  m.WorkDayHours, m.HolidayHours, m.MotoTotalHours,
			  m.StartDate, m.EndDate
			  into #Tmp_MonRent
			  from SYN_MonthlyRent m with(nolock)
			  where m.SubsId = @match_SubsId 
			  order by m.StartDate asc

			  select @Tmp_MonRent_count = count(*) from #Tmp_MonRent
			  select top 1 @nowPeriod = t.mPeriod from #Tmp_MonRent t where t.IsNowPeriod = 1

			  if @nowPeriod = 0 
			  begin
				set @xError = 1
				SET @ErrorMsg = '無對應目前期數月租檔'
				SET @ErrorCode = 'ERR259'
			  end
		  end

		  if @xError = 0 and @Tmp_MonRent_count = 0
		  begin
		    set @xError = 1
			SET @ErrorMsg = '無對應待升月租檔'
			SET @ErrorCode = 'ERR260'
		  end

		  if @xError = 0
		  begin
				--把目前期數回傳
				SET @UPDPeriod = @nowPeriod
				declare @SubsGroup bigint = 0
				select @SubsGroup = s.SubsGroup from TB_SubsMain s with(nolock)
				where s.SubsId = (select top 1 m.SubsId from #Tmp_MonRent m)

				--已用期數flag=0
				update m  
				set m.useFlag = 0, m.FlagCodeId = 1, --升轉
				m.U_PRGID=@PRGID,m.U_USERID=@IDNO,m.UPDTime=@NowTime
				from SYN_MonthlyRent m with(nolock)
				--where m.MonthlyRentId in (select t.MonthlyRentId from #Tmp_MonRent t where t.mPeriod <= @nowPeriod)
				--20211026 ADD BY ADAM REASON.孟耆說應該是更新當期
				where m.MonthlyRentId in (select t.MonthlyRentId from #Tmp_MonRent t where t.mPeriod = @nowPeriod)

			   --未使用期數月租刪除
			   delete m
			   from SYN_MonthlyRent m with(nolock)
			   where m.MonthlyRentId in (select t.MonthlyRentId from #Tmp_MonRent t where t.mPeriod > @nowPeriod)
		    
				--更新訂閱主檔
				update s
				set s.UpdPeriod = @nowPeriod,
				s.UpdPeriodTime = @NOW,
				s.U_PRGID=@PRGID,s.U_USERID=@IDNO,s.UPDTime=@NowTime
				from TB_SubsMain s with(nolock)
				where s.SubsId = (select top 1 t.SubsId from #Tmp_MonRent t)

				declare @BefNow datetime = (select top 1 t.StartDate from #Tmp_MonRent t where t.mPeriod = 1)
				set @OriSDATE = @BefNow

			  --建立新月租 		
			   declare @calsp_error int = -1
			   begin try
                   exec dbo.usp_CreateSubsMonth_U01
				   @IDNO, @LogID, 
				   @UP_MonProjID, @UP_MonProPeriod, @UP_ShortDays, 
				   @PayTypeId, @InvoTypeId, @BefNow, @SubsGroup,1 ,0,
				   @MerchantTradeNo, @TaishinTradeNo,@PRGID,
				   @calsp_error out,@MonthlyRentId out, @ErrorCode out,@ErrorMsg, @SQLExceptionCode out, @SQLExceptionMsg out 
			   
			      if @calsp_error <> 0
				  begin
					   set @SpNote += ('"usp_CreateSubsMonth_U01":'+ (select
							@IDNO[IDNO],@LogID[LogID],@UP_MonProjID[MonProjID],@UP_MonProPeriod[MonProPeriod],
							@UP_ShortDays[ShortDays], @PayTypeId[PayTypeId], @InvoTypeId[InvoTypeId],
							@BefNow[BefNow],@SubsGroup[SubsGroup],1[SetPayOne],1[SetSubsNxt],
							@MerchantTradeNo[MerchantTradeNo], @TaishinTradeNo[TaishinTradeNo],@PRGID[PRGID],
							@xError[Er],@MonthlyRentId[MonthlyRentId], @ErrorCode[ErCode], @ErrorMsg[ErMsg], @SQLExceptionCode[ExCode], @SQLExceptionMsg[ExMsg] 				   							
							for json path,WITHOUT_ARRAY_WRAPPER))	
						set @xError = 1
						set @ErrorCode = 'ERR252'
						RAISERROR('usp_CreateSubsMonth_U01失敗',16,1)
				  end
			   end try
			   begin catch
			       set @SpNote += ('"usp_CreateSubsMonth_U01":'+ (select
						@IDNO[IDNO],@LogID[LogID],@UP_MonProjID[MonProjID],@UP_MonProPeriod[MonProPeriod],
						@UP_ShortDays[ShortDays], @PayTypeId[PayTypeId], @InvoTypeId[InvoTypeId],
						@BefNow[BefNow],@SubsGroup[SubsGroup],1[SetPayOne],1[SetSubsNxt],@PRGID[PRGID],
		                @xError[Er],@MonthlyRentId[MonthlyRentId], @ErrorCode[ErCode], @ErrorMsg[ErMsg], @SQLExceptionCode[ExCode], @SQLExceptionMsg[ExMsg] 				   							
						for json path,WITHOUT_ARRAY_WRAPPER))	
					set @xError = 1
					set @ErrorCode = 'ERR252'
					RAISERROR('usp_CreateSubsMonth_U01失敗',16,1)
			   end catch		   

			   --新月租時數修正
			   declare @use_carWhour float =0
			   declare @use_carHhour float =0
			   declare @use_motoTotalMins float =0

			   if @xError =0
			   begin
				   select 
				   @use_carWhour = s.CarWDHours - t.WorkDayHours,
				   @use_carHhour = s.CarHDHours - t.HolidayHours,
				   @use_motoTotalMins = s.MotoTotalMins - t.MotoTotalHours
				   from #Tmp_MonRent t 
				   join TB_MonthlyRentSet s with(nolock)
				   on s.MonProjID = t.ProjID and s.MonProPeriod = t.MonProPeriod and s.ShortDays = t.ShortDays 
				   where t.IsNowPeriod = 1	      		   

				   --扣除舊合約使用時數  
				   update m
				   set m.WorkDayHours -= case when  @NOW between m.StartDate and m.EndDate then  @use_carWhour else m.WorkDayHours end,
				   m.HolidayHours -= case when @NOW between m.StartDate and m.EndDate then  @use_carHhour else m.HolidayHours end,
				   m.MotoTotalHours -= case when @NOW between m.StartDate and m.EndDate then @use_motoTotalMins else m.MotoTotalHours end,
				   m.U_PRGID=@PRGID,m.U_USERID=@IDNO,m.UPDTime=@NowTime
				   from SYN_MonthlyRent m with(nolock)
				   where m.ProjID = @UP_MonProjID and m.MonProPeriod = @UP_MonProPeriod and m.ShortDays = @UP_ShortDays
				   and m.IDNO = @IDNO and m.useFlag = 1 and m.MonType = 2 
				   and @NOW > m.StartDate

				   --20211005 ADD BY ADAM REASON.把升轉後之前的資料移除，並且更新TB_MonthlyPay的目前ID
				   --===============START=========================
				   --因為資料是先產生，但是開立付費資訊會抓第一筆，所以不管怎麼樣升轉後的TB_MonthlyPay會先指定到第一期
				   --目前能採取的作法，就只能把升轉後的資料移除，並且更新更新TB_MonthlyPay的目前ID

				   --找升轉當下的MonthlyRentId
				   declare @NowUpMonRentId BIGINT
				   SELECT @NowUpMonRentId=MonthlyRentId FROM SYN_MonthlyRent WITH(NOLOCK) 
				   WHERE ProjID=@UP_MonProjID
				   and MonProPeriod=@UP_MonProPeriod
				   and ShortDays= @UP_ShortDays
				   and IDNO= @IDNO and MonType = 2
				   AND @NOW BETWEEN StartDate AND EndDate

				   --更正TB_MonthlyPay為當下的MonthlyRentId
				   --20211026 ADD BY ADAM REASON.修正問題
				   UPDATE TB_MonthlyPay SET MonthlyRentId=@NowUpMonRentId,PayDate=@NOW,
				   U_PRGID=@PRGID,U_USERID=@IDNO,UPDTime=@NowTime
				   WHERE MonthlyRentId IN (SELECT TOP 1 MonthlyRentId FROM SYN_MonthlyRent WITH(NOLOCK) 
				   WHERE ProjID=@UP_MonProjID
				   and MonProPeriod=@UP_MonProPeriod
				   and ShortDays= @UP_ShortDays
				   and IDNO= @IDNO and MonType = 2)

				   delete from SYN_MonthlyRent 
				   where ProjID=@UP_MonProjID
				   and MonProPeriod=@UP_MonProPeriod
				   and ShortDays= @UP_ShortDays
				   and IDNO= @IDNO and MonType = 2
				   and StartDate < @now and EndDate < @now
				   
				   --===============END=========================

				   --更新自動續約主表
				   declare @nxt_MonSetID bigint = 0				   
				   select top 1 @nxt_MonSetID = s.MonSetID from TB_MonthlyRentSet s with(nolock)
				   where s.MonProjID = @MonProjID and s.MonProPeriod = @MonProPeriod and s.ShortDays = @ShortDays				   	

				   --目前自動續約主表ID
				   declare @NowSubsNxtID bigint = 0				   
				   select top 1 @NowSubsNxtID = n.SubsNxtID 
				   from TB_SubsNxt n with(nolock)
				   join TB_MonthlyRentSet s  with(nolock) on  s.MonSetID = n.NowMonSetID 
				   where n.IDNO = @IDNO and s.MonProjID = @MonProjID and s.MonProPeriod = @MonProPeriod and s.ShortDays = @ShortDays
				   and SubsNxtTime is null --20220122 ADD BY AMBER REASON.不撈取已展期資料

				    declare @NowMaxEndDate datetime
					declare @NxtMaxEndDate datetime
				    select @NowMaxEndDate = max(s.EndDate) from SYN_MonthlyRent s with(nolock)
					where s.SubsId = @match_SubsId

				   --刪除續約檔(升轉視同新專案,需重新設定續約或變更方案)
				   delete from TB_SubsNxt 
				   where IDNO = @IDNO and IsMotor = (
				   select top 1 s.IsMoto from TB_MonthlyRentSet s with(nolock)
				   where s.MonProjID = @MonProjID and s.MonProPeriod = @MonProPeriod and s.ShortDays = @ShortDays
				   and SubsNxtTime is null )--20220122 ADD BY AMBER REASON.保留已展期資料
			   end
		  end 

		END
		COMMIT TRAN tr_UpSubsMonth_U1
	END TRY
	BEGIN CATCH	   

	   if @@TRANCOUNT >0	 
		  ROLLBACK TRAN tr_UpSubsMonth_U1

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



