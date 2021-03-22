CREATE VIEW [dbo].[VW_GetFavoriteStation]
	AS 
	SELECT [IDNO],Favorite.StationID,Station.[Location] AS StationName,Station.ADDR,Station.Tel,Station.Latitude,Station.Longitude,Station.Content,Favorite.MKTime
	,ContentForAPP,IsRequiredForReturn			--20210319 ADD BY ADAM REASON.補上欄位ContentForAPP,IsRequiredForReturn
  FROM [TB_FavoriteStation] AS Favorite
  INNER JOIN TB_iRentStation AS Station WITH(NOLOCK) ON Favorite.StationID=Station.StationID 
