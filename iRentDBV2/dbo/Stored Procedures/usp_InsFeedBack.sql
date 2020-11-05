﻿/****************************************************************
** Name: [dbo].[usp_InsFeedBack]
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
** EXEC @Error=[dbo].[usp_InsFeedBack]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/19 上午 05:52:01 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/19 上午 05:52:01    |  Eric|          First Release
**			 |			  |
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_InsFeedBack]
	@IDNO                   VARCHAR(10)           ,
	@OrderNo                BIGINT                ,
	@Mode					INT					  ,
	@Star					INT					  ,
	@FeedBackKind           VARCHAR(1024)         ,
	@Descript               NVARCHAR(500)         ,
	@Token                  VARCHAR(1024)         ,
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
DECLARE @car_mgt_status TINYINT;
DECLARE @cancel_status TINYINT;
DECLARE @booking_status TINYINT;
DECLARE @PIC1 NVARCHAR(100);
DECLARE @PIC2 NVARCHAR(100);
DECLARE @PIC3 NVARCHAR(100);
DECLARE @PIC4 NVARCHAR(100);

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';
SET @FunName='usp_InsFeedBack';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @car_mgt_status=0;
SET @cancel_status =0;
SET @booking_status=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @IDNO    =ISNULL (@IDNO    ,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Token    =ISNULL (@Token    ,'');
SET @Descript=ISNULL(@Descript,'');
SET @Mode=ISNULL(@Mode,-1);
SET @Star=ISNULL(@Star,-1);
SET @PIC1='';
SET @PIC2='';
SET @PIC3='';
SET @PIC4='';

		BEGIN TRY

		 
		 IF @Token='' OR @IDNO=''  OR @OrderNo=0 OR @Descript='' OR @Mode=-1 OR @Star=-1
		 BEGIN
		   SET @Error=1;
		   SET @ErrorCode='ERR900'
 		 END
		 
		  --0.再次檢核token
		 IF @Error=0
		 BEGIN
		 	SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
			ELSE
			BEGIN
			    SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_Token WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
				IF @hasData=0
				BEGIN
				   SET @Error=1;
				   SET @ErrorCode='ERR101';
				END
			END
		 END
		 IF @Error=0
		 BEGIN
			IF @Mode<2
			BEGIN
			    SET @hasData=0
				
				SELECT @hasData=COUNT(order_number)  FROM TB_OrderMain WHERE IDNO=@IDNO AND order_number=@OrderNo 
					AND ((@Mode=0 AND car_mgt_status<=3 AND cancel_status=0 AND booking_status<3) 
						OR (@Mode=1 AND car_mgt_status >= 16 AND cancel_status=0));
				IF @hasData=0
				BEGIN
				    SET @Error=1;
				    SET @ErrorCode='ERR223';
				END
				ELSE
				BEGIN
					        SELECT @booking_status=booking_status,@cancel_status=cancel_status,@car_mgt_status=car_mgt_status
							FROM TB_OrderMain
							WHERE order_number=@OrderNo;
							IF @Mode=0 AND @car_mgt_status>4
							BEGIN
							  SET @Error=1;
							  SET @ErrorCode='ERR224';
							END
							ELSE
							BEGIN
								IF @Mode=1 AND @car_mgt_status<16
								BEGIN
									  SET @Error=1;
										SET @ErrorCode='ERR225';
								END
							END
				END
			END
		 END
		 IF @Error=0
		 BEGIN
				IF @Mode=0
				BEGIN
					  SELECT  @PIC1=ISNULL(FeedBackFile,'') FROM [TB_tmpFeedBackPIC] WHERE SEQNO=1 AND LEN(FeedBackFile)<=100 AND OrderNo=@OrderNo;
					  SELECT  @PIC2=ISNULL(FeedBackFile,'') FROM [TB_tmpFeedBackPIC] WHERE SEQNO=2 AND LEN(FeedBackFile)<=100 AND OrderNo=@OrderNo;
					  SELECT  @PIC3=ISNULL(FeedBackFile,'') FROM [TB_tmpFeedBackPIC] WHERE SEQNO=3 AND LEN(FeedBackFile)<=100 AND OrderNo=@OrderNo;
					  SELECT  @PIC4=ISNULL(FeedBackFile,'') FROM [TB_tmpFeedBackPIC] WHERE SEQNO=4 AND LEN(FeedBackFile)<=100 AND OrderNo=@OrderNo;
				END
				INSERT INTO TB_FeedBack(IDNO,OrderNo,mode,FeedBackKind,descript,star,PIC1,PIC2,PIC3,PIC4)VALUES(@IDNO,@OrderNo,@Mode,@FeedBackKind,@Descript,@Star,@PIC1,@PIC2,@PIC3,@PIC4);

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsFeedBack';


GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsFeedBack';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寫入取還車回饋', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsFeedBack';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsFeedBack';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_InsFeedBack';