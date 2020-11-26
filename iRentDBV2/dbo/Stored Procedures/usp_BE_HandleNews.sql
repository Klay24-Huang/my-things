/****************************************************************
** Name: [dbo].[usp_BE_HandleNews]
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
** EXEC @Error=[dbo].[usp_BE_HandleNews]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/26 上午 10:05:25 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/26 上午 10:05:25    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_HandleNews]
    @NewsID                 INT                   ,
    @Title                  NVARCHAR (50)         ,
    @NewsType               TINYINT               ,
    @NewsClass              TINYINT               ,
    @Content                NVARCHAR (100)        ,
    @URL                    VARCHAR (250)         ,
    @SD                     DATETIME              ,
    @ED                     DATETIME              ,
    @Mode                   TINYINT               , --0:新增;1:修改;2:刪除
    @BeTop                   VARCHAR(10)               , --0:否;1:是
	@UserID                 NVARCHAR(10)          , --使用者
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;
DECLARE @pushID BIGINT;
DECLARE @isSend TINYINT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_HandleNews';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @NewsID   =ISNULL(@NewsID  ,0);
SET @Title    =ISNULL(@Title   ,'');
SET @NewsType =ISNULL(@NewsType,2);
SET @NewsClass =ISNULL(@NewsClass,1);
SET @Content  =ISNULL(@Content ,'');
SET @URL      =ISNULL(@URL     ,'');
SET @SD       =ISNULL(@SD      ,'1911-01-01 00:00:00');
SET @ED       =ISNULL(@ED      ,'1911-01-01 00:00:00');
SET @Mode     =ISNULL(@Mode    ,3);
SET @pushID=0;
SET @isSend=0;
SET @UserID    =ISNULL (@UserID    ,'');


		BEGIN TRY

		 
		 IF @UserID='' OR @Title='' OR @Content='' OR @SD='1911-01-01 00:00:00'  OR @ED='1911-01-01 00:00:00' OR @Mode=3 OR (@NewsType=1 AND @URL='') OR (@Mode>0 AND @NewsID=0) OR (@Mode=0 AND @NewsID>0)
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
		 	IF @Mode=0
			BEGIN
			   INSERT INTO TB_News(NewsType,Title,Content,SD,ED,[URL],[NewsClass],[isTop])VALUES(@NewsType,@Title,@Content,@SD,@ED,@URL,@NewsClass,@BeTop);
			   IF @@ROWCOUNT=1
			   BEGIN
			      SET @NewsID=@@IDENTITY;
			      INSERT INTO TB_PersonNotification(NType,Title,[Message],[url],NewsID,STime)VALUES(0,@Title,@Content,@URL,@NewsID,@SD);
			   END
			   ELSE
			   BEGIN
			      SET @Error=1;
				  SET @ErrorCode='ERR770';
			   END
			END
			ELSE IF @Mode=1
			BEGIN
			   SELECT @pushID=NotificationID,@isSend=isSend FROM TB_PersonNotification WHERE NewsID=@NewsID;
			   IF @pushID>0 AND @isSend=1
			   BEGIN
			      SET @Error=1;
				  SET @ErrorCode='ERR771'
			   END

			   IF @Error=0
			   BEGIN
			      UPDATE TB_News
				  SET Title=@Title,Content=@Content,SD=@SD,ED=@ED,[URL]=@URL,NewsType=@NewsType,[NewsClass]=@NewsClass,[isTop]=@BeTop
				  WHERE NewsID=@NewsID;
				  IF @pushID>0
				  BEGIN
				      UPDATE TB_PersonNotification
				      SET Title=@Title,[Message]=@Content,[url]=@URL
				      WHERE NewsID=@NewsID;
				  END
				  ELSE
				  BEGIN
				     INSERT INTO TB_PersonNotification(NType,Title,[Message],[url],NewsID,PushTime)VALUES(0,@Title,@Content,@URL,@NewsID,@SD);
				  END
			   END
			END
			ELSE IF @Mode=2
			BEGIN
			    SELECT @pushID=NotificationID,@isSend=isSend FROM TB_PersonNotification WHERE NewsID=@NewsID;
			   IF @pushID>0 AND @isSend=1
			   BEGIN
			      SET @Error=1;
				  SET @ErrorCode='ERR772';
			   END

			   IF @Error=0
			   BEGIN
			     DELETE  FROM  TB_PersonNotification WHERE NewsID=@NewsID;
			     DELETE  FROM  TB_News WHERE NewsID=@NewsID;
			   END
			END
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleNews';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleNews';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台推播處理', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleNews';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleNews';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleNews';