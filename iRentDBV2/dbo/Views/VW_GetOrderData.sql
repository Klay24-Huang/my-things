

CREATE VIEW [dbo].[VW_GetOrderData]
AS
	SELECT OrderMain.[order_number]
		,OrderMain.[IDNO]
		,OrderMain.[CarNo]
		,OrderMain.[ProjID]
		,OrderMain.[lend_place]
		,OrderMain.[return_place]
		,OrderMain.[start_time]
		,OrderMain.[stop_time]
		,OrderMain.[stop_pick_time]
		,OrderMain.[fine_Time]
		,OrderMain.[init_price]
		,OrderMain.[init_TransDiscount]
		,OrderMain.[Insurance]
		,OrderMain.[InsurancePurePrice]
		,OrderMain.[car_mgt_status]
		,OrderMain.[booking_status]
		,OrderMain.[cancel_status]
		,OrderMain.[modified_status]
		,OrderMain.[ProjType]
		,OrderMain.[PayMode]
		,OrderMain.invoice_date
		,OrderMain.invoice_price
		,OrderMain.invoiceCode
		,OrderMain.bill_option
		,OrderMain.[CARRIERID]
		,OrderMain.[NPOBAN]
		,OrderMain.unified_business_no
		,ISNULL(OrderDetil.already_lend_car,2) AS already_lend_car
		,OrderDetil.final_start_time
		,OrderDetil.final_stop_time
		,OrderDetil.start_door_time		--20201027 ADD BY ADAM REASON
		,OrderDetil.end_door_time			--20201027 ADD BY ADAM REASON
		,ISNULL(OrderDetil.final_price,0) AS final_price
		,ISNULL(OrderDetil.pure_price,0) AS  pure_price
		,ISNULL(OrderDetil.mileage_price,0) AS mileage_price
		,ISNULL(OrderDetil.Insurance_price,0) AS Insurance_price
		,ISNULL(OrderDetil.fine_price,0) AS fine_price
		,ISNULL(OrderDetil.gift_point,0) AS gift_point
		,ISNULL(OrderDetil.gift_motor_point, 0) AS gift_motor_point
		,ISNULL(OrderDetil.Etag,0) AS Etag
		,ISNULL(OrderDetil.already_payment,0) AS already_payment
		,ISNULL(OrderDetil.start_mile,0) AS start_mile
		,ISNULL(OrderDetil.end_mile,0) AS end_mile
		,ISNULL(OrderDetil.trade_status,0) AS trade_status
		,ISNULL(OrderDetil.parkingFee,0) AS parkingFee
		,ISNULL(OrderDetil.[parkingSpace],'') AS parkingSpace
		,ISNULL(OrderDetil.TransDiscount,0) AS TransDiscount
		,OrderDetil.monthly_workday
		,OrderDetil.monthly_holiday
		,Car.CarType
		,Car.CarOfArea
		,Car.LastOrderNo
		,Car.available AS IsReturnCar
		,Car.NowOrderNo	--20201026 ADD BY ADAM
		,CarInfo.IsMotor --20201006 - eason
		,CarInfo.WeekdayPrice --20201006 - eason
		,CarInfo.HoildayPrice	--20201006 - eason  
		,Operator.OperatorID
		,Operator.OperatorName
		,Operator.OperatorICon
		,Operator.Score
		,VW.PRONAME
		,VW.PRODESC
		,VW.PRICE
		,VW.PRICE_H
		,VW.CarBrend
		,VW.CarTypeName
		,VW.CarTypeGroupCode
		,VW.CarTypeImg
		,VW.Seat
		,ISNULL(PriceByMinutes.BaseMinutes,0) AS BaseMinutes
		,ISNULL(PriceByMinutes.BaseMinutesPrice,0) AS BaseMinutesPrice
		,ISNULL(PriceByMinutes.Price,0.0) AS MinuteOfPrice
		,ISNULL(PriceByMinutes.MaxPrice,0) AS MaxPrice
		,ISNULL(PriceByMinutes.MaxPriceH,0) AS MaxPriceH --20201006 - eason
		,ISNULL(CarStatus.device3TBA,0) AS device3TBA
		,ISNULL(CarStatus.deviceRDistance,'') AS RemainingMilage
		,CarStatus.Latitude AS CarLatitude
		,CarStatus.Longitude AS CarLongitude
		,CarStatus.Millage
		,Station.Content AS [Content]
		,Station.Latitude
		,Station.Longitude
		,Station.[Location] AS StationName 
		,Station.ADDR 
		,Station.Tel
		,Station.Area
	FROM [dbo].[TB_OrderMain] AS OrderMain WITH(NOLOCK)
	LEFT JOIN [dbo].[TB_OrderDetail] AS OrderDetil WITH(NOLOCK) ON OrderDetil.order_number=OrderMain.order_number 
	LEFT JOIN [dbo].[TB_Car] AS Car WITH(NOLOCK) ON Car.CarNo=OrderMain.CarNo
	LEFT JOIN [dbo].[TB_CarInfo] CarInfo WITH(NOLOCK) ON CarInfo.CarNo = OrderMain.CarNo --20201006 - eason  
	LEFT JOIN [dbo].[TB_OperatorBase] AS Operator  WITH(NOLOCK)  ON Operator.OperatorID=Car.Operator
	LEFT JOIN [dbo].[VW_GetFullProjectCollectionOfCarTypeGroup] AS VW  WITH(NOLOCK) ON VW.CARTYPE=Car.CarType AND VW.StationID=OrderMain.lend_place AND VW.PROJID=OrderMain.ProjID AND VW.IOType='O'
	LEFT JOIN [dbo].[TB_PriceByMinutes] AS PriceByMinutes WITH(NOLOCK) ON PriceByMinutes.CarType=Car.CarType AND PriceByMinutes.ProjID=VW.PROJID
	LEFT JOIN [dbo].[TB_CarStatus] AS CarStatus WITH(NOLOCK) ON CarStatus.CarNo=OrderMain.CarNo AND CarInfo.CID=CarStatus.CID		--20201127 ADD BY ADAM REASON.過濾多的CID
	LEFT JOIN [dbo].[TB_iRentStation] AS Station WITH(NOLOCK) ON Station.StationID=OrderMain.lend_place
GO
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetOrderData';

GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetOrderData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂單資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetOrderData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetOrderData';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetOrderData';

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetOrderData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'0
            TopColumn = 0
         End
         Begin Table = "CarStatus"
            Begin Extent = 
               Top = 666
               Left = 38
               Bottom = 796
               Right = 247
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Station"
            Begin Extent = 
               Top = 798
               Left = 38
               Bottom = 928
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetOrderData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[22] 4[22] 2[37] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "OrderMain"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 238
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OrderDetil"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 260
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Car"
            Begin Extent = 
               Top = 6
               Left = 276
               Bottom = 136
               Right = 443
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "CarInfo"
            Begin Extent = 
               Top = 270
               Left = 38
               Bottom = 400
               Right = 265
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Operator"
            Begin Extent = 
               Top = 402
               Left = 38
               Bottom = 532
               Right = 225
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "VW"
            Begin Extent = 
               Top = 402
               Left = 263
               Bottom = 532
               Right = 466
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PriceByMinutes"
            Begin Extent = 
               Top = 534
               Left = 38
               Bottom = 664
               Right = 247
            End
            DisplayFlags = 28', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetOrderData';
