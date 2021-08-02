/****************************************************************
** 用　　途：計算訂單達成的成就
*****************************************************************
** Change History
*****************************************************************
** 20210528 ADD BY YEH
** 20210531 UPD BY YEH REASON:資料寫入後就加總至成就檔
** 20210621 UPD BY YEH REASON:承租區域改用據點縣市判斷
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_CalOrderMedal]
	@OrderNo			BIGINT					,	--訂單編號
	@ActiveFlag			VARCHAR(1)				,	--A:待計算,N:不計算(金流未進帳)
	@MKUser				VARCHAR(50)				,	--來源程式
	@LogID				BIGINT					,
	@ErrorCode			VARCHAR(6)		OUTPUT	,	--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;

DECLARE @IDNO VARCHAR(10);			-- 會員帳號
DECLARE @ProjType INT;				-- 專案類型：0:同站;3:路邊;4:機車
DECLARE @start_time DATETIME;		-- 預約取車時間
DECLARE @stop_time DATETIME;		-- 預約還車時間
DECLARE @fine_Time DATETIME;		-- 逾時時間
DECLARE @lend_place VARCHAR(10);	-- 出車據點
DECLARE @final_start_time DATETIME;	-- 實際出車時間
DECLARE @final_stop_time DATETIME;	-- 實際還車時間
DECLARE @TransDiscount INT;			-- 轉乘優惠
DECLARE @start_mile FLOAT;			-- 取車里程
DECLARE @end_mile FLOAT;			-- 還車里程
DECLARE @mile INT;					-- 里程數
DECLARE @TotalHours FLOAT;			-- 使用時數(以小時為單位)
DECLARE @TotalGift INT;				-- 換電獎勵
DECLARE @CityID	INT;				-- 據點縣市代碼
DECLARE @TotalMinutes INT;			-- 使用分鐘數
DECLARE @ReturnDiff INT;			-- 還車差值

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_CalOrderMedal';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @OrderNo=ISNULL(@OrderNo,0);
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @TotalGift=0;
SET @CityID=0;
SET @TotalMinutes=0;
SET @ReturnDiff=0;
SET @MKUser=ISNULL(@MKUser,@FunName);

BEGIN TRY
	IF @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		SELECT @IDNO=A.IDNO,
			@ProjType=A.ProjType,
			@start_time=A.start_time,
			@stop_time=A.stop_time,
			@fine_Time=A.fine_Time,
			@lend_place=A.lend_place,
			@final_start_time=B.final_start_time,
			@final_stop_time=B.final_stop_time,
			@TransDiscount=B.TransDiscount,
			@start_mile=B.start_mile,
			@end_mile=B.end_mile
		FROM TB_OrderMain A WITH(NOLOCK)
		INNER JOIN TB_OrderDetail B WITH(NOLOCK) ON B.order_number=A.order_number
		WHERE A.order_number=@OrderNo;

		-- 里程數
		SET @mile = ROUND((@end_mile-@start_mile),0);

		-- 使用時數、使用分鐘數
		SELECT @TotalHours=TotalHour,@TotalMinutes=TotalMinute FROM dbo.FN_CONV_RENTDAYS(@final_start_time,@final_stop_time);

		-- 取換電獎勵
		SELECT @TotalGift=ISNULL(TotalGift,0) FROM TB_MotorChangeBattHis WITH(NOLOCK) WHERE order_number=@OrderNo;

		-- 取據點縣市代碼
		SELECT @CityID=ISNULL(CityID,0) FROM TB_iRentStation WITH(NOLOCK) WHERE StationID=@lend_place;



		-- ************************ 以下開始處理寫入徽章計次歷程檔 ************************

		-- 轉乘優惠
		IF @TransDiscount > 0
		BEGIN
			INSERT INTO TB_MedalHistory ([IDNO],[Event],[Action],[Amt],[MKTime],[MKUser],[UPDTime],[UPDUser],[ActiveFLG])
			VALUES(@IDNO,@OrderNo,'Transfer',1,@NowTime,@MKUser,@NowTime,@MKUser,@ActiveFlag);
		END

		IF @ProjType = 4	-- 機車
		BEGIN
			-- 租用次數
			INSERT INTO TB_MedalHistory ([IDNO],[Event],[Action],[Amt],[MKTime],[MKUser],[UPDTime],[UPDUser],[ActiveFLG])
			VALUES(@IDNO,@OrderNo,'Ride',1,@NowTime,@MKUser,@NowTime,@MKUser,@ActiveFlag);

			-- 騎乘里程數
			IF @mile > 0
			BEGIN
				INSERT INTO TB_MedalHistory ([IDNO],[Event],[Action],[Amt],[MKTime],[MKUser],[UPDTime],[UPDUser],[ActiveFLG])
				VALUES(@IDNO,@OrderNo,'RideMile',@mile,@NowTime,@MKUser,@NowTime,@MKUser,@ActiveFlag);
			END

			-- 租用分鐘數
			IF @TotalMinutes > 0
			BEGIN
				INSERT INTO TB_MedalHistory ([IDNO],[Event],[Action],[Amt],[MKTime],[MKUser],[UPDTime],[UPDUser],[ActiveFLG])
				VALUES(@IDNO,@OrderNo,'RideMinute',@TotalMinutes,@NowTime,@MKUser,@NowTime,@MKUser,@ActiveFlag);
			END

			-- 承租區域
			IF @CityID <> 0
			BEGIN
				SELECT @hasData=COUNT(*) FROM TB_MedalHistory WITH(NOLOCK) WHERE IDNO=@IDNO AND Action='RideArea' AND SubEvent=@CityID;
				IF @hasData = 0
				BEGIN
					INSERT INTO TB_MedalHistory ([IDNO],[Event],[Action],[SubEvent],[Amt],[MKTime],[MKUser],[UPDTime],[UPDUser],[ActiveFLG])
					VALUES(@IDNO,@OrderNo,'RideArea',@CityID,1,@NowTime,@MKUser,@NowTime,@MKUser,@ActiveFlag);
				END
			END

			-- 換電獎勵
			IF @TotalGift > 0
			BEGIN
				INSERT INTO TB_MedalHistory ([IDNO],[Event],[Action],[Amt],[MKTime],[MKUser],[UPDTime],[UPDUser],[ActiveFLG])
				VALUES(@IDNO,@OrderNo,'Electric',1,@NowTime,@MKUser,@NowTime,@MKUser,@ActiveFlag);
			END
		END
		ELSE	-- 汽車
		BEGIN
			--租用次數
			INSERT INTO TB_MedalHistory ([IDNO],[Event],[Action],[Amt],[MKTime],[MKUser],[UPDTime],[UPDUser],[ActiveFLG])
			VALUES(@IDNO,@OrderNo,'DriveTimes',1,@NowTime,@MKUser,@NowTime,@MKUser,@ActiveFlag);

			-- 駕駛里程數
			IF @mile > 0
			BEGIN
				INSERT INTO TB_MedalHistory ([IDNO],[Event],[Action],[Amt],[MKTime],[MKUser],[UPDTime],[UPDUser],[ActiveFLG])
				VALUES(@IDNO,@OrderNo,'DriveKM',@mile,@NowTime,@MKUser,@NowTime,@MKUser,@ActiveFlag);
			END

			--租用時數
			IF @TotalHours > 0
			BEGIN
				INSERT INTO TB_MedalHistory ([IDNO],[Event],[Action],[Amt],[MKTime],[MKUser],[UPDTime],[UPDUser],[ActiveFLG])
				VALUES(@IDNO,@OrderNo,'DriveHour',@TotalHours,@NowTime,@MKUser,@NowTime,@MKUser,@ActiveFlag);
			END

			-- 承租區域
			IF @CityID <> 0
			BEGIN
				SELECT @hasData=COUNT(*) FROM TB_MedalHistory WITH(NOLOCK) WHERE IDNO=@IDNO AND Action='DriveArea' AND SubEvent=@CityID;
				IF @hasData = 0
				BEGIN
					INSERT INTO TB_MedalHistory ([IDNO],[Event],[Action],[SubEvent],[Amt],[MKTime],[MKUser],[UPDTime],[UPDUser],[ActiveFLG])
					VALUES(@IDNO,@OrderNo,'DriveArea',@CityID,1,@NowTime,@MKUser,@NowTime,@MKUser,@ActiveFlag);
				END
			END

			-- 準時還車
			IF @ProjType = 0	--同站
			BEGIN
				SET @ReturnDiff = DATEDIFF(HOUR, @stop_time, @final_stop_time);	-- 還車差值

				IF @ReturnDiff > -24 AND @ReturnDiff <= 0
				BEGIN
					IF @final_stop_time < DATEADD(MINUTE,30,@final_stop_time)
					BEGIN
						INSERT INTO TB_MedalHistory ([IDNO],[Event],[Action],[Amt],[MKTime],[MKUser],[UPDTime],[UPDUser],[ActiveFLG])
						VALUES(@IDNO,@OrderNo,'StationRtn',1,@NowTime,@MKUser,@NowTime,@MKUser,@ActiveFlag);
					END
				END
			END
		END

		-- 20210531 UPD BY YEH REASON:資料寫入後就加總至成就檔
		EXEC [usp_CalMemberMedal] @IDNO,'','','','';
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CalOrderMedal';
GO

