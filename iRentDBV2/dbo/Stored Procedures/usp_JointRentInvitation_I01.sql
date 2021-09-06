/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_JointRentInvitation_I01
* 系    統 : IRENT
* 程式功能 : 共同承租人邀請
* 作    者 : AMBER
* 撰寫日期 : 20210827
* 修改日期 : 20210831 UPD BY AMBER REASON: 參數名稱更正&新增錯誤代碼
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_JointRentInvitation_I01]
	@OrderNo                BIGINT                ,	--訂單編號
	@InviteeId              VARCHAR(20)           , --被邀請的ID
	@QueryId                VARCHAR(20)           ,	--要邀請的ID或手機
	@IDNO                   VARCHAR(20)           ,	--帳號
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
DECLARE @PushTime DATETIME;
DECLARE @Title NVARCHAR(500);
DECLARE @MEMCNAME NVARCHAR(60);
DECLARE @Message nvarchar(500)=''
DECLARE @url varchar(500) =''
DECLARE @imageurl varchar(500) = ''


/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_JointRentInvitation_I01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @PushTime=DATEADD(SECOND,10,@NowTime);
SET @Token=ISNULL (@Token,'');
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);

BEGIN TRY
	IF @Token='' OR @IDNO=''
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

	IF @Error=0   
	IF NOT EXISTS(SELECT * FROM TB_TogetherPassenger WITH(NOLOCK) WHERE Order_number=@OrderNo AND MEMIDNO=@InviteeId)	
	BEGIN
		INSERT INTO [dbo].[TB_TogetherPassenger]
			([Order_number],[MEMIDNO],[APPUSEID],[MEMCNAME],[MEMTEL],[ChkType],[MKTime],[UPTime])
			SELECT 
			order_number=@OrderNo,
			m.MEMIDNO,
			APPUSEID=@QueryId,
			m.MEMCNAME,
			m.MEMTEL,
			ChkType='S',
			MKTIME=@NowTime,
			UPTime=@NowTime 
			FROM TB_MemberData m WITH(NOLOCK) 	
			WHERE m.MEMIDNO=@InviteeId;

		IF @@ERROR <> 0 AND @@ROWCOUNT = 0
			BEGIN
				SET @Error=1
				SET @ErrorCode='ERR929'
		    END		
		
		IF @Error=0		
		BEGIN
		    SELECT @MEMCNAME=dbo.FN_BlockName(MEMCNAME,'O') FROM TB_MemberData WITH(NOLOCK) WHERE  MEMIDNO=@IDNO
			SET @Title=N'【共同承租】'+@MEMCNAME+'邀請您共同承租唷！'
			EXEC @Error= usp_InsPersonNotification_I01   @OrderNo,@InviteeId,19,@PushTime,@Title,@Message,@url,@imageurl,@LogID,@ErrorCode output,@ErrorMsg output,@SQLExceptionCode output,@SQLExceptionMsg output
		END
	END
	ELSE
	BEGIN
	   SET @Error=1
	   SET @ErrorCode='ERR928'
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_JointRentInvitation_I01';
END


