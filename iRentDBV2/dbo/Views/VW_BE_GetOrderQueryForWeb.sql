CREATE VIEW [dbo].[VW_BE_GetOrderQueryForWeb]
	AS 
	SELECT [OrderNum],[IDNO],[CarNo],[car_mgt_status],[cancel_status]
      ,[SD],[ED],[FS],[FE],VW.[StationID]
	  ,Station.[Location] AS StationName
  FROM [dbo].[VW_GetCarSchedule] AS VW 
  LEFT JOIN [TB_iRentStation] AS Station ON Station.StationID=VW.StationID
      GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderQueryForWeb';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderQueryForWeb';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台訂單查詢列表專用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderQueryForWeb';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderQueryForWeb';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderQueryForWeb';