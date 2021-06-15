/*
DECLARE @IsFavStation INT
DECLARE @IsRent VARCHAR(10)
EXEC usp_GetStationCarType 'X0II','2021-04-10 00:00:00','2021-04-08 00:00:00','SIENTA5','','A122364317',99999,@IsFavStation OUTPUT,@IsRent OUTPUT,'','','',''
select @IsFavStation[IsFavStation], @IsRent[IsRent]
*/
CREATE PROCEDURE [dbo].[usp_GetStationCarType]
	@StationID              VARCHAR(10)           ,
	@SD                     DATETIME = NULL	      , 
	@ED                     DATETIME = NULL       ,
	@CarTypes               VARCHAR(MAX)          , --車型群組代碼（1~多個,逗號分隔）
	@Seats                  VARCHAR(MAX)          , --座位數（1~多個,逗號分隔）
	@IDNO					VARCHAR(10)			  , --會員代碼
	@LogID                  BIGINT   = 0          ,
	@IsFavStation           int             OUTPUT, --是否為最愛站點
	@IsRent                 VARCHAR(10)     OUTPUT, --是否為可租站點
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
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @NowTime=DATEADD(HOUR,8,DATEADD(hour,8,GETDATE()));

SET @FunName='usp_GetStationCarType';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;

    drop table if exists #TB_OrderMain
	drop table if exists #BookingList
	drop table if exists #Tmp_Final
	drop table if exists #Tmp_Final2

	BEGIN TRY      

	    SET @IDNO = ISNULL(@IDNO,'')
		SET @CarTypes = ISNULL(@CarTypes,'')
		SET @Seats = ISNULL(@Seats,'')

		DECLARE @Tmp_CarTypes_Count int = 0
		select @Tmp_CarTypes_Count = count(*) from dbo.FN_StrListToTb(@CarTypes)

		set @IsFavStation = iif((select count(*) FROM VW_GetFavoriteStation fs WHERE fs.StationID = @StationID AND fs.IDNO = @IDNO) > 0,1,0)

		DECLARE @Tmp_Seats_Count int = 0
		select @Tmp_Seats_Count = count(*) from dbo.FN_StrListToTb(@Seats)	

		--找出所有據點
		SELECT distinct 
                s.StationID
	        ,s.Location AS StationName
	        ,s.Tel
	        ,s.ADDR
	        ,s.Latitude
	        ,s.Longitude
	        ,s.Content
			,c.CarType
			,e.CarTypeGroupCode
		into #Tmp_iRenStation
        FROM TB_iRentStation s WITH (NOLOCK)
        JOIN TB_Car c WITH (NOLOCK) on c.nowStationID = s.StationID
        JOIN TB_CarTypeGroupConsist f WITH(NOLOCK) ON f.CarType=c.CarType
        JOIN TB_CarTypeGroup e WITH(NOLOCK) ON f.CarTypeGroupID=e.CarTypeGroupID
        WHERE s.use_flag = 3
        AND IsNormalStation = 0
        AND DATEADD(HOUR, 8, GETDATE()) BETWEEN SDate AND EDate
		AND s.StationID=@StationID
		--AND e.CarTypeGroupCode<>'COROLLACRO'	--20210423 ADD BY ADAM REASON.先排掉cc

		IF @SD IS NOT NULL AND @ED IS NOT NULL
		BEGIN
			--20210408 ADD BY ADAM REASON.增加IsRent欄位
			--找出被預約的車輛
			IF OBJECT_ID('tempdb..#TB_OrderMain') IS NOT NULL DROP TABLE #TB_OrderMain
			--SELECT order_number,IDNO,CarNo
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
				SELECT nowStationID,c.CarType,TotalCount=COUNT(c.CarType) 
				FROM TB_Car c WITH(NOLOCK) 
				--JOIN @StationID s ON c.nowStationID=s.StationID 
				JOIN TB_CarInfo I WITH(NOLOCK) ON c.CarNo=I.CarNo		--20201231 ADD BY ADAM REASON.排除車機未設定
				WHERE c.available < 2 
				AND I.CID<>''		--20201231 ADD BY ADAM REASON.排除車機未設定
				AND c.nowStationID=@StationID
				GROUP BY nowStationID,c.CarType
				) AS A
			JOIN (
				SELECT  nowStationID,c.CarType,NGCount = COUNT(c.CarType) 
				FROM TB_Car c WITH(NOLOCK)
				JOIN #TB_OrderMain o ON c.CarNo = o.CarNo
				GROUP BY nowStationID,c.CarType
				) AS B ON A.nowStationID=B.nowStationID AND A.CarType=B.CarType
			--JOIN TB_CarType D WITH(NOLOCK) ON A.CarType=D.CarType
			--JOIN TB_CarTypeGroupConsist F WITH(NOLOCK) ON F.CarType=A.CarType
			--JOIN TB_CarTypeGroup E WITH(NOLOCK) ON F.CarTypeGroupID=E.CarTypeGroupID
			WHERE A.TotalCount = B.NGCount

			--先串車在串車型
			SELECT  DISTINCT CarBrend										--廠牌
					,CarType		= E.CarTypeGroupCode					--車型
					,CarTypeName	= D.CarBrend + ' ' + D.CarTypeName		--廠牌+車型
					--20210505 ADD BY ADAM REASON.針對CROSS車型把TOYOTA拿掉
					--,CarTypeName	= CASE WHEN D.CarTypeName='COROLLA CROSS' THEN D.CarTypeName ELSE D.CarBrend + ' ' + D.CarTypeName END		--廠牌+車型
					,CarTypePic		= E.CarTypeImg			--車輛ICON對照
					,Operator		= O.OperatorICon		--運營商ICON
					,OperatorScore  = O.Score				--分數
					,Price_N		= P.PROPRICE_N			--平日
					,Price_H		= P.PROPRICE_H			--假日
					,Price          = dbo.FN_CalSpread(@SD, @ED, P.PROPRICE_N, P.PROPRICE_H) --租金計算 2020-11-10 eason
					,Seat			= E.Seat				--座椅數
					,IsRent			= CASE WHEN T3.CarTypeGroupCode IS NULL THEN 'N' ELSE 'Y' END
            into #Tmp_Final
			FROM (SELECT nowStationID,CarType FROM TB_Car WITH(NOLOCK) WHERE nowStationID=@StationID AND available<2) C
			JOIN TB_CarType D WITH(NOLOCK) ON C.CarType=D.CarType
			JOIN TB_CarTypeGroupConsist F WITH(NOLOCK) ON F.CarType=C.CarType
			JOIN TB_CarTypeGroup E WITH(NOLOCK) ON F.CarTypeGroupID=E.CarTypeGroupID
			JOIN TB_OperatorBase O WITH(NOLOCK) ON D.Operator=O.OperatorID
			JOIN TB_ProjectStation S WITH(NOLOCK) ON S.StationID=C.nowStationID AND S.IOType='O'
			JOIN TB_Project P WITH(NOLOCK) ON P.PROJID=S.PROJID AND P.SPCLOCK = 'Z'
			LEFT JOIN TB_iRentStationInfo I  WITH(NOLOCK) ON I.StationID = S.StationID
			--LEFT JOIN #BookingList BL ON C.nowStationID=BL.nowStationID AND C.CarType=BL.CarType
			LEFT JOIN (SELECT DISTINCT StationID,CarTypeGroupCode from #Tmp_iRenStation 
					WHERE RTRIM(CarTypeGroupCode) IN (case when @Tmp_CarTypes_Count > 0 then 
					(select top 1 ct.sValue from dbo.FN_StrListToTb(upper(@CarTypes)) ct where ct.sValue = CarTypeGroupCode)
					else RTRIM(CarTypeGroupCode) end)
				) T3 ON C.nowStationID=T3.StationID 
			--JOIN TB_OrderMain OM ON OM.ProjID = P.PROJID  
			WHERE C.nowStationID = @StationID 
			--AND ( 
			--	(OM.start_time between @SD AND @ED)  
			--	OR (OM.stop_time between @SD AND @ED) 
			--	OR (@SD BETWEEN OM.start_time AND OM.stop_time) 
			--	OR (@ED BETWEEN OM.start_time AND OM.stop_time) 
			--	OR (DATEADD(MINUTE,-30,@SD) between OM.start_time AND OM.stop_time) 
			--	OR (DATEADD(MINUTE,30,@ED) between OM.start_time AND OM.stop_time) 
			--)
			--20201212 ADD BY ADAM REASON.增加顯示時間的條件
			AND ((ShowStart BETWEEN @SD AND @ED) OR (ShowEnd BETWEEN @SD AND @ED) OR (@SD BETWEEN ShowStart AND ShowEnd) OR (@ED BETWEEN ShowStart AND ShowEnd))
			AND E.CarTypeGroupCode = (case when @Tmp_CarTypes_Count > 0 then (select top 1 sValue from dbo.FN_StrListToTb(@CarTypes) fn where fn.sValue = E.CarTypeGroupCode) else E.CarTypeGroupCode end)
			AND E.Seat = (case when @Tmp_Seats_Count > 0 then (select top 1 sValue from dbo.FN_StrListToTb(@Seats) fn where fn.sValue = E.Seat) else E.Seat end)	   
			--AND E.CarTypeGroupCode <> 'COROLLACRO'		--20210423 ADD BY ADAM REASON.先排掉cc

		    set @IsRent = iif ((select COUNT(*) from #Tmp_Final f where lower(f.IsRent) = 'y')>0,'Y','N')
		    select * from #Tmp_Final
		END
		ELSE
		BEGIN
			IF @SD IS NULL
			SET @SD=DATEADD(hour,8,GETDATE())

			IF @ED IS NULL
			SET @ED=DATEADD(DAY,1,@SD)

			--先串車在串車型
			SELECT  DISTINCT CarBrend										--廠牌
					,CarType		= E.CarTypeGroupCode					--車型
					,CarTypeName	= D.CarBrend + ' ' + D.CarTypeName		--廠牌+車型
					,CarTypePic		= E.CarTypeImg			--車輛ICON對照
					,Operator		= O.OperatorICon		--運營商ICON
					,OperatorScore  = O.Score				--分數
					,Price_N		= P.PROPRICE_N			--平日
					,Price_H		= P.PROPRICE_H			--假日
					,Price          = dbo.FN_CalSpread(@SD, @ED, P.PROPRICE_N, P.PROPRICE_H) --租金計算 2020-11-10 eason
					,Seat			= E.Seat				--座椅數
					,IsRent			= 'Y'					--無時間參數先當作可以預約
            into #Tmp_Final2
			FROM (SELECT nowStationID,CarType FROM TB_Car WITH(NOLOCK) WHERE nowStationID=@StationID AND available<2) C
			JOIN TB_CarType D WITH(NOLOCK) ON C.CarType=D.CarType
			JOIN TB_CarTypeGroupConsist F WITH(NOLOCK) ON F.CarType=C.CarType
			JOIN TB_CarTypeGroup E WITH(NOLOCK) ON F.CarTypeGroupID=E.CarTypeGroupID
			JOIN TB_OperatorBase O WITH(NOLOCK) ON D.Operator=O.OperatorID
			JOIN TB_ProjectStation S WITH(NOLOCK) ON S.StationID=C.nowStationID AND S.IOType='O'
			JOIN TB_Project P WITH(NOLOCK) ON P.PROJID=S.PROJID AND P.SPCLOCK = 'Z'
			LEFT JOIN TB_iRentStation I  WITH(NOLOCK) ON I.StationID = S.StationID
			--JOIN TB_OrderMain OM ON OM.ProjID = P.PROJID  
			WHERE C.nowStationID = @StationID 
			AND E.CarTypeGroupCode = (case when @Tmp_CarTypes_Count > 0 then (select top 1 sValue from dbo.FN_StrListToTb(@CarTypes) fn where fn.sValue = E.CarTypeGroupCode) else E.CarTypeGroupCode end)
			AND E.Seat = (case when @Tmp_Seats_Count > 0 then (select top 1 sValue from dbo.FN_StrListToTb(@Seats) fn where fn.sValue = E.Seat) else E.Seat end)
			--AND E.CarTypeGroupCode <> 'COROLLACRO'		--20210423 ADD BY ADAM REASON.先排掉cc

		    set @IsRent = iif ((select COUNT(*) from #Tmp_Final2 f where lower(f.IsRent) = 'y')>0,'Y','N')
		    select * from #Tmp_Final2		    
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

    drop table if exists #TB_OrderMain
	drop table if exists #BookingList
	drop table if exists #Tmp_Final
	drop table if exists #Tmp_Final2
	drop table if exists #Tmp_iRenStation
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetStationCarType';
GO