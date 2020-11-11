/****************************************************************
** Name: [dbo].[usp_BE_UPDUserGroup]
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
** EXEC @Error=[dbo].[usp_BE_UPDUserGroup]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/10 下午 02:34:35 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/10 下午 02:34:35    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_UPDUserGroup]
	@SEQNO      			BIGINT                ,
	@UserGroupID            VARCHAR(60)           ,
	@UserGroupName          NVARCHAR(60)		  ,
	@OperatorID             INT					  ,
	@StartDate              DATETIME              ,
	@EndDate                DATETIME              ,
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

DECLARE @NowTime DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_BE_UPDUserGroup';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @UserGroupID    =ISNULL (@UserGroupID    ,'');
SET @UserGroupName  =ISNULL (@UserGroupName    ,'');
SET @OperatorID     =ISNULL (@OperatorID    ,0);
SET @StartDate      =ISNULL(@StartDate      ,'1911-01-01 00:00:00');
SET @EndDate        =ISNULL(@EndDate        ,'1911-01-01 00:00:00');
SET @UserID         =ISNULL(@UserID         ,'');
SET @SEQNO=ISNULL(@SEQNO,0);
		BEGIN TRY
		 IF @UserGroupID='' OR @UserGroupName='' OR @OperatorID=0  OR @UserID='' OR @SEQNO=0 OR @StartDate='1911-01-01 00:00:00' OR @EndDate='1911-01-01 00:00:00'
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 	 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
		 	SELECT @hasData=COUNT(1) FROM TB_UserGroup WHERE USEQNO=@SEQNO ;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR747';
			END
			ELSE
			BEGIN
				SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_UserGroup WHERE UserGroupID=@UserGroupID AND OperatorID=@OperatorID AND USEQNO<>@SEQNO;
				IF @hasData>0
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR746';
				END
				ELSE
				BEGIN
					UPDATE TB_UserGroup
					SET UserGroupID=@UserGroupID,UserGroupName=@UserGroupName,OperatorID=@OperatorID,StartDate=@StartDate,EndDate=@EndDate,U_USERID=@UserID,U_SYSDT=@NowTime
					WHERE USEQNO=@SEQNO
					
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_UPDUserGroup';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_UPDUserGroup';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新使用者群組', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_UPDUserGroup';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_UPDUserGroup';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_UPDUserGroup';