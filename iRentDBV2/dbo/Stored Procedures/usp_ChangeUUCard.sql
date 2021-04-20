/****** Object:  StoredProcedure [dbo].[usp_ChangeUUCard]    Script Date: 2021/4/15 上午 09:22:43 ******/

/****************************************************************
** Name: [dbo].[usp_ChangeUUCard]
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
** EXEC @Error=[dbo].[usp_ChangeUUCard]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Jet
** Date:2021/4/14 17:30:00 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2021/4/14 |  Jet		  |	First Release
** 17:30:00  |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_ChangeUUCard]
	@OrderNo			BIGINT					,	--訂單編號
	@IDNO				VARCHAR(10)				,	--身分證字號
	@CID				VARCHAR(10)				,	--車機編號
	@DeviceToken		VARCHAR(256)			,	--遠傳車機token
	@IsCens				INT						,	--是否為興聯車機(0:否;1:是)
	@OldCardNo			VARCHAR(30)				,	--舊悠遊卡卡號
	@NewCardNo			VARCHAR(30)				,	--新悠遊卡卡號
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
DECLARE @NowTime DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_ChangeUUCard';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @OrderNo=ISNULL(@OrderNo,0);

BEGIN TRY
	IF @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END

	IF @Error=0
	BEGIN
		IF EXISTS(SELECT * FROM TB_BindUUCard WITH(NOLOCK) WHERE OrderNumber=@OrderNo)
		BEGIN
			UPDATE TB_BindUUCard
			SET IDNO=@IDNO,
				CID=@CID,
				DeviceToken=@DeviceToken,
				IsCens=@IsCens,
				OldCardNo=@OldCardNo,
				NewCardNo=@NewCardNo,
				Result=0,
				UPTime=@NowTime
			WHERE OrderNumber=@OrderNo;

			INSERT INTO TB_BindUUCard_Log
			SELECT @NowTime,* FROM TB_BindUUCard WITH(NOLOCK)
			WHERE OrderNumber=@OrderNo;
		END
		ELSE
		BEGIN
			INSERT INTO TB_BindUUCard VALUES(@OrderNo,@IDNO,@CID,@DeviceToken,@IsCens,@OldCardNo,@NewCardNo,0,@NowTime,@NowTime);

			INSERT INTO TB_BindUUCard_Log
			SELECT @NowTime,* FROM TB_BindUUCard WITH(NOLOCK)
			WHERE OrderNumber=@OrderNo;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_ChangeUUCard';
GO

