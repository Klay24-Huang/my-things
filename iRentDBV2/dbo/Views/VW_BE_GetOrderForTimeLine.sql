CREATE VIEW [dbo].[VW_BE_GetOrderForTimeLine]
	AS 
	SELECT Main.order_number AS OrderNo,Main.CarNo,Main.start_time AS SD,Main.stop_time AS ED,
       ISNULL(Detail.final_start_time,'1911-01-01 00:00:00') AS FS, ISNULL(Detail.final_stop_time,'1911-01-01 00:00:00') AS FE
FROM TB_OrderMain AS Main
INNER JOIN TB_OrderDetail AS Detail ON Main.order_number=Detail.order_number
WHERE Main.cancel_status=0

  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderForTimeLine';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderForTimeLine';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台-車輛軌跡查詢（訂單）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderForTimeLine';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderForTimeLine';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderForTimeLine';