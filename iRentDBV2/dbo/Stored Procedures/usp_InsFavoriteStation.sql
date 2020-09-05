﻿/****************************************************************
** Name: [dbo].[usp_InsFavoriteStation]
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
** EXEC @Error=[dbo].[usp_InsFavoriteStation]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/8/19
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/8/19 |    Eric    |          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsFavoriteStation]
	@IDNO                   VARCHAR(10)           ,
	@StationID              VARCHAR(10)           ,
	@Mode                   TINYINT               ,  --0:移除;1:新增
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
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_InsFavoriteStation';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @IDNO     =ISNULL (@IDNO    ,'');
SET @Mode     =ISNULL (@Mode,2);
SET @StationID=ISNULL(@StationID,'');

		BEGIN TRY
		 IF @Mode=2 OR @IDNO='' 
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		 IF @Error=0
		 BEGIN
			IF @Mode=0
			BEGIN
				SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_FavoriteStation WITH(NOLOCK) WHERE IDNO=@IDNO AND StationID=@StationID;
				IF @hasData=0
				BEGIN
				   SET @Error=1;
				   SET @ErrorCode='ERR147';
				END
				ELSE
				BEGIN
					DELETE FROM TB_FavoriteStation WHERE IDNO=@IDNO AND StationID=@StationID;
				END
			END
			ELSE
			BEGIN
				SET @hasData=0;
				--判斷有沒有這個據點
				SELECT @hasData=COUNT(1) FROM TB_iRentStation WITH(NOLOCK) WHERE StationID=@StationID AND use_flag=3;
				IF @hasData=0
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR149';
				END
				--判斷有沒有新增過這個據點
				IF @Error=0
				BEGIN
					SET @hasData=0;
					SELECT @hasData=COUNT(1) FROM TB_FavoriteStation WITH(NOLOCK) WHERE IDNO=@IDNO AND StationID=@StationID;
					IF @hasData=0
					BEGIN
					   INSERT INTO TB_FavoriteStation(IDNO,StationID)VALUES(@IDNO,@StationID);
					END
					ELSE
					BEGIN
						SET @Error=1;
					    SET @ErrorCode='ERR148';
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsFavoriteStation';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsFavoriteStation';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'新增/移除常用站點', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsFavoriteStation';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsFavoriteStation';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsFavoriteStation';