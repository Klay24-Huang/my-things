CREATE VIEW [dbo].[VW_GetFavoriteStation]
	AS 
	SELECT [IDNO],Favorite.StationID,Station.[Location] AS StationName,Station.ADDR,Station.Tel,Station.Latitude,Station.Longitude,Station.Content,Favorite.MKTime
  FROM [TB_FavoriteStation] AS Favorite
  INNER JOIN TB_iRentStation AS Station WITH(NOLOCK) ON Favorite.StationID=Station.StationID 
