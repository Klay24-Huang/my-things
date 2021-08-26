-- =============================================
-- Author:Umeko
-- Create date:2021/08/25
-- Description:邀請清單異動
-- =============================================
Create PROCEDURE [dbo].[usp_JointRentInviteeModify_U01]
	@OrderNo                BIGINT                ,	--訂單編號
	@InviteeId              BIGINT                , --被邀請人帳號
	@ActionType             Char(1)               , --行為
	@IDNO                   VARCHAR(10)           ,	--帳號
	@Token                  VARCHAR(1024)         ,	--JWT TOKEN
	@LogID                  BIGINT                ,	--執行的api log
	@Insurance				INT					  , --加購安心服務(0:否;1:有)
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

SET @FunName='usp_GetJointRentInviteeList';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
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

	--輸出訂單資訊
	IF @Error=0
	BEGIN
		IF @ActionType = 'S' --重新邀請
		Begin
			--執行邀請判斷
			Update TB_TogetherPassenger Set ChkType = 'S'
			Where MEMIDNO = @InviteeId And Order_number = @OrderNo And ChkType in ('F','N')

			IF @@ERROR <> 0 And @@ROWCOUNT = 0
			Begin
				SET @Error=1;
				SET @ErrorCode='ERR101';
			End

			IF @Error = 0
			Begin
				--寫入推播
				SET @Error=1;
				SET @ErrorCode='ERR101';
			End
		End
		Else If  @ActionType = 'F' --取消
		Begin
			Update TB_TogetherPassenger Set ChkType = 'F'
			Where MEMIDNO = @InviteeId And Order_number = @OrderNo And ChkType in ('Y','S')

			IF @@ERROR <> 0 And @@ROWCOUNT = 0
			Begin
				SET @Error=1;
				SET @ErrorCode='ERR101';
			End

			IF @Error = 0
			Begin
				-- todo 寫入推播

				Select 1 
			End
		End
		Else If  @ActionType = 'D' --移除
		Begin
			IF Exists(Select 1 From TB_TogetherPassenger Where MEMIDNO = @InviteeId And Order_number = @OrderNo And ChkType in ('F','N'))
			Begin
				Delete From TB_TogetherPassenger Where MEMIDNO = @InviteeId And Order_number = @OrderNo And ChkType in ('F','N')

				IF @@ERROR <> 0 And @@ROWCOUNT = 0
				Begin
					--todo 定ERRORCODE
					SET @Error=1;
					SET @ErrorCode='ERR101';
				End

			End
			Else
			Begin
				SET @Error=1;
				SET @ErrorCode='ERR101';
			End
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetJointRentInviteeList';
END