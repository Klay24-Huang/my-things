/****** Object:  StoredProcedure [dbo].[usp_BE_GetReturnCarControl]    Script Date: 2021/2/25 上午 09:39:48 ******/

/****************************************************************
** Name: [dbo].[usp_BE_GetReturnCarControl]
** Desc: 
**
** Return values: 0 成功 else 錯誤
** Return Recordset: 
**
** Called by: 
**
** Parameters:
** Input
** -----------

** 
**
** Output
** -----------
		
	@ErrorCode 				VARCHAR(6)			
	@ErrorCodeDesc			NVARCHAR(100)	
	@SQLExceptionCode		VARCHAR(10)				
	@SqlExceptionMsg		NVARCHAR(1000)	
**
** 
** Example
**------------
** DECLARE @Error               INT;
** DECLARE @ErrorCode 			VARCHAR(6);		
** DECLARE @ErrorMsg  			NVARCHAR(100);
** DECLARE @SQLExceptionCode	VARCHAR(10);		
** DECLARE @SQLExceptionMsg		NVARCHAR(1000);
** EXEC @Error=[dbo].[usp_BE_GetReturnCarControl]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:
** Date:
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_GetReturnCarControl]
	@OrderNo                BIGINT                ,
	@UserID                 NVARCHAR(10)          ,
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_BE_GetReturnCarControl';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @OrderNo=ISNULL (@OrderNo,0);
SET @UserID=ISNULL (@UserID,'');

BEGIN TRY
	IF @UserID='' OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		-- ReturnCarControl有資料就送短租，沒資料再從LendCarControl取值，以利邏輯一致
		SELECT @hasData=COUNT(*) FROM TB_ReturnCarControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo;
		IF @hasData > 0
		BEGIN
			SELECT TB_ReturnCarControl.[PROCD],
				TB_ReturnCarControl.[ORDNO],
				TB_ReturnCarControl.[IRENTORDNO],
				TB_ReturnCarControl.[CUSTID],
				TB_ReturnCarControl.[CUSTNM],
				TB_ReturnCarControl.[BIRTH],
				TB_ReturnCarControl.[CUSTTYPE],
				TB_ReturnCarControl.[ODCUSTID],
				TB_ReturnCarControl.[CARTYPE],
				TB_ReturnCarControl.[CARNO],
				TB_ReturnCarControl.[TSEQNO],
				TB_ReturnCarControl.[GIVEDATE],
				TB_ReturnCarControl.[GIVETIME],
				TB_ReturnCarControl.[RENTDAYS],
				TB_ReturnCarControl.[GIVEKM],
				TB_ReturnCarControl.[OUTBRNHCD],
				TB_ReturnCarControl.[RNTDATE],
				TB_ReturnCarControl.[RNTTIME],
				TB_ReturnCarControl.[RNTKM],
				TB_ReturnCarControl.[RPRICE],
				TB_ReturnCarControl.[RINSU],
				TB_ReturnCarControl.[DISRATE],
				TB_ReturnCarControl.[OVERHOURS],
				TB_ReturnCarControl.[OVERAMT2],
				TB_ReturnCarControl.[RNTAMT],
				TB_ReturnCarControl.[RENTAMT],
				TB_ReturnCarControl.[LOSSAMT2],
				TB_ReturnCarControl.[PROJID],
				ISNULL(Trade.TaishinTradeNo, '') AS REMARK,
				TB_ReturnCarControl.[INVKIND],
				TB_ReturnCarControl.[UNIMNO],
				TB_ReturnCarControl.[INVTITLE],
				TB_ReturnCarControl.[INVADDR],
				TB_ReturnCarControl.[GIFT],
				TB_ReturnCarControl.[GIFT_MOTO],
				ISNULL(Trade.CardNumber, '') AS CARDNO,
				IIF(ISNULL(Trade.AuthIdResp, 0)=0, '', CONVERT(VARCHAR(20), Trade.AuthIdResp)) AS AUTHCODE,
				--20210127 ADD BY ADAM REASON.調整轉NPR130時，先以排程取款的資料為主
				CASE WHEN OrderAuth.order_number IS NULL THEN ISNULL(Trade.AUTHAMT, 0)-ISNULL(OrderDetail.Etag, 0) ELSE ISNULL(OrderAuth.final_price, 0)-ISNULL(OrderDetail.Etag, 0) END AS PAYAMT,
				TB_ReturnCarControl.[CARRIERID],
				TB_ReturnCarControl.[NPOBAN],
				ISNULL(Machi.Amount, 0) AS PARKINGAMT2,
				TB_ReturnCarControl.[NOCAMT],
				ISNULL(OrderDetail.Etag, 0) AS eTag
			FROM TB_ReturnCarControl WITH(NOLOCK)
			JOIN TB_OrderMain AS OrderMain WITH(NOLOCK) ON TB_ReturnCarControl.IRENTORDNO=OrderMain.order_number
			LEFT JOIN TB_OrderDetail AS OrderDetail WITH(NOLOCK) ON OrderDetail.order_number=TB_ReturnCarControl.IRENTORDNO
			LEFT JOIN TB_Trade AS Trade WITH(NOLOCK) ON Trade.CreditType=0 AND IsSuccess=1 AND Trade.OrderNo=OrderDetail.order_number
			LEFT JOIN TB_OrderParkingFeeByMachi AS Machi WITH(NOLOCK) ON Machi.OrderNo=OrderDetail.order_number
			LEFT JOIN TB_OrderAuth AS OrderAuth WITH(NOLOCK) ON OrderMain.order_number=OrderAuth.order_number AND OrderAuth.AuthCode='1000'
			WHERE TB_ReturnCarControl.IRENTORDNO=@OrderNo;
		END
		ELSE
		BEGIN
			SELECT LendCarControl.PROCD,
				LendCarControl.ORDNO,
				LendCarControl.IRENTORDNO,
				LendCarControl.CUSTID,
				LendCarControl.CUSTNM,
				LendCarControl.BIRTH,
				LendCarControl.CUSTTYPE,
				LendCarControl.ODCUSTID,
				LendCarControl.CARTYPE,
				LendCarControl.CARNO,
				LendCarControl.TSEQNO,
				LendCarControl.GIVEDATE,
				LendCarControl.GIVETIME,
				LendCarControl.RENTDAYS,
				CEILING(LendCarControl.GIVEKM) AS GIVEKM,
				LendCarControl.OUTBRNHCD,
				ISNULL(CONVERT(VARCHAR(8), BookingDetail.final_stop_time, 112), '') AS RNTDATE,
				ISNULL(REPLACE(CONVERT(VARCHAR(5), BookingDetail.final_stop_time, 8), ':', ''), '') AS RNTTIME,
				CEILING(BookingDetail.end_mile) AS RNTKM,
				LendCarControl.RPRICE,
				LendCarControl.RINSU,
				LendCarControl.DISRATE,
				LendCarControl.OVERHOURS,
				BookingDetail.fine_price AS OVERAMT2,
				(BookingDetail.fine_price+BookingDetail.mileage_price) AS RNTAMT,
				--20210223;RENTAMT=pure_price-TransDiscount
				CASE WHEN (BookingDetail.pure_price - BookingDetail.TransDiscount) > 0 THEN (BookingDetail.pure_price - BookingDetail.TransDiscount) ELSE 0 END AS RENTAMT,
				BookingDetail.mileage_price AS LOSSAMT2,
				LendCarControl.PROJID ,
				ISNULL(Trade.TaishinTradeNo, '') AS REMARK, --20201209 ADD BY ADAM REASON.財務又說要改回來
				INVKIND=OrderMain.bill_option ,
				UNIMNO=OrderMain.unified_business_no ,
				LendCarControl.INVTITLE,
				INVADDR=OrderMain.invoiceAddress ,
				BookingDetail.gift_point AS GIFT,
				BookingDetail.gift_motor_point AS GIFT_MOTO ,
				ISNULL(Trade.CardNumber, '') AS CARDNO,
				IIF(ISNULL(Trade.AuthIdResp, 0)=0, '', CONVERT(VARCHAR(20), Trade.AuthIdResp)) AS AUTHCODE,
				--20210127 ADD BY ADAM REASON.調整轉NPR130時，先以排程取款的資料為主
				CASE WHEN OA.order_number IS NULL THEN ISNULL(Trade.AUTHAMT, 0)-ISNULL(BookingDetail.Etag, 0) ELSE ISNULL(OA.final_price, 0)-ISNULL(BookingDetail.Etag, 0) END AS PAYAMT,
				OrderMain.[CARRIERID],
				OrderMain.[NPOBAN],
				ISNULL(Machi.Amount, 0) AS PARKINGAMT2,
				BookingDetail.Insurance_price AS NOCAMT,
				ISNULL(BookingDetail.Etag, 0) AS eTag
			FROM TB_lendCarControl AS LendCarControl WITH(NOLOCK)
			JOIN TB_OrderMain OrderMain WITH(NOLOCK) ON LendCarControl.IRENTORDNO=OrderMain.order_number
			LEFT JOIN TB_OrderDetail AS BookingDetail WITH(NOLOCK) ON BookingDetail.order_number=LendCarControl.IRENTORDNO
			--20201230 ADD BY ADAM REASON.因應台新交易編號有時會串不到，先把該條件先移除，等找到原因後再加回去
			LEFT JOIN TB_Trade AS Trade WITH(NOLOCK) ON Trade.CreditType=0 AND IsSuccess=1 AND Trade.OrderNo=BookingDetail.order_number
			LEFT JOIN TB_OrderParkingFeeByMachi AS Machi WITH(NOLOCK) ON Machi.OrderNo=BookingDetail.order_number
			--20210127 ADD BY ADAM REASON.調整轉NPR130時，先以排程取款的資料為主
			LEFT JOIN TB_OrderAuth OA WITH(NOLOCK) ON OrderMain.order_number=OA.order_number AND OA.AuthCode='1000'
			WHERE OrderMain.order_number=@OrderNo
		END
	END

	--寫入錯誤訊息
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END
END TRY
BEGIN CATCH
	SET @Error=-1;
	SET @ErrorCode='ERR999';
	SET @ErrorMsg='我要寫錯誤訊息';
	SET @SQLExceptionCode=ERROR_NUMBER();
	SET @SQLExceptionMsg=ERROR_MESSAGE();
	IF @@TRANCOUNT > 0
	BEGIN
		print 'rolling back transaction' /* <- this is never printed */
		ROLLBACK TRAN
	END
	SET @IsSystem=1;
	SET @ErrorType=4;
	INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetReturnCarControl';
GO

