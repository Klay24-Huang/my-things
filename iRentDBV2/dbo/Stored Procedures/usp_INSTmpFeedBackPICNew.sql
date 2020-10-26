/****************************************************************
** Name: [dbo].[usp_INSTmpFeedBackPIC]
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
** EXEC @Error=[dbo].[usp_INSTmpFeedBackPIC]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/18 上午 05:38:56 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/18 上午 05:38:56    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_INSTmpFeedBackPICNew]
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@Token                  VARCHAR(1024)         ,
	@LogID                  BIGINT                ,
	@CarImages              TY_CarImage		READONLY
	--@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	--@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	--@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	--@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @ErrorCode 				VARCHAR(6)		,	--回傳錯誤代碼
		@SQLExceptionCode		VARCHAR(10)		,	--回傳sqlException代碼
		@SQLExceptionMsg		NVARCHAR(1000)		--回傳sqlException訊息
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @car_mgt_status TINYINT;
DECLARE @cancel_status TINYINT;
DECLARE @booking_status TINYINT;
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
--SET @ErrorMsg='SUCCESS'; 
SET @ErrorMsg=''; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_INSTmpFeedBackPICNew';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【上傳取車回饋照片】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token    =ISNULL (@Token    ,'');

		BEGIN TRY

		 
		 IF @Token='' OR @IDNO=''  OR @OrderNo=0
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
		 BEGIN
			SELECT @hasData=COUNT(1) FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo AND cancel_status=0;
			IF @hasData=1
			BEGIN
				  SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status
				  FROM TB_OrderMain WITH(NOLOCK)
				  WHERE order_number=@OrderNo;	
				--未取車或已按還車
				IF @car_mgt_status<4 OR @car_mgt_status>=11
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR198';
				END

			END
			--ELSE
			--BEGIN
			--	SET @Error=1;
			--	SET @ErrorCode='ERR170';
			--END
		 END
		 IF @Error=0
		 BEGIN
			--SELECT @hasData=COUNT(1) FROM TB_tmpFeedBackPIC WITH(NOLOCK) WHERE OrderNo=@OrderNo AND SEQNO=@SEQNO ;
			--IF @hasData=0
			--BEGIN
			--   INSERT INTO TB_tmpFeedBackPIC(OrderNo,SEQNO,FeedBackFile)VALUES(@OrderNo,@SEQNO,@FeedBackFile);
			--END
			--ELSE
			--BEGIN
			--	UPDATE TB_tmpFeedBackPIC
			--	SET OrderNo=@OrderNo, SEQNO=@SEQNO,FeedBackFile=@FeedBackFile,UPDTime=@NowTime
			--	WHERE OrderNo=@OrderNo AND  SEQNO=@SEQNO;
			--END

			UPDATE A
			SET FeedBackFile=B.CarImage
				,UPDTime=@NowTime
			FROM TB_tmpFeedBackPIC A WITH(NOLOCK)
			JOIN @CarImages B ON A.SEQNO=B.CarImageType
			WHERE A.OrderNo=@OrderNo;

			INSERT INTO TB_tmpFeedBackPIC(OrderNo,SEQNO,FeedBackFile)
			SELECT 
				OrderNo=@OrderNo
				,A.CarImageType
				,A.CarImage
			FROM @CarImages A 
			LEFT JOIN TB_tmpFeedBackPIC B WITH(NOLOCK) ON A.CarImageType=B.SEQNO AND B.OrderNo=@OrderNo 
			WHERE B.OrderNo IS NULL;
		 END
		--增加錯誤回傳
		SELECT 
			Error=@Error,
			ErrorCode=@ErrorCode,
			ErrorMsg=@ErrorMsg,
			SQLExceptionCode=@SQLExceptionCode,
			SQLExceptionMsg=@SQLExceptionMsg;

		 SELECT SEQNO,1 AS HasUpload FROM TB_tmpFeedBackPIC WITH(NOLOCK) WHERE OrderNo=@OrderNo;
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
			
			--增加錯誤回傳
			SELECT 
				Error=@Error,
				ErrorCode=@ErrorCode,
				ErrorMsg=@ErrorMsg,
				SQLExceptionCode=@SQLExceptionCode,
				SQLExceptionMsg=@SQLExceptionMsg;

			--IF @@TRANCOUNT > 0
			--BEGIN
			--	print 'rolling back transaction' /* <- this is never printed */
			--	ROLLBACK TRAN
			--END
			 SET @IsSystem=1;
			 SET @ErrorType=4;
			      INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
				 VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
		END CATCH
--RETURN @Error
RETURN 0

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_INSTmpFeedBackPIC';