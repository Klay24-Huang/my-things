/****************************************************************
** Name: [dbo].[usp_UnBindCard]
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
** EXEC @Error=[dbo].[usp_SetPWD]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/12 下午 12:09:42 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/12 下午 12:09:42    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_UnBindCard]
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@IDNO                   VARCHAR(20)          ,
	@CardToken				VARCHAR(60)          ,
	@LogID                  BIGINT               ,
	@BindList				TY_BindList READONLY
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE
	@ErrorCode 				VARCHAR(6)		,	--回傳錯誤代碼
	@SQLExceptionCode		VARCHAR(10)		,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)		--回傳sqlException訊息
DECLARE @NowTime DATETIME;
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg=''; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_UnBindCard';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @CardToken    =ISNULL (@CardToken    ,'');
SET @NowTime=DATEADD(HOUR,8,GETDATE());

		BEGIN TRY
		 IF @IDNO='' OR @CardToken=''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  IF @Error=0
		 BEGIN
			--再次確認身份證是否存在
			BEGIN TRAN
				SELECT @hasData=COUNT(1) FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;
				IF @hasData=1
				BEGIN
					--更新解綁紀錄
					UPDATE TB_CardUnBindLog
					SET IsSuccess=1
					WHERE IDNO=@IDNO
					AND CardToken=@CardToken
					
					--新增綁定卡片
					INSERT INTO TB_MemberCardBinding
					(IDNO, BankNo, CardNumber, CardName, AvailableAmount, CardToken, IsValid, MKTime, 
                            UPDTime)
					SELECT IDNO=@IDNO
						, A.BankNo
						, A.CardNumber
						, A.CardName
						, A.AvailableAmount
						, A.CardToken
						, IsValid=1
						, MKTime=@NowTime
						, UPDTime=@NowTime
					FROM @BindList A
					LEFT JOIN TB_MemberCardBinding B WITH(NOLOCK) ON A.CardToken=B.CardToken
					WHERE B.CardToken IS NULL 
					AND A.CardToken<>''

					--更新已解綁的卡片
					UPDATE TB_MemberCardBinding
					SET IsValid=0,
						UPDTime=@NowTime
					WHERE IDNO=@IDNO
					AND CardToken=@CardToken
				END
				ELSE
				BEGIN
				    SET @Error=1;
				    SET @ErrorCode='ERR145';
				END
		 END

	--增加錯誤回傳
	SELECT 
		Error=@Error,
		ErrorCode=@ErrorCode,
		ErrorMsg=@ErrorMsg,
		SQLExceptionCode=@SQLExceptionCode,
		SQLExceptionMsg=@SQLExceptionMsg

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

	--增加錯誤回傳
	SELECT 
		Error=@Error,
		ErrorCode=@ErrorCode,
		ErrorMsg=@ErrorMsg,
		SQLExceptionCode=@SQLExceptionCode,
		SQLExceptionMsg=@SQLExceptionMsg

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_SetPWD';