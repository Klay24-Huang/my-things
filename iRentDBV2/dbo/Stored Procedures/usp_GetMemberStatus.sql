/****************************************************************
** Name: [dbo].[usp_GetMemberStatus]
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
** EXEC @Error=[dbo].[usp_GetMemberInfo]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Jet 
** Date:2020/10/13 下午 03:24:00 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/16 下午 03:24:00    |  ADAM   |          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetMemberStatus]
	@IDNO		            VARCHAR(10)           ,
	@Token                  VARCHAR(1024)         ,
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
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

SET @FunName='usp_GetMemberStatus';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=DATEADD(HOUR,8,GETDATE());

	BEGIN TRY
        IF @Token='' OR @IDNO=''
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR900'
		END
		        
        --0.再次檢核token
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

		--1.取得資料
		IF @Error=0
		BEGIN
			SELECT 
				 MEMIDNO
				,Login				= 'Y'		--登入
				,Register			= IrFlag	--註冊
				,Audit 
				,Audit_ID			= ISNULL(C.ID_1,0)				--身分證
				,Audit_Car			= ISNULL(C.CarDriver_1,0)		--汽車駕照
				,Audit_Motor		= ISNULL(C.MotorDriver_1,0)		--機車駕照
				,Audit_Selfie		= ISNULL(C.Self_1,0)			--自拍照
				,Audit_F01			= ISNULL(C.Law_Agent,0)			--法定代理人
				,Audit_Signture		= ISNULL(C.Signture,0)			--簽名檔
				--會員頁9.0卡狀態 (0:PASS 1:未完成註冊 2:完成註冊未上傳照片 3:身分審核中 4:審核不通過 5:身分變更審核中 6:身分變更審核失敗)
				,MenuCTRL			= CASE WHEN IrFlag=0 THEN 1
										   --身分證未上傳,駕照只要上其中一個
										   WHEN C.ID_1=0 OR (C.CarDriver_1<>1 OR C.MotorDriver_1<>1) OR C.Self_1=0 THEN 2
										   WHEN Audit=0 THEN 3
										   WHEN Audit=2 THEN 4
										   WHEN Audit=1 AND C.ID_1=1 THEN 5
										   WHEN Audit=1 AND C.CarDriver_1=1 THEN 5
										   WHEN Audit=1 AND C.MotorDriver_1=1 THEN 5
										   WHEN Audit=1 AND C.Self_1=1 THEN 5
										   WHEN Audit=1 AND C.Signture=1 THEN 5
										   WHEN Audit=1 AND C.ID_1=-1 THEN 6
										   WHEN Audit=1 AND C.CarDriver_1=-1 THEN 6
										   WHEN Audit=1 AND C.MotorDriver_1=-1 THEN 6
										   WHEN Audit=1 AND C.Self_1=-1 THEN 6
										   WHEN Audit=1 AND C.Signture=-1 THEN 6
										   ELSE 0 END
				--會員頁9.0狀態顯示 (這邊要通過審核才會有文字 MenuCTRL5 6才會有文字提示)
				,MenuStatusText		= CASE WHEN Audit=1 AND C.ID_1=1 THEN '身分變更審核中'
										   WHEN Audit=1 AND C.CarDriver_1=1 THEN '身分變更審核中'
										   WHEN Audit=1 AND C.MotorDriver_1=1 THEN '身分變更審核中'
										   WHEN Audit=1 AND C.Self_1=1 THEN '身分變更審核中'
										   WHEN Audit=1 AND C.Signture=1 THEN '身分變更審核中'
										   WHEN Audit=1 AND C.ID_1=-1 THEN '身分變更審核失敗'
										   WHEN Audit=1 AND C.CarDriver_1=-1 THEN '身分變更審核失敗'
										   WHEN Audit=1 AND C.MotorDriver_1=-1 THEN '身分變更審核失敗'
										   WHEN Audit=1 AND C.Self_1=-1 THEN '身分變更審核失敗'
										   WHEN Audit=1 AND C.Signture=-1 THEN '身分變更審核失敗'
										   ELSE '' END
				,BlackList			= 'N'
				,StatusText			= CASE WHEN A.IrFlag < 1 THEN '完成註冊/審核，即可開始租車'
										   WHEN A.Audit = 0 AND C.CarDriver_1=0 THEN '上傳駕照通過審核，即可開始租車'
										   WHEN A.Audit = 0 AND C.CarDriver_1=1 THEN '身分審核中~'
										   WHEN A.Audit = 2 AND C.CarDriver_1=-1 THEN '審核不通過，請重新提交資料'
										   ELSE '' END
				,StatusTextMotor	= CASE WHEN A.IrFlag < 1 THEN '完成註冊/審核，即可開始租車'
										   WHEN A.Audit = 0 AND C.MotorDriver_1=0 THEN '上傳駕照通過審核，即可開始租車'
										   WHEN A.Audit = 0 AND C.MotorDriver_1=1 THEN '身分審核中~'
										   WHEN A.Audit = 2 AND C.MotorDriver_1=-1 THEN '審核不通過，請重新提交資料'
										   ELSE '' END
				,NormalRentCount	= B.NormalRentBookingNowCount
				,AnyRentCount		= B.AnyRentBookingNowCount
				,MotorRentCount		= B.MotorRentBookingNowCount
				,TotalRentCount		= B.NormalRentBookingNowCount + B.AnyRentBookingNowCount + B.MotorRentBookingNowCount
				FROM TB_MemberData A WITH(NOLOCK)
				JOIN TB_BookingStatusOfUser B WITH(NOLOCK) ON A.MEMIDNO=B.IDNO
				LEFT JOIN TB_Credentials C WITH(NOLOCK) ON A.MEMIDNO=C.IDNO
				WHERE A.MEMIDNO=@IDNO
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberInfo';



GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberInfo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員狀態', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberInfo';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberInfo';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberInfo';