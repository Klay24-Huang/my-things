/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetSubsCNT_Q1
* 系    統 : IRENT
* 程式功能 : 取得訂閱月租合約明細
* 作    者 : eason
* 撰寫日期 : 2021-04-16
* 修改日期 : 2022-01-22 ADD BY AMBER REASON.排除續約已展期資料
--20211124 ADD BY ADAM REASON.排除繳欠費後問題
* Example  : 
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetSubsCNT_Q1]
(   
	@MSG			VARCHAR(10) OUTPUT,
	@IDNO			VARCHAR(20)       , --身分證號
    @LogID			BIGINT,
	@MonProjID      VARCHAR(10)       ,
	@MonProPeriod   INT               ,
	@ShortDays      INT               ,
	@SetNow         DATETIME = null   
)
AS
BEGIN
    SET NOCOUNT ON

	drop table if exists #Tmp_MonNow

	DECLARE @Error INT = 0
    DECLARE	@ErrorCode VARCHAR(6) = '0000'	
    DECLARE	@ErrorMsg  		   NVARCHAR(100) = 'SUCCESS'	
    DECLARE	@SQLExceptionCode  VARCHAR(10) = ''		
    DECLARE	@SQLExceptionMsg   NVARCHAR(1000) = ''	
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_GetSubsCNT_Q1'
	DECLARE @NOW DATETIME = iif(@SetNow is null, DATEADD(HOUR, 8, GETDATE()),@SetNow)
	DECLARE @str_now varchar(10) = convert(varchar, @Now, 112) 

	BEGIN TRY
	    set @LogID = isnull(@LogID,'')
	    set @IDNO = isnull(@IDNO,'')

		IF @LogID = ''
		BEGIN
			SET @Error=1
			SET @ErrorCode = 'ERR254'
		END

		IF @IDNO = ''
		BEGIN
			SET @Error=1
			SET @ErrorCode = 'ERR256'
		END

		IF @Error = 0
		BEGIN
		    declare @IsMoto int = -1
			declare @MonSetID int = 0
			select top 1 @IsMoto = s.IsMoto, @MonSetId = s.MonSetID from TB_MonthlyRentSet s with(nolock)
			where s.MonProjID = @MonProjID and s.MonProPeriod = @MonProPeriod and s.ShortDays = @ShortDays

			if @IsMoto = -1
			begin
			   set @Error = 1
			   set @ErrorMsg = 'IsMoto錯誤'
			   set @ErrorCode = 'ERR257'--參數遺漏
			end

			if @Error = 0
			begin
		   		--目前合約
			   select distinct top 1
			   m.SubsId,
			   s.MonProjID, s.MonProPeriod, s.ShortDays, s.MonProjNM,
			   --s.CarWDHours, s.CarHDHours, s.MotoTotalMins,
			   CarWDHours = case when s.CarWDHours > 0 then s.CarWDHours else -999 end,
			   CarHDHours = case when s.CarHDHours > 0 then s.CarHDHours else -999 end,
			   MotoTotalMins = case when s.MotoTotalMins > 0 then s.MotoTotalMins else -999 end,
			   s.WDRateForCar, s.HDRateForCar, s.WDRateForMoto, s.HDRateForMoto,
			   m.StartDate, m.EndDate,
			   s.MonProDisc,
		       IsChange = iif(( sn.SubsNxtID is null or sn.NxtMonSetID = s.MonSetID),0,1), --受否變更下期合約
			   IsPay = isnull(p.ActualPay,0)
			   ,IsMix = case when ((s.CarWDHours > 0 or s.CarHDHours > 0) and (s.MotoTotalMins > 0 or s.HDRateForMoto < 2)) then 1 else 0 end,	--20210525 ADD BY ADAM REASON.增加城市車手
			   --20210526 ADD BY ADAM REASON.補欄位
				MonthStartDate = (select min(m1.StartDate) from SYN_MonthlyRent m1 with(nolock) where m1.SubsId = m.SubsId and m1.useFlag = 1),
				MonthEndDate = (select max(m2.EndDate) from SYN_MonthlyRent m2 with(nolock) where m2.SubsId = m.SubsId and m2.useFlag = 1),
				NxtMonProPeriod = isnull((select top 1 s1.MonProPeriod from TB_MonthlyRentSet s1 with(nolock) where s1.MonSetID = sn.NxtMonSetID),0),
				IsUpd = case when exists(select count(sm0.SubsGroup) 
				 from TB_SubsMain sm0 with(nolock) where sm0.SubsGroup = (
				   select top 1 sm1.SubsGroup from TB_SubsMain sm1 with(nolock) where sm1.SubsId = m.SubsId
				 )
				 having count(sm0.SubsGroup) > 1) then 1 else 0 end,
				 SubsNxt = iif(sn.SubsNxtID is null,0,1),	 --是否自動續訂
				 IsMoto = s.IsMoto
			   into #Tmp_MonNow
			   from SYN_MonthlyRent m with(nolock)
			   left join TB_MonthlyPay p with(nolock) on p.MonthlyRentId = m.MonthlyRentId
			   AND p.ActualPay >=0		--20211124 ADD BY ADAM REASON.排除繳欠費後問題
 			   join TB_MonthlyRentSet s with(nolock) 
			   on s.MonProjID = m.ProjID and s.MonProPeriod = m.MonProPeriod and s.ShortDays = m.ShortDays 
		       left join TB_SubsNxt sn with(nolock) on sn.IDNO = m.IDNO and sn.IsMotor = s.IsMoto and sn.SubsNxtTime is null  --20220122 ADD BY AMBER REASON.排除續約已展期資料
			   where 1=1
			   and m.useFlag = 1 and m.IDNO = @IDNO 
			   and @NOW between m.StartDate and m.EndDate 
			   and s.MonProjID = @MonProjID and s.MonProPeriod = @MonProPeriod and s.ShortDays = @ShortDays

			   declare @NowSubsId bigint = 0
			   select @NowSubsId = t.SubsId from #Tmp_MonNow t

			   --目前專案牌卡
			   select 
			   t.MonProjID, t.MonProPeriod, t.ShortDays, t.MonProjNM,
			   t.CarWDHours, t.CarHDHours, MotoTotalMins = case when IsMix=0 then t.MotoTotalMins else 0 end,
			   t.WDRateForCar, t.HDRateForCar, t.WDRateForMoto, t.HDRateForMoto,	  
			   StartDate = (select min(t0.StartDate) 
			     from SYN_MonthlyRent t0 with(nolock)
				 where t0.SubsId = @NowSubsId and t0.useFlag = 1 ),
               EndDate = (select max(t1.EndDate)
			      from SYN_MonthlyRent t1 with(nolock)
				  where t1.SubsId = @NowSubsId and t1.useFlag = 1 ),
			   t.MonProDisc,
			   t.IsPay
			   ,t.IsMix		--20210525 ADD BY ADAM REASON.增加城市車手
			   --20210526 ADD BY ADAM REASON.補欄位
			   ,MonthStartDate = CONVERT(VARCHAR,t.MonthStartDate,111)
			   ,MonthEndDate = CONVERT(VARCHAR,t.MonthEndDate,111)
			   ,t.NxtMonProPeriod
			   ,t.IsUpd,t.SubsNxt,t.IsChange,t.IsMoto
			   from #Tmp_MonNow t

			   declare @IsChange int = 0 --是否變更下期合約
			   select top 1  @IsChange = t.IsChange from #Tmp_MonNow t

			   --下期合約牌卡
			   ;with tmp as(
				  select top 1 * from TB_SubsNxt sn with(nolock)
				  where sn.IsMotor = @IsMoto and sn.IDNO = @IDNO			   
			   )
			   select distinct top 1
			     @IsChange[IsChange],
			     s.MonProjID, s.MonProPeriod, s.ShortDays, s.MonProjNM,
			     --s.CarWDHours, s.CarHDHours, s.MotoTotalMins,
				 CarWDHours = case when s.CarWDHours > 0 then s.CarWDHours else -999 end,
				 CarHDHours = case when s.CarHDHours > 0 then s.CarHDHours else -999 end,
				 MotoTotalMins = case when s.MotoTotalMins > 0 then s.MotoTotalMins else -999 end,
				 s.WDRateForCar, s.HDRateForCar, s.WDRateForMoto, s.HDRateForMoto,
				 SD = t.NowSubsEDate,
				 ED = dateadd(day,s.MonProPeriod * 30, t.NowSubsEDate),
				 s.MonProDisc
				 ,IsMix = case when ((s.CarWDHours > 0 or s.CarHDHours > 0) and (s.MotoTotalMins > 0 or s.HDRateForMoto < 2)) then 1 else 0 end	--20210525 ADD BY ADAM REASON.增加城市車手
				 ,SubsNxt = iif(t.SubsNxtID is null,0,1)
				 ,IsPay=0,IsUpd=0
				 ,MonthStartDate=CONVERT(VARCHAR,t.NowSubsEDate,111)
				 ,MonthEndDate=CONVERT(VARCHAR,dateadd(day,s.MonProPeriod * 30, t.NowSubsEDate),111)
				 ,NxtMonProPeriod = isnull((select top 1 s1.MonProPeriod from TB_MonthlyRentSet s1 WITH(NOLOCK) where s1.MonSetID = t.NxtMonSetID),0)
				 ,IsMoto = s.IsMoto
			   from 
			   TB_MonthlyRentSet s with(nolock)
			   join tmp t on t.NxtMonSetID = s.MonSetID 	
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

	drop table if exists #Tmp_MonNow
END



