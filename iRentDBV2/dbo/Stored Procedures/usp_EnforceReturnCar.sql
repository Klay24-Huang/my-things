/****** Object:  StoredProcedure [dbo].[usp_EnforceReturnCar]    Script Date: 2020/10/29 下午 02:52:29 ******/

/****************************************************************
** Name: [dbo].[usp_EnforceReturnCar]
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
** EXEC @Error=[dbo].[usp_RegisterMemberData]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
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
CREATE PROCEDURE [dbo].[usp_EnforceReturnCar]
	@CarNo					VARCHAR(10)           , --車號
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
DECLARE @order_number INT;
DECLARE @MEMIDNO VARCHAR(10);
DECLARE @ProjType VARCHAR(5);
DECLARE @NowTime	DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_EnforceReturnCar';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @CarNo=ISNULL (@CarNo,'');
SET @order_number=0;
SET @MEMIDNO='';
SET @ProjType='';
SET @NowTime=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
	IF @CarNo=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		SELECT @order_number=NowOrderNo FROM TB_Car WITH(NOLOCK) WHERE CarNo=@CarNo;

		SELECT @MEMIDNO=IDNO,@ProjType=ProjType FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@order_number;

		UPDATE TB_OrderMain SET car_mgt_status=16,booking_status=5 WHERE order_number=@order_number;

		UPDATE TB_OrderDetail SET final_stop_time=@NowTime WHERE order_number=@order_number;

		UPDATE TB_Car SET NowOrderNo=0,LastOrderNo=@order_number,available=1,UPDTime=@NowTime WHERE CarNo=@CarNo;

		UPDATE TB_BookingStatusOfUser 
		SET MotorRentBookingNowCount=CASE WHEN @ProjType=4 THEN 0 ELSE MotorRentBookingNowCount END
			,AnyRentBookingNowCount=CASE WHEN @ProjType=3 THEN 0 ELSE AnyRentBookingNowCount END
			,RentNowActiveType=5
			,NowActiveOrderNum=0
		WHERE IDNO=@MEMIDNO;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_EnforceReturnCar';
GO