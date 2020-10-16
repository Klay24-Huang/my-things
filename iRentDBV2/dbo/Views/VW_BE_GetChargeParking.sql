CREATE VIEW [dbo].[VW_BE_GetChargeParking]
	AS 
	SELECT  [ParkId]
      ,[Name] AS ParkingName
	  ,ISNULL([Operator],'') AS [Operator]
      ,[lat] AS Latitude
      ,[lng] AS Longitude
      --,ISNULL([city],'') AS City 
      ,ISNULL([addr],'') AS ParkingAddress
      ,ISNULL([SettingPrice],0) AS SettingPrice
      ,[StartTime]
      ,[CloseTime]

  FROM [dbo].[TB_MochiPark]


        GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetChargeParking';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetChargeParking';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台停車便利付，之後若有再增加廠商，要再加上join', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetChargeParking';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetChargeParking';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetChargeParking'