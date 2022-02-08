/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_CreateSubsMonth_U01
* 系    統 : IRENT
* 程式功能 : 訂閱制建立
* 作    者 : eason
* 撰寫日期 : 20210323
* 修改日期 : 20220122 ADD BY AMBER REASON.調整取MonthlyRentID邏輯、新增PRGID參數&六兄弟、排除已展期資料
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_CreateSubsMonth_U01]
(   
	@IDNO			VARCHAR(20)       , --身分證號
    @LogID			BIGINT,
	@MonProjID      VARCHAR(10)       ,
	@MonProPeriod   INT               ,
	@ShortDays      INT               ,
	@PayTypeId      bigint = 0        ,--付款方式
	@InvoTypeId     bigint = 0        ,--發票設定
	@SetNow         DATETIME = null   ,
	@SetSubGrop     bigint = 0,
	@SetPayOne         INT = 0, --寫入第1期付款紀錄0:不執行, 1:執行
	@SetSubsNxt        int = 0, --設定自動續約(續約自己)
	@MerchantTradeNo		VARCHAR(30),	--20210709 ADD BY ADAM REASON.補上台新IR編
	@TaishinTradeNo			VARCHAR(50),	--20210709 ADD BY ADAM REASON.補上台新IR編
	@PRGID                  VARCHAR(50),	--20220122 ADD BY AMBER
	@xError                 INT             OUTPUT,
	@MonthlyRentId			BIGINT				OUTPUT, --20210714 ADD BY ADAM 
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
)
AS
BEGIN
    SET NOCOUNT ON

	declare @spIn nvarchar(max), @SpOut nvarchar(max), @SpNote nvarchar(max) = '' --splog
	begin 		
		select @spIn = isnull((
		select @IDNO[IDNO], @LogID[LogID], @MonProjID[MonProjID], @MonProPeriod[MonProPeriod], @ShortDays[ShortDays],
		@PayTypeId[PayTypeId],@InvoTypeId[InvoTypeId],
		@SetNow[SetNow], @SetSubGrop[SetSubGrop], @SetPayOne[SetPayOne], @SetSubsNxt[SetSubsNxt],
		@MerchantTradeNo[MerchantTradeNo], @TaishinTradeNo[TaishinTradeNo],@PRGID[PRGID]
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),'{}')
	end

	drop table if exists #Tmp_CsMonSet
 	SET @xError = 0
    SET	@ErrorCode  = '0000'	
    SET	@ErrorMsg   = 'SUCCESS'	
    SET	@SQLExceptionCode = ''		
    SET	@SQLExceptionMsg = ''	

	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_CreateSubsMonth_U01'
	DECLARE @NOW DATETIME = iif(@SetNow is null, DATEADD(HOUR, 8, GETDATE()),@SetNow)
	DECLARE @IsMotor TINYINT
	--declare @str_now varchar(10) = convert(varchar, @NOW, 112) 
	--20211001 ADD BY ADAM REASON.起日要改為系統日時間
	declare @str_now varchar(30) = convert(varchar, @NOW, 121) 
	DECLARE @NowTime DATETIME = dbo.GET_TWDATE()

	SET @MonProjID = ISNULL(@MonProjID,'')
	SET @IDNO = ISNULL(@IDNO,'')
	SET @PRGID = ISNULL(@PRGID,'')

	BEGIN TRY
		BEGIN TRAN 		    

		IF @LogID IS NULL OR @LogID = ''
		BEGIN
			SET @xError=1
			set @ErrorMsg = 'LogID必填'
			SET @ErrorCode = 'ERR254'
		END

		IF @IDNO = '' OR @MonProjID = '' OR @MonProPeriod = 0  
		BEGIN
			SET @xError=1
			SET @ErrorMsg = '參數遺漏'
			SET @ErrorCode = 'ERR257' 
		END

		declare @Tmp_MonSet_count int = 0
		select * 
		into #Tmp_CsMonSet
		from TB_MonthlyRentSet s with(nolock)
		where s.MonProjID = @MonProjID and s.MonProPeriod = @MonProPeriod and s.ShortDays = @ShortDays
		select @Tmp_MonSet_count = count(*) from #Tmp_CsMonSet

		if @Tmp_MonSet_count = 0
		begin
			SET @xError=1
			SET @ErrorMsg = '要建立的專案不存在'
			SET @ErrorCode = 'ERR261'
		end

		declare @nowCarProjCount int = 0
		declare @nowMotoProjCount int = 0
		IF @xError = 0 
		BEGIN 	
		   select @nowCarProjCount = count(*) from SYN_MonthlyRent m with(nolock)
		   where  m.useFlag = 1 and m.MonType = 2 and  m.IDNO = @IDNO and m.Mode = 0		   
		   and @NOW between m.StartDate and m.EndDate 

		   if @nowCarProjCount > 0 and (select count(*) from #Tmp_CsMonSet t where t.IsMoto=0) > 0
		   begin
				SET @xError=1
				SET @ErrorMsg = '同時段只能訂閱一個汽車訂閱制月租'
				SET @ErrorCode = 'ERR262' 
		   end
		END

		IF @xError = 0 
		BEGIN   	 
		   select @nowMotoProjCount = count(*) from SYN_MonthlyRent m with(nolock)
		   where m.useFlag = 1 and m.MonType = 2  and  m.IDNO = @IDNO and m.Mode = 1	
		   and @NOW between m.StartDate and m.EndDate 

		   if @nowMotoProjCount > 0 and (select count(*) from #Tmp_CsMonSet t where t.IsMoto=1) > 0
		   begin
				SET @xError=1
				SET @ErrorMsg = '同時段只能訂閱一個機車訂閱制月租'
				SET @ErrorCode = 'ERR263' 
		   end
		END

		IF @xError = 0
		BEGIN

			 declare @tb_period_count int = 0
			 declare @tb_period table(
			    MonProjID varchar(10),
				MonProPeriod int,
				ShortDays int,
				MonPeriod int,
				--SDATE varchar(8),
				SDATE varchar(30),	--20211001 ADD BY ADAM REASON.起日要改為系統日時間
				EDATE varchar(8)
			 )

			 declare @periods int = 0
			 select @periods = s.MonProPeriod from TB_MonthlyRentSet s with(nolock)
			 where s.MonProjID = @MonProjID and s.MonProPeriod = @MonProPeriod and s.ShortDays = @ShortDays

			 if @periods > 0
			 begin	     
				 declare @loop_count int = 1
				 
				 --declare @nxt_sd varchar(10) = @str_now
				 --20211001 ADD BY ADAM REASON.起日要改為系統日時間
				 declare @nxt_sd varchar(30) = @str_now
				 declare @nxt_ed varchar(10) = '' 
				 while @periods > 0 and @loop_count <= @periods 
				 begin		   
				   set @nxt_ed = convert(varchar, DATEADD(day,@loop_count*30,@NOW), 112)
				   insert into @tb_period
				   values(@MonProjID, @MonProPeriod, @ShortDays, @loop_count, @nxt_sd, @nxt_ed)	
				   --set @nxt_sd = CONVERT(varchar, DATEADD(day,1,CONVERT(datetime,@nxt_ed)),112)
				   set @nxt_sd = CONVERT(varchar, CONVERT(datetime,@nxt_ed),112)
				   set @loop_count+=1
				 end
				 select @tb_period_count = count(*) from @tb_period
			 end
			 else
			 begin
			    set @xError = 1
				set @ErrorMsg = '期數為0'
				set @ErrorCode = 'ERR264' 
			 end

			 --建立月租
			 if @tb_period_count > 0
			 begin
			     declare @MonRentId bigint = 0
				 declare @Tmp_SubsId int =0
				 declare @subGrop bigint = 0		 		    
				 if @SetSubGrop = 0
				    SELECT @subGrop = NEXT VALUE FOR NM_SubsGroup
                 else
				    SELECT @subGrop = @SetSubGrop

				 insert into TB_SubsMain(MonProjID, MonProPeriod, ShortDays, SubsGroup, AutoSubs, IDNO, RenewPeriod, UpdPeriod,A_PRGID,A_USERID,MKTime,U_PRGID,U_USERID,UPDTime)
				 values(@MonProjID, @MonProPeriod, @ShortDays, @subGrop , 0, @IDNO, 0, 0,@PRGID,@IDNO,@NowTime,@PRGID,@IDNO,@NowTime)
				 SELECT @Tmp_SubsId = @@IDENTITY 

				 
				INSERT INTO SYN_MonthlyRent 
				(SubsId, MonLvl,ProjID,ProjNM,MonProPeriod,ShortDays,SEQNO,Mode,MonType,IDNO,
				CarFreeType,CarTotalHours,WorkDayHours,HolidayHours,
				MotoFreeType,MotoTotalHours,MotoWorkDayMins,MotoHolidayMins,
				WorkDayRateForCar,HoildayRateForCar,
				WorkDayRateForMoto,HoildayRateForMoto,
				StartDate,EndDate,
				A_PRGID,A_USERID,MKTime,U_PRGID,U_USERID,UPDTime
				)
				select distinct
				@Tmp_SubsId, s.MonLvl, s.MonProjID, s.MonProjNM, s.MonProPeriod, s.ShortDays, 0, s.IsMoto, s.MonType, @IDNO,
				0, s.CarTotalHours, s.CarWDHours, s.CarHDHours,
				0, s.MotoTotalMins, s.MotoWDMins, s.MotoHDMins,
				s.WDRateForCar, s.HDRateForCar,
				s.WDRateForMoto, s.HDRateForMoto,
				p.SDATE, p.EDATE,
				@PRGID,@IDNO,@NowTime,@PRGID,@IDNO,@NowTime
				from @tb_period p
				join TB_MonthlyRentSet s on s.MonProjID = p.MonProjID and s.MonProPeriod = p.MonProPeriod and s.ShortDays = p.ShortDays				
			    
				--20210714 ADD BY ADAM REASON.取得流水號
				--SELECT @MonthlyRentId = SCOPE_IDENTITY()
				--20220122 ADD BY AMBER REASON.上面寫法會取得最後期數ID，應取最小期數
				select top 1 @MonthlyRentId= MonthlyRentId from SYN_MonthlyRent with(nolock) where SubsId =@Tmp_SubsId and useFlag=1 order by MonthlyRentId

				 if @SetPayOne = 1
				 begin		     
					  exec dbo.usp_PaySubs_U01 
					  @IDNO, @LogID, 
					  @MonProjID, @MonProPeriod, @ShortDays,
					  @PayTypeId, @InvoTypeId, @NowTime, 
					  @MerchantTradeNo, @TaishinTradeNo,@PRGID,		--20210709 ADD BY ADAM REASON.補上台新IR編
					  @xError output, @ErrorCode output, @ErrorMsg output, @SQLExceptionCode output, @SQLExceptionMsg output

					 if @xError = 1
					 begin
						  set @SpNote += 
						      '"usp_PaySubs_U01":'+ (select
							  @IDNO[IDNO], @LogID[LogID],  @MonProjID[ProjID], @MonProPeriod[Period], 
							  @ShortDays[ShortDay], 
							  @PayTypeId[PayTypeId], @InvoTypeId[InvoTypeId], @Now[Now],@NowTime[NowTime],	
							  @MerchantTradeNo[MerchantTradeNo], @TaishinTradeNo[TaishinTradeNo],@PRGID[PRGID],	--20210709 ADD BY ADAM REASON.補上台新IR編
							  @xError[Er], @ErrorCode[ErCode], @ErrorMsg[ErMSg], 
							  @SQLExceptionCode[ExCode], @SQLExceptionMsg[ExMsg]
							  for json path, WITHOUT_ARRAY_WRAPPER)						 						 					     
						  set @ErrorMsg = 'usp_PaySubs_U01執行失敗'
						  set @ErrorCode = 'ERR252'
						  RAISERROR('usp_PaySubs_U01失敗',16,1)
					 end
			     end

				 --自動續訂
				 if @xError = 0 and @SetSubsNxt = 1
				 begin			     
				    exec usp_SetSubsNxt_U01
					@IDNO, @LogID,
					@MonProjID, @MonProPeriod, @ShortDays, @SetSubsNxt,@NowTime,@PRGID,
					@xError output, @ErrorCode output, @ErrorMsg output, @SQLExceptionCode output, @SQLExceptionMsg output
					
				    if @xError = 1
					begin
					   set @SpNote += 
						      '"usp_SetSubsNxt_U01":'+ (select
							  @IDNO[IDNO], @LogID[LogID],  
							  @MonProjID[NxtMonProjID], @MonProPeriod[NxtMonProPeriod], 
							  @ShortDays[NxtShortDays], @SetSubsNxt[AutoSubs], @Now[Now],@PRGID[PRGID],						  
							  @xError[Er], @ErrorCode[ErCode], @ErrorMsg[ErMSg], 
							  @SQLExceptionCode[ExCode], @SQLExceptionMsg[ExMsg]
							  for json path, WITHOUT_ARRAY_WRAPPER)						 						 					     
						  set @ErrorMsg = 'usp_SetSubsNxt_U01執行失敗'
						  set @ErrorCode = 'ERR252'
						  RAISERROR('usp_SetSubsNxt_U01失敗',16,1)	
					end			 				 				 
				 end

				 --20210709 ADD BY ADAM REASON.設定不自動時要刪除原本設定資料
				 if @xError = 0 and @SetSubsNxt = 0
				 begin
					SELECT @IsMotor = IsMoto FROM TB_MonthlyRentSet WITH(NOLOCK) WHERE MonProjID=@MonProjID AND MonProPeriod=@MonProPeriod AND ShortDays=@ShortDays
					--自動續訂只看會員跟是否為機車
					DELETE FROM TB_SubsNxt WHERE IDNO=@IDNO AND IsMotor=@IsMotor  AND SubsNxtTime IS NULL --20220122 ADD BY AMBER REASON.排除已展期資料
				 end
			 end			
		END
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
		exec dbo.usp_SpSubsLog @FunName, @spIn, null,@xError, @ErrorCode, @ErrorMsg, @SQLExceptionCode, @SQLExceptionMsg, @SpNote
	
	drop table if exists #Tmp_CsMonSet
END



