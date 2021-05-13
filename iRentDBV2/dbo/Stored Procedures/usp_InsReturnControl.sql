-- =============================================
-- Author:      ADAM
-- Create Date: 2021-01-15
-- Description: 補傳轉短租還車合約
-- =============================================
/* EXAMPLE
EXEC usp_SqlJob_Q02 '','2020-12-18'
*/
CREATE PROCEDURE [dbo].[usp_InsReturnControl]
	 @MSG			VARCHAR(100) OUTPUT
	,@OrderNo			INT
AS

	SET @MSG = ''
	SET NOCOUNT ON

	IF NOT EXISTS(SELECT IRENTORDNO FROM TB_ReturnCarControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo)
	BEGIN
		INSERT INTO TB_ReturnCarControl
		(
			[PROCD], [ORDNO], [CNTRNO], [IRENTORDNO], [CUSTID], [CUSTNM], [BIRTH], 
			[CUSTTYPE], [ODCUSTID], [CARTYPE], [CARNO], [TSEQNO], [GIVEDATE], 
			[GIVETIME], [RENTDAYS], [GIVEKM], [OUTBRNHCD], [RNTDATE], [RNTTIME], 
			[RNTKM], [INBRNHCD], [RPRICE], [RINSU], [DISRATE], [OVERHOURS], 
			[OVERAMT2], [RNTAMT], 
			[RENTAMT], 
			[LOSSAMT2], [PROJID], [REMARK], 
			[INVKIND], [UNIMNO], [INVTITLE], [INVADDR], [GIFT], [GIFT_MOTO], 
			[CARDNO], [PAYAMT], [AUTHCODE], [isRetry], [RetryTimes], [eTag], 
			[CARRIERID], [NPOBAN], [NOCAMT], [PARKINGAMT2], [MKTime], [UPDTime]
		)
		SELECT PROCD='A',C.ORDNO,C.CNTRNO,A.order_number,C.CUSTID,C.CUSTNM, C.BIRTH,
			C.CUSTTYPE,C.ODCUSTID,C.CARTYPE,CASRNO=A.CarNo,C.TSEQNO,C.GIVEDATE,
			C.GIVETIME,dbo.FN_CalRntdays(B.final_start_time,B.final_stop_time),CAST(B.start_mile AS INT),C.OUTBRNHCD,CONVERT(VARCHAR,B.final_stop_time,112),REPLACE(CONVERT(VARCHAR(5),B.final_stop_time,108),':',''),
			CAST(B.end_mile AS INT),C.INBRNHCD,C.RPRICE,C.RINSU,C.DISRATE,B.fine_interval/600,
			fine_price,RNTAMT=(B.fine_price+B.mileage_price),
			RENTAMT=CASE WHEN (pure_price- CASE WHEN TransDiscount>0 THEN TransDiscount ELSE 0 END) > 0 THEN (pure_price- CASE WHEN TransDiscount>0 THEN TransDiscount ELSE 0 END) ELSE 0 END,	--20201229 租金要扣掉轉乘優惠
			mileage_price,A.ProjID,C.REMARK,
			A.bill_option,A.unified_business_no,'',A.invoiceAddress,B.gift_point,B.gift_motor_point,
			CARDNO=ISNULL(Trade.CardNumber,''),PAYAMT=ISNULL(Trade.AUTHAMT,0),AUTHCODE=IIF(ISNULL(Trade.AuthIdResp,0)=0,'',CONVERT(VARCHAR(20),Trade.AuthIdResp)),isRetry=1,RetryTimes=0,B.Etag,
			C.CARRIERID,C.NPOBAN,B.Insurance_price,ISNULL(B.parkingFee,0) AS PARKINGAMT2,dbo.GET_TWDATE(),dbo.GET_TWDATE()
		FROM TB_OrderMain A WITH(NOLOCK)
		JOIN TB_OrderDetail B WITH(NOLOCK) ON A.order_number=B.order_number
		JOIN TB_lendCarControl C WITH(NOLOCK) ON A.order_number=C.IRENTORDNO
		LEFT JOIN TB_Trade AS Trade WITH(NOLOCK) ON Trade.MerchantTradeNo =B.transaction_no AND Trade.CreditType=0 AND IsSuccess=1 AND Trade.OrderNo=B.order_number
		WHERE A.order_number=@OrderNo
	END
GO

