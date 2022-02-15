/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_MonthlyRentEscrowCheck_Q01
* 系    統 : IRENT
* 程式功能 : 訂閱制履保檢核
* 作    者 : AMBER
* 撰寫日期 : 20220209
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_MonthlyRentEscrowCheck_Q01]
    @IdNo                   VARCHAR(10),
	@MonthlyRentId	        INT,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息    
AS
DECLARE @IsSystem tinyint=0
DECLARE @ErrorType tinyint = 0
DECLARE @LogID bigint = 0
DECLARE @FunName varchar(50)='usp_MonthlyRentEscrowCheck_Q01'
DECLARE @hasData INT=0
DECLARE @Error  INT = 0

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

BEGIN TRY
    SELECT @hasData=COUNT(1) FROM TB_EscrowHist WITH(NOLOCK) WHERE MonthlyNo=@MonthlyRentId;

	IF @hasData >0
	BEGIN
	   SET @Error=1;
	   SET @ErrorCode='ERR273';
	END 

	IF @Error=1
	BEGIN
	  INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
	  VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END
END TRY
BEGIN CATCH
	ROLLBACK TRAN
	SET @Error=-1;
	SET @ErrorCode='ERR999';
	SET @ErrorMsg='我要寫錯誤訊息';
	SET @SQLExceptionCode=ERROR_NUMBER();
	SET @SQLExceptionMsg=ERROR_MESSAGE();

    INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
    VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH	
RETURN @Error
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MonthlyRentEscrowCheck_Q01';



