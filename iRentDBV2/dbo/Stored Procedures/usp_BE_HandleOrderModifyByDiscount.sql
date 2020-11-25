/****************************************************************
** Name: [dbo].[usp_BE_HandleOrderModifyByDiscount]
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
** EXEC @Error=[dbo].[usp_BE_HandleOrderModifyByDiscount]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/11/24 上午 10:44:33 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/11/24 上午 10:44:33    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_HandleOrderModifyByDiscount]
    @OrderNo                BIGINT                ,
	@CarPoint               INT                   ,
	@MotorPoint             INT                   ,
	@RNTAMT                 INT                   ,
	@FinalPrice             INT                   ,
	@Reson					INT      			  ,
	@Remark					NVARCHAR(50)		  ,
	@PAYAMT                 INT                   ,
	@UserID                 NVARCHAR(10)          , --使用者
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
DECLARE @Descript NVARCHAR(200);
DECLARE @NowTime DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_BE_HandleOrderModifyByDiscount';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());

SET @OrderNo    =ISNULL(@OrderNo    ,-1);
SET @CarPoint   =ISNULL(@CarPoint   ,-1);
SET @MotorPoint =ISNULL(@MotorPoint ,-1);
SET @FinalPrice =ISNULL(@FinalPrice ,-1);
SET @UserID    =ISNULL (@UserID    ,'');


		BEGIN TRY

		 
		 IF @UserID='' OR @OrderNo<=0 OR @CarPoint<0 OR @MotorPoint<0 OR @FinalPrice<0
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
				INSERT INTO TB_OrderModifyLog([order_number],[already_lend_car],[already_return_car],[extend_stop_time],[force_extend_stop_time],
											  [final_start_time],[final_stop_time],[start_door_time],[end_door_time],[transaction_no],
											  [final_price],[pure_price],[mileage_price],[Insurance_price],[fine_price],[fine_interval],
											  [fine_rate],[gift_point],[gift_motor_point],[monthly_workday],[monthly_holiday],
											  [Etag],[already_payment],[start_mile],[end_mile],[trade_status],
											  [parkingFee], [parkingSpace], [TransDiscount], 
											 [U_USERID],[U_SYSDT],[U_Reson],[U_Remark]) SELECT *,@UserID,@NowTime,@Reson,@Remark FROM TB_OrderDetail  WITH(NOLOCK) WHERE order_number=@OrderNo;
				UPDATE TB_OrderDetail
				SET gift_motor_point=@MotorPoint,gift_point=@CarPoint,final_price=@FinalPrice
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
											 FROM VW_BE_BeforeNPR136GetData WHERE IRENTORDNO=@OrderNo;

					UPDATE TB_NPR136Save SET GIFT=@CarPoint,GIFT_MOTO=@MotorPoint,RENTAMT=@FinalPrice,isRetry=1,PAYAMT=@PAYAMT
					WHERE IRENTORDNO=@OrderNo;
				END
				ELSE
				BEGIN
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
					UPDATE TB_NPR136Save SET GIFT=@CarPoint,GIFT_MOTO=@MotorPoint,RENTAMT=@FinalPrice,isRetry=1,PAYAMT=@PAYAMT
					WHERE IRENTORDNO=@OrderNo;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleOrderModifyByDiscount';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleOrderModifyByDiscount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'合約修改(點數)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleOrderModifyByDiscount';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleOrderModifyByDiscount';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_BE_HandleOrderModifyByDiscount';