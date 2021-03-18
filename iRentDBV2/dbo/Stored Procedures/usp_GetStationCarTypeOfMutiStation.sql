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
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetStationCarTypeOfMutiStation]
	@StationIDs             VARCHAR(MAX)          , --據點代碼（1~多個）
	@SD                     DATETIME		      , --起日
	@ED                     DATETIME              , --迄日
	@CarType                VARCHAR(10)           , --車型群組代碼
	@IDNO					VARCHAR(10)			  , --會員代碼
	@Insurance				TINYINT				  , --是否計算安心服務
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
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetStationCarTypeOfMutiStation';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime = DATEADD(hour,8,GETDATE())

	BEGIN TRY
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

		SET @CarType = ISNULL(@CarType,'')

		--計算預估時數
		SELECT @TotalHours = dbo.FN_CalHours(@SD,@ED)

		--查詢
		IF @Error = 0
		BEGIN
		   BEGIN
				--取得特殊身分
				SELECT @SpecStatus=ISNULL(SPECSTATUS,'') FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO
				
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

				CREATE NONCLUSTERED INDEX ix_tempTB_OrderMain_CarNo ON #TB_OrderMain (CarNo)

				SELECT A.nowStationID,A.CarType
				INTO #BookingList
				FROM (
					SELECT nowStationID,c.CarType,TotalCount=COUNT(c.CarType) 
					FROM TB_Car c WITH(NOLOCK) 
					JOIN @tb_StationID s ON c.nowStationID=s.StationID 
					JOIN TB_CarInfo I WITH(NOLOCK) ON c.CarNo=I.CarNo		--20201231 ADD BY ADAM REASON.排除車機未設定
					WHERE c.available < 2 
					AND I.CID<>''		--20201231 ADD BY ADAM REASON.排除車機未設定
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

				--測試春節專案(暫用2/1~1/3)，先用寫死方式之後再改
				CREATE TABLE #SPDATE (STADATE DATETIME,ENDDATE DATETIME)
				INSERT INTO #SPDATE VALUES('2021-02-09','2021-02-17')
				SELECT * INTO #TB_Project FROM TB_Project WITH(NOLOCK) WHERE ((ShowStart BETWEEN @SD AND @ED) OR (ShowEnd BETWEEN @SD AND @ED) OR (@SD BETWEEN ShowStart AND ShowEnd) OR (@ED BETWEEN ShowStart AND ShowEnd))
				IF EXISTS(SELECT * FROM #SPDATE WHERE ((STADATE BETWEEN @SD AND @ED) OR (ENDDATE BETWEEN @SD AND @ED) OR (@SD BETWEEN STADATE AND ENDDATE) OR (@ED BETWEEN STADATE AND ENDDATE))) --AND @SpecStatus='99'
				BEGIN
					--在此時間的專案會只剩特殊專案
					DELETE FROM #TB_Project WHERE PROJID<>'R129' 
				END
				ELSE
				BEGIN
					DELETE FROM #TB_Project WHERE PROJID='R129'
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
				        ,IsFavStation           = CASE WHEN (SELECT COUNT(*) FROM VW_GetFavoriteStation fs WHERE fs.StationID = I.StationID AND fs.IDNO = @IDNO) > 0
						                          THEN 1 ELSE 0 END   
				--20201231 ADD BY ADAM REASON.排除車機未設定
				FROM (SELECT nowStationID,c.CarType,CarOfArea,c.CarNo FROM TB_Car c WITH(NOLOCK) 
						JOIN @tb_StationID s ON c.nowStationID=s.StationID 
						JOIN TB_CarInfo I WITH(NOLOCK) ON c.CarNo=I.CarNo
						WHERE c.available < 2 AND I.CID<>'') C
				JOIN TB_CarType D WITH(NOLOCK) ON C.CarType=D.CarType
				JOIN TB_CarTypeGroupConsist F WITH(NOLOCK) ON F.CarType=C.CarType
				JOIN TB_CarTypeGroup E WITH(NOLOCK) ON F.CarTypeGroupID=E.CarTypeGroupID
				JOIN TB_OperatorBase O WITH(NOLOCK) ON D.Operator=O.OperatorID
				JOIN TB_ProjectStation S WITH(NOLOCK) ON S.StationID=C.nowStationID AND S.IOType='O'
				JOIN #TB_Project P WITH(NOLOCK) ON P.PROJID=S.PROJID
				LEFT JOIN TB_ProjectDiscount PD WITH(NOLOCK) ON PD.ProjID = S.PROJID AND D.CarType = PD.CARTYPE --2020-12-17 eason
				LEFT JOIN TB_iRentStation I WITH(NOLOCK) ON I.StationID=C.nowStationID AND (@SD BETWEEN I.SDate AND I.EDate) AND (@ED BETWEEN I.SDate AND I.EDate)
				LEFT JOIN TB_City CT WITH(NOLOCK) ON CT.CityID = I.CityID --eason 2020-11-27
				LEFT JOIN TB_AreaZip AZ WITH(NOLOCK) ON AZ.AreaID = I.AreaID --eason 2020-11-27
				LEFT JOIN #BookingList BL ON C.nowStationID=BL.nowStationID AND C.CarType=BL.CarType
				LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=@IDNO
				LEFT JOIN TB_InsuranceInfo K WITH(NOLOCK) ON K.CarTypeGroupCode=E.CarTypeGroupCode AND K.useflg='Y' AND BU.InsuranceLevel=K.InsuranceLevel
				LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=E.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3		--預設專用
				LEFT JOIN TB_MilageSetting MS WITH(NOLOCK) ON MS.ProjID=P.PROJID AND MS.use_flag=1 AND @NowTime BETWEEN MS.SDate AND MS.EDate
				--LEFT JOIN (SELECT CarNo FROM #TB_OrderMain GROUP BY CarNo) T ON C.CarNo=T.CarNo
				WHERE 1=1
				AND E.CarTypeGroupCode = CASE WHEN @CarType<>'' THEN @CarType ELSE E.CarTypeGroupCode END
				AND SPCLOCK='Z'
				ORDER BY StationID,Price,IsRent DESC

				DROP TABLE #TB_OrderMain
				DROP TABLE #BookingList
			END
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetStationCarTypeOfMutiStation';

/*
TB_OrderMain	訂單基本資料檔	booking_status	預約單狀態
0 = 會員預約、
1 = 管理員清潔預約、
2 = 管理員保修預約、
3 = 延長用車狀態、
4 = 強迫延長用車狀態、
5 = 合約完成(已完成解除卡號動作)


TB_OrderMain	訂單基本資料檔	car_mgt_status	取還車狀態：
0 = 尚未取車、
1 = 已經上傳出車照片、
2 = 已經簽名出車單、
3 = 已經信用卡認證、
4 = 已經取車(記錄起始時間)、
11 = 已經紀錄還車時間、
12 = 已經上傳還車角度照片、
13 = 已經上傳還車車損照片、
14 = 已經簽名還車單、
15 = 已經信用卡付款、
16 = 已經檢查車輛完成並已經解除卡號

TB_OrderMain	訂單基本資料檔	cancel_status	訂單修改狀態：
0 = 無(訂單未刪除，正常預約狀態)、
1 = 修改指派車輛(此訂單因其他預約單強迫延長而更改過訂單 or 後台重新配車過 or 取車時無車，重新配車)、
2 = 此訂單被人工介入過(後台協助取還車 or 後台修改訂單資料)、
3 = 訂單已取消(會員主動取消 or 逾時15分鐘未取車)、
4 = 訂單已取消(因車輛仍在使用中又無法預約到其他車輛而取消)


TB_Car	車輛總表	available	目前狀態：
0:出租中;
1:可出租;
2:未上線
*/
GO


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eason', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetStationCarTypeOfMutiStation';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取得同站的專案資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetStationCarTypeOfMutiStation';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetStationCarTypeOfMutiStation';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetStationCarTypeOfMutiStation';