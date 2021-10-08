
/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_JointRentInviteeModify_U01
* 系    統 : IRENT
* 程式功能 : 共同承租人邀請清單異動
* 作    者 : Umeko
* 撰寫日期 : 20210825
* 修改日期 : 20210906 UPD BY Umeko REASON: 配合檢核 帶入 "是否檢查Token參數"
* 修改日期 : 20210906 UPD BY Umeko REASON: 加入邀請使用的推播網址"
* 修改日期 : 20210909 UPD BY Umeko REASON: Example :@notificationUrl 改大寫 @NotificationUrl
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_JointRentInviteeModify_U01]
	@OrderNo                BIGINT                ,	--訂單編號
	@InviteeId              VARCHAR(10)                , --被邀請人帳號
	@ActionType             Char(1)               , --行為
	@NotificationUrl      nvarchar(500)     , --邀請網址
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

SET @FunName='usp_JointRentInviteeModify_U01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @Token=ISNULL (@Token,'');
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
Set @InviteeId = Isnull(@InviteeId,'')
Set @ActionType = Isnull(@ActionType,'')

DECLARE @STime DateTime
Declare @ChkType char(1)

BEGIN TRY
	IF @Token='' OR @IDNO=''   OR @InviteeId = '' OR @ActionType = ''
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

	--推播需要欄位
	Declare @MemberName nvarchar(20)
	Select @MemberName = dbo.FN_BlockName(MEMCNAME,'O') From TB_MemberData with(nolock) Where MEMIDNO = @IDNO
	Declare @Title nvarchar(500)
		,@Message nvarchar(500)=''
		,@url varchar(500) =''
		,@imageurl varchar(500) = ''
		,@ActionName nvarchar(10)
	
	--主行為
	IF @Error=0
	BEGIN
		Select @ChkType = ChkType From TB_TogetherPassenger Where MEMIDNO = @InviteeId And Order_number = @OrderNo

		IF @ActionType = 'S' --重新邀請
		Begin
			Set @ActionName = '邀請您共同承租唷!'
			Declare @ReturnID VARCHAR(20) 
			--執行邀請判斷
			Exec @Error = usp_JointRentInviteeVerify_Q01 @InviteeId,@OrderNo,@Token,@IDNO,@LogID,1,@ReturnID output,@ErrorCode output,@ErrorMsg output,@SQLExceptionCode output,@SQLExceptionMsg output
			
			print '@ErrorCode='+@ErrorCode
			IF @ChkType Not in ('F','N')
			Begin
				Set @Error=1
				SET @ErrorCode='ERR925'
			End

			--狀態異動
			if @Error = 0
			Begin
				Update TB_TogetherPassenger Set ChkType = 'S'
				Where MEMIDNO = @InviteeId And Order_number = @OrderNo And ChkType in ('F','N')

				IF @@ERROR <> 0 And @@ROWCOUNT = 0
				Begin
					Set @Error=1
					SET @ErrorCode='ERR925'
				End
			End

			IF @Error = 0
			Begin
				Set @STime = DateAdd(SECOND,10,dbo.GET_TWDATE())
				
				Set @Title = CONCAT('【共同承租】',@MemberName,@ActionName)
				if @notificationUrl <> ''
				Begin
					Set @url = @notificationUrl
				End

				Set @Message = @Title

				Exec @Error = usp_InsPersonNotification_I01 
								  @OrderNo
								, @InviteeId
								, 19				
								, @STime
								, @Title
								, @Message
								, @url
								, @imageurl
								, @LogID
								, @ErrorCode output
								, @ErrorMsg output
								, @SQLExceptionCode output
								, @SQLExceptionMsg output

								print '@ErrorCode='+@ErrorCode
			End
		End
		Else If  @ActionType = 'F' --取消
		Begin
			Set @ActionName = '取消邀請了唷!!'

			IF @ChkType Not in ('Y','S')
			Begin
				Set @Error=1
				SET @ErrorCode='ERR924'
			End

			IF @Error = 0
			Begin
				Update TB_TogetherPassenger Set ChkType = 'F'
				Where MEMIDNO = @InviteeId And Order_number = @OrderNo And ChkType in ('Y','S')

				IF @@ERROR <> 0 And @@ROWCOUNT = 0
				Begin
					SET @Error=1;
					SET @ErrorCode='ERR924';
				End
			End

			IF @Error = 0
			Begin	
				Set @STime = DateAdd(SECOND,10,dbo.GET_TWDATE())
				
				Set @Title = CONCAT('【共同承租】',@MemberName,@ActionName)
				
				if @notificationUrl <> ''
				Begin
					Set @url = @notificationUrl
				End

				Set @Message = @Title

				Exec @Error = usp_InsPersonNotification_I01 
								  @OrderNo
								, @InviteeId
								, 19				
								, @STime
								, @Title
								, @Message
								, @url
								, @imageurl
								, @LogID
								, @ErrorCode output
								, @ErrorMsg output
								, @SQLExceptionCode output
								, @SQLExceptionMsg output

								print '@ErrorCode='+@ErrorCode
			End
		End
		Else If  @ActionType = 'D' --移除
		Begin
			IF @ChkType Not in ('F','N')
			Begin
				Set @Error=1
				SET @ErrorCode='ERR926'
			End
			IF @Error = 0
			Begin
				Delete From TB_TogetherPassenger Where MEMIDNO = @InviteeId And Order_number = @OrderNo And ChkType in ('F','N')
				IF @@ERROR <> 0 And @@ROWCOUNT = 0
				Begin	
					SET @Error=1;
					SET @ErrorCode='ERR926';
				End
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_JointRentInviteeModify_U01';
END