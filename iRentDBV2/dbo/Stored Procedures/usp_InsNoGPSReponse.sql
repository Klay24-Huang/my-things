/****** Object:  StoredProcedure [dbo].[usp_InsNoGPSReponse]    Script Date: 2021/4/21 上午 09:08:57 ******/

/****************************************************************
** Name: [dbo].[usp_InsNoGPSReponse]
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
** EXEC @Error=[dbo].[usp_InsNoGPSReponse]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Jet
** Date:2021/4/20 15:24:00
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2021/4/20 15:24:00    |  Jet|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsNoGPSReponse]
	@LogID                  BIGINT					,
	@ErrorCode 				VARCHAR(6)		OUTPUT	,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT	,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT	,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT		--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @NowTime DATETIME;
DECLARE @OneHTime DATETIME;
DECLARE @EventType INT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_InsNoGPSReponse';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @OneHTime=DATEADD(HOUR,-1,@NowTime);
SET @EventType=0;

BEGIN TRY
	IF @Error=0
	BEGIN
		--10:車機失聯1小時
		--排除條件：1.下線車輛 2.據點(XXXX/X0XX)
		SET @EventType=10;

		DROP TABLE IF EXISTS #CarList;

		CREATE TABLE #CarList (
			CID		VARCHAR(10),
			CarNo	VARCHAR(10),
			Mail	VARCHAR(100),
			UPDTime DATETIME
		);
		
		INSERT INTO #CarList
		SELECT A.CID,A.CarNo,CONCAT(ISNULL(C.AlertEmail,C.[ManageStationID]),'@hotaimotor.com.tw;'),A.UPDTime
		FROM TB_CarStatus A WITH(NOLOCK)
		INNER JOIN TB_Car B WITH(NOLOCK) ON B.CarNo=A.CarNo AND B.available<>2
		LEFT JOIN TB_iRentStation C WITH(NOLOCK) ON C.StationID=B.nowStationID
		WHERE A.UPDTime<=@OneHTime
		AND B.nowStationID NOT IN ('XXXX','X0XX');

		INSERT INTO TB_EventHandle(EventType,MachineNo,CarNo,MKTime,UPDTime)
		SELECT @EventType,CID,CarNo,UPDTime,@NowTime
		FROM #CarList;

		INSERT INTO TB_AlertMailLog([EventType],[Receiver],[Sender],[HasSend],CarNo,[MKTime],UPDTime)
		SELECT @EventType,Mail,'0',0,CarNo,UPDTime,@NowTime
		FROM #CarList;

		DROP TABLE IF EXISTS #CarList;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsNoGPSReponse';
GO