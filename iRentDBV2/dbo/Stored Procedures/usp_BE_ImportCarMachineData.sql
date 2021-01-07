/****************************************************************
** Name: [dbo].[usp_BE_ImportCarMachineData]
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
** EXEC @Error=[dbo].[usp_BE_ImportCarMachineData]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/16 下午 02:49:49 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/16|  Eric      |          First Release
** 2021/01/05|  Eric      |			增加@depositary存放地點             
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_ImportCarMachineData]
	@CID                    VARCHAR(10)           ,
	@MobileNum              VARCHAR(50)           ,
	@SIMCardNo              VARCHAR(128)          ,
	@deviceToken            VARCHAR(128)          ,
	@depositary             NVARCHAR(255)         ,
	@UserID                 NVARCHAR(10)          ,
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
DECLARE @MobileID INT;

DECLARE @NowTime DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_ImportCarMachineData';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @MobileID=0;
SET @CID      =ISNULL(@CID      ,'');
SET @MobileNum=ISNULL(@MobileNum,'');
SET @SIMCardNo=ISNULL(@SIMCardNo,'');
SET @UserID   =ISNULL(@UserID   ,'');
SET @depositary=ISNULL(@depositary,'');

		BEGIN TRY

		 
		 IF @CID='' OR @MobileNum=''  OR @SIMCardNo='' OR @UserID=''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
		 	SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Mobile WHERE MobileNum=@MobileNum --OR SIMCardNo=@SIMCardNo; --先mark
			IF @hasData>0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR710';
			END
		 END
		 IF @Error=0
		 BEGIN
		 SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_CarMachine WHERE MachineNo=@CID;
				IF @hasData>0
				BEGIN
					SET @Error=1;
				    SET @ErrorCode='ERR711';
				END
		 END
		 IF @Error=0
		 BEGIN
				INSERT INTO TB_Mobile(MobileNum,SIMCardNo,last_Opt,UPDTime)VALUES(@MobileNum,@SIMCardNo,@UserID,@NowTime);
				SET @MobileID=@@IDENTITY;
				IF @MobileID>0
				BEGIN
					INSERT INTO TB_CarMachine(MachineNo,MobileID,deviceToken,depositary,last_Opt,UPDTime)VALUES(@CID,@MobileID,@deviceToken,@depositary,@UserID,@NowTime);
				END
				ELSE
				BEGIN
					SET @Error=1;
				    SET @ErrorCode='ERR712';
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ImportCarMachineData';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ImportCarMachineData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台匯入車機資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ImportCarMachineData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ImportCarMachineData';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_ImportCarMachineData';