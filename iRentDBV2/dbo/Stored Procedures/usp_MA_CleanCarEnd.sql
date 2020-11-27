/****************************************************************
** Name: [dbo].[usp_MA_CleanCarEnd]
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
** EXEC @Error=[dbo].[usp_MA_CleanCarEnd]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/27 上午 11:25:03 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/27 上午 11:25:03    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_MA_CleanCarEnd]
	@UserID                 VARCHAR(20)          ,
	@CarNo                  VARCHAR(10)          ,
	@OrderNum               VARCHAR(20)          ,
	@outsideClean           TINYINT              ,
	@insideClean            TINYINT              ,
	@rescue					TINYINT              ,
	@dispatch				TINYINT              ,
	@Anydispatch			TINYINT              ,
	@Maintenance            TINYINT              ,
	@remark                 NVARCHAR(1024)       ,
	@incarPic               VARCHAR(max)         ,
	@outcarPic              VARCHAR(max)         ,
	@incarPicType           VARCHAR(50)          ,
	@outcarPicType          VARCHAR(50)          ,
	@isCar                  INT              ,
	@LogID                  BIGINT                ,
	@CID                    VARCHAR(10)     OUTPUT,
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
DECLARE @hasDataNew TINYINT;
DECLARE @tmpRentTimes INT;
DECLARE @lastCleanTime DATETIME;
DECLARE @nowMaintainMilage INT;
DECLARE @tmpIDNo     VARCHAR(10);
DECLARE @tmpOrderNum VARCHAR(20);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_MA_CleanCarEnd';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @tmpOrderNum='';
SET @UserID    =ISNULL (@UserID    ,'');
SET @CarNo=ISNULL (@CarNo,'');
SET @OrderNum=ISNULL (@OrderNum,0);
SET @outsideClean=ISNULL(@outsideClean,0);  
SET @insideClean=ISNULL(@insideClean,0);   
SET @rescue=ISNULL(@rescue,0);
SET @dispatch=ISNULL(@dispatch,0);	
SET @Anydispatch=ISNULL(@Anydispatch,0);	
SET @Maintenance=ISNULL(@Maintenance,0);	
SET @nowMaintainMilage=0;
SET @isCar=ISNULL(@isCar,0);	
SET @hasData=0;
SET @CID='';
SET @tmpRentTimes=0;
SET @hasDataNew=0;


		BEGIN TRY

		 
		 IF @UserID='' OR @CarNo='' OR @OrderNum=0 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.儲存目前里程
		 IF @Error=0
		 BEGIN
			     SELECT @nowMaintainMilage=ISNULL([Millage],0) FROM TB_CarStatus WITH(NOLOCK) WHERE CarNo=@CarNo;
		 END
		 IF @Error=0
		 BEGIN
		    SELECT @hasData=1 FROM[dbo].[TB_CarCleanLog] WITH(NOLOCK) WHERE OrderNum=@OrderNum AND OrderStatus=1;
			IF @hasData=0
			BEGIN
			   SET @Error=1;
			   SET @ErrorCode='ERR830';
			   SET @ErrorMsg='找不到符合的清潔租約';
			END
			ELSE
			BEGIN
			   SET @hasData=0;
			   --SELECT @tmpRentTimes=ISNULL(UncleanCount,0) FROM  [iRent_BackEnd].[dbo].[TB_CarInfo_201609] WITH(NOLOCK) WHERE CarNo=@CarNo;
			    SELECT @tmpRentTimes=ISNULL(UncleanCount,0) FROM [TB_iRentClearControl] WITH(NOLOCK) WHERE CarNo=@CarNo;

				--
				IF @rescue=1 OR @dispatch=1
				BEGIN
				  UPDATE [TB_iRentClearControl] SET [LastRentTime]=GETDATE() WHERE CarNo=@CarNo;
				END

				SELECT @hasData=1,@lastCleanTime=ISNULL(lastCleanTime,'2019-07-31 00:00:00') FROM [dbo].[TB_CarCleanData] WITH(NOLOCK) WHERE CarNo=@CarNo;
			--	SELECT @hasDataNew=1,@lastCleanTime=ISNULL(LastCleanTime,'2019-07-31 00:00:00') FROM [iRent_BackEnd].[dbo].[TB_iRentClearControl_202003] WITH(NOLOCK) WHERE CarNo=@CarNo;
				IF @hasData=0
				BEGIN
				   INSERT INTO [dbo].[TB_CarCleanData] (CarNo,lastCleanTime,lastRentTimes,lastOpt,UPDTime)VALUES(@CarNo,GETDATE(),@tmpRentTimes,@UserID,GETDATE());
				END
				ELSE
				BEGIN
				   UPDATE  [dbo].[TB_CarCleanData]  
				   SET  lastCleanTime=GETDATE(),MKTime=@lastCleanTime,UPDTime=GETDATE(),lastRentTimes=@tmpRentTimes,lastOpt=@UserID
				   WHERE CarNo=@CarNo
				END

			   UPDATE [dbo].[TB_CarCleanLog]
			   SET OrderStatus=2,outsideClean=@outsideClean,insideClean=@insideClean,rescue=@rescue
				  ,dispatch=@dispatch,Anydispatch=@Anydispatch,remark=@remark
				  ,incarPic=@incarPic,outcarPic=@outcarPic,bookingEnd=GETDATE()
				  ,incarPicType=@incarPicType,outcarPicType=@outcarPicType
				  ,Maintenance=@Maintenance
			   WHERE OrderNum=@OrderNum AND OrderStatus=1

			   IF @insideClean=1 OR @outsideClean=1
			   BEGIN
					UPDATE  [dbo].[TB_CarInfo]
					SET [UncleanCount]=0,RentCount=0
					WHERE CarNo=@CarNo;

					UPDATE [dbo].[TB_iRentClearControl]
					SET UnCleanCount=0,RentCount=0,LastCleanTime=GETDATE()
					WHERE CarNo=@CarNo
			   END
			   IF @Maintenance=1
			   BEGIN
			
					UPDATE [dbo].[TB_iRentClearControl]
					SET LastMaintenanceTime=GETDATE(),LastMaintenanceMilage=@nowMaintainMilage,LastMaintenanceOrderNo=@OrderNum
					WHERE CarNo=@CarNo;
					
			   END
			   
			   SELECT @CID=ISNULL(CID,'') FROM TB_CarInfo AS CarInfo WITH(NOLOCK)
			
			    WHERE CarNo=@CarNo
				
				UPDATE [dbo].[TB_OrderMain]
				SET cancel_status=5
				WHERE order_number=@OrderNum;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_CleanCarEnd';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_CleanCarEnd';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'整備人員還車', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_CleanCarEnd';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_CleanCarEnd';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_CleanCarEnd';