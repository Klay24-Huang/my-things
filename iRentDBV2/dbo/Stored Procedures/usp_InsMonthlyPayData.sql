/****************************************************************
** 用　　途：產生訂閱制續期付款清單
*****************************************************************
** Change History
*****************************************************************
** 20210810 ADD BY AMBER
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsMonthlyPayData]            
　　@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
　　SET	@ErrorCode  = '0000'	
	SET	@ErrorMsg   = 'SUCCESS'	
	SET	@SQLExceptionCode = ''		
	SET	@SQLExceptionMsg = ''		
	DECLARE @IsSystem  TINYINT     = 1
    DECLARE @hasData   TINYINT
	DECLARE @ErrorType TINYINT     = 4
	DECLARE @FunName   VARCHAR(50) = 'usp_InsMonthlyPayData'
	DECLARE @NowTime   DATETIME    = dbo.GET_TWDATE()
	DECLARE @LogID     INT         =0
	DECLARE @Error INT;
	--DECLARE @SetDate   VARCHAR(10) ='20210819'
	/*初始設定*/
	SET @Error=0
	SET @hasData=0

BEGIN TRY
 	BEGIN
	        DROP TABLE IF EXISTS #MonthlyPayTmp;
			DROP TABLE IF EXISTS #SeqMonthlyPayTmp;

			SELECT 		
			A.IDNO,A.MonProPeriod,A.ProjID,A.ShortDays  
			INTO #MonthlyPayTmp
			FROM SYN_MonthlyRent A WITH(NOLOCK) 
			LEFT JOIN TB_MonthlyPay B WITH(NOLOCK) ON A.MonthlyRentId=B.MonthlyRentId
			WHERE CONVERT(VARCHAR,DATEADD(day,-1,A.StartDate),112)=CONVERT(VARCHAR,@NowTime,112)
			AND A.useFlag=1 AND A.MonProPeriod >2 AND ISNULL(B.ActualPay,0)=0 
			AND NOT EXISTS (SELECT 1 FROM TB_OrderAuthMonthly O
			WHERE O.MonthlyRentId =A.MonthlyRentId)
			AND A.MonthlyRentId=316;
			
			SELECT * INTO #SeqMonthlyPayTmp FROM (
		    SELECT ROW_NUMBER() OVER(PARTITION BY S.MonProPeriod,S.ProjID,S.ShortDays,S.IDNO ORDER BY S.startDate) AS NowPeriod,
		    S.MonProPeriod,S.ProjID,S.ShortDays,S.IDNO,S.StartDate,S.EndDate,S.MonthlyRentId 
		    FROM SYN_MonthlyRent S WITH(NOLOCK) 
			JOIN #MonthlyPayTmp MPT ON S.IDNO=MPT.IDNO 
			AND S.ProjID=MPT.ProjID  AND S.MonProPeriod=MPT.MonProPeriod AND S.ShortDays=MPT.ShortDays) T 	
			WHERE CONVERT(VARCHAR,DATEADD(day,-1,T.StartDate),112)=CONVERT(VARCHAR,@NowTime,112);

			SELECT @hasData=COUNT(1) FROM #SeqMonthlyPayTmp SMPT ;

		  IF @hasData=0
		  BEGIN
		    SET @Error=1
	    	SET @ErrorCode='ERR911'  
		  END
	END

    IF @Error=0 AND  @hasData >0
	BEGIN
	INSERT INTO [dbo].[TB_OrderAuthMonthly]
           ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],[IDNO],[order_number],[AuthMessage],[MonthlyRentId],[final_price],[NowPeriod])
		SELECT 
		  A_PRGID='InsMonthlyPayData'
		, A_USERID='InsMonthlyPayData'
		, A_SYSDT=@NowTime
		, U_PRGID='InsMonthlyPayData'
		, U_USERID='InsMonthlyPayData'
		, U_SYSDT=@NowTime
		, SMPT.IDNO	
		, ORDER_NUMBER=0
		, AUTHMESSAGE=''
		, MONTHLYRENTID=SMPT.MonthlyRentId
		, FINAL_PRICE=S.PeriodPrice
		, NOWPERIOD=SMPT.NowPeriod
		FROM #SeqMonthlyPayTmp SMPT
	    JOIN TB_MonthlyRentSet S WITH(NOLOCK) 
		ON SMPT.ProjID=S.MonProjID AND SMPT.MonProPeriod=S.MonProPeriod  AND SMPT.ShortDays = S.ShortDays;

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

    DROP TABLE IF EXISTS #MonthlyPayTmp;
    DROP TABLE IF EXISTS #SeqMonthlyPayTmp;
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsMonthlyPayData';

GO


