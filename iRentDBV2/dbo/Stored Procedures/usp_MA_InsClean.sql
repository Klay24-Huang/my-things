/****************************************************************
** Name: [dbo].[usp_MA_InsClean]
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
** EXEC @Error=[dbo].[usp_MA_InsClean]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/27 上午 08:16:31 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/27 上午 08:16:31    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_MA_InsClean]
	@manager                VARCHAR(20)           ,
	@CarNo                  VARCHAR(10)           ,
	@SD                     DATETIME              ,
	@ED                     DATETIME              ,
	@SpecCode               VARCHAR(10)           ,
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
DECLARE @NowTime DATETIME;
DECLARE @StationID VARCHAR(10);
DECLARE @OrderNum BIGINT;
DECLARE @LogName NVARCHAR(100);
DECLARE @SpecNum INT;
DECLARE @tmpIDNo     VARCHAR(10);
DECLARE @tmpOrderNum VARCHAR(20);
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_MA_InsClean';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @tmpOrderNum='';
SET @OrderNum=0;
SET @manager=ISNULL (@manager,'');
SET @CarNo=ISNULL (@CarNo,'');
SET @SD=ISNULL(@SD,'1911-01-01 00:00:00');
SET @ED=ISNULL(@ED,'1911-01-01 00:00:00');
SET @LogName = '';
SET @SpecNum=0;

BEGIN TRY
	IF @SpecCode = '1'
	BEGIN
		SET @LogName = '【'+@manager+'】後台新增清潔合約';
		SET @SpecNum = 1;
	END
	IF @SpecCode = '2'
	BEGIN
		SET @LogName = '【'+@manager+'】後台新增保修合約';
		SET @SpecNum = 2;
	END
	IF @SpecCode = '3'
	BEGIN
		SET @LogName = '【'+@manager+'】短租透過API新增調撥合約';
		SET @SpecNum = 3;
	END
	ELSE IF @SpecCode='4'
	BEGIN
		SET @LogName = '【'+@manager+'】整備人員網站新增合約';
		SET @SpecNum = 4;
	END

	IF @manager='' 
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		
	--一個帳號只能有30筆訂單，每筆訂單只能三小時的時間
	IF @Error=0
	BEGIN
		IF @SpecCode='4'
		BEGIN
			SELECT @hasData=COUNT(*) FROM TB_OrderMain WITH(NOLOCK) 
			WHERE IDNO=@manager AND cancel_status=0 AND car_mgt_status < 16
			AND start_time > dateadd(day,-7,dbo.GET_TWDATE())

			IF @hasData > 30
			BEGIN
				SET @Error=-1
				SET @ErrorCode = 'ERR804'
			END

			IF DATEDIFF(MINUTE,@SD,@ED) > 180
			BEGIN
				SET @Error=-1
				SET @ErrorCode = 'ERR805'
			END
		END
	END

	IF @Error=0
	BEGIN
		SET @tmpIDNo=@manager;
		SELECT @StationID=nowStationID   FROM TB_Car WITH(NOLOCK) WHERE CarNo=@CarNo; --取得車輛目前所在據點
		--查詢後面是否有預約
		BEGIN TRAN
		DECLARE @tmpCount2 INT;
		SET @tmpCount2=0;
		SELECT @tmpCount2=COUNT(IDNO) 
		FROM TB_OrderMain WITH(NOLOCK)
		WHERE (CarNo=@CarNo AND ((car_mgt_status>=0 AND car_mgt_status<15) AND booking_status<5 AND cancel_status=0))
		AND (
				(start_time between @SD AND @ED ) OR 
				(stop_time between @SD AND @ED) OR 
				(@SD BETWEEN start_time AND stop_time) OR 
				(@ED BETWEEN start_time AND stop_time) OR 
				(DATEADD(MINUTE,-30,@SD) between start_time AND stop_time) OR 
				(DATEADD(MINUTE,30,@ED) between start_time AND stop_time)
			);
		IF @tmpCount2=0
		BEGIN
			INSERT INTO TB_OrderMain (IDNO,CarNo,lend_place,return_place,start_time,stop_time,booking_status,spec_status,stop_pick_time)
			VALUES(@manager,@CarNo,@StationID,@StationID,@SD,@ED,1,@SpecNum,DATEADD(minute,5,@SD));

			SET @OrderNum=@@IDENTITY;
			INSERT INTO TB_OrderHistory(OrderNum,cancel_status,car_mgt_status,booking_status,Descript)VALUES(@OrderNum,0,0,0,@LogName);

			--寫入短租060
			INSERT INTO TB_BookingControl( [PROCD],[order_number],[ODCUSTID],[ODCUSTNM],[TEL1],[TEL2],[TEL3]
											,[ODDATE],[GIVEDATE],[GIVETIME],[RNTDATE],[RNTTIME],[CARTYPE]
											,[CARNO],[OUTBRNH],[INBRNH],[ORDAMT],[REMARK],[RPRICE],[RNTAMT]
											,[PROJTYPE],[INVKIND],[INVTITLE],[UNIMNO]
											,[TSEQNO],[isRetry])
			SELECT 'A', OrderNo,[ODCUSTID],[ODCUSTID],'','',''
					,[ODDATE],[GIVEDATE],[GIVETIME],[RNTDATE],[RNTTIME],[CarType]
					,[CARNO],[OUTBRNH],[INBRNH],[ORDAMT],N'iRent單號【'+CONVERT(VARCHAR(20),@OrderNum)+'】',0 ,0
					,'',2 ,[INVTITLE],[UNIMNO]
					,[TSEQNO],1
			FROM VW_BE_GetBookingControlData WITH(NOLOCK)
			WHERE OrderNo=@OrderNum;

			--如果超過8小時的話 寫入告警
			IF DATEDIFF(HOUR,@SD,@ED) >= 8
			BEGIN
				INSERT INTO [dbo].[TB_EventHandle](EventType,MachineNo,CarNo,Remark) VALUES(20,'', @CarNo, @OrderNum);
			END

			DECLARE @lastCleanTime DATETIME;
			DECLARE @lastRentTime INT;
			DECLARE @UnCleanTimes INT;
			SET @UnCleanTimes=0;
			SET @lastRentTime=0;
			SET @lastCleanTime=GETDATE();
			SELECT @lastCleanTime=ISNULL(lastCleanTime,GETDATE()) FROM TB_CarCleanData WITH(NOLOCK) WHERE CarNo=@CarNo;
			SELECT @lastRentTime=ISNULL(RentCount,0),@UnCleanTimes=ISNULL(UncleanCount,0) FROM TB_CarInfo WITH(NOLOCK) WHERE CarNo=@CarNo
			
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM [dbo].[TB_iRentClearControl] WHERE CarNo=@CarNo
			IF @hasData=0
			BEGIN
				INSERT INTO  [dbo].[TB_iRentClearControl]([CarNo],[RentCount],[UnCleanCount])VALUES(@CarNo,@lastRentTime,@UnCleanTimes)
			END
			ELSE
			BEGIN
				UPDATE [dbo].[TB_iRentClearControl]
				SET [RentCount]=@lastRentTime,
					[UnCleanCount]=@UnCleanTimes
				WHERE CarNo=@CarNo
			END

			INSERT INTO [TB_CarCleanLog]([OrderNum],[UserID],lastCleanTime,lastRentTimes)VALUES(@OrderNum,@manager,@lastCleanTime,@lastRentTime);
			
			COMMIT TRAN;
		END
		ELSE
		BEGIN
			ROLLBACK TRAN;
			SET @Error=1;
			SET @ErrorCode='ERR801'
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_InsClean';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_InsClean';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'新增保修清潔合約', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_InsClean';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_InsClean';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MA_InsClean';