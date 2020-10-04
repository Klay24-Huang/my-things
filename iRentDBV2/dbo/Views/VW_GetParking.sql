CREATE VIEW [dbo].[VW_GetParking]
	AS
	
SELECT [ParkingType]
      ,[ParkingName]
      ,[ParkingAddress]
      ,[ParkingLng] AS Longitude
      ,[ParkingLat] AS Latitude
      ,[OpenTime]
      ,[CloseTime]
	  ,use_flag
  FROM [dbo].[TB_ParkingData]
  WHERE ParkingLng<>0 AND ParkingLat<>0	--20201003 ADD BY ADAM 排除經緯度為0的
  UNION(
  SELECT [ParkingType]

      ,[Name] AS ParkingName
	   ,[addr] AS [ParkingAddress]
	   ,[lng] AS Longitude
      ,[lat]  AS Latitude
      ,[StartTime]
      ,[CloseTime]
	  ,use_flag
  FROM [dbo].[TB_MochiPark]
  WHERE lng<>0 AND lat<>0				--20201003 ADD BY ADAM 排除經緯度為0的
  )

