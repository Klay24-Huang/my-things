CREATE VIEW [dbo].[VW_GetFavoriteStation]
	AS 
SELECT IDNO
	,f.StationID
	,s.[Location] AS StationName
	,s.ADDR
	,s.Tel
	,s.Latitude
	,s.Longitude
	,s.Content
	,f.MKTime
	,ContentForAPP --20210319 ADD BY ADAM
	,IsRequiredForReturn --20210319 ADD BY ADAM
FROM [TB_FavoriteStation] AS f WITH (NOLOCK)
INNER JOIN TB_iRentStation AS s WITH (NOLOCK) ON f.StationID = s.StationID
WHERE s.EDate >= DATEADD(HOUR,8,GETDATE())
GO