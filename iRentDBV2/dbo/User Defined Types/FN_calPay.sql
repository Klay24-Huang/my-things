-- =============================================
-- Author:      eason
-- Create Date: 2020-11-09
-- Description: 計算租金
-- =============================================
CREATE FUNCTION [dbo].[FN_calPay]
(
    @mins int, --分
	@hours float, --時
	@Price int, --平日價
	@PriceH int, --假日價
	@DateStr varchar(20) --日期
)
RETURNS int
AS
BEGIN
    DECLARE @re int = 0

	if @mins <= 0 and @hours = 0
	  return 0
    else if @mins <= 15
	  set @mins = 0
    else if @mins > 15 and @mins <= 45
	begin
	  set @hours += 0.5
	  set @mins = 0
	end
	else if @mins > 45
	begin
	  set @hours += 1
	  set @mins = 0
	end

	declare @tmpPay float = 0
	declare @totalPay float = 0
	declare @HolidayIndex int = 0

	select top 1 @HolidayIndex = h.HolidayID from TB_Holiday h
    where h.HolidayDate = @DateStr 
	and h.use_flag = 1

	if @HolidayIndex > 0
	begin
	  set @tmpPay = @hours * (@PriceH/10)
	  if @tmpPay > @PriceH
	    set @totalPay = @PriceH
      else
	    set @totalPay = @tmpPay
	end
	else
	begin
	  set @tmpPay = @hours * (@Price/10)
	  if @tmpPay > @Price
	    set @totalPay = @Price
      else
	    set @totalPay = @tmpPay	   
	end

	set @re = convert(int, @totalPay)

    RETURN @re
END

/*
   declare @mins int --分
   declare	@hours float --時
   declare	@Price int --平日價
   declare	@PriceH int --假日價
   declare	@DateStr varchar(20) --日期
*/
GO