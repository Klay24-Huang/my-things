/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_JointRentInviteeVerify_Q01
* 系    統 : IRENT
* 程式功能 : 共同承租人邀請檢核
* 作    者 : AMBER
* 撰寫日期 : 20210825
* 修改日期 : 20210830 UPD BY AMBER REASON: 修正判斷副承租人同時段是否有預約或合約邏輯
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_JointRentInviteeVerify_Q01]
	@QureyId                VARCHAR(20)           , --要邀請的ID或手機(原input參數)
	@OrderNo                BIGINT                , --訂單編號
	@Token                  VARCHAR(1024)         , --JWT TOKEN
	@IDNO                   VARCHAR(20)           , --帳號
	@LogID                  BIGINT                , --執行的api log
	@InviteeId              VARCHAR(20)     OUTPUT, --被邀請的ID		
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
DECLARE @Seat TINYINT ;
DECLARE @ProjType  VARCHAR(10);
DECLARE @Audit_Car TINYINT;
DECLARE @Audit_Moto TINYINT;
DECLARE @SD      DATETIME;
DECLARE @ED      DATETIME;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_JointRentInviteeVerify_Q01';
SET @IsSystem=0;
SET @ErrorType=0;
SET @IsSystem=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());
SET @Token=ISNULL (@Token,'');
SET @IDNO=ISNULL (@IDNO,'');
SET @OrderNo=ISNULL (@OrderNo,0);
SET @Seat=0;
SET @Audit_Car=0;
SET @Audit_Moto=0;
SET @ProjType='';

	BEGIN TRY
        IF @Token='' OR @LogID=''
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR900'		
		END
		        
        --0.再次檢核token
		IF @Error=0
		BEGIN
			SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token  AND Rxpires_in>@NowTime;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
			ELSE
			BEGIN
				SET @hasData=0;
				SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE  Access_Token=@Token AND MEMIDNO=@IDNO;
				IF @hasData=0
				BEGIN
					SET @Error=1;
					SET @ErrorCode='ERR101';
				END
			END
		END

		--1.判斷會員狀態(已審核、通過手機驗證、非黑名單)
		IF @Error=0
		BEGIN
		IF ISNUMERIC(@QureyId)>0 
		BEGIN
		   IF SUBSTRING(@QureyId,1,2) <> '09'
		    BEGIN
			SET @Error=1
			SET @ErrorCode='ERR919'
		    END
		   
		   IF @Error=0
		   BEGIN 
				SET @hasData=0
				SELECT @hasData=COUNT(1),@InviteeId=m.MEMIDNO FROM TB_MemberData m WITH(NOLOCK) 
				JOIN TB_MemberScoreMain s  WITH(NOLOCK)  ON m.MEMIDNO=s.MEMIDNO 
				WHERE  m.Audit=1 and m.HasCheckMobile=1 and s.ISBLOCK=0
				AND m.MEMTEL=@QureyId
			  	GROUP BY m.MEMIDNO;
		   END

		   IF @hasData=0
		    BEGIN
				SET @Error=1
				SET @ErrorCode='ERR919'
		    END
		END   
	    ELSE
		BEGIN		  
			SET @hasData=0;
			SELECT @hasData= COUNT(1),@InviteeId=m.MEMIDNO FROM  TB_MemberData m WITH(NOLOCK) 
			JOIN TB_MemberScoreMain s  WITH(NOLOCK)  ON m.MEMIDNO=s.MEMIDNO 
			WHERE  m.Audit=1 and m.HasCheckMobile=1 and s.ISBLOCK=0 
			AND m.MEMIDNO=@QureyId
			GROUP BY m.MEMIDNO;

			IF @hasData=0
				BEGIN
				SET @Error=1
				SET @ErrorCode='ERR919'
				END
		END
		END

		--1.1判斷會員狀態(駕照已審核)
		IF @Error=0
		BEGIN
			SELECT @Seat=Seat,
				   @ProjType=CASE WHEN  ProjType in (0,3) THEN 'Car'  WHEN ProjType=4  THEN 'Moto' ELSE 'MA' END,
				   @SD=m.start_time,
				   @ED=m.stop_time
			FROM   TB_OrderMain m WITH(NOLOCK) JOIN TB_CarInfo o WITH(NOLOCK) ON m.CarNo=o.CarNo
			WHERE  m.order_number=@OrderNo;	

			SELECT 
			@Audit_Car= CASE WHEN ISNULL(c.CarDriver_1,0) <>2 OR ISNULL(c.CarDriver_2,0) <>2 THEN -1
						WHEN ISNULL(c.Law_Agent,0) <> 2 AND DATEDIFF(MONTH,m.MEMBIRTH,DATEADD(HOUR,8,GETDATE()))/12 <20 THEN -1
						ELSE ISNULL(c.CarDriver_1,0) END,
			@Audit_Moto=CASE WHEN ISNULL(c.Law_Agent,0) <> 2 AND DATEDIFF(MONTH,m.MEMBIRTH,DATEADD(HOUR,8,GETDATE()))/12 <20 THEN -1			
			            WHEN ISNULL(c.CarDriver_1,0)=2 AND ISNULL(c.CarDriver_2,0)=2 THEN 2		
				        WHEN ISNULL(c.MotorDriver_1,0) <>2 OR ISNULL(c.MotorDriver_2,0) <>2 THEN -1			   	          				 
				        ELSE ISNULL(c.MotorDriver_1,0) END
			FROM TB_MemberData m WITH(NOLOCK) 
			JOIN TB_Credentials c WITH(NOLOCK) ON m.MEMIDNO=c.IDNO
			WHERE  m.MEMIDNO=@InviteeId;

			IF (@Audit_Car <> 2 AND @ProjType='Car') OR (@Audit_Moto <> 2 AND @ProjType='Moto')
				BEGIN
					SET @Error=1
					SET @ErrorCode='ERR919'
				END
		END 

	    --2.判斷副承租人同時段是否有預約或合約
		IF @Error=0
		BEGIN		

			SET @hasData=0;		
		　　SELECT @hasData=count(1) FROM TB_OrderMain o WITH(NOLOCK) 
			WHERE IDNO=@InviteeId
			AND (o.cancel_status =0 AND o.car_mgt_status =0 AND o.booking_status=0)
			OR (o.car_mgt_status >=4 AND o.car_mgt_status <16)
		    AND (o.start_time BETWEEN @SD AND @ED) AND  (stop_time BETWEEN @SD AND @ED) 
			AND EXISTS (SELECT 1 FROM TB_TogetherPassenger t WITH(NOLOCK) 
			WHERE o.IDNO=t.MEMIDNO AND t.ChkType IN ('Y','S'));
		
			IF @hasData>0
				 BEGIN
					SET @Error=1
					SET @ErrorCode='ERR920'
				  END
		END 

		--3.判斷邀請人數上限
		IF @Error=0
		BEGIN
		
			SET @hasData=0
			SELECT @hasData=count(1) FROM TB_TogetherPassenger  WITH(NOLOCK) WHERE Order_number=@OrderNo;

			IF @hasData>=@Seat-1
					BEGIN
						SET @Error=1
						SET @ErrorCode='ERR921'
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_JointRentInviteeVerify_Q01'