-- =============================================
-- Author:     eason
-- Create Date: 2021-01-29
-- Description: 沒收訂金
-- =============================================
CREATE FUNCTION [dbo].[FN_UnUseOrderPrice]
(
   @OrderPrice FLOAT, 
   @BookingSD DATETIME, 
   @BookingED DATETIME,
   @finalSD DATETIME,
   @finalED DATETIME
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

       declare @bok_mins int = datediff(minute,@BookingSD,@BookingED)	  
	   declare @bok_days int = floor(cast(@bok_mins as float)/(24*60))
	   declare @bok_xmins int = @bok_mins  % (24*60) 
	   if @bok_xmins > 0
	      set @bok_days += 1
	   declare @orderDayPice float = floor(@OrderPrice / cast(@bok_days as float)) --每日金額

	   declare @ck_days int = 0
	   declare @ck_mins int = 0
	   --@BookingED與@FinalED的絕對值計算比對天數
	   if @FinalED <= @BookingED
	   begin	     
		  set @ck_mins = datediff(minute,@finalED, @BookingED) 
		  set @ck_days = floor(cast(@ck_mins as float)/(24*60))
       end
	   else
	   begin
		   set @ck_mins =  datediff(minute, @BookingED,@finalED)
		   set @ck_days = floor(cast(@ck_mins as float)/(24*60))
	   end
	   declare @ck_xmins int = @ck_mins  % (24*60)    

	   if @ck_mins < (24*60)
	   begin
	      --ED前後24HR內(不包含)免罰
	      set @re = @OrderPrice
	   end
	   else
	   begin		 
		   if @ck_xmins > 0
			  set @ck_days += 1	      		  
		   declare @unUseDays float = @bok_days - @ck_days
		   set @unUseDays = iif(@unUseDays>0,@unUseDays,0)
		   set @re = @unUseDays * @orderDayPice --未使用天數訂金
		   set @re = iif(@re > 0,@re,0)
	   end
   end  
   RETURN @re
END
/*
declare @OrderPrice FLOAT = 3000 
declare @BookingSD datetime = '2021-02-01 08:00:00'
declare @BookingED datetime = dateadd(day,3,@BookingSD)
declare @FinalSD datetime = dateadd(minute,-120,@BookingED)
declare @FinalED datetime = dateadd(hour,3,@FinalSD)
select dbo.FN_UnUseOrderPrice(3000,@BookingSD,@BookingED,@FinalSD,@FinalED)
*/
GO