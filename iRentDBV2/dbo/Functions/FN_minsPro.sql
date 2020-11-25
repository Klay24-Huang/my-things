-- =============================================
-- Author:     eason
-- Create Date: 2020-11-24
-- Description: 剩餘分轉計費分(未滿60)
-- =============================================
CREATE FUNCTION [dbo].[FN_minsPro]
(
   @funType varchar(20) = '', --car
   @mins int = 0 --剩餘分
)
RETURNS int
AS
BEGIN
   
    if @funType = 'car'
	begin
	   if @mins < 0 or @mins >60
	      return 0       
	   if @mins >= 15 and @mins < 45
	      return 30
       if @mins >= 45
	      return 60
	end	  

    RETURN @mins
END
GO