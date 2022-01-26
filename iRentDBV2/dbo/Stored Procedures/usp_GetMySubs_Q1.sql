/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetMySubs_Q1
* 系    統 : IRENT
* 程式功能 : 我的訂閱制方案
* 作    者 : eason
* 撰寫日期 : 20210406
* 修改日期 : 20211124 ADD BY ADAM REASON.排除繳欠費後問題
             20220122 ADD BY AMBER REASON.排除續約已展期資料
* Example  : 
exec usp_GetMySubs_Q1 '','A122364317',99999,2,NULL
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_GetMySubs_Q1]
(   
	@MSG			VARCHAR(10) OUTPUT,
	@IDNO			VARCHAR(20)       , --身分證號
    @LogID			BIGINT,
	@MonType        INT =0, --月租分類0無1月租2訂閱制3短租
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
	DECLARE @FunName VARCHAR(50) = 'usp_GetMySubs_Q1'
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
		   --月租列表
		   select distinct
		   s.MonProjID, s.MonProPeriod, s.ShortDays, s.MonProjNM,
		   --m.WorkDayHours, m.HolidayHours, m.MotoTotalHours,
		   WorkDayHours = case when s.CarWDHours > 0 then m.WorkDayHours else -999 end,
		   HolidayHours = case when s.CarHDHours > 0 then m.HolidayHours else -999 end,
		   MotoTotalHours = case when s.MotoTotalMins >0 then m.MotoTotalHours else -999 end,
		   m.WorkDayRateForCar, m.HoildayRateForCar,
		   m.WorkDayRateForMoto, m.HoildayRateForMoto,
		   m.StartDate, m.EndDate,
		   MonthStartDate = (select min(m1.StartDate) from SYN_MonthlyRent m1 with(nolock) where m1.SubsId = m.SubsId and m1.useFlag = 1),
		   MonthEndDate = (select max(m2.EndDate) from SYN_MonthlyRent m2 with(nolock) where m2.SubsId = m.SubsId and m2.useFlag = 1),
		   NxtMonProPeriod = isnull((select top 1 s1.MonProPeriod from TB_MonthlyRentSet s1 with(nolock) where s1.MonSetID = sn.NxtMonSetID),0),
		   IsMix = case when ((s.CarWDHours > 0 or s.CarHDHours > 0) and (s.MotoTotalMins > 0 or s.HDRateForMoto < 2)) then 1 else 0 end,
		   IsUpd = case when exists(select count(sm0.SubsGroup) 
		     from TB_SubsMain sm0 with(nolock) where sm0.SubsGroup = (
			   select top 1 sm1.SubsGroup from TB_SubsMain sm1 with(nolock) where sm1.SubsId = m.SubsId
			 )
			 having count(sm0.SubsGroup) > 1) then 1 else 0 end,
		   SubsNxt = iif(sn.SubsNxtID is null,0,1), --是否自動續訂
		   IsChange = iif(( sn.SubsNxtID is null or sn.NxtMonSetID = s.MonSetID),0,1), --受否變更下期合約
		   IsPay = isnull(p.ActualPay,0),
			NxtPay = case when exists (
			select * from SYN_MonthlyRent m5 with(nolock)
			where m5.SubsId = m.SubsId and m5.StartDate > m.StartDate) then (
			select top 1 isnull(p3.ActualPay,0) from SYN_MonthlyRent m3 with(nolock)
			left join TB_MonthlyPay p3 on p3.MonthlyRentId = m3.MonthlyRentId
			where m3.SubsId = m.SubsId   
			and m3.StartDate > m.StartDate order by m3.StartDate
			) else 0 end,
		   s.IsMoto	--20210527 ADD BY ADAM REASON.補欄位
		   from SYN_MonthlyRent m with(nolock)
 		   join TB_MonthlyRentSet s with(nolock) 
		   on s.MonProjID = m.ProjID and s.MonProPeriod = m.MonProPeriod and s.ShortDays = m.ShortDays  
		   left join TB_MonthlyPay p with(nolock) on p.MonthlyRentId = m.MonthlyRentId and p.IDNO = @IDNO
		   AND p.ActualPay >=0 --20211124 ADD BY ADAM REASON.排除繳欠費後問題
		   left join TB_SubsNxt sn with(nolock) on sn.IDNO = m.IDNO and sn.IsMotor = s.IsMoto and sn.SubsNxtTime is null --20220122 ADD BY AMBER REASON.排除續約已展期資料
		   where  s.MonSetID is not null and 
		   m.useFlag = 1  and  m.IDNO = @IDNO 
		   and @NOW between m.StartDate and m.EndDate 
		   and m.MonType = case when @MonType = 0 then m.MonType else @MonType end

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



