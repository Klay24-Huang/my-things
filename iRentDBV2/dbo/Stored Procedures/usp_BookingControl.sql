/****** Object:  StoredProcedure [dbo].[usp_BookingControl]    Script Date: 2020/11/11 上午 10:40:53 ******/

/****************************************************************
** Name: [dbo].[usp_BookingControl]
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
** EXEC @Error=[dbo].[usp_BookingControl]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Jet
** Date:2020/11/11 上午 10:40:53
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BookingControl]
	@IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@Token                  VARCHAR(1024)         ,
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
DECLARE @Error		INT;
DECLARE @IsSystem	TINYINT;
DECLARE @FunName	VARCHAR(50);
DECLARE @ErrorType	TINYINT;
DECLARE @hasData	BIGINT;
DECLARE @Descript	NVARCHAR(200);
DECLARE @NowTime	DATETIME;

DECLARE @PROCD 		VARCHAR(1);		--處理區分(A、U、F)
DECLARE @ODCUSTID 	VARCHAR(10);	--訂車客戶ID
DECLARE @ODCUSTNM 	NVARCHAR(10);	--訂車客戶名稱
DECLARE @TEL1 		VARCHAR(20);	--連絡電話
DECLARE @TEL2 		VARCHAR(20);	--行動電話
DECLARE @TEL3 		VARCHAR(20);	--公司電話
DECLARE @ODDATE 	VARCHAR(8);		--受訂日期
DECLARE @GIVEDATE 	VARCHAR(8);		--取車日期
DECLARE @GIVETIME 	VARCHAR(4);		--取車時間
DECLARE @RNTDATE 	VARCHAR(8);		--預還日期
DECLARE @RNTTIME 	VARCHAR(4);		--預還時間
DECLARE @CARTYPE 	VARCHAR(10);	--車型代碼
DECLARE @CarNo 		VARCHAR(10);	--車號
DECLARE @OUTBRNH 	VARCHAR(6);		--出車據點
DECLARE @INBRNH 	VARCHAR(6);		--預還據點
DECLARE @ORDAMT 	INT;			--租金
DECLARE @REMARK 	NVARCHAR(50);	--備註
DECLARE @PAYAMT 	INT;			--已付租金
DECLARE @RPRICE 	INT;			--租金(日)
DECLARE @RINV 		INT;			--保費(日)
DECLARE @DISRATE 	FLOAT;			--折扣率
DECLARE @NETRPRICE 	INT;			--折扣後租金
DECLARE @RNTAMT 	INT;			--租金小計
DECLARE @INSUAMT 	INT;			--保費小計
DECLARE @RENTDAY 	FLOAT;			--預定使用天數
DECLARE @EBONUS 	INT;			--EBONUS
DECLARE @PROJID		VARCHAR(6);		--專案代碼
DECLARE @TYPE		TINYINT;		--預約型態 1:短租 2:附駕
DECLARE @INVKIND	TINYINT;		--1:捐贈 2:EMAIL 3:郵寄二聯 4:郵寄三聯 5:手機載具(會員設定) 6:自然人憑證條碼(會員設定) 7:其他社福團體(會員設定)
DECLARE @INVTITLE	NVARCHAR(20);	--發票抬頭
DECLARE @UNIMNO		VARCHAR(20);	--發票統編
DECLARE @TSEQNO		VARCHAR(10);	--車輛編號
DECLARE @ORDNO		VARCHAR(50);	--預約編號
DECLARE @isRetry	TINYINT;		--是否Retry
DECLARE @RetryTimes	TINYINT;		--重試次數
DECLARE @CARRIERID	VARCHAR(100);	--載具條碼
DECLARE @NPOBAN		VARCHAR(100);	--愛心碼
DECLARE @NOCAMT		INT;			--安心服務費
DECLARE @IsHoliday	INT;			--是否為假日(1:是 0:否)
DECLARE @SDate		DateTime;		--預約取車時間
DECLARE @EDate		DateTime;		--預約還車時間
DECLARE @PriceN		INT;			--平日價格
DECLARE @PriceH		INT;			--假日價格
DECLARE @ProjType	TINYINT;		--專案類型：0:同站;3:路邊;4:機車
DECLARE  @IRENTORDNO	VARCHAR(10)		--IRENT訂單編號
		,@BIRTH			DATETIME
		,@GIVEKM		INT
		,@RNTKM			INT

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @IsSystem=0;
SET @FunName='usp_BookingControl';
SET @ErrorType=0;
SET @hasData=0;
SET @Descript=N'新增資料至BookingControl';
SET @NowTime=DATEADD(HOUR,8,GETDATE());

SET @IsHoliday=0;
SET @IDNO=ISNULL(@IDNO,'');
SET @OrderNo=ISNULL(@OrderNo,0);
SET @Token=ISNULL(@Token,'');

BEGIN TRY
	--IF @Token='' OR @IDNO='' OR @OrderNo=0
	--20201118 ADD BY ADAM 取消TOKEN檢核
	IF @IDNO='' OR @OrderNo=0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	--0.再次檢核token
	/*
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WHERE Access_Token=@Token AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token WHERE Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END
	*/

	IF @Error=0
	BEGIN
		BEGIN TRAN
		SET @hasData=0
		
		SELECT @hasData=COUNT(order_number) FROM TB_OrderMain WHERE IDNO=@IDNO AND order_number=@OrderNo;
				
		IF @hasData>0
		BEGIN
			--取資料
			SELECT @ODCUSTID=[MEMIDNO],
				@ODCUSTNM=[MEMCNAME],
				@TEL1=[MEMHTEL],
				@TEL2=[MEMTEL],
				@TEL3=[MEMCOMTEL],
				@BIRTH=[MEMBIRTH]
			FROM [dbo].[TB_MemberData] WHERE [MEMIDNO]=@IDNO;
			
			SELECT @CarNo=[CarNo],
				@ODDATE=convert(varchar,[booking_date],112),
				@GIVEDATE=convert(varchar,[start_time],112),
				@GIVETIME=SUBSTRING(REPLACE(convert(varchar,[start_time],108),':',''),1,4),
				@RNTDATE=CONVERT(VARCHAR,[stop_time],112),
				@RNTTIME=SUBSTRING(REPLACE(CONVERT(VARCHAR,[stop_time],108),':',''),1,4),
				@OUTBRNH=[lend_place],
				@INBRNH=[return_place],
				@ORDAMT=[init_price],
				
				@PROJID=[ProjID],
				@INVKIND=[bill_option],
				@INVTITLE=[title],
				@UNIMNO=[unified_business_no],
				@CARRIERID=[CARRIERID],
				@NPOBAN=[NPOBAN],
				@NOCAMT=[InsurancePurePrice],
				@SDate=start_time,
				@EDate=stop_time,
				@ProjType=ProjType
			FROM [dbo].[TB_OrderMain] WHERE [order_number]=@OrderNo;
			
			
			SELECT --@GIVEDATE=CONVERT(VARCHAR,[final_start_time],112),
				--@GIVETIME=SUBSTRING(REPLACE(CONVERT(VARCHAR,[final_start_time],108),':',''),1,4),
				--@RNTAMT=[pure_price]
				@GIVEKM = CAST(start_mile AS INT),
				@RNTKM = CAST(end_mile AS INT)
			FROM [dbo].[TB_OrderDetail]
			WHERE [order_number]=@OrderNo;
			
			SELECT @TSEQNO=[TSEQNO],@CARTYPE=[CarType] FROM [dbo].[TB_Car] Where CarNo=@CarNo;
			
			SELECT @IsHoliday=COUNT(1) FROM [dbo].[TB_Holiday] Where use_flag=1 And HolidayDate=@GIVEDATE;

			SELECT @RPRICE=Case When @IsHoliday=1 Then PROPRICE_H Else PROPRICE_N End FROM [dbo].[TB_Project] WHERE [PROJID]=@PROJID;

			SELECT @PriceN=[PRICE],@PriceH=[PRICE_H] FROM [dbo].[VW_GetFullProjectCollectionOfCarTypeGroup] WHERE PROJID=@PROJID AND CARTYPE=@CARTYPE;
			
			--部分參數寫死
			SET @PROCD='A';
			SET @REMARK='iRent單號【' + 'H' + RIGHT(REPLICATE('0', 7) + CAST(@OrderNo as VARCHAR), 7) +'】';
			SET @PAYAMT=0;
			SET @RINV=0;
			SET @DISRATE=0;
			SET @NETRPRICE=0;
			SET @INSUAMT=0;
			SET @RENTDAY=0;
			SET @EBONUS=0;
			SET @TYPE=1;
			SET @isRetry=0;
			SET @RetryTimes=0;
			SET @ORDNO='';
			SET @IRENTORDNO = 'H' + RIGHT(REPLICATE('0', 7) + CAST(@OrderNo as VARCHAR), 7)
			--計算租金
			If @ProjType = 4
			BEGIN
				--機車
				exec @ORDAMT=[dbo].[FN_MotoRentTrial] @SDate,@EDate,@PROJID,@CARTYPE;
			END
			ELSE
			BEGIN
				--汽車
				exec @ORDAMT=[dbo].[FN_CalSpread] @SDate,@EDate,@PriceN,@PriceH;
				SET @RNTAMT=@ORDAMT
			END

			--寫資料
			INSERT INTO TB_BookingControl(order_number,PROCD,ODCUSTID,ODCUSTNM,TEL1,TEL2,TEL3,ODDATE,GIVEDATE,GIVETIME
				,RNTDATE,RNTTIME,CARTYPE,CARNO,OUTBRNH,INBRNH,ORDAMT,REMARK,PAYAMT,RPRICE
				,RINV,DISRATE,NETRPRICE,RNTAMT,INSUAMT,RENTDAY,EBONUS,PROJTYPE,TYPE,INVKIND
				,INVTITLE,UNIMNO,TSEQNO,ORDNO,MKTime,UPDTime,isRetry,RetryTimes,CARRIERID,NPOBAN
				,NOCAMT)
			VALUES(@OrderNo,@PROCD,@ODCUSTID,@ODCUSTNM,@TEL1,@TEL2,@TEL3,@ODDATE,@GIVEDATE,@GIVETIME
				,@RNTDATE,@RNTTIME,@CARTYPE,@CarNo,@OUTBRNH,@INBRNH,@ORDAMT,@REMARK,@PAYAMT,@RPRICE
				,@RINV,@DISRATE,@NETRPRICE,@ORDAMT,@INSUAMT,@RENTDAY,@EBONUS,@PROJID,@TYPE,@INVKIND
				,@INVTITLE,@UNIMNO,@TSEQNO,@ORDNO,@NowTime,@NowTime,@isRetry,@RetryTimes,@CARRIERID,@NPOBAN
				,@NOCAMT);
			
			IF NOT EXISTS(SELECT * FROM TB_lendCarControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo)
			BEGIN
			--順便寫入出車(125必須要等060跑過產生預約編號才可以進行同步)
			INSERT INTO TB_lendCarControl
			(
				  [PROCD], [ORDNO], [IRENTORDNO], [CUSTID], [CUSTNM]
				, [BIRTH], [CUSTTYPE], [ODCUSTID], [CARTYPE], [CARNO]
				, [TSEQNO], [GIVEDATE], [GIVETIME], [RENTDAYS], [GIVEKM]
				, [OUTBRNHCD], [RNTDATE], [RNTTIME], [RNTKM], [INBRNHCD]
				, [RPRICE], [RINSU], [DISRATE], [OVERHOURS], [OVERAMT2]
				, [RNTAMT], [RENTAMT], [LOSSAMT2], [PROJID], [REMARK]
				, [INVKIND], [UNIMNO], [INVTITLE], [INVADDR], [MKTime]
				, [UPDTime], [isRetry], [RetryTimes], [CARRIERID], [NPOBAN], [NOCAMT]
			)
			SELECT @PROCD
				  ,@ORDNO		AS ORDNO		--現階段不會有短租的預約編號
				  ,@OrderNo		AS IRENTORDNO
				  ,@IDNO		AS CUSTID
				  ,@ODCUSTNM	AS CUSTNM

				  ,CONVERT(VARCHAR(10),@BIRTH,120) AS BIRTH
				  ,CASE WHEN LEN(@IDNO)=10 THEN '1' ELSE '2' END AS CUSTTYPE
				  ,'' AS ODCUSTID
				  ,@CARTYPE		AS CARTYPE
				  ,@CarNo		AS CARNO
				  
				  ,@TSEQNO		AS TSEQNO
				  ,@GIVEDATE	AS GIVEDATE
				  ,@GIVETIME	AS GIVETIME
				  ,0			AS RENTDAYS
				  ,@GIVEKM		AS GIVEKM

				  ,@OUTBRNH		AS OUTBRNHCD
				  ,@RNTDATE		AS RNTDATE
				  ,@RNTTIME		As RNTTIME
				  ,@RNTKM		AS RNTKM
				  ,@INBRNH		AS INBRNHCD

				  ,@RPRICE		AS RPRICE
				  ,@RINV		AS RINSU
				  ,@DISRATE		AS DISRATE
				  ,0			AS OVERHOURS
				  ,0			AS OVERAMT2

				  ,@RNTAMT		AS RNTAMT
				  ,@RNTAMT		AS RENTAMT
				  ,0			AS LOSSAMT2
				  ,@PROJID		AS PROJID
				  ,@REMARK		AS REMARK

				  ,@INVKIND		AS INVKIND
				  ,@UNIMNO		AS UNIMNO
				  ,@INVTITLE	AS INVTITLE
				  ,''			AS INVADDR
				  ,@NowTime		AS MKTime

				  ,@NowTime		AS UPDTIME
				  ,0			AS IsRetry
				  ,0			AS RetryTimes
				  ,@CARRIERID	AS CARRIERID
				  ,@NPOBAN		AS NPOBAN
				  ,@NOCAMT		AS NOCAMT
			END

			COMMIT TRAN;
		END
		ELSE
		BEGIN
			ROLLBACK TRAN;
			SET @Error=1;
			SET @ErrorCode='ERR171';
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BookingControl';
GO

