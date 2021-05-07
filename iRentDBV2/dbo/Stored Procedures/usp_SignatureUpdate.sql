/****** Object:  StoredProcedure [dbo].[usp_SignatureUpdate]    Script Date: 2021/2/26 上� 11:25:56 ******/

/****************************************************************
** Name: [dbo].[usp_SignatureUpdate]
** Desc: 
**
** Return values: 0 �� else �誤
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
** EXEC @Error=[dbo].[usp_ChangePWD]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/6 上� 07:21:25 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/6 上� 07:21:25    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_SignatureUpdate]
	@IDNO                   VARCHAR(10)           ,
	@CrentialsFile          VARCHAR(150)           ,
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--�傳�誤仢�
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--�傳�誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--�傳sqlException仢�
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--�傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @tmpPWD VARCHAR(20);
DECLARE @NowDate DATETIME;

/*��設�*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_SignatureUpdate';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @IDNO=ISNULL (@IDNO,'');
SET @NowDate=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
	IF  @IDNO=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR100'
	END

	IF @Error=0
	BEGIN
		BEGIN TRAN
		SELECT @hasData=COUNT(1) FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO ;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR132';
			ROLLBACK TRAN;
		END
		ELSE
		BEGIN
			-- 20210507 UPD BY YEH REASON:從短租�簽�檔改�判�TB_tmpCrentialsPIC�否��案�沒�案�INSERT��審�，並��改���
			IF NOT EXISTS (SELECT * FROM TB_tmpCrentialsPIC WITH(NOLOCK) WHERE IDNO=@IDNO AND CrentialsType=11 AND CrentialsFile='')
			BEGIN
				INSERT INTO TB_tmpCrentialsPIC (IDNO,CrentialsType,CrentialsFile,MKTime,UPDTime)
				VALUES (@IDNO,11,@CrentialsFile,@NowDate,@NowDate);

				UPDATE TB_Credentials
				SET Signture=1,
					UPDTime=@NowDate
				WHERE IDNO=@IDNO;
			END
			COMMIT TRAN;
		END
	END
	--寫入�誤訊息
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem)
	END
END TRY
BEGIN CATCH
	SET @Error=-1;
	SET @ErrorCode='ERR999';
	SET @ErrorMsg='��寫錯誤�;
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
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem)
END CATCH
RETURN @Error

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SignatureUpdate';
GO

