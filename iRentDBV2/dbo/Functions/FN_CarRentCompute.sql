-- =============================================
-- Author:      eason
-- Create Date: 2020-11-25
-- Description: 汽車租金試算
-- =============================================
CREATE FUNCTION [dbo].[FN_CarRentCompute]
(
    @sd datetime, --起
	@ed datetime, --迄
	@price int, --平日價
	@priceH int, --假日價
	@dayMaxHour int, --單日時數上限
    @overTime int = 0 --是否為汽車逾時 1(逾時),0(未逾時)
)
RETURNS int
AS
BEGIN
    declare @re int = 0
	declare @minsPro varchar(20) = 'car'
	declare @dayPro varchar(20) = iif(@overTime = 1, 'carOverTime', '')
	declare @baseMinutes int = 60

	declare @wMins int = 0
	declare @hMins int = 0

	declare @wPriceHour float = 0
	declare @hPriceHour float = 0

	if @overTime = 1 --逾時超過6小時以10小時價格計算
	begin 
	   set @wPriceHour = @price / 10
	   set @hPriceHour = @priceH / 10
	end
	else
	begin
	   set @wPriceHour = @price / @dayMaxHour
	   set @hPriceHour = @priceH / @dayMaxHour
	end

	select top 1 
	@wMins = g.w_mins, 
	@hMins = g.h_mins 
	from dbo.FN_GetRangeMins(@sd, @ed, @baseMinutes, @dayMaxHour*60, @minsPro, @dayPro) g

	set @re +=  round(((convert(float,@wMins)/60) * @wPriceHour),1,0)
	set @re +=  round(((convert(float,@hMins)/60) * @hPriceHour),1,0)

    RETURN @re
END
GO
