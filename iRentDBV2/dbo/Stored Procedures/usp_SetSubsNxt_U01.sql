/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_SetSubsNxt_U01
* 系    統 : IRENT
* 程式功能 : 設定自動訂閱
* 作    者 : eason
* 撰寫日期 : 20210408
* 修改日期 : 20220121 UPD BY AMBER REASON:檢核合約迄日2天前不能調整、已展期資料保留不刪除
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_SetSubsNxt_U01]
(   
	@IDNO			   VARCHAR(20)       , --身分證號
    @LogID			   BIGINT,
	@NxtMonProjID      VARCHAR(10)       , --續約專案Id
	@NxtMonProPeriod   INT               , --續約期數
	@NxtShortDays      INT               , --續約短期總天數
	@AutoSubs          INT               , --自動續訂1啟用0停用
	@SetNow            DATETIME = null   ,	
	@PRGID             VARCHAR(50)       , --程式名稱
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
		select @IDNO[IDNO], @LogID[LogID], 
		@NxtMonProjID[NxtMonProjID], @NxtMonProPeriod[NxtMonProPeriod], 
		@NxtShortDays[NxtShortDays],
		@AutoSubs[AutoSubs], @SetNow[SetNow] 
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),'{}')
	end

 	SET @xError = 0
    SET	@ErrorCode  = '0000'	
    SET	@ErrorMsg   = 'SUCCESS'	
    SET	@SQLExceptionCode = ''		
    SET	@SQLExceptionMsg = ''		
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_SetSubsNxt_U01'
	DECLARE @NOW DATETIME = iif(@SetNow is null, DATEADD(HOUR, 8, GETDATE()),@SetNow)
	DECLARE @NowTime DATETIME = dbo.GET_TWDATE()
	SET @PRGID=ISNULL(@PRGID,'')

	IF @LogID IS NULL OR @LogID = ''
	BEGIN
		SET @xError=1
		SET @ErrorCode = 'ERR254'--LogID必填
	END	

	IF @IDNO = '' OR @NxtMonProjID = '' OR @NxtMonProPeriod = 0  
	BEGIN
		SET @xError=1
		SET @ErrorCode = 'ERR257' --參數遺漏
	END

	if @AutoSubs <> 0 and @AutoSubs <> 1
	begin
	   set @xError = 1
	   set @ErrorMsg = 'AutoSubs格式錯誤'
	   set @ErrorCode = 'ERR247' --參數格式不符
	end

	BEGIN TRY 
		BEGIN TRAN   

		   	declare @IsMotor int = -1
	    
			declare @NowMonRentId bigint =0			
			declare @NowSubsID bigint = 0			
			declare @NowMRSetID bigint = 0

			declare @NowProjID varchar(10)
			declare @NowProPeriod int = 0
			declare @NowShortDays int = 0
			declare @NowSubsEDate datetime

			declare @NxtMRSetID bigint = 0

			select top 1 @NxtMRSetID = s.MonSetID from TB_MonthlyRentSet s with(nolock)
			where  s.MonProjID = @NxtMonProjID and s.MonProPeriod = @NxtMonProPeriod and s.ShortDays = @NxtShortDays

			if @NxtMRSetID = 0
			begin
				SET @xError=1
				set @ErrorMsg = 'NxtMRSetID不存在'
				SET @ErrorCode = 'ERR909' --專案不存在
			end

			if @xError = 0
			begin
			    select top 1 @IsMotor = s.IsMoto from TB_MonthlyRentSet s with(nolock) --下期IsMotor需與現在相同
				where s.MonSetID = @NxtMRSetID
				if @ISMotor <> 0 and @ISMotor <> 1
				begin
				   set @xError = 1
				   set @ErrorMsg = 'IsMotor格式錯誤'
				   set @ErrorCode = 'ERR247'
				end
			end

			if @xError = 0
			begin
			    --目前月租
				select top 1 @NowSubsID = mr.SubsId, @NowMonRentId=mr.MonthlyRentId,
				@NowProjID = mr.ProjID, @NowProPeriod = mr.MonProPeriod, @NowShortDays = mr.ShortDays
				from SYN_MonthlyRent mr with(nolock)
				where mr.useFlag = 1  and mr.IDNO = @IDNO and mr.Mode = @IsMotor		
				and mr.MonType = 2 --目前都是訂閱制月租2
				and @NOW between mr.StartDate and mr.EndDate

				if @NowSubsID = 0 or @NowMonRentId =0 or isnull(@NowProjID,'') = '' or @NowProPeriod = 0 
				begin
				   set @xError = 1
				   set @ErrorMsg = '目前月租不存在'
				   set @ErrorCode = 'ERR258' --月租不存在
				end

				--20210721 ADD BY ADAM REASON.只有一期的  不可以自動續約
				if @NowProPeriod = 1
				begin
					set @xError = 1
					set @ErrorMsg = '一期專案不可自動續訂'
					set @ErrorCode = 'ERR909'
				end
			end

			declare @NowSubsMain_count int = 0
			if @xError = 0
			begin			   
			   select @NowSubsMain_count = count(*) from TB_SubsMain s with(nolock)
			   where s.SubsId = @NowSubsID
			   if @NowSubsMain_count = 0
			   begin
			      set @xError = 1
				  set @ErrorMsg = '訂閱主檔不存在'
				  set @ErrorMsg = 'ERR909'
			   end
			end

			

			if @xError = 0
			begin
			   --目前月租結束時間
			   select top 1 @NowSubsEDate = m.EndDate from SYN_MonthlyRent m with(nolock)
			   where m.SubsId = @NowSubsID
			   order by m.EndDate desc

			   --目前MRSetId
			   select top 1 @NowMRSetID = s.MonSetID from TB_MonthlyRentSet s with(nolock)
			   where s.MonProjID = @NowProjID and s.MonProPeriod = @NowProPeriod and s.ShortDays = @NowShortDays

			   if @NowSubsEDate is null or @NowMRSetID = 0
			   begin
			      set @xError = 1
				  set @ErrorMsg = 'NowSubsEDate或NowMRSetID錯誤'
				  set @ErrorCode = 'ERR257'
			   end

			   --20210706 ADD BY ADAM REASON.迄日當天不可以設定自動訂閱
			   IF @xError = 0 AND CONVERT(VARCHAR,DATEADD(minute,-1,@NowSubsEDate),112)=CONVERT(VARCHAR,GETDATE(),112)
			   BEGIN
					SET @xError = 1
					SET @ErrorMsg = '迄日當天不可以設定自動訂閱!'
					SET @ErrorCode = 'ERR276'
			   END

			   --20220121 ADD BY AMBER REASON.合約迄日2天前不能調整
			   IF @xError = 0 AND @NowTime Between DATEADD(D,-3,@NowSubsEDate) and DATEADD(ms,-3,@NowSubsEDate)
			    BEGIN
					SET @xError = 1
					SET @ErrorMsg = '合約迄日兩天前不可以設定自動訂閱!'
					SET @ErrorCode = 'ERR992'
			   END
			end

            if @xError = 0
			begin
			    --訂閱主表
			    update s set
				s.AutoSubs = @AutoSubs,
				s.RenewPeriod = iif(@AutoSubs = 1, @NowProPeriod,0), --目前都是最後一期執行續訂
				s.UPDTime = @NOW,			
				A_USERID=@IDNO,
				A_PRGID=@PRGID
				from TB_SubsMain s
				where s.SubsId = @NowSubsID

				--續約主表
				if @AutoSubs = 1
				begin
				   if exists (select * from TB_SubsNxt with(nolock) where IDNO = @IDNO and IsMotor = @IsMotor and SubsNxtTime is null)
				   begin
				      update n set 
					  n.NowMonthlyRentId = @NowMonRentId,
					  n.NowMonSetID = @NowMRSetID,
					  n.NxtMonSetID = @NxtMRSetID,
					  n.NowSubsEDate = @NowSubsEDate,
					  U_PRGID=@PRGID,
					  U_USERID=@IDNO,
					  UPDTime=@NowTime
					  from TB_SubsNxt n
					  where n.IDNO = @IDNO and n.IsMotor = @IsMotor and SubsNxtTime is null
				   end
				   else
				   begin
					   insert into TB_SubsNxt(IDNO,IsMotor,NowMonthlyRentId ,NowMonSetID,NxtMonSetID,NowSubsEDate,SubsNxtTime,A_PRGID,A_USERID,MKTime)
					   values(@IDNO,@IsMotor,@NowMonRentId, @NowMRSetID,@NxtMRSetID,@NowSubsEDate,null,@PRGID,@IDNO,@NowTime)
				   end
				end
				else
				   --20220121 UPD BY AMBER REASON:已展期資料保留不刪除
				   delete from TB_SubsNxt where IDNO = @IDNO and IsMotor = @IsMotor and SubsNxtTime is null
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



