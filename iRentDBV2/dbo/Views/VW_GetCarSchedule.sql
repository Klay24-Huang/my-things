CREATE VIEW [dbo].[VW_GetCarSchedule]
	AS 
	SELECT          Main.order_number AS OrderNum, Main.IDNO AS IDNO, Main.CarNo AS CarNo, 
                            ISNULL(UserInfo.MEMCNAME, Main.IDNO) AS UName, ISNULL(UserInfo.MEMTEL, '') AS Mobile, 
                            Main.car_mgt_status, Main.booking_status, Main.cancel_status, Main.start_time AS SD, Main.stop_time AS ED, 
                            ISNULL(Detail.final_start_time, '1911-01-01 00:00:00') AS FS, ISNULL(Detail.final_stop_time, '1911-01-01 00:00:00') 
                            AS FE,lend_place AS StationID
FROM              dbo.TB_OrderMain AS Main LEFT OUTER JOIN
                            dbo.TB_OrderDetail AS Detail ON Detail.order_number = Main.order_number LEFT OUTER JOIN
                            dbo.TB_MemberData AS UserInfo ON UserInfo.MEMIDNO = Main.IDNO
GO
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarSchedule';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarSchedule';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車輛timeline', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarSchedule';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarSchedule';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarSchedule';