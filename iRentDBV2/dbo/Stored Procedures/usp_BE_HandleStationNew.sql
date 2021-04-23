/****************************************************************
** Name: [dbo].[usp_BE_HandleStation]
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
** EXEC @Error=[dbo].[usp_BE_HandleStation]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/6 上午 06:14:51 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/6 |    Eric    |          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_HandleStationNew]
    @StationType 			INT,					--據點類別
    @StationID   			VARCHAR(10),			--據點代碼
    @StationName 			NVARCHAR(50),			--據點名稱
    @ManagerStationID		VARCHAR(10),			--管理據點代碼
    @UniCode 				VARCHAR(20),			--統一編號
    @CityID 				VARCHAR(10),			--縣市代碼
    @AreaID 				INT,					--據點地塊行政區代碼
    @Addr   				NVARCHAR(150),			--地址
    @TEL    				VARCHAR(50),			--電話
    @Longitude 				DECIMAL(9,6),			--經度
    @Latitude  				DECIMAL(9,6),			--緯度
    @in_description			NVARCHAR(1024),			--據點描述(內部註記)
    @show_description		NVARCHAR(1024),			--據點描述(app顯示)
    @IsRequired				INT,					--是否還車位置資訊必填
    @StationPick			VARCHAR(10),			--共同出車庫位
    @FCode 					VARCHAR(50),			--財務部門代碼
    @SDate 					DateTime,				--有效起日
    @EDate 					DateTime,				--有效迄日
    @ParkingNum 			VARCHAR(10),			--車位數
    @OnlineNum 				INT,					--實際上線數
    @Area 					NVARCHAR(10),			--行政區
    @fileName1 				VARCHAR(150),			--上傳照片1
    @fileName2 				VARCHAR(150),			--上傳照片2
    @fileName3 				VARCHAR(150),			--上傳照片3
    @fileName4 				VARCHAR(150),			--上傳照片4
	--2020/11/17 Jerry 增加圖片數量至5張
    @fileName5 				VARCHAR(150),			--上傳照片5
    @fileDescript1 			NVARCHAR(1024),			--上傳照片1照片說明
    @fileDescript2 			NVARCHAR(1024),			--上傳照片2照片說明
    @fileDescript3 			NVARCHAR(1024),			--上傳照片3照片說明
    @fileDescript4 			NVARCHAR(1024),			--上傳照片4照片說明
	--2020/11/17 Jerry 增加圖片數量至5張
    @fileDescript5 			NVARCHAR(1024),			--上傳照片5照片說明
	@UserID					NVARCHAR(10),			--使用者
    @Mode 					INT,					--模式(0:新增 1:修改)
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
DECLARE @tmpFileName1 VARCHAR(150);
DECLARE @tmpFileName2 VARCHAR(150);
DECLARE @tmpFileName3 VARCHAR(150);
DECLARE @tmpFileName4 VARCHAR(150);
--2020/11/17 Jerry 增加圖片數量至5張
DECLARE @tmpFileName5 VARCHAR(150);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_HandleStationNew';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @tmpFileName1='';
SET @tmpFileName2='';
SET @tmpFileName3='';
SET @tmpFileName4='';
--2020/11/17 Jerry 增加圖片數量至5張
SET @tmpFileName5='';

SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @StationID=ISNULL(@StationID,'');
SET @UserID=ISNULL(@UserID,'');

BEGIN TRY
	IF @StationID='' OR @UserID='' 
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	--0.再次檢核token
	IF @Error=0
	BEGIN
		IF @Mode=1
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_iRentStation WITH(NOLOCK) WHERE StationID=@StationID;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR742';
			END
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_iRentStation WITH(NOLOCK) WHERE StationID=@StationID;
			IF @hasData=1
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR741';
			END
		END
	END

	IF @Error=0
	BEGIN
		IF @Mode=0
		BEGIN
			INSERT INTO TB_iRentStation([StationID],[ManageStationID] ,[Location],[Tel],[ADDR]
										,[Latitude],[Longitude],[Content],[ContentForAPP],[UNICode]
										,[CityID],[AreaID],[IsRequiredForReturn],[CommonLendStation],[FCODE]
										,[SDate],[EDate],[IsNormalStation],[AllowParkingNum],[NowOnlineNum]
										,[use_flag],[Area],[A_USER_ID])
								VALUES(@StationID,@ManagerStationID,@StationName,@TEL,@Addr
										,@Latitude,@Longitude,@in_description,@show_description,@UniCode
										,@CityID,@AreaID,@IsRequired,@StationPick,@FCode
										,@SDate,@EDate,@StationType,@ParkingNum,@OnlineNum
										,1,@Area,@UserID);
			IF @fileName1<>''
			BEGIN
				INSERT INTO TB_iRentStationInfo(StationID,StationPic,PicDescription,Sort,use_flag)VALUES(@StationID,@fileName1,@fileDescript1,1,1);
			END
			IF @fileName2<>''
			BEGIN
				INSERT INTO TB_iRentStationInfo(StationID,StationPic,PicDescription,Sort,use_flag)VALUES(@StationID,@fileName2,@fileDescript2,2,1);
			END
			IF @fileName3<>''
			BEGIN
				INSERT INTO TB_iRentStationInfo(StationID,StationPic,PicDescription,Sort,use_flag)VALUES(@StationID,@fileName3,@fileDescript3,3,1);
			END
			IF @fileName4<>''
			BEGIN
				INSERT INTO TB_iRentStationInfo(StationID,StationPic,PicDescription,Sort,use_flag)VALUES(@StationID,@fileName4,@fileDescript4,4,1);
			END
			--2020/11/17 Jerry 增加圖片數量至5張
			IF @fileName5<>''
			BEGIN
				INSERT INTO TB_iRentStationInfo(StationID,StationPic,PicDescription,Sort,use_flag)VALUES(@StationID,@fileName5,@fileDescript5,5,1);
			END

			--20210412;針對據點類別=0(同站)，將同站的非區域性專案新增至ProjectStation
			IF @StationType=0
			BEGIN
				INSERT INTO [TB_ProjectStation]([PROJID],[IOType],[StationID],MKTime)
				SELECT [PROJID],'I',@StationID,@NowTime
				FROM [TB_Project]
				WHERE PROJTYPE=0 AND ShowEnd>@SDate AND IsRegional=0;

				INSERT INTO [TB_ProjectStation]([PROJID],[IOType],[StationID],MKTime)
				SELECT [PROJID],'O',@StationID,@NowTime
				FROM [TB_Project]
				WHERE PROJTYPE=0 AND ShowEnd>@SDate AND IsRegional=0;
			END
		END
		ELSE
		BEGIN
			UPDATE TB_iRentStation
			SET [ManageStationID]=@ManagerStationID,
				[Location]=@StationName,
				[Tel]=@TEL,
				[ADDR]=@Addr,
				[Latitude]=@Latitude,
				[Longitude]=@Longitude,
				[Content]=@in_description,
				[ContentForAPP]=@show_description,
				[UNICode]=@UniCode,
				[CityID]=@CityID,
				[AreaID]=@AreaID,
				[IsRequiredForReturn]=@IsRequired,
				[CommonLendStation]=@StationPick,
				[FCODE]=@FCode,
				[SDate]=@SDate,
				[EDate]=@EDate,
				[IsNormalStation]=@StationType,
				[AllowParkingNum]=@ParkingNum,
				[NowOnlineNum]=@OnlineNum,
				[Area]=@Area,
				[U_USER_ID]=@UserID,
				[UPDTime]=@NowTime
			WHERE StationID=@StationID

			SELECT @tmpFileName1=ISNULL(StationPic,'') FROM TB_iRentStationInfo WITH(NOLOCK) WHERE StationID=@StationID AND Sort=1;
			SELECT @tmpFileName2=ISNULL(StationPic,'') FROM TB_iRentStationInfo WITH(NOLOCK) WHERE StationID=@StationID AND Sort=2;
			SELECT @tmpFileName3=ISNULL(StationPic,'') FROM TB_iRentStationInfo WITH(NOLOCK) WHERE StationID=@StationID AND Sort=3;
			SELECT @tmpFileName4=ISNULL(StationPic,'') FROM TB_iRentStationInfo WITH(NOLOCK) WHERE StationID=@StationID AND Sort=4;
			--2020/11/17 Jerry 增加圖片數量至5張
			SELECT @tmpFileName5=ISNULL(StationPic,'') FROM TB_iRentStationInfo WITH(NOLOCK) WHERE StationID=@StationID AND Sort=5;

			IF @fileName1!=''
			BEGIN
				IF @tmpFileName1=''
				BEGIN
					INSERT INTO TB_iRentStationInfo(StationID,StationPic,PicDescription,Sort,use_flag)VALUES(@StationID,@fileName1,@fileDescript1,1,1);
				END
				ELSE
				BEGIN
					IF @fileName1<>@tmpFileName1
					BEGIN
						UPDATE TB_iRentStationInfo 
						SET StationPic=@fileName1,
							PicDescription=@fileDescript1,
							UPDTime=@NowTime
						WHERE StationID=@StationID AND Sort=1
					END
					ELSE
					BEGIN
						UPDATE TB_iRentStationInfo 
						SET PicDescription=@fileDescript1,
							UPDTime=@NowTime
						WHERE StationID=@StationID AND Sort=1
					END
				END
			END
			ELSE
			BEGIN
				UPDATE TB_iRentStationInfo 
				SET StationPic=@fileName1,
					PicDescription=@fileDescript1,
					UPDTime=@NowTime 
				WHERE StationID=@StationID AND Sort=1
			END

			IF @fileName2!=''
			BEGIN
				IF @tmpFileName2=''
				BEGIN
					INSERT INTO TB_iRentStationInfo(StationID,StationPic,PicDescription,Sort,use_flag)VALUES(@StationID,@fileName2,@fileDescript2,2,1);
				END
				ELSE
				BEGIN
					IF @fileName2<>@tmpFileName2
					BEGIN
						UPDATE TB_iRentStationInfo 
						SET StationPic=@fileName2,
							PicDescription=@fileDescript2,
							UPDTime=@NowTime 
						WHERE StationID=@StationID AND Sort=2
					END
					ELSE
					BEGIN
						UPDATE TB_iRentStationInfo 
						SET PicDescription=@fileDescript2,
							UPDTime=@NowTime
						WHERE StationID=@StationID AND Sort=2
					END
				END
			END
			ELSE
			BEGIN
				UPDATE TB_iRentStationInfo 
				SET StationPic=@fileName1,
					PicDescription=@fileDescript1,
					UPDTime=@NowTime
				WHERE StationID=@StationID AND Sort=1
			END

			IF @fileName3!=''
			BEGIN
				IF @tmpFileName3=''
				BEGIN
					INSERT INTO TB_iRentStationInfo(StationID,StationPic,PicDescription,Sort,use_flag)VALUES(@StationID,@fileName3,@fileDescript3,3,1);
				END
				ELSE
				BEGIN
					IF @fileName3<>@tmpFileName3
					BEGIN
						UPDATE TB_iRentStationInfo 
						SET StationPic=@fileName3,
							PicDescription=@fileDescript3,
							UPDTime=@NowTime
						WHERE StationID=@StationID AND Sort=3
					END
					ELSE
					BEGIN
						UPDATE TB_iRentStationInfo 
						SET PicDescription=@fileDescript3,
							UPDTime=@NowTime
						WHERE StationID=@StationID AND Sort=3
					END
				END
			END
			ELSE
			BEGIN
				UPDATE TB_iRentStationInfo 
				SET StationPic=@fileName1,
					PicDescription=@fileDescript3,
					UPDTime=@NowTime
				WHERE StationID=@StationID AND Sort=3
			END

			IF @fileName4!=''
			BEGIN
				IF @tmpFileName4=''
				BEGIN
					INSERT INTO TB_iRentStationInfo(StationID,StationPic,PicDescription,Sort,use_flag)VALUES(@StationID,@fileName4,@fileDescript4,4,1);
				END
				ELSE
				BEGIN
					IF @fileName1<>@tmpFileName1
					BEGIN
						UPDATE TB_iRentStationInfo 
						SET StationPic=@fileName4,
							PicDescription=@fileDescript4,
							UPDTime=@NowTime
						WHERE StationID=@StationID AND Sort=4
					END
					ELSE
					BEGIN
						UPDATE TB_iRentStationInfo 
						SET PicDescription=@fileDescript4,
							UPDTime=@NowTime
						WHERE StationID=@StationID AND Sort=4
					END
				END
			END
			ELSE
			BEGIN
				UPDATE TB_iRentStationInfo 
				SET StationPic=@fileName4,
					PicDescription=@fileDescript4,
					UPDTime=@NowTime
				WHERE StationID=@StationID AND Sort=4
			END			
				
			--2020/11/17 Jerry 增加圖片數量至5張
			IF @fileName5!=''
			BEGIN
				IF @tmpFileName5=''
				BEGIN
					INSERT INTO TB_iRentStationInfo(StationID,StationPic,PicDescription,Sort,use_flag)VALUES(@StationID,@fileName5,@fileDescript5,5,1);
				END
				ELSE
				BEGIN
					IF @fileName1<>@tmpFileName1
					BEGIN
						UPDATE TB_iRentStationInfo 
						SET StationPic=@fileName5,
							PicDescription=@fileDescript5,
							UPDTime=@NowTime
						WHERE StationID=@StationID AND Sort=5
					END
					ELSE
					BEGIN
						UPDATE TB_iRentStationInfo 
						SET PicDescription=@fileDescript5,
							UPDTime=@NowTime 
						WHERE StationID=@StationID AND Sort=5
					END
				END
			END
			ELSE
			BEGIN
				UPDATE TB_iRentStationInfo 
				SET StationPic=@fileName5,
					PicDescription=@fileDescript5,
					UPDTime=@NowTime 
				WHERE StationID=@StationID AND Sort=5
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleStationNew';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleStationNew';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleStationNew';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleStationNew';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleStationNew';