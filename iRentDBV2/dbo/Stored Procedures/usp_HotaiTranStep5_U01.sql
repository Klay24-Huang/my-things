/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_HotaiTranStep5_U01
* 系    統 : IRENT
* 程式功能 : 更新和泰Pay請款前狀態
* 作    者 : Amber
* 撰寫日期 : 20211208
* 修改日期 :
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_HotaiTranStep5_U01]
	@Xid                    VARCHAR(40),             --中信後台的交易識別碼
	@PRGName                VARCHAR(50),             --程式名稱                            
	@PreStep                INT,                     --先前步驟 
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT =0;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT =0;
DECLARE @NowTime DATETIME;
DECLARE @LogID  BIGINT=0;
DECLARE @NowStep INT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_HotaiTranStep5_U01';
SET @NowTime=dbo.GET_TWDATE()
SET @Xid	    =ISNULL(@Xid,'');
SET @PreStep	=ISNULL(@PreStep,0);
SET @PRGName	=ISNULL(@PRGName,'');
SET @NowStep=5;

BEGIN TRY

   IF @Xid=''OR @PreStep !=4
   BEGIN
     SET @Error=1
     SET @ErrorCode='ERR900'
   END
   
   IF @Error=0
   BEGIN
	 DECLARE @PRGID VARCHAR(20) = '0'
	 IF EXISTS(SELECT 1 FROM TB_APIList WITH(NOLOCK) WHERE APIName = @PRGName)
	 BEGIN
		SELECT @PRGID = CONVERT(VARCHAR(20),APIID) FROM TB_APIList WITH(NOLOCK) WHERE APIName = @PRGName
	 END
	 ELSE
	 BEGIN
		SET @PRGID = Left(@PRGName,20)
	 END

	 IF EXISTS(SELECT 1 FROM TB_HotaiTransaction WITH(NOLOCK) WHERE Xid = @Xid AND Step=@PreStep)
	 UPDATE TB_HotaiTransaction
	 SET  Step = @NowStep,U_PRGID = @PRGID,[U_USERID] = @PRGID,U_SYSDT = @NowTime
	 WHERE Xid=@Xid AND Step = @PreStep;
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