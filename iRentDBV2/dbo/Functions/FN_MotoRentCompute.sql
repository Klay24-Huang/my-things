-- =============================================
-- Author:      eason
-- Create Date: 2020-11-25
-- Description: 機車租金試算
-- =============================================
CREATE FUNCTION [dbo].[FN_MotoRentCompute]
(
    @sd datetime, --起
	@ed datetime, --迄
	@priceMin float, --每分鐘價格
	@baseMinutes int, --基本分鐘數
	@dayMaxPrice int, --單日計費上限
	@disc int --折扣點數
)
RETURNS int
AS
BEGIN
    declare @re int = 0  

	if @disc < 0 or (@disc > 0 and @disc <= 6)
       set @disc = 0 --6分鐘以下不可折扣

    declare @dayMaxMins float =  convert(float,@dayMaxPrice)/ @priceMin  

	set @sd = DATEADD(second, DATEPART(SECOND, @sd)*-1,@sd)--去秒數
	set @ed = DATEADD(second, DATEPART(SECOND, @ed)*-1,@ed)--去秒數

	declare @mins int = datediff(minute, @sd, @ed)
	declare @fpay float = 0

	if @mins < 24*60
	begin
	   if @mins <= 6
	     set @fpay = 10
       else if @mins > convert(int,@dayMaxMins)
	     set @fpay = @dayMaxPrice
	   else
	      set @fpay = (@mins-@baseMinutes) * @priceMin + 10

       if @disc >= 199
	     set @re = 0
	end
	else
	begin
	    declare @wMins int = 0
		declare @hMins int = 0

		select top 1 
		@wMins = g.w_mins, 
		@hMins = g.h_mins 
		from dbo.FN_GetRangeMins(@sd, @ed, @baseMinutes, @dayMaxMins,'','') g

		set @fpay = convert(float,(@wMins + @hMins)) * @priceMin
	end

	if @disc > 0
	begin
	   declare @disc_f float = convert(float, @disc)

	   if @disc < 199
	      set @fpay = @fpay-10 - (@disc_f-6)*@priceMin 
       else
	      set @fpay = (@fpay-300) - (@disc_f-199) * @priceMin
	end

	set @fpay = iif(@fpay >=0, @fpay, 0)

	set @re = round(@fpay,0)--四捨五入

    RETURN @re
END

/*
   declare @sd datetime
   declare	@ed datetime
   declare	@priceMin float
   declare	@baseMinutes int
   declare  @dayMaxPrice int
   declare	@disc int 
*/
GO
