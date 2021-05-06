/****** Object:  StoredProcedure [dbo].[usp_BE_HandleOrderModify]    Script Date: 2021/2/20 上午 11:01:05 ******/

/****************************************************************
** Name: [dbo].[usp_BE_HandleOrderModify]
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
** EXEC @Error=[dbo].[usp_BE_HandleOrderModify]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/25 上午 11:03:12 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/25 上午 11:03:12    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_HandleOrderModify]
    @OrderNo					BIGINT                ,		--訂單編號
	@CarPoint					INT                   ,		--折抵時數(汽車)
	@MotorPoint					INT                   ,		--折抵時數(機車)
	@ProjType					INT                   ,		--專案類型
	@StartDate					DATETIME              ,		--實際取車
	@EndDate					DATETIME              ,		--實際還車
	@start_mile					numeric(10,2)		  ,		--取車里程
	@end_mile					numeric(10,2)         ,		--還車里程
	@fine_price					INT                   ,		--逾時費用
	@FinalPrice					INT                   ,		--還車費用
	@Reson						INT      			  ,		--理由
	@Remark						NVARCHAR(50)		  ,		--備註
	@CarDispatch				INT                   ,		--車輛調度費
	@DispatchRemark				NVARCHAR(50)          ,		--車輛調度費備註
	@CleanFee					INT                   ,		--清潔費
	@CleanFeeRemark				NVARCHAR(50)          ,		--清潔費說明
	@DestroyFee					INT                   ,		--物品損壞/遺失費
	@DestroyFeeRemark			NVARCHAR(50)          ,		--物品損壞/遺失費設明
	@ParkingFee					INT                   ,		--停車費
	@ParkingFeeRemark			NVARCHAR(50)          ,		--停車費說明
	@DraggingFee				INT                   ,		--拖吊費
	@DraggingFeeRemark			NVARCHAR(50)          ,		--拖吊費備註
	@OtherFee					INT                   ,		--其他
	@OtherFeeRemark				NVARCHAR(50)          ,		--其他費用備註
	@ParkingFeeByMachi			INT                   ,		--代收停車費
	@ParkingFeeByMachiRemark	NVARCHAR(50)		  ,		--代收停車費說明
	@PAYAMT						INT                   ,		--差額
	@Insurance_price			INT                   ,		--安心服務
	@Mileage					INT                   ,		--里程費
	@Pure						INT                   ,		--純租金
	@ParkingFeeTotal			INT                   ,		--停車費用(總)     // 20210506;ADD BY YEH REASON.新增停車費用(總)
	@UserID						NVARCHAR(10)          ,		--使用者
	@LogID						BIGINT                ,
	@ErrorCode 					VARCHAR(6)		OUTPUT,		--回傳錯誤代碼
	@ErrorMsg  					NVARCHAR(100)	OUTPUT,		--回傳錯誤訊息
	@SQLExceptionCode			VARCHAR(10)		OUTPUT,		--回傳sqlException代碼
	@SQLExceptionMsg			NVARCHAR(1000)	OUTPUT		--回傳sqlException訊息
AS
DECLARE @Error INT;
DECLARE @IsSystem TINYINT;
DECLARE @FunName VARCHAR(50);
DECLARE @ErrorType TINYINT;
DECLARE @hasData TINYINT;
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;
DECLARE @IRENTORDNO VARCHAR(20)
/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_HandleOrderModify';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @OrderNo    =ISNULL(@OrderNo    ,-1);
SET @CarPoint   =ISNULL(@CarPoint   ,-1);
SET @MotorPoint =ISNULL(@MotorPoint ,-1);
SET @FinalPrice =ISNULL(@FinalPrice ,-1);
SET @Pure       =ISNULL(@Pure,-1);
SET @UserID    =ISNULL (@UserID    ,'');
SET @IRENTORDNO =  'H' + CAST(@OrderNo AS VARCHAR)

BEGIN TRY
	IF @UserID='' OR @OrderNo<=0 OR @CarPoint<0 OR @MotorPoint<0 OR @FinalPrice<0 OR @CarDispatch<0 OR @CleanFee<0 OR @DestroyFee<0 OR @ParkingFee<0 OR @DraggingFee<0 OR @OtherFee<0 OR @ParkingFeeByMachi<0 OR @Pure<0
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		 
	--0.再次檢核token
	IF @Error=0
	BEGIN
		IF @PAYAMT<>0
		BEGIN
			SET @PAYAMT=@PAYAMT*-1;
		END
		SET @hasData=0;
		SELECT @hasData=COUNT(1) FROM TB_OrderDetail WITH(NOLOCK) WHERE order_number=@OrderNo;
		IF @hasData=1
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_OrderOtherFee WITH(NOLOCK) WHERE OrderNo=@OrderNo;
			IF @hasData=0
			BEGIN
				INSERT INTO TB_OrderOtherFee(OrderNo,CarDispatch,DispatchRemark,CleanFee,CleanFeeRemark,DestroyFee             
											,DestroyFeeRemark,ParkingFee,ParkingFeeRemark,DraggingFee,DraggingFeeRemark      
											,OtherFee,OtherFeeRemark,ParkingFeeByMachi,ParkingFeeByMachiRemark
											,[AddUser],[AddTime],UpdateUser,UpdateTime
				)VALUES(@OrderNo,@CarDispatch,@DispatchRemark,@CleanFee,@CleanFeeRemark,@DestroyFee             
						,@DestroyFeeRemark,@ParkingFee,@ParkingFeeRemark,@DraggingFee,@DraggingFeeRemark      
						,@OtherFee,@OtherFeeRemark,@ParkingFeeByMachi,@ParkingFeeByMachiRemark
						,@UserID,@NowTime,@UserID,@NowTime
				);

				-- 20210506;ADD BY YEH REASON.把LOG機制修正，資料更新後就寫LOG
				INSERT INTO TB_OrderOtherFeeHistory(OrderNo,CarDispatch,DispatchRemark,CleanFee,CleanFeeRemark,DestroyFee             
													,DestroyFeeRemark,ParkingFee,ParkingFeeRemark,DraggingFee,DraggingFeeRemark      
													,OtherFee,OtherFeeRemark,ParkingFeeByMachi,ParkingFeeByMachiRemark
													,[AddUser],[AddTime],UpdateUser,UpdateTime
				)
				SELECT OrderNo,CarDispatch,DispatchRemark,CleanFee,CleanFeeRemark,DestroyFee             
						,DestroyFeeRemark,ParkingFee,ParkingFeeRemark,DraggingFee,DraggingFeeRemark      
						,OtherFee,OtherFeeRemark,ParkingFeeByMachi,ParkingFeeByMachiRemark
						,[AddUser],[AddTime],@UserID,@NowTime
				FROM TB_OrderOtherFee WITH(NOLOCK) WHERE OrderNo=@OrderNo;
			END
			ELSE
			BEGIN
				UPDATE TB_OrderOtherFee
				SET CarDispatch=@CarDispatch,
					DispatchRemark=@DispatchRemark,
					CleanFee=@CleanFee,
					CleanFeeRemark=@CleanFeeRemark,
					DestroyFee=@DestroyFee,
					DestroyFeeRemark=@DestroyFeeRemark,
					ParkingFee=@ParkingFee,
					ParkingFeeRemark=@ParkingFeeRemark,
					DraggingFee=@DraggingFee,
					DraggingFeeRemark=@DraggingFeeRemark,
					OtherFee=@OtherFee,
					OtherFeeRemark=@OtherFeeRemark,
					ParkingFeeByMachi=@ParkingFeeByMachi,
					ParkingFeeByMachiRemark=@ParkingFeeByMachiRemark,
					UpdateUser=@UserID,
					UpdateTime=@NowTime
				WHERE OrderNo=@OrderNo;

				-- 20210506;ADD BY YEH REASON.把LOG機制修正，資料更新後就寫LOG
				INSERT INTO TB_OrderOtherFeeHistory(OrderNo,CarDispatch,DispatchRemark,CleanFee,CleanFeeRemark,DestroyFee             
													,DestroyFeeRemark,ParkingFee,ParkingFeeRemark,DraggingFee,DraggingFeeRemark      
													,OtherFee,OtherFeeRemark,ParkingFeeByMachi,ParkingFeeByMachiRemark
													,[AddUser],[AddTime],UpdateUser,UpdateTime
				)
				SELECT OrderNo,CarDispatch,DispatchRemark,CleanFee,CleanFeeRemark,DestroyFee             
						,DestroyFeeRemark,ParkingFee,ParkingFeeRemark,DraggingFee,DraggingFeeRemark      
						,OtherFee,OtherFeeRemark,ParkingFeeByMachi,ParkingFeeByMachiRemark
						,[AddUser],[AddTime],@UserID,@NowTime
				FROM TB_OrderOtherFee WITH(NOLOCK) WHERE OrderNo=@OrderNo;
			END

			INSERT INTO TB_OrderModifyLog([order_number],[already_lend_car],[already_return_car],[extend_stop_time],[force_extend_stop_time],
											[final_start_time],[final_stop_time],[start_door_time],[end_door_time],[transaction_no],
											[final_price],[pure_price],[mileage_price],[Insurance_price],[fine_price],[fine_interval],
											[fine_rate],[gift_point],[gift_motor_point],[monthly_workday],[monthly_holiday],
											[Etag],[already_payment],[start_mile],[end_mile],[trade_status],
											[parkingFee], [parkingSpace], [TransDiscount], 
											[U_USERID],[U_SYSDT],[U_Reson],[U_Remark])
			SELECT *,@UserID,@NowTime,@Reson,@Remark FROM TB_OrderDetail WITH(NOLOCK) WHERE order_number=@OrderNo;

			UPDATE TB_OrderDetail
			SET gift_motor_point=@MotorPoint,
				gift_point=@CarPoint,
				final_price=@FinalPrice,
				final_start_time=@StartDate,
				final_stop_time=@EndDate,
				start_mile=@start_mile,
				end_mile=@end_mile,
				fine_price=@fine_price,
				Insurance_price=@Insurance_price,
				pure_price=@Pure,
				mileage_price=@Mileage,		--20201214 UPD BY JERRY	增加更新里程費
				parkingFee=@ParkingFeeTotal	-- 20210506;ADD BY YEH REASON.新增停車費用(總)
			WHERE order_number=@OrderNo;
				
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_NPR136Save WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo;
			IF @hasData=0
			BEGIN
				INSERT INTO TB_NPR136Save([PROCD],[ORDNO] ,[IRENTORDNO],[CUSTID],[CUSTNM]
										,[BIRTH],[CUSTTYPE],[ODCUSTID],[CARTYPE],[CARNO]
										,[TSEQNO],[GIVEDATE],[GIVETIME],[RENTDAYS],[GIVEKM]
										,[OUTBRNHCD],[RNTDATE],[RNTTIME],[RNTKM],[INBRNHCD]
										,[RPRICE],[RINSU],[DISRATE],[OVERHOURS],[OVERAMT2]
										,[RNTAMT],[RENTAMT],[LOSSAMT2],[PROJID],[REMARK]
										,[INVKIND],[UNIMNO],[INVTITLE],[INVADDR],[GIFT]
										,[GIFT_MOTO],[CARDNO],[PAYAMT],[AUTHCODE],[isRetry]
										,[RetryTimes],[CARRIERID],[NPOBAN],[NOCAMT]  
										,CTRLAMT,CTRLMEMO,CLEANAMT,CLEANMEMO,EQUIPAMT
										,EQUIPMEMO,TOWINGAMT,TOWINGMEMO,OTHERAMT,OTHERMEMO
										,PARKINGMEMO,PARKINGAMT2,PARKINGMEMO2) 
				SELECT [PROCD],[ORDNO] ,[IRENTORDNO],[CUSTID],[CUSTNM]
					,[BIRTH],[CUSTTYPE],[ODCUSTID],[CARTYPE],[CARNO]
					,[TSEQNO],CONVERT(VARCHAR(10),@StartDate,112),REPLACE(CONVERT(VARCHAR(5), @StartDate,108),':',''),(DATEDIFF(day, @StartDate, @EndDate)),@start_mile
					,[OUTBRNHCD],CONVERT(VARCHAR(10),@EndDate,112),REPLACE(CONVERT(VARCHAR(5), @EndDate,108),':',''),@end_mile,[INBRNHCD]
					,[RPRICE],[RINSU],[DISRATE],[OVERHOURS],@fine_price
					,@FinalPrice,@Pure,@Mileage,[PROJID],'iRent單號【'+ISNULL(@IRENTORDNO,'')+'】'
					,[INVKIND],[UNIMNO],[INVTITLE],[INVADDR],@CarPoint
					,@MotorPoint,[CARDNO],@PAYAMT,[AUTHCODE],1
					,[RetryTimes],[CARRIERID],[NPOBAN],@Insurance_price  
					,CTRLAMT,CTRLMEMO,CLEANAMT,CLEANMEMO,EQUIPAMT
					,EQUIPMEMO,TOWINGAMT,TOWINGMEMO,OTHERAMT,OTHERMEMO
					,PARKINGMEMO,PARKINGAMT2,PARKINGMEMO2
				FROM VW_BE_BeforeNPR136GetData WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo;

				-- 20210506;MARK BY YEH REASON.將這段UPDATE邏輯移至上面處理，這段舊不需要了
				--UPDATE TB_NPR136Save 
				--SET GIFT=@CarPoint,
				--	GIFT_MOTO=@MotorPoint,
				--	RNTAMT=@FinalPrice,		--20210206 ADD BY ADAM REASON.修正備註,還車費用,逾時費,里程費未存檔
				--	[GIVEDATE]=CONVERT(VARCHAR(10),@StartDate,112),
				--	[RNTDATE]=CONVERT(VARCHAR(10),@EndDate,112),
				--	[GIVETIME]=REPLACE(CONVERT(VARCHAR(5), @StartDate,108),':',''),
				--	[RNTTIME]=REPLACE(CONVERT(VARCHAR(5), @EndDate,108),':',''),
				--	[GIVEKM]=@start_mile,
				--	[RNTKM]=@end_mile,
				--	isRetry=1,
				--	PAYAMT=@PAYAMT,
				--	RENTDAYS=(DATEDIFF ( day , @StartDate , @EndDate )  ),
				--	NOCAMT=@Insurance_price,
				--	--20210206 ADD BY ADAM REASON.修正備註,還車費用,逾時費,里程費未存檔
				--	REMARK='iRent單號【'+ISNULL(@IRENTORDNO,'')+'】',
				--	RENTAMT=@Pure,
				--	LOSSAMT2=@Mileage,
				--	OVERAMT2=@fine_price
				--WHERE IRENTORDNO=@OrderNo;

				-- 20210506;ADD BY YEH REASON.把LOG機制修正，資料更新後就寫LOG
				INSERT INTO TB_NPR136SaveHistory([PROCD],[ORDNO] ,[IRENTORDNO],[CUSTID],[CUSTNM]
												,[BIRTH],[CUSTTYPE],[ODCUSTID],[CARTYPE],[CARNO]
												,[TSEQNO],[GIVEDATE],[GIVETIME],[RENTDAYS],[GIVEKM]
												,[OUTBRNHCD],[RNTDATE],[RNTTIME],[RNTKM],[INBRNHCD]
												,[RPRICE],[RINSU],[DISRATE],[OVERHOURS],[OVERAMT2]
												,[RNTAMT],[RENTAMT],[LOSSAMT2],[PROJID],[REMARK]
												,[INVKIND],[UNIMNO],[INVTITLE],[INVADDR],[GIFT]
												,[GIFT_MOTO],[CARDNO],[PAYAMT],[AUTHCODE],[isRetry]
												,[RetryTimes],[CARRIERID],[NPOBAN],[NOCAMT]  
												,CTRLAMT,CTRLMEMO,CLEANAMT,CLEANMEMO,EQUIPAMT
												,EQUIPMEMO,TOWINGAMT,TOWINGMEMO,OTHERAMT,OTHERMEMO
												,PARKINGMEMO,PARKINGAMT2,PARKINGMEMO2) 
				SELECT [PROCD],[ORDNO] ,[IRENTORDNO],[CUSTID],[CUSTNM]
						,[BIRTH],[CUSTTYPE],[ODCUSTID],[CARTYPE],[CARNO]
						,[TSEQNO],[GIVEDATE],[GIVETIME],[RENTDAYS],[GIVEKM]
						,[OUTBRNHCD],[RNTDATE],[RNTTIME],[RNTKM],[INBRNHCD]
						,[RPRICE],[RINSU],[DISRATE],[OVERHOURS],[OVERAMT2]
						,[RNTAMT],[RENTAMT],[LOSSAMT2],[PROJID],[REMARK]
						,[INVKIND],[UNIMNO],[INVTITLE],[INVADDR],[GIFT]
						,[GIFT_MOTO],[CARDNO],[PAYAMT],[AUTHCODE],[isRetry]
						,[RetryTimes],[CARRIERID],[NPOBAN],[NOCAMT]  
						,CTRLAMT,CTRLMEMO,CLEANAMT,CLEANMEMO,EQUIPAMT
						,EQUIPMEMO,TOWINGAMT,TOWINGMEMO,OTHERAMT,OTHERMEMO
						,PARKINGMEMO,PARKINGAMT2,PARKINGMEMO2 
				FROM TB_NPR136Save WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo;
			END
			ELSE
			BEGIN
				UPDATE TB_NPR136Save 
				SET GIFT=@CarPoint
					,GIFT_MOTO=@MotorPoint
					,RNTAMT=@FinalPrice		--20210206 ADD BY ADAM REASON.修正備註,還車費用,逾時費,里程費未存檔
					,[GIVEDATE]=CONVERT(VARCHAR(10),@StartDate,112)
					,[RNTDATE]=CONVERT(VARCHAR(10),@EndDate,112)
					,[GIVETIME]=REPLACE(CONVERT(VARCHAR(5), @StartDate,108),':','')
					,[RNTTIME]=REPLACE(CONVERT(VARCHAR(5), @EndDate,108),':','')
					,[GIVEKM]=@start_mile
					,[RNTKM]=@end_mile
					,isRetry=1
					,PAYAMT=@PAYAMT
					,RENTDAYS=(DATEDIFF(day, @StartDate, @EndDate) )
					,NOCAMT=@Insurance_price
					--20210206 ADD BY ADAM REASON.修正備註,還車費用,逾時費,里程費未存檔
					,RENTAMT=@Pure
					,LOSSAMT2=@Mileage
					,OVERAMT2=@fine_price
					--20210208 ADD BY ADAM REASON.
					,CTRLAMT=@CarDispatch
					,CTRLMEMO=@DispatchRemark
					,CLEANAMT=@CleanFee
					,CLEANMEMO=@CleanFeeRemark
					,EQUIPAMT=@DestroyFee
					,EQUIPMEMO=@DestroyFeeRemark
					,PARKINGAMT=@ParkingFee
					,PARKINGMEMO=@ParkingFeeRemark
					--,PARKINGAMT2=@ParkingFeeByMachi			-- 20210506;MARK BY YEH REASON.這兩個欄位目前也沒使用，就不再更新了
					--,PARKINGMEMO2=@ParkingFeeByMachiRemark
					,OTHERAMT=@OtherFee
					,OTHERMEMO=@OtherFeeRemark
				WHERE IRENTORDNO=@OrderNo;

				-- 20210506;ADD BY YEH REASON.把LOG機制修正，資料更新後就寫LOG
				INSERT INTO TB_NPR136SaveHistory([PROCD],[ORDNO] ,[IRENTORDNO],[CUSTID],[CUSTNM]
												,[BIRTH],[CUSTTYPE],[ODCUSTID],[CARTYPE],[CARNO]
												,[TSEQNO],[GIVEDATE],[GIVETIME],[RENTDAYS],[GIVEKM]
												,[OUTBRNHCD],[RNTDATE],[RNTTIME],[RNTKM],[INBRNHCD]
												,[RPRICE],[RINSU],[DISRATE],[OVERHOURS],[OVERAMT2]
												,[RNTAMT],[RENTAMT],[LOSSAMT2],[PROJID],[REMARK]
												,[INVKIND],[UNIMNO],[INVTITLE],[INVADDR],[GIFT]
												,[GIFT_MOTO],[CARDNO],[PAYAMT],[AUTHCODE],[isRetry]
												,[RetryTimes],[CARRIERID],[NPOBAN],[NOCAMT]  
												,CTRLAMT,CTRLMEMO,CLEANAMT,CLEANMEMO,EQUIPAMT
												,EQUIPMEMO,TOWINGAMT,TOWINGMEMO,OTHERAMT,OTHERMEMO
												,PARKINGMEMO,PARKINGAMT2,PARKINGMEMO2) 
				SELECT [PROCD],[ORDNO] ,[IRENTORDNO],[CUSTID],[CUSTNM]
						,[BIRTH],[CUSTTYPE],[ODCUSTID],[CARTYPE],[CARNO]
						,[TSEQNO],[GIVEDATE],[GIVETIME],[RENTDAYS],[GIVEKM]
						,[OUTBRNHCD],[RNTDATE],[RNTTIME],[RNTKM],[INBRNHCD]
						,[RPRICE],[RINSU],[DISRATE],[OVERHOURS],[OVERAMT2]
						,[RNTAMT],[RENTAMT],[LOSSAMT2],[PROJID],[REMARK]
						,[INVKIND],[UNIMNO],[INVTITLE],[INVADDR],[GIFT]
						,[GIFT_MOTO],[CARDNO],[PAYAMT],[AUTHCODE],[isRetry]
						,[RetryTimes],[CARRIERID],[NPOBAN],[NOCAMT]  
						,CTRLAMT,CTRLMEMO,CLEANAMT,CLEANMEMO,EQUIPAMT
						,EQUIPMEMO,TOWINGAMT,TOWINGMEMO,OTHERAMT,OTHERMEMO
						,PARKINGMEMO,PARKINGAMT2,PARKINGMEMO2 
				FROM TB_NPR136Save WITH(NOLOCK) WHERE IRENTORDNO=@OrderNo;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleOrderModify';
GO