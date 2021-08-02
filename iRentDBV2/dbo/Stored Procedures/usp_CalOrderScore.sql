/****************************************************************
** 用　　途：計算訂單的會員積分
*****************************************************************
** Change History
*****************************************************************
** 20210531 ADD BY YEH
** 20210707 UPD BY YEH REASON:企劃說還車超過2021/7/7 12:00:00才開始計算
** 20210722 UPD BY YEH REASON:主動取消，前車用車中取消訂單不扣分
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_CalOrderScore]
	@OrderNo			BIGINT					,	-- 訂單編號
	@Action				VARCHAR(1)				,	-- 來源(A:取消預約 B:正常還車 C:後台強還)
	@ActionFlag			INT						,	-- (-1:不計算(金流未進帳) 0:待計算)
	@Mode				INT						,	-- 強還動作(3:系統操作異常/4:逾時未還/5:營運範圍外無法還車/6:車輛沒電/7:其他/0:無用)
	@MKUser				VARCHAR(50)				,	-- 程式來源
	@LogID				BIGINT					,
	@ErrorCode			VARCHAR(6)		OUTPUT	,	-- 回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT	,	-- 回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT	,	-- 回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT		-- 回傳sqlException訊息
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
DECLARE @booking_date DATETIME;		-- 預約時間
DECLARE @CarNo VARCHAR(10);			-- 車號
DECLARE @BookingDiff INT;			-- 預約差值
DECLARE @PickDiff INT;				-- 預計取車差值
DECLARE @UseDiff INT;				-- 使用差值
DECLARE @SEQ INT;					-- TB_ScoreDef SEQ(序號)
DECLARE @ReturnDiff INT;			-- 還車差值
DECLARE @ActionStartDate Datetime;	-- 積分計算起始時間
DECLARE @stop_pick_time DATETIME;	-- 最晚取車時間
DECLARE @start_pick_time DATETIME;	-- 最早取車時間

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_CalOrderScore';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @OrderNo=ISNULL(@OrderNo,0);
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @BookingDiff=0;
SET @PickDiff=0;
SET @SEQ=0;
SET @ReturnDiff=0;
SET @MKUser=ISNULL(@MKUser,@FunName);
SET @ActionStartDate = '2021/7/7 12:00:00';

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
			@booking_date=A.booking_date,
			@CarNo=A.CarNo,
			@stop_pick_time=A.stop_pick_time
		FROM TB_OrderMain A WITH(NOLOCK)
		LEFT JOIN TB_OrderDetail B WITH(NOLOCK) ON B.order_number=A.order_number
		WHERE A.order_number=@OrderNo;

		IF @ProjType = 0	-- 0:同站
		BEGIN
			IF @Action = 'A'	-- A:取消預約
			BEGIN
				-- 判斷此車號是否正在使用
				SELECT @hasData=COUNT(*) FROM TB_OrderMain WITH(NOLOCK) 
				WHERE CarNo=@CarNo AND (car_mgt_status>=4 and car_mgt_status<16) AND cancel_status=0
				AND start_time>='2021/1/1 00:00:00';

				SET @BookingDiff = DATEDIFF(MINUTE, @booking_date, @start_time);	-- 預約差值
				SET @PickDiff = DATEDIFF(MINUTE, @NowTime, @start_time);			-- 預計取車差值
				SET @UseDiff = DATEDIFF(MINUTE, @start_time, @stop_time);			-- 使用差值
				SET @start_pick_time = DATEADD(MINUTE, -30, @start_time);			-- 最早取車時間

				IF @hasData > 0	-- 車輛使用中
				BEGIN
					-- 20210722 UPD BY YEH REASON:在可取車區間內取消不扣分
					IF (@start_pick_time <= @NowTime AND @NowTime <= @stop_pick_time)
					BEGIN
						DECLARE @BeforeOrderNo INT;	-- 車輛使用中訂單編號

						SELECT @BeforeOrderNo=order_number FROM TB_OrderMain WITH(NOLOCK)
						WHERE CarNo=@CarNo AND (car_mgt_status>=4 and car_mgt_status<16) AND cancel_status=0
						AND start_time>='2021/1/1 00:00:00';

						-- 檢查前單是否有被扣分，沒扣過才扣分
						IF NOT EXISTS(SELECT * FROM TB_MemberScoreDetail WITH(NOLOCK) WHERE ORDERNO=@OrderNo AND DEF_SEQ=14)
						BEGIN
							-- 前單用戶延誤導致主動取消，前單用戶要扣分
							INSERT INTO TB_MemberScoreDetail([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
											[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
							SELECT @FunName,IDNO,@NowTime,@FunName,IDNO,@NowTime,
								IDNO,order_number,14,-20,0,-1
							FROM TB_OrderMain WITH(NOLOCK)
							WHERE order_number=@BeforeOrderNo;
						END
					END
					ELSE
					BEGIN	-- 不是在可取車區間內取消就要扣分
						IF @BookingDiff > 180	-- 預約時間 > 3小時
						BEGIN
							IF @PickDiff <= 180	-- 預計取車時間 < 3小時
							BEGIN
								IF @UseDiff >= 600	-- 用車時間>=10小時扣10分，<10扣5分
								BEGIN
									SET @SEQ = 3;
								END
								ELSE
								BEGIN
									SET @SEQ = 2;
								END
							END
						END
					END
				END
				ELSE	-- 車輛無使用就判斷是否要扣分
				BEGIN
					IF @BookingDiff > 180	-- 預約時間 > 3小時
					BEGIN
						IF @PickDiff <= 180	-- 預計取車時間 < 3小時
						BEGIN
							IF @UseDiff >= 600	-- 用車時間>=10小時扣10分，<10扣5分
							BEGIN
								SET @SEQ = 3;
							END
							ELSE
							BEGIN
								SET @SEQ = 2;
							END
						END
					END
				END
				
				IF @SEQ <> 0
				BEGIN
					INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
														[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
					SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
						@IDNO,@OrderNo,SEQ,SCORE,0,@ActionFlag
					FROM TB_ScoreDef WITH(NOLOCK)
					WHERE SEQ=@SEQ;
				END
			END

			IF @Action = 'B'	-- B:正常還車
			BEGIN
				IF @final_start_time >= @ActionStartDate	-- 20210707 UPD BY YEH REASON:企劃說實際取車時間超過2021/7/7 12:00:00才開始計算
				BEGIN
					SET @ReturnDiff = DATEDIFF(HOUR, @stop_time, @final_stop_time);	-- 還車差值

					-- 提早還車
					IF @ReturnDiff < -24	-- 當差值 < -24 是提前還車超過24小時
					BEGIN
						SET @SEQ=13;

						DECLARE @EarlyMultiple INT;	-- 提早還車扣點倍數
						SET @EarlyMultiple = ABS(ROUND(@ReturnDiff / 24,0));
					
						INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
															[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
						SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
							@IDNO,@OrderNo,SEQ,(@EarlyMultiple * SCORE),0,@ActionFlag
						FROM TB_ScoreDef WITH(NOLOCK)
						WHERE SEQ=@SEQ;
					END

					-- 逾時還車
					IF @final_stop_time > @stop_time	-- 實際換車時間 > 預計還車時間
					BEGIN
						SET @hasData = 0;
						-- 取該訂單是否有被預扣分數
						SELECT @hasData=COUNT(*) FROM TB_MemberScoreDetail WITH(NOLOCK) WHERE ORDERNO=@OrderNo AND DEF_SEQ=14;

						IF @hasData > 0	-- 有被預扣代表後續有訂單
						BEGIN
							IF @final_stop_time > DATEADD(MINUTE,10,@stop_time)	-- 判斷實際還車時間是否超過(預計還車時間+10分鐘)
							BEGIN
								-- 超過的話就是-20分，因此將預扣的狀態改成待計算
								UPDATE TB_MemberScoreDetail
								SET ISPROCESSED=0,
									U_PRGID=@MKUser,
									U_USERID=@IDNO,
									U_SYSDT=@NowTime
								WHERE ORDERNO=@OrderNo AND DEF_SEQ=14;

								SET @SEQ=14;
							END
							ELSE
							BEGIN
								-- 沒超過就一般逾時扣分
								SET @SEQ=15;

								INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
															[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
								SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
									@IDNO,@OrderNo,SEQ,SCORE,0,@ActionFlag
								FROM TB_ScoreDef WITH(NOLOCK)
								WHERE SEQ=@SEQ;

								-- 將預扣的狀態改為作廢
								UPDATE TB_MemberScoreDetail
								SET ISPROCESSED=2,
									U_PRGID=@MKUser,
									U_USERID=@IDNO,
									U_SYSDT=@NowTime
								WHERE ORDERNO=@OrderNo AND DEF_SEQ=14;
							END
						END
						ELSE
						BEGIN
							-- 沒預扣就一般逾時扣分
							SET @SEQ=15;

							INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
															[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
							SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
								@IDNO,@OrderNo,SEQ,SCORE,0,@ActionFlag
							FROM TB_ScoreDef WITH(NOLOCK)
							WHERE SEQ=@SEQ;
						END
					END

					-- 沒被扣分才給加分
					IF @SEQ = 0
					BEGIN
						SET @SEQ=1;

						INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
															[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
						SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
							@IDNO,@OrderNo,SEQ,SCORE,0,@ActionFlag
						FROM TB_ScoreDef WITH(NOLOCK)
						WHERE SEQ=@SEQ;
					END
				END
			END

			IF @Action = 'C'	-- C:後台強還
			BEGIN
				IF @final_start_time >= @ActionStartDate	-- 20210707 UPD BY YEH REASON:企劃說實際取車時間超過2021/7/7 12:00:00才開始計算
				BEGIN
					IF @Mode = 3	-- 強還動作(3:系統操作異常)：+1分
					BEGIN
						SET @SEQ=1;

						INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
															[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
						SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
							@IDNO,@OrderNo,SEQ,SCORE,0,@ActionFlag
						FROM TB_ScoreDef WITH(NOLOCK)
						WHERE SEQ=@SEQ;
					END

					IF @Mode = 4 OR @Mode = 5 OR @Mode = 7	-- 強還動作(4:逾時未還/5:營運範圍外無法還車/7:其他)
					BEGIN
						SET @ReturnDiff = DATEDIFF(HOUR, @stop_time, @final_stop_time);	-- 還車差值

						-- 提早還車
						IF @ReturnDiff < -24	-- 當差值 < -24 是提前還車超過24小時
						BEGIN
							SET @SEQ=13;

							DECLARE @BE_EarlyMultiple INT;	-- 提早還車扣點倍數
							SET @BE_EarlyMultiple = ABS(ROUND(@ReturnDiff / 24,0));
					
							INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
																[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
							SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
								@IDNO,@OrderNo,SEQ,(@BE_EarlyMultiple * SCORE),0,@ActionFlag
							FROM TB_ScoreDef WITH(NOLOCK)
							WHERE SEQ=@SEQ;
						END

						-- 逾時還車
						IF @final_stop_time > @stop_time	-- 實際換車時間 > 預計還車時間
						BEGIN
							SET @hasData = 0;
							-- 取該訂單是否有被預扣分數
							SELECT @hasData=COUNT(*) FROM TB_MemberScoreDetail WITH(NOLOCK) WHERE ORDERNO=@OrderNo AND DEF_SEQ=14;

							IF @hasData > 0	-- 有被預扣代表後續有訂單
							BEGIN
								IF @final_stop_time > DATEADD(MINUTE,10,@stop_time)	-- 判斷實際還車時間是否超過(預計還車時間+10分鐘)
								BEGIN
									-- 超過的話就是-20分，因此將預扣的狀態改成待計算
									UPDATE TB_MemberScoreDetail
									SET ISPROCESSED=-1,
										U_PRGID=@MKUser,
										U_USERID=@IDNO,
										U_SYSDT=@NowTime
									WHERE ORDERNO=@OrderNo AND DEF_SEQ=14;

									SET @SEQ=14;
								END
								ELSE
								BEGIN
									-- 沒超過就一般逾時扣分
									SET @SEQ=15;

									INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
																[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
									SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
										@IDNO,@OrderNo,SEQ,SCORE,0,@ActionFlag
									FROM TB_ScoreDef WITH(NOLOCK)
									WHERE SEQ=@SEQ;

									-- 將預扣的狀態改為作廢
									UPDATE TB_MemberScoreDetail
									SET ISPROCESSED=2,
										U_PRGID=@MKUser,
										U_USERID=@IDNO,
										U_SYSDT=@NowTime
									WHERE ORDERNO=@OrderNo AND DEF_SEQ=14;
								END
							END
							ELSE
							BEGIN
								-- 沒預扣就一般逾時扣分
								SET @SEQ=15;

								INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
																[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
								SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
									@IDNO,@OrderNo,SEQ,SCORE,0,@ActionFlag
								FROM TB_ScoreDef WITH(NOLOCK)
								WHERE SEQ=@SEQ;
							END
						END

					END

					IF @Mode = 5	-- 5:營運範圍外無法還車 要額外再扣分
					BEGIN
						SET @SEQ=54;

						INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
															[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
						SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
							@IDNO,@OrderNo,SEQ,SCORE,0,@ActionFlag
						FROM TB_ScoreDef WITH(NOLOCK)
						WHERE SEQ=@SEQ;
					END
				END
			END
		END
		ELSE	-- 3:路邊;4:機車
		BEGIN
			IF @final_start_time >= @ActionStartDate	-- 20210707 UPD BY YEH REASON:企劃說實際取車時間超過2021/7/7 12:00:00才開始計算
			BEGIN
				IF @Action = 'B'	-- B:正常還車
				BEGIN
					-- 沒被扣分才給加分
					IF @SEQ = 0
					BEGIN
						SET @SEQ=1;

						INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
															[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
						SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
							@IDNO,@OrderNo,SEQ,SCORE,0,@ActionFlag
						FROM TB_ScoreDef WITH(NOLOCK)
						WHERE SEQ=@SEQ;
					END
				END

				IF @Action = 'C'	-- C:後台強還
				BEGIN
					IF @Mode = 3	-- 強還動作(3:系統操作異常)：+1分
					BEGIN
						SET @SEQ=1;

						INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
															[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
						SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
							@IDNO,@OrderNo,SEQ,SCORE,0,@ActionFlag
						FROM TB_ScoreDef WITH(NOLOCK)
						WHERE SEQ=@SEQ;
					END

					IF @Mode = 5	-- 強還動作(5:營運範圍外無法還車)：-20分
					BEGIN
						SET @SEQ=54;

						INSERT INTO TB_MemberScoreDetail ([A_PRGID],[A_USERID],[A_SYSDT],[U_PRGID],[U_USERID],[U_SYSDT],
															[MEMIDNO],[ORDERNO],[DEF_SEQ],[SCORE],[UIDISABLE],[ISPROCESSED])
						SELECT @MKUser,@IDNO,@NowTime,@MKUser,@IDNO,@NowTime,
							@IDNO,@OrderNo,SEQ,SCORE,0,@ActionFlag
						FROM TB_ScoreDef WITH(NOLOCK)
						WHERE SEQ=@SEQ;
					END
				END
			END
		END

		-- 寫完Detail就呼叫加總SP將資料寫至Main
		EXEC [usp_CalMemberScore] @IDNO,'','','','';
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CalOrderScore';
GO

