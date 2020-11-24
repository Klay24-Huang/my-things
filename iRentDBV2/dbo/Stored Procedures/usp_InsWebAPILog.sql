/****************************************************************
** Name: [dbo].[usp_InsWebAPILog]
** Desc: 
**
** Return values: 0 成功 else 錯誤
** Return Recordset: 
**
** Called by: 
**
** Parameters:
** Input
** -----------

** 
**
** Output
** -----------
		
	@ErrorCode 				VARCHAR(6)			
	@ErrorCodeDesc			NVARCHAR(100)	
	@SQLExceptionCode		VARCHAR(10)				
	@SqlExceptionMsg		NVARCHAR(1000)	
**
** 
** Example
**------------
** DECLARE @Error               INT;
** DECLARE @ErrorCode 			VARCHAR(6);		
** DECLARE @ErrorMsg  			NVARCHAR(100);
** DECLARE @SQLExceptionCode	VARCHAR(10);		
** DECLARE @SQLExceptionMsg		NVARCHAR(1000);
** EXEC @Error=[dbo].[usp_InsWebAPILog]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/5 下午 06:55:06 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/5 下午 06:55:06    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsWebAPILog]
    @WebAPIURL              VARCHAR(512)          , --送出的url
	@WebAPIName             VARCHAR(50)           , --執行的api名稱
	@WebAPIInput			VARCHAR(4096)		  , --送出的json
	@WebAPIOutput           NVARCHAR(MAX)         , --收到的json
	@MKTime                 DATETIME              , --送出時間
	@UPDTime                DATETIME              , --收到回覆時間
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;

DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @NowTime DATETIME;
DECLARE @TmpWebAPIURL VARCHAR(512); --API SITE
DECLARE @TmpWebAPIName VARCHAR(50); --API Name
DECLARE @APIID INT; --找出來的WebAPI ID
DECLARE @ErrorType TINYINT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_InsWebAPILog';
SET @IsSystem=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @TmpWebAPIURL='';
SET @TmpWebAPIName='';
SET @APIID=0;
SET @ErrorType=0;
SET @WebAPIURL   =ISNULL(@WebAPIURL   ,'');
SET @WebAPIName  =ISNULL(@WebAPIName  ,'');
SET @WebAPIInput =ISNULL(@WebAPIInput,'');
SET @WebAPIOutput=ISNULL(@WebAPIOutput,'');
SET @MKTime      =ISNULL(@MKTime      ,@NowTime);
SET @UPDTime     =ISNULL(@UPDTime     ,@NowTime);

		BEGIN TRY
		 IF @WebAPIURL='' OR @WebAPIName='' OR @WebAPIInput='' OR @WebAPIOutput=''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR100' --
 		 END
		 IF @Error=0
		 BEGIN
		    IF @MKTime=@UPDTime AND @MKTime=@NowTime
			BEGIN
			   SET @ErrorCode='ERR910'
			END
		 END
		 IF @Error=0
		 BEGIN
		     SELECT @APIID=WebAPIId,@TmpWebAPIURL=WebAPIURL,@TmpWebAPIName=WebAPIName FROM TB_WebAPIList WITH (NOLOCK) WHERE WebAPIName=@WebAPIName;

			 IF @APIID=0
			 BEGIN
			    SET @Error=1;
				SET @ErrorCode='ERR908';
				SET @SQLExceptionMsg = 'Input: @WebAPIName=' + @WebAPIName + ';' + '@WebAPIURL=' + @WebAPIURL
			 END
			 ELSE
			 BEGIN
			   IF @TmpWebAPIName<>@WebAPIName
			   BEGIN
			     SET @Error=1;
				 SET @ErrorCode='ERR909';  --呼叫的url或是功能不正確
				 SET @SQLExceptionMsg = 'Input: @WebAPIId=' + CAST(@APIID AS VARCHAR(10)) + ';' + '@WebAPIName=' + @WebAPIName + ';' + '@WebAPIURL=' + @WebAPIURL
			   END
	
			 END
		 END
		 IF @Error=0
		 BEGIN
		   INSERT INTO TB_WebAPILog(WebAPIId,WebAPIInput,WebAPIOutput,DTime,UPDTime)VALUES(@APIID,@WebAPIInput,@WebAPIOutput,@MKTime,@UPDTime);
		 END
		--寫入錯誤訊息
		    IF @Error=1
			BEGIN
			 INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
				 VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,0,@IsSystem);
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
				 VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,0,@IsSystem);
		END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsWebAPILog';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsWebAPILog';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寫入呼叫第三方WebAPI Log', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsWebAPILog';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsWebAPILog';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsWebAPILog';