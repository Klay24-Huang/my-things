/****************************************************************
** Name: [dbo].[usp_MochiParkHandle]
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
** EXEC @Error=[dbo].[usp_MochiParkHandle]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/10 上午 05:35:28 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/10 上午 05:35:28    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_MochiParkHandle]
	@Id						VARCHAR(100) , 
	@t_Operator				NVARCHAR(128) ,
	@Name					NVARCHAR(128) , 
	@cooperation_state		VARCHAR(50) , 
	@price					INT, 
	@charge_mode			VARCHAR(50), 
	@lat					DECIMAL(12,6), 
	@lng					DECIMAL(12,6), 
	@open_status			VARCHAR(50), 
	@t_period					NVARCHAR(50), 
	@all_day_open			TINYINT , 
	@detail					NVARCHAR(128), 
	@city					NVARCHAR(50), 
	@addr					NVARCHAR(256), 
	@tel					VARCHAR(20), 
	@addUser                NVARCHAR(256),
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
DECLARE @car_mgt_status TINYINT;
DECLARE @cancel_status TINYINT;
DECLARE @booking_status TINYINT;
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

SET @FunName='usp_MochiParkHandle';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @Id					=ISNULL(@Id					,'');
SET @t_Operator			=ISNULL(@t_Operator			,'');
SET @Name				=ISNULL(@Name				,'');
SET @cooperation_state	=ISNULL(@cooperation_state	,'');
SET @price				=ISNULL(@price				,0);
SET @charge_mode		=ISNULL(@charge_mode		,'');
SET @lat				=ISNULL(@lat				,0.000000);
SET @lng				=ISNULL(@lng				,0.000000);
SET @open_status		=ISNULL(@open_status		,'');
SET @t_period			=ISNULL(@t_period				,'');
SET @all_day_open		=ISNULL(@all_day_open		,0);
SET @detail				=ISNULL(@detail				,'');
SET @city				=ISNULL(@city				,'');
SET @addr				=ISNULL(@addr				,'');
SET @tel				=ISNULL(@tel				,'');
SET @addUser=ISNULL(@addUser,N'');

		BEGIN TRY
	 IF @Id='' OR @addUser=''
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR100'
 		 END
		 
		 IF @Error=0
		 BEGIN
		   SELECT @hasData=COUNT(1) FROM TB_MochiPark WITH(NOLOCK) WHERE Id=@Id;
		   IF @hasData=0
		   BEGIN
		       INSERT INTO TB_MochiPark([Id],[Operator],[Name],[cooperation_state],[price]
						                       ,[charge_mode],[lat],[lng],[open_status],[period]
						                       ,[all_day_open],[detail],[city],[addr],[tel]
						                       ,[AddTime],[AddUser],[UpdateTime],[UpdateUser])
										VALUES(@Id,@t_Operator,@Name,@cooperation_state,@price
						                       ,@charge_mode,@lat,@lng,@open_status,@t_period
												,@all_day_open,@detail,@city,@addr,@tel
						                       ,@NowTime,@addUser,@NowTime,@addUser);
		   END
		   ELSE
		   BEGIN
		       UPDATE TB_MochiPark 
			   SET [Id]=@Id,[Operator]=@t_Operator,[Name]=@Name,[cooperation_state]=@cooperation_state,[price]=@price
				  ,[charge_mode]=@charge_mode,[lat]=@lat,[lng]=@lng,[open_status]=@open_status,[period]=@t_period
				  ,[all_day_open]=@all_day_open,[detail]=@detail,[city]=@city,[addr]=@addr,[tel]=@tel
				  ,[UpdateTime]=@NowTime,[UpdateUser]=@addUser
			  WHERE Id=@Id;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MochiParkHandle';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MochiParkHandle';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'同步車麻吉停車場', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MochiParkHandle';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MochiParkHandle';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_MochiParkHandle';