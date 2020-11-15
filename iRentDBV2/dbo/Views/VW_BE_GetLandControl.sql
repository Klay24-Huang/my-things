CREATE VIEW [dbo].[VW_BE_GetLandControl]
	AS 
	SELECT          BookingControl.PROCD, BookingControl.ORDNO, BookingControl.order_number AS IRENTORDNO, 
                            BookingControl.ODCUSTID AS CUSTID, BookingControl.ODCUSTNM AS CUSTNM, ISNULL(CONVERT(VARCHAR(8),UserInfo.MEMBIRTH, 112), '')  AS BIRTH, BookingControl.CARTYPE, BookingControl.CARNO, BookingControl.TSEQNO, 
                            ISNULL(CONVERT(VARCHAR(8), BookingDetail.final_start_time, 112), '') AS GIVEDATE, 
                            ISNULL(REPLACE(CONVERT(VARCHAR(5), BookingDetail.final_start_time, 8), ':', ''), '') AS GIVETIME, 
                            BookingControl.RENTDAY AS RENTDAYS, ISNULL(BookingDetail.start_mile, 0) AS GIVEKM, 
                            BookingControl.OUTBRNH AS OUTBRNHCD, BookingControl.RNTDATE, BookingControl.RNTTIME, 
                            BookingControl.INBRNH AS INBRNHCD, BookingControl.RPRICE, 0 AS RINSU, 0 AS OVERHOURS, 0 AS OVERAMT2, 
                            BookingControl.ORDAMT, BookingControl.ORDAMT AS RENTAMT, 0 AS LOSSAMT2, 
                            BookingControl.PROJTYPE AS PROJID, '' AS XID, BookingControl.INVKIND, 
                            BookingControl.UNIMNO, BookingControl.INVTITLE, ISNULL(City.CityName, '') + ISNULL(AREA.AreaName, '') 
                            + ISNULL(BookingMain.invoiceAddress, '') AS INVADDR, ISNULL(BookingDetail.final_start_time, 
                            '1911-01-01 00:00:00') AS FinalStartTime, BookingMain.stop_time AS BookingStopTime, BookingControl.CARRIERID, 
                            BookingControl.NPOBAN,BookingControl.NOCAMT
FROM              dbo.TB_BookingControl AS BookingControl LEFT OUTER JOIN
                            dbo.TB_OrderDetail AS BookingDetail ON 
                            BookingDetail.order_number = BookingControl.order_number LEFT OUTER JOIN
                            dbo.TB_OrderMain AS BookingMain ON 
                            BookingMain.order_number = BookingControl.order_number LEFT OUTER JOIN
                            dbo.TB_MemberData AS UserInfo ON UserInfo.MEMIDNO = BookingControl.ODCUSTID  LEFT OUTER JOIN
                            dbo.TB_City AS City ON City.CityID = BookingMain.City LEFT OUTER JOIN
                            dbo.TB_AreaZip AS AREA ON AREA.ZIPCode = BookingMain.ZipCode
WHERE          (BookingControl.PROCD IN ('A', 'U')) AND (BookingControl.ORDNO <> '')

                                                          GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetLandControl';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetLandControl';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取得要寫入短租125的資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetLandControl';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetLandControl';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetLandControl';