/****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2021/04/07 ADD BY YEH
** 2021/07/26 UPD BY YEH REASON:增加StationID
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetAlertMailLog]
	@SDate	DATETIME,	--起日
	@EDate	DATETIME	--迄日
AS

DECLARE @NowDate Datetime;

SET @NowDate=DATEADD(HOUR,8,GETDATE());

SELECT AlertID,EventType,Receiver,CarNo,OrderNo,StationID,HasSend,SendTime,MKTime 
FROM TB_AlertMailLog WITH(NOLOCK) 
WHERE 1=1 
AND (HasSend=0 AND MKTime BETWEEN @SDate AND @EDate) 
OR (HasSend=2 AND MKTime >= DATEADD(HOUR,-4,@NowDate))
OR (HasSend=0 AND UPDTime BETWEEN @SDate AND @EDate)
ORDER BY AlertID DESC;
GO