/********************************************************************
功能：報表/月租報表
新增人員：胡湘梅(Umeko)
新增期間：2021/07/12
修改歷程：

***********************************************************************/
CREATE PROCEDURE [dbo].[usp_GetMonthlyDetail]
	@IDNO varchar(20) = null,
	@SD datetime = null,
	@ED datetime = null,
	@OrderNo BIGINT = null,
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS

	SET	@ErrorCode  = '0000'	
	SET	@ErrorMsg   = 'SUCCESS'	
	SET	@SQLExceptionCode = ''		
	SET	@SQLExceptionMsg = ''		
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_GetMonthlyDetail'

	DECLARE @Error INT;
	/*初始設定*/
	SET @Error=0;

	if (@SD is Not null And @ED is null) OR (@SD is null And @ED is not null)
	Begin
		Set @Error= 1
		Set @ErrorCode = 'ERR101'
		Set @ErrorMsg = 'Missing Parameter '
	End

	BEGIN TRY

	IF @Error = 0
	Begin
		if @SD is null And @ED is null
		Begin
			Select OrderNo,IDNO,lend_place,UseWorkDayHours,UseHolidayHours,UseMotoTotalHours,MKTime,SEQNO,ProjID,
			ProjNM,
			Case When ProjType in (0,3) Then Dbo.FN_MonthlyRentRateForCarHours('Work',ProjType,w_mins,h_mins,monthly_workday,monthly_holiday,gift_point) Else '-' End WorkDayRateForCarHours,
			Case When ProjType in (0,3) Then Dbo.FN_MonthlyRentRateForCarHours('holiday',ProjType,w_mins,h_mins,monthly_workday,monthly_holiday,gift_point) Else '-' End HolidayRateForCarHours,
			Case When ProjType = 4 then (t_mins)-(monthly_workday+monthly_holiday+gift_point) Else '-' End RateForMotorHours
			From (
				SELECT History.OrderNo, History.IDNO, Main.lend_place, History.UseWorkDayHours, History.UseHolidayHours, 
					History.UseMotoTotalHours, History.MKTime, ISNULL(Rate.SEQNO, 0) AS SEQNO, ISNULL(Rate.ProjID, '') AS ProjID, 
					ISNULL(Rate.ProjNM, '') AS ProjNM,Main.ProjType,
					Detail.final_start_time, Detail.final_stop_time,
					Detail.monthly_workday ,
					Detail.monthly_holiday ,
					Detail.gift_point ,
					Detail.gift_motor_point ,
					Case When Main.ProjType in (0,3) Then (Select w_mins From [dbo].[FN_GetRangeMins](Detail.final_start_time, Detail.final_stop_time,0,600,'car','')) Else 0 End w_mins,
					Case When Main.ProjType in (0,3) Then (Select h_mins From [dbo].[FN_GetRangeMins](Detail.final_start_time, Detail.final_stop_time,0,600,'car','')) Else 0 End h_mins,
					Case When Main.ProjType = 4  Then (Select w_mins+h_mins From [dbo].[FN_GetRangeMins](Detail.final_start_time, Detail.final_stop_time,0,600,'car','')) Else 0 End t_mins
				FROM dbo.TB_MonthlyRentHistory AS History WITH (NOLOCK) LEFT OUTER JOIN
						dbo.TB_MonthlyRent AS Rate WITH (NOLOCK) ON History.MonthlyRentId = Rate.MonthlyRentId INNER JOIN
						dbo.TB_OrderMain AS Main WITH (NOLOCK) ON Main.order_number = History.OrderNo  INNER JOIN
						dbo.TB_OrderDetail AS Detail WITH (NOLOCK) ON Main.order_number = Detail.order_number
				WHERE History.IDNO = Case When @IDNO is null Then History.IDNO Else @IDNO End 
					And OrderNo = Case When @OrderNo is null Then OrderNo Else @OrderNo End 
			) tA
			Order by OrderNo
		End
		Else
		Begin
			Select OrderNo,IDNO,lend_place,UseWorkDayHours,UseHolidayHours,UseMotoTotalHours,MKTime,SEQNO,ProjID,
			ProjNM,
			Dbo.FN_MonthlyRentRateForCarHours('Work',ProjType,w_mins,h_mins,monthly_workday,monthly_holiday,gift_point) WorkDayRateForCarHours,
			Dbo.FN_MonthlyRentRateForCarHours('holiday',ProjType,w_mins,h_mins,monthly_workday,monthly_holiday,gift_point) HolidayRateForCarHours,
			Case When ProjType = 4 then (t_mins)-(monthly_workday+monthly_holiday+gift_point) Else 0 End RateForMotorHours
			From (
				SELECT History.OrderNo, History.IDNO, Main.lend_place, History.UseWorkDayHours, History.UseHolidayHours, 
					History.UseMotoTotalHours, History.MKTime, ISNULL(Rate.SEQNO, 0) AS SEQNO, ISNULL(Rate.ProjID, '') AS ProjID, 
					ISNULL(Rate.ProjNM, '') AS ProjNM,Main.ProjType,
					Detail.final_start_time, Detail.final_stop_time,
					Detail.monthly_workday ,
					Detail.monthly_holiday ,
					Detail.gift_point ,
					Detail.gift_motor_point ,
					(Select w_mins From [dbo].[FN_GetRangeMins](Detail.final_start_time, Detail.final_stop_time,0,600,'car','')) w_mins,
					(Select h_mins From [dbo].[FN_GetRangeMins](Detail.final_start_time, Detail.final_stop_time,0,600,'car','')) h_mins,
					(Select h_mins+w_mins From [dbo].[FN_GetRangeMins](Detail.final_start_time, Detail.final_stop_time,0,600,'','')) t_mins
				FROM dbo.TB_MonthlyRentHistory AS History WITH (NOLOCK) LEFT OUTER JOIN
						dbo.TB_MonthlyRent AS Rate WITH (NOLOCK) ON History.MonthlyRentId = Rate.MonthlyRentId INNER JOIN
						dbo.TB_OrderMain AS Main WITH (NOLOCK) ON Main.order_number = History.OrderNo  INNER JOIN
						dbo.TB_OrderDetail AS Detail WITH (NOLOCK) ON Main.order_number = Detail.order_number
				WHERE History.IDNO = Case When @IDNO is null Then History.IDNO Else @IDNO End 
					And OrderNo = Case When @OrderNo is null Then OrderNo Else @OrderNo End 
					And History.MKTime between @SD And @ED
			) tA
			Order by OrderNo ASC
		End
	End 

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
	
	SET @IsSystem=1;
	SET @ErrorType=4;
	INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMonthlyDetail';