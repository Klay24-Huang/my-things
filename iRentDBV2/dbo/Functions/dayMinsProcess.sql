-- =============================================
-- Author:		eason
-- Create date: 2020-11-24
-- Description:	單日時間轉計費時間(特殊)
-- =============================================
CREATE FUNCTION [dbo].[dayMinsProcess]
(
    @funType varchar(20) = '', --carOverTime /	汽車逾時
	@wMins int = 0,
	@hMins int = 0
)
RETURNS 
@re table(
  wMins int,
  hMins int
)
AS
BEGIN
	declare @re_wMins int = 0
	declare @re_hMins int = 0
	
	if @funType = 'carOverTime'
	begin
		set @re_wMins = iif(@wMins>=360, 600, @wMins)
		set @re_hMins = iif(@hMins>=360, 600, @hMins)
	end
	else
	begin
	   set @re_wMins = @wMins
	   set @re_hMins = @hMins
	end

    insert into @re values(@re_wMins, @re_hMins)

	RETURN 
END
GO