CREATE VIEW [dbo].[VW_BE_GetOrderModifyInfo]
	AS 
SELECT          main.order_number, main.CarNo, main.IDNO, userinfo.MEMCNAME, main.start_time, main.stop_time, 
                            ISNULL(main.fine_Time,'1900-01-01 00:00:00.000') AS fine_Time, detail.final_start_time, detail.final_stop_time, detail.start_mile, detail.end_mile, detail.pure_price, 
                            detail.fine_price, detail.mileage_price, detail.Etag, detail.gift_point, detail.final_price, detail.trade_status, 
                            car.CarType, main.ProjID,
                                (SELECT          PRICE_H
                                  FROM               dbo.TB_ProjectDiscount
                                  WHERE           (ProjID = main.ProjID) AND (CARTYPE = car.CarType) ) AS price, 
                            Project.PRONAME, ISNULL(OrderOtherFee.CarDispatch, 0) AS CarDispatch, ISNULL(OrderOtherFee.DispatchRemark, 
                            '') AS DispatchRemark, ISNULL(OrderOtherFee.CleanFee, 0) AS CleanFee, ISNULL(OrderOtherFee.CleanFeeRemark, '') 
                            AS CleanFeeRemark, ISNULL(OrderOtherFee.DestroyFee, 0) AS DestroyFee, 
                            ISNULL(OrderOtherFee.DestroyFeeRemark, '') AS DestroyFeeRemark, ISNULL(OrderOtherFee.ParkingFee, 0) 
                            AS ParkingFee, ISNULL(OrderOtherFee.ParkingFeeRemark, '') AS ParkingFeeRemark, 
                            ISNULL(OrderOtherFee.DraggingFee, 0) AS DraggingFee, ISNULL(OrderOtherFee.DraggingFeeRemark, '') 
                            AS DraggingFeeRemark, ISNULL(OrderOtherFee.OtherFee, 0) AS OtherFee, ISNULL(OrderOtherFee.OtherFeeRemark, 
                            '') AS OtherFeeRemark
FROM              dbo.TB_OrderMain AS main INNER JOIN
                            dbo.TB_OrderDetail AS detail ON main.order_number = detail.order_number INNER JOIN
                            dbo.TB_MemberData AS userinfo ON main.IDNO = userinfo.MEMIDNO INNER JOIN
                            dbo.TB_CarInfo AS car ON main.CarNo = car.CarNo INNER JOIN
                            dbo.TB_Project AS Project ON Project.PROJID = main.ProjID LEFT OUTER JOIN
                            dbo.TB_OrderOtherFee AS OrderOtherFee ON OrderOtherFee.OrderNo = main.order_number


                              GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderModifyInfo';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderModifyInfo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取得訂單修改前資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderModifyInfo';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderModifyInfo';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderModifyInfo';