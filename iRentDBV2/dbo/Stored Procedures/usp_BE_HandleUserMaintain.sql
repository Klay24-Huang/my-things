/****** Object:  StoredProcedure [dbo].[usp_BE_HandleUserMaintain]    Script Date: 2021/2/20 上午 11:02:11 ******/

/****************************************************************
** Name: [dbo].[usp_BE_HandleUserMaintain]
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
** EXEC @Error=[dbo].[usp_BE_HandleUserMaintain]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/13 上午 05:48:06 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/13 上午 05:48:06    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_HandleUserMaintain]
    @SEQNO                  INT                   ,
	@Mode                   VARCHAR(10)           ,
	@Operator               INT                   ,
	@UserGroupID            INT                   ,
	@UserAccount            VARCHAR(50)           ,
	@UserPWD                VARCHAR(1024)         ,
	@UserName               NVARCHAR(50)          ,
	@StartDate              DATETIME              ,
	@EndDate                DATETIME              ,
	@PowerStr               VARCHAR(MAX)          ,
	@UserID                 NVARCHAR(10)          , --使用者
	--@AreaV					NVARCHAR(50)          ,--20210209唐加
	--@mtypeV					NVARCHAR(50)          ,--20210209唐加
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
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;
DECLARE @tmpPowerStr VARCHAR(MAX);
DECLARE @FSEQNO INT;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_HandleUserMaintain';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @FSEQNO=0;
SET @tmpPowerStr='';
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @UserID    =ISNULL (@UserID    ,'');
SET @SEQNO      =ISNULL(@SEQNO      ,0);
SET @Mode       =ISNULL(@Mode       ,'');
SET @UserGroupID=ISNULL(@UserGroupID,0);
SET @UserAccount=ISNULL(@UserAccount,'');
SET @UserName   =ISNULL(@UserName   ,'');
SET @StartDate  =ISNULL(@StartDate  ,'1911-01-01 00:00:00');
SET @EndDate    =ISNULL(@EndDate    ,'1911-01-01 00:00:00');
SET @PowerStr   =ISNULL(@PowerStr   ,'');
SET @UserPWD    =ISNULL(@UserPWD,'');
SET @Mode       =ISNULL(@Mode,'');
SET @SEQNO      =ISNULL(@SEQNO,0); 

		BEGIN TRY
		 
		 IF @UserID='' OR @UserGroupID=0 OR @UserAccount='' OR @UserName='' OR @StartDate='1911-01-01 00:00:00' OR @EndDate='1911-01-01 00:00:00'  OR @Mode=''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
			IF @Mode<>'Add' AND @SEQNO=0
			BEGIN
				SET @Error=1;
		        SET @ErrorCode='ERR900'
			END
		 END
		 IF @Error=0
		 BEGIN
			IF @PowerStr=''
			BEGIN
				SELECT @tmpPowerStr=FuncGroupPower FROM VW_BE_GetFuncPower WITH(NOLOCK) WHERE FuncGroupID=(SELECT FuncGroupID FROM TB_UserGroup WHERE USEQNO=@UserGroupID)
			END
			ELSE
			BEGIN
				SET @tmpPowerStr=@PowerStr;
			END
			IF @Mode='Add'
			BEGIN
				SELECT @hasData=COUNT(1) FROM TB_Manager WITH(NOLOCK) WHERE Account=@UserAccount AND UserGroupID=@UserGroupID;
				IF @hasData=0
				BEGIN
					INSERT INTO TB_Manager(Account,UserPwd,UserName,UserGroup,UserGroupID,Operator,StartDate,EndDate,PowerList,AddUser)VALUES(@UserAccount,HASHBYTES('sha1',@UserPwd),@UserName,'',@UserGroupID,@Operator,@StartDate,@EndDate,@tmpPowerStr,@UserID);
					--20210209唐加
					/*
					IF @UserGroupID IN(2,4)
					BEGIN
						INSERT INTO TB_Maintain_User(Account,Password,UserName,ServerAccount,use_flag,MKTime,UPDTime)VALUES(@UserAccount,@UserPwd,@UserName,0,1,GETDATE(),GETDATE())
					END
					IF @UserGroupID IN(8,10)
					BEGIN
						INSERT INTO TB_Maintain_User(Account,Password,UserName,ServerAccount,use_flag,MKTime,UPDTime)VALUES(@UserAccount,@UserPwd,@UserName,0,1,GETDATE(),GETDATE())
						INSERT INTO A_20210115_KymcoMaintainUser(id,name,area,type,MEMONEW,MEMONEW2)VALUES('ir-'+@UserAccount,@UserName,@AreaV,@mtypeV,'','')
					END
					*/
				END
				ELSE
				BEGIN
					SET @Error=1;
		            SET @ErrorCode='ERR752'
				END
			END
			ELSE
			BEGIN
					SELECT @hasData=COUNT(1) FROM TB_Manager WITH(NOLOCK) WHERE SEQNO=@SEQNO;
					IF @hasData=0
					BEGIN
						SET @Error=1;
						SET @ErrorCode='ERR753';
					END
					ELSE
					BEGIN
						SET @hasData=0;
						SELECT @hasData=COUNT(1) FROM TB_Manager WITH(NOLOCK) WHERE Account=@UserAccount AND UserGroupID=@UserGroupID AND SEQNO<>@SEQNO;
						IF @hasData=0
						BEGIN
							IF @UserPWD<>''
							BEGIN
								UPDATE TB_Manager
								SET Account=@UserAccount,UserPwd=HASHBYTES('sha1',@UserPwd),UserGroupID=@UserGroupID,Operator=@Operator,StartDate=@StartDate,EndDate=@EndDate,PowerList=@tmpPowerStr,UPDTime=@NowTime,UPDUser=@UserID,UserName=@UserName
								WHERE SEQNO=@SEQNO
							END
							ELSE
							BEGIN
								UPDATE TB_Manager
								SET Account=@UserAccount,UserGroupID=@UserGroupID,Operator=@Operator,StartDate=@StartDate,EndDate=@EndDate,PowerList=@tmpPowerStr,UPDTime=@NowTime,UPDUser=@UserID,UserName=@UserName
								WHERE SEQNO=@SEQNO
							END
						END
						ELSE
						BEGIN
							SET @Error=1;
							SET @ErrorCode='ERR752'
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleUserMaintain';
GO


