/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetPayInfo_Q1
* 系    統 : IRENT
* 程式功能 : 取得帳號預設付款方式及現有付款方式清單
* 作    者 : Umeko
* 撰寫日期 : 20210913
* 修改日期 :
Example :
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_GetPayInfo_Q1]
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

	SET @FunName='usp_GetPayInfo_Q1';
	SET @IsSystem=0;
	SET @ErrorType=0;
	SET @IsSystem=0;
	SET @hasData=0;
	SET @NowTime=DATEADD(HOUR,8,GETDATE());
	SET @Token=ISNULL (@Token,'');
	SET @IDNO=ISNULL (@IDNO,'');


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

		--輸出資訊
		IF @Error=0
		BEGIN
			--Set @IDNO = 'A123456789'
			--Get Default PayMode
				Declare @DefaultPayMode tinyint = 0,@AutoStoreFlag tinyint = 0,@PayModeBindCount  int = 0--,@IDNO varchar(10) = 'A123456789'
					
					Select @DefaultPayMode =PayMode,@AutoStoreFlag = AutoStored
					From  TB_MemberData with(nolock)
					Where MEMIDNO = @IDNO

						Declare @UserPayModeList as Table(PayMode int,PayModeName nvarchar(20),HasBind int,PayInfo varchar(20),Balance int,
						AutoStoreFlag int ,NotBindMsg nvarchar(50))

						Insert into @UserPayModeList
						Select 	ModeList.PayMode,PayModeName
							,Case When UserPay.PayInfo is null Then 0 Else 1 End HasBind,Isnull(UserPay.payInfo,'') PayInfo,Isnull(UserPay.Balance,0)  Balance
							,0 AutoStoreFlag,Case When UserPay.PayInfo is null Then NotBindMsgList.NotBindMsg  Else '' End NotBindMsg
						From(
						--取出目前開放的付款方式
							Select Cast(Code0 as tinyint) 'PayMode',CodeName 'PayModeName'
							From [dbo].[TB_WalletCodeTable] with(nolock)
							Where  CodeGroup = 'CheckoutMode' And UseFlg = 1
						)ModeList 
						Join 
						(
						--設定該儲值方式為綁定之訊息
								Select * 
								From (values
								(0 , '尚未綁定'),
								(1 , '尚未開通')) a (PayMode ,NotBindMsg ) 
						) NotBindMsgList On ModeList.PayMode = NotBindMsgList.PayMode
						Left Join
						(
							--取出用戶綁定付費資訊
							Select * 
							From (
								--取出綁定信用卡
								SELECT Top 1  0 'PayMode',CardNumber PayInfo ,0 Balance,0 AutoStoreFlag
								FROM TB_MemberCardBinding WITH(NOLOCK)
								WHERE IDNO=@IDNO
								AND IsValid=1
								ORDER BY MKTime DESC
							) tA
							Union All
							(
								--取出和雲錢包
								Select 1 'CheckoutMode', Convert(varchar,WalletBalance,126),WalletBalance Balance,@AutoStoreFlag
								From TB_UserWallet with(nolock)
								Where IDNO = @IDNO And [Status] = 2
							)
						) UserPay On ModeList.PayMode = UserPay.PayMode

						Select @PayModeBindCount = sum(HasBind) From @UserPayModeList
						
						Select @DefaultPayMode DefPayMode ,@PayModeBindCount as PayModeBindCount
						,(Select  PayMode ,PayModeName,HasBind   
						,PayInfo  ,Balance 
						,AutoStoreFlag  ,NotBindMsg  
						From @UserPayModeList 
						For Json Path ) PayModeList						
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

	EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetPayInfo_Q1';
END