/****************************************************************
** Name: [dbo].[usp_GetOrderDetail]
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
** EXEC @Error=[dbo].[usp_GetOrderDetail]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/11 下午 04:44:22 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/11 下午 04:44:22    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetOrderDetail]
	@IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@Token                  VARCHAR(1024)         ,
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
DECLARE @car_mgt_status TINYINT;
DECLARE @cancel_status TINYINT;
DECLARE @booking_status TINYINT;
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetOrderDetail';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【取消訂單】';

SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token=ISNULL (@Token,'');

BEGIN TRY
	IF @Token='' OR @IDNO=''  OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	--0.再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END

	IF @Error=0
	BEGIN
		SELECT VW.[order_number] AS OrderNo,
			   [OperatorICon] AS Operator,
			   [CarTypeImg] AS CarTypePic,
			   VW.[CarNo] ,
			   [Seat],
			   [CarBrend],
			   [CarTypeName] ,
			   [StationName],
			   [Score] AS OperatorScore,
			   [PRONAME] AS ProjName,
			   VW.[pure_price] ,
			   VW.[gift_point] AS GiftPoint,
			   VW.[gift_motor_point] As GiftMotorPoint,
			   (VW.monthly_workday + VW.monthly_holiday) AS MonthlyHours ,
			   VW.[ProjType],
			   VW.[final_start_time] AS StartTime,
			   VW.[final_stop_time] AS EndTime ,
			   VW.[mileage_price],
			   VW.[Insurance_price],
			   VW.[Etag],
			   VW.[fine_price] ,
			   VW.[final_price] ,
			   VW.[TransDiscount],
			   VW.[parkingFee] ,
			   VW.invoice_date,
			   VW.invoice_price,
			   VW.invoiceCode,
			   VW.bill_option AS InvoiceType ,
			   VW.[CARRIERID],
			   VW.[NPOBAN],
			   VW.unified_business_no,
			   ISNULL(Love.LoveName, '') AS NPOBAN_Name,
			   VW.Millage,
			   VW.Area,
			   VW.start_mile,
			   VW.end_mile,
			   0 AS DiscountAmount,
			   '' As DiscountName
		FROM [dbo].[VW_GetOrderData] AS VW
		LEFT JOIN TB_LoveCode AS Love ON Love.LoveCode=VW.NPOBAN
		WHERE VW.order_number=@OrderNo;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderDetail';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderDetail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂單明細', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderDetail';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderDetail';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetOrderDetail';