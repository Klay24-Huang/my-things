CREATE VIEW [dbo].[VW_BE_GetOrderFullDetail]
	AS 
SELECT OrderMain.[order_number] AS OrderNo,
       OrderMain.[IDNO],
       ISNULL(MemberData.MEMCNAME, '') AS UserName,
       booking_date AS BookingDate,
       [car_mgt_status] AS CMS,
       [booking_status] AS BS,
       [cancel_status] AS CS,
       [modified_status] AS MS ,
       [start_time] AS SD,
       [stop_time] AS ED,
       ISNULL((SELECT TOP (1) StopTime FROM TB_OrderExtendHistory AS History WHERE (History.order_number = OrderMain.order_number)), '1911-01-01 00:00:00') AS OStopTime ,
	   ISNULL((SELECT [Location] FROM [TB_iRentStation] WHERE StationID=OrderMain.lend_place),'') AS LStation ,
	   ISNULL((SELECT [Location] FROM [TB_iRentStation] WHERE StationID=OrderMain.[return_place]),'') AS RStation ,
	   CarDetail.CarTypeName,
	   [OrderMain].CarNo,
	   Project.PRONAME ,
	   [bill_option] AS InvoicKind ,
	   [unified_business_no] AS [BCode],
	   MemberData.MEMTEL AS TEL ,
	   init_price AS PurePrice,
	   OrderMain.[CARRIERID],
	   OrderMain.[NPOBAN],
	   [title] ,
	   ISNULL(Detail.final_start_time, '1911-01-01 00:00:00') AS FS,
	   ISNULL(Detail.final_stop_time, '1911-01-01 00:00:00') AS FE ,
	   ISNULL(Detail.start_mile, -1) AS StartMile,
	   ISNULL(Detail.end_mile, -1) AS StopMile,
	   ISNULL(Detail.pure_price, 0) AS CarRent,
	   ISNULL(Detail.final_price, 0) AS FinalPrice,
	   ISNULL(Detail.fine_price, 0) AS FinePrice,
	   ISNULL(Detail.mileage_price, 0) AS Mileage,
	   ISNULL(Detail.Etag, 0) AS eTag,
	   ISNULL(Detail.TransDiscount, 0) AS TransDiscount ,
	   ISNULL(Detail.gift_point, 0) AS CarPoint,
	   ISNULL(Detail.gift_motor_point, 0) AS MotorPoint ,
	   ISNULL(Detail.parkingFee, 0) AS ParkingFeeTotal ,	-- 20210506;ADD BY YEH REASON.顯示合約的總停車費
	   ISNULL([fine_Time], '1911-01-01 00:00:00') AS FineTime,
	   [Insurance],
	   [Insurance_price],	--20210204唐改，由預估金額改抓實際的金額
	   ISNULL((SELECT TOP 1 descript FROM TB_FeedBack AS FeedBack WHERE FeedBack.OrderNo=OrderMain.order_number AND mode=0),'') AS LFeedBack ,
	   ISNULL((SELECT TOP 1 descript FROM TB_FeedBack AS FeedBack WHERE FeedBack.OrderNo=OrderMain.order_number AND mode=1),'') AS RFeedBack ,
	   ISNULL(MotorData.[P_LBA], 0) AS [P_LBA],
	   ISNULL(MotorData.[P_RBA], 0) AS [P_RBA],
	   ISNULL(MotorData.[P_TBA], 0) AS [P_TBA],
	   ISNULL(MotorData.[P_MBA], 0) AS [P_MBA] ,
	   ISNULL(MotorData.[P_lon], 0) AS [P_lon],
	   ISNULL(MotorData.[P_lat], 0) AS [P_lat],
	   ISNULL(MotorData.[R_LBA], 0) AS [R_LBA],
	   ISNULL(MotorData.[R_RBA], 0) AS [R_RBA] ,
	   ISNULL(MotorData.[R_TBA], 0) AS [R_TBA],
	   ISNULL(MotorData.[R_MBA], 0) AS [R_MBA],
	   ISNULL(MotorData.[R_lon], 0) AS [R_lon],
	   ISNULL(MotorData.[R_lat], 0) AS [R_lat] ,
	   ISNULL(MotorData.[Reward], 0) AS [Reward] ,
	   ISNULL(OrderOtherFee.[CarDispatch] , 0) AS [CarDispatch],
	   ISNULL(OrderOtherFee.[DispatchRemark] , '') AS [DispatchRemark],
	   ISNULL(OrderOtherFee.[CleanFee] , 0) AS [CleanFee] ,
	   ISNULL(OrderOtherFee.[CleanFeeRemark] , '') AS [CleanFeeRemark],
	   ISNULL(OrderOtherFee.[DestroyFee] , 0) AS [DestroyFee],
	   ISNULL(OrderOtherFee.[DestroyFeeRemark], '') AS [DestroyFeeRemark] ,
	   ISNULL(OrderOtherFee.[ParkingFee] , 0) AS [ParkingFee],
	   ISNULL(OrderOtherFee.[ParkingFeeRemark], '') AS [ParkingFeeRemark],
	   ISNULL(OrderOtherFee.[DraggingFee] , 0) AS [DraggingFee] ,
	   ISNULL(OrderOtherFee.[DraggingFeeRemark], '') AS [DraggingFeeRemark],
	   ISNULL(OrderOtherFee.[OtherFee] , 0) AS [OtherFee],
	   ISNULL(OrderOtherFee.[OtherFeeRemark] , '') AS [OtherFeeRemark] ,
	   ISNULL((SELECT SUM(Amount) FROM TB_OrderParkingDetailByMachi AS Machi WITH(NOLOCK) WHERE Machi.order_number=OrderMain.order_number),0) AS MachiFee, --20201112 ADD BY JERRY 增加缺漏欄位	--20210423 ADD BY JET 抓TB_OrderParkingDetailByMachi
	   ISNULL(CarDetail.EngineNO, '') AS EngineNO ,
	   ISNULL(CarDetail.CarColor, '') AS CarColor ,
	   ISNULL(CarDetail.CarBrend, '') AS CarBrend ,
	   ISNULL(MemberData.MEMBIRTH, '') AS MEMBIRTH ,
	   ISNULL(City.CityName, '') AS CityName ,
	   ISNULL(AreaZip.AreaName, '') AS AreaName, --20210106唐註解
	   ISNULL(MemberData.MEMADDR, '') AS MEMADDR ,
	   Detail.parkingSpace ,
	   ISNULL((SELECT SUM(CASE WHEN RetCode='1000' THEN amount ELSE 0 END) FROM TB_Trade AS Trade WITH(NOLOCK) WHERE Trade.OrderNo=OrderMain.order_number),0) AS PayAmount ,
	   ISNULL(InsInfo.InsurancePerHours, 0) AS InsurancePerHours ,
	   ISNULL((SELECT [StationID] FROM [TB_iRentStation] WHERE StationID=OrderMain.lend_place),'') AS StationID, --20210325唐加，不然匯出報表會錯，錯好久了
	   --20210727 Add by Umeko s
	   ISNULL(RSOC_S,0) RSOC_S, 
	   ISNULL(RSOC_E,0) RSOC_E,
	   ISNULL(ChgGift,0) ChgGift,
	   ISNULL(ChgTimes,0) ChgTimes,
	   ISNULL(RewardGift,0) RewardGift,
	   ISNULL(TotalGift,0) TotalGift
	   --20210727 Add by Umeko e
FROM [dbo].[TB_OrderMain] AS OrderMain WITH(NOLOCK)
LEFT JOIN TB_OrderDetail AS Detail WITH(NOLOCK) ON Detail.order_number=OrderMain.order_number
LEFT JOIN TB_MemberData AS MemberData WITH(NOLOCK) ON MemberData.MEMIDNO=OrderMain.IDNO
LEFT JOIN TB_AreaZip AS AreaZip WITH(NOLOCK) ON MemberData.MEMCITY=AreaZip.AreaID --20210106唐註解
LEFT JOIN TB_City AS City WITH(NOLOCK) ON AreaZip.CityID=City.CityID --20210106唐註解
LEFT JOIN VW_GetCarDetail AS CarDetail WITH(NOLOCK) ON CarDetail.CarNo=OrderMain.CarNo
LEFT JOIN TB_Project AS Project WITH(NOLOCK) ON Project.PROJID=OrderMain.ProjID
LEFT JOIN TB_OrderDataByMotor AS MotorData WITH(NOLOCK) ON MotorData.OrderNo=OrderMain.order_number
Left Join TB_MotorChangeBattHis As MotorChangeBatteryReward With(Nolock) On MotorChangeBatteryReward.order_number = OrderMain.order_number--20210727 Add by Umeko
LEFT JOIN TB_OrderOtherFee AS OrderOtherFee WITH(NOLOCK) ON OrderOtherFee.OrderNo=OrderMain.order_number
LEFT JOIN TB_BookingInsuranceOfUser AS Ins WITH(NOLOCK) ON MemberData.MEMIDNO=Ins.IDNO
INNER JOIN TB_CarInfo AS CarInfo WITH(NOLOCK) ON CarInfo.CarNo=OrderMain.CarNo
LEFT JOIN TB_CarTypeGroupConsist AS carType ON carType.CarType=CarInfo.CarType
LEFT JOIN TB_CarTypeGroup AS carTypeGP ON carTypeGP.CarTypeGroupID=carType.CarTypeGroupID
LEFT JOIN TB_InsuranceInfo AS InsInfo WITH(NOLOCK) ON InsInfo.InsuranceLevel=ISNULL(Ins.InsuranceLevel, '3') AND InsInfo.CarTypeGroupCode=carTypeGP.CarTypeGroupCode
GO
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台訂單明細取得完整資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';
GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';
GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'0
            TopColumn = 0
         End
         Begin Table = "MotorData"
            Begin Extent = 
               Top = 1092
               Left = 48
               Bottom = 1250
               Right = 239
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MotorChangeBatteryReward"
            Begin Extent = 
               Top = 1253
               Left = 48
               Bottom = 1411
               Right = 244
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OrderOtherFee"
            Begin Extent = 
               Top = 1414
               Left = 48
               Bottom = 1572
               Right = 334
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Ins"
            Begin Extent = 
               Top = 1575
               Left = 48
               Bottom = 1733
               Right = 271
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "CarInfo"
            Begin Extent = 
               Top = 1736
               Left = 48
               Bottom = 1894
               Right = 315
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "carType"
            Begin Extent = 
               Top = 1897
               Left = 48
               Bottom = 2055
               Right = 259
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "carTypeGP"
            Begin Extent = 
               Top = 2058
               Left = 48
               Bottom = 2216
               Right = 281
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InsInfo"
            Begin Extent = 
               Top = 2219
               Left = 48
               Bottom = 2377
               Right = 281
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
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
               Top = 7
               Left = 48
               Bottom = 165
               Right = 284
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Detail"
            Begin Extent = 
               Top = 168
               Left = 48
               Bottom = 326
               Right = 307
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MemberData"
            Begin Extent = 
               Top = 329
               Left = 48
               Bottom = 487
               Right = 270
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AreaZip"
            Begin Extent = 
               Top = 490
               Left = 48
               Bottom = 648
               Right = 239
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "City"
            Begin Extent = 
               Top = 651
               Left = 48
               Bottom = 767
               Right = 239
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "CarDetail"
            Begin Extent = 
               Top = 770
               Left = 48
               Bottom = 928
               Right = 315
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Project"
            Begin Extent = 
               Top = 931
               Left = 48
               Bottom = 1089
               Right = 259
            End
            DisplayFlags = 28', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';

