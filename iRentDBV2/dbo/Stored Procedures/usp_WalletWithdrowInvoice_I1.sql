/****** Object:  StoredProcedure [dbo].[usp_WalletWithdrowInvoice_I1]    Script Date: 2021/9/30 �U�� 03:52:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* �{���W�� : usp_WalletWithdrowInvoice_I1
* �t    �� : IRENT
* �{���\�� : �g�J�]�Ȧ^�ߤ���O�o����T
* �@    �� : YANKEY
* ���g��� : 20210930
* �ק��� : 
			
Example :

EXEC usp_WalletWithdrowInvoice_I1 '',1,95,5,100,'51994648','�^�}���a','�x�_�����s��XX��X��X��','/WSDA5CF','1999','8562','0104235','0009853648526957','�]���s','999'

select TOP 10 * from TB_WalletWithdrawInvoiceInfo WITH(NOLOCK) ORDER BY MKTime DESC
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_WalletWithdrowInvoice_I1]
(   
	@MSG			VARCHAR(10) OUTPUT,
	@SEQNO			BIGINT,		--����TB_WalletyTradeMain.SEQNO(Mapping��)
	@SALAMT			INT,		--�P����B(�o�����t�|)
	@TAXAMT			INT,		--��~�|�B(�o���|�B)
	@FEEAMT			INT,		--����O�`�B(�o���`�B)
	@INV_CUSTID		VARCHAR(11), --�Ȥ�νs
	@INV_CUSTNM		NVARCHAR(30), --�Ȥ�W��
	@INV_ADDR		NVARCHAR(152), --�a�}
	@INVCARRIER		VARCHAR(10), --���㸹�X
	@NPOBAN			VARCHAR(10), --�R�߽X
	@RNDCODE		VARCHAR(4), --�o���H���X
	@RVBANK			VARCHAR(7), --�״ڻȦ�N��
	@RVACNT			VARCHAR(16), --�״ڻȦ�b��
	@RV_NAME		NVARCHAR(60), --�״ڤ�W
    @LogID			BIGINT
)
AS
BEGIN
    SET NOCOUNT ON
		
	DECLARE @NOW DATETIME = DATEADD(HOUR, 8, GETDATE())
	DECLARE @Error INT = 0
    DECLARE	@ErrorCode VARCHAR(6) = '0000'	
    DECLARE	@ErrorMsg  		   NVARCHAR(100) = 'SUCCESS'	
    DECLARE	@SQLExceptionCode  VARCHAR(10) = ''		
    DECLARE	@SQLExceptionMsg   NVARCHAR(1000) = ''	
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_WalletWithdrowInvoice_U1'

	BEGIN TRY
	    set @LogID = isnull(@LogID,'')
		

		IF @LogID = ''
		BEGIN
			SET @Error=1
			set @ErrorMsg = 'LogID����'
			SET @ErrorCode = 'ERR254'
		END

		if @Error = 0
		begin
			SET @Error=1
			SET @ErrorCode = 'ERR256'
				 IF isNull(@SEQNO,'')		= ''  SET @ErrorMsg += '���]�D���ѧO�X����g,'
			ELSE IF isNull(@SALAMT,'')		= ''  SET @ErrorMsg += '�P����B(�o�����t�|)����g,'
			ELSE IF isNull(@TAXAMT,'')		= ''  SET @ErrorMsg += '��~�|�B(�o���|�B)����g,'
			ELSE IF isNull(@FEEAMT,'')		= ''  SET @ErrorMsg += '����O�`�B(�o���`�B)����g,'
			--ELSE IF isNull(@INV_CUSTID,'')	= ''	SET @ErrorMsg += '�Ȥ�νs����g,'
			--ELSE IF isNull(@INV_CUSTNM,'')	= ''	SET @ErrorMsg += '�Ȥ�W�٥���g,'
			--ELSE IF isNull(@INV_ADDR,'')		= ''	SET @ErrorMsg += '�o���H�e�a�}����g,'
			--ELSE IF isNull(@INVCARRIER,'')	= ''	SET @ErrorMsg += '���㸹�X����g,'
			--ELSE IF isNull(@NPOBAN,'')		= ''	SET @ErrorMsg += '�R�߽X����g,'
			--ELSE IF isNull(@RNDCODE,'')		= ''	SET @ErrorMsg += '�o���H���X����g,'
			ELSE IF isNull(@RVBANK,'')		= ''  SET @ErrorMsg += '�״ڻȦ�N������g,'
			ELSE IF isNull(@RVACNT,'')		= ''  SET @ErrorMsg += '�״ڻȦ�b������g,'
			--ELSE IF isNull(@RV_NAME,'')		= ''  SET @ErrorMsg += '�״ڤ�W����g,'
			ELSE 
				BEGIN
					SET @Error=0
					SET @ErrorCode = '0000'
				END
		end


		IF @Error = 0
		BEGIN
			insert into dbo.TB_WalletWithdrawInvoiceInfo (SEQNO,SALAMT,TAXAMT,FEEAMT,INV_CUSTID,INV_CUSTNM,INV_ADDR,INVCARRIER,NPOBAN,RNDCODE,RVBANK,RVACNT,RV_NAME,UPDTime,UPDUser,UPDPRGID,MKTime,MKUser,MKPRGID) 
			SELECT SEQNO = @SEQNO , SALAMT = @SALAMT ,TAXAMT = @TAXAMT , FEEAMT = @FEEAMT , INV_CUSTID = @INV_CUSTID , INV_CUSTNM = @INV_CUSTNM , INV_ADDR =@INV_ADDR , INVCARRIER = @INVCARRIER , NPOBAN = @NPOBAN , RNDCODE = @RNDCODE , RVBANK = @RVBANK , RVACNT = @RVACNT , RV_NAME = @RV_NAME 
			, UPDTime = @NOW , UPDUser = 'SYS' , UPDPRGID = 'WithdrowI1' , MKTime = @NOW , MKUser = 'SYS' , MKPRGID = 'WithdrowI1'
		END

		--�g�J���~�T��
		IF @Error=1
		BEGIN
			INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
			VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
		END
	END TRY
	BEGIN CATCH
		SET @Error=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='�ڭn�g���~�T��';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	--��X�t�ΰT��
	SELECT @ErrorCode[ErrorCode], @ErrorMsg[ErrorMsg], @SQLExceptionCode[SQLExceptionCode], @SQLExceptionMsg[SQLExceptionMsg], @Error[Error]


END

GO


