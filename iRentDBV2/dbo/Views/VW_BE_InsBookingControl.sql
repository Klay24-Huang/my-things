CREATE VIEW [dbo].[VW_BE_InsBookingControl]
	AS 
	  SELECT          dbo.TB_OrderMain.order_number, dbo.TB_OrderMain.IDNO AS ODCUSTID, 
                            dbo.TB_MemberData.MEMCNAME AS ODCUSTNM, dbo.TB_MemberData.MEMTEL AS TEL1, 
                            dbo.TB_MemberData.MEMCONTEL AS TEL2, dbo.TB_MemberData.MEMCOMTEL AS TEL3, 
                            CONVERT(VARCHAR(8), dbo.TB_OrderMain.booking_date, 112) AS ODDATE, CONVERT(VARCHAR(8), 
                            dbo.TB_OrderMain.start_time, 112) AS GIVEDATE, LEFT(REPLACE(CONVERT(VARCHAR(8), 
                            dbo.TB_OrderMain.start_time, 108), ':', ''),4) AS GIVETIME, CONVERT(VARCHAR(8), 
                            dbo.TB_OrderMain.stop_time, 112) AS RNTDATE, LEFT(REPLACE(CONVERT(VARCHAR(8), 
                            dbo.TB_OrderMain.stop_time, 108), ':', ''),4) AS RNTTIME, dbo.TB_CarInfo.CarType, 
                            dbo.TB_OrderMain.CarNo AS CARNO, dbo.TB_OrderMain.lend_place AS OUTBRNH, 
                            dbo.TB_OrderMain.return_place AS INBRNH, dbo.TB_OrderMain.init_price AS ORDAMT, 
                            dbo.TB_CarInfo.HoildayPrice, dbo.TB_CarInfo.WeekdayPrice, 
                            dbo.TB_OrderMain.ProjID AS PROJTYPE, dbo.TB_OrderMain.bill_option AS INVKIND, 
                            dbo.TB_OrderMain.title AS INVTITLE, dbo.TB_OrderMain.unified_business_no AS UNIMNO, 
                            dbo.TB_CarInfo.TSEQNO,TB_OrderMain.CARRIERID,TB_OrderMain.NPOBAN,TB_OrderMain.InsurancePurePrice AS NOCAMT
                            ,TB_OrderMain.init_price AS Price
FROM              dbo.TB_OrderMain LEFT OUTER JOIN
                            dbo.TB_CarInfo ON 
                            dbo.TB_OrderMain.CarNo = dbo.TB_CarInfo.CarNo LEFT OUTER JOIN
                            dbo.TB_MemberData ON dbo.TB_MemberData.MEMIDNO = dbo.TB_OrderMain.IDNO

                                                          GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_InsBookingControl';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_InsBookingControl';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取得要寫入短租060的資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_InsBookingControl';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_InsBookingControl';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_InsBookingControl';
