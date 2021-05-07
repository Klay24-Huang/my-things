CREATE VIEW [dbo].[VW_BE_GetReturnControl]
	AS 
SELECT PROCD,
       ORDNO,
       CNTRNO,
       IRENTORDNO,
       CUSTID,
       CUSTNM,
       BIRTH,
       CUSTTYPE,
       ODCUSTID,
       CARTYPE,
       LendCarControl.CARNO,
       TSEQNO ,
       GIVEDATE,
       GIVETIME,
       RENTDAYS,
       CEILING(GIVEKM) AS GIVEKM,
       OUTBRNHCD,
       ISNULL(CONVERT(VARCHAR(8), BookingDetail.final_stop_time, 112), '') AS RNTDATE,
       ISNULL(REPLACE(CONVERT(VARCHAR(5), BookingDetail.final_stop_time, 8), ':', ''), '') AS RNTTIME ,
       CEILING(BookingDetail.end_mile) AS RNTKM,
       RPRICE ,
       RINSU,
       DISRATE,
       OVERHOURS,
       BookingDetail.fine_price AS OVERAMT2,
       (BookingDetail.fine_price+BookingDetail.mileage_price) AS RNTAMT,
	   --20210223;RENTAMT=pure_price-TransDiscount
       CASE WHEN (BookingDetail.pure_price - BookingDetail.TransDiscount) > 0 THEN (BookingDetail.pure_price - BookingDetail.TransDiscount) ELSE 0 END AS RENTAMT,
       BookingDetail.mileage_price AS LOSSAMT2,
       LendCarControl.PROJID ,
       ISNULL(Trade.TaishinTradeNo, '') AS REMARK, --20201209 ADD BY ADAM REASON.財務又說要改回來
	--,ISNULL(Trade.MerchantTradeNo,'') AS REMARK       --20201130 ADD BY ADAM REASON.網刷編號調整為MerchantTradeNo方便對帳
	--,INVKIND,UNIMNO,INVTITLE,INVADDR,BookingDetail.gift_point AS GIFT,BookingDetail.gift_motor_point AS GIFT_MOTO
       INVKIND=M.bill_option ,
       UNIMNO=M.unified_business_no ,
       INVTITLE ,
       INVADDR=M.invoiceAddress ,
       BookingDetail.gift_point AS GIFT,
       BookingDetail.gift_motor_point AS GIFT_MOTO ,
       ISNULL(Trade.CardNumber, '') AS CARDNO,
       IIF(ISNULL(Trade.AuthIdResp, 0)=0, '', CONVERT(VARCHAR(20), Trade.AuthIdResp)) AS AUTHCODE ,
       M.[CARRIERID] ,
       M.[NPOBAN] ,
       BookingDetail.Insurance_price AS NOCAMT ,
	   --,ISNULL(Trade.AUTHAMT,0)-ISNULL(BookingDetail.Etag,0) AS PAYAMT
	   --20210127 ADD BY ADAM REASON.調整轉NPR130時，先以排程取款的資料為主
       CASE WHEN OA.order_number IS NULL THEN ISNULL(Trade.AUTHAMT, 0)-ISNULL(BookingDetail.Etag, 0) ELSE ISNULL(OA.final_price, 0)-ISNULL(BookingDetail.Etag, 0) END AS PAYAMT ,
       ISNULL(BookingDetail.parkingFee, 0) AS PARKINGAMT2,	--20210506;UPD BY YEH REASON.PARKINGAMT2改抓OrderDetail的parkingFee
       ISNULL(BookingDetail.Etag, 0) AS eTag
FROM TB_lendCarControl AS LendCarControl WITH(NOLOCK)
JOIN TB_OrderMain M WITH(NOLOCK) ON LendCarControl.IRENTORDNO=M.order_number
LEFT JOIN TB_OrderDetail AS BookingDetail WITH(NOLOCK) ON BookingDetail.order_number=LendCarControl.IRENTORDNO
--LEFT JOIN TB_Trade AS Trade WITH(NOLOCK) ON Trade.MerchantTradeNo =BookingDetail.transaction_no AND Trade.CreditType=0 AND IsSuccess=1 AND Trade.OrderNo=BookingDetail.order_number
--20201230 ADD BY ADAM REASON.因應台新交易編號有時會串不到，先把該條件先移除，等找到原因後再加回去
LEFT JOIN TB_Trade AS Trade WITH(NOLOCK) ON Trade.CreditType=0 AND IsSuccess=1 AND Trade.OrderNo=BookingDetail.order_number
LEFT JOIN TB_OrderParkingFeeByMachi AS Machi WITH(NOLOCK) ON Machi.OrderNo=BookingDetail.order_number
--20210127 ADD BY ADAM REASON.調整轉NPR130時，先以排程取款的資料為主
LEFT JOIN TB_OrderAuth OA WITH(NOLOCK) ON M.order_number=OA.order_number AND OA.AuthCode='1000'
WHERE LendCarControl.IRENTORDNO IN (SELECT a.IRENTORDNO FROM TB_lendCarControl a with(NOLOCK) JOIN TB_OrderHistory b with(NOLOCK) ON a.IRENTORDNO = b.OrderNum)

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