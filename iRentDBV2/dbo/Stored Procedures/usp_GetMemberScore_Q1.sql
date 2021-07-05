-- =============================================
-- Author:      YEH
-- Create Date: 2021-05-18
-- Description: 取得會員積分明細
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetMemberScore_Q1]
(   
	@MSG		VARCHAR(10)	OUTPUT	,
	@IDNO		VARCHAR(10)			,	--身分證號
	@PageNo		INT					,	--第幾頁
	@PageSize	INT					,	--每頁幾筆
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
	DECLARE @FunName VARCHAR(50) = 'usp_GetMemberScore_Q1';
		
	SET @IDNO = ISNULL(@IDNO,'');
	SET @PageNo = ISNULL(@PageNo,1);
	SET @PageSize = ISNULL(@PageSize,10);
	SET @LogID = ISNULL(@LogID,0);

	BEGIN TRY
		IF @IDNO=''
		BEGIN
			SET @Error=1
			SET @ErrorCode = 'ERR900'
		END

		IF @Error = 0
		BEGIN
			--總積分
			SELECT SCORE FROM TB_MemberScoreMain WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

			--明細
			;WITH T
			AS (
				SELECT ROW_NUMBER() OVER (ORDER BY A.A_SYSDT DESC) AS RowNo,
					CONVERT(VARCHAR, A.A_SYSDT, 120) AS GetDate,
					A.SEQ,
					A.SCORE,
					CASE WHEN B.SCTYPENO='O' THEN A.UIDESC ELSE B.UIDESC END AS UIDESC
				FROM TB_MemberScoreDetail A WITH(NOLOCK)
				LEFT JOIN TB_ScoreDef B WITH(NOLOCK) ON B.SEQ=A.DEF_SEQ
				WHERE A.MEMIDNO=@IDNO AND A.UIDISABLE=0 AND A.ISPROCESSED=1
			),
			T2 AS (
				SELECT COUNT(1) TotalCount FROM T
			)

			SELECT *
			FROM T2, T
			WHERE RowNo BETWEEN (@PageNo - 1) * @PageSize  + 1 AND @PageNo * @PageSize;
		END

		--寫入錯誤訊息
		IF @Error=1
		BEGIN
			INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
			VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
		END

		--COMMIT TRAN 
	END TRY
	BEGIN CATCH
		--ROLLBACK TRAN
		SET @Error=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	--輸出系統訊息
	SELECT @ErrorCode[ErrorCode], @ErrorMsg[ErrorMsg], @SQLExceptionCode[SQLExceptionCode], @SQLExceptionMsg[SQLExceptionMsg], @Error[Error]
END

GO

