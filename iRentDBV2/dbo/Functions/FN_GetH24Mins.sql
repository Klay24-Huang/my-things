-- =============================================
-- Author:		eason
-- Create date: 2020-11-24
-- Description:	24小時計費分鐘數
-- =============================================
CREATE FUNCTION [dbo].[FN_GetH24Mins]
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
	declare @w_allmins int =0 --總平日分鐘
	declare @h_allmins int = 0 --總假日分鐘


	if @sd is null or @ed is null or @sd > @ed
	  return 
    else
	begin
	   set @sd = DATEADD(second, DATEPART(SECOND, @sd)*-1,@sd)--去秒數
	   set @ed = DATEADD(second, DATEPART(SECOND, @ed)*-1,@ed)--去秒數

	   declare @mins int = datediff(minute,@sd,@ed)
	   if @mins > 24*60 --不可大於24小時
	     return	   

	   declare @sd_end datetime = convert(datetime, (convert(varchar, dateadd(day,1, @sd), 111)+ ' ' + '00:00:00'))

	   declare @xhours int = round(@mins/60,0,1)
	   declare @xmins int = @mins % 60 --未達1小時分鐘數

	   declare @sd10 datetime = dateadd(minute,@dayMaxMins,@sd) --計費截止時間

	   declare @str_sd varchar(10) = convert(varchar, @sd, 112) 
	   declare @str_ed varchar(10) = convert(varchar, @ed, 112) 

	   	declare @sd_isHoliday int = 0
		declare @ed_isHoliday int = 0

		select top 1 @sd_isHoliday = h.HolidayID from TB_Holiday h
		where h.HolidayDate = @str_sd and h.use_flag = 1	

		select top 1 @ed_isHoliday = h.HolidayID from TB_Holiday h
		where h.HolidayDate = @str_ed and h.use_flag = 1	

	    if @mins < @baseMinutes --未達基本分鐘
		begin
		   if @sd_isHoliday > 0
		      set @h_allmins = @baseMinutes
           else
		      set @w_allmins = @baseMinutes
		end	    
		else
		begin
		   if @str_sd = @str_ed
		   begin		     
			 if @minsPro <> '' and @xmins >0 and @xmins < 60
			    set @xmins = dbo.FN_minsPro(@minsPro, @xmins)
             
			 declare @allPayMins int = @xhours * 60 + @xmins
			 set @allPayMins = iif(@allPayMins > @dayMaxMins, @dayMaxMins, @allPayMins)

			 if @sd_isHoliday > 0
			    set @h_allmins = @allPayMins
             else
			    set @w_allmins = @allPayMins
		   end
		   else
		   begin
		      if @mins < @dayMaxMins--未達上限
			     set @sd10 = @ed

              if @sd10 <= @sd_end--計費未跨日
			  begin
			      declare @pay_mins int = datediff(MINUTE,@sd,@sd10)
				  declare @pay_xhours int = round(@pay_mins/60,1,0)
				  declare @pay_xmins int = @pay_mins % 60

				  if @minsPro <> '' and @pay_xmins >0 and @pay_xmins < 60
				     set @pay_xmins = dbo.FN_minsPro(@minsPro,@pay_xmins)
				  
				  declare @allPayMis int = @pay_xhours * 60 + @pay_xmins
				  
				  if @sd_isHoliday > 0
				     set @h_allmins = @allPayMis;
                  else
				     set @w_allmins = @allPayMis
			  end
			  else
			  begin
			      declare @bef_mins int = datediff(MINUTE, @sd, @sd_end) --前日總分鐘
				  declare @bef_xhours int = round(@bef_mins/60,1,0) --前日小時
				  declare @bef_xmins int = @bef_mins % 60 --前日分

                  if @bef_xmins = 0 --物理整點
				  begin
				      if @sd_isHoliday > 0
					      set @h_allmins += @bef_xhours * 60
                      else
					      set @w_allmins += @bef_xhours * 60

                      declare @_af_mins int = datediff(minute,@sd_end,@sd10)
					  declare @_af_xhour int = round(@_af_mins/60,1,0) --後日小時
					  declare @_af_xmins int = @_af_mins % 60 --後日分
					  
					  if @minsPro <> '' and @_af_xmins > 0 and @_af_xmins < 60 --未滿60處理
					     set @_af_xmins = dbo.FN_minsPro(@minsPro, @_af_xmins)
                      
					  declare @allPayMins2 int =  @_af_xhour * 60 + @_af_xmins

					  if @ed_isHoliday > 0
					      set @h_allmins += @allPayMins2
                      else
					      set @w_allmins += @allPayMins2
				  end
				  else
				  begin
				     --前日完整hour
					 if @sd_isHoliday >0
					     set @h_allmins += @bef_xhours * 60
                     else
					     set @w_allmins += @bef_xhours * 60

                     --後日-前日相對整點的point
					 declare @lastMins int = datediff(minute, dateadd(hour,@bef_xhours,@sd), @ed)

					 if @lastMins < 60
					 begin
					     if @minsPro <> '' and @lastMins >0 and @lastMins < 60
						   set @lastMins = dbo.FN_minsPro(@minsPro, @lastMins)
                         
						 if @sd_isHoliday >0
						    set @h_allmins += @lastMins
                         else
						    set @w_allmins += @lastMins 
					 end
					 else
					 begin
					     --交界小時算前日
						 if @sd_isHoliday > 0
						    set @h_allmins += 60
                         else
						    set @w_allmins += 60
                         
						 --後日-相對整點起
						 declare @af_star datetime = dateadd(hour, @bef_xhours+1,@sd)
						 declare @af_mins int = datediff(minute, @af_star, @sd10)
						 declare @af_xhours int = round( @af_mins/60,1,0)
						 declare @af_xmins int = @af_mins % 60

						 if @minsPro <> '' and @af_xmins > 0 and @af_xmins < 60
						    set @af_xmins = dbo.FN_minsPro(@minsPro, @af_xmins)

                         if @ed_isHoliday > 0
						    set @h_allmins += @af_xhours * 60 + @af_xmins
                         else
						   set @w_allmins += @af_xhours * 60 + @af_xmins
					 end
				  end
			  end
		   end
		end		

		if @dayPro <> ''
		begin
           select top 1 @w_allmins = d.wMins, @h_allmins =  d.hMins from dbo.FN_dayPro(@dayPro, @w_allmins,@h_allmins) d          
		end

		insert into @re
		select @w_allmins, @h_allmins

	end

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