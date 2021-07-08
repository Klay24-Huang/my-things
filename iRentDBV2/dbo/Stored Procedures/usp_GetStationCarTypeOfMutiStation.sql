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
** 2021/03/30 ADD BY ADAM REASON.測試多身分
** 2021/06/14 UPD BY YEH REASON:增加積分<60分只能用定價專案
*****************************************************************/
/*
EXEC usp_GetStationCarTypeOfMutiStation_ForTest 'X1UU','2021-7-1 08:00:00','2021-7-1 09:00:00','YARIS','A124215371',0,0,9999,'','','',''
*/
CREATE PROCEDURE [dbo].[usp_GetStationCarTypeOfMutiStation]
	@StationIDs             VARCHAR(MAX)          , --據點代碼（1~多個）
	@SD                     DATETIME		      , --起日
	@ED                     DATETIME              , --迄日
	@CarTypes               VARCHAR(MAX)          , --車型群組代碼（1~多個）
	@IDNO					VARCHAR(10)			  , --會員代碼
	@Insurance				TINYINT				  , --是否計算安心服務
	@Mode					TINYINT				  , --模式判斷(0:沒有送定位點/1:則有送)
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
DECLARE @TotalHours FLOAT;
DECLARE @SpecStatus VARCHAR(2);
DECLARE @Score INT;	-- 會員積分

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetStationCarTypeOfMutiStation';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime = DATEADD(hour,8,GETDATE());
SET @Score=100;	-- 預設積分

BEGIN TRY

	DROP TABLE IF EXISTS #TB_OrderMain
	DROP TABLE IF EXISTS #SPDATE
	DROP TABLE IF EXISTS #TB_Project
	DROP TABLE IF EXISTS #BookingList

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

	SET @CarTypes = ISNULL(@CarTypes,'')
	SET @IDNO = ISNULL(@IDNO,'')

	DECLARE @Tmp_CarTypes_Count int = 0
	select @Tmp_CarTypes_Count = count(*) from dbo.FN_StrListToTb(@CarTypes)

	--計算預估時數
	SELECT @TotalHours = dbo.FN_CalHours(@SD,@ED)

	--查詢
	IF @Error = 0
	BEGIN
		--取得特殊身分
		SELECT @SpecStatus=ISNULL(SPECSTATUS,'') FROM TB_MemberData WITH(NOLOCK) 
		WHERE MEMIDNO=@IDNO AND CONVERT(VARCHAR,dbo.GET_TWDATE(),112) BETWEEN SPSD AND SPED;
				
		--找出被預約的車輛
		IF OBJECT_ID('tempdb..#TB_OrderMain') IS NOT NULL DROP TABLE #TB_OrderMain

		SELECT DISTINCT A.CarNo	--排除重複以免造成問題
		INTO #TB_OrderMain
		FROM TB_OrderMain A WITH(NOLOCK)
		JOIN TB_CarInfo I WITH(NOLOCK) ON A.CarNo=I.CarNo		--20201231 ADD BY ADAM REASON.排除車機未設定
		WHERE 1=1
		AND I.CID<>''				--20201231 ADD BY ADAM REASON.排除車機未設定
		AND booking_status<5		--未完成合約
		AND car_mgt_status<16		--未完成合約
		AND cancel_status=0			--未刪除訂單
		AND ((start_time BETWEEN @SD AND @ED)
				OR (stop_time BETWEEN @SD AND @ED)
				OR (@SD BETWEEN start_time AND stop_time)
				OR (@ED BETWEEN start_time AND stop_time)
				OR (DATEADD(MINUTE, -30, @SD) BETWEEN start_time AND stop_time)
				OR (DATEADD(MINUTE, 30, @ED) BETWEEN start_time AND stop_time))

		--20210322 ADD BY ADAM REASON.增加逾時未還的車
		INSERT INTO #TB_OrderMain
		SELECT CarNo FROM TB_OrderMain WITH(NOLOCK)	-- 排除逾時未還的車
		WHERE start_time>DATEADD(Month,-1,dbo.GET_TWDATE()) AND cancel_status=0 AND (car_mgt_status>=4 and car_mgt_status<16) 
		AND stop_time < dbo.GET_TWDATE()
		AND CarNo NOT IN (SELECT CarNo FROM #TB_OrderMain)	--20210401 ADD BY ADAM REASON.排除掉已經有的車

		CREATE NONCLUSTERED INDEX ix_tempTB_OrderMain_CarNo ON #TB_OrderMain (CarNo)

		SELECT A.nowStationID,A.CarType
		INTO #BookingList
		FROM (
			SELECT nowStationID,CarType=E.CarTypeGroupCode,TotalCount=COUNT(E.CarTypeGroupCode) 
			FROM TB_Car c WITH(NOLOCK) 
			JOIN @tb_StationID s ON c.nowStationID=s.StationID 
			JOIN TB_CarInfo I WITH(NOLOCK) ON c.CarNo=I.CarNo		--20201231 ADD BY ADAM REASON.排除車機未設定
			JOIN TB_CarTypeGroupConsist F WITH(NOLOCK) ON F.CarType=c.CarType
			JOIN TB_CarTypeGroup E WITH(NOLOCK) ON F.CarTypeGroupID=E.CarTypeGroupID
			WHERE c.available < 2 
			AND I.CID<>''		--20201231 ADD BY ADAM REASON.排除車機未設定
			GROUP BY nowStationID,E.CarTypeGroupCode
			) AS A
		JOIN (
			SELECT  nowStationID,CarType=E.CarTypeGroupCode,NGCount = COUNT(E.CarTypeGroupCode) 
			FROM TB_Car c WITH(NOLOCK)
			JOIN #TB_OrderMain o ON c.CarNo = o.CarNo
			JOIN TB_CarTypeGroupConsist F WITH(NOLOCK) ON F.CarType=c.CarType
			JOIN TB_CarTypeGroup E WITH(NOLOCK) ON F.CarTypeGroupID=E.CarTypeGroupID
			GROUP BY nowStationID,E.CarTypeGroupCode
			) AS B ON A.nowStationID=B.nowStationID AND A.CarType=B.CarType
		WHERE A.TotalCount = B.NGCount

		SELECT * INTO #TB_Project 
		FROM TB_Project WITH(NOLOCK) 
		WHERE ((ShowStart BETWEEN @SD AND @ED) OR (ShowEnd BETWEEN @SD AND @ED) OR (@SD BETWEEN ShowStart AND ShowEnd) OR (@ED BETWEEN ShowStart AND ShowEnd))

		--測試春節專案(暫用2/1~1/3)，先用寫死方式之後再改
		--CREATE TABLE #SPDATE (STADATE DATETIME,ENDDATE DATETIME)
		--INSERT INTO #SPDATE VALUES('2021-02-09','2021-02-17')

		--IF EXISTS(SELECT * FROM #SPDATE WHERE ((STADATE BETWEEN @SD AND @ED) OR (ENDDATE BETWEEN @SD AND @ED) OR (@SD BETWEEN STADATE AND ENDDATE) OR (@ED BETWEEN STADATE AND ENDDATE))) --AND @SpecStatus='99'
		--BEGIN
		--	--在此時間的專案會只剩特殊專案
		--	DELETE FROM #TB_Project WHERE PROJID<>'R129' 
		--END
		--ELSE
		--BEGIN
		--	DELETE FROM #TB_Project WHERE PROJID='R129'
		--END

		-- 取得會員積分
		SELECT @Score=SCORE FROM TB_MemberScoreMain WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

		IF @Score < 60	-- 積分<60分只能用定價專案
		BEGIN
			SET @SpecStatus = '90';	-- 定價專用特殊身分
			-- 將#TB_Project的非定價專案都移除
			DELETE FROM #TB_Project WHERE SPCLOCK<>@SpecStatus;
		END

		IF @Score >= 60	-- 積分>60才處理澎湖花火節專案
		BEGIN
			--20210331 ADD 澎湖花火節專案限制
			DECLARE @Penghu	VARCHAR(10);
			SELECT @Penghu=ISNULL('Y','N') FROM @tb_StationID WHERE StationID='X1UU';
			IF @Penghu='Y'
			BEGIN
				CREATE TABLE #SPDATE (STADATE DATETIME,ENDDATE DATETIME)
				INSERT INTO #SPDATE VALUES('2021-04-01','2021-08-31')
					
				--IF EXISTS(SELECT * FROM #SPDATE WHERE ((STADATE BETWEEN @SD AND @ED) OR (ENDDATE BETWEEN @SD AND @ED) OR (@SD BETWEEN STADATE AND ENDDATE) OR (@ED BETWEEN STADATE AND ENDDATE)))
				IF EXISTS(SELECT * FROM #SPDATE WHERE ((@SD BETWEEN STADATE AND ENDDATE)))
				BEGIN
					--在此時間的專案會只剩特殊專案
					DELETE FROM #TB_Project WHERE PROJID<>'R024' 
				END
				ELSE
				BEGIN
					DELETE FROM #TB_Project WHERE PROJID='R024'
				END
			END
		END

		IF @Mode=1
		BEGIN
			SELECT DISTINCT P.PROJID,PRICE=dbo.FN_CarRentCompute(@SD, @ED, IIF(PD.PRICE>0 , PD.PRICE, P.PROPRICE_N), IIF(PD.PRICE>0 , PD.PRICE_H, P.PROPRICE_H), 10, 0) 
			INTO #PRICE
			FROM TB_CarType D WITH(NOLOCK)
			JOIN TB_CarTypeGroupConsist F WITH(NOLOCK) ON F.CarType=D.CarType
			JOIN TB_CarTypeGroup E WITH(NOLOCK) ON F.CarTypeGroupID=E.CarTypeGroupID
			JOIN TB_ProjectStation AS S WITH(NOLOCK) ON S.IOType='I'
			JOIN TB_ProjectDiscount AS PD ON PD.ProjID=S.PROJID AND PD.CARTYPE=D.CarType
			JOIN #TB_Project AS P ON P.PROJID=PD.ProjID
			JOIN @tb_StationID AS S1 ON S1.StationID=S.StationID
			WHERE 1=1
			AND E.CarTypeGroupCode=(case when @Tmp_CarTypes_Count > 0 then (select top 1 sValue from dbo.FN_StrListToTb(@CarTypes) fn where fn.sValue = E.CarTypeGroupCode) else E.CarTypeGroupCode end)
			AND (SPCLOCK='Z' OR (@SpecStatus<>'' AND SPCLOCK=@SpecStatus))

			DELETE FROM #TB_Project WHERE PROJID NOT IN (SELECT TOP 1 PROJID FROM #PRICE GROUP BY PROJID ORDER BY MIN(PRICE))
		END


		--20201023 ADD BY ADAM REASON.調整邏輯
		SELECT  DISTINCT StationID		= C.nowStationID
				,PROJID					= P.PROJID
				,PRONAME				= P.PRONAME
				,PRODESC				= P.PRODESC
				,Price					= IIF(PD.PRICE>0 , PD.PRICE, P.PROPRICE_N) --平日	--2020-12-17 eason
				,Price_H				= IIF(PD.PRICE_H>0 , PD.PRICE_H, P.PROPRICE_H) --假日	--2020-12-17 eason
				,PriceBill              = dbo.FN_CarRentCompute(@SD, @ED, IIF(PD.PRICE>0 , PD.PRICE, P.PROPRICE_N), IIF(PD.PRICE>0 , PD.PRICE_H, P.PROPRICE_H), 10, 0)
										+ (@TotalHours * ISNULL(MS.MilageBase,0)*20)
										+ CASE WHEN @Insurance=1 THEN (@TotalHours *  CASE WHEN E.isMoto=1 THEN 0 WHEN K.InsuranceLevel IS NULL THEN II.InsurancePerHours WHEN K.InsuranceLevel < 6 THEN K.InsurancePerHours ELSE 0 END) ELSE 0 END
				,MilageBase             = ISNULL(MS.MilageBase,0)
				,CarBrend				= D.CarBrend
				,CarType				= E.CarTypeGroupCode
				,CarTypeName			= D.CarBrend + ' ' + D.CarTypeName		--廠牌+車型
				,CarTypePic				= E.CarTypeImg					--車輛ICON對照
				,Operator				= O.OperatorICon				--運營商ICON
				,OperatorScore			= O.Score						--分數
				,Seat					= E.Seat						--座椅數
				,ADDR					= ISNULL(I.ADDR,'')				--據點位置
				,StationName			= ISNULL(I.Location,'')			--據點名稱
				,Longitude				= ISNULL(I.Longitude,0)
				,Latitude				= ISNULL(I.Latitude,0)
				,Content				= ISNULL(I.Content,'')
				,ContentForAPP          = ISNULL(I.ContentForAPP,'') --據點描述app顯示） eason 2020-11-27
				,CityName               = ISNULL(CT.CityName,'')--縣市 eason 2020-11-27					
				,AreaName               = ISNULL(AZ.AreaName,'') -- 行政區 eason 2020-11-27
				,PayMode				= P.PayMode
				,CarOfArea				= I.Area
				,IsRent					= CASE WHEN BL.CarType<>'' THEN 'N' ELSE 'Y' END
				,Insurance				= CASE WHEN E.isMoto=1 THEN 0 WHEN ISNULL(BU.InsuranceLevel,3) >= 4 THEN 0 ELSE 1 END		--安心服務  20201206改為等級4就是停權
				,InsurancePerHours		= CASE WHEN E.isMoto=1 THEN 0 WHEN K.InsuranceLevel IS NULL THEN II.InsurancePerHours WHEN K.InsuranceLevel < 4 THEN K.InsurancePerHours ELSE 0 END		--安心服務每小時價
				,StationPicJson			= ISNULL((SELECT [StationPic],[PicDescription] FROM [TB_iRentStationInfo] SI WITH(NOLOCK) JOIN @tb_StationID s ON SI.[StationID]=s.StationID WHERE SI.use_flag=1 AND s.StationID=C.nowStationID FOR JSON PATH),'[]')
				,IsFavStation           = CASE WHEN (SELECT COUNT(*) FROM VW_GetFavoriteStation fs WITH(NOLOCK) WHERE fs.StationID = I.StationID AND fs.IDNO = @IDNO) > 0
						                    THEN 1 ELSE 0 END   
		--20201231 ADD BY ADAM REASON.排除車機未設定
		FROM (SELECT nowStationID,c.CarType,CarOfArea,c.CarNo FROM TB_Car c WITH(NOLOCK) 
				JOIN @tb_StationID s ON c.nowStationID=s.StationID 
				JOIN TB_CarInfo I WITH(NOLOCK) ON c.CarNo=I.CarNo
				WHERE c.available < 2 AND I.CID<>'') C
		LEFT JOIN TB_CarType D WITH(NOLOCK) ON C.CarType=D.CarType
		LEFT JOIN TB_CarTypeGroupConsist F WITH(NOLOCK) ON F.CarType=C.CarType
		LEFT JOIN TB_CarTypeGroup E WITH(NOLOCK) ON F.CarTypeGroupID=E.CarTypeGroupID
		LEFT JOIN TB_OperatorBase O WITH(NOLOCK) ON D.Operator=O.OperatorID
		LEFT JOIN TB_ProjectStation S WITH(NOLOCK) ON S.StationID=C.nowStationID AND S.IOType='O'
		LEFT JOIN #TB_Project P WITH(NOLOCK) ON P.PROJID=S.PROJID
		JOIN TB_ProjectDiscount PD WITH(NOLOCK) ON PD.ProjID = S.PROJID AND D.CarType = PD.CARTYPE --2020-12-17 eason
		LEFT JOIN TB_iRentStation I WITH(NOLOCK) ON I.StationID=C.nowStationID AND (@SD BETWEEN I.SDate AND I.EDate) AND (@ED BETWEEN I.SDate AND I.EDate)
		LEFT JOIN TB_City CT WITH(NOLOCK) ON CT.CityID = I.CityID --eason 2020-11-27
		LEFT JOIN TB_AreaZip AZ WITH(NOLOCK) ON AZ.AreaID = I.AreaID --eason 2020-11-27
		LEFT JOIN #BookingList BL ON C.nowStationID=BL.nowStationID AND E.CarTypeGroupCode=BL.CarType
		LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=@IDNO
		LEFT JOIN TB_InsuranceInfo K WITH(NOLOCK) ON K.CarTypeGroupCode=E.CarTypeGroupCode AND K.useflg='Y' AND BU.InsuranceLevel=K.InsuranceLevel
		LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=E.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3		--預設專用
		LEFT JOIN TB_MilageSetting MS WITH(NOLOCK) ON MS.ProjID=P.PROJID AND MS.use_flag=1 --AND @NowTime BETWEEN MS.SDate AND MS.EDate		--20210407 ADD BY ADAM REASON.因應5/1專案先上線需要把此邏輯先拿掉
		--LEFT JOIN (SELECT CarNo FROM #TB_OrderMain GROUP BY CarNo) T ON C.CarNo=T.CarNo
		WHERE 1=1
		AND ((SPCLOCK='Z') OR (@SpecStatus<>'' AND SPCLOCK=@SpecStatus))
		ORDER BY StationID,PriceBill,IsRent DESC
	END

	DROP TABLE IF EXISTS #TB_OrderMain
	DROP TABLE IF EXISTS #SPDATE
	DROP TABLE IF EXISTS #TB_Project
	DROP TABLE IF EXISTS #BookingList

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