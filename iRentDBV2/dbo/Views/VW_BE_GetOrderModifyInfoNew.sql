CREATE VIEW [dbo].[VW_BE_GetOrderModifyInfoNew]
	AS
SELECT Main.[order_number] AS OrderNo
      ,[IDNO]
      ,Main.[CarNo]
      ,Main.[ProjID]
	  ,Project.PRONAME
	  ,Project.PROJTYPE
	  ,IIF(ISNULL(Detail.final_start_time ,'')='','',CONVERT(VARCHAR(20),Detail.final_start_time ,120)) AS FS
	  ,IIF(ISNULL(Detail.final_stop_time ,'')='','',CONVERT(VARCHAR(20),Detail.final_stop_time ,120)) AS FE
	  ,Detail.start_mile AS SM
	  ,Detail.end_mile AS EM
	  ,Detail.mileage_price
	  ,Detail.final_price
	  ,Detail.fine_price
	  ,Detail.pure_price
	  ,Detail.gift_point
	  ,Detail.gift_motor_point
	  ,Detail.parkingFee
	  ,Detail.TransDiscount
	  ,Detail.Insurance_price
      ,Detail.trade_status
	  ,Detail.transaction_no
	  ,CarInfo.HoildayPrice
	  ,CarInfo.HoildayPriceByMinutes
	  ,CarInfo.WeekdayPrice
	  ,CarInfo.WeekdayPriceByMinutes
	  ,ISNULL(PriceMinutes.MaxPrice,0) AS MaxPrice
	  ,ISNULL(PriceMinutes.MaxPriceH,0) AS MaxPriceH
	  ,ISNULL(Trade.[AUTHAMT],0) AS Paid
  FROM [dbo].[TB_OrderMain] AS Main
  INNER JOIN TB_OrderDetail AS Detail ON Main.order_number=Detail.order_number
  INNER JOIN TB_CarInfo AS CarInfo ON CarInfo.CarNo=Main.CarNo
  LEFT JOIN TB_Project AS Project ON Project.PROJID=Main.ProjID 
  LEFT JOIN TB_PriceByMinutes AS PriceMinutes ON PriceMinutes.ProjID=Main.ProjID AND PriceMinutes.CARTYPE=CarInfo.CarType
  LEFT JOIN TB_Trade AS Trade ON Trade.SOrderNum=Main.order_number AND Trade.IsSuccess=1

                                GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderModifyInfoNew';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderModifyInfoNew';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取得訂單修改前資料(New)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderModifyInfoNew';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderModifyInfoNew';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderModifyInfoNew';