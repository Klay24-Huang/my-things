/****************************************************************
** Name: [dbo].[usp_BE_GetOrderInfoBeforeModify]
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
** EXEC @Error=[dbo].[usp_BE_GetOrderInfoBeforeModify]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/15
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/15|		Eric  |          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_GetOrderInfoBeforeModify]
	@OrderNo                BIGINT                , --訂單編號
	@UserID                 NVARCHAR(10)          , --使用者
	@LogID                  BIGINT                ,
	@hasModify              TINYINT         OUTPUT, --是否有修改過(0:否;1:是)
	@ModifyTime             VARCHAR(20)     OUTPUT, --上次修改時間
	@ModifyUserID           NVARCHAR(10)    OUTPUT, --上次修改者
	@LastStartTime			VARCHAR(20)		OUTPUT,
	@LastStopTime			VARCHAR(20)		OUTPUT,
	@LastEndMile			INT             OUTPUT,
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
DECLARE @CarNo   VARCHAR(10);
DECLARE @ProjID  VARCHAR(10);
DECLARE @ProjType TINYINT;
/*DECLARE @LastStartTime DATETIME;
DECLARE @LastStopTime  DATETIME;
DECLARE @LastEndMile   INT;*/
DECLARE @StopTime      DATETIME;


/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_GetOrderInfoBeforeModify';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @UserID    =ISNULL (@UserID    ,'');


		BEGIN TRY

		 
		 IF @UserID='' 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.查詢是否有此訂單
		 IF @Error=0
		 BEGIN
		 SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo;	--查詢是否有此訂單
				IF @hasData=0
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR754';	--找不到此訂單
				END
				ELSE
				BEGIN
					SET @hasData=0;
					SELECT @hasData=COUNT(1) FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo AND car_mgt_status = 16 AND booking_status = 5 AND cancel_status = 0;
					IF @hasData=0
					BEGIN
						SET @Error=1;
						SET @ErrorCode='ERR755';	--未完成還車
					END
				END
		 END
		 IF @Error=0
		 BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_OrderMain main
		   JOIN TB_OrderDetail detail on main.order_number = detail.order_number
		   WHERE main.order_number = @OrderNo AND detail.final_stop_time BETWEEN DATEADD(DAY, -90, @NowTime) AND @NowTime
		   IF @hasData=0
		   BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR756';
		   END
		 END
		 IF @Error=0
		 BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_ReturnCarControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo;
			--方便測試，先封印
			IF @hasData=0
			BEGIN
			   
				SET @Error=1;
				SET @ErrorCode='ERR757';
			END
		 END
		 --取出此車號前一筆還車時間及里程
		 IF @Error=0
		 BEGIN
				SELECT @CarNo=CarNo FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo;
				SELECT @StopTime=final_stop_time FROM TB_OrderDetail  WITH(NOLOCK) WHERE order_number=@OrderNo;

				SELECT TOP 1 @LastStartTime=IIF(ISNULL(final_start_time,'')='','',CONVERT(VARCHAR(20),final_start_time,120)),@LastStopTime=IIF(ISNULL(final_stop_time,'')='','',CONVERT(VARCHAR(20),final_stop_time,120)),@LastEndMile=ISNULL(detail.end_mile,0)
				FROM TB_OrderMain main JOIN TB_OrderDetail detail on main.order_number = detail.order_number
				WHERE car_mgt_status = 16 and booking_status = 5 and cancel_status = 0 and ProjID != '' and CarNo = @CarNo and main.order_number != @OrderNo and final_stop_time < @StopTime  ORDER BY start_time DESC
		 END
		 --取出上次修改時間及修改者
		 IF @Error=0	
		 BEGIN
			SELECT TOP 1 @hasModify=ISNULL(modifyId,0),@ModifyUserID=U_USERID,@ModifyTime=IIF(ISNULL(U_SYSDT,'')='','',CONVERT(VARCHAR(20),U_SYSDT,120)) FROM TB_OrderModifyLog WITH(NOLOCK) WHERE order_number=@OrderNo ORDER BY modifyId DESC
		 END


		 IF @Error=0
		 BEGIN
			SELECT * FROM VW_BE_GetOrderModifyInfoNew WITH(NOLOCK) WHERE OrderNo=@OrderNo;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetOrderInfoBeforeModify';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetOrderInfoBeforeModify';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改合約前取出資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetOrderInfoBeforeModify';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetOrderInfoBeforeModify';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetOrderInfoBeforeModify';