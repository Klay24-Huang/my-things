-- =============================================
-- Author:      eason
-- Create Date: 2020-10-22
-- Description: 欠費查詢
-- =============================================
CREATE PROCEDURE [dbo].[usp_ArrearsQuery_U1]
(   
	@MSG			VARCHAR(10) OUTPUT,
	@IDNO			VARCHAR(20)       , --身分證號
	@IsSave			INT = 0           , --使否儲存查詢紀錄:0(不儲存), 1(儲存)
    @LogID			BIGINT,
    @ArrearsQuery	TY_ArrearsQuery  READONLY
)
AS
BEGIN
    SET NOCOUNT ON

	DECLARE @Error INT = 0
    DECLARE	@ErrorCode VARCHAR(6) = '0000'	
    DECLARE	@ErrorMsg  		   NVARCHAR(100) = 'SUCCESS'	
    DECLARE	@SQLExceptionCode  VARCHAR(10) = ''		
    DECLARE	@SQLExceptionMsg   NVARCHAR(1000) = ''	
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_ArrearsQuery_U1'

	BEGIN TRY
		BEGIN TRAN 	
	    
		DROP TABLE IF EXISTS #tmp_ArrearsQuery_ori
		DROP TABLE IF EXISTS #tmp_ArrearsQuery

		IF @LogID IS NULL OR @LogID = ''
		BEGIN
			SET @Error=1
			SET @ErrorCode = 'spErr'
			SET @ErrorMsg = 'LogID必填'
		END

		IF @Error = 0
		BEGIN
			DECLARE @ArrearsQuery_count INT 
			SELECT @ArrearsQuery_count = COUNT(*) FROM @ArrearsQuery
			IF @ArrearsQuery_count > 0
			BEGIN					
				SELECT 
				a.CARNO AS CarNo, --車號
				a.TAMT AS Amount, --待繳金額
				a.PAYMENTTYPE AS ArrearsKind, --欠費種類 1:租金,2:罰單,3:停車費,4:ETAG
				a.RNTDATE AS EndDate, --實際還車時間
				a.GIVEDATE AS StartDate, --實際取車時間
				a.IRENTORDNO,	--IRENT訂單編號
				a.ORDNO,	--短租預約編號
				a.CNTRNO, --短租合約編號
				a.POLNO,	--罰單編號
				a.INBRNHCD AS StationID, --取車據點
				CarType = ( SELECT TOP 1 c.CarType FROM TB_Car c WITH(NOLOCK) WHERE c.CarNo = a.CARNO), --車型代碼
				IsMotor = (SELECT TOP 1 c.IsMotor FROM TB_CarInfo c WITH(NOLOCK) WHERE c.CarNo = a.CARNO) ---是否是機車0否,1是
				INTO #tmp_ArrearsQuery_ori
				FROM @ArrearsQuery a

				--eason 0218:短租未存檔前進行欠費查詢會將已繳款但未完成存檔資料回傳
				--但已完成TB_NPR330Detail資料變更,(已確認)
				--會包含已繳款資訊,過慮已繳款
				declare @tmp_ArrearsQuery_ori_Count int = 0
				declare @tmp_ArrearsQuery_Count int = 0
				select @tmp_ArrearsQuery_ori_Count = count(*) from #tmp_ArrearsQuery_ori
				if @tmp_ArrearsQuery_ori_Count >0
				begin                    
					select distinct o.* 
					into #tmp_ArrearsQuery 
					from #tmp_ArrearsQuery_ori o 
					join TB_NPR330Detail d WITH(NOLOCK) on
					d.Amount = o.Amount and d.ArrearsKind = o.ArrearsKind and d.CarNo = o.CarNo 
					and d.CNTRNO = o.CNTRNO and d.IRENTORDNO = o.IRENTORDNO and d.ORDNO = o.ORDNO
					and d.POLNO = o.POLNO and d.StationID = o.StationID
					and d.StartDate = o.StartDate and d.EndDate = o.EndDate					
					where d.IsPay <> 1 
				    select @tmp_ArrearsQuery_Count = count(*) from #tmp_ArrearsQuery
				end

				DECLARE @MasteId int = 0
				IF @IsSave = 1 and @tmp_ArrearsQuery_Count > 0
				BEGIN		    
					INSERT INTO TB_NPR330Save ([IDNO], [IsPay], [useFlag], [MKTime], [UPDTime]) VALUES (@IDNO,0,1, DATEADD(HOUR,8,GETDATE()), DATEADD(HOUR,8,GETDATE()))				   
					SELECT @MasteId = @@IDENTITY 
					IF @MasteId > 0
					BEGIN
						INSERT INTO TB_NPR330Detail  
						SELECT 
						@MasteId,
						t.CarNo,
						t.Amount,
						t.ArrearsKind,
						t.StartDate,
						t.EndDate,
						t.IRENTORDNO,
						t.ORDNO,
						t.CNTRNO,
						t.POLNO,
						t.StationID,
						t.CarType,
						t.IsMotor,
						0,
						1,
						DATEADD(HOUR,8,GETDATE()),
						DATEADD(HOUR,8,GETDATE())
						FROM #tmp_ArrearsQuery t
					END
					ELSE
					BEGIN
						SET @Error=1
						SET @ErrorCode = 'spErr'
						SET @ErrorMsg = 'TB_NPR330Save存檔失敗'
					END
				END

				IF @Error = 0
			    BEGIN
					SELECT 
						NPR330Save_ID = ISNULL(@MasteId,0), --若IsSave為0, @MasteId為0
						Rent_Amount = ISNULL((SELECT SUM(t0.Amount) FROM #tmp_ArrearsQuery t0 WHERE t0.IRENTORDNO = t.IRENTORDNO AND t0.ArrearsKind = 1),0), --租金
						Ticket_Amount = ISNULL((SELECT SUM(t0.Amount) FROM #tmp_ArrearsQuery t0 WHERE t0.IRENTORDNO = t.IRENTORDNO AND t0.ArrearsKind = 2),0), --罰單
						Park_Amount = ISNULL((SELECT SUM(t0.Amount) FROM #tmp_ArrearsQuery t0 WHERE t0.IRENTORDNO = t.IRENTORDNO AND t0.ArrearsKind = 3),0), --停車費
						ETAG_Amount = ISNULL((SELECT SUM(t0.Amount) FROM #tmp_ArrearsQuery t0 WHERE t0.IRENTORDNO = t.IRENTORDNO AND t0.ArrearsKind = 4),0), --ETAG
						OperatingLoss_Amount = ISNULL((SELECT SUM(t0.Amount) FROM #tmp_ArrearsQuery t0 WHERE t0.IRENTORDNO = t.IRENTORDNO AND t0.ArrearsKind = 5),0),	--營損 / 代收停車費						
						Total_Amount = ISNULL((SELECT SUM(t0.Amount) FROM #tmp_ArrearsQuery t0 WHERE t0.IRENTORDNO = t.IRENTORDNO ),0), --欠費總和
						StartDate = (SELECT TOP 1 t0.StartDate FROM #tmp_ArrearsQuery t0 WHERE t0.IRENTORDNO = t.IRENTORDNO  ), --實際取車時間
						EndDate = (SELECT TOP 1 t0.EndDate FROM #tmp_ArrearsQuery t0 WHERE t0.IRENTORDNO = t.IRENTORDNO  ), --實際還車時間
						OrderNo = CASE WHEN t.IRENTORDNO IS NULL OR t.IRENTORDNO = '' THEN '-'
									ELSE
										CASE WHEN LEN(t.IRENTORDNO) <= 7 THEN
										'H' + RIGHT(REPLICATE('0', 7) + CAST(t.IRENTORDNO as NVARCHAR), 7) 
										ELSE t.IRENTORDNO END
									END,
						ShortOrderNo = (SELECT TOP 1 t0.CNTRNO FROM #tmp_ArrearsQuery t0 WHERE t0.IRENTORDNO = t.IRENTORDNO ), --短租合約編號
						StationName = ISNULL((SELECT TOP 1 s.StationName FROM TB_ManagerStation s WITH(NOLOCK) WHERE s.StationID = t.StationID),'-'), --取車據點
						CarType = ISNULL((SELECT TOP 1 ctg.CarTypeImg FROM TB_CarType ct WITH(NOLOCK)
											LEFT JOIN TB_CarTypeGroupConsist ctgc WITH(NOLOCK) ON ct.CarType=ctgc.CarType
											LEFT JOIN TB_CarTypeGroup ctg WITH(NOLOCK) ON ctgc.CarTypeGroupID=ctg.CarTypeGroupID
											LEFT JOIN #tmp_ArrearsQuery t0 ON t0.CarType=ct.CarType
											WHERE ct.CarType=t0.CarType
										),''),	--車型代碼
						IsMotor = ISNULL((SELECT TOP 1 c.IsMotor FROM TB_CarInfo c WITH(NOLOCK) WHERE c.CarNo = t.CarNo),0) ---是否是機車0否,1是				
					FROM (SELECT DISTINCT tmp.IRENTORDNO, tmp.StationID, tmp.CarNo FROM #tmp_ArrearsQuery tmp) t
				END
			END
			ELSE
			BEGIN
				SET @Error = 1
				SET @ErrorCode = 'spErr'
				SET @ErrorMsg = 'ArrearsQuery 為必填' 
			END
		END
		COMMIT TRAN 
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		SET @Error=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();
		--IF @@TRANCOUNT > 0
  --      BEGIN
  --          print 'rolling back transaction' /* <- this is never printed */
  --          ROLLBACK TRAN
  --      END

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	DROP TABLE IF EXISTS #tmp_ArrearsQuery_ori
	DROP TABLE IF EXISTS #tmp_ArrearsQuery

	--輸出系統訊息
	SELECT @ErrorCode[ErrorCode], @ErrorMsg[ErrorMsg], @SQLExceptionCode[SQLExceptionCode], @SQLExceptionMsg[SQLExceptionMsg], @Error[Error]
END
GO