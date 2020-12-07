CREATE VIEW [dbo].[VW_BE_CleanDataQuery]
	AS 
	SELECT          CleanData.OrderNum, CleanData.UserID, CleanData.outsideClean, CleanData.insideClean, CleanData.rescue, 
                            CleanData.dispatch, CleanData.Anydispatch,CleanData.Maintenance, CleanData.OrderStatus, CleanData.remark,ISNULL(CleanData.incarPic,'') AS incarPicStr, IIF(LEN(CleanData.incarPic)=0,0,1) AS incarPic, 
							CleanData.incarPicType,ISNULL(CleanData.outcarPic,'') AS outcarPicStr,IIF(LEN(CleanData.outcarPic)=0,0,1) AS outcarPic,CleanData.outcarPicType, ISNULL(CleanData.bookingStart, '') AS BookingStart, ISNULL(CleanData.bookingEnd, '') 
                            AS BookingEnd, ISNULL(CleanData.lastCleanTime, '') AS lastCleanTime, CleanData.lastRentTimes, 
                            BookingMain.CarNo AS CarNo, BookingMain.lend_place, BookingMain.start_time, BookingMain.stop_time, 
                            CleanData.MKTime
FROM              dbo.TB_CarCleanLog AS CleanData INNER JOIN
                            dbo.TB_OrderMain AS BookingMain ON BookingMain.order_number = CleanData.OrderNum

                            GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_CleanDataQuery';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_CleanDataQuery';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取出整備人員報表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_CleanDataQuery';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_CleanDataQuery';


GO
