/****** Object:  StoredProcedure [dbo].[usp_BE_HandleAudit]    Script Date: 2021/1/15 下午 02:27:23 ******/
/*
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
*/
/****************************************************************
** Name: [dbo].[usp_BE_HandleAudit]
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
** EXEC @Error=[dbo].[usp_BE_HandleAudit]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/4 上午 06:14:18 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/4 上午 06:14:18    |  Eric|          First Release
** 2020/11/9 下午 16:25:18    |  Jerry|          通過審核@isReject設定為0
**			 |			  |
BEGIN TRAN
DECLARE
	@ErrorCode 				VARCHAR(6)		,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)		--回傳sqlException訊息
EXEC usp_BE_HandleAudit 'A125138195','11','','','','1980-08-11','0918790054','7','大道路42巷18號4樓','','1','1','','','0','0','','','','ibanezchen@gmail.com','0','Y','31275','72542022',@ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg OUTPUT
ROLLBACK TRAN
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_HandleAudit]
	@IDNO					VARCHAR(20)			  ,
	@Driver					VARCHAR(10)			  ,
	@SPECSTATUS				VARCHAR(10)			  ,
	@SPSD					VARCHAR(50)			  ,
	@SPED					VARCHAR(50)			  ,
	@Birth					VARCHAR(50)			  ,
	@Mobile					VARCHAR(20)			  ,
	@Area					INT					  ,
	@Addr					NVARCHAR(150)		  ,
	@UniCode				VARCHAR(10)			  ,

	@InvoiceType			TINYINT				  ,
	@AuditStatus			INT					  ,
	@NotAuditReason			VARCHAR(50)			  ,
	@RejectReason			NVARCHAR(150)		  ,
	@IsNew					TINYINT				  ,

	@MEMHTEL				VARCHAR(20)			  ,
	@MEMCOMTEL				VARCHAR(20)			  ,
	@MEMCONTRACT			NVARCHAR(60)		  ,
	@MEMCONTEL				VARCHAR(20)			  ,
	@MEMEMAIL				VARCHAR(150)		  ,

	@HasVaildEMail			TINYINT				  ,
	@MEMMSG					VARCHAR(1)			  ,
	@UserID					NVARCHAR(10)		  ,

	@LogID                  BIGINT                ,
	@MEMONEW				NVARCHAR(50)		  ,--20210115唐加
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
DECLARE @car_mgt_status TINYINT;
DECLARE @cancel_status TINYINT;
DECLARE @booking_status TINYINT;
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;
DECLARE @isReject TINYINT;
DECLARE @RentType TINYINT;
DECLARE @MEMCNAME NVARCHAR(10);
DECLARE @EMAIL VARCHAR(200);

PRINT 124
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_HandleAudit';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【取消訂單】';
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @RentType=0;
SET @UserID    =ISNULL (@UserID    ,'');

		BEGIN TRY
		
		 
		 IF @UserID='' OR @IDNO=''  
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
			IF @AuditStatus<1
			BEGIN
				SET @isReject=1;
			END
			--20201109 UPD BY JERRY 通過審核@isReject設定為0
			ELSE
			BEGIN
				SET @isReject=0;
			END
			DECLARE @AuditKind TINYINT;
			DECLARE @AuditID BIGINT;
			SET @AuditKind=0;
			--SELECT TOP 1 @AuditKind=AuditKind,@AuditID=AuditID FROM TB_MemberDataOfAutdit WITH(NOLOCK) WHERE MEMIDNO=@IDNO ORDER BY AuditID DESC
			--20201109 UPD BY JERRY 只取未審核的資料
			--SELECT TOP 1 @AuditKind=AuditKind,@AuditID=AuditID FROM TB_MemberDataOfAutdit WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND HasAudit=0 ORDER BY AuditID DESC
			--20201114 ADD BY ADAM 補上漏的資料
			SELECT TOP 1 @AuditKind=AuditKind,@AuditID=AuditID
			,@MEMCNAME=CASE WHEN ISNULL(@MEMCNAME,'')='' THEN MEMCNAME ELSE @MEMCNAME END--,@EMAIL=MEMEMAIL
			FROM TB_MemberDataOfAutdit WITH(NOLOCK) WHERE MEMIDNO=@IDNO AND HasAudit=0 ORDER BY AuditID DESC

			--20201124 UPD BY JERRY 如果取不到資料，改取最新一筆的已審核資料
			IF @MEMCNAME IS NULL
			SELECT TOP 1 @AuditKind=AuditKind,@AuditID=AuditID
			,@MEMCNAME=CASE WHEN ISNULL(@MEMCNAME,'')='' THEN MEMCNAME ELSE @MEMCNAME END--,@EMAIL=MEMEMAIL
			FROM TB_MemberDataOfAutdit WITH(NOLOCK) WHERE MEMIDNO=@IDNO ORDER BY AuditID DESC
			
			--20201124 UPD BY JERRY 如果取不到資料，改取最新一筆的已審核資料
			IF @MEMCNAME IS NULL
			SELECT @MEMCNAME=CASE WHEN ISNULL(@MEMCNAME,'')='' THEN MEMCNAME ELSE @MEMCNAME END--,@EMAIL=MEMEMAIL
			FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO 
			--INSERT INTO TB_AuditHistory([IDNO],[AuditUser],[AuditDate],[HandleItem],[HandleType],[IsReject],[RejectReason],	[RejectExplain])VALUES(@IDNO,@UserID,@NowTime,@IsNew,@AuditKind,@isReject,@NotAuditReason,@RejectReason);
			--20201124 UPD BY Jerry 更新審核結果欄位對應
			--INSERT INTO TB_AuditHistory([IDNO],[AuditUser],[AuditDate],[HandleItem],[HandleType],[IsReject],	[RejectExplain],[RejectReason])VALUES(@IDNO,@UserID,@NowTime,@IsNew,@AuditKind,@isReject,@NotAuditReason,@RejectReason);
			--20201124 UPD BY Jerry 增加RentType欄位
			INSERT INTO TB_AuditHistory([IDNO],[AuditUser],[AuditDate],[HandleItem],[HandleType],[IsReject],	[RejectExplain],[RejectReason],[RentType]) VALUES(@IDNO,@UserID,@NowTime,@IsNew,@AuditKind,@isReject,@NotAuditReason,@RejectReason,@RentType);
			--PRINT 234
		 	IF @AuditStatus=1 --審核通過
			BEGIN
				IF @Driver='10'
				BEGIN
					SET @RentType=1;
				END
				ELSE IF @Driver='11' OR @Driver='12'
				BEGIN
					SET @RentType=3;
				END
				ELSE IF @Driver='01' OR @Driver='02'
				BEGIN
					SET @RentType=2;
				END
		
				--PRINT @MEMCNAME

				UPDATE TB_MemberData
				SET [Audit]=1
					,RentType=@RentType
					,MEMBIRTH=@Birth
					,UNIMNO=@UniCode
					,U_USERID=@UserID
					,U_SYSDT=@NowTime 
					,MEMADDR=@Addr
					,MEMCITY=@Area
					,MEMSENDCD=@InvoiceType
					,MEMTEL=@Mobile
					,MEMCNAME=@MEMCNAME	--20201114 ADD BY ADAM 補上漏的資料
					,SPECSTATUS=@SPECSTATUS
					,SPSD=@SPSD
					,SPED=@SPED	--20201120 ADD BY JERRY 補上漏的資料
					--20201125 ADD BY JERRY 補上漏的資料
					,MEMHTEL=@MEMHTEL
					,MEMCOMTEL=@MEMCOMTEL
					,MEMCONTRACT=@MEMCONTRACT
					,MEMCONTEL=@MEMCONTEL
					,MEMEMAIL=@MEMEMAIL
					,HasVaildEMail=@HasVaildEMail
					,MEMMSG=@MEMMSG
					,U_PRGID=133
					,MEMONEW2=@MEMONEW--20210115唐加
				WHERE MEMIDNO=@IDNO;

				--PRINT 456

				UPDATE TB_MemberDataOfAutdit
				SET HasAudit=2
				,SPECSTATUS=@SPECSTATUS,SPSD=@SPSD,SPED=@SPED	--20201120 ADD BY JERRY 補上漏的資料
				--20201125 ADD BY JERRY 補上漏的資料
				,MEMHTEL=@MEMHTEL
				,MEMCOMTEL=@MEMCOMTEL
				,MEMCONTRACT=@MEMCONTRACT
				,MEMCONTEL=@MEMCONTEL
				,MEMEMAIL=@MEMEMAIL
				,MEMMSG=@MEMMSG
				WHERE AuditID=@AuditID

				IF EXISTS (SELECT * FROM TB_MemberData WITH(NOLOCK) WHERE MEMTEL=@Mobile AND HasCheckMobile=1 AND MEMIDNO!=@IDNO)
				BEGIN
					UPDATE TB_MemberData
					SET HasCheckMobile=0
						,U_SYSDT=GETDATE()
						,U_USERID=@UserID
						,U_PRGID=133
					WHERE MEMTEL=@Mobile 
					AND HasCheckMobile=1 
					AND MEMIDNO!=@IDNO;
				END
				
				--PRINT 456
			END
			ELSE
			BEGIN

					UPDATE TB_MemberData 
					SET [AuditMessage]=@NotAuditReason,[Audit]=2,U_USERID=@UserID,U_SYSDT=@NowTime
					WHERE MEMIDNO=@IDNO;
						
					UPDATE TB_MemberDataOfAutdit
					SET HasAudit=1
					WHERE AuditID=@AuditID
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleAudit';


