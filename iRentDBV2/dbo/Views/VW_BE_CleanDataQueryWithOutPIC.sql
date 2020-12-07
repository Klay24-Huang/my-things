CREATE VIEW [dbo].[VW_BE_CleanDataQueryWithOutPIC]
	AS 
	SELECT          CleanData.OrderNum, Manager.Account, ISNULL(CleanData.UserID, N'') AS UserID, CleanData.outsideClean, 
                            CleanData.insideClean, CleanData.rescue, CleanData.dispatch, CleanData.Anydispatch, CleanData.OrderStatus, 
                            CleanData.remark, ISNULL(CleanData.bookingStart, N'') AS BookingStart, ISNULL(CleanData.bookingEnd, N'') 
                            AS BookingEnd, ISNULL(CleanData.lastCleanTime, N'') AS lastCleanTime, CleanData.lastRentTimes,CleanData.Maintenance,
                            BookingMain.CarNo AS CarNo, BookingMain.lend_place, BookingMain.start_time, BookingMain.stop_time, 
                            CleanData.MKTime
FROM              dbo.TB_CarCleanLog AS CleanData WITH (NOLOCK) INNER JOIN
                            dbo.TB_OrderMain AS BookingMain WITH (NOLOCK) ON 
                            BookingMain.order_number = CleanData.OrderNum LEFT OUTER JOIN
                            dbo.TB_Manager AS Manager WITH (NOLOCK) ON Manager.UserName = CleanData.UserID
                            GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_CleanDataQueryWithOutPIC';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_CleanDataQueryWithOutPIC';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取出整備人員匯出報表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_CleanDataQueryWithOutPIC';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_CleanDataQueryWithOutPIC';


GO