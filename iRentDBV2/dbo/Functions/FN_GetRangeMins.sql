-- =============================================
-- Author:		eason
-- Create date: 2020-11-24
-- Description:	時間區段內平日計費分鐘總和,假日計費分鐘總和
-- =============================================
CREATE FUNCTION [dbo].[FN_GetRangeMins]
(
    @sd datetime, --起
	@ed datetime, --迄
	@baseMinutes int , --基本分鐘數
	@dayMaxMins int , --每日最大收費分鐘數 
	@minsPro varchar(50) = '', --剩餘分鐘數加工: car 
	@dayPro varchar(50) = '' --單日時間特殊邏輯: carOverTime
)
RETURNS 
@re TABLE 
(
   w_mins int,
   h_mins int
)
AS
BEGIN
	declare @w_allMins int = 0 --總平日分鐘
	declare @h_allMins int = 0 --總假日分鐘

	if @sd is null or @ed is null or @sd > @ed
	  return
    else
	begin
	   set @sd = DATEADD(second, DATEPART(SECOND, @sd)*-1,@sd)--去秒數
	   set @ed = DATEADD(second, DATEPART(SECOND, @ed)*-1,@ed)--去秒數

	   declare @str_sd varchar(10) = convert(varchar, @sd, 112) 
	   declare @str_ed varchar(10) = convert(varchar, @ed, 112) 

	   declare @mins int = datediff(minute,@sd,@ed)

	   if @str_sd = @str_ed or (DATEADD(HOUR,24,@sd) >= @ed)
	   begin
		   select top 1 
		   @w_allMins += g.w_mins, 
		   @h_allMins += g.h_mins 
		   from dbo.FN_GetH24Mins(@sd, @ed, @baseMinutes, @dayMaxMins, @minsPro, @dayPro) g
	   end
	   else
	   begin
			while ( @sd < @ed)
			begin		
			   declare @sd24 datetime = dateadd(hour,24,@sd)
			   if @ed > @sd24
			   begin
		            select top 1 
					@w_allMins +=g.w_mins, 
					@h_allMins +=g.h_mins 
					from dbo.FN_GetH24Mins(@sd, @sd24, @baseMinutes, @dayMaxMins, @minsPro, @dayPro) g			      
			   end
			   else
			   begin
		            select top 1 
					@w_allMins +=g.w_mins, 
					@h_allMins +=g.h_mins 
					from dbo.FN_GetH24Mins(@sd, @ed, @baseMinutes, @dayMaxMins, @minsPro, @dayPro) g		      
			   end
			   set @sd = @sd24
			end	    
	   end
	end

	insert into @re
	select @w_allMins, @h_allMins 

	RETURN 
END

/*
  declare @sd datetime
  declare @ed datetime
  declare @baseMinutes int
  declare @dayMaxMins int
  declare @minsPro varchar(50) = ''
  declare @dayPro varchar(50) = ''

  declare @re table
  (
    w_mins int,
    h_mins int
  )
*/
GO