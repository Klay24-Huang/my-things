/****************************************************************
** Name: [dbo].[usp_MonthRent_I01]
** Desc: 
**
** Return values: 0 成功 else 錯誤

**------------
** Auth:ADAM
** Date:2020/10/31
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/31|  ADAM      |    First Release
** 2021/02/05 UPD BY Brian 調整匯入月租判斷條件抓取SEQNO
*****************************************************************/
/*
EXAMPLE
BEGIN TRAN
DECLARE @TY_MonthlyRent AS dbo.TY_MonthlyRent

DECLARE @MSG VARCHAR(100)
		,@IDNO VARCHAR(10) = 'A140584782'
		,@WorkDayHours FLOAT = 100
		,@HolidayHours FLOAT = 0
		,@MotorTotalHours FLOAT = 0
		,@SEQNO INT = 12345
		,@StartDate DATETIME = '2020-01-01 00:00:00'
		,@EndDate DATETIME = '2020-01-31 23:59:59'
		,@ProjID VARCHAR(10) = 'MR01'
		,@ProjNM NVARCHAR(50) = '月租測試'
		,@Error INT, @ErrorCode VARCHAR(6),@ErrorMsg VARCHAR(100), @SQLExceptionCode VARCHAR(10), @SQLExceptionMsg NVARCHAR(1000)
INSERT INTO @TY_MonthlyRent VALUES(@IDNO,@WorkDayHours,@HolidayHours,@MotorTotalHours,@StartDate,@EndDate,@SEQNO,@ProjID,@ProjNM)
EXEC usp_MonthRent_I01 @MSG OUTPUT,999,@TY_MonthlyRent
SELECT * FROM TB_MonthlyRent WHERE IDNO=@IDNO
ROLLBACK TRAN
*/
CREATE PROCEDURE [dbo].[usp_MonthRent_I01]
	@ErrorMsg  			NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@LogID              BIGINT,
	@MonthlyRent as	dbo.TY_MonthlyRent readonly
AS
SET NOCOUNT ON
SET @ErrorMsg = ''

DECLARE @ErrorCode 				VARCHAR(6)		,	--回傳錯誤代碼
		@SQLExceptionCode		VARCHAR(10)		,	--回傳sqlException代碼
		@SQLExceptionMsg		NVARCHAR(1000)		--回傳sqlException訊息

DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;
DECLARE @Mode INT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
--SET @ErrorMsg='SUCCESS'; 
SET @ErrorMsg=''; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_MonthRent_I01'
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Mode=0
SET @NowTime=DATEADD(hour,8,GETDATE());

BEGIN TRY
		--IF @MotorTotalHours > 0
		--BEGIN
		--	SET @Mode=1
		--END

	SELECT @hasData=COUNT(1) FROM TB_MonthlyRent A WITH(NOLOCK)  
	JOIN @MonthlyRent B ON A.IDNO=B.IDNO 
	AND A.Mode=CASE WHEN B.MotorTotalHours > 0 THEN 1 ELSE 0 END
	--AND B.StartDate BETWEEN A.StartDate AND A.EndDate
	AND A.SEQNO = B.SEQNO	--2021/02/05 UPD BY Brian 調整匯入月租判斷條件抓取SEQNO

	IF @hasData>0
	BEGIN
		SET @Error=1
		SET @ErrorCode='ERR227';
	END

	IF @Error=0
	BEGIN
		INSERT INTO TB_MonthlyRent
		(
			ProjID, ProjNM, SEQNO, Mode, IDNO, WorkDayHours,
			HolidayHours, MotoTotalHours, StartDate, EndDate,
			WorkDayRateForMoto, HoildayRateForMoto,
			MKTime, UPDTime
		)
		/*
		VALUES
		(
			@ProjID, @ProjNM, @SEQNO, @Mode, @IDNO, @WorkDayHours,
			@HolidayHours, @MotorTotalHours, @StartDate, @EndDate,
			@NowTime, @NowTime
		)*/
		SELECT ProjID, ProjNM, SEQNO, CASE WHEN MotorTotalHours > 0 THEN 1 ELSE 0 END, IDNO, WorkDayHours,
			HolidayHours, MotorTotalHours, StartDate, EndDate,
			CASE WHEN MotorTotalHours > 0 THEN FavHFee ELSE 1.5 END, CASE WHEN MotorTotalHours > 0 THEN FavHFee ELSE 1.5 END,
			@NowTime, @NowTime
		FROM @MonthlyRent
	END

	--增加錯誤回傳
	SELECT 
		Error=@Error,
		ErrorCode=@ErrorCode,
		ErrorMsg=@ErrorMsg,
		SQLExceptionCode=@SQLExceptionCode,
		SQLExceptionMsg=@SQLExceptionMsg;
END TRY
BEGIN CATCH
	SET @Error=-1;
	SET @ErrorCode='ERR999';
	SET @ErrorMsg='我要寫錯誤訊息';
	SET @SQLExceptionCode=ERROR_NUMBER();
	SET @SQLExceptionMsg=ERROR_MESSAGE();
			
	--增加錯誤回傳
	SELECT 
		Error=@Error,
		ErrorCode=@ErrorCode,
		ErrorMsg=@ErrorMsg,
		SQLExceptionCode=@SQLExceptionCode,
		SQLExceptionMsg=@SQLExceptionMsg;

	--IF @@TRANCOUNT > 0
	--BEGIN
	--	print 'rolling back transaction' /* <- this is never printed */
	--	ROLLBACK TRAN
	--END
	SET @IsSystem=1;
	SET @ErrorType=4;
	INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
RETURN 0

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MonthRent_I01';
GO

EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Adam', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MonthRent_I01';
GO

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'月租訂閱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MonthRent_I01';
GO

EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MonthRent_I01';
GO

EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MonthRent_I01';