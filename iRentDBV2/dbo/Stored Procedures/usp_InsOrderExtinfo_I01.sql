/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_InsOrderExtinfo_I01
* 系    統 : IRENT
* 程式功能 : 新增訂單備註
* 作    者 : AMBER
* 撰寫日期 : 20220216
* 修改日期 :
Example :
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_InsOrderExtinfo_I01]
	@OrderNo                BIGINT                ,	--訂單編號
	@IDNO                   VARCHAR(10)           ,	--帳號
	@PRGID                  VARCHAR(50)           ,	--程式代號
	@PreAuthMode            INT                   ,	--取預授權方式(0信用卡，1錢包，2LinePay，3街口，4和泰Pay)
    @CheckoutMode           INT                   ,	--付款方式
	@DiffAmount             INT                   ,	--差額
	@LogID                  BIGINT                ,	--執行的api log
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
BEGIN
DECLARE @Error INT;
DECLARE @IsSystem TINYINT =0;
DECLARE @FunName VARCHAR(50)='usp_InsOrderExtinfo_I01'
DECLARE @ErrorType TINYINT
DECLARE @hasData INT=0
DECLARE @NowTime DATETIME=dbo.GET_TWDATE()

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @LogID=ISNULL (@LogID,0);

BEGIN TRY
	IF @IDNO='' OR @OrderNo=0
	BEGIN
		SET @Error = 1
		SET @ErrorCode = 'ERR900'
	END

	IF @Error=0
	BEGIN
		IF NOT EXISTS(SELECT * FROM TB_OrderExtinfo WITH(NOLOCK) WHERE order_number=@OrderNo)
		BEGIN
		  INSERT INTO TB_OrderExtinfo (order_number,PreAuthMode,CheckoutMode,DiffAmount,MKTime,MKUser,MKPRGID,UPDTime,UPDUser,UPDPRGID)
		  VALUES(@OrderNo,@PreAuthMode,@CheckoutMode,@DiffAmount,@NowTime,@IDNO,@PRGID,@NowTime,@IDNO,@PRGID);
		END
		ELSE
		BEGIN
		  UPDATE TB_OrderExtinfo
		  SET PreAuthMode=@PreAuthMode,CheckoutMode=@CheckoutMode,DiffAmount=@DiffAmount,UPDTime=@NowTime,UPDUser=@IDNO,UPDPRGID=@PRGID
		  WHERE order_number=@OrderNo;
		END
	END

	IF @@ERROR <> 0 And @@ROWCOUNT = 0
	BEGIN
	  SET @Error = 1
	  SET @ErrorCode = 'ERR252'
	  SET @ErrorMsg='SP執行失敗'
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsOrderExtinfo_I01';
END



