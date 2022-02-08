/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_InsMonthlyPayData
* 系    統 : IRENT
* 程式功能 : 產生訂閱制續期付款清單
* 作    者 : AMBER
* 撰寫日期 : 20210810
* 修改日期 : 20210826 ADD BY ADAM REASON.補上兩期一付處理，跑過就好
             20220122 ADD BY AMBER REASON.補上六兄弟
Example :
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
	DECLARE @FunName   VARCHAR(20) = 'InsMonthlyPayData'
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
			DROP TABLE IF EXISTS #TB_MonthlyPayTmp;

			SELECT
			A.IDNO,A.MonProPeriod,A.ProjID,A.ShortDays  
			INTO #MonthlyPayTmp
			FROM SYN_MonthlyRent A WITH(NOLOCK) 
			LEFT JOIN TB_MonthlyPay B WITH(NOLOCK) ON A.MonthlyRentId=B.MonthlyRentId
			--WHERE CONVERT(VARCHAR,DATEADD(day,-1,A.StartDate),112)=@SetDate  
			WHERE CONVERT(VARCHAR,DATEADD(day,-1,A.StartDate),112)=CONVERT(VARCHAR,@NowTime,112)
			AND A.useFlag=1 AND A.MonProPeriod >2 AND ISNULL(B.ActualPay,0)=0 
			AND NOT EXISTS (SELECT 1 FROM TB_OrderAuthMonthly O WITH(NOLOCK) 
			WHERE O.MonthlyRentId =A.MonthlyRentId);
	
			
			
			SELECT * INTO #SeqMonthlyPayTmp FROM (
		    SELECT ROW_NUMBER() OVER(PARTITION BY S.MonProPeriod,S.ProjID,S.ShortDays,S.IDNO ORDER BY S.startDate) AS NowPeriod,
		    S.MonProPeriod,S.ProjID,S.ShortDays,S.IDNO,S.StartDate,S.EndDate,S.MonthlyRentId 
		    FROM SYN_MonthlyRent S WITH(NOLOCK) 
			JOIN #MonthlyPayTmp MPT ON S.IDNO=MPT.IDNO 
			AND S.ProjID=MPT.ProjID  AND S.MonProPeriod=MPT.MonProPeriod AND S.ShortDays=MPT.ShortDays) T 	
			--WHERE CONVERT(VARCHAR,DATEADD(day,-1,T.StartDate),112)=@SetDate;   
			WHERE CONVERT(VARCHAR,DATEADD(day,-1,T.StartDate),112)=CONVERT(VARCHAR,@NowTime,112);

			SELECT @hasData=COUNT(1) FROM #SeqMonthlyPayTmp SMPT ;

			IF @hasData=0
			BEGIN
				SET @Error=1
				SET @ErrorCode='ERR911'  
			END

			--20210826 ADD BY ADAM REASON.補上兩期一付處理，跑過就好
			select row_number() OVER(PARTITION BY SubsId ORDER BY MonthlyRentId) As Period,* 
			into #TMP_TB_MonthlyRentUse
			from TB_MonthlyRentUse
			where StartDate >='2021-07-21 00:00:00.000' and useFlag = 1 and IDNO !='A1223643170'

			insert into TB_MonthlyPay(MonthlyRentId,MRSDetailID,IDNO,ActualPay,ExecPayDate,PayDate,MKTime,A_PRGID,A_USERID,U_PRGID,U_USERID,UPDTime)
			select a.MonthlyRentId,c.MRSDetailID,a.IDNO,1 AS ActualPay,
			ExecPayDate=dateadd(day,-1,a.StartDate),
			PayDate=dateadd(day,-1,a.StartDate),
			MKTime=@NowTime,
			A_PRGID=@FunName,
			A_USERID=@FunName,
		    U_PRGID=@FunName,
			U_USERID=@FunName,
			UPDTime=@NowTime
			from #TMP_TB_MonthlyRentUse a
			left join TB_MonthlyPay b on a.MonthlyRentId = b.MonthlyRentId
			join TB_MonthlyRentSetDetail c on a.ProjID = c.MonProjID and a.Period = c.Period
			where convert(varchar(8),a.StartDate,112) = convert(varchar(8),dateadd(day,1,DATEADD(HOUR,8,GETDATE())),112)
			and a.MonProPeriod = 2 and a.Period = 2 and b.MonthlyRentId is null

	END

    IF @Error=0 AND  @hasData >0
	BEGIN
	INSERT INTO [dbo].[TB_OrderAuthMonthly]
           ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],[IDNO],[order_number],[AuthMessage],[MonthlyRentId],[final_price],[NowPeriod])
		SELECT 
		  A_PRGID=@FunName
		, A_USERID=@FunName
		, A_SYSDT=@NowTime
		, U_PRGID=@FunName
		, U_USERID=@FunName
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
		
		INSERT INTO [dbo].[TB_OrderAuthMonthly_LOG] 
		SELECT 'A',A.* FROM [dbo].[TB_OrderAuthMonthly] A WITH(NOLOCK) 
		JOIN #SeqMonthlyPayTmp T ON A.MonthlyRentId=T.MonthlyRentId AND A.IDNO=T.IDNO
		WHERE A.AuthFlg=0;
		
	  --20210819 ADD BY AMBER REASON.補寫至TB_MonthlyPay
	  ;with tmp  as (
	  select t.MonthlyRentId,d.MRSDetailID,t.IDNO,
	  case when m.MEMSENDCD IN (0,7) then 2 else m.MEMSENDCD end as InvoTypeId	
	  from #SeqMonthlyPayTmp t 
	  join TB_MonthlyRentSetDetail d with(nolock) on t.MonProPeriod=d.MonProPeriod and t.ShortDays=d.ShortDays and t.ProjID=d.MonProjID and NowPeriod=d.Period
	  join TB_MemberData m with(nolock) on t.IDNO =m.MEMIDNO)
	  select
	   MonthlyRentId=tmp.MonthlyRentId
	  ,MRSDetailID=tmp.MRSDetailID
	  ,tmp.IDNO
	  ,ActualPay = 0
	  ,PayDate=@NowTime
	  ,PayTypeId=5
	  ,InvoTypeId=code.CodeId 
	  ,MerchantTradeNo =''
	  ,TaishinTradeNo  ='' 
	  ,MKTime=@NowTime
	  ,ExecPayDate=@NowTime
	  ,A_PRGID=@FunName
	  ,A_USERID=@FunName
	  ,U_PRGID=@FunName
	  ,U_USERID=@FunName
	  ,UPDTime=@NowTime
	  into #TB_MonthlyPayTmp from tmp 
	  join TB_Code code on tmp.InvoTypeId= code.MapCode
	  and code.CodeGroup='InvoiceType' and code.TBMap='TB_MemberData' and code.TBFieldMap='MEMSENDCD' and code.UseFlag=1;
		
		INSERT INTO [dbo].[TB_MonthlyPay]
		([MonthlyRentId],[MRSDetailID],[IDNO],[ActualPay],[PayDate],[PayTypeId],[InvoTypeId],[MerchantTradeNo],[TaishinTradeNo],[MKTime],[ExecPayDate],A_PRGID,A_USERID,U_PRGID,U_USERID,UPDTime)
		select * from #TB_MonthlyPayTmp;

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
	DROP TABLE IF EXISTS #TB_MonthlyPayTmp;
	DROP TABLE IF EXISTS #TMP_TB_MonthlyRentUse;	--20210826 ADD BY ADAM REASON.補上兩期一付處理，跑過就好
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsMonthlyPayData';



