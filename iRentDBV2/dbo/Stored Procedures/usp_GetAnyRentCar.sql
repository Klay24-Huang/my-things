/****** Object:  StoredProcedure [dbo].[usp_GetAnyRentCar]    Script Date: 2020/12/8 上午 09:58:50 ******/

/****************************************************************
** Name: [dbo].[usp_GetAnyRentCar]
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
** EXEC @Error=[dbo].[usp_GetMemberInfo]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:
** Date:
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 
**
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetAnyRentCar]
	@IDNO				VARCHAR(10)				,	--帳號
	@ShowALL			INT						,	--是否顯示全部(0:否 1:是)
	@MinLatitude		FLOAT					,	--最小緯度
	@MinLongitude		FLOAT					,	--最小經度
	@MaxLatitude		FLOAT					,	--最大緯度
	@MaxLongitude		FLOAT					,	--最大經度
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

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetAnyRentCar';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;

SET @IDNO=ISNULL(@IDNO,'');
SET @MinLatitude=ISNULL(@MinLatitude,0);
SET @MinLongitude=ISNULL(@MinLongitude,0);
SET @MaxLatitude=ISNULL(@MaxLatitude,0);
SET @MaxLongitude=ISNULL(@MaxLongitude,0);

BEGIN TRY
	--1.取得資料
	IF @Error=0
	BEGIN
		IF @ShowALL=1	--是否顯示全部(0:否 1:是)
		BEGIN
			SELECT [CarNo],
				   [CarType]=VW.CarTypeGroupCode,
				   CONCAT([CarBrend], ' ', [CarTypeName]) AS CarTypeName,
				   Area AS CarOfArea,
				   [PRONAME] AS ProjectName,
				   CASE WHEN H.[HolidayDate] IS NOT NULL THEN [PRICE_H]/10 ELSE [PRICE]/10 END AS Rental,
				   M.MilageBase AS Mileage,
				   0 AS ShowSpecial,
				   '' AS SpecialInfo,
				   [Latitude],
				   [Longitude],
				   OperatorICon AS [OPERATOR],
				   Score AS [OperatorScore],
				   CarTypeImg AS [CarTypePic],
				   Seat,
				   VW.[PROJID] AS ProjID,
				   CASE WHEN ISNULL(BU.InsuranceLevel, 3) >= 4 THEN 0 ELSE 1 END AS Insurance,
				   InsurancePrice=CASE WHEN K.InsurancePerHours IS NOT NULL THEN K.InsurancePerHours ELSE II.InsurancePerHours END
			FROM [VW_GetAllAnyRentData] VW WITH(NOLOCK)
			LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=@IDNO
			LEFT JOIN TB_InsuranceInfo K WITH(NOLOCK) ON K.CarTypeGroupCode=VW.CarTypeGroupCode AND K.useflg='Y' AND BU.InsuranceLevel=K.InsuranceLevel	
			LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=VW.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3		--預設專用
			LEFT JOIN TB_MilageSetting M WITH(NOLOCK) ON VW.PROJID=M.ProjID
			LEFT JOIN TB_Holiday H WITH(NOLOCK) ON H.HolidayDate=CONVERT(VARCHAR,GETDATE(),112) AND H.use_flag=1
			WHERE GPSTime>=DATEADD(MINUTE, -30, GETDATE())
			  AND available=1
			  AND CarNo NOT IN (SELECT CarNo FROM TB_OrderMain M WITH(NOLOCK) WHERE car_mgt_status < 16 AND cancel_status = 0 AND booking_status<5);
		END
		ELSE
		BEGIN
			SELECT [CarNo],
				   [CarType]=VW.CarTypeGroupCode,
				   CONCAT([CarBrend], ' ', [CarTypeName]) AS CarTypeName,
				   Area AS CarOfArea,
				   [PRONAME] AS ProjectName,
				   CASE WHEN H.[HolidayDate] IS NOT NULL THEN [PRICE_H]/10 ELSE [PRICE]/10 END AS Rental,
				   M.MilageBase AS Mileage,
				   0 AS ShowSpecial,
				   '' AS SpecialInfo,
				   [Latitude],
				   [Longitude],
				   OperatorICon AS [OPERATOR],
				   Score AS [OperatorScore],
				   CarTypeImg AS [CarTypePic],
				   Seat,
				   VW.[PROJID] AS ProjID,
				   CASE WHEN ISNULL(BU.InsuranceLevel, 3) >= 4 THEN 0 ELSE 1 END AS Insurance,
				   InsurancePrice=CASE WHEN K.InsurancePerHours IS NOT NULL THEN K.InsurancePerHours ELSE II.InsurancePerHours END
			FROM [VW_GetAllAnyRentData] VW WITH(NOLOCK)
			LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON BU.IDNO=@IDNO
			LEFT JOIN TB_InsuranceInfo K WITH(NOLOCK) ON K.CarTypeGroupCode=VW.CarTypeGroupCode AND K.useflg='Y' AND BU.InsuranceLevel=K.InsuranceLevel	
			LEFT JOIN TB_InsuranceInfo II WITH(NOLOCK) ON II.CarTypeGroupCode=VW.CarTypeGroupCode AND II.useflg='Y' AND II.InsuranceLevel=3		--預設專用
			LEFT JOIN TB_MilageSetting M WITH(NOLOCK) ON VW.PROJID=M.ProjID
			LEFT JOIN TB_Holiday H WITH(NOLOCK) ON H.HolidayDate=CONVERT(VARCHAR,GETDATE(),112) AND H.use_flag=1
			WHERE GPSTime>=DATEADD(MINUTE, -30, GETDATE())
			  AND available=1
			  AND CarNo NOT IN (SELECT CarNo FROM TB_OrderMain M WITH(NOLOCK) WHERE car_mgt_status < 16 AND cancel_status = 0 AND booking_status<5)
			  AND (Latitude>=@MinLatitude AND Latitude<=@MaxLatitude)
			  AND (Longitude>=@MinLongitude AND Longitude<=@MaxLongitude);
		END
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
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetAnyRentCar';
GO