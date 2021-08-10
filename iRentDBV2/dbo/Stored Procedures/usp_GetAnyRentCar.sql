/****************************************************************
** 用途：取得路邊汽車
*****************************************************************
** Change History
*****************************************************************
** 2020/12/08 ADD BY YEH
** 2021/06/22 UPD BY YEH REASON:積分<60分只能用定價專案
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetAnyRentCar]
	@IDNO				VARCHAR(10)				,	--帳號
	@ShowALL			INT						,	--是否顯示全部(0:否 1:是)
	@MinLatitude		FLOAT					,	--最小緯度
	@MinLongitude		FLOAT					,	--最小經度
	@MaxLatitude		FLOAT					,	--最大緯度
	@MaxLongitude		FLOAT					,	--最大經度
	@LogID				BIGINT					,
	@ErrorCode			VARCHAR(6)		OUTPUT	,	--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowDate DATETIME;	-- 系統時間
DECLARE @SpecStatus VARCHAR(2);	-- 特殊身分
DECLARE @Score INT;	-- 會員積分

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetAnyRentCar';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowDate=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @MinLatitude=ISNULL(@MinLatitude,0);
SET @MinLongitude=ISNULL(@MinLongitude,0);
SET @MaxLatitude=ISNULL(@MaxLatitude,0);
SET @MaxLongitude=ISNULL(@MaxLongitude,0);
SET @SpecStatus='';
SET @Score=100;	-- 預設積分100

BEGIN TRY
	IF @Error=0
	BEGIN
		DROP TABLE IF EXISTS #TB_Project

		-- 取得路邊汽車專案
		SELECT * INTO #TB_Project 
		FROM TB_Project WITH(NOLOCK) 
		WHERE PROJTYPE=3
		AND @NowDate BETWEEN ShowStart AND ShowEnd;

		-- 取得會員積分
		SELECT @Score=SCORE FROM TB_MemberScoreMain WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

		IF @Score < 60	-- 積分<60分只能用定價專案
		BEGIN
			SET @SpecStatus = '90';	-- 定價專用特殊身分
			-- 將#TB_Project的非定價專案都移除
			DELETE FROM #TB_Project WHERE SPCLOCK<>@SpecStatus;
		END

		IF @ShowALL=1	--是否顯示全部(0:否 1:是)
		BEGIN
			SELECT C.CarNo,
				CG.CarTypeGroupCode AS CarType,
				CONCAT(CT.CarBrend, ' ', CT.CarTypeName) AS CarTypeName,
				S.Area AS CarOfArea,
				P.PRONAME AS ProjectName,
				CASE WHEN H.HolidayDate IS NOT NULL THEN PD.PRICE_H / 10 ELSE PD.PRICE / 10 END AS Rental,
				M.MilageBase AS Mileage,
				0 AS ShowSpecial,
				'' AS SpecialInfo,
				CS.Latitude,
				CS.Longitude,
				O.OperatorICon AS Operator,
				O.Score AS OperatorScore,
				CG.CarTypeImg AS CarTypePic,
				CG.Seat,
				P.PROJID,
				CASE WHEN ISNULL(BU.InsuranceLevel, 3) >= 4 THEN 0 ELSE 1 END AS Insurance,
				InsurancePrice=CASE WHEN K.InsurancePerHours IS NOT NULL THEN K.InsurancePerHours ELSE II.InsurancePerHours END
			FROM #TB_Project AS P WITH(NOLOCK)
			LEFT JOIN TB_ProjectStation AS PS WITH(NOLOCK) ON PS.PROJID=P.PROJID AND PS.IOType='O'
			LEFT JOIN TB_ProjectDiscount AS PD WITH(NOLOCK) ON PD.ProjID=PS.PROJID AND PD.ProjID=P.PROJID
			LEFT JOIN TB_CarType AS CT WITH(NOLOCK) ON CT.CarType=PD.CARTYPE
			INNER JOIN TB_Car AS C WITH (NOLOCK) ON C.CarType=CT.CarType AND C.nowStationID=PS.StationID
			INNER JOIN TB_CarStatus AS CS WITH (NOLOCK)ON CS.CarNo=C.CarNo
			INNER JOIN dbo.TB_CarTypeGroupConsist AS CGC WITH (NOLOCK) ON CGC.CarType=C.CarType
			INNER JOIN dbo.TB_CarTypeGroup AS CG WITH (NOLOCK) ON CG.CarTypeGroupID=CGC.CarTypeGroupID
			INNER JOIN dbo.TB_OperatorBase AS O WITH (NOLOCK) ON O.OperatorID=C.Operator
			LEFT JOIN dbo.TB_iRentStation AS S WITH (NOLOCK) ON S.StationID=C.nowStationID
			LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=@IDNO
			LEFT JOIN TB_InsuranceInfo K WITH(NOLOCK) ON K.CarTypeGroupCode=CG.CarTypeGroupCode AND K.useflg='Y' AND BU.InsuranceLevel=K.InsuranceLevel	
			LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=CG.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3		--預設專用
			LEFT JOIN TB_MilageSetting M WITH(NOLOCK) ON P.PROJID=M.ProjID
			LEFT JOIN TB_Holiday H WITH(NOLOCK) ON H.HolidayDate=CONVERT(VARCHAR,dbo.GET_TWDATE(),112) AND H.use_flag=1
			WHERE CS.GPSTime>=DATEADD(MINUTE, -30, dbo.GET_TWDATE())
			AND available=1
			AND ((P.SPCLOCK='Z') OR (@SpecStatus<>'' AND P.SPCLOCK=@SpecStatus))
			AND @NowDate BETWEEN P.ShowStart AND P.ShowEnd
			AND C.CarNo NOT IN (SELECT CarNo FROM TB_OrderMain WITH(NOLOCK) WHERE car_mgt_status < 16 AND cancel_status = 0 AND booking_status<5 
									AND (start_time > DATEADD(dAY,-7,dbo.GET_TWDATE()) OR  stop_pick_time > dbo.GET_TWDATE()) );
		END
		ELSE
		BEGIN
			SELECT C.CarNo,
				CG.CarTypeGroupCode AS CarType,
				CONCAT(CT.CarBrend, ' ', CT.CarTypeName) AS CarTypeName,
				S.Area AS CarOfArea,
				P.PRONAME AS ProjectName,
				CASE WHEN H.HolidayDate IS NOT NULL THEN PD.PRICE_H / 10 ELSE PD.PRICE / 10 END AS Rental,
				M.MilageBase AS Mileage,
				0 AS ShowSpecial,
				'' AS SpecialInfo,
				CS.Latitude,
				CS.Longitude,
				O.OperatorICon AS Operator,
				O.Score AS OperatorScore,
				CG.CarTypeImg AS CarTypePic,
				CG.Seat,
				P.PROJID,
				CASE WHEN ISNULL(BU.InsuranceLevel, 3) >= 4 THEN 0 ELSE 1 END AS Insurance,
				InsurancePrice=CASE WHEN K.InsurancePerHours IS NOT NULL THEN K.InsurancePerHours ELSE II.InsurancePerHours END
			FROM #TB_Project AS P WITH(NOLOCK)
			LEFT JOIN TB_ProjectStation AS PS WITH(NOLOCK) ON PS.PROJID=P.PROJID AND PS.IOType='O'
			LEFT JOIN TB_ProjectDiscount AS PD WITH(NOLOCK) ON PD.ProjID=PS.PROJID AND PD.ProjID=P.PROJID
			LEFT JOIN TB_CarType AS CT WITH(NOLOCK) ON CT.CarType=PD.CARTYPE
			INNER JOIN TB_Car AS C WITH (NOLOCK) ON C.CarType=CT.CarType AND C.nowStationID=PS.StationID
			INNER JOIN TB_CarStatus AS CS WITH (NOLOCK)ON CS.CarNo=C.CarNo
			INNER JOIN dbo.TB_CarTypeGroupConsist AS CGC WITH (NOLOCK) ON CGC.CarType=C.CarType
			INNER JOIN dbo.TB_CarTypeGroup AS CG WITH (NOLOCK) ON CG.CarTypeGroupID=CGC.CarTypeGroupID
			INNER JOIN dbo.TB_OperatorBase AS O WITH (NOLOCK) ON O.OperatorID=C.Operator
			LEFT JOIN dbo.TB_iRentStation AS S WITH (NOLOCK) ON S.StationID=C.nowStationID
			LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=@IDNO
			LEFT JOIN TB_InsuranceInfo K WITH(NOLOCK) ON K.CarTypeGroupCode=CG.CarTypeGroupCode AND K.useflg='Y' AND BU.InsuranceLevel=K.InsuranceLevel	
			LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=CG.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3		--預設專用
			LEFT JOIN TB_MilageSetting M WITH(NOLOCK) ON P.PROJID=M.ProjID
			LEFT JOIN TB_Holiday H WITH(NOLOCK) ON H.HolidayDate=CONVERT(VARCHAR,dbo.GET_TWDATE(),112) AND H.use_flag=1
			WHERE CS.GPSTime>=DATEADD(MINUTE, -30, dbo.GET_TWDATE())
			AND available=1
			AND ((P.SPCLOCK='Z') OR (@SpecStatus<>'' AND P.SPCLOCK=@SpecStatus))
			AND @NowDate BETWEEN P.ShowStart AND P.ShowEnd
			AND C.CarNo NOT IN (SELECT CarNo FROM TB_OrderMain WITH(NOLOCK) WHERE car_mgt_status < 16 AND cancel_status = 0 AND booking_status<5 
									AND (start_time > DATEADD(dAY,-7,dbo.GET_TWDATE()) OR  stop_pick_time > dbo.GET_TWDATE()) )
			AND (CS.Latitude>=@MinLatitude AND CS.Latitude<=@MaxLatitude)
			AND (CS.Longitude>=@MinLongitude AND CS.Longitude<=@MaxLongitude);
		END

		DROP TABLE IF EXISTS #TB_Project
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetAnyRentCar_ForTest';
GO