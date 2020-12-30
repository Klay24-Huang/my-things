/****************************************************************
** Name: [dbo].[usp_GetProjectPriceBase]
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
** EXEC @Error=[dbo].[usp_GetProjectPriceBase]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Adam 
** Date:2020/11/10 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/10|  Adam	  |          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetProjectPriceBase]
	@ProjID				VARCHAR(20),			--專案代號
	@CarNo				VARCHAR(10),			--車號
	@CarType			VARCHAR(20),			--車型
	@ProjType			INT,					--專案類型
	@IDNO				VARCHAR(10),			--帳號
	@LogID				BIGINT                ,
	@ErrorCode			VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;
DECLARE @Descript NVARCHAR(200);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetProjectPriceBase';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'取得專案價基本';
SET @NowTime=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
	
	IF @ProjID = ''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		CREATE TABLE #TB_Car (CarNo VARCHAR(20),CarType VARCHAR(20),nowStationID VARCHAR(10))
		IF @CarNo <> ''
		BEGIN
			INSERT INTO #TB_Car SELECT CarNo,CarType,nowStationID FROM TB_Car WITH(NOLOCK) WHERE CarNo=@CarNo
		END

		SELECT TOP 1 PROJID,PRICE,PRICE_H  
		,InsurancePerHours=CASE WHEN BU.IDNO IS NULL THEN II.InsurancePerHours ELSE K.InsurancePerHours END	--20201228 ADD BY ADAM REASON.多判斷訪客
		FROM VW_GetFullProjectCollectionOfCarTypeGroup  AS VW WITH(NOLOCK)
		LEFT JOIN #TB_Car AS Car WITH(NOLOCK) ON Car.CarType=VW.CARTYPE AND Car.nowStationID=VW.StationID
		LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=@IDNO
		LEFT JOIN TB_InsuranceInfo K WITH(NOLOCK) ON K.CarTypeGroupCode=VW.CarTypeGroupCode AND K.useflg='Y' AND BU.InsuranceLevel=K.InsuranceLevel	
		LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=VW.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3		--預設專用
		WHERE VW.SPCLOCK='Z' 
		AND ISNULL(CarNo,'') = CASE WHEN @CarNo<>'' THEN @CarNo ELSE ISNULL(CarNo,'') END
		AND VW.CarTypeGroupCode=CASE WHEN @CarType<>'' THEN @CarType ELSE VW.CarTypeGroupCode END
		AND VW.PROJID=@ProjID
		ORDER BY PRICE DESC

		DROP TABLE #TB_Car
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

RETURN 0
