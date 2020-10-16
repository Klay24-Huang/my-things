/****************************************************************
** Name: [dbo].[usp_PolygonListQuery]
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
** EXEC @Error=[dbo].[usp_VerifyEMail]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:eason 
** Date:2020/10/07 上午 09:24:42 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/07 上午 09:24:42    |  eason|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_PolygonListQuery]
	@StationID              VARCHAR(10)           ,
	@IsMotor                VARCHAR(200)		  , --是否為機車,0:否 1:是
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

SET @FunName='usp_PolygonListQuery';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

	BEGIN TRY       
         --查詢電子柵欄
            BEGIN		     
                IF @StationID IS NULL OR @StationID = ''
                BEGIN
					DECLARE @Longs VARCHAR(max)
					DECLARE @Lats VARCHAR(max) 
					declare @tb_Polygon table(   
						Longitude VARCHAR(max),
						Latitude VARCHAR(max)
					)
                    IF @IsMotor = 0 --汽車
                    BEGIN						
						;WITH tmp AS(
							SELECT p.PolygonMode, p.Longitude, p.Latitude FROM TB_Polygon p
							WHERE p.use_flag=1
							AND p.StationID in ('X0R4','X0U4','X0SR','X1V4') --汽車：台中(X0R4)、台南(X0U4)、北區(X0SR)、高雄(預定X1V4)    	
						)
						INSERT INTO @tb_Polygon
						SELECT t.Longitude, t.Latitude FROM tmp  t order by t.PolygonMode ASC					
						SELECT @Longs = ISNULL(@Longs + ',','') + p.Longitude FROM @tb_Polygon p
						SELECT @Lats = ISNULL(@Lats + ',','') + p.Latitude FROM @tb_Polygon p
						SELECT 0[PolygonMode], @Longs[Longitude], @Lats[Latitude]
                    END
                    ELSE IF @IsMotor = 1 --機車
                    BEGIN
						;WITH tmp AS(
							SELECT p.PolygonMode, p.Longitude, p.Latitude FROM TB_Polygon p
							WHERE p.use_flag=1
							AND p.StationID in ('X0WR','X1JT','X1KY','X1KZ','X1ZZ') --機車：北北桃(X0WR)、台南(X1JT)、台中(X1KY)、高雄(X1KZ)、宜蘭(X1ZZ)"   	
						)
						INSERT INTO @tb_Polygon
						SELECT t.Longitude, t.Latitude FROM tmp  t order by t.PolygonMode ASC	
						SELECT @Longs = ISNULL(@Longs + ',','') + p.Longitude FROM @tb_Polygon p
						SELECT @Lats = ISNULL(@Lats + ',','') + p.Latitude FROM @tb_Polygon p
						SELECT 0[PolygonMode], @Longs[Longitude], @Lats[Latitude]
                    END
                    ELSE
                    BEGIN
                        SET @Error = 1				  
                        SET @ErrorMsg = 'IsMotor只能是0或1'
                    END
                END
                ELSE
                BEGIN
                    SELECT PolygonMode,Longitude,Latitude FROM TB_Polygon
                    WHERE StationID=@StationID  AND use_flag=1
                    ORDER BY PolygonMode ASC
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_PolygonListQuery';

GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eason', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_PolygonListQuery';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_PolygonListQuery';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_PolygonListQuery';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_PolygonListQuery';