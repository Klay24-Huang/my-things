CREATE VIEW [dbo].[VW_BE_GetContactData]
	AS 
	SELECT VW.[OrderNo],VW.[CarNo],CarDetail.EngineNO,CarDetail.CarColor
	   ,VW.StartMile,VW.FS,CarDetail.CarBrend,VW.[CarTypeName]
	   ,VW.ED,VW.LStation,VW.UserName,VW.IDNO,ISNULL(MemberData.MEMBIRTH,'1911-01-01 00:00:00') AS MEMBIRTH
	   ,VW.TEL,ISNULL(City.CityName,'') AS CityName,ISNULL(Area.AreaName,'') AS AreaName,MemberData.MEMADDR
	   ,VW.[PurePrice],VW.FE,VW.[FinalPrice],VW.CarRent
	   ,VW.Mileage,VW.FinePrice,CredentialImg.Signture AS hasSignture
	   ,ISNULL(CredentialPic.CrentialsFile,'') AS Signture
  FROM [dbo].[VW_BE_GetOrderFullDetail] AS VW
  LEFT JOIN TB_MemberData AS MemberData ON MemberData.MEMIDNO=VW.IDNO
  LEFT JOIN VW_GetCarDetail AS CarDetail ON CarDetail.CarNo=VW.CarNo
  LEFT JOIN TB_AreaZip AS Area ON Area.AreaID=MemberData.MEMCITY
  LEFT JOIN TB_City AS City ON City.CityID=Area.CityID
  LEFT JOIN TB_Credentials AS CredentialImg ON CredentialImg.IDNO=VW.IDNO
    LEFT JOIN TB_CrentialsPIC AS CredentialPic ON CredentialPic.IDNO=VW.IDNO AND CredentialPic.CrentialsType=11
	WHERE VW.CMS>=4
       GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetContactData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetContactData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'產生合約資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetContactData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetContactData';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetContactData';