/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_AutoSubscribeJob_Q01
* 系    統 : IRENT
* 程式功能 : 取得訂閱制自動續訂清單
* 作    者 : YEH
* 撰寫日期 : 20220119
* 修改日期 : 

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_AutoSubscribeJob_Q01]
	@LogID				BIGINT                ,
	@ErrorCode			VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;
DECLARE @NowString VARCHAR(8);
DECLARE @TwoDay DATETIME;
DECLARE @TwoDayString VARCHAR(8);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_AutoSubscribeJob_Q01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @NowString=CONVERT(VARCHAR, @NowTime, 112);
SET @TwoDay=DATEADD(DAY,2,@NowTime);
SET @TwoDayString=CONVERT(VARCHAR, @TwoDay, 112);

BEGIN TRY
	-- 取得資料
	IF @Error=0
	BEGIN
		DROP TABLE IF EXISTS #Temp;
		DROP TABLE IF EXISTS #Temp2;

		SELECT A.IDNO,B.SubsId,A.SubsNxtID,A.NxtMonSetID
		INTO #Temp
		FROM TB_SubsNxt A WITH(NOLOCK)
		INNER JOIN TB_MonthlyRentUse B WITH(NOLOCK) ON B.IDNO=A.IDNO AND B.MonthlyRentId=A.NowMonthlyRentId
		WHERE CONVERT(VARCHAR, A.NowSubsEDate, 112) = @TwoDayString
		AND A.SubsNxtTime IS NULL;

		SELECT B.IDNO,B.SubsId,MAX(B.MonthlyRentId) AS MonthlyRentID
		INTO #Temp2
		FROM TB_SubsMain A WITH(NOLOCK)
		INNER JOIN TB_MonthlyRentUse B WITH(NOLOCK) ON B.IDNO=A.IDNO AND B.SubsId=A.SubsId
		INNER JOIN #Temp C ON C.SubsId=B.SubsId AND C.IDNO=B.IDNO
		GROUP BY B.IDNO,B.SubsId;

		SELECT A.IDNO,
			B.MonthlyRentID,
			D.MonProjID,
			D.MonProPeriod,
			D.ShortDays,
			A.SubsNxtID,
			D.IsMoto AS IsMotor,
			A.NxtMonSetID,
			C.EndDate,
			E.PeriodPayPrice,
			ISNULL(G.MEMSENDCD,2) AS InvoiceType,
			G.CARRIERID,
			G.UNIMNO,
			G.NPOBAN,
			H.CodeId AS InvoiceID
		FROM #Temp A WITH(NOLOCK)
		INNER JOIN #Temp2 B WITH(NOLOCK) ON B.IDNO=A.IDNO AND B.SubsId=A.SubsId
		INNER JOIN TB_MonthlyRentUse C WITH(NOLOCK) ON C.IDNO=B.IDNO AND C.MonthlyRentId=B.MonthlyRentID
		INNER JOIN TB_MonthlyRentSet D WITH(NOLOCK) ON D.MonSetID=A.NxtMonSetID
		INNER JOIN TB_MonthlyRentSetDetail E WITH(NOLOCK) ON E.MonProjID=D.MonProjID AND E.MonProPeriod=D.MonProPeriod AND E.ShortDays=D.ShortDays AND E.Period=1
		INNER JOIN TB_MemberScoreMain F WITH(NOLOCK) ON F.MEMIDNO=A.IDNO
		INNER JOIN TB_MemberData G WITH(NOLOCK) ON G.MEMIDNO=A.IDNO
		LEFT JOIN TB_Code H WITH(NOLOCK) ON H.MapCode=G.MEMSENDCD AND H.UseFlag=1 AND H.CodeGroup='InvoiceType'
		WHERE F.SCORE >= 60;

		DROP TABLE IF EXISTS #Temp;
		DROP TABLE IF EXISTS #Temp2;
	END
		
	-- 寫入錯誤訊息
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberMedal';
GO

