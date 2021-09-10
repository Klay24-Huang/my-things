
/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_AlertMailLog_U01
* 系    統 : IRENT
* 程式功能 : 更新告警通知檔
* 作    者 : YEH
* 撰寫日期 : 20210906
* 修改日期 : 20210907 UPD BY YEH REASON:邏輯補正

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_AlertMailLog_U01]
	@Msg				VARCHAR(100)	OUTPUT	,	-- 回傳錯誤訊息
	@Sender				VARCHAR(250)			,	-- 發送信箱
	@HasSend			TINYINT					,	-- 是否發送：0:否;1:是;2:失敗;3:不處理
	@LogID				BIGINT					,
	@AlertMailLogs		TY_AlertMailLog READONLY
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @Error INT;
	DECLARE @ErrorCode VARCHAR(6);	--回傳錯誤代碼
	DECLARE	@ErrorMsg NVARCHAR(100);
	DECLARE @SQLExceptionCode VARCHAR(10);	--回傳sqlException代碼
	DECLARE @SQLExceptionMsg NVARCHAR(1000);	--回傳sqlException訊息

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
	SET @IsSystem=0;
	SET @FunName='usp_AlertMailLog_U01';
	SET @ErrorType=0;
	SET @hasData=0;
	SET @NowTime=DATEADD(HOUR,8,GETDATE());

	BEGIN TRY
		BEGIN TRAN

		DECLARE @Update_Count int = 0;
		SELECT @Update_Count = COUNT(*) FROM @AlertMailLogs;
		 
		IF @Update_Count=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR900'
		END

		IF @Error=0
		BEGIN
			UPDATE TB_AlertMailLog
			SET Sender=@Sender,
				HasSend=@HasSend,
				SendTime=CASE WHEN @HasSend = 1 THEN @NowTime ELSE NULL END,
				UPDTime=@NowTime
			FROM TB_AlertMailLog A
			INNER JOIN @AlertMailLogs B ON B.EventType=A.EventType AND B.AlertID=A.AlertID;
		END

		--寫入錯誤訊息
		IF @Error=1
		BEGIN
			INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
			VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
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

		SET @IsSystem=1;
		SET @ErrorType=4;
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	--輸出系統訊息
	SELECT @ErrorCode [ErrorCode], @ErrorMsg [ErrorMsg], @SQLExceptionCode [SQLExceptionCode], @SQLExceptionMsg [SQLExceptionMsg], @Error [Error]
END
GO

