/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_JointRentInviteeList_Q01
* 系    統 : IRENT
* 程式功能 : 取得共同承租邀請清單
* 作    者 : Umeko
* 撰寫日期 : 20210825
* 修改日期 : 20210914 UPD BY Umeko REASON: 修改使用中的訂單的回傳清單
Example :
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_JointRentInviteeList_Q01]
	@OrderNo                BIGINT                ,	--訂單編號
	@IDNO                   VARCHAR(10)           ,	--帳號
	@Token                  VARCHAR(1024)         ,	--JWT TOKEN
	@LogID                  BIGINT                ,	--執行的api log
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

SET @FunName='usp_JointRentInviteeList_Q01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @Token=ISNULL (@Token,'');
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);

BEGIN TRY
	IF @Token='' OR @IDNO=''  OR @OrderNo = 0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	--0.再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END

	--IF @Error=0
	--BEGIN
		--Declare @car_mgt_status int = -1,@cancel_status int=-1, @booking_status int=-1
		--Select @car_mgt_status = car_mgt_status,@cancel_status = cancel_status,@booking_status = booking_status
		--From [dbo].[TB_OrderMain] with(nolock)
		--Where order_number  = @OrderNo
	--	
	--	--找不到訂單
	--	IF @car_mgt_status = -1
	--	Begin
	--			SET @Error=1;
	--			SET @ErrorCode='ERR101';
	--	End

	--	IF @Error=0
	--	Begin	
	--		--訂單已取消
	--		IF @cancel_status = 3
	--		Begin
	--				SET @Error=1;
	--				SET @ErrorCode='ERR101';
	--		End
	--	End
	--END

	--輸出訂單資訊
	IF @Error=0
	BEGIN
		Declare @car_mgt_status int = -1,@cancel_status int=-1, @booking_status int=-1
		
		Select @car_mgt_status = car_mgt_status,@cancel_status = cancel_status,@booking_status = booking_status
		From [dbo].[TB_OrderMain] with(nolock)
		Where order_number  = @OrderNo

		--已取車的訂單
		IF @car_mgt_status >= 4 And @car_mgt_status < 16 And @booking_status < 5
		Begin
				Select APPUSEID,MEMIDNO,MEMCNAME,ChkType  
				From TB_TogetherPassenger with(nolock)
				Where [Order_number] = @OrderNo And ChkType = 'Y'
		End
		Else
		Begin
			Select APPUSEID,MEMIDNO,MEMCNAME,ChkType  
			From TB_TogetherPassenger with(nolock)
			Where [Order_number] = @OrderNo
		End
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_JointRentInviteeList_Q01';
END