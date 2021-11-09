/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetPreAmount
* 系    統 : IRENT
* 程式功能 : 取得訂單差額、預授權金額
* 作    者 : YEH
* 撰寫日期 : 20211109
* 修改日期 : 

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_GetPreAmount]
(   
	@MSG		VARCHAR(10)	OUTPUT	,
	@IDNO		VARCHAR(10)			,	-- 帳號
	@Token		VARCHAR(1024)		,	-- Token
	@OrderNo	BIGINT				,	-- 訂單編號
	@NeedToken	VARCHAR(1)			,	-- 是否需要Token
    @LogID		BIGINT
)
AS
BEGIN
    SET NOCOUNT ON

	DECLARE @Error INT = 0;
    DECLARE	@ErrorCode VARCHAR(6) = '0000';
    DECLARE	@ErrorMsg	NVARCHAR(100) = 'SUCCESS';
    DECLARE	@SQLExceptionCode	VARCHAR(10) = '';	
    DECLARE	@SQLExceptionMsg	NVARCHAR(1000) = '';
	DECLARE @IsSystem TINYINT = 1;
	DECLARE @ErrorType TINYINT = 4;
	DECLARE @FunName VARCHAR(50) = 'usp_GetPreAmount';
	DECLARE @NowTime DATETIME;
		
	SET @IDNO = ISNULL(@IDNO,'');
	SET @Token = ISNULL(@Token,'');
	SET @OrderNo = ISNULL(@OrderNo,0);
	SET @NeedToken = ISNULL(@NeedToken,'');
	SET @LogID = ISNULL(@LogID,0);
	SET @NowTime = DATEADD(HOUR,8,GETDATE());

	BEGIN TRY
		IF @IDNO='' OR @NeedToken='' OR @OrderNo=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode = 'ERR900';
		END

		-- Token檢核
		IF @Error=0
		BEGIN
			IF @NeedToken = 'Y'	-- 需要Token才檢查
			BEGIN
				IF NOT EXISTS(SELECT * FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND Rxpires_in>@NowTime)
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR101';
				END
				ELSE
				BEGIN
					IF NOT EXISTS(SELECT * FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND MEMIDNO=@IDNO)
					BEGIN
						SET @Error=1;
						SET @ErrorCode='ERR101';
					END
				END
			END
		END

		-- 取資料
		IF @Error = 0
		BEGIN
			-- 差額
			SELECT ISNULL(DiffAmount,0) AS DiffAmount FROM TB_OrderExtinfo WITH(NOLOCK) WHERE order_number=@OrderNo;

			-- TradeClose
			SELECT CloseID,AuthType,CloseAmout,ChkClose FROM TB_TradeClose WITH(NOLOCK) WHERE OrderNo=@OrderNo Order By AuthType;
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

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	--輸出系統訊息
	SELECT @ErrorCode ErrorCode, @ErrorMsg ErrorMsg, @SQLExceptionCode SQLExceptionCode, @SQLExceptionMsg SQLExceptionMsg, @Error Error
END

GO