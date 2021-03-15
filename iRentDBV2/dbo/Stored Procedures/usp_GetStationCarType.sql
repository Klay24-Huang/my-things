/****************************************************************
** Name: [dbo].[usp_GetStationCarType]
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
** Date:2020/10/23 上午 09:24:42 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/23 上午 09:24:42    |  adam|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_GetStationCarType]
	@StationID              VARCHAR(10)           ,
	@SD                     DATETIME = NULL	      , 
	@ED                     DATETIME = NULL       ,
	@IDNO					VARCHAR(10)			  , --會員代碼
	@LogID                  BIGINT   = 0          ,
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
SET @NowTime=DATEADD(HOUR,8,DATEADD(hour,8,GETDATE()));

SET @FunName='usp_GetStationCarType';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;

	BEGIN TRY      

	    SET @IDNO = ISNULL(@IDNO,'')

		IF @SD IS NOT NULL AND @ED IS NOT NULL
		BEGIN
			--先串車在串車型
			SELECT  DISTINCT CarBrend										--廠牌
					,CarType		= E.CarTypeGroupCode					--車型
					,CarTypeName	= D.CarBrend + ' ' + D.CarTypeName		--廠牌+車型
					,CarTypePic		= E.CarTypeImg			--車輛ICON對照
					,Operator		= O.OperatorICon		--運營商ICON
					,OperatorScore  = O.Score				--分數
					,Price_N		= P.PROPRICE_N			--平日
					,Price_H		= P.PROPRICE_H			--假日
					,Price          = dbo.FN_CalSpread(@SD, @ED, P.PROPRICE_N, P.PROPRICE_H) --租金計算 2020-11-10 eason
					,Seat			= E.Seat				--座椅數
					,IsFavStation   = CASE WHEN (SELECT COUNT(*) FROM VW_GetFavoriteStation fs WHERE fs.StationID = I.StationID AND fs.IDNO = @IDNO) > 0
									  THEN 1 ELSE 0 END   
			FROM (SELECT nowStationID,CarType FROM TB_Car WITH(NOLOCK) WHERE nowStationID=@StationID AND available<2) C
			JOIN TB_CarType D WITH(NOLOCK) ON C.CarType=D.CarType
			JOIN TB_CarTypeGroupConsist F WITH(NOLOCK) ON F.CarType=C.CarType
			JOIN TB_CarTypeGroup E WITH(NOLOCK) ON F.CarTypeGroupID=E.CarTypeGroupID
			JOIN TB_OperatorBase O WITH(NOLOCK) ON D.Operator=O.OperatorID
			JOIN TB_ProjectStation S WITH(NOLOCK) ON S.StationID=C.nowStationID AND S.IOType='O'
			JOIN TB_Project P WITH(NOLOCK) ON P.PROJID=S.PROJID AND P.SPCLOCK = 'Z'
			LEFT JOIN TB_iRentStationInfo I  WITH(NOLOCK) ON I.StationID = S.StationID
			--JOIN TB_OrderMain OM ON OM.ProjID = P.PROJID  
			WHERE C.nowStationID = @StationID 
			--AND ( 
			--	(OM.start_time between @SD AND @ED)  
			--	OR (OM.stop_time between @SD AND @ED) 
			--	OR (@SD BETWEEN OM.start_time AND OM.stop_time) 
			--	OR (@ED BETWEEN OM.start_time AND OM.stop_time) 
			--	OR (DATEADD(MINUTE,-30,@SD) between OM.start_time AND OM.stop_time) 
			--	OR (DATEADD(MINUTE,30,@ED) between OM.start_time AND OM.stop_time) 
			--)
			--20201212 ADD BY ADAM REASON.增加顯示時間的條件
			AND ((ShowStart BETWEEN @SD AND @ED) OR (ShowEnd BETWEEN @SD AND @ED) OR (@SD BETWEEN ShowStart AND ShowEnd) OR (@ED BETWEEN ShowStart AND ShowEnd))
		END
		ELSE
		BEGIN
			IF @SD IS NULL
			SET @SD=DATEADD(hour,8,GETDATE())

			IF @ED IS NULL
			SET @ED=DATEADD(DAY,1,@SD)

			--先串車在串車型
			SELECT  DISTINCT CarBrend										--廠牌
					,CarType		= E.CarTypeGroupCode					--車型
					,CarTypeName	= D.CarBrend + ' ' + D.CarTypeName		--廠牌+車型
					,CarTypePic		= E.CarTypeImg			--車輛ICON對照
					,Operator		= O.OperatorICon		--運營商ICON
					,OperatorScore  = O.Score				--分數
					,Price_N		= P.PROPRICE_N			--平日
					,Price_H		= P.PROPRICE_H			--假日
					,Price          = dbo.FN_CalSpread(@SD, @ED, P.PROPRICE_N, P.PROPRICE_H) --租金計算 2020-11-10 eason
					,Seat			= E.Seat				--座椅數
					,IsFavStation   = CASE WHEN (SELECT COUNT(*) FROM VW_GetFavoriteStation fs WITH(NOLOCK) WHERE fs.StationID = I.StationID AND fs.IDNO = @IDNO) > 0
						  THEN 1 ELSE 0 END   
			FROM (SELECT nowStationID,CarType FROM TB_Car WITH(NOLOCK) WHERE nowStationID=@StationID AND available<2) C
			JOIN TB_CarType D WITH(NOLOCK) ON C.CarType=D.CarType
			JOIN TB_CarTypeGroupConsist F WITH(NOLOCK) ON F.CarType=C.CarType
			JOIN TB_CarTypeGroup E WITH(NOLOCK) ON F.CarTypeGroupID=E.CarTypeGroupID
			JOIN TB_OperatorBase O WITH(NOLOCK) ON D.Operator=O.OperatorID
			JOIN TB_ProjectStation S WITH(NOLOCK) ON S.StationID=C.nowStationID AND S.IOType='O'
			JOIN TB_Project P WITH(NOLOCK) ON P.PROJID=S.PROJID AND P.SPCLOCK = 'Z'
			LEFT JOIN TB_iRentStation I  WITH(NOLOCK) ON I.StationID = S.StationID
			--JOIN TB_OrderMain OM ON OM.ProjID = P.PROJID  
			WHERE C.nowStationID = @StationID 
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetStationCarType';
GO