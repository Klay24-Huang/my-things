/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_JointRentIviteeFeedBack_U01
* 系    統 : IRENT
* 程式功能 : 案件共同承租人回應邀請
* 作    者 : AMBER
* 撰寫日期 : 20210901
* 修改日期 : 20210906 UPD BY AMBER REASON: 新增是否檢查Token參數@CheckToken
             20210910 UPD BY AMBER REASON: 拒絕邀請不須檢核
			 20210911 UPD BY AMBER REASON: 調整寫入推播判斷邏輯
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_JointRentIviteeFeedBack_U01]
	@OrderNo                BIGINT                ,	--訂單編號
	@InviteeId              VARCHAR(20)           , --被邀請的ID
	@FeedbackType           VARCHAR(1)            , --邀請回覆(Y:同意  N:拒絕)
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
DECLARE @InviterId VARCHAR(20);
DECLARE @Message nvarchar(500)=''
DECLARE @url varchar(500) =''
DECLARE @imageurl varchar(500) = ''
DECLARE @CheckToken  TINYINT=0


/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_JointRentIviteeFeedBack_U01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @PushTime=DATEADD(SECOND,10,@NowTime);
SET @OrderNo=ISNULL (@OrderNo,0);
SET @InviteeId=ISNULL (@InviteeId,'');
SET @FeedbackType=ISNULL (@FeedbackType,'');

BEGIN TRY
	IF  @InviteeId = '' OR @FeedbackType = ''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
			 

	IF @Error=0
	IF  EXISTS(SELECT 1 FROM TB_TogetherPassenger WITH(NOLOCK) WHERE Order_number=@OrderNo AND MEMIDNO=@InviteeId AND ChkType='S') 
	BEGIN
	   SELECT @InviterId=o.IDNO FROM TB_OrderMain o WITH(NOLOCK) WHERE order_number=@OrderNo;	
	IF @FeedbackType='Y'
	BEGIN			　　						
		DECLARE  @ReturnID VARCHAR(20) 	
		EXEC @Error = usp_JointRentInviteeVerify_Q01 @InviteeId,@OrderNo,@Token,@InviteeId,@LogID,@CheckToken,@ReturnID output,@ErrorCode output,@ErrorMsg output,@SQLExceptionCode output,@SQLExceptionMsg output			  						
	END
	END	
	ELSE
	BEGIN
		SET @Error=1
		SET @ErrorCode='ERR927'
	END
	
    IF @Error=0		
	BEGIN
		UPDATE TB_TogetherPassenger SET ChkType=@FeedbackType,UPTime=@NowTime WHERE  Order_number=@OrderNo AND MEMIDNO=@InviteeId AND ChkType='S';	
			
		IF @@ERROR <> 0 AND @@ROWCOUNT = 0
		 BEGIN
			SET @Error=1
			SET @ErrorCode='ERR929'
		 END		
	END		

	IF @Error=0		
	BEGIN
		SELECT @MEMCNAME=dbo.FN_BlockName(MEMCNAME,'O') FROM TB_MemberData WITH(NOLOCK) WHERE  MEMIDNO=@InviteeId;								
		IF @FeedbackType='Y'
		BEGIN
			SET @Title=N'【共同承租】'+@MEMCNAME+'已同意邀請了唷！'
		END 
		ELSE
		BEGIN
			SET @Title=N'【共同承租】'+@MEMCNAME+'拒絕您的邀請唷！'
		END 						    
		EXEC @Error= usp_InsPersonNotification_I01 @OrderNo,@InviterId,19,@PushTime,@Title,@Message,@url,@imageurl,@LogID,@ErrorCode output,@ErrorMsg output,@SQLExceptionCode output,@SQLExceptionMsg output	
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_JointRentIviteeFeedBack_U01';
END


