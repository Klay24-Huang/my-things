/****************************************************************
** 用途：取得路邊機車專案
*****************************************************************
** Change History
*****************************************************************
** 2021/06/17 ADD BY YEH REASON:新增SP
*****************************************************************/

CREATE PROCEDURE [dbo].[usp_GetMotorRentProject]
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
DECLARE @SpecStatus VARCHAR(2);	-- 特殊身分
DECLARE @Score INT;	-- 會員積分

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetMotorRentProject';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @IDNO=ISNULL(@IDNO,'');
SET @SpecStatus='';
SET @Score=100;	-- 預設積分100

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
			ISNULL(C.CarBrend,'') AS CarBrend,
			E.CarTypeGroupCode AS CarType,
			ISNULL(C.CarTypeName,'') AS CarTypeName,
			E.CarTypeImg AS CarTypePic,
			O.OperatorICon AS Operator,
			O.Score AS OperatorScore,
			E.Seat,
			PM.BaseMinutes,
			PM.BaseMinutesPrice AS BasePrice,
			PM.Price AS PerMinutesPrice,
			PM.MaxPrice,		       
			S.ContentForAPP AS Content,
			CS.device3TBA AS Power,
			ISNULL(CS.deviceRDistance,'') AS RemainingMileage,
			S.Area AS CarOfArea
		FROM #TB_Project AS P WITH(NOLOCK)
		LEFT JOIN TB_ProjectStation AS PS WITH(NOLOCK) ON PS.PROJID=P.PROJID AND IOType='O'
		LEFT JOIN TB_ProjectDiscount AS PD WITH(NOLOCK) ON PD.ProjID=PS.PROJID AND PD.ProjID=P.PROJID
		LEFT JOIN TB_CarType AS C WITH(NOLOCK) ON C.CarType=PD.CARTYPE
		INNER JOIN TB_CarTypeGroupConsist AS D WITH(NOLOCK) ON C.CarType=D.CarType
		INNER JOIN TB_CarTypeGroup AS E WITH(NOLOCK) ON E.CarTypeGroupID=D.CarTypeGroupID
		INNER JOIN TB_OperatorBase AS O WITH(NOLOCK) ON O.OperatorID=C.Operator
		INNER JOIN TB_Car AS Car ON Car.CarType=C.CarType AND PS.StationID=Car.nowStationID           
		INNER JOIN TB_CarStatus AS CS ON CS.CarNo=Car.CarNo            
		INNER JOIN TB_iRentStation S ON S.StationID=PS.StationID AND S.IsNormalStation=4
		INNER JOIN TB_PriceByMinutes AS PM ON PM.ProjID=P.PROJID AND PM.CarType=Car.CarType AND PM.use_flag=1
		WHERE Car.CarNo=@CarNo
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

