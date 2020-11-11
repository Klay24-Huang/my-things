CREATE VIEW [dbo].[VW_GetAuditList]
	AS 
	  SELECT MKTime AS ApplyDate,MEMCNAME,MEMIDNO,SUBSTRING(MEMIDNO,2,1) AS SEX,AuditKind,HasAudit,IsNew,SUBSTRING(MEMIDNO,10,1) AS IDNOSUFF
  FROM [TB_MemberDataOfAutdit] WITH(NOLOCK)
  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditList';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditList';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取得需要審核的資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditList';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditList';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditList';