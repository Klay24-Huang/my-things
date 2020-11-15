/****************************************************************
** Name: [dbo].[usp_BE_HandleHiEasyRentRetry]
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
** EXEC @Error=[dbo].[usp_BE_HandleHiEasyRentRetry]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/1 下午 02:48:42 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/1 下午 02:48:42    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_HandleHiEasyRentRetry]
	@OrderNo                BIGINT                ,
	@Mode                   TINYINT               , --1:060;2:125;3:130
	@UserID                 NVARCHAR(10)          ,
	@LogID                  BIGINT                ,
	@ReturnMode             INT             OUTPUT, --1:060;2:125:3:130
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
DECLARE @isHoilday TINYINT;
DECLARE @tmpDate VARCHAR(10);
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_HandleHiEasyRentRetry';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @isHoilday=0;
SET @OrderNo=ISNULL (@OrderNo,0);
SET @UserID    =ISNULL (@UserID    ,'');
SET @Mode=ISNULL (@Mode,0);
SET @ReturnMode=0;

		BEGIN TRY

		 
		 IF @UserID='' OR @Mode=0 OR @OrderNo=0
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		--0.取得目前訂單狀態
		IF @Error=0
		BEGIN
			SELECT @car_mgt_status=CMS,@cancel_status =CS,@booking_status=BS,@tmpDate=CONVERT(VARCHAR(10),SD,112) FROM VW_BE_GetOrderFullDetail WITH(NOLOCK) WHERE OrderNo=@OrderNo;
			SELECT @isHoilday=COUNT(1) FROM TB_Holiday WHERE HolidayYearMonth=@tmpDate;
			IF @cancel_status>0 AND @Mode>1
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR744';
			END
			ELSE IF  @car_mgt_status<4 AND @Mode=2
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR744';
			END
			ELSE IF @car_mgt_status<16 AND @Mode=3
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR744';
			END
		END
		IF @Error=0
		BEGIN
			IF @Mode=1
			BEGIN
				SELECT @hasData=COUNT(1) FROM TB_BookingControl WHERE order_number=@OrderNo;
				IF @hasData=0
				BEGIN
						 INSERT INTO TB_BookingControl( [order_number],[ODCUSTID],[ODCUSTNM],[TEL1],[TEL2],[TEL3]
                                                              ,[ODDATE],[GIVEDATE],[GIVETIME],[RNTDATE],[RNTTIME],[CARTYPE]
                                                              ,[CARNO],[OUTBRNH],[INBRNH],[ORDAMT],[REMARK],[RPRICE],[RNTAMT]
                                                              ,[PROJTYPE],[INVKIND],[INVTITLE],[UNIMNO]
                                                              ,[TSEQNO],[CARRIERID],[NPOBAN])
						      SELECT  [order_number],[ODCUSTID],[ODCUSTNM],[TEL1],[TEL2],[TEL3]
						      		,[ODDATE],[GIVEDATE],[GIVETIME],[RNTDATE],[RNTTIME],[CarType]
						      		,[CARNO],[OUTBRNH],[INBRNH],[ORDAMT],N'iRent單號【H'+REPLICATE('0',7-LEN(CONVERT(VARCHAR(20),@OrderNo)))+'】',CASE @isHoilday WHEN 0 THEN [WeekdayPrice] WHEN 1 THEN [HoildayPrice] END ,Price
						      		,[PROJTYPE], [INVKIND]  ,[INVTITLE],IIF(INVKIND=1,'',[UNIMNO])
						      		,[TSEQNO],CARRIERID,NPOBAN
						      FROM VW_BE_InsBookingControl
						      WHERE order_number=@OrderNo;	
							  SET @ReturnMode=1;
				END
				ELSE
				BEGIN
					SET @ReturnMode=1;
				END
				--IF @hasData=0
				--BEGIN
					
				--END
			END
			ELSE IF @Mode=2
			BEGIN
				SET @hasData=0; 
				SELECT @hasData=COUNT(1) FROM TB_BookingControl WITH(NOLOCK) WHERE order_number=@OrderNo;
				IF @hasData=0
				BEGIN
						 INSERT INTO TB_BookingControl( [order_number],[ODCUSTID],[ODCUSTNM],[TEL1],[TEL2],[TEL3]
                                                              ,[ODDATE],[GIVEDATE],[GIVETIME],[RNTDATE],[RNTTIME],[CARTYPE]
                                                              ,[CARNO],[OUTBRNH],[INBRNH],[ORDAMT],[REMARK],[RPRICE],[RNTAMT]
                                                              ,[PROJTYPE],[INVKIND],[INVTITLE],[UNIMNO]
                                                              ,[TSEQNO],[CARRIERID],[NPOBAN])
						      SELECT  [order_number],[ODCUSTID],[ODCUSTNM],[TEL1],[TEL2],[TEL3]
						      		,[ODDATE],[GIVEDATE],[GIVETIME],[RNTDATE],[RNTTIME],[CarType]
						      		,[CARNO],[OUTBRNH],[INBRNH],[ORDAMT],N'iRent單號【H'+REPLICATE('0',7-LEN(CONVERT(VARCHAR(20),@OrderNo)))+'】',CASE @isHoilday WHEN 0 THEN [WeekdayPrice] WHEN 1 THEN [HoildayPrice] END ,Price
						      		,[PROJTYPE], [INVKIND]  ,[INVTITLE],IIF(INVKIND=1,'',[UNIMNO]),[TSEQNO],CARRIERID,NPOBAN
						      FROM VW_BE_InsBookingControl
						      WHERE order_number=@OrderNo;	
						SET @ReturnMode=1;
				END
				ELSE
				BEGIN
					SET @hasData=0; 
					SELECT @hasData=COUNT(1) FROM  TB_BookingControl  WITH(NOLOCK) WHERE order_number=@OrderNo AND isRetry=1; --需要補傳
					IF @hasData>0
					BEGIN
						SET @ReturnMode=1;
					END
					ELSE
					BEGIN
						SET @hasData=0; 
						SELECT @hasData=COUNT(1) FROM TB_lendCarControl  WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo;
						IF @hasData=0
						BEGIN
							  INSERT INTO TB_lendCarControl([PROCD],[ORDNO],[IRENTORDNO],[CUSTID],[CUSTNM],[BIRTH],[CUSTTYPE],[ODCUSTID],[CARTYPE],[CARNO],[TSEQNO],[GIVEDATE],[GIVETIME]
                          ,[RENTDAYS],[GIVEKM],[OUTBRNHCD],[RNTDATE],[RNTTIME],[RNTKM],[INBRNHCD],[RPRICE],[RINSU],[DISRATE],[OVERHOURS],[OVERAMT2],[RNTAMT]
                          ,[RENTAMT],[LOSSAMT2],[PROJID],[REMARK],[INVKIND],[UNIMNO],[INVTITLE],[INVADDR],[CARRIERID],[NPOBAN],isRetry)
							SELECT      [PROCD],[ORDNO],[IRENTORDNO],[CUSTID],[CUSTNM],ISNULL([BIRTH],''),1,'',[CARTYPE],[CARNO],[TSEQNO],[GIVEDATE],[GIVETIME]
										 ,[RENTDAYS],[GIVEKM],[OUTBRNHCD],[RNTDATE],[RNTTIME],CEILING([GIVEKM]),[INBRNHCD],[RPRICE],[RINSU],0,[OVERHOURS],[OVERAMT2],0
							           ,0,[LOSSAMT2],[PROJID],[XID],[INVKIND],[UNIMNO],[INVTITLE],[INVADDR],[CARRIERID],[NPOBAN],1
							FROM VW_BE_GetLandControl
							WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo;
							SET @ReturnMode=2;
						END
						ELSE
						BEGIN
							SET @hasData=0;
							SELECT @hasData=COUNT(1) FROM  TB_lendCarControl  WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo AND isRetry=1; --需要補傳
							IF @hasData>0
							BEGIN
								SET @ReturnMode=2;
							END
							ELSE
							BEGIN
								SET @Error=1;
								SET @ErrorCode='ERR745';
							END
						END
					END

				END
				
			END
			ELSE IF @Mode=3
			BEGIN
				SET @hasData=0; 
				IF @hasData=0
				BEGIN
						 INSERT INTO TB_BookingControl( [order_number],[ODCUSTID],[ODCUSTNM],[TEL1],[TEL2],[TEL3]
                                                              ,[ODDATE],[GIVEDATE],[GIVETIME],[RNTDATE],[RNTTIME],[CARTYPE]
                                                              ,[CARNO],[OUTBRNH],[INBRNH],[ORDAMT],[REMARK],[RPRICE],[RNTAMT]
                                                              ,[PROJTYPE],[INVKIND],[INVTITLE],[UNIMNO]
                                                              ,[TSEQNO],[CARRIERID],[NPOBAN])
						      SELECT  [order_number],[ODCUSTID],[ODCUSTNM],[TEL1],[TEL2],[TEL3]
						      		,[ODDATE],[GIVEDATE],[GIVETIME],[RNTDATE],[RNTTIME],[CarType]
						      		,[CARNO],[OUTBRNH],[INBRNH],[ORDAMT],N'iRent單號【H'+REPLICATE('0',7-LEN(CONVERT(VARCHAR(20),@OrderNo)))+'】',CASE @isHoilday WHEN 0 THEN [WeekdayPrice] WHEN 1 THEN [HoildayPrice] END ,Price
						      		,[PROJTYPE], [INVKIND]  ,[INVTITLE],IIF(INVKIND=1,'',[UNIMNO])
						      		,[TSEQNO],CARRIERID,NPOBAN
						      FROM VW_BE_InsBookingControl
						      WHERE order_number=@OrderNo;	
							  SET @ReturnMode=1;
				END
				ELSE
				BEGIN
					SET @hasData=0; 
					SELECT @hasData=COUNT(1) FROM  TB_BookingControl  WITH(NOLOCK) WHERE order_number=@OrderNo AND isRetry=1; --需要補傳
					IF @hasData>0
					BEGIN
						SET @ReturnMode=1;
					END
					ELSE
					BEGIN
						SET @hasData=0; 
						SELECT @hasData=COUNT(1) FROM TB_lendCarControl  WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo;
						IF @hasData=0
						BEGIN
							  INSERT INTO TB_lendCarControl([PROCD],[ORDNO],[IRENTORDNO],[CUSTID],[CUSTNM],[BIRTH],[CUSTTYPE],[ODCUSTID],[CARTYPE],[CARNO],[TSEQNO],[GIVEDATE],[GIVETIME]
                          ,[RENTDAYS],[GIVEKM],[OUTBRNHCD],[RNTDATE],[RNTTIME],[RNTKM],[INBRNHCD],[RPRICE],[RINSU],[DISRATE],[OVERHOURS],[OVERAMT2],[RNTAMT]
                          ,[RENTAMT],[LOSSAMT2],[PROJID],[REMARK],[INVKIND],[UNIMNO],[INVTITLE],[INVADDR],[CARRIERID],[NPOBAN],isRetry)
							SELECT      [PROCD],[ORDNO],[IRENTORDNO],[CUSTID],[CUSTNM],[BIRTH],1,'',[CARTYPE],[CARNO],[TSEQNO],[GIVEDATE],[GIVETIME]
										 ,[RENTDAYS],[GIVEKM],[OUTBRNHCD],[RNTDATE],[RNTTIME],CEILING([GIVEKM]),[INBRNHCD],[RPRICE],[RINSU],0,[OVERHOURS],[OVERAMT2],0
							           ,0,[LOSSAMT2],[PROJID],[XID],[INVKIND],[UNIMNO],[INVTITLE],[INVADDR],[CARRIERID],[NPOBAN],1
							FROM VW_BE_GetLandControl
							WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo;
							SET @ReturnMode=2;
						END
						ELSE
						BEGIN
							SET @hasData=0;
							SELECT @hasData=COUNT(1) FROM  TB_lendCarControl  WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo AND isRetry=1; --需要補傳
							IF @hasData>0
							BEGIN
								SET @ReturnMode=2;
							END
							ELSE
							BEGIN
								SET @hasData=0;
								SELECT @hasData=COUNT(1) FROM TB_ReturnCarControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo;
								IF @hasData=0
								BEGIN
									 INSERT INTO TB_ReturnCarControl(PROCD,ORDNO,IRENTORDNO,CUSTID,CUSTNM,BIRTH,CUSTTYPE,ODCUSTID,CARTYPE,CARNO,TSEQNO	    
					    				   ,GIVEDATE,GIVETIME,RENTDAYS,GIVEKM,OUTBRNHCD,RNTDATE,RNTTIME	    
					    				   ,RNTKM,INBRNHCD,RPRICE	    
					    				   ,RINSU,DISRATE,OVERHOURS,OVERAMT2,RNTAMT,RENTAMT,LOSSAMT2,PROJID,REMARK	    
					    				   ,INVKIND,UNIMNO,INVTITLE,INVADDR,GIFT,CARDNO,AUTHCODE,[CARRIERID],[NPOBAN]
										   ,NOCAMT,isRetry
					    	)
				      
                         SELECT PROCD,ORDNO,IRENTORDNO,CUSTID,CUSTNM,BIRTH,CUSTTYPE,ODCUSTID,CARTYPE,CARNO,TSEQNO	  
                         	  ,GIVEDATE,GIVETIME,RENTDAYS,GIVEKM,OUTBRNHCD,  RNTDATE, RNTTIME
                         	  , RNTKM,OUTBRNHCD,RPRICE	  
                         	  ,RINSU,DISRATE,OVERHOURS, OVERAMT2,RNTAMT, RENTAMT, LOSSAMT2,PROJID, REMARK
                         	  ,INVKIND,UNIMNO,INVTITLE,INVADDR,GIFT	
							  , CARDNO, AUTHCODE
							  ,[CARRIERID],[NPOBAN],NOCAMT,1
                         FROM VW_BE_GetReturnControl WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo
									SET @ReturnMode=3;
								END
								ELSE
								BEGIN
									SET @hasData=0; 
									SELECT @hasData=COUNT(1) FROM  TB_ReturnCarControl  WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo AND isRetry=1; --需要補傳
									IF @hasData>0
									BEGIN
										SET @ReturnMode=3;
									END
									ELSE
									BEGIN
										SET @Error=1;
										SET @ErrorCode='ERR745';
									END
								END

							END
						END
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleHiEasyRentRetry';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleHiEasyRentRetry';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'短租補傳', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleHiEasyRentRetry';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleHiEasyRentRetry';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleHiEasyRentRetry';