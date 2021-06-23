/****************************************************************
** 用途：取得路邊汽車專案
*****************************************************************
** Change History
*****************************************************************
** 2021/06/16 ADD BY YEH REASON:新增SP
*****************************************************************/

CREATE PROCEDURE [dbo].[usp_GetAnyRentProject]
	@IDNO					VARCHAR(10)				,	-- 帳號
	@CarNo					VARCHAR(10)				,	-- 車號
	@SD						DATETIME				,	-- 起日
	@ED						DATETIME				,	-- 迄日
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
DECLARE @NowTime DATETIME;
DECLARE @SpecStatus VARCHAR(2);	-- 特殊身分
DECLARE @Score INT;	-- 會員積分

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetAnyRentProject';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(hour,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @SpecStatus='';
SET @Score=100;	-- 預設積分100

BEGIN TRY
	DROP TABLE IF EXISTS #TB_Project

	IF @Error = 0
	BEGIN
		-- 取得特殊身分
		SELECT @SpecStatus=ISNULL(SPECSTATUS,'') FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND CONVERT(VARCHAR,dbo.GET_TWDATE(),112) BETWEEN SPSD AND SPED;

		-- 取得路邊汽車專案
		SELECT * INTO #TB_Project 
		FROM TB_Project WITH(NOLOCK) 
		WHERE PROJTYPE=3
		AND ((ShowStart BETWEEN @SD AND @ED) OR (ShowEnd BETWEEN @SD AND @ED) OR (@SD BETWEEN ShowStart AND ShowEnd) OR (@ED BETWEEN ShowStart AND ShowEnd));

		-- 取得會員積分
		SELECT @Score=SCORE FROM TB_MemberScoreMain WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

		IF @Score < 60	-- 積分<60分只能用定價專案
		BEGIN
			SET @SpecStatus = '90';	-- 定價專用特殊身分
			-- 將#TB_Project的非定價專案都移除
			DELETE FROM #TB_Project WHERE SPCLOCK<>@SpecStatus;
		END

		SELECT P.PROJID,
			P.PRONAME,
			P.PRODESC,
			PD.PRICE,
			PD.PRICE_H,
			ISNULL(C.CarBrend,'') AS CarBrend,
			E.CarTypeGroupCode AS CarType,
			ISNULL(C.CarTypeName,'') AS CarTypeName,
			E.CarTypeImg AS CarTypePic,
			F.OperatorICon AS Operator,
			F.Score AS OperatorScore,
			E.Seat,
			P.PayMode,
			S.ContentForAPP AS Content,
			S.Area As CarOfArea,
			PS.StationID,
			Insurance = CASE WHEN E.isMoto=1 THEN 0 WHEN ISNULL(BU.InsuranceLevel,3) = 6 THEN 0 ELSE 1 END,
			InsurancePerHours = CASE WHEN E.isMoto=1 THEN 0 WHEN K.InsuranceLevel IS NULL THEN II.InsurancePerHours WHEN K.InsuranceLevel < 6 THEN K.InsurancePerHours ELSE 0 END	--安心服務每小時價
		FROM #TB_Project AS P WITH(NOLOCK)
		LEFT JOIN TB_ProjectStation AS PS WITH(NOLOCK) ON PS.PROJID=P.PROJID AND IOType='O'
		LEFT JOIN TB_ProjectDiscount AS PD WITH(NOLOCK) ON PD.ProjID=PS.PROJID AND PD.ProjID=P.PROJID
		LEFT JOIN TB_CarType AS C WITH(NOLOCK) ON C.CarType=PD.CARTYPE
		INNER JOIN TB_CarTypeGroupConsist AS D WITH(NOLOCK) ON C.CarType=D.CarType
		INNER JOIN TB_CarTypeGroup AS E WITH(NOLOCK) ON E.CarTypeGroupID=D.CarTypeGroupID
		INNER JOIN TB_Car AS Car ON Car.CarType=C.CarType
		INNER JOIN TB_OperatorBase AS F WITH(NOLOCK) ON F.OperatorID=C.Operator
		INNER JOIN TB_iRentStation S ON S.StationID = PS.StationID AND PS.StationID=Car.nowStationID AND S.IsNormalStation=3
		LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=@IDNO
		LEFT JOIN TB_InsuranceInfo K WITH(NOLOCK) ON K.CarTypeGroupCode=E.CarTypeGroupCode AND K.useflg='Y' AND BU.InsuranceLevel=K.InsuranceLevel
		LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=E.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3
		WHERE Car.CarNo = @CarNo
		AND ((P.SPCLOCK='Z') OR (@SpecStatus<>'' AND P.SPCLOCK=@SpecStatus))
		ORDER BY PROJID ASC;
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

