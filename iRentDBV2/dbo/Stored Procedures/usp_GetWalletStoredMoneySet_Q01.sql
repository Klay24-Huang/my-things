﻿/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetWalletStoredMoneySet_Q01
* 系    統 : IRENT
* 程式功能 : 錢包儲值-設定資訊
* 作    者 : AMBER
* 撰寫日期 : 20210909
* 修改日期 : 20210916 UPD BY AMBER REASON: 調整資料撈取邏輯
             20211111 UPD BY AMBER REASON: StoreType新增和泰PAY選項
			 20211129 UPD BY AMBER REASON: 從設定檔撈取儲值上限
			 20211201 UPD BY AMBER REASON: 因應APP調整StoreType參數值(信用卡:1=>0)
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetWalletStoredMoneySet_Q01]
(   
    @StoreType              TINYINT               , --儲值方式(0:信用卡,2:虛擬帳號,3:超商繳費,4:和泰PAY)
	@IDNO			        VARCHAR(20)           , --身分證號
	@Token                  VARCHAR(1024)         ,	--JWT TOKEN
    @LogID			        BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
)
AS
BEGIN
DECLARE @Error INT;
DECLARE @IsSystem TINYINT = 0 
DECLARE @FunName VARCHAR(50)= 'usp_GetWalletStoredMoneySet_Q01';
DECLARE @ErrorType TINYINT =0
DECLARE @NowTime DATETIME;
DECLARE @hasData INT;
DECLARE @StoreTypeDetail varchar(20) =''
DECLARE @defSet      TINYINT =0
DECLARE @StoreLimit　INT=0
DECLARE @StoreMax　　INT=0
DECLARE @UpperLimit INT=0

/*初始設定*/
SET @Error=0
SET @ErrorCode='0000'
SET @ErrorMsg='SUCCESS' 
SET @SQLExceptionCode=''
SET @SQLExceptionMsg=''
SET @hasData=0
SET @Token=ISNULL (@Token,'')
SET @NowTime=dbo.GET_TWDATE()

BEGIN TRY 
    DROP TABLE IF EXISTS #StoreLimit;

	IF @Token='' OR @IDNO=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

		--0.再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END

	IF @Error=0   
		BEGIN
			BEGIN
			SET @StoreType=CAST(@StoreType as varchar(10)) 
			END
	   
			SELECT C.Code0,C.Code1,C.Code2 INTO #StoreLimit  FROM TB_WalletCodeTable C
			WHERE CodeGroup='StoreLimit' AND UseFlg=1 AND C.Code3= @StoreType;

			SELECT @UpperLimit=Code0 FROM TB_WalletCodeTable WHERE CodeGroup='WalletMaxLimit'; --20211129 UPD BY AMBER REASON: 從設定檔撈取儲值上限

			SELECT 
			@StoreType AS StoreType,
			CASE @StoreType WHEN '3' THEN l.Code0 ELSE '' END AS StoreTypeDetail,
			ISNULL(w.WalletBalance,0) AS WalletBalance,
			IIF(@UpperLimit-ISNULL(w.WalletBalance,0)<0,0,@UpperLimit-ISNULL(w.WalletBalance,0)) AS Rechargeable, 
			l.Code1 AS StoreLimit,
			l.Code2 AS StoreMax,
			QuickBtns = 
		    (SELECT STRING_AGG(Code1,',') WITHIN GROUP (ORDER BY Code2)
			FROM TB_WalletCodeTable WHERE  Code3=@StoreType AND CodeGroup='StoreOption' AND UseFlg=1 AND (Code0=c.Code3 OR Code3 IN (0,2,4)) ),
			CASE  c.Code2 WHEN '' THEN 0 ELSE 1 END AS defSet
			FROM TB_MemberData m WITH(NOLOCK)
			LEFT JOIN TB_UserWallet w WITH(NOLOCK)  ON w.IDNO=m.MEMIDNO 			
			JOIN #StoreLimit l ON 1=1
			LEFT JOIN TB_WalletCodeTable c ON l.Code0=c.Code3
			WHERE m.MEMIDNO=@IDNO;
		END

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
	SET @IsSystem=1;
	SET @ErrorType=4;
	INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
    DROP TABLE IF EXISTS #StoreLimit;
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetWalletStoredMoneySet_Q01';
END


