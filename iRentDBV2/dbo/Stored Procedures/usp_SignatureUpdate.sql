/****** Object:  StoredProcedure [dbo].[usp_SignatureUpdate]    Script Date: 2021/5/10 �W�� 09:22:18 ******/

/****************************************************************
** Name: [dbo].[usp_SignatureUpdate]
** Desc: 
**
** Return values: 0 ���\ else ���~
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
** Date:2020/8/6 �W�� 07:21:25 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/6 �W�� 07:21:25    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_SignatureUpdate]
	@IDNO                   VARCHAR(10)           ,
	@CrentialsFile          VARCHAR(150)           ,
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--�^�ǿ��~�N�X
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--�^�ǿ��~�T��
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--�^��sqlException�N�X
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--�^��sqlException�T��
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @tmpPWD VARCHAR(20);
DECLARE @NowDate DATETIME;

/*��l�]�w*/
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
			-- 20210507 UPD BY YEH REASON:�q�u����ñ�W�ɧאּ�P�_TB_tmpCrentialsPIC�O�_���ɮסA�S�ɮפ~�g�ܫݼf�ɡA�ç⪬�A�אּ�ݼf
			IF NOT EXISTS (SELECT * FROM TB_tmpCrentialsPIC WITH(NOLOCK) WHERE IDNO=@IDNO AND CrentialsType=11)
			BEGIN
				INSERT INTO TB_tmpCrentialsPIC (IDNO,CrentialsType,CrentialsFile,MKTime,UPDTime)
				VALUES (@IDNO,11,@CrentialsFile,@NowDate,@NowDate);

				UPDATE TB_Credentials
				SET Signture=1,
					UPDTime=@NowDate
				WHERE IDNO=@IDNO;
			END
			ELSE
			BEGIN
				UPDATE TB_tmpCrentialsPIC
				SET CrentialsFile=@CrentialsFile,
					UPDTime=@NowDate
				WHERE IDNO=@IDNO AND CrentialsType=11;

				UPDATE TB_Credentials
				SET Signture=1,
					UPDTime=@NowDate
				WHERE IDNO=@IDNO;
			END

			COMMIT TRAN;
		END
	END
	--�g�J���~�T��
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem)
	END
END TRY
BEGIN CATCH
	SET @Error=-1;
	SET @ErrorCode='ERR999';
	SET @ErrorMsg='�ڭn�g���~�T��';
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