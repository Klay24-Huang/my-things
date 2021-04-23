/****** Object:  StoredProcedure [dbo].[usp_BindUUCardJob]    Script Date: 2021/4/15 上午 11:25:26 ******/

/****************************************************************
** Name: [dbo].[usp_BindUUCardJob]
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
** EXEC @Error=[dbo].[usp_BindUUCardJob]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Jet
** Date:2021-04-15 11:09:50.390
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2021-04-15 11:09:50.390    |  Jet|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BindUUCardJob]
	@IDNO                   VARCHAR(10)				,--帳號
	@OrderNo                BIGINT					,--訂單編號
	@Result					INT						,--執行結果(0:未處理 1:成功 2:失敗)
	@CardNo                 VARCHAR(20)				,--悠遊卡卡號
	@LogID                  BIGINT					,--執行此筆的api log
	@ErrorCode 				VARCHAR(6)		OUTPUT	,--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT	,--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT	,--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	 --回傳sqlException訊息
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

SET @FunName='usp_BindUUCardJob';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @OrderNo=ISNULL(@OrderNo,0);

BEGIN TRY
	IF @IDNO='' OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		IF EXISTS(SELECT * FROM TB_BindUUCard WITH(NOLOCK) WHERE OrderNumber=@OrderNo AND IDNO=@IDNO)
		BEGIN
			UPDATE TB_BindUUCard
			SET Result=@Result,
				UPTime=@NowTime
			WHERE OrderNumber=@OrderNo AND IDNO=@IDNO;

			INSERT INTO TB_BindUUCard_Log
			SELECT @NowTime,* FROM TB_BindUUCard WITH(NOLOCK)
			WHERE OrderNumber=@OrderNo AND IDNO=@IDNO;
		END
		ELSE
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR900'
		END

		-- 執行成功才將MemberData的CardNo更新
		IF @Result = 1
		BEGIN
			UPDATE TB_MemberData
			SET CARDNO=@CardNo,
				U_PRGID=185,
				U_USERID=@IDNO,
				U_SYSDT=@NowTime 
			WHERE MEMIDNO=@IDNO;

			INSERT INTO TB_MemberData_Log
			SELECT 'U','185',@NowTime,* FROM TB_MemberData WHERE MEMIDNO=@IDNO;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BindUUCardJob';
GO

