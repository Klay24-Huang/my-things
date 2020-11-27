CREATE VIEW [dbo].[VW_BE_GetReturnControl]
	AS 
	 SELECT PROCD,ORDNO,CNTRNO,IRENTORDNO,CUSTID,CUSTNM,BIRTH,CUSTTYPE,ODCUSTID,CARTYPE,CARNO,TSEQNO	  
                         	  ,GIVEDATE,GIVETIME,RENTDAYS,CEILING(GIVEKM) AS GIVEKM,OUTBRNHCD, ISNULL(CONVERT(VARCHAR(8),BookingDetail.final_stop_time, 112),'') AS RNTDATE, ISNULL(REPLACE(CONVERT(VARCHAR(5),BookingDetail.final_stop_time, 8), ':', ''),'') AS RNTTIME
                         	  ,CEILING(BookingDetail.end_mile) AS RNTKM,RPRICE	  
                         	  ,RINSU,DISRATE,OVERHOURS,BookingDetail.fine_price AS OVERAMT2,(BookingDetail.fine_price+BookingDetail.mileage_price) AS RNTAMT,BookingDetail.pure_price AS  RENTAMT,BookingDetail.mileage_price AS LOSSAMT2,PROJID,ISNULL(Trade.TaishinTradeNo,'') AS REMARK
                         	  ,INVKIND,UNIMNO,INVTITLE,INVADDR,BookingDetail.gift_point AS GIFT,BookingDetail.gift_motor_point AS GIFT_MOTO
							  ,ISNULL(Trade.CardNumber,'') AS CARDNO,IIF(ISNULL(Trade.AuthIdResp,0)=0,'',CONVERT(VARCHAR(20),Trade.AuthIdResp)) AS   AUTHCODE
							  ,LendCarControl.[CARRIERID],LendCarControl.[NPOBAN]
							  ,BookingDetail.Insurance_price AS NOCAMT,ISNULL(Trade.AUTHAMT,0) AS PAYAMT
							  ,ISNULL(Machi.Amount,0) AS PARKINGAMT2,ISNULL(BookingDetail.Etag,0) AS eTag
                         FROM TB_lendCarControl AS LendCarControl
                         LEFT JOIN TB_OrderDetail AS BookingDetail ON BookingDetail.order_number=LendCarControl.IRENTORDNO
                         LEFT JOIN TB_Trade AS Trade ON Trade.MerchantTradeNo =BookingDetail.transaction_no AND Trade.CreditType=0 AND IsSuccess=1 AND Trade.OrderNo=BookingDetail.order_number
						 LEFT JOIN TB_OrderParkingFeeByMachi AS Machi ON Machi.OrderNo=BookingDetail.order_number
                         WHERE LendCarControl.IRENTORDNO IN (
						 select a.IRENTORDNO
from TB_lendCarControl a with(NOLOCK)
join TB_OrderHistory b with(NOLOCK) on a.IRENTORDNO = b.OrderNum)

                                                          GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetReturnControl';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetReturnControl';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取得要寫入短租130的資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetReturnControl';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetReturnControl';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetReturnControl';