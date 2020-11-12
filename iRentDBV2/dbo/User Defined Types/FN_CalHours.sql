-- =============================================
-- Author:      Adam
-- Create Date: 2020-11-11
-- Description: 時數試算
-- =============================================
/* 範例
SELECT dbo.FN_CalHours(GETDATE(),DATEADD(hour,25,GETDATE()))
*/
CREATE FUNCTION [dbo].[FN_CalHours]
(
    @SD datetime, --預計取車時間
	@ED datetime --預計還車時間
)
RETURNS FLOAT
AS
BEGIN
	declare @totalHours float = FLOOR( DATEDIFF(hour, @SD, @ED)) 
	declare @totalMinutes int = DATEDIFF(minute, @SD, @ED) % 60 --日期相減剩餘分鐘數
	DECLARE @CalHours FLOAT
	declare @Day float =  FLOOR(@totalHours / 24)	
	declare @tHours float = CAST(@totalHours as INT) % 24 --因為float無法取餘數,所以@totalHours先轉整數再算

	if @tHours >= 10
	begin
	   set @Day +=1
	   set @tHours = 0
	   set @totalMinutes = 0
	end

	RETURN (@Day * 10) + @tHours
END