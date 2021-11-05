/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetMemberStatus
* 系    統 : IRENT
* 程式功能 : 取得會員狀態
* 作    者 : ADAM
* 撰寫日期 : 20201016
* 修改日期 : 20201204 ADD BY ADAM REASON.改為線上統計
			 20210104 UPD BY JERRY 只要有一個審核通過，就不顯示審核異常
			 20210105 UPD BY JERRY 更新自拍照沒有，但是審核通過就不顯示未完成註冊訊息
			 20210106 UPD BY JERRY 更新判斷邏輯，如果汽車駕照是審核通過機車就可以使用
			 20210504 ADD BY JET 姓名/生日/地址未填，顯示1:未完成註冊
			 20210504 ADD BY JET 未滿20歲且未上傳法定代理人，顯示2:完成註冊未上傳照片
			 20210507 ADD BY YEH 基本資料(姓名/生日/地址) 照片未上傳(身分證/駕照/自拍照/簽名檔/未滿20歲+法定代理人) 要顯示錯誤訊息
			 20210517 ADD BY YEH 增加積分相關欄位
			 20210519 ADD BY YEH 基本資料(姓名/生日/地址) 照片未上傳(身分證/駕照/自拍照/簽名檔/未滿20歲+法定代理人) 要顯示錯誤訊息
			 20210702 UPD BY YEH REASON:積分<0顯示0
			 20210708 UPD BY Olivia 調整會員權限檢核順序
			 20210811 UPD BY YEH REASON:增加會員條款狀態
			 20210910 UPD BY YEH REASON:增加是否顯示購買牌卡
			 20210917 ADD BY ADAM REASON.是否有推播判斷
			 20211105 UPD BY YEH REASON:增加預授權條款狀態
* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_GetMemberStatus]
	@IDNO		            VARCHAR(10)           ,
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
DECLARE @Audit	INT;

/*初始設定*/
SET @Error=0;
SET @ErrorCode='0000';
SET @ErrorMsg='SUCCESS'; 
SET @SQLExceptionCode='';
SET @SQLExceptionMsg='';

SET @FunName='usp_GetMemberStatus';
SET @IsSystem=0;
SET @ErrorType=0;
SET @hasData=0;
SET @NowTime=DATEADD(HOUR,8,GETDATE());

BEGIN TRY
    IF @Token='' OR @IDNO=''
	BEGIN
		SET @Error=1;
		SET @ErrorCode='ERR900'
	END
		        
    --0.再次檢核token
	IF @Error=0
	BEGIN
		SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND Rxpires_in>@NowTime;
		IF @hasData=0
		BEGIN
			SET @Error=1;
			SET @ErrorCode='ERR101';
		END
		ELSE
		BEGIN
			SET @hasData=0;
			SELECT @hasData=COUNT(1) FROM TB_Token WITH(NOLOCK) WHERE Access_Token=@Token AND MEMIDNO=@IDNO;
			IF @hasData=0
			BEGIN
				SET @Error=1;
				SET @ErrorCode='ERR101';
			END
		END
	END

	--1.取得資料
	IF @Error=0
	BEGIN
		DROP TABLE IF EXISTS #OrderProjCount;
		DROP TABLE IF EXISTS #CMKDef;

		--統計在線案件數量
		SELECT ProjType,COUNT(ProjType) AS Total
		INTO #OrderProjCount
		FROM TB_OrderMain WITH(NOLOCK)
		WHERE IDNO=@IDNO
		AND cancel_status =0
		AND car_mgt_status < 16 --AND car_mgt_status > 3
		AND stop_time > DATEADD(DAY,-90,GETDATE())
		GROUP BY ProjType;

		-- 20210813 UPD BY YEH REASON:取得設定檔資料
		SELECT VerType,MAX(Version) AS Version
		INTO #CMKDef
		FROM TB_CMKDef
		WHERE @NowTime >= SDATE
		GROUP BY VerType;

		SELECT @Audit=Audit FROM TB_MemberData WITH(NOLOCK) WHERE MEMIDNO=@IDNO;

		--20210917 ADD BY ADAM REASON.是否有未讀推播判斷
		DECLARE @HasNoticeMsg VARCHAR(1)
				,@NOTIFYTIME DATETIME
		SELECT TOP 1 @HasNoticeMsg = CASE WHEN COUNT(A.NotificationID)>0 THEN 'Y' ELSE 'N' END
		,@NOTIFYTIME=MAX(A.MKTime)
		FROM TB_PersonNotification A WITH(NOLOCK)
		LEFT JOIN TB_PersonNotificationRLog B WITH(NOLOCK) ON A.NotificationID=B.NotificationID
		WHERE A.IDNO=@IDNO 
		AND A.MKTime > DATEADD(day,-1,@NowTime)
		AND B.NotificationID IS NULL
		GROUP BY A.NotificationID
		ORDER BY A.NotificationID DESC


		IF @Audit = 1	--審核過以主檔判定，沒過用待審
		BEGIN
			SELECT A.MEMIDNO
				,A.MEMCNAME AS MEMNAME
				,Login				= 'Y'		--登入
				,Register			= A.IrFlag	--註冊
				,A.Audit 
				,Audit_ID			= ISNULL(C.ID_1,0)				--身分證
				,Audit_Car			= CASE WHEN ISNULL(C.ID_1,0)<>2 OR ISNULL(C.ID_2,0)<>2 THEN -1
											WHEN ISNULL(C.Self_1,0) <> 2 THEN -1
											WHEN ISNULL(C.Signture,0) <> 2 THEN -1
											WHEN ISNULL(C.Law_Agent,0) <> 2 AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 <20 THEN -1
											ELSE ISNULL(C.CarDriver_1,0) END		--汽車駕照
				,Audit_Motor		= CASE WHEN ISNULL(C.ID_1,0)<>2 OR ISNULL(C.ID_2,0)<>2 THEN -1
											WHEN ISNULL(C.Self_1,0) <> 2 THEN -1
											WHEN ISNULL(C.Signture,0) <> 2 THEN -1
											WHEN ISNULL(C.Law_Agent,0) <> 2 AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 <20 THEN -1
											--20210106 UPD BY JERRY 更新判斷邏輯，如果汽車駕照是審核通過機車就可以使用
											WHEN ISNULL(C.CarDriver_1,0)=2 THEN 2
											ELSE ISNULL(C.MotorDriver_1,0) END		--機車駕照
				,Audit_Selfie		= ISNULL(C.Self_1,0)			--自拍照
				,Audit_F01			= ISNULL(C.Law_Agent,0)			--法定代理人
				,Audit_Signture		= ISNULL(C.Signture,0)			--簽名檔
				--會員頁9.0卡狀態 (0:PASS 1:未完成註冊 2:完成註冊未上傳照片 3:身分審核中 4:審核不通過 5:身分變更審核中 6:身分變更審核失敗)
				,MenuCTRL			= CASE WHEN A.IrFlag=0 THEN 1
											-- 20210504 ADD BY JET 姓名/生日/地址未填，顯示1:未完成註冊
											WHEN ISNULL(A.MEMCNAME,'')='' OR ISNULL(CONVERT(VARCHAR, A.MEMBIRTH, 111),'')='' OR ISNULL(A.MEMADDR,'')='' THEN 2
											--身分證未上傳,駕照只要上其中一個
											--WHEN C.ID_1=0 OR (C.CarDriver_1=0 AND C.MotorDriver_1=0) OR C.Self_1=0 THEN 2
											--20210105 UPD BY JERRY 更新自拍照沒有，但是審核通過就不顯示未完成註冊訊息
											WHEN C.ID_1=0 OR (C.CarDriver_1=0 AND C.MotorDriver_1=0) OR (C.Self_1=0 AND Audit<>1)  THEN 2
											-- 20210504 ADD BY JET 未滿20歲且未上傳法定代理人，顯示2:完成註冊未上傳照片
											WHEN ISNULL(C.Law_Agent,0)=0 AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 >=18 
																		AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 <20 THEN 2
											WHEN A.Audit=0 THEN 3
											WHEN A.Audit=2 THEN 4
											--20210708 UPDATE BY OLIVIA 調整檢查順序，身份證/自拍照/簽名檔/未滿20歲未上傳法定代理人/駕照2選1
											WHEN A.Audit=1 AND C.ID_1=1 THEN 5--檢查身份證
											WHEN A.Audit=1 AND C.ID_1=-1 THEN 6
											WHEN A.Audit=1 AND C.Self_1=1 THEN 5--檢查自拍照
											WHEN A.Audit=1 AND C.Self_1=-1 THEN 6
											WHEN A.Audit=1 AND C.Signture=1 THEN 5--檢查簽名檔
											WHEN A.Audit=1 AND C.Signture=-1 THEN 6											
											WHEN A.Audit=1 AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 >=18 
														AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 <20
														AND C.Law_Agent=-1 THEN 6
											WHEN A.Audit=1 AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 >=18 
														AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 <20
														AND C.Law_Agent=1 THEN 5
											--20210104 UPD BY JERRY 只要有一個審核通過，就不顯示審核異常
											WHEN A.Audit=1 AND (C.CarDriver_1=2 OR C.MotorDriver_1=2) THEN 0
											--20210106 UPD BY JERRY 更新判斷邏輯，如果汽車駕照是審核通過機車就可以使用
											WHEN A.Audit=1 AND C.MotorDriver_1=-1 AND ISNULL(C.CarDriver_1,0)=2 THEN 0
											WHEN A.Audit=1 AND C.CarDriver_1=1 THEN 5
											WHEN A.Audit=1 AND C.MotorDriver_1=1 THEN 5
											WHEN A.Audit=1 AND C.CarDriver_1=-1 THEN 6
											WHEN A.Audit=1 AND C.MotorDriver_1=-1 THEN 6											
											ELSE 0 END
				--會員頁9.0狀態顯示 (這邊要通過審核才會有文字 MenuCTRL5 6才會有文字提示)
				,MenuStatusText		= CASE WHEN A.Audit=1 AND C.ID_1=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND C.ID_1=-1 THEN '身分變更審核失敗'
											WHEN A.Audit=1 AND C.Self_1=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND C.Self_1=-1 THEN '身分變更審核失敗'
											WHEN A.Audit=1 AND C.Signture=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND C.Signture=-1 THEN '身分變更審核失敗'
											WHEN A.Audit=1 AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 >=18 
														AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 <20
														AND C.Law_Agent=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 >=18 
														AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 <20
														AND C.Law_Agent=-1 THEN '身分變更審核失敗'
											WHEN A.Audit=1 AND C.CarDriver_1=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND C.MotorDriver_1=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND C.CarDriver_1=-1 THEN '身分變更審核失敗'
											WHEN A.Audit=1 AND C.MotorDriver_1=-1 THEN '身分變更審核失敗'
											ELSE '' END
				,BlackList			= 'N'
				,StatusTextCar		= CASE WHEN A.IrFlag < 1 THEN '完成註冊/審核，即可開始租車'
											-- 20210507 ADD BY YEH 基本資料(姓名/生日/地址) 照片未上傳(身分證/駕照/自拍照/簽名檔/未滿20歲+法定代理人) 要顯示錯誤訊息
											WHEN ISNULL(A.MEMCNAME,'')='' OR ISNULL(CONVERT(VARCHAR, A.MEMBIRTH, 111),'')='' OR ISNULL(A.MEMADDR,'')='' THEN '完成註冊/審核，即可開始租車'
											--20210708 UPDATE BY OLIVIA 照片狀態原判斷=0調整成判斷小於2的都要顯示
											WHEN C.ID_1<2 OR (C.CarDriver_1<2 AND C.CarDriver_2<2) OR (C.Self_1<2 AND A.Audit<>1)  THEN '完成註冊/審核，即可開始租車'
											--20210708 UPDATE BY OLIVIA 照片狀態原判斷=0調整成判斷小於2的都要顯示
											WHEN ISNULL(C.Law_Agent,0)<2 AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 >=18 
																		AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 <20 THEN '完成註冊/審核，即可開始租車'
											WHEN A.Audit = 0 AND C.CarDriver_1=0 THEN '上傳駕照通過審核，即可開始租車'
											WHEN C.CarDriver_1=1 THEN '身分審核中~'
											WHEN C.CarDriver_1=-1 THEN '審核不通過，請重新提交資料'
											ELSE '' END
				,StatusTextMotor	= CASE WHEN A.IrFlag < 1 THEN '完成註冊/審核，即可開始租車'
											-- 20210519 ADD BY YEH 基本資料(姓名/生日/地址) 照片未上傳(身分證/駕照/自拍照/簽名檔/未滿20歲+法定代理人) 要顯示錯誤訊息
											WHEN ISNULL(A.MEMCNAME,'')='' OR ISNULL(CONVERT(VARCHAR, A.MEMBIRTH, 111),'')='' OR ISNULL(A.MEMADDR,'')='' THEN '完成註冊/審核，即可開始租車'
											--20210708 UPDATE BY OLIVIA 照片狀態原判斷=0調整成判斷小於2的都要顯示
											WHEN C.ID_1<2 OR ((C.MotorDriver_1<2 AND C.MotorDriver_2<2) AND (C.CarDriver_1<2 AND C.CarDriver_2<2)) OR (C.Self_1<2 AND A.Audit<>1)  THEN '完成註冊/審核，即可開始租車'
											--20210708 UPDATE BY OLIVIA 照片狀態原判斷=0調整成判斷小於2的都要顯示
											WHEN ISNULL(C.Law_Agent,0)<2 AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 >=18 
																		AND DATEDIFF(MONTH,A.MEMBIRTH,@NowTime)/12 <20 THEN '完成註冊/審核，即可開始租車'
											WHEN A.Audit = 0 AND C.MotorDriver_1=0 THEN '上傳駕照通過審核，即可開始租車'
											WHEN C.MotorDriver_1=1 THEN '身分審核中~'
											--20210106 UPD BY JERRY 更新判斷邏輯，如果汽車駕照是審核通過機車就可以使用
											WHEN C.MotorDriver_1=-1 AND ISNULL(C.CarDriver_1,0)=2 THEN ''
											WHEN C.MotorDriver_1=-1 THEN '審核不通過，請重新提交資料'
											ELSE '' END
				--20201204 ADD BY ADAM REASON.改為線上統計
				,NormalRentCount	= ISNULL((SELECT SUM(Total) FROM #OrderProjCount WHERE ProjType=0),0)
				,AnyRentCount		= ISNULL((SELECT SUM(Total) FROM #OrderProjCount WHERE ProjType=3),0)
				,MotorRentCount		= ISNULL((SELECT SUM(Total) FROM #OrderProjCount WHERE ProjType=4),0)
				,TotalRentCount		= ISNULL((SELECT SUM(Total) FROM #OrderProjCount),0)
				,Score				= ISNULL(IIF(D.SCORE < 0, 0, D.Score),100)	-- 20210702 UPD BY YEH REASON:積分<0顯示0
				,BlockFlag			= CASE WHEN ISNULL(D.ISBLOCK,0) = 0 THEN 0
										WHEN ISNULL(D.ISBLOCK,0) = 1 AND ISNULL(D.BLOCK_CNT,0) < 3 THEN 1
										WHEN ISNULL(D.ISBLOCK,0) = 1 AND ISNULL(D.BLOCK_CNT,0) >= 3 THEN 2 END
				,BLOCK_EDATE		= ISNULL(CONVERT(varchar, D.BLOCK_EDATE, 111),'')
				,CMKStatus			= ISNULL((SELECT 'N' FROM #CMKDef WHERE VerType='Hims' AND Version=E.Version),'Y')	-- 20210811 UPD BY YEH REASON:增加會員條款狀態
				,IsShowBuy			= CASE WHEN ISNULL(D.Score,100) >= 60 THEN 'Y' ELSE 'N' END	-- 20210910 UPD BY YEH REASON:增加是否顯示購買牌卡
				,HasNoticeMsg		= CASE WHEN R.IDNO IS NULL THEN @HasNoticeMsg 
										   WHEN @NowTime < R.NextTime AND @HasNoticeMsg='N' THEN 'N' 
										   WHEN @NowTime < R.NextTime AND @HasNoticeMsg='Y' AND R.CHKTime < @NOTIFYTIME THEN 'Y'
										   ELSE 'N' END	--20210917 ADD BY ADAM REASON.是否有推播判斷
				,AuthStatus			= ISNULL((SELECT 'N' FROM #CMKDef WHERE VerType='Auth' AND Version=E.Version),'Y')	-- 20211105 UPD BY YEH REASON:增加預授權條款狀態
			FROM TB_MemberData A WITH(NOLOCK)
			LEFT JOIN TB_BookingStatusOfUser B WITH(NOLOCK) ON A.MEMIDNO=B.IDNO
			LEFT JOIN TB_Credentials C WITH(NOLOCK) ON A.MEMIDNO=C.IDNO
			LEFT JOIN TB_MemberScoreMain D WITH(NOLOCK) ON D.MEMIDNO=A.MEMIDNO
			LEFT JOIN TB_MemberCMK E WITH(NOLOCK) ON E.MEMIDNO=A.MEMIDNO
			LEFT JOIN TB_NoticeRLog R WITH(NOLOCK) ON R.IDNO=A.MEMIDNO
			WHERE A.MEMIDNO=@IDNO;
		END
		ELSE
		BEGIN
			SELECT A.MEMIDNO
				,E.MEMCNAME AS MEMNAME
				,Login				= 'Y'		--登入
				,Register			= A.IrFlag	--註冊
				,A.Audit 
				,Audit_ID			= ISNULL(C.ID_1,0)				--身分證
				,Audit_Car			= CASE WHEN ISNULL(C.ID_1,0)<>2 OR ISNULL(C.ID_2,0)<>2 THEN -1
											WHEN ISNULL(C.Self_1,0) <> 2 THEN -1
											WHEN ISNULL(C.Signture,0) <> 2 THEN -1
											WHEN ISNULL(C.Law_Agent,0) <> 2 AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 <20 THEN -1
											ELSE ISNULL(C.CarDriver_1,0) END		--汽車駕照
				,Audit_Motor		= CASE WHEN ISNULL(C.ID_1,0)<>2 OR ISNULL(C.ID_2,0)<>2 THEN -1
											WHEN ISNULL(C.Self_1,0) <> 2 THEN -1
											WHEN ISNULL(C.Signture,0) <> 2 THEN -1
											WHEN ISNULL(C.Law_Agent,0) <> 2 AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 <20 THEN -1
											--20210106 UPD BY JERRY 更新判斷邏輯，如果汽車駕照是審核通過機車就可以使用
											WHEN ISNULL(C.CarDriver_1,0)=2 THEN 2
											ELSE ISNULL(C.MotorDriver_1,0) END		--機車駕照
				,Audit_Selfie		= ISNULL(C.Self_1,0)			--自拍照
				,Audit_F01			= ISNULL(C.Law_Agent,0)			--法定代理人
				,Audit_Signture		= ISNULL(C.Signture,0)			--簽名檔
				--會員頁9.0卡狀態 (0:PASS 1:未完成註冊 2:完成註冊未上傳照片 3:身分審核中 4:審核不通過 5:身分變更審核中 6:身分變更審核失敗)
				,MenuCTRL			= CASE WHEN A.IrFlag=0 THEN 1
											-- 20210504 ADD BY YEH 姓名/生日/地址未填，顯示1:未完成註冊
											WHEN ISNULL(E.MEMCNAME,'')='' OR ISNULL(CONVERT(VARCHAR, E.MEMBIRTH, 111),'')='' OR ISNULL(E.MEMADDR,'')='' THEN 2
											--身分證未上傳,駕照只要上其中一個
											--WHEN C.ID_1=0 OR (C.CarDriver_1=0 AND C.MotorDriver_1=0) OR C.Self_1=0 THEN 2
											--20210105 UPD BY JERRY 更新自拍照沒有，但是審核通過就不顯示未完成註冊訊息
											WHEN C.ID_1=0 OR (C.CarDriver_1=0 AND C.MotorDriver_1=0) OR (C.Self_1=0 AND Audit<>1)  THEN 2
											-- 20210504 ADD BY JET 未滿20歲且未上傳法定代理人，顯示2:完成註冊未上傳照片
											WHEN ISNULL(C.Law_Agent,0)=0 AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 >=18 
																		AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 <20 THEN 2
											WHEN A.Audit=0 THEN 3
											WHEN A.Audit=2 THEN 4											
											--20210708 UPDATE BY OLIVIA 調整檢查順序，身份證/自拍照/簽名檔/未滿20歲未上傳法定代理人/駕照2選1
											WHEN A.Audit=1 AND C.ID_1=1 THEN 5
											WHEN A.Audit=1 AND C.ID_1=-1 THEN 6
											WHEN A.Audit=1 AND C.Self_1=1 THEN 5
											WHEN A.Audit=1 AND C.Self_1=-1 THEN 6
											WHEN A.Audit=1 AND C.Signture=1 THEN 5
											WHEN A.Audit=1 AND C.Signture=-1 THEN 6
											WHEN A.Audit=1 AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 >=18 
														AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 <20
														AND C.Law_Agent=-1 THEN 6
											WHEN A.Audit=1 AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 >=18 
														AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 <20
														AND C.Law_Agent=1 THEN 5
											--20210104 UPD BY JERRY 只要有一個審核通過，就不顯示審核異常
											WHEN A.Audit=1 AND (C.CarDriver_1=2 OR C.MotorDriver_1=2) THEN 0
											--20210106 UPD BY JERRY 更新判斷邏輯，如果汽車駕照是審核通過機車就可以使用
											WHEN A.Audit=1 AND C.MotorDriver_1=-1 AND ISNULL(C.CarDriver_1,0)=2 THEN 0	
											WHEN A.Audit=1 AND C.CarDriver_1=1 THEN 5
											WHEN A.Audit=1 AND C.MotorDriver_1=1 THEN 5
											WHEN A.Audit=1 AND C.CarDriver_1=-1 THEN 6
											WHEN A.Audit=1 AND C.MotorDriver_1=-1 THEN 6
											ELSE 0 END
				--會員頁9.0狀態顯示 (這邊要通過審核才會有文字 MenuCTRL5 6才會有文字提示)
				,MenuStatusText		= CASE WHEN A.Audit=1 AND C.ID_1=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND C.CarDriver_1=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND C.MotorDriver_1=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND C.Self_1=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND C.Signture=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 >=18 
														AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 <20
														AND C.Law_Agent=1 THEN '身分變更審核中'
											WHEN A.Audit=1 AND C.ID_1=-1 THEN '身分變更審核失敗'
											WHEN A.Audit=1 AND C.CarDriver_1=-1 THEN '身分變更審核失敗'
											WHEN A.Audit=1 AND C.MotorDriver_1=-1 THEN '身分變更審核失敗'
											WHEN A.Audit=1 AND C.Self_1=-1 THEN '身分變更審核失敗'
											WHEN A.Audit=1 AND C.Signture=-1 THEN '身分變更審核失敗'
											WHEN A.Audit=1 AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 >=18 
														AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 <20
														AND C.Law_Agent=-1 THEN '身分變更審核失敗'
											ELSE '' END
				,BlackList			= 'N'
				,StatusTextCar		= CASE WHEN A.IrFlag < 1 THEN '完成註冊/審核，即可開始租車'
											-- 20210507 ADD BY YEH 基本資料(姓名/生日/地址) 照片未上傳(身分證/駕照/自拍照/簽名檔/未滿20歲+法定代理人) 要顯示錯誤訊息
											WHEN ISNULL(E.MEMCNAME,'')='' OR ISNULL(CONVERT(VARCHAR, E.MEMBIRTH, 111),'')='' OR ISNULL(E.MEMADDR,'')='' THEN '完成註冊/審核，即可開始租車'
											--20210708 UPDATE BY OLIVIA 照片狀態原判斷=0調整成判斷小於2的都要顯示
											WHEN C.ID_1<2 OR (C.CarDriver_1<2 AND C.CarDriver_2<2) OR (C.Self_1<2 AND A.Audit<>1)  THEN '完成註冊/審核，即可開始租車'
											--20210708 UPDATE BY OLIVIA 照片狀態原判斷=0調整成判斷小於2的都要顯示
											WHEN ISNULL(C.Law_Agent,0)<2 AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 >=18 
																		AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 <20 THEN '完成註冊/審核，即可開始租車'
											WHEN A.Audit = 0 AND C.CarDriver_1=0 THEN '上傳駕照通過審核，即可開始租車'
											WHEN C.CarDriver_1=1 THEN '身分審核中~'
											WHEN C.CarDriver_1=-1 THEN '審核不通過，請重新提交資料'
											ELSE '' END
				,StatusTextMotor	= CASE WHEN A.IrFlag < 1 THEN '完成註冊/審核，即可開始租車'
											-- 20210519 ADD BY YEH 基本資料(姓名/生日/地址) 照片未上傳(身分證/駕照/自拍照/簽名檔/未滿20歲+法定代理人) 要顯示錯誤訊息
											WHEN ISNULL(E.MEMCNAME,'')='' OR ISNULL(CONVERT(VARCHAR, E.MEMBIRTH, 111),'')='' OR ISNULL(E.MEMADDR,'')='' THEN '完成註冊/審核，即可開始租車'
											--20210708 UPDATE BY OLIVIA 狀態核狀態改判斷小於2
											WHEN C.ID_1<2 OR ((C.CarDriver_1<2 AND C.CarDriver_2<2) AND (C.MotorDriver_1<2 AND C.MotorDriver_2<2)) OR (C.Self_1<2 AND A.Audit<>1)  THEN '完成註冊/審核，即可開始租車'
											WHEN ISNULL(C.Law_Agent,0)<2 AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 >=18 
																		AND DATEDIFF(MONTH,E.MEMBIRTH,@NowTime)/12 <20 THEN '完成註冊/審核，即可開始租車'
											WHEN A.Audit = 0 AND C.MotorDriver_1=0 THEN '上傳駕照通過審核，即可開始租車'
											WHEN C.MotorDriver_1=1 THEN '身分審核中~'
											--20210106 UPD BY JERRY 更新判斷邏輯，如果汽車駕照是審核通過機車就可以使用
											WHEN C.MotorDriver_1=-1 AND ISNULL(C.CarDriver_1,0)=2 THEN ''
											WHEN C.MotorDriver_1=-1 THEN '審核不通過，請重新提交資料'
											ELSE '' END
				--20201204 ADD BY ADAM REASON.改為線上統計
				,NormalRentCount	= ISNULL((SELECT SUM(Total) FROM #OrderProjCount WHERE ProjType=0),0)
				,AnyRentCount		= ISNULL((SELECT SUM(Total) FROM #OrderProjCount WHERE ProjType=3),0)
				,MotorRentCount		= ISNULL((SELECT SUM(Total) FROM #OrderProjCount WHERE ProjType=4),0)
				,TotalRentCount		= ISNULL((SELECT SUM(Total) FROM #OrderProjCount),0)
				,Score				= ISNULL(IIF(D.SCORE < 0, 0, D.Score),100)	-- 20210702 UPD BY YEH REASON:積分<0顯示0
				,BlockFlag			= CASE WHEN ISNULL(D.ISBLOCK,0) = 0 THEN 0
										WHEN ISNULL(D.ISBLOCK,0) = 1 AND ISNULL(D.BLOCK_CNT,0) < 3 THEN 1
										WHEN ISNULL(D.ISBLOCK,0) = 1 AND ISNULL(D.BLOCK_CNT,0) >= 3 THEN 2 END
				,BLOCK_EDATE		= ISNULL(CONVERT(varchar, D.BLOCK_EDATE, 111),'')
				,CMKStatus			= ISNULL((SELECT 'N' FROM #CMKDef WHERE VerType='Hims' AND Version=F.Version),'Y')	-- 20210811 UPD BY YEH REASON:增加會員條款狀態
				,IsShowBuy			= CASE WHEN ISNULL(D.Score,100) >= 60 THEN 'Y' ELSE 'N' END	-- 20210910 UPD BY YEH REASON:增加是否顯示購買牌卡
				,HasNoticeMsg		= CASE WHEN R.IDNO IS NULL THEN @HasNoticeMsg 
										   WHEN @NowTime < R.NextTime AND @HasNoticeMsg='N' THEN 'N' 
										   WHEN @NowTime < R.NextTime AND @HasNoticeMsg='Y' AND R.CHKTime < @NOTIFYTIME THEN 'Y'
										   ELSE 'N' END	--20210917 ADD BY ADAM REASON.是否有推播判斷
				,AuthStatus			= ISNULL((SELECT 'N' FROM #CMKDef WHERE VerType='Auth' AND Version=F.Version),'Y')	-- 20211105 UPD BY YEH REASON:增加預授權條款狀態
			FROM TB_MemberData A WITH(NOLOCK)
			LEFT JOIN TB_BookingStatusOfUser B WITH(NOLOCK) ON A.MEMIDNO=B.IDNO
			LEFT JOIN TB_Credentials C WITH(NOLOCK) ON A.MEMIDNO=C.IDNO
			LEFT JOIN TB_MemberScoreMain D WITH(NOLOCK) ON D.MEMIDNO=A.MEMIDNO
			LEFT JOIN TB_MemberDataOfAutdit E WITH(NOLOCK) ON E.MEMIDNO=A.MEMIDNO
			LEFT JOIN TB_MemberCMK F WITH(NOLOCK) ON F.MEMIDNO=A.MEMIDNO
			LEFT JOIN TB_NoticeRLog R WITH(NOLOCK) ON R.IDNO=A.MEMIDNO
			WHERE A.MEMIDNO=@IDNO;
		END

		DROP TABLE IF EXISTS #OrderProjCount;
		DROP TABLE IF EXISTS #CMKDef;
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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMemberStatus';
GO