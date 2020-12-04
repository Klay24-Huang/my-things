CREATE VIEW [dbo].[VW_BE_GetFeedBackByOrderMain]
	AS 
	
							SELECT          FeedBack.FeedBackID, FeedBack.IDNO, FeedBack.mode, FeedBack.descript, FeedBack.PIC1, FeedBack.PIC2, 
                            FeedBack.PIC3, FeedBack.PIC4, FeedBack.handleDescript, FeedBack.opt, FeedBack.isHandle, FeedBack.MKTime, 
                            userInfo.MEMCNAME, userInfo.MEMTEL, FeedBack.OrderNo, OrderMain.CarNo AS CarNo, 
                            OrderMain.lend_place, OrderMain.return_place, Car.StationID, Car.nowStationID, ISNULL(Station.Location, '') 
                            AS StationName
FROM              dbo.TB_FeedBack AS FeedBack INNER JOIN
                            dbo.TB_MemberData AS userInfo ON FeedBack.IDNO = userInfo.MEMIDNO INNER JOIN
                            dbo.TB_OrderMain AS OrderMain ON OrderMain.order_number = FeedBack.OrderNo INNER JOIN
                            dbo.TB_Car AS Car ON Car.CarNo = OrderMain.CarNo LEFT OUTER JOIN
                            dbo.TB_iRentStation AS Station ON Station.StationID = Car.nowStationID
                              GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFeedBackByOrderMain';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFeedBackByOrderMain';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台回饋', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFeedBackByOrderMain';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFeedBackByOrderMain';


GO