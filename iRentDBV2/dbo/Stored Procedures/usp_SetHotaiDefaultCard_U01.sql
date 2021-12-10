



/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_SetHotaiDefaultCard_U01
* 系    統 : IRENT
* 程式功能 : 綁定和泰Pay預設卡
* 作    者 : AMBER
* 撰寫日期 : 20211123
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_SetHotaiDefaultCard_U01]
(   
	@IDNO		            VARCHAR(10)			  ,	--身分證號 
    @OneID  		        VARCHAR(50)	          ,
	@CardToken  		    VARCHAR(60)	          ,	--信用卡密鑰
	@CardNo  		        VARCHAR(50)	          ,	--隱碼卡號
	@CardType  		        NVARCHAR(20)	      ,	--發卡機構
	@BankDesc  		        NVARCHAR(20)	      ,	--發卡銀行
	@PRGName                VARCHAR(50)	          ,	--程式名稱
    @ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
)
AS
DECLARE @Error      INT = 0;
DECLARE @IsSystem   TINYINT = 1;
DECLARE @ErrorType  TINYINT = 4;
DECLARE @FunName    VARCHAR(50) = 'usp_SetHotaiDefaultCard_U01';
DECLARE @NowTime DATETIME;
DECLARE @LogID      BIGINT=0;

SET @ErrorCode= '0000';
SET @ErrorMsg= 'SUCCESS';
SET @SQLExceptionCode= '';	
SET @SQLExceptionMsg= '';	
SET @IDNO = ISNULL(@IDNO,'');
SET @PRGName = ISNULL(@PRGName,'');
SET @NowTime =dbo.GET_TWDATE();

BEGIN
	BEGIN TRY
		IF @IDNO='' OR @CardToken=''
		BEGIN
			SET @Error=1
			SET @ErrorCode = 'ERR900'
		END

		IF @Error = 0
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
		
	   	 IF EXISTS (SELECT 1 FROM TB_MemberHotaiCard WITH(NOLOCK) WHERE IDNO=@IDNO AND CardToken=@CardToken)
		 BEGIN
		    UPDATE TB_MemberHotaiCard 
			SET isCancel=0,U_PRGID=@PRGID,U_USERID=@IDNO WHERE IDNO=@IDNO AND CardToken=@CardToken;
		 END
		 ELSE
		 BEGIN
		   INSERT INTO TB_MemberHotaiCard (OneID,IDNO,CardType,BankDesc,CardNo,CardToken,A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT)
		   VALUES (@OneID,@IDNO,@CardType,@BankDesc,@CardNo,@CardToken,@PRGID,@IDNO,@NowTime,@PRGID,@IDNO,@NowTime);
		 END
        
		 --PayMode(0:信用卡 4:和泰PAY)
	     UPDATE TB_MemberData SET PayMode=4,U_PRGID=0,U_USERID=@IDNO,U_SYSDT=@NowTime WHERE MEMIDNO=@IDNO AND PayMode=0;
		END
		
		--寫入錯誤訊息
		IF @Error=1
		BEGIN
			INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
			VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
		END

	END TRY
	BEGIN CATCH
		SET @Error=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH
END


