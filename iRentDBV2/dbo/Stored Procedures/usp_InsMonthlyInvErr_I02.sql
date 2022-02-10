/****** Object:  StoredProcedure [dbo].[usp_InsMonthlyInvErr_I02]    Script Date: 2022/1/26 上午 09:11:02 ******/
/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_InsMonthlyInvErr_I02
* 系    統 : IRENT
* 程式功能 : 寫入發票錯誤歷程
* 作    者 : ADAM
* 撰寫日期 : 20210826
* 修改日期 : 20210827 ADD BY ADAM REASON.補上debug資訊
             20220124 ADD BY AMBER REASON.新增PRGID、ErrCode、ErrMsg參數&六兄弟
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_InsMonthlyInvErr_I02]
(
	@ApiInput		NVARCHAR(4000),
	@IDNO			VARCHAR(20),
	@LogID			BIGINT,
	@MonthlyRentID	INT,
	@MonProjID		VARCHAR(10),
	@MonProPeriod	INT,
	@ShortDays		INT,
	@NowPeriod		SMALLINT,
	@PayTypeId		BIGINT = 0,
	@InvoTypeId		BIGINT = 0,
	@InvoiceType	VARCHAR(5),
	@CARRIERID		VARCHAR(20),
	@UNIMNO			VARCHAR(10),
	@NPOBAN			VARCHAR(10),
	@INVAMT			INT,	
	@PRGID          VARCHAR(50),
	@RtnCode        VARCHAR(30),
	@RtnMsg         NVARCHAR(100),
	@xError                 INT             OUTPUT,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
)
AS
BEGIN
	SET NOCOUNT ON

	SET @xError = 0
    SET	@ErrorCode  = '0000'	
    SET	@ErrorMsg   = 'SUCCESS'	
    SET	@SQLExceptionCode = ''		
    SET	@SQLExceptionMsg = ''	

	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_InsMonthlyInvErr_I02'
	DECLARE @NOW DATETIME = dbo.GET_TWDATE()
	DECLARE @IsMotor TINYINT
	--DECLARE @MonthlyRentID INT
	declare @spIn nvarchar(max), @SpNote nvarchar(max) = ''
	SET @PRGID=ISNULL(@PRGID,'')
	SET @RtnCode=ISNULL(@RtnCode,'')
	SET @RtnMsg=ISNULL(@RtnMsg,'')

	BEGIN TRY
		--20210827 ADD BY ADAM REASON.補上debug資訊
		select @spIn = isnull((
		select @ApiInput[ApiInput],@IDNO[IDNO], @LogID[LogID], @MonProjID[MonProjID], @MonProPeriod[MonProPeriod], @ShortDays[ShortDays],
		@NowPeriod[NowPeriod], @PayTypeId[PayTypeId], @InvoTypeId[InvoTypeId],
		@InvoiceType[InvoiceType], @CARRIERID[CARRIERID], @UNIMNO[UNIMNO],
		@NPOBAN[NPOBAN],@INVAMT[INVAMT],@PRGID[PRGID],@RtnCode[RtnCode],@RtnMsg[RtnMsg]
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),'{}')

		IF @xError = 0
		IF NOT EXISTS (SELECT 1 FROM TB_MonthlyInvErrLog WITH(NOLOCK) WHERE MonthlyRentID=@MonthlyRentID)
		BEGIN		   
			INSERT INTO TB_MonthlyInvErrLog 
			(
				MKTime, UPDTime, TransFlg, apiInput, IDNO, LOGID,
				MonthlyRentID, MonProjID, MonProPeriod, ShortDays,
				NowPeriod, PayTypeId, InvoTypeId, InvoiceType,
				UNIMNO, CARRIERID, NPOBAN, INVAMT,ErrCode,ErrMsg,
				A_PRGID,A_USERID,U_PRGID,U_USERID
			)
			VALUES
			(
				@NOW, @NOW, 'N', @apiInput, @IDNO, @LOGID,
				@MonthlyRentID, @MonProjID, @MonProPeriod, @ShortDays,
				@NowPeriod, @PayTypeId, @InvoTypeId, @InvoiceType,
				@UNIMNO, @CARRIERID, @NPOBAN, @INVAMT,@RtnCode,@RtnMsg,
				@PRGID,@IDNO,@PRGID,@IDNO
			)			
		END
		ELSE
		BEGIN
		   UPDATE TB_MonthlyInvErrLog SET apiInput=@apiInput,IDNO=@IDNO,LOGID=@LogID,
		   MonProjID=@MonProjID,MonProPeriod=@MonProPeriod,ShortDays=@ShortDays,
		   NowPeriod=@NowPeriod,PayTypeId=@PayTypeId,InvoTypeId=@InvoTypeId,InvoiceType=@InvoiceType,
		   UNIMNO=@UNIMNO,CARRIERID=@CARRIERID,NPOBAN=@NPOBAN,INVAMT=@INVAMT,ErrCode=@RtnCode,ErrMsg=@RtnMsg,
		   U_PRGID=@PRGID,U_USERID=@IDNO,UPDTime=@NOW
		   WHERE MonthlyRentID=@MonthlyRentID AND ISNULL(TransFlg,'N')='N';
		END 

		IF @@ERROR <> 0 
			BEGIN
				SET @xError = 1
				SET @ErrorMsg = '寫入失敗'
				SET @ErrorCode = 'ERR999'
			END
		
	END TRY
	BEGIN CATCH
		--ROLLBACK TRAN
		SET @xError=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH	

	--20210827 ADD BY ADAM REASON.補上debug資訊
	if @xError <> 0 --sp錯誤log
		exec dbo.usp_SpSubsLog @FunName, @spIn, null,@xError, @ErrorCode, @ErrorMsg, @SQLExceptionCode, @SQLExceptionMsg, @SpNote
END	



