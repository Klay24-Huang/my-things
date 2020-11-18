CREATE VIEW [dbo].[VW_GetOrderData]
	AS

SELECT OrderMain.[order_number]
      ,[IDNO]
      ,OrderMain.[CarNo]
      ,OrderMain.[ProjID]
      ,[lend_place]
      ,[return_place]
      ,[start_time]
      ,[stop_time]
      ,[stop_pick_time]
      ,[fine_Time]
      ,[init_price]
	  ,[init_TransDiscount]
      ,[Insurance]
      ,[InsurancePurePrice]
      ,[car_mgt_status]
      ,[booking_status]
      ,[cancel_status]
      ,[modified_status]
      ,OrderMain.[ProjType]
      ,OrderMain.[PayMode]
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
	  ,ISNULL(PriceByMinutes.BaseMinutes	 ,0) AS BaseMinutes
	  ,ISNULL(PriceByMinutes.BaseMinutesPrice,0) AS BaseMinutesPrice
	  ,ISNULL(PriceByMinutes.Price			 ,0.0) AS MinuteOfPrice
	  ,ISNULL(PriceByMinutes.MaxPrice		 ,0) AS MaxPrice
	  ,ISNULL(PriceByMinutes.MaxPriceH		 ,0) AS MaxPriceH --20201006 - eason
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
  FROM [dbo].[TB_OrderMain] AS OrderMain  WITH(NOLOCK)
  LEFT JOIN [dbo].[TB_OrderDetail] AS OrderDetil WITH(NOLOCK) ON OrderDetil.order_number=OrderMain.order_number 
  LEFT JOIN [dbo].[TB_Car] AS Car WITH(NOLOCK) ON Car.CarNo=OrderMain.CarNo
  LEFT JOIN [dbo].[TB_CarInfo] CarInfo WITH(NOLOCK) ON CarInfo.CarNo = OrderMain.CarNo --20201006 - eason  
  LEFT JOIN [dbo].[TB_OperatorBase] AS Operator  WITH(NOLOCK)  ON Operator.OperatorID=Car.Operator
  LEFT JOIN [dbo].[VW_GetFullProjectCollectionOfCarTypeGroup] AS VW  WITH(NOLOCK) ON VW.CARTYPE=Car.CarType AND VW.StationID=OrderMain.lend_place AND VW.PROJID=OrderMain.ProjID AND VW.IOType='O'
  LEFT JOIN [dbo].[TB_PriceByMinutes] AS PriceByMinutes WITH(NOLOCK) ON PriceByMinutes.CarType=Car.CarType AND PriceByMinutes.ProjID=VW.PROJID
  LEFT JOIN [dbo].[TB_CarStatus] AS CarStatus WITH(NOLOCK) ON CarStatus.CarNo=OrderMain.CarNo
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
