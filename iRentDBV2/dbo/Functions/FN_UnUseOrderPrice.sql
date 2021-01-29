-- =============================================
-- Author:     eason
-- Create Date: 2021-01-29
-- Description: 未使用天數訂金
-- =============================================
CREATE FUNCTION [dbo].[FN_UnUseOrderPrice]
(
   @OrderPrice FLOAT, 
   @BookingSD DATETIME, 
   @BookingED DATETIME,
   @FinalSD DATETIME,
   @FinalED DATETIME
)
RETURNS float
AS
BEGIN
   declare @re float = 0
   if @OrderPrice > 0 and @BookingSD <= @BookingED and @FinalSD <= @FinalED and @FinalSD >= @BookingSD
   begin      
	   set @BookingSD = DATEADD(second, DATEPART(SECOND, @BookingSD)*-1,@BookingSD)--去秒數
	   set @BookingED = DATEADD(second, DATEPART(SECOND, @BookingED)*-1,@BookingED)--去秒數
	   set @FinalSD = DATEADD(second, DATEPART(SECOND, @FinalSD)*-1,@FinalSD)--去秒數
	   set @FinalED = DATEADD(second, DATEPART(SECOND, @FinalED)*-1,@FinalED)--去秒數      

       declare @bok_days int = datediff(day,@BookingSD,@BookingED)
       declare @bok_mins int = datediff(minute,@BookingSD,@BookingED)	  
	   declare @bok_xmins int = @bok_mins % (24*60)
	   if @bok_xmins > 0
	      set @bok_days += 1
	   declare @orderDayPice float = floor(@OrderPrice / @bok_days) --每日金額

	   declare @use_days int = 0
	   declare @use_mins int = 0
	   --預計還車時間為檢核點
	   if @FinalED <= @BookingED
	   begin
	      set @use_days = datediff(day,@finalSD, @FinalED) 
		  set @use_mins = datediff(minute,@finalSD, @FinalED) 
	   end
	   else
	   begin
	      set @use_days = datediff(day,@finalSD, @BookingED) 
		  set @use_mins = datediff(minute,@finalSD, @BookingED) 
	   end

	   declare @use_xmins int = @use_mins  % (24*60)
	   if @use_xmins > 0
	     set @use_days += 1

       declare @unUseDays float = @bok_days - @use_days
       set @unUseDays = iif(@unUseDays>0,@unUseDays,0)
	   set @re = @unUseDays * @orderDayPice --未使用天數訂金
	   set @re = iif(@re > 0,@re,0)
   end  
   RETURN @re
END
/*
declare @sd datetime = '2021-02-01 08:00:00'
declare @ed datetime = dateadd(day,3,@sd)
declare @fsd datetime = dateadd(minute,30,@sd)
declare @fed datetime = dateadd(hour,3,@fsd)

select @sd[sd], @ed[ed], @fsd[fsd], @fed[fed]
select dbo.FN_UnUseOrderPrice(3000, @sd, @ed, @fsd, @fed)
*/