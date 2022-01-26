/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_AutoSubscribeJob_I01
* 系    統 : IRENT
* 程式功能 : 訂閱制自動續訂排程-建立月租
* 作    者 : YEH
* 撰寫日期 : 20220121
* 修改日期 : 

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_AutoSubscribeJob_I01]
(   
	@IDNO				VARCHAR(20)				,	-- 身分證號
	@MonthlyRentId		BIGINT					,	-- MonthlyRentId
	@MonProjID			VARCHAR(10)				,	-- 方案代碼
	@MonProPeriod		INT						,	-- 總期數
	@ShortDays			INT						,	-- 短期總天數
	@SubsNxtID			BIGINT					,	-- 續訂檔ID
	@IsMotor			INT						,	-- 是否為機車 (0:汽車 1:機車)
	@NxtMonSetID		INT						,	-- 下期方案ID
	@PayTypeId			INT						,	-- 付款方式
	@InvoTypeId			INT						,	-- 發票設定
	@MerchantTradeNo	VARCHAR(30)				,	-- 台新送出的訂單編號
	@TaishinTradeNo		VARCHAR(50)				,	-- 台新的交易序號
	@LogID				BIGINT					,	-- LogID
	@NewMonthlyRentID	BIGINT			OUTPUT	,	-- 新期數的第一筆MonthlyRentId
	@ErrorCode			VARCHAR(6)		OUTPUT	,	-- 回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	-- 回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	-- 回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		-- 回傳sqlException訊息
)
AS
SET NOCOUNT ON

DECLARE @Error INT;
DECLARE @IsSystem TINYINT = 1;
DECLARE @ErrorType TINYINT = 4;
DECLARE @FunName VARCHAR(50);
DECLARE @NowTime DATETIME;
DECLARE @COUNT INT;
DECLARE @StartDate DATETIME;

/*初始設定*/
SET @Error=0;
SET	@ErrorCode = '0000';
SET	@ErrorMsg = 'SUCCESS';
SET	@SQLExceptionCode = '';
SET	@SQLExceptionMsg = '';
SET @FunName = 'usp_AutoSubscribeJob_I01';
SET @NowTime = DATEADD(HOUR,8,GETDATE());
SET @COUNT = 0 ;
	
SET @IDNO = ISNULL(@IDNO,'');
SET @MonthlyRentId = ISNULL(@MonthlyRentId,0);
SET @MonProjID = ISNULL(@MonProjID,'');
SET @NxtMonSetID = ISNULL(@NxtMonSetID,0);
SET @LogID = ISNULL(@LogID,0);

BEGIN TRY
	BEGIN TRAN

	IF @IDNO = '' OR @MonthlyRentId = 0 OR @NxtMonSetID = 0  
	BEGIN
		SET @Error = 1;
		SET @ErrorMsg = '參數遺漏';
		SET @ErrorCode = 'ERR257';
	END

	IF @Error = 0
	BEGIN
		-- 當期的迄日為要續訂的第一期起日
		SELECT @StartDate = EndDate FROM TB_MonthlyRentUse WITH(NOLOCK) WHERE MonthlyRentId=@MonthlyRentId;

		DROP TABLE IF EXISTS #Set;

		-- 用下期方案ID回設定檔取得專案資訊
		SELECT * 
		INTO #Set
		FROM TB_MonthlyRentSet WITH(NOLOCK)
		WHERE MonSetID=@NxtMonSetID;

		SELECT @COUNT = COUNT(*) FROM #Set;

		IF @COUNT = 0
		BEGIN
			SET @Error = 1;
			SET @ErrorMsg = '要建立的專案不存在';
			SET @ErrorCode = 'ERR261';
		END
	END

	IF @Error = 0 
	BEGIN
		DECLARE @nowCarProjCount int = 0;

		SELECT @nowCarProjCount = COUNT(*)
		FROM TB_MonthlyRentUse WITH(NOLOCK)
		WHERE IDNO = @IDNO AND MonType = 2 AND useFlag = 1 AND Mode = 0
		AND MonthlyRentId <> @MonthlyRentId
		AND @StartDate BETWEEN StartDate and EndDate;

		IF @nowCarProjCount > 0 AND (SELECT COUNT(*) FROM #Set WHERE IsMoto=0) > 0
		BEGIN
			SET @Error=1;
			SET @ErrorMsg = '同時段只能訂閱一個汽車訂閱制月租';
			SET @ErrorCode = 'ERR262';
		END
	END

	IF @Error = 0 
	BEGIN
		DECLARE @nowMotoProjCount int = 0;

		SELECT @nowMotoProjCount = COUNT(*)
		FROM TB_MonthlyRentUse WITH(NOLOCK)
		WHERE IDNO = @IDNO AND MonType = 2 AND useFlag = 1 AND Mode = 1
		AND MonthlyRentId <> @MonthlyRentId
		AND @StartDate BETWEEN StartDate and EndDate;

		IF @nowMotoProjCount > 0 AND (SELECT COUNT(*) FROM #Set WHERE IsMoto=1) > 0
		BEGIN
			SET @Error=1;
			SET @ErrorMsg = '同時段只能訂閱一個機車訂閱制月租';
			SET @ErrorCode = 'ERR263';
		END
	END

	IF @Error = 0
	BEGIN
		DECLARE @Period_Count INT = 0;
		DECLARE @TB_Period TABLE(
			MonProjID VARCHAR(10),
			MonProPeriod INT,
			ShortDays INT,
			MonPeriod INT,
			NxtMonSetID INT,
			SDATE DATETIME,
			EDATE DATETIME
		);

		DECLARE @Period INT = 0;	-- 總期數
		SELECT @Period = MonProPeriod FROM #Set WITH(NOLOCK) WHERE MonSetID=@NxtMonSetID;

		DECLARE @Loop_Count int = 1;
		DECLARE @NextSD DATETIME = @StartDate;
		DECLARE @NextED DATETIME; 

		WHILE @Period > 0 AND @loop_count <= @Period 
		BEGIN
			SET @NextED = DATEADD(DAY, 30, @NextSD);	-- 迄日 = 起日 + 30天

			INSERT INTO @TB_Period
			VALUES(@MonProjID, @MonProPeriod, @ShortDays, @loop_count, @NxtMonSetID, @NextSD, @NextED);

			SET @NextSD = @NextED;
			SET @Loop_Count += 1;
		END

		SELECT @Period_Count = count(*) FROM @TB_Period;

		-- 建立月租
		IF @Period_Count > 0
		BEGIN
			DECLARE @SubsId BIGINT =0;
			DECLARE @SubsGroup INT = NEXT VALUE FOR NM_SubsGroup;	-- 不懂這作法用意，不就流水號嗎?

			INSERT INTO TB_SubsMain(MonProjID, MonProPeriod, ShortDays, SubsGroup, AutoSubs, IDNO, RenewPeriod, UpdPeriod, MKTime, UPDTime, A_PRGID, A_USERID, U_PRGID, U_USERID)
			VALUES(@MonProjID, @MonProPeriod, @ShortDays, @SubsGroup, 1, @IDNO, 0, 0, @NowTime, @NowTime, @FunName, @IDNO, @FunName, @IDNO);

			SELECT @SubsId = @@IDENTITY;

			INSERT INTO TB_MonthlyRentUse 
			(
				SubsId, MonLvl, ProjID, ProjNM, MonProPeriod, 
				ShortDays, SEQNO, Mode, MonType, IDNO,
				CarFreeType, CarTotalHours, WorkDayHours, HolidayHours,
				MotoFreeType, MotoTotalHours, MotoWorkDayMins, MotoHolidayMins,
				WorkDayRateForCar, HoildayRateForCar, WorkDayRateForMoto, HoildayRateForMoto,
				StartDate, EndDate, MKTime, UPDTime,
				A_PRGID, A_USERID, U_PRGID, U_USERID
			)
			SELECT @SubsId, B.MonLvl, B.MonProjID, B.MonProjNM, B.MonProPeriod,
				B.ShortDays, 0, B.IsMoto, B.MonType, @IDNO,
				0, B.CarTotalHours, B.CarWDHours, B.CarHDHours,
				0, B.MotoTotalMins, B.MotoWDMins, B.MotoHDMins,
				B.WDRateForCar, B.HDRateForCar, B.WDRateForMoto, B.HDRateForMoto,
				A.SDATE, A.EDATE, @NowTime, @NowTime,
				@FunName, @IDNO, @FunName, @IDNO
			FROM @TB_Period A
			INNER JOIN TB_MonthlyRentSet B ON B.MonSetID=A.NxtMonSetID;
			
			SELECT TOP 1 @NewMonthlyRentID=MonthlyRentId FROM TB_MonthlyRentUse WITH(NOLOCK) WHERE SubsId=@SubsId AND useFlag=1 Order BY MonthlyRentId;
		END

		-- 建立付費紀錄
		IF @NewMonthlyRentID > 0
		BEGIN
			DECLARE @MRSDetailID INT;	-- 月租設定檔流水號

			SELECT TOP 1 @MRSDetailID=MRSDetailID FROM TB_MonthlyRentSetDetail WITH(NOLOCK) WHERE MonProjID=@MonProjID ORDER BY Period;

			INSERT INTO TB_MonthlyPay (MonthlyRentId, MRSDetailID, IDNO, ActualPay,
									   ExecPayDate, PayDate, PayTypeId, InvoTypeId, 
									   MerchantTradeNo, TaishinTradeNo, MKTime, UPDTime,
									   A_PRGID, A_USERID, U_PRGID, U_USERID)
			VALUES(@NewMonthlyRentID, @MRSDetailID, @IDNO, 1,
				   @NowTime, @NowTime, @PayTypeId, @InvoTypeId,
				   @MerchantTradeNo, @TaishinTradeNo, @NowTime, @NowTime,
				   @FunName, @IDNO, @FunName, @IDNO
			);
		END

		-- 自動續訂
		IF @NewMonthlyRentID > 0
		BEGIN
			UPDATE TB_SubsNxt
			SET SubsNxtTime=@NowTime,
				U_PRGID=@FunName,
				U_USERID=@IDNO
			WHERE SubsNxtID=@SubsNxtID;

			INSERT INTO TB_SubsNxt (IDNO, IsMotor, NowMonthlyRentId, NowMonSetID, NxtMonSetID, NowSubsEDate, SubsNxtTime, MKTime, UPDTime, A_PRGID, A_USERID, U_PRGID, U_USERID)
			VALUES(@IDNO, @IsMotor, @NewMonthlyRentID, @NxtMonSetID ,@NxtMonSetID, @NowTime, NULL, @NowTime, @NowTime, @FunName, @IDNO, @FunName, @IDNO);
		END
	END
	COMMIT TRAN

	DROP TABLE IF EXISTS #Set;
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	SET @Error=-1;
	SET @ErrorCode='ERR999';
	SET @ErrorMsg='我要寫錯誤訊息';
	SET @SQLExceptionCode=ERROR_NUMBER();
	SET @SQLExceptionMsg=ERROR_MESSAGE();

    INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
    VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH

RETURN @Error
GO

