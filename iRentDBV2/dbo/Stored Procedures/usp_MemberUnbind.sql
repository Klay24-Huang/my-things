/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_MemberUnbind
* 系    統 : IRENT
* 程式功能 : 會員解綁
* 作    者 : 唐瑋祁
* 撰寫日期 : 20211126
* 修改日期 : 20211206 UPD BY YEH REASON:解綁邏輯調整，將會員相關檔案搬走，並將資料清回預設值
* Example  : 
DECLARE @Error              INT;
DECLARE @ErrorCode 			VARCHAR(6);		
DECLARE @ErrorMsg  			NVARCHAR(100);
DECLARE @SQLExceptionCode	VARCHAR(10);		
DECLARE @SQLExceptionMsg	NVARCHAR(1000);
EXEC @Error=[dbo].[usp_MemberUnbind] 'A129425984','08C1307C49CCDB90D21DA00A181EEA52E74FB6E609A8229982453FBE1A212A7E','MemberUnbindController','80345',@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg OUTPUT;
SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_MemberUnbind]
	@IDNO				VARCHAR(10)				,	--帳號
	@Token				VARCHAR(1024)			,	--Token
	@APIName			VARCHAR(100)			,	--功能名稱
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
DECLARE @APIID INT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_MemberUnbind';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @APIID=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO=ISNULL(@IDNO,'');
SET @Token=ISNULL(@Token,'');

BEGIN TRY
	IF @Token='' OR @IDNO=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900';
	END
		 
	-- 再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END

	-- 執行解綁
	IF @Error=0
	BEGIN
		BEGIN TRAN

		SELECT @APIID=APIID FROM TB_APIList WITH(NOLOCK) WHERE APIName=@APIName;

		-- 20211206 UPD BY YEH REASON:解綁邏輯調整，將會員相關檔案搬走，並將資料清回預設值

		-- 會員解綁資料流程：
		-- 1.TB_MemberData / TB_MemberDataOfAutdit / TB_Credentials / TB_CrentialsPIC / TB_tmpCrentialsPIC 資料搬至Cnl_LOG檔
		-- 2.UPDATE TB_MemberData 資料清空，並壓上isCancel=1
		-- 3.UPDATE TB_MemberDataOfAutdit 資料清空
		-- 4.DELETE TB_Credentials / TB_CrentialsPIC / TB_tmpCrentialsPIC
		-- 5.會員積分>100，分數退回100，明細檔刪除
		-- 6.徽章相關檔案刪除

		-- TB_MemberData 搬到 TB_MemberDataCnl_LOG
		INSERT INTO TB_MemberDataCnl_LOG (CancelDate,MEMIDNO,MEMCNAME,MEMPWD
			,MEMTEL,MEMHTEL,MEMCOUNTRY,MEMCITY,MEMADDR,MEMEMAIL
			,MEMCOMTEL,MEMCONTRACT,MEMCONTEL,MEMMSG
			,CARDNO,UNIMNO,MEMSENDCD,CARRIERID,NPOBAN
			,HasVaildEMail,HasCheckMobile,NeedChangePWD,HasBindSocial,Audit,AuditMessage
			,IrFlag,PayMode,RentType,SPECSTATUS,SPSD,SPED,PushREGID
			,MEMRFNBR,MEMONEW2,MEMUPDT,APPLYDT,AutoStored,isCancel
			,A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT)
		SELECT CONVERT(VARCHAR, @NowTime, 112),MEMIDNO,MEMCNAME,MEMPWD
			,MEMTEL,MEMHTEL,MEMCOUNTRY,MEMCITY,MEMADDR,MEMEMAIL
			,MEMCOMTEL,MEMCONTRACT,MEMCONTEL,MEMMSG
			,CARDNO,UNIMNO,MEMSENDCD,CARRIERID,NPOBAN
			,HasVaildEMail,HasCheckMobile,NeedChangePWD,HasBindSocial,Audit,AuditMessage
			,IrFlag,PayMode,RentType,SPECSTATUS,SPSD,SPED,PushREGID
			,MEMRFNBR,MEMONEW2,MEMUPDT,APPLYDT,ISNULL(AutoStored,0),isCancel
			,A_PRGID,A_USERID,A_SYSDT,U_PRGID,U_USERID,U_SYSDT
		FROM TB_MemberData WITH(NOLOCK)
		WHERE MEMIDNO=@IDNO;

		-- TB_MemberDataOfAutdit 搬到 TB_MemberDataOfAutditCnl_LOG
		INSERT INTO TB_MemberDataOfAutditCnl_LOG (CancelDate,AuditID,MEMIDNO,MEMCNAME,MEMTEL,MEMHTEL
			,MEMBIRTH,MEMCOUNTRY,MEMCITY,MEMADDR,MEMEMAIL,MEMCOMTEL
			,MEMCONTRACT,MEMCONTEL,MEMMSG,CARDNO,UNIMNO,MEMSENDCD
			,CARRIERID,NPOBAN,AuditKind,HasAudit,IsNew
			,SPECSTATUS,SPSD,SPED,MKTime,UPDTime)
		SELECT CONVERT(VARCHAR, @NowTime, 112),AuditID,MEMIDNO,MEMCNAME,MEMTEL,MEMHTEL
			,MEMBIRTH,MEMCOUNTRY,MEMCITY,MEMADDR,MEMEMAIL,MEMCOMTEL
			,MEMCONTRACT,MEMCONTEL,MEMMSG,CARDNO,UNIMNO,MEMSENDCD
			,CARRIERID,NPOBAN,AuditKind,HasAudit,IsNew
			,SPECSTATUS,SPSD,SPED,MKTime,UPDTime
		FROM TB_MemberDataOfAutdit WITH(NOLOCK)
		WHERE MEMIDNO=@IDNO;

		-- TB_Credentials 搬到 TB_CredentialsCnl_LOG
		INSERT INTO TB_CredentialsCnl_LOG (CancelDate,IDNO,ID_1,ID_2
			,CarDriver_1,CarDriver_2,MotorDriver_1,MotorDriver_2
			,Self_1,Law_Agent,Other_1,Business_1
			,Signture,MKTime,UPDTime)
		SELECT CONVERT(VARCHAR, @NowTime, 112),IDNO,ID_1,ID_2
			,CarDriver_1,CarDriver_2,MotorDriver_1,MotorDriver_2
			,Self_1,Law_Agent,Other_1,Business_1
			,Signture,MKTime,UPDTime
		FROM TB_Credentials WITH(NOLOCK)
		WHERE IDNO=@IDNO;

		-- TB_CrentialsPIC 搬到 TB_CrentialsPICCnl_LOG
		INSERT INTO TB_CrentialsPICCnl_LOG (CancelDate,IDNO,CrentialsType,CrentialsFile,LSFLG,MKTime,UPDTime)
		SELECT CONVERT(VARCHAR, @NowTime, 112),IDNO,CrentialsType,CrentialsFile,LSFLG,MKTime,UPDTime
		FROM TB_CrentialsPIC WITH(NOLOCK)
		WHERE IDNO=@IDNO;

		-- TB_tmpCrentialsPIC 搬到 TB_tmpCrentialsPICCnl_LOG
		INSERT INTO TB_tmpCrentialsPICCnl_LOG (CancelDate,IDNO,CrentialsType,CrentialsFile,MKTime,UPDTime)
		SELECT CONVERT(VARCHAR, @NowTime, 112),IDNO,CrentialsType,CrentialsFile,MKTime,UPDTime
		FROM TB_tmpCrentialsPIC WITH(NOLOCK)
		WHERE IDNO=@IDNO;

		-- 更新TB_MemberData，壓回預設值
		UPDATE TB_MemberData 
		SET isCancel=1,
			MEMCNAME=N'',
			MEMPWD='',
			MEMTEL='',
			MEMHTEL='',
			MEMBIRTH=NULL,
			MEMCOUNTRY=0,
			MEMCITY=0,
			MEMADDR=N'',
			MEMEMAIL='',
			MEMCOMTEL='',
			MEMCONTRACT=N'',
			MEMCONTEL='',
			MEMMSG='Y',
			CARDNO='',
			UNIMNO='',
			MEMSENDCD=2,
			CARRIERID='',
			NPOBAN='',
			HasVaildEMail=0,
			HasCheckMobile=0,
			NeedChangePWD=0,
			HasBindSocial=0,
			Audit=0,
			AuditMessage=N'',
			IrFlag=-1,
			PayMode=0,
			RentType=0,
			SPECSTATUS='00',
			SPSD='',
			SPED='',
			MEMONEW2=N'',
			MEMUPDT=NULL,
			APPLYDT=NULL,
			AutoStored=0,
			U_PRGID=@APIID,
			U_USERID=@IDNO,
			U_SYSDT=@NowTime
		WHERE MEMIDNO=@IDNO;

		-- 寫入TB_MemberData_Log
		INSERT INTO TB_MemberData_Log
		SELECT 'U',@APIID,@NowTime,* FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

		-- 更新TB_MemberDataOfAutdit，壓回預設值
		UPDATE TB_MemberDataOfAutdit
		SET MEMCNAME=N'',
			MEMTEL='',
			MEMHTEL='',
			MEMBIRTH=NULL,
			MEMCOUNTRY=0,
			MEMCITY=0,
			MEMADDR=N'',
			MEMEMAIL='',
			MEMCOMTEL='',
			MEMCONTRACT=N'',
			MEMCONTEL='',
			MEMMSG='Y',
			CARDNO='',
			UNIMNO='',
			MEMSENDCD=2,
			CARRIERID='',
			NPOBAN='',
			AuditKind=0,
			HasAudit=0,
			IsNew=0,
			SPECSTATUS='00',
			SPSD='',
			SPED='',
			UPDTime=@NowTime
		WHERE MEMIDNO=@IDNO;

		-- 寫入TB_MemberDataOfAutdit_Log
		INSERT INTO TB_MemberDataOfAutdit_Log
		SELECT 'U',@APIID,@NowTime,* FROM TB_MemberDataOfAutdit WHERE MEMIDNO=@IDNO;

		-- 更新TB_Credentials，壓回未上傳
		UPDATE TB_Credentials
		SET ID_1=0,
			ID_2=0,
			CarDriver_1=0,
			CarDriver_2=0,
			MotorDriver_1=0,
			MotorDriver_2=0,
			Self_1=0,
			Law_Agent=0,
			Other_1=0,
			Business_1=0,
			Signture=0,
			UPDTime=@NowTime
		WHERE IDNO=@IDNO;

		-- 刪除TB_CrentialsPIC
		DELETE FROM TB_CrentialsPIC WHERE IDNO=@IDNO;

		-- 刪除TB_tmpCrentialsPIC
		DELETE FROM TB_tmpCrentialsPIC WHERE IDNO=@IDNO;

		DECLARE @Score INT;

		SELECT @Score=SCORE FROM TB_MemberScoreMain WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

		IF @Score > 100
		BEGIN
			-- 會員積分壓回原始分數100分
			UPDATE TB_MemberScoreMain
			SET SCORE=100,
				U_PRGID=@FunName,
				U_USERID=@IDNO,
				U_SYSDT=@NowTime
			WHERE MEMIDNO=@IDNO;

			-- 刪除TB_MemberScoreDetail
			DELETE FROM TB_MemberScoreDetail WHERE MEMIDNO=@IDNO;
		END

		-- 刪除TB_MedalHistory
		DELETE FROM TB_MedalHistory WHERE IDNO=@IDNO;

		-- 刪除TB_MedalProgressStatus
		DELETE FROM TB_MedalProgressStatus WHERE IDNO=@IDNO;

		-- 刪除TB_MedalMileStone
		DELETE FROM TB_MedalMileStone WHERE IDNO=@IDNO;

		COMMIT TRAN;
	END

	--寫入錯誤訊息
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
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
		print 'rolling back transaction'; /* <- this is never printed */
		ROLLBACK TRAN;
	END
	SET @IsSystem=1;
	SET @ErrorType=4;
	INSERT INTO TB_ErrorLog(FunName,ErrorCode,ErrType,SQLErrorCode,SQLErrorDesc,LogID,IsSystem)
	VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH
RETURN @Error
GO

