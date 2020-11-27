CREATE VIEW [dbo].[VW_MA_GetCleanOrderList]
	AS 
	SELECT [OrderNum]
	  ,[OrderStatus]
      ,[UserID]
      ,[outsideClean]
      ,[insideClean]
      ,[rescue]
      ,[dispatch]
      ,[Anydispatch]
	  ,IDNO
      ,BookingMain.CarNo
      ,[start_time]
      ,[stop_time]
	  ,[cancel_status]
	  ,CarMachine.CarMachineID AS MachineID
	  ,CarMachine.MachineNo
	  ,case CarInfo.IsMotor WHEN  0 then 1 when 1 then 0 END AS IsCar
  FROM [dbo].[TB_CarCleanLog] AS CarClean
  LEFT JOIN TB_OrderMain AS BookingMain  WITH(NOLOCK) on CarClean.OrderNum=BookingMain.order_number
  LEFT JOIN [TB_CarInfo] AS CarInfo WITH(NOLOCK) on BookingMain.CarNo=CarInfo.CarNo
  LEFT JOIN [TB_CarType] AS CarType WITH(NOLOCK) ON CarInfo.CarType=CarType.CarType
  LEFT JOIN [TB_CarMachine] AS CarMachine WITH(NOLOCK) ON CarMachine.MachineNo=CarInfo.CID
  GO
    EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetCleanOrderList';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetCleanOrderList';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'整備人員預約清單', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetCleanOrderList';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_MA_GetCleanOrderList';


GO