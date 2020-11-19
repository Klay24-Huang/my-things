﻿/****************************************************************
** Name: [dbo].[usp_CheckCanOpenDoor]
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
** EXEC @Error=[dbo].[usp_CheckCanOpenDoor]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/11 下午 02:55:46 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/11 下午 02:55:46    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_CheckCanOpenDoor]
	@IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@Token                  VARCHAR(1024)         ,
	@LogID                  BIGINT                ,
	@DeadLine               DATETIME        OUTPUT, --使用期限
	@Mobile                 VARCHAR(10)     OUTPUT, --手機
	@CID                    VARCHAR(10)     OUTPUT, --車機編號
	@StationID              VARCHAR(10)     OUTPUT, --出車據點
	@IsCens                 INT             OUTPUT, --是否為興聯車機
	@IsMotor                INT             OUTPUT, --是否為機車車機
	@deviceToken            VARCHAR(256)    OUTPUT, --遠傳車機token
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
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;
DECLARE @CarNo VARCHAR(10);
DECLARE @ProjType INT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_CheckCanOpenDoor';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @Descript=N'使用者操作【申請一次性開門】';
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @CarNo='';
SET @ProjType=5;
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token=ISNULL (@Token,'');
SET @DeadLine=@NowTime;
SET @Mobile='';

	BEGIN TRY
		IF @Token='' OR @IDNO=''  OR @OrderNo=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR900'
		END

		--0.再次檢核token
		IF @Error=0
		BEGIN
			SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
			ELSE
			BEGIN
				SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
				IF @hasData=0
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR101';
				END
			END
		END

		--取資料
		IF @Error=0
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_OpenDoor WHERE OrderNo=@OrderNo AND DeadLine>@NowTime AND IsVerify=0;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR221';
			END
			ELSE
			BEGIN
				SELECT @CarNo=CarNo,@StationID=lend_place,@ProjType=ProjType FROM TB_OrderMain WHERE order_number=@OrderNo;
				SELECT @DeadLine=DeadLine FROM TB_OpenDoor WHERE OrderNo=@OrderNo AND DeadLine>@NowTime AND IsVerify=0;
				SELECT @Mobile=MEMTEL FROM TB_MemberData WHERE MEMIDNO=@IDNO;
				SELECT @deviceToken=ISNULL(deviceToken,''),@CID=CID,@IsCens=IsCens,@IsMotor=IsMotor FROM TB_CarInfo WITH(NOLOCK) WHERE CarNo=@CarNo;

				IF @ProjType=4	-- 專案類型：0:同站;3:路邊;4:機車
				BEGIN
					UPDATE TB_OpenDoor
					SET IsVerify=1,UPDTime=@NowTime,nowStatus=2
					WHERE OrderNo=@OrderNo AND DeadLine>@NowTime AND IsVerify=0;
				END
				ELSE
				BEGIN
					UPDATE TB_OpenDoor
					SET IsVerify=1,UPDTime=@NowTime,nowStatus=1
					WHERE OrderNo=@OrderNo AND DeadLine>@NowTime AND IsVerify=0;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckCanOpenDoor';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckCanOpenDoor';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'一次性開門前判斷', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckCanOpenDoor';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckCanOpenDoor';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_CheckCanOpenDoor';