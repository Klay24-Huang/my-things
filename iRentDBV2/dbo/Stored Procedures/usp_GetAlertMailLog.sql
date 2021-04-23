/****** Object:  StoredProcedure [dbo].[usp_GetAlertMailLog]    Script Date: 2021/4/7 上午 11:05:36 ******/

/****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2021/04/07 11:10:00 Jet Add
** 
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetAlertMailLog]
	@SDate	DATETIME,	--起日
	@EDate	DATETIME	--迄日
AS

DECLARE @NowDate Datetime;

SET @NowDate=DATEADD(HOUR,8,GETDATE());

SELECT AlertID,EventType,Receiver,CarNo,HasSend,SendTime,MKTime 
FROM TB_AlertMailLog WITH(NOLOCK) 
WHERE 1=1 
AND (HasSend=0 AND MKTime BETWEEN @SDate AND @EDate) 
OR (HasSend=2 AND MKTime >= DATEADD(HOUR,-4,@NowDate))
ORDER BY AlertID DESC;
GO