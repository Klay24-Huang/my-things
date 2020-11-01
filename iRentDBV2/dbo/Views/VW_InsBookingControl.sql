CREATE VIEW [dbo].[VW_InsBookingControl]
	AS 
	       SELECT          OrderMain.order_number, OrderMain.IDNO AS ODCUSTID,MemberData.MEMCNAME AS ODCUSTNM, 
                             MemberData.MEMTEL AS TEL1,MemberData.MEMTEL AS TEL2, MemberData.MEMCOMTEL AS TEL3, 
                            CONVERT(VARCHAR(8), OrderMain.booking_date, 112) AS ODDATE, CONVERT(VARCHAR(8), 
                            OrderMain.start_time, 112) AS GIVEDATE, LEFT(REPLACE(CONVERT(VARCHAR(8), 
                            OrderMain.start_time, 108), ':', ''),4) AS GIVETIME, CONVERT(VARCHAR(8), 
                            OrderMain.stop_time, 112) AS RNTDATE, LEFT(REPLACE(CONVERT(VARCHAR(8), 
                            OrderMain.stop_time, 108), ':', ''),4) AS RNTTIME, CarInfo.CarType, 
                            OrderMain.CarNo AS CARNO, OrderMain.lend_place AS OUTBRNH, 
                            OrderMain.return_place AS INBRNH, OrderMain.init_price AS ORDAMT, 
                            CarInfo.HoildayPrice, CarInfo.WeekdayPrice, 
                            OrderMain.ProjID AS PROJTYPE, OrderMain.bill_option AS INVKIND, 
                            OrderMain.title AS INVTITLE, OrderMain.unified_business_no AS UNIMNO, 
                            CarInfo.TSEQNO,OrderMain.CARRIERID,OrderMain.NPOBAN,OrderMain.InsurancePurePrice AS NOCAMT
                            ,DATEDIFF(day, OrderMain.start_time, OrderMain.stop_time) AS RENTDAY
FROM              dbo.TB_OrderMain AS OrderMain LEFT OUTER JOIN
                            dbo.TB_CarInfo AS CarInfo ON 
                            OrderMain.CarNo = CarInfo.CarNo LEFT OUTER JOIN
                            dbo.TB_MemberData AS MemberData ON MemberData.MEMIDNO = OrderMain.IDNO

                              GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_InsBookingControl';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_InsBookingControl';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寫入060', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_InsBookingControl';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_InsBookingControl';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_InsBookingControl';
