/****************************************************************
** Name: [dbo].[usp_GetInsurancePrice]
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
** EXEC @Error=[dbo].[usp_GetInsurancePrice]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:ADAM 
** Date:2020/11/03
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/03|  ADAM	  |          First Release
**			 |			  |
*****************************************************************/

CREATE PROCEDURE [dbo].[usp_GetInsurancePrice]
	@IDNO               VARCHAR(10),
	@CarType			VARCHAR(10),
	@LogID                  BIGINT                , --               ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error INT;
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

SET @FunName='usp_GetInsurancePrice';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

	BEGIN TRY

		IF @LogID IS NULL OR @LogID = ''
		BEGIN
			SET @Error=1
			SET @ErrorMsg = 'LogID必填'
		END

		IF @CarType IS NULL OR @CarType = ''
		BEGIN
			SET @Error=1
			SET @ErrorMsg = 'CarType必填'
		END

		IF @IDNO IS NULL OR @IDNO=''
		BEGIN
			SET @Error=1
			SET @ErrorMsg = 'IDNO必填'
		END

		IF @Error = 0
		BEGIN
			SELECT DISTINCT BU.InsuranceLevel,II.InsurancePerHours
			FROM TB_InsuranceInfo II WITH(NOLOCK)
			LEFT JOIN TB_BookingInsuranceOfUser BU WITH(NOLOCK) ON ISNULL(BU.InsuranceLevel,3)=II.InsuranceLevel
			LEFT JOIN TB_CarTypeGroup E WITH(NOLOCK) ON II.CarTypeGroupCode=E.CarTypeGroupCode 
			LEFT JOIN TB_CarTypeGroupConsist F WITH(NOLOCK) ON F.CarTypeGroupID=E.CarTypeGroupID
			LEFT JOIN TB_CarType D WITH(NOLOCK) ON F.CarType=D.CarType
			WHERE II.useflg='Y' 
			AND BU.IDNO=@IDNO
			AND E.CarTypeGroupCode=@CarType
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