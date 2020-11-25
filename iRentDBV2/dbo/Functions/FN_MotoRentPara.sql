-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE FUNCTION [dbo].[FN_MotoRentPara]
(
    @mins int, --分
	@Price float, --平日價
	@PriceH float, --假日價
	@MaxPrice float, --平日上限
	@MaxPriceH float, --假日上限
	@DateStr varchar(20) --日期
)
RETURNS float
AS
BEGIN
    declare @re float = 0  
	declare @dayMaxMin int = 600 --單日分鐘上限

	declare @HolidayIndex int = 0
	select top 1 @HolidayIndex = h.HolidayID from TB_Holiday h
    where h.HolidayDate = @DateStr 
	and h.use_flag = 1		

	--達上限
	if @mins >= @dayMaxMin 
	begin
       if @HolidayIndex > 0
	      return @MaxPriceH
       else
	      return @MaxPrice
	end
	else
	begin
		if @HolidayIndex > 0
		begin  
		    set @re = @mins * @PriceH
			if @re > @MaxPriceH
			   set @re = @MaxPriceH
		end
		else
		begin
		    set @re = @mins * @Price	   
		    if @re > @MaxPrice
			   set @re = @MaxPrice
		end	   
	end
   
    RETURN FLOOR(@re)
END
GO

