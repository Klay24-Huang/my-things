/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_SendReturnCarControl_I01
* 系    統 : IRENT
* 程式功能 : 合約傳送
* 作    者 : Umeko
* 撰寫日期 : 20211108
* 修改日期 : 

* Example  : 
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_SendReturnCarControl_I01]
	@OrderNo				BIGINT,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息          
AS
BEGIN
	DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;

DECLARE @NowTime DATETIME;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_SendReturnCarControl_I01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=dbo.GET_TWDATE();
BEGIN TRY
IF NOT EXISTS(SELECT IRENTORDNO FROM TB_ReturnCarControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo)
BEGIN

			Declare @OrderAmount int = 0
			
			SELECT @OrderAmount =SUM(CloseAmout) 
			FROM TB_TradeClose WITH(READPAST) 
			WHERE OrderNo=@OrderNo AND ChkClose=1 GROUP BY OrderNo

			INSERT INTO TB_ReturnCarControl
			(
				[PROCD], [ORDNO], [CNTRNO], [IRENTORDNO], [CUSTID], [CUSTNM], [BIRTH], 
				[CUSTTYPE], [ODCUSTID], [CARTYPE], [CARNO], [TSEQNO], [GIVEDATE], 
				[GIVETIME], [RENTDAYS], [GIVEKM], [OUTBRNHCD], [RNTDATE], [RNTTIME], 
				[RNTKM], [INBRNHCD], [RPRICE], [RINSU], [DISRATE], [OVERHOURS], 
				[OVERAMT2], [RNTAMT], [RENTAMT], [LOSSAMT2], [PROJID], [REMARK], 
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
				mileage_price,A.ProjID,C.REMARK,	--20201229 租金要扣掉轉乘優惠
				A.bill_option,A.unified_business_no,'',A.invoiceAddress,B.gift_point,B.gift_motor_point,
				CARDNO='',PAYAMT=@OrderAmount,AUTHCODE='',isRetry=1,RetryTimes=0,B.Etag,
				C.CARRIERID,C.NPOBAN,B.Insurance_price,ISNULL(B.parkingFee,0) AS PARKINGAMT2,@NowTime,@NowTime
			FROM TB_OrderMain A WITH(NOLOCK)
			JOIN TB_OrderDetail B WITH(NOLOCK) ON A.order_number=B.order_number
			JOIN TB_lendCarControl C WITH(NOLOCK) ON A.order_number=C.IRENTORDNO
			WHERE A.order_number=@OrderNo;

			-- 20210707;ADD BY YEH REASON:計算徽章成就
			EXEC [usp_CalOrderMedal] @OrderNo,'A',@FunName,123456,'','','','';

			-- 20210707 ADD BY YEH REASON:計算會員積分
			EXEC [usp_CalOrderScore] @OrderNo,'B',0,0,@FunName,123456,'','','','';
End
	--寫入錯誤訊息
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,0,@IsSystem);
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
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,0,@IsSystem);
END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SendReturnCarControl_I01';

END