CREATE VIEW [dbo].[VW_SYNC_GetSyncData]
	AS 

	SELECT          ISNULL(BookingMain.order_number, 0) AS OrderNum, BookingMain.IDNO, BookingMain.CarNo, 
                            LoginData.MEMCNAME + CASE substring(LoginData.MEMIDNO,2,1) WHEN '1' THEN N' 先生' WHEN '2' THEN N' 女士' ELSE '' END AS Title, 
                            BookingMain.start_time AS StartTime, BookingMain.stop_time AS StopTime, ISNULL(BookingMain.fine_Time, 
                            N'1911-01-01 00:00:00') AS FineTime, ISNULL(BookingDetail.final_start_time, N'1911-01-01 00:00:00') 
                            AS FinalStartTime, ISNULL(BookingDetail.final_stop_time, N'1911-01-01 00:00:00') AS FinalStopTime, 
                            ISNULL(LoginData.CardNo, N'') AS CardNo, BookingMain.car_mgt_status, BookingMain.booking_status, 
                            BookingMain.cancel_status, CarInfo.CID, ISNULL(LoginData.PushREGID,'') AS DeviceToken
                            , BookingMain.ProjID, ISNULL(Project.PROJTYPE, - 1) 
                            AS PROJTYPE,BookingMain.spec_status
FROM              dbo.TB_OrderMain AS BookingMain LEFT OUTER JOIN
                            dbo.TB_OrderDetail AS BookingDetail ON 
                            BookingMain.order_number = BookingDetail.order_number LEFT OUTER JOIN
                            dbo.TB_Project AS Project ON Project.PROJID = BookingMain.ProjID LEFT OUTER JOIN
                            dbo.TB_MemberData AS LoginData ON LoginData.MEMIDNO = BookingMain.IDNO LEFT OUTER JOIN
                            dbo.TB_CarInfo AS CarInfo ON CarInfo.CarNo = BookingMain.CarNO 

                            GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_SYNC_GetSyncData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_SYNC_GetSyncData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取得排程訂單資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_SYNC_GetSyncData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_SYNC_GetSyncData';


GO
