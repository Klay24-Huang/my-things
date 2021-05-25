CREATE VIEW [dbo].[VW_BE_GetRawDataOfMachi]
	AS 
SELECT MachiParking.machi_id,
       ISNULL(OrderDetail.order_number, 0) AS OrderNo,
       MachiParking.carno AS CarNo,
       ISNULL(ParkingData.Name, MachiParking.machi_parking_id) AS Name,
       MachiParking.Check_in,
       MachiParking.Check_out,
       ISNULL(OrderDetail.final_start_time, '1911-01-01 00:00:00') AS SD,
       ISNULL(OrderDetail.final_stop_time, '1911-01-01 00:00:00') AS ED,
       MachiParking.Amount
FROM dbo.TB_OrderParkingDetailByMachi AS MachiParking WITH (NOLOCK)
LEFT JOIN dbo.TB_MochiPark AS ParkingData WITH (NOLOCK) ON ParkingData.Id = MachiParking.machi_parking_id
LEFT JOIN dbo.TB_OrderDetail AS OrderDetail WITH (NOLOCK) ON OrderDetail.order_number = MachiParking.order_number
GO
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetRawDataOfMachi';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetRawDataOfMachi';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'【後台】查詢車麻吉進出明細報表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetRawDataOfMachi';
GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetRawDataOfMachi';
GO