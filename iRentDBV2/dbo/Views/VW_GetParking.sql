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
  )

