-- =============================================
-- Author:      eason
-- Create Date: 2020-11-09
-- Description: 租金試算
-- =============================================
CREATE FUNCTION [dbo].[FN_CalSpread]
(
    @SD datetime, --預計取車時間
	@ED datetime, --預計還車時間
	@Price int, --平日費用
	@PriceH int --假日費用
)
RETURNS float
AS
BEGIN

	DECLARE @re float

	declare @totalPay float = 0
	declare @totalHours float = FLOOR( DATEDIFF(hour, @SD, @ED)) 
	declare @totalMinutes int = DATEDIFF(minute, @SD, @ED) % 60 --日期相減剩餘分鐘數

	declare @Day float =  FLOOR(@totalHours / 24)	
	declare @tHours float = CAST(@totalHours as INT) % 24 --因為float無法取餘數,所以@totalHours先轉整數再算

	declare @str_SD nvarchar(20) = convert(varchar, @SD, 112) --SD
	declare @str_ED nvarchar(20) = convert(varchar, @ED, 112) --ED

	declare @DaySD datetime = dateadd(HOUR, @Day*24, @SD)
	declare @str_DaySD varchar(20) = convert(varchar, @DaySD, 112) --DaySD

	if @tHours >= 10
	begin
	   set @Day +=1
	   set @tHours = 0
	   set @totalMinutes = 0
	end

	if @Day >= 1
	begin	
	    declare @counter int 
		set @counter=0
		while ( @counter < @Day)
		begin
		    declare @iSD datetime = dateadd(HOUR, @counter*24, @SD)			
			declare @str_iSD varchar(20) = convert(varchar, @iSD, 112) 
			set @totalPay += dbo.FN_calPay(0, 24, @Price, @PriceH, @str_iSD)
			set @counter  = @counter  + 1
		end

		if @tHours > 0
		   set @totalPay += dbo.FN_calPay(0, @tHours, @Price, @PriceH, @str_DaySD)
        
		if @totalMinutes > 0
		   set @totalPay += dbo.FN_calPay(@totalMinutes,0, @Price, @PriceH, @str_DaySD)
	end
	else if @Day = 0
	begin
	     if @tHours < 10
		 begin
	        declare @diffTotalHours float = FLOOR(DATEDIFF(hour,@SD, @ED)) --總時數
			declare @diffTotalMinus int = FLOOR(DATEDIFF(minute, @SD, @ED)) % 60 --總分鐘數(相減剩餘分鐘數)

			if @diffTotalHours > 1
			begin
			   if convert(date, @SD) = convert(date, @ED)
			      set @totalPay = dbo.FN_calPay(@diffTotalMinus, @diffTotalHours, @Price, @PriceH, @str_SD)
               else
			   begin
			      declare @ttSD datetime = (dateadd(DAY,(1),@SD))			 
				  declare @tmpSD datetime = convert(datetime, (convert(varchar, @ttSD, 111)+ ' ' + '00:00:00')) --yyyy/mm/dd hh:mm:ss
				  declare @tmpED datetime = convert(datetime, (convert(varchar, @ED, 111)+ ' ' + '00:00:00'))

				  declare @totalSDHours float = FLOOR(DATEDIFF(HOUR,@SD, @tmpSD))
				  declare @totalSDMinute int = DATEDIFF(minute, @SD, @tmpSD) % 60 --日期相減剩餘分鐘數
				  declare @totalEDHours float = DATEDIFF(hour, @tmpED, @ED)
				  declare @totalEDMinute int = DATEDIFF(minute, @tmpED, @ED) % 60 --日期相減剩餘分鐘數

				  declare @tmp int = 60 - @totalSDMinute

				  if @totalEDMinute >= @tmp
				  begin
				     set @totalEDMinute -= @tmp
					 set @totalPay += dbo.FN_calPay(0, @totalSDHours + 1, @Price, @PriceH, @str_SD)
					 set @totalPay += dbo.FN_calPay(@totalEDMinute, @totalEDHours, @Price, @PriceH, @str_ED)
				  end
				  else
				  begin
				     set @totalEDMinute = @totalEDMinute + 60 -@tmp
					 set @totalEDHours -= 1
					 set @totalPay += dbo.FN_calPay(0, @totalSDHours+1, @Price, @PriceH, @str_SD)
					 set @totalPay += dbo.FN_calPay(@totalEDMinute, @totalEDHours, @Price, @PriceH, @str_ED)
				  end
			   end
			end
			else if @diffTotalHours = 1 and @diffTotalMinus = 0
			begin
			   set @totalPay += dbo.FN_calPay(0, 1, @Price, @PriceH, @str_SD)
			end
			else if @diffTotalHours = 1 and @diffTotalMinus > 0
			begin
			   set @totalPay += dbo.FN_calPay(0,1,@Price, @PriceH, @str_SD)
			   set @totalPay += dbo.FN_calPay(@diffTotalMinus, 0, @Price, @PriceH, @str_ED)
			end
			else if @diffTotalHours < 1
			begin
			   set @totalPay+= dbo.FN_calPay(0,1,@Price,@PriceH,@str_SD)
			end
	     end
		 else	   
		 begin
		    -- 大於十小時，小於1天
		    set @totalPay = dbo.FN_calPay(@totalMinutes, @tHours, @Price, @PriceH, @str_SD)
         end
	end

	set @re = ISNULL(@totalPay,0)

    RETURN @re
END

/*
	測試用備註
	declare  @SD datetime --預計取車時間
	declare	@ED datetime --預計還車時間
	declare	@Price int --平日費用
	declare	@PriceH int --假日費用	
*/
GO