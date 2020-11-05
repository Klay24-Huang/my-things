CREATE VIEW [dbo].[VW_GetAuditImage]
	AS 
	SELECT  AuditCrential.IDNO
      ,AuditCrential.[CrentialsType]
      ,AuditCrential.[CrentialsFile]
	  ,ISNULL(ACredent.[CrentialsType],99) AS AlreadyType
	   ,ISNULL(ACredent.[CrentialsFile],'') AS AlreadyFile
      ,ISNULL(Member.IsNew,0) AS IsNew
  FROM [dbo].[TB_tmpCrentialsPIC] AS AuditCrential
  LEFT JOIN [dbo].[TB_AuditCredentials] AS ACredent ON ACredent.IDNO=AuditCrential.IDNO AND ACredent.[CrentialsType]=AuditCrential.CrentialsType
  LEFT JOIN [dbo].TB_MemberDataOfAutdit AS Member ON Member.MEMIDNO=AuditCrential.IDNO

      GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditImage';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditImage';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取得待審核的照片', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditImage';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditImage';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditImage';