CREATE VIEW [dbo].[VW_BE_GetSameMobile]
	AS 
SELECT MEMIDNO AS IDNO,MEMCNAME AS UserName,ISNULL(City.CityName,'') AS CityName,ISNULL(AREA.AreaName,'') AS AreaName,MEMADDR,MEMTEL
FROM TB_MemberData  AS MemberData 
LEFT JOIN TB_AreaZip AS AREA ON AREA.AreaID=MemberData.MEMCITY
LEFT JOIN TB_City AS City ON City.CityID=AREA.CityID
WHERE MEMTEL IN (
SELECT MEMTEL
      
  FROM TB_MemberData

 GROUP BY MEMTEL
 having COUNT(MEMTEL)>1
 )


   GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetSameMobile';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetSameMobile';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台會員審核，取出重覆手機號碼的會員', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetSameMobile';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetSameMobile';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetSameMobile';