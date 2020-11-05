CREATE VIEW [dbo].[VW_BE_GetiRentStation]
	AS 
	SELECT  [StationID] ,[ManageStationID] ,[Location] AS StationName ,[Tel] ,[ADDR]
      ,[Latitude]
      ,[Longitude]
      ,[Content]
      ,[ContentForAPP]
      ,[UNICode]
      ,[CityID]
      ,[AreaID]
      ,[IsRequiredForReturn]
      ,[CommonLendStation]
      ,[FCODE]
      ,[SDate]
      ,[EDate]
      ,[IsNormalStation]
      ,[AllowParkingNum]
      ,[NowOnlineNum]
      ,[use_flag]
      ,[Area]

  FROM [TB_iRentStation]
    GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetiRentStation';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetiRentStation';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取得據點資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetiRentStation';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetiRentStation';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetiRentStation';
