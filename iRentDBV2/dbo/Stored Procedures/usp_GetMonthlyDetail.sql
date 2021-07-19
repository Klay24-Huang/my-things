/********************************************************************
功能：報表/月租報表
新增人員：胡湘梅(Umeko)
新增期間：2021/07/12
修改歷程：

***********************************************************************/
CREATE PROCEDURE [dbo].[usp_GetMonthlyDetail]
	@IDNO varchar(20) = null,
	@SD datetime = null,
	@ED datetime = null,
	@OrderNo BIGINT = null,
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	SET	@ErrorCode  = '0000'	
	SET	@ErrorMsg   = 'SUCCESS'	
	SET	@SQLExceptionCode = ''		
	SET	@SQLExceptionMsg = ''		
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_GetMonthlyDetail'

	DECLARE @Error INT;
	/*初始設定*/
	SET @Error=0;

	if (@SD is Not null And @ED is null) OR (@SD is null And @ED is not null)
	Begin
		Set @Error= 1
		Set @ErrorCode = 'ERR101'
		Set @ErrorMsg = 'Missing Parameter '
	End

	BEGIN TRY

	IF @Error = 0
	Begin
		if @SD is null And @ED is null
		Begin
			SELECT * 
			FROM VW_BE_GetMonthlyReportData 
			WHERE IDNO = Case When @IDNO is null Then IDNO Else @IDNO End 
			And OrderNo = Case When @OrderNo is null Then OrderNo Else @OrderNo End 
			ORDER BY OrderNo ASC
		End
		Else
		Begin
			SELECT * 
			FROM VW_BE_GetMonthlyReportData 
			WHERE IDNO = Case When @IDNO is null Then IDNO Else @IDNO End 
			And OrderNo = Case When @OrderNo is null Then OrderNo Else @OrderNo End 
			And MKTime between @SD And @ED
			ORDER BY OrderNo ASC
		End
	End 

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
	
	SET @IsSystem=1;
	SET @ErrorType=4;
	INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMonthlyDetail';