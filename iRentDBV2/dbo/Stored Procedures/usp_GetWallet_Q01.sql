/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetWallet_Q01
* 系    統 : IRENT
* 程式功能 : 取得會員錢包相關資訊
* 作    者 : AMBER
* 撰寫日期 : 20210914
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetWallet_Q01]
	@IDNO                   VARCHAR(10)           , --操作的會員帳號
	@Token                  VARCHAR(1024)         , --JWT TOKEN
	@LogID                  BIGINT                , --執行的api log
	@WalletMemberID         VARCHAR(20)     OUTPUT, --錢包會員ID
	@WalletAccountID		VARCHAR(20)		OUTPUT, --錢包虛擬ID
	@Email				    VARCHAR(200)	OUTPUT, --開戶時申請的mail
	@PhoneNo				VARCHAR(20)		OUTPUT, --開戶時申請的電話
	@Name                   NVARCHAR(10)    OUTPUT, --開戶時申請的姓名
	@WalletBalance          INT             OUTPUT, --錢包餘額
	@MonthlyTransAmount     INT             OUTPUT, --當月交易金流
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData INT;
DECLARE @NowTime DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET	@SQLExceptionCode = ''		
SET	@SQLExceptionMsg = ''		

SET @FunName='usp_GetWallet_Q01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=DATEADD(M,-1,DATEADD(HOUR,8,GETDATE()));
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @Token    =ISNULL (@Token    ,'');

BEGIN TRY		 
		 IF @Token='' OR @IDNO='' 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		 -- 0.再次檢核token
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
		 
		 -- 1.取得基本資料
		 IF @Error=0
		 BEGIN
			SELECT @hasData=COUNT(1) FROM TB_UserWallet WITH(NOLOCK) WHERE IDNO=@IDNO;
			IF @hasData>0
			BEGIN
				SELECT @WalletMemberID=WalletMemberID,@WalletAccountID=WalletAccountID,@Email=Email,@PhoneNo=PhoneNo,@Name=ISNULL(MEMCNAME,''),@WalletBalance=ISNULL(WalletBalance,0)
				FROM TB_UserWallet  WITH(NOLOCK)
				LEFT JOIN TB_MemberData WITH(NOLOCK) ON MEMIDNO=IDNO
				WHERE IDNO=@IDNO;
			END
			ELSE
			BEGIN
			  SELECT @WalletMemberID='',@WalletAccountID='',@Email=MEMEMAIL,@PhoneNo=MEMTEL,@Name=MEMCNAME FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
			END
		 END

		 --2.取得當月儲值金流(含受贈)
		 IF @Error=0
		   SELECT @MonthlyTransAmount =ISNULL(SUM(TradeAMT),0)
		   FROM TB_WalletTradeMain tm WITH(NOLOCK)
		   JOIN TB_WalletCodeTable c 
		   ON c.CodeGroup = 'TradeType' and c.Code0 = tm.TradeType
		   WHERE  tm.IDNO = @IDNO
		   AND  tm.TradeDate >= DATEADD(M, DATEDIFF(M,0,@NowTime),0)
		   AND  tm.TradeDate <= DATEADD(ms,-2,DATEADD(mm, DATEDIFF(m,0,@NowTime)+1, 0)) 
		   AND  c.Negative=0;	
		 
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetWallet_Q01';



