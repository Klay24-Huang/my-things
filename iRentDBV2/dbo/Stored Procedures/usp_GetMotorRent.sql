/****************************************************************
** 用途：取得路邊機車
*****************************************************************
** Change History
*****************************************************************
** 2021/06/21 ADD BY YEH REASON:新增SP
*****************************************************************/

CREATE PROCEDURE [dbo].[usp_GetMotorRent]
	@IDNO					VARCHAR(10)				,	-- 帳號
	@ShowALL				INT						,	-- 是否顯示全部(0:否/1:是)
	@Latitude				FLOAT					,	-- 緯度
	@Longitude				FLOAT					,	-- 經度
	@Radius					FLOAT					,	-- 半徑
	@LogID                  BIGINT					,	--
	@ErrorCode 				VARCHAR(6)		OUTPUT	,	-- 回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT	,	-- 回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT	,	-- 回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT		-- 回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowDate DATETIME;	-- 系統時間
DECLARE @SpecStatus VARCHAR(2);	-- 特殊身分
DECLARE @Score INT;	-- 會員積分
DECLARE @MinLat FLOAT;	-- 最小緯度
DECLARE @MinLng FLOAT;	-- 最小經度
DECLARE @MaxLat FLOAT;	-- 最大緯度
DECLARE @MaxLng FLOAT;	-- 最大經度

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetMotorRent';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowDate=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @SpecStatus='';
SET @Score=100;	-- 預設積分100
SET @Latitude=ISNULL(@Latitude,0);
SET @Longitude=ISNULL(@Longitude,0);
SET @Radius=ISNULL(@Radius,0);

BEGIN TRY
	DROP TABLE IF EXISTS #TB_Project

	IF @Error = 0
	BEGIN
		-- 取得特殊身分
		SELECT @SpecStatus=ISNULL(SPECSTATUS,'') FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND CONVERT(VARCHAR,dbo.GET_TWDATE(),112) BETWEEN SPSD AND SPED;

		-- 取得機車專案
		SELECT * INTO #TB_Project 
		FROM TB_Project WITH(NOLOCK) 
		WHERE PROJTYPE=4
		AND @NowDate BETWEEN ShowStart AND ShowEnd;

		-- 取得會員積分
		SELECT @Score=SCORE FROM TB_MemberScoreMain WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

		IF @Score < 60	-- 積分<60分只能用定價專案
		BEGIN
			SET @SpecStatus = '90';	-- 定價專用特殊身分
			-- 將#TB_Project的非定價專案都移除
			DELETE FROM #TB_Project WHERE SPCLOCK<>@SpecStatus;
		END

		IF @ShowALL = 0
		BEGIN
			-- 依照經緯度&半徑取得最大/小範圍的經緯度
			SELECT TOP 1 @MinLat = minLat, @MinLng = minLng , @MaxLat = maxLat, @MaxLng = maxLng
			FROM dbo.FN_GetAround(@Latitude,@Longitude, @Radius);

			SELECT C.CarNo,
				CT.CarTypeName AS CarType,
				CONCAT(CT.CarBrend,' ',CT.CarTypeName) AS CarTypeName,
				S.Area AS CarOfArea,
				P.PRONAME AS ProjectName,
				PD.PRICE AS Rental,
				2.5 AS Mileage,
				0 AS Insurance,
				0 As InsurancePrice,
				0 As ShowSpecial,
				'' As SpecialInfo,
				CS.Latitude,
				CS.Longitude,
				CAST(CS.device3TBA as INT) AS 'Power',
				CAST(CONVERT(FLOAT,CS.deviceRDistance) as INT) AS RemainingMileage,
				O.OperatorICon As Operator,
				O.Score As OperatorScore,
				P.PROJID As ProjID,
				PM.BaseMinutes,
				PM.BaseMinutesPrice As BasePrice,
				PM.Price AS PerMinutesPrice
			FROM #TB_Project AS P WITH(NOLOCK)
			LEFT JOIN TB_ProjectStation AS PS WITH(NOLOCK) ON PS.PROJID=P.PROJID AND PS.IOType='O'
			LEFT JOIN TB_ProjectDiscount AS PD WITH(NOLOCK) ON PD.ProjID=PS.PROJID AND PD.ProjID=P.PROJID
			LEFT JOIN TB_CarType AS CT WITH(NOLOCK) ON CT.CarType=PD.CARTYPE
			INNER JOIN TB_Car AS C WITH (NOLOCK) ON C.CarType=CT.CarType AND C.nowStationID=PS.StationID
			INNER JOIN TB_CarStatus AS CS WITH(NOLOCK) ON CS.CarNo=C.CarNo
			INNER JOIN TB_CarInfo AS CI WITH (NOLOCK) ON CI.CarNo=CS.CarNo AND CI.CarType=PD.CARTYPE
			INNER JOIN TB_PriceByMinutes AS PM WITH (NOLOCK) ON PM.ProjID=P.PROJID AND PM.CarType=C.CarType
			INNER JOIN TB_OperatorBase AS O WITH (NOLOCK) ON O.OperatorID=CT.Operator
			LEFT JOIN TB_iRentStation AS S WITH (NOLOCK) ON S.StationID = C.nowStationID
			WHERE C.available=1
			AND ((P.SPCLOCK='Z') OR (@SpecStatus<>'' AND P.SPCLOCK=@SpecStatus))
			AND CS.GPSTime >= DATEADD(MINUTE,-30,@NowDate)
			AND CS.deviceMBA >= 80
			AND CS.device2TBA >= 20
			AND (CS.Latitude >= @MinLat AND CS.Latitude <= @MaxLat)
			AND (CS.Longitude >= @MinLng AND CS.Longitude <= @MaxLng)
			AND C.CarNo NOT IN (SELECT CarNo FROM TB_OrderMain WITH(NOLOCK)		-- 過濾後台保修
								WHERE cancel_status = 0 AND booking_status=1 AND ProjType=5 
								AND start_time < @NowDate AND stop_time > @NowDate)
			AND C.CarNo NOT IN (SELECT CarNo FROM TB_OrderMain M WITH(NOLOCK)	-- 過濾車輛改為一個月內的訂單
								WHERE car_mgt_status < 16 AND cancel_status = 0 AND booking_status<5 
								AND ProjType=4 AND start_time > DATEADD(MONTH,-1,@NowDate) )
		END
		ELSE
		BEGIN
			SELECT C.CarNo,
				CT.CarTypeName AS CarType,
				CONCAT(CT.CarBrend,' ',CT.CarTypeName) AS CarTypeName,
				S.Area AS CarOfArea,
				P.PRONAME AS ProjectName,
				PD.PRICE AS Rental,
				2.5 AS Mileage,
				0 AS Insurance,
				0 As InsurancePrice,
				0 As ShowSpecial,
				'' As SpecialInfo,
				CS.Latitude,
				CS.Longitude,
				CAST(CS.device3TBA as INT) AS 'Power',
				CAST(CONVERT(FLOAT,CS.deviceRDistance) as INT) AS RemainingMileage,
				O.OperatorICon As Operator,
				O.Score As OperatorScore,
				P.PROJID As ProjID,
				PM.BaseMinutes,
				PM.BaseMinutesPrice As BasePrice,
				PM.Price AS PerMinutesPrice
			FROM #TB_Project AS P WITH(NOLOCK)
			LEFT JOIN TB_ProjectStation AS PS WITH(NOLOCK) ON PS.PROJID=P.PROJID AND PS.IOType='O'
			LEFT JOIN TB_ProjectDiscount AS PD WITH(NOLOCK) ON PD.ProjID=PS.PROJID AND PD.ProjID=P.PROJID
			LEFT JOIN TB_CarType AS CT WITH(NOLOCK) ON CT.CarType=PD.CARTYPE
			INNER JOIN TB_Car AS C WITH (NOLOCK) ON C.CarType=CT.CarType AND C.nowStationID=PS.StationID
			INNER JOIN TB_CarStatus AS CS WITH(NOLOCK) ON CS.CarNo=C.CarNo
			INNER JOIN TB_CarInfo AS CI WITH (NOLOCK) ON CI.CarNo=CS.CarNo AND CI.CarType=PD.CARTYPE
			INNER JOIN TB_PriceByMinutes AS PM WITH (NOLOCK) ON PM.ProjID=P.PROJID AND PM.CarType=C.CarType
			INNER JOIN TB_OperatorBase AS O WITH (NOLOCK) ON O.OperatorID=CT.Operator
			LEFT JOIN TB_iRentStation AS S WITH (NOLOCK) ON S.StationID = C.nowStationID
			WHERE C.available=1
			AND ((P.SPCLOCK='Z') OR (@SpecStatus<>'' AND P.SPCLOCK=@SpecStatus))
			AND CS.GPSTime >= DATEADD(MINUTE,-30,@NowDate)
			AND CS.deviceMBA >= 80
			AND CS.device2TBA >= 20
			AND C.CarNo NOT IN (SELECT CarNo FROM TB_OrderMain WITH(NOLOCK)		-- 過濾後台保修
								WHERE cancel_status = 0 AND booking_status=1 AND ProjType=5 
								AND start_time < @NowDate AND stop_time > @NowDate)
			AND C.CarNo NOT IN (SELECT CarNo FROM TB_OrderMain M WITH(NOLOCK)	-- 過濾車輛改為一個月內的訂單
								WHERE car_mgt_status < 16 AND cancel_status = 0 AND booking_status<5 
								AND ProjType=4 AND start_time > DATEADD(MONTH,-1,@NowDate) )
		END
	END

	DROP TABLE IF EXISTS #TB_Project

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
GO