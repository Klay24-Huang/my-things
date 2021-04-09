-- =============================================
-- Author:      eason
-- Create Date: 2021-03-31
-- Description: 車型下拉選單
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetCarTypeGroupList_Q1]
(   
	@MSG					VARCHAR(10) OUTPUT
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
	DECLARE @FunName VARCHAR(50) = 'usp_GetCarTypeGroupList_Q1'

	BEGIN TRY

		IF @Error=0
		BEGIN
			select distinct 
			c.CarBrend, c.CarTypeName, 
			CarTypeImg = case when isnull(g.CarTypeImg,'') ='' then lower(isnull(g.CarTypeName,'')) else g.CarTypeImg end ,
			g.Seat  
		    FROM TB_CarType c WITH(NOLOCK)
			JOIN TB_CarTypeGroupConsist gc WITH(NOLOCK) ON gc.CarType=c.CarType
			JOIN TB_CarTypeGroup g WITH(NOLOCK) ON g.CarTypeGroupID=gc.CarTypeGroupID
		END

	END TRY
	BEGIN CATCH
		SET @Error=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,9999,@IsSystem);
	END CATCH

	--輸出系統訊息
	SELECT @ErrorCode[ErrorCode], @ErrorMsg[ErrorMsg], @SQLExceptionCode[SQLExceptionCode], @SQLExceptionMsg[SQLExceptionMsg], @Error[Error]

END
GO