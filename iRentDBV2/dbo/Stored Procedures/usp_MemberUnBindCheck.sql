/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_MemberUnBindCheck
* 系    統 : IRENT
* 程式功能 : 會員解綁檢查
* 作    者 : YEH
* 撰寫日期 : 20211203
* 修改日期 : 
* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_MemberUnBindCheck]
	@IDNO                   VARCHAR(10)           ,	--帳號
	@Token                  VARCHAR(1024)         ,	--Token
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

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_MemberUnBindCheck';
SET @IsSystem=0;
SET @ErrorType=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @hasData=0;
SET @IDNO=ISNULL(@IDNO,'');
SET @Token=ISNULL(@Token,'');

BEGIN TRY
	IF @Token='' OR @IDNO=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900';
	END
		 
	-- 再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token with(nolock) WHERE Access_Token=@Token AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token with(nolock) WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END

	-- 判斷訂單狀態
	IF @Error=0
	BEGIN
		SET @hasData=0;
		SELECT @hasData=COUNT(order_number) FROM TB_OrderMain WITH(NOLOCK) WHERE IDNO=@IDNO AND (car_mgt_status>=4 AND car_mgt_status<16);
		IF @hasData>0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR986';	-- 會員尚有未還車訂單時不可解綁
		END
	END
	IF @Error=0
	BEGIN
		SET @hasData=0;
		SELECT @hasData=COUNT(order_number) FROM TB_OrderMain WITH(NOLOCK) WHERE IDNO=@IDNO AND car_mgt_status=0 AND cancel_status=0 AND booking_status=0;
		IF @hasData>0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR987';	-- 會員尚有預約訂單時不可解綁
		END
	END
	IF @Error=0
	BEGIN
		SET @hasData=0;
		SELECT @hasData=COUNT(A.order_number) FROM TB_OrderMain A WITH(NOLOCK)
		INNER JOIN TB_OrderDetail B WITH(NOLOCK) ON B.order_number=A.order_number
		WHERE A.IDNO=@IDNO
		AND A.car_mgt_status=16 AND A.booking_status=5 AND A.cancel_status=0
		AND DATEADD(MONTH,3,B.final_stop_time) >= DATEADD(HOUR,8,GETDATE());
		
		IF @hasData>0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR990';	-- 訂單完成三個月內不可解綁
		END
	END
	IF @Error=0
	BEGIN
		DECLARE @WalletAmout INT = 0;
		SELECT @WalletAmout=WalletBalance FROM TB_UserWallet WITH(NOLOCK) WHERE IDNO=@IDNO;
		IF @WalletAmout > 0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR989';	-- 錢包有餘額
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingCancel';
GO

