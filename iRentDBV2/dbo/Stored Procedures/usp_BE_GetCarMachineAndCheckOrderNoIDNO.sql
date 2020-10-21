﻿/****************************************************************
** Name: [dbo].[usp_BE_GetCarMachineAndCheckOrderNoIDNO]
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
** EXEC @Error=[dbo].[usp_BE_GetCarMachineAndCheckOrderNoIDNO]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/20 下午 02:40:13 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/20 下午 02:40:13    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_GetCarMachineAndCheckOrderNoIDNO]
    @CardNo                 VARCHAR(20)           ,
	@OrderNo                BIGINT                ,
	@UserId                 VARCHAR(10)           ,
	@LogID                  BIGINT                ,

	@IsCens                 TINYINT         OUTPUT,
	@CID                    VARCHAR(10)     OUTPUT,
	@deviceToken            VARCHAR(64)     OUTPUT,
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
DECLARE @IDNO VARCHAR(20);
DECLARE @NowTime DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_GetCarMachineAndCheckOrderNoIDNO';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @CardNo=ISNULL (@CardNo,'');

SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO='';

SET @OrderNo=ISNULL (@OrderNo,0);


		BEGIN TRY
	
		 
		 IF  @IDNO=''  OR @OrderNo=0
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
				SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo;
				IF @hasData=0
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR720'
				END
				ELSE
				BEGIN
					SELECT @CID=ISNULL([CID],''),@deviceToken=ISNULL([deviceToken],''),@IsCens=ISNULL([IsCens],''),@IDNO=ISNULL(OrderMain.IDNO,'')
					FROM [TB_CarInfo] AS CarInfo
					INNER JOIN TB_OrderMain AS OrderMain ON OrderMain.CarNo=CarInfo.CarNo AND OrderMain.order_number=@OrderNo
					INNER JOIN TB_MemberData AS MemberData ON MemberData.MEMIDNO=OrderMain.IDNO
					IF @deviceToken='' AND @IsCens=0
					BEGIN
						SET @Error=1;
					    SET @ErrorCode='ERR721'
					END
		
					IF @Error=0
					BEGIN
						IF @CID=''
						BEGIN
							SET @Error=1;
					        SET @ErrorCode='ERR723'
						END
					END
				END
				IF @Error=0
				BEGIN
					 UPDATE TB_MemberData SET CARDNO=@CardNo,U_USERID=@UserId,U_SYSDT=@NowTime WHERE MEMIDNO=@IDNO;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetCarMachineAndCheckOrderNoIDNO';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetCarMachineAndCheckOrderNoIDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'確認訂單並取出車機、卡號、身份證', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetCarMachineAndCheckOrderNoIDNO';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetCarMachineAndCheckOrderNoIDNO';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_GetCarMachineAndCheckOrderNoIDNO';