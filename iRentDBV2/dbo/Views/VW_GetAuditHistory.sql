CREATE VIEW [dbo].[VW_GetAuditHistory]
	AS 
	SELECT [AuditHistoryID]
      ,ISNULL(Member.MEMCNAME,'') AS UserName
	  ,ISNULL(Manager.UserName,'') AS MUserName
      ,[IDNO]
      ,[AuditUser]
      ,[AuditDate]
      ,[HandleItem]
      ,[HandleType]
      ,[IsReject]
      ,[RejectReason]
      ,[RejectExplain]
  FROM [TB_AuditHistory] AS AHistory
  LEFT JOIN [TB_MemberData] AS Member ON Member.MEMIDNO=AHistory.IDNO
  LEFT JOIN [TB_Manager] AS Manager ON Manager.Account=AHistory.AuditUser
     GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditHistory';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditHistory';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台會員審核，取出審核歷程', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditHistory';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditHistory';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditHistory';
