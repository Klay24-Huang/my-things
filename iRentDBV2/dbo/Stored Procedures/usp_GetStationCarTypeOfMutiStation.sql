/****************************************************************
** Name: [dbo].[usp_GetStationCarTypeOfMutiStation]
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
** EXEC @Error=[dbo].[usp_VerifyEMail]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:eason 
** Date:2020/10/14 上午 04:00:00 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/14 下午 04:00:00    |  eason|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetStationCarTypeOfMutiStation]
	@StationIDs             VARCHAR(MAX)          , --據點代碼（1~多個）
	@SD                     DATETIME		      , --起日
	@ED                     DATETIME              , --迄日
	@CarType                VARCHAR(10)           , --車型群組代碼
	@LogID                  BIGINT                , --               ,
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
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetStationCarTypeOfMutiStation';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

	BEGIN TRY     

	   IF @StationIDs IS NULL OR @StationIDs = ''
	   BEGIN
		   SET @Error=1
		   SET @ErrorMsg = 'StationIDs必填'
	   END

	   IF @SD IS NULL 
	   BEGIN
		   SET @Error=1
		   SET @ErrorMsg = 'SD必填'
	   END

	   IF @ED IS NULL 
	   BEGIN
		   SET @Error=1
		   SET @ErrorMsg = 'ED必填'
	   END

	   IF @LogID IS NULL OR @LogID = ''
	   BEGIN
		   SET @Error=1
		   SET @ErrorMsg = 'LogID必填'
	   END

	   DECLARE @tb_StationID TABLE (StationID varchar(max))
	   DECLARE @tb_StationID_Count int = 0
	   IF @Error = 0
	   BEGIN
	        DECLARE @returnList TABLE ([Name] [nvarchar] (max))
			DECLARE @stringToSplit VARCHAR(MAX) = @StationIDs		
			DECLARE @name NVARCHAR(max)
			DECLARE @pos INT
			WHILE CHARINDEX(',', @stringToSplit) > 0
			BEGIN
			    SELECT @pos  = CHARINDEX(',', @stringToSplit)  
			    SELECT @name = SUBSTRING(@stringToSplit, 1, @pos-1)
			    INSERT INTO @returnList 
			    SELECT @name
			    SELECT @stringToSplit = SUBSTRING(@stringToSplit, @pos+1, LEN(@stringToSplit)-@pos)
			END
			INSERT INTO @returnList
			SELECT @stringToSplit 

			DECLARE @re_Count INT = 0
			SELECT @re_Count = COUNT(*) FROM @returnList r WHERE r.Name IS NOT NULL AND r.Name <> ''			
			
			IF @re_Count > 0
			BEGIN
				INSERT INTO @tb_StationID
				SELECT r.Name FROM @returnList r WHERE r.Name IS NOT NULL AND r.Name <> ''
			END

			SELECT @tb_StationID_Count = COUNT(*) FROM @tb_StationID 
			IF  @tb_StationID_Count = 0
			BEGIN
				SET @Error=1
				SET @ErrorMsg = 'StationIDs至少要有1筆'
			END
	   END 

        --查詢
		IF @Error = 0
		BEGIN
		   IF @CarType IS NULL OR @CarType = ''
		   BEGIN
				SELECT PROJID,
					   PRONAME,
					   Price,
					   PRICE_H,
					   [CarBrend],
					   [CarTypeGroupCode] AS CarType,
					   [CarTypeName],
					   [CarTypeImg] AS CarTypePic,
					   OperatorICon AS Operator,
					   Score AS OperatorScore,
					   Seat,
					   iRentStation.StationID,
					   iRentStation.ADDR,
					   iRentStation.Location AS StationName,
					   iRentStation.Longitude,
					   iRentStation.Latitude,
					   iRentStation.Content,
					   PayMode,
					   Car.CarOfArea
				FROM VW_GetFullProjectCollectionOfCarTypeGroup AS VW
				INNER JOIN TB_Car AS Car ON Car.CarType=VW.CarType
				AND CarNo NOT IN
				  (SELECT CarNo
				   FROM [TB_OrderMain]
				   WHERE (booking_status<5
						  AND car_mgt_status<16
						  AND cancel_status=0)
					 AND CarNo in
					   (SELECT [CarNo]
						FROM [dbo].[TB_Car]
						WHERE nowStationID IN (SELECT s.StationID FROM  @tb_StationID s)
						  AND CarType IN
							(SELECT CarType
							 FROM VW_GetFullProjectCollectionOfCarTypeGroup
							 WHERE StationID IN (SELECT s.StationID FROM  @tb_StationID s) )
						  AND available<2 )
					 AND ((start_time BETWEEN @SD AND @ED)
						  OR (stop_time BETWEEN @SD AND @ED)
						  OR (@SD BETWEEN start_time AND stop_time)
						  OR (@ED BETWEEN start_time AND stop_time)
						  OR (DATEADD(MINUTE, -30, @SD) BETWEEN start_time AND stop_time)
						  OR (DATEADD(MINUTE, 30, @ED) BETWEEN start_time AND stop_time)) )
				LEFT JOIN TB_iRentStation AS iRentStation ON iRentStation.StationID=VW.StationID
				WHERE VW.StationID IN (SELECT s.StationID FROM  @tb_StationID s)
				AND SPCLOCK='Z' 
				GROUP BY PROJID,PRONAME,Price,PRICE_H,[CarBrend],[CarTypeGroupCode] ,[CarTypeName],[CarTypeImg] ,OperatorICon ,Score ,Seat,iRentStation.StationID,iRentStation.ADDR,iRentStation.Location ,iRentStation.Longitude ,iRentStation.Latitude,iRentStation.Content,PayMode,CarOfArea
				ORDER BY Price,PRICE_H ASC
		   END
		   ELSE
		   BEGIN
				SELECT PROJID,
					   PRONAME,
					   Price,
					   PRICE_H,
					   [CarBrend],
					   [CarTypeGroupCode] AS CarType,
					   [CarTypeName],
					   [CarTypeImg] AS CarTypePic,
					   OperatorICon AS Operator,
					   Score AS OperatorScore,
					   Seat,
					   iRentStation.StationID,
					   iRentStation.ADDR,
					   iRentStation.Location AS StationName,
					   iRentStation.Longitude,
					   iRentStation.Latitude,
					   iRentStation.Content,
					   PayMode,
					   Car.CarOfArea
				FROM VW_GetFullProjectCollectionOfCarTypeGroup AS VW
				INNER JOIN TB_Car AS Car ON Car.CarType=VW.CarType
				AND CarNo NOT IN
				  (SELECT CarNo
				   FROM [TB_OrderMain]
				   WHERE (booking_status<5
						  AND car_mgt_status<16
						  AND cancel_status=0)
					 AND CarNo in
					   (SELECT [CarNo]
						FROM [dbo].[TB_Car]
						WHERE nowStationID IN (SELECT s.StationID FROM  @tb_StationID s)
						  AND CarType IN
							(SELECT CarType
							 FROM VW_GetFullProjectCollectionOfCarTypeGroup
							 WHERE StationID IN (SELECT s.StationID FROM  @tb_StationID s) )
						  AND available<2 )
					 AND ((start_time BETWEEN @SD AND @ED)
						  OR (stop_time BETWEEN @SD AND @ED)
						  OR (@SD BETWEEN start_time AND stop_time)
						  OR (@ED BETWEEN start_time AND stop_time)
						  OR (DATEADD(MINUTE, -30, @SD) BETWEEN start_time AND stop_time)
						  OR (DATEADD(MINUTE, 30, @ED) BETWEEN start_time AND stop_time)) )
				LEFT JOIN TB_iRentStation AS iRentStation ON iRentStation.StationID=VW.StationID
				WHERE VW.StationID IN (SELECT s.StationID FROM  @tb_StationID s)
				AND SPCLOCK='Z' 
                AND VW.CarTypeGroupCode=@CarType --過濾CarType
				GROUP BY PROJID,PRONAME,Price,PRICE_H,[CarBrend],[CarTypeGroupCode] ,[CarTypeName],[CarTypeImg] ,OperatorICon ,Score ,Seat,iRentStation.StationID,iRentStation.ADDR,iRentStation.Location ,iRentStation.Longitude ,iRentStation.Latitude,iRentStation.Content,PayMode,CarOfArea
				ORDER BY Price,PRICE_H ASC
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetStationCarTypeOfMutiStation';
GO


