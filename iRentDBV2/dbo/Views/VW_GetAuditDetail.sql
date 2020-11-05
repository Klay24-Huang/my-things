CREATE VIEW [dbo].[VW_GetAuditDetail]
	AS 
SELECT  [AuditID]
      ,AuditData.[MEMIDNO]
      ,AuditData.[MEMCNAME]
      ,AuditData.[MEMTEL]
      ,AuditData.[MEMBIRTH]
      ,AuditData.[MEMCOUNTRY]
      ,AuditData.[MEMCITY]
      ,AuditData.[MEMADDR]
      ,AuditData.[MEMEMAIL]
      ,AuditData.[MEMCOMTEL]
      ,AuditData.[MEMCONTRACT]
      ,AuditData.[MEMCONTEL]
      ,AuditData.[MEMMSG]
      ,AuditData.[CARDNO]
      ,AuditData.[UNIMNO]
      ,AuditData.[MEMSENDCD]
      ,AuditData.[CARRIERID]
      ,AuditData.[NPOBAN]
	  ,ISNULL(MemberData.[HasCheckMobile],0) AS [HasCheckMobile]
      ,[AuditKind]
      ,[HasAudit]
      ,[IsNew]
	  ,ISNULL(City.CityID,0) AS CityID
	  ,ISNULL(City.CityName,'') AS CityName
	  ,ISNULL(Area.AreaID,0) AS AreaID
	  ,ISNULL(Area.AreaName,'') AS AreaName
	  ,ISNULL((SELECT TOP 1 [AuditUser] FROM [TB_AuditHistory] WHERE IDNO=AuditData.MEMIDNO ORDER BY AuditHistoryID DESC) ,'') AS LastOpt
  FROM [dbo].[TB_MemberDataOfAutdit] AS AuditData
  LEFT JOIN TB_MemberData AS MemberData ON MemberData.MEMIDNO=AuditData.MEMIDNO
  LEFT JOIN TB_AreaZip AS Area ON Area.AreaID=AuditData.MEMCITY
  LEFT JOIN TB_City AS City ON City.CityID=Area.CityID
  
    GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditDetail';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditDetail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取得需要審核的資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditDetail';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditDetail';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetAuditDetail';