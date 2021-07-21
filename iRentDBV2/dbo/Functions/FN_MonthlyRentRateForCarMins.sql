-- =============================================
-- Author:Umeko
-- Create date: 2021/07/21
-- Description:取得月租報表中的汽車優惠費率分鐘數
-- =============================================
CREATE FUNCTION FN_MonthlyRentRateForCarMins
(
	@RetutnType varchar(10),
	@ProjType int,
	@WMins int ,
	@HMins int ,
	@monthly_WMins int,
	@monthly_HMins int,
	@gift_point int

)
RETURNS int
AS
BEGIN
	if @RetutnType Not in ('Work', 'Holiday')
		return 0

	if @WMins = 0 And @HMins = 0
		return 0
	
	Declare @WPayMins int = 0
	Declare @HPayMins int = 0
	Declare @ReturnMins int = 0

	if @ProjType in (0,3)
	Begin
		If @WMins > 0 And @HMins = 0
		Begin
			Set @WPayMins = @WMins - (@monthly_WMins+@gift_point)
		End
		Else If  @WMins = 0 And @HMins > 0
		Begin
			Set @HPayMins = @HMins - (@monthly_HMins+@gift_point)
		End
		Else -- @WMins> 0 ,@HMins > 0
		Begin
			if @WPayMins > (@monthly_WMins+@gift_point)
			Begin
				Set @WPayMins = @WMins - (@monthly_WMins+@gift_point)
				Set @HPayMins = @HMins - @monthly_HMins
			End
			Else
			Begin
				Set @WPayMins = @WMins - @monthly_WMins
				Set @gift_point = @gift_point-@WPayMins
				Set @WPayMins = 0
				Set @HPayMins =  @HMins - (@monthly_HMins+@gift_point)
			End
		End

		if @RetutnType = 'Work'
			Set @ReturnMins= @WPayMins
		Else --@RetutnType = 'Holiday'
			Set @ReturnMins= @HPayMins
	End
	Else
	Begin
		Set @ReturnMins = 0
	End

	return @ReturnMins
END