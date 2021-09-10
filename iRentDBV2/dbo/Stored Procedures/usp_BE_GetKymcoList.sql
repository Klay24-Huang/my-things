/****************************************************************
EXEC usp_BE_GetKymcoList  '1',NULL,NULL
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_GetKymcoList]
	@AuditMode	INT,
	@StartDate	datetime,
	@EndDate    datetime 
AS	


IF @AuditMode = 1
BEGIN
	SELECT MaintainUserName AS UserID 
	,ISNULL((select name from A_20210115_KymcoMaintainUser where id=MaintainUserName),'') AS UserName
	,ISNULL((select area from A_20210115_KymcoMaintainUser where id=MaintainUserName),'') AS Area
	,ISNULL((select type from A_20210115_KymcoMaintainUser where id=MaintainUserName),'') AS TypeK
	,ISNULL(CarNo,'') AS CarNo
	,ISNULL(Dealer,'') AS DealerCodeValue
	,ISNULL(CarAddr,'') AS MemoAddr
	,case ISNULL(MaintainType,0) when 1 then '排拖' when 2 then '經銷商' else '' end AS MaintainType
	,ISNULL(Reason,'') AS Reason
	,ISNULL(Offline,'') AS Offline
	,CONVERT(char(20),DATEADD(hour,8, MKTime),20) AS UpdTime 
	FROM TB_MotoMaintain WITH(NOLOCK) 
	where CONVERT(char(8),DATEADD(hour,8, MKTime),112) between 
	ISNULL(CONVERT(char(8),@StartDate,112),CONVERT(char(8),GETDATE(),112)) and 
	ISNULL(CONVERT(char(8),@EndDate,112),CONVERT(char(8),GETDATE(),112))
	ORDER BY DATEADD(hour,8, MKTime) DESC 
END
ELSE
BEGIN
	SELECT UserID AS UserID 
	,ISNULL((select name from A_20210115_KymcoMaintainUser where id=UserID),'') AS UserName
	,ISNULL((select area from A_20210115_KymcoMaintainUser where id=UserID),'') AS Area 
	,ISNULL((select type from A_20210115_KymcoMaintainUser where id=UserID),'') AS TypeK
	,ISNULL(CarNo,'') AS CarNo
	,ISNULL(CodeValue,'') AS DealerCodeValue
	,ISNULL(Memo,'') AS MemoAddr
	,'' AS MaintainType
	,'' AS Reason
	,'' AS Offline
	,CONVERT(char(20),DATEADD(hour,8, MKTime),20) AS UpdTime 
	FROM TB_PrepareLog WITH(NOLOCK) 
	where CONVERT(char(8),DATEADD(hour,8, MKTime),112) between 
	ISNULL(CONVERT(char(8),@StartDate,112),CONVERT(char(8),GETDATE(),112)) and 
	ISNULL(CONVERT(char(8),@EndDate,112),CONVERT(char(8),GETDATE(),112))
	ORDER BY DATEADD(hour,8, MKTime) DESC 
END
