CREATE VIEW [dbo].[VW_BE_GetRawDataOfMachi]
	AS 
	SELECT      OrderParkingData.machi_id, ISNULL(BookingDetail.order_number, 0) AS OrderNo, OrderParkingData.CarNo, ISNULL(ParkingData.Name,OrderParkingData.machi_parking_id) AS Name, 
                   OrderParkingData.Check_in, OrderParkingData.Check_out, ISNULL(BookingDetail.final_start_time, '1911-01-01 00:00:00') AS SD, 
                   ISNULL(BookingDetail.final_stop_time, '1911-01-01 00:00:00') AS ED, OrderParkingData.Amount, OrderParkingData.refund_amount, 
                   OrderParkingData.paid_at, OrderParkingData.Conviction
FROM         dbo.TB_OrderParkingDetailByMachiOfSync AS OrderParkingData
LEFT JOIN    dbo.TB_MochiPark AS ParkingData WITH (NOLOCK) ON ParkingData.Id = OrderParkingData.machi_parking_id
LEFT  JOIN   dbo.TB_OrderDetail AS BookingDetail WITH (NOLOCK) ON BookingDetail.order_number = OrderParkingData.order_number
  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetRawDataOfMachi';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetRawDataOfMachi';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'【後台】查詢車麻吉進出明細報表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetRawDataOfMachi';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetRawDataOfMachi';


GO
