CREATE VIEW [dbo].[VW_BE_GetParkingDetail]
	AS 
	 SELECT          OrderParkData.order_number AS OrderNo, MochiPark.Name, MochiPark.city, MochiPark.addr, 
                            OrderParkData.Amount, OrderParkData.Check_in, OrderParkData.Check_out
FROM              dbo.TB_OrderParkingDetailByMachi AS OrderParkData WITH (NOLOCK) LEFT OUTER JOIN
                            dbo.TB_MochiPark AS MochiPark WITH (NOLOCK) ON MochiPark.Id = OrderParkData.machi_parking_id
  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetParkingDetail';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetParkingDetail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'【後台】查詢車麻吉進出明細', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetParkingDetail';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetParkingDetail';


GO
