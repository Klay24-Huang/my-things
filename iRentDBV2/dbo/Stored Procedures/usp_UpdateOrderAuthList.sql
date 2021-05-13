/****************************************************************
** Name: [dbo].[usp_GetOrderAuthList]
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
** EXEC @Error=[dbo].[usp_GetOrderStatusByOrderNo]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/5 下午 01:29:01 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/5 下午 01:29:01    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_UpdateOrderAuthList]
	@authSeq				BIGINT,   
	@AuthFlg				INT,     
	@AuthCode				VARCHAR(50), 
	@AuthMessage			NVARCHAR(120),        
	@OrderNo				BIGINT,          
	@transaction_no			VARCHAR(50),      
	@Reward					INT				OUTPUT,	--換電獎勵
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
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_UpdateOrderAuthList';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
		UPDATE TB_OrderAuth
		SET AuthFlg=@AuthFlg,
			AuthCode=@AuthCode,
			AuthMessage=@AuthMessage,
			transaction_no=@transaction_no,
			U_USERID='UpdateOrderAuthList',
			U_PRGID='UpdateOrderAuthList',
			U_SYSDT=@NowTime
		WHERE authSeq=@authSeq
		 
		--授權成功準備傳送合約
		IF @AuthFlg=1
		BEGIN
			IF NOT EXISTS(SELECT IRENTORDNO FROM TB_ReturnCarControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo)
			BEGIN
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
						CARDNO=ISNULL(Trade.CardNumber,''),PAYAMT=ISNULL(Trade.AUTHAMT,0),AUTHCODE=IIF(ISNULL(Trade.AuthIdResp,0)=0,'',CONVERT(VARCHAR(20),Trade.AuthIdResp)),isRetry=1,RetryTimes=0,B.Etag,
						C.CARRIERID,C.NPOBAN,B.Insurance_price,ISNULL(B.parkingFee,0) AS PARKINGAMT2,@NowTime,@NowTime
					FROM TB_OrderMain A WITH(NOLOCK)
					JOIN TB_OrderDetail B WITH(NOLOCK) ON A.order_number=B.order_number
					JOIN TB_lendCarControl C WITH(NOLOCK) ON A.order_number=C.IRENTORDNO
					LEFT JOIN TB_Trade AS Trade WITH(NOLOCK) ON Trade.MerchantTradeNo =B.transaction_no AND Trade.CreditType=0 AND IsSuccess=1 AND Trade.OrderNo=B.order_number
					WHERE A.order_number=@OrderNo
			END

			--取出換電獎勵
			SELECT @Reward=Reward FROM TB_OrderDataByMotor WITH(NOLOCK) WHERE OrderNo=@OrderNo;
		END

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderStatusByOrderNo';
GO

