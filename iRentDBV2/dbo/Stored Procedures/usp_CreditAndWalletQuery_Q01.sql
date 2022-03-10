/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_CreditAndWalletQuery_Q01
* 系    統 : IRENT
* 程式功能 : 取得錢包&發票資訊
* 作    者 : YEH
* 撰寫日期 : 20210922
* 修改日期 : 20210923 UPD BY YEH REASON:增加是否同意自動儲值
			 20220221 UPD BY YEH REASON:增加機車預扣款金額

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_CreditAndWalletQuery_Q01]
	@IDNO				VARCHAR(10)				,	-- 帳號
	@Token				VARCHAR(1024)			,	-- ACCESS TOKEN
	@LogID				BIGINT					,	-- LOGID
	@PayMode			INT				OUTPUT	,	-- 付費方式：0:信用卡;1:和雲錢包
	@WalletStatus		VARCHAR(1)		OUTPUT	,	-- 錢包狀態 (1:未啟用 2:啟用 3:凍結 4:註記刪除)
	@WalletAmout		INT				OUTPUT	,	-- 錢包餘額
	@MEMSENDCD			INT				OUTPUT	,	-- 發票寄送方式：1:捐贈;2:email;3:二聯;4:三聯;5:手機條碼;6:自然人憑證
	@UNIMNO				VARCHAR(10)		OUTPUT	,	-- 統編
	@CARRIERID			VARCHAR(20)		OUTPUT	,	-- 手機條碼
	@NPOBAN				VARCHAR(20)		OUTPUT	,	-- 愛心碼
	@AutoStored			INT				OUTPUT	,	-- 是否同意自動儲值(0:不同意，1:同意)
	@MotorPreAmt		INT				OUTPUT	,	-- 機車預扣款金額
	@ErrorCode			VARCHAR(6)		OUTPUT	,	-- 回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	-- 回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	-- 回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		-- 回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowDate DATETIME;	-- 系統時間

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_CreditAndWalletQuery_Q01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowDate=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @Token=ISNULL (@Token,'');

BEGIN TRY
	IF @Token='' OR @IDNO='' 
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900';
	END

	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token  AND Rxpires_in>@NowDate;
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

	IF @Error=0
	BEGIN
		-- 會員主檔取付款方式、發票相關資訊
		SELECT @PayMode=PayMode,
			@MEMSENDCD=MEMSENDCD,
			@UNIMNO=UNIMNO,
			@CARRIERID=CARRIERID,
			@NPOBAN=NPOBAN,
			@AutoStored=ISNULL(AutoStored,0)
		FROM TB_MemberData WITH(NOLOCK) 
		WHERE MEMIDNO=@IDNO;

		-- 錢包主檔取狀態、餘額
		SELECT @WalletStatus=Status,
			@WalletAmout=WalletBalance
		FROM TB_UserWallet WITH(NOLOCK)
		WHERE IDNO=@IDNO;

		-- 20220221 UPD BY YEH REASON:增加機車預扣款金額
		SELECT @MotorPreAmt=MapCode FROM TB_Code WITH(NOLOCK) WHERE CodeGroup = 'MotorPreAmt';
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
GO