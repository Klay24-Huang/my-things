-- =============================================
-- Author:      eason
-- Create Date: 2020-11-10
-- Description: 機車租金試算-不分平假日
-- =============================================
CREATE FUNCTION [dbo].[FN_MotoRentTrial]
(
    @SD datetime, --預計取車時間(必填)
	@ED datetime, --預計還車時間(必填)
	@ProjID varchar(10), --(必填)
	@CarType varchar(10) --(必填)
)
RETURNS float
AS
BEGIN
   declare @re float =0
   declare @TotalPrice float = 0

   set @ProjID = isnull(@ProjID,'')
   set @CarType = isnull(@CarType,'')
   if @SD is null or @ED is null or  @ProjID = '' or @CarType = ''
      return -1

   if @ED < @SD
      return -1

   declare @TotalMinutes int = DATEDIFF(minute, @SD, @ED) --總分鐘數

   declare @BaseMinutes int = 0
   declare @BasePrice float = 0
   declare @Price float = 0 --平日價
   declare @PriceH float = 0 --假日價
   declare @MaxPrice int = 0 --平日上限
   declare @MaxPriceH int = 0 --假日上限

   SELECT TOP 1 
	@BaseMinutes = isnull(p.BaseMinutes,0),
    @BasePrice = isnull(p.BaseMinutesPrice,0) * isnull(p.BaseMinutes,0), --低消
	@Price = isnull(p.Price, 0),
	@PriceH = isnull(p.MaxPriceH, 0),
	@MaxPrice = isnull(p.MaxPrice, 0), --平日上限
	@MaxPriceH = isnull(p.MaxPriceH, 0) --假日上限
   FROM TB_PriceByMinutes p
   WHERE p.ProjID = @ProjID and p.CarType = @CarType

   if @Price = 0 or @MaxPrice = 0
       return -1 

   if @TotalMinutes > 0 and @TotalMinutes <=  @BaseMinutes
       set @TotalPrice = @BasePrice
   else if @TotalMinutes >0
   begin
       
	   --fix logic
       --declare @days int = DATEDIFF(day,@SD,@ED)
       --declare @hours int = DATEDIFF(hour,@SD,@ED) % 24
       --declare @mins int = @TotalMinutes % 60

	   --原cs邏輯保留
       declare @days int = 0
       declare @hours int = 0
       declare @mins int = 0
	   set @days = convert(int, FLOOR(@TotalMinutes/600)) 
	   set @hours = convert(int, FLOOR((@TotalMinutes % 600)/60))
	   set @mins = @TotalMinutes - (@days * 600) - (@hours*60)

	   declare @final_hour_cost float = 0 
	   declare @hour_cost float = (@Price*60*@hours)

	   if @hour_cost < @MaxPrice
	   begin
	      set @final_hour_cost = @hour_cost
       end
	   else
          set @final_hour_cost = @MaxPrice

	   set @TotalPrice = (@MaxPrice * @days) + @final_hour_cost + (@mins * @Price) + @BaseMinutes 
   end
   else
       set @TotalPrice = 0 --時數是0

   set @re = @TotalPrice

   RETURN @re
END

/*
    declare @SD datetime --預計取車時間
	declare @ED datetime --預計還車時間
	declare @ProjID varchar(10) 
	declare @CarType varchar(10)
*/
GO