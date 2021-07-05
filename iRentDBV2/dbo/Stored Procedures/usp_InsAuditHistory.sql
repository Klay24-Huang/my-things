/****************************************************************
** Name: [dbo].[usp_InsAuditHistory]
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
** EXEC @Error=[dbo].[usp_InsAuditHistory]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Jet 
** Date:2021-05-03 17:33:40.923
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2021-05-03 17:33:40.923    |  Jet|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsAuditHistory]
	@IDNO				VARCHAR(20)           ,
	@LogID				BIGINT                ,
	@ErrorCode			VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg			NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode	VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg	NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error		INT;
DECLARE @IsSystem	TINYINT;
DECLARE @FunName	VARCHAR(50);
DECLARE @ErrorType	TINYINT;
/*初始設定*/
DECLARE @hasData	INT;
DECLARE @NowDate	DATETIME;

--TB_MemberData
DECLARE @MEMCNAME	VARCHAR(50);	--姓名
DECLARE @MEMBIRTH	VARCHAR(10);	--生日
DECLARE @Age		INT;			--年齡
DECLARE @MEMADDR	VARCHAR(500);	--地址
--TB_Credentials
DECLARE @ID_1			INT;	--身份證正面 (0:未上傳 1:待審核 2:通過 -1:不通過)
DECLARE @ID_2			INT;	--身份證反面
DECLARE @CarDriver_1	INT;	--汽車駕證正面
DECLARE @CarDriver_2	INT;	--汽車駕照反面
DECLARE @MotorDriver_1	INT;	--機車駕照正面
DECLARE @MotorDriver_2	INT;	--機車駕照反面
DECLARE @Self_1			INT;	--自拍照
DECLARE @Law_Agent		INT;	--法定代理人
DECLARE @Signture		INT;	--簽名檔
--TB_AuditHistory
DECLARE @AuditHistoryID	INT;
DECLARE @AuditType		INT;	--審核類型 (0:審核,1:待審核)
DECLARE @IsDo	INT;

SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_InsAuditHistory';
SET @IsSystem=0;
SET @ErrorType=1;
SET @LogID=ISNULL(@LogID,0);
SET @hasData=0;
SET @NowDate=DATEADD(HOUR,8,GETDATE());
SET @IsDo=0;

BEGIN TRY
	IF @IDNO=''
	BEGIN
		SET @ErrorType=0;
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	IF @Error=0
	BEGIN
		SELECT @MEMCNAME=ISNULL(MEMCNAME,''),
			@MEMBIRTH=ISNULL(CONVERT(VARCHAR, MEMBIRTH, 111),''),
			@Age=DATEDIFF(MONTH,MEMBIRTH,DATEADD(HOUR,8,GETDATE()))/12,
			@MEMADDR=ISNULL(MEMADDR,'') 
		FROM TB_MemberData WITH(NOLOCK) 
		WHERE MEMIDNO=@IDNO;

		SELECT @ID_1=ISNULL(ID_1,0),
			@ID_2=ISNULL(ID_2,0),
			@CarDriver_1=ISNULL(CarDriver_1,0),
			@CarDriver_2=ISNULL(CarDriver_2,0),
			@MotorDriver_1=ISNULL(MotorDriver_1,0),
			@MotorDriver_2=ISNULL(MotorDriver_2,0),
			@Self_1=ISNULL(Self_1,0),
			@Law_Agent=ISNULL(Law_Agent,0),
			@Signture=ISNULL(Signture,0)
		FROM TB_Credentials WITH(NOLOCK) 
		WHERE IDNO=@IDNO;

		-- 有填基本資料(姓名、生日、地址) & 有上傳(身分證、自拍照、簽名檔、汽車駕照/機車駕照其一)
		IF @MEMCNAME<>'' AND @MEMBIRTH<>'' AND @MEMADDR<>'' 
			AND @ID_1>= 1 AND @ID_2>= 1 
			AND ((@CarDriver_1>= 1 AND @CarDriver_2>= 1) OR (@MotorDriver_1>= 1 AND @MotorDriver_2>= 1))
			AND @Self_1>= 1 AND @Signture>= 1
		BEGIN
			SET @IsDo=1;

			-- 未滿20要上傳法定代理人證件
			IF (@Age>= 18 AND @Age< 20)
			BEGIN
				IF (@Law_Agent>= 1)
				BEGIN
					SET @IsDo=1;
				END
				ELSE
				BEGIN
					SET @IsDo=0;
				END
			END

			-- 都符合條件才寫資料進TB_AuditHistory
			IF (@IsDo =1)
			BEGIN
				DECLARE @IsNew INT;	--是否為新加入(0:否;1:是)
				SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_AuditHistory WITH(NOLOCK) WHERE HandleItem=1 and IsReject=0 and IDNO=@IDNO;
				IF @hasData=0
				BEGIN
					SET @hasData=0;
					SELECT @hasData=COUNT(1) FROM MEMBER_NEW WITH(NOLOCK) WHERE IRENTFLG='Y' AND MEMIDNO=@IDNO;
					IF @hasData=0
					BEGIN
						SET @IsNew=1;	--都不存在就是新會員
					END
					ELSE
					BEGIN
						SET @IsNew=0;	--iRent1.0有審核過
					END
				END
				ELSE
				BEGIN
					SET @IsNew=0;	--審核通過
				END

				-- 20210507 UPD BY YEH REASON.先判斷TB_AuditHistory是否有資料，沒資料就直接INSERT
				IF NOT EXISTS(SELECT * FROM TB_AuditHistory WITH(NOLOCK) WHERE IDNO=@IDNO)
				BEGIN
					INSERT INTO TB_AuditHistory (IDNO,AuditUser,AuditDate,HandleItem,HandleType,IsReject,RejectReason,RejectExplain,RentType,MKTime,AuditType)
					VALUES (@IDNO,@IDNO,@NowDate,@IsNew,0,1,'','',0,@NowDate,1);
				END
				ELSE
				BEGIN
					-- 有資料的話，則判斷最後一筆是否為審核，是的話也INSERT一筆新的，不是則修改待審那筆的資料
					SELECT TOP 1 @AuditHistoryID=AuditHistoryID,
						@AuditType=AuditType 
					FROM TB_AuditHistory WITH(NOLOCK) 
					WHERE IDNO=@IDNO 
					ORDER BY AuditHistoryID DESC;

					IF @AuditType=0
					BEGIN
						INSERT INTO TB_AuditHistory (IDNO,AuditUser,AuditDate,HandleItem,HandleType,IsReject,RejectReason,RejectExplain,RentType,MKTime,AuditType)
						VALUES (@IDNO,@IDNO,@NowDate,@IsNew,0,1,'','',0,@NowDate,1);
					END
					ELSE
					BEGIN
						UPDATE TB_AuditHistory
						SET AuditDate=@NowDate
						WHERE AuditHistoryID=@AuditHistoryID;
					END
				END
			END
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsAuditHistory';
GO

