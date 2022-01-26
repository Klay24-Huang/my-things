/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetEstimate_Q1
* 系    統 : IRENT
* 程式功能 : 取得專案價格
* 作    者 : EASON
* 撰寫日期 : 
* 修改日期 : 20211227 UPD BY YEH REASON:邏輯調整

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_GetEstimate_Q1]
(   
	@MSG		VARCHAR(10)	OUTPUT	,
	@ProjID		VARCHAR(10)			,	-- 專案代碼
	@CarType	VARCHAR(20)			,	-- 車型代碼
	@OrderNo	BIGINT				,	-- 訂單編號
	@IDNO		VARCHAR(10)			,	-- 帳號
	@SD			DATETIME			,	-- 起日
	@ED			DATETIME			,	-- 迄日
	@ProjType	INT					,	-- 專案類型
	@CarNo		VARCHAR(10)			,	-- 車號
    @LogID		BIGINT
)
AS
BEGIN
    SET NOCOUNT ON

	DECLARE @Error INT = 0;
    DECLARE	@ErrorCode VARCHAR(6) = '0000';
    DECLARE	@ErrorMsg NVARCHAR(100) = 'SUCCESS';
    DECLARE	@SQLExceptionCode VARCHAR(10) = '';
    DECLARE	@SQLExceptionMsg NVARCHAR(1000) = '';
	DECLARE @IsSystem TINYINT = 1;
	DECLARE @ErrorType TINYINT = 4;
	DECLARE @FunName VARCHAR(50) = 'usp_GetEstimate_Q1';

	DECLARE @NowDate DATETIME;
	
	SET @NowDate = dbo.GET_TWDATE();
	SET @LogID = ISNULL(@LogID,0);
	SET @IDNO = ISNULL(@IDNO,'');
	SET @SD = ISNULL(@SD,@NowDate);
	SET @ED = ISNULL(@ED,DATEADD(HOUR,1,@NowDate));
	SET @CarType = ISNULL(@CarType,'');
	SET @CarNo = ISNULL(@CarNo,'');

	BEGIN TRY
		IF @Error=0
		BEGIN
			DECLARE @Score INT=100;	-- 積分
			DECLARE @SPCLOCK VARCHAR(2);	-- 特殊身分

			IF @IDNO <> ''
			BEGIN
				SELECT @Score=ISNULL(SCORE,100) FROM TB_MemberScoreMain WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
			END

			IF @Score >= 60
			BEGIN
				SET @SPCLOCK = 'Z';
			END
			ELSE
			BEGIN
				SET @SPCLOCK = '90';
			END

			IF @OrderNo > 0
			BEGIN
				-- 有訂單編號就用TB_OrderMain去關聯
				SELECT G.PROJID, G.PROJTYPE, E.CarTypeGroupCode, H.PRICE, H.PRICE_H, G.Event
				FROM TB_OrderMain A WITH(NOLOCK)
				INNER JOIN TB_Car B WITH(NOLOCK) ON B.CarNo = A.CarNo
				INNER JOIN TB_CarType C WITH(NOLOCK) ON C.CarType = B.CarType
				INNER JOIN TB_CarTypeGroupConsist D WITH(NOLOCK) ON D.CarType = C.CarType
				INNER JOIN TB_CarTypeGroup E WITH(NOLOCK) ON E.CarTypeGroupID = D.CarTypeGroupID
				INNER JOIN TB_ProjectStation F WITH(NOLOCK) ON F.StationID = A.lend_place AND F.IOType = 'O'
				INNER JOIN TB_Project G WITH(NOLOCK) ON G.PROJID = F.PROJID
				INNER JOIN TB_ProjectDiscount H WITH(NOLOCK) ON H.ProjID = G.PROJID AND H.CARTYPE = B.CarType
				WHERE A.order_number = @OrderNo
				AND ((G.ShowStart BETWEEN @SD AND @ED) OR (G.ShowEnd BETWEEN @SD AND @ED) OR (@SD BETWEEN G.ShowStart AND G.ShowEnd) OR (@ED BETWEEN G.ShowStart AND G.ShowEnd))
				AND G.SPCLOCK = @SPCLOCK
				AND G.PROJTYPE = A.ProjType
				--AND G.PROJID <> A.ProjID
				ORDER BY H.PRICE, H.PRICE_H;
			END
			ELSE
			BEGIN
				-- 沒訂單編號就用車型代碼、據點代碼、專案類型、專案代碼當條件取結果
				SELECT DISTINCT A.PROJID, A.PROJTYPE, G.CarTypeGroupCode, C.PRICE, C.PRICE_H, A.Event
				FROM TB_Project A
				INNER JOIN TB_ProjectStation B ON B.PROJID = A.PROJID AND B.IOType = 'O'
				INNER JOIN TB_ProjectDiscount C ON C.ProjID = A.PROJID
				INNER JOIN TB_Car D ON D.nowStationID = B.StationID AND D.CarType = C.CARTYPE
				INNER JOIN TB_CarType E ON E.CarType = C.CARTYPE
				INNER JOIN TB_CarTypeGroupConsist F ON F.CarType = C.CARTYPE
				INNER JOIN TB_CarTypeGroup G ON G.CarTypeGroupID = F.CarTypeGroupID
				WHERE ((A.ShowStart BETWEEN @SD AND @ED) OR (A.ShowEnd BETWEEN @SD AND @ED) OR (@SD BETWEEN A.ShowStart AND A.ShowEnd) OR (@ED BETWEEN A.ShowStart AND A.ShowEnd))
				AND G.CarTypeGroupCode = CASE WHEN @CarType <> '' THEN @CarType ELSE G.CarTypeGroupCode END
				AND D.CarNo = CASE WHEN @CarNo <> '' THEN @CarNo ELSE D.CarNo END
				AND A.SPCLOCK = @SPCLOCK
				AND A.PROJTYPE = @ProjType
				--AND A.PROJID <> @ProjID
				AND A.IsRegional <> 1
				ORDER BY C.PRICE, C.PRICE_H;
			END
		END
	END TRY
	BEGIN CATCH
		SET @Error=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	--輸出系統訊息
	SELECT @ErrorCode [ErrorCode], @ErrorMsg [ErrorMsg], @SQLExceptionCode [SQLExceptionCode], @SQLExceptionMsg [SQLExceptionMsg], @Error [Error]
END
GO