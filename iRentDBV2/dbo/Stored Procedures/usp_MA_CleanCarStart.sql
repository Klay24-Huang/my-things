/****************************************************************
** Name: [dbo].[usp_MA_CleanCarStart]
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
** EXEC @Error=[dbo].[usp_MA_CleanCarStart]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/27 上午 11:21:40 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/27 上午 11:21:40    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_MA_CleanCarStart]
	@UserID                 VARCHAR(20)          ,
	@CarNo                  VARCHAR(10)          ,
	@OrderNum               BIGINT               ,
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
DECLARE @tmpRentTimes INT;
DECLARE @tmpDetail BIGINT;
DECLARE @lastCleanTime DATETIME;
DECLARE @tmpOrderNum VARCHAR(20);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_MA_CleanCarStart';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @tmpOrderNum='';
SET @tmpRentTimes=0;
SET @UserID    =ISNULL (@UserID    ,'');
SET @CarNo=ISNULL (@CarNo,'');
SET @OrderNum=ISNULL (@OrderNum,0);
SET @hasData=0;
SET @tmpDetail=0;



		BEGIN TRY

		 
		 IF @CarNo='' OR @UserID='' OR @OrderNum=0
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
				SELECT @tmpRentTimes=ISNULL(UncleanCount,0) FROM [dbo].[TB_CarInfo] WITH(NOLOCK) WHERE CarNo=@CarNo;
				SELECT @hasData=ISNULL(1,0),@lastCleanTime=ISNULL(lastCleanTime,'2019-07-31 00:00:00') FROM [dbo].[TB_CarCleanData] WITH(NOLOCK) WHERE CarNo=@CarNo;
				--IF @hasData=0
				--BEGIN
				--   INSERT INTO  [iRent_BackEnd].[dbo].[TB_CarCleanData_201906] (CarNo,lastCleanTime,lastRentTimes,lastOpt,UPDTime)VALUES(@CarNo,GETDATE(),@tmpRentTimes,@UserID,GETDATE());
				--END
				--ELSE
				--BEGIN
				--   UPDATE  [iRent_BackEnd].[dbo].[TB_CarCleanData_201906]  
				--   SET  lastCleanTime=GETDATE(),MKTime=@lastCleanTime,UPDTime=GETDATE(),lastRentTimes=@tmpRentTimes,lastOpt=@UserID
				--   WHERE CarNo=@CarNo
				--END
				--UPDATE [iRent_BackEnd].[dbo].[TB_CarCleanLog_201906]
				--SET OrderStatus=1,bookingStart=GETDATE(),lastCleanTime=GETDATE(),lastRentTimes=@tmpRentTimes
				--WHERE OrderNum=@OrderNum AND OrderStatus=0;
				UPDATE [dbo].[TB_CarCleanLog]
				SET OrderStatus=1,bookingStart=@NowTime
				WHERE OrderNum=@OrderNum AND OrderStatus=0;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_CleanCarStart';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_CleanCarStart';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'整備人員取車', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_CleanCarStart';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_CleanCarStart';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_CleanCarStart';