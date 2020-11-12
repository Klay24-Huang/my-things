-- =============================================
-- Author:      eason
-- Create Date: 2020-11-11
-- Description: 機車租金試算
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

   if @TotalMinutes = 0 --開始結束相同
      return 0

   declare @BaseMinutes int = 0
   declare @BaseMinutesPrice float = 0
   declare @BaseMinutesPriceH float = 0
   declare @Price float = 0 --平日價
   declare @PriceH float = 0 --假日價
   declare @MaxPrice int = 0 --平日上限
   declare @MaxPriceH int = 0 --假日上限   

   SELECT TOP 1 
	@BaseMinutes = isnull(p.BaseMinutes,0),
    @BaseMinutesPrice = isnull(p.BaseMinutesPrice,0), 
	@BaseMinutesPriceH = isnull(p.BaseMinutesPriceH,0),
	@Price = isnull(p.Price, 0),
	@PriceH = isnull(p.PriceH, 0),
	@MaxPrice = isnull(p.MaxPrice, 0), --平日上限
	@MaxPriceH = isnull(p.MaxPriceH, 0) --假日上限
   FROM TB_PriceByMinutes p
   WHERE p.ProjID = @ProjID and p.CarType = @CarType
   and p.use_flag = 1
   
   if @BaseMinutes = 0 or @BaseMinutesPrice = 0 or @BaseMinutesPriceH = 0 or @Price = 0  or @PriceH = 0 or @MaxPrice = 0 or @MaxPriceH =0
       return -1 

   declare @xdays int = DATEDIFF(day,@SD,@ED)
   declare @strSD varchar(10) = convert(varchar,@SD,112)
   declare @strED varchar(10) = convert(varchar,@ED,112)

   --低於最小分鐘數
   if @TotalMinutes > 0 and @TotalMinutes <=  @BaseMinutes
   begin
		declare @HolidayIndex int = 0
		select top 1 @HolidayIndex = h.HolidayID from TB_Holiday h
		where h.HolidayDate = @strSD 
		and h.use_flag = 1		

		if @HolidayIndex > 0
		  return @BaseMinutes * @BaseMinutesPriceH
        else
		  return @BaseMinutes * @BaseMinutesPrice       
   end

   declare @xMins int = 0
   if @xdays = 0 --當天
   begin       
      set @xMins = DATEDIFF(minute, @SD, @ED)
	  set @re = dbo.FN_MotoRentCompute(@xMins,@Price,@PriceH,@MaxPrice,@MaxPriceH,@strSD)
   end
   else if @xdays = 1 --隔天
   begin
      declare @tmp_ed_star datetime = convert(datetime, (convert(varchar, @ED, 111)+ ' ' + '00:00:00'))
	  declare @day_one_mins int = DATEDIFF(minute,@SD, @tmp_ed_star)
	  declare @day_end_mins int = DATEDIFF(minute,@tmp_ed_star, @ED) 
	  set @re += dbo.FN_MotoRentCompute(@day_one_mins, @Price, @PriceH, @MaxPrice, @MaxPriceH, @strSD) --首日花費
	  set @re += dbo.FN_MotoRentCompute(@day_end_mins, @Price, @PriceH, @MaxPrice, @MaxPriceH, @strED) --尾日花費
   end
   else if @xdays > 1 
   begin

       declare @tm_sd_end datetime = convert(datetime, (convert(varchar, dateadd(day,1, @SD) , 111)+ ' ' + '00:00:00')) --首日結束
  	   declare @tm_ed_star datetime = convert(datetime, (convert(varchar, @ED, 111)+ ' ' + '00:00:00')) --尾日開始

	    declare @day_one_m int = DATEDIFF(minute,@SD, @tm_sd_end) --首日時間
	    set @re += dbo.FN_MotoRentCompute(@day_one_m, @Price, @PriceH, @MaxPrice, @MaxPriceH, @strSD) --首日花費

	    declare @day_end_m int = DATEDIFF(minute,@tm_ed_star,@ED)
	    set @re += dbo.FN_MotoRentCompute(@day_end_m, @Price, @PriceH, @MaxPrice, @MaxPriceH, @strED) --尾日花費 

		declare @day_star_count date = convert(date, @strSD) 
        declare @day_end_count date = convert(date, @strED)
		set @day_star_count = DATEADD(day,1,@day_star_count) --次日開始

		while ( @day_star_count < @day_end_count)
		begin				
			declare @str_date varchar(20) = convert(varchar, @day_star_count, 112) 
			set @re += dbo.FN_MotoRentCompute(24*60, @Price, @PriceH, @MaxPrice, @MaxPriceH, @str_date)
			set @day_star_count  = DATEADD(day,1,@day_star_count)
		end
   end

   set @re = FLOOR(@re)

   RETURN @re
END

/*
    declare @SD datetime 
	declare @ED datetime 
	declare @ProjID varchar(10) 
	declare @CarType varchar(10)

	select dbo.FN_MotoRentTrial(@SD,@ED,@ProjID,@CarType)
*/
GO