/****** Object:  StoredProcedure [dbo].[usp_BE_GetAuditList]    Script Date: 2021/7/7 下午 01:41:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****************************************************************
** Name: [dbo].[usp_BE_BookingStart]
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
** EXEC @Error=[dbo].[usp_BE_BookingStart]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
** SELECT @Error,@ErrorCode ,@ErrorMsg ,@SQLExceptionCode ,@SQLExceptionMsg;
**------------
** Auth:Eric 
** Date:2020/10/29 下午 05:23:56 
**
*****************************************************************
** Change History
*****************************************************************
** Date:     |   Author:  |          Description:
** ----------|------------| ------------------------------------
** 2020/10/29 下午 05:23:56    |  Eric|          First Release
2021/6/2 唐瑋祁改
2021/6/30 唐瑋祁改
**			 |			  |
EXEC usp_BE_GetAuditList '-1','-1','','','-1','','F126142705','','' --條件全部+ID
EXEC usp_BE_GetAuditList '-1','-1','2021-03-03 00:00:00','2021-03-03 23:59:59','-1','','','','' --條件全部+日期
EXEC usp_BE_GetAuditList '1','0','2021-03-24 00:00:00','2021-03-24 23:59:59','-1','','','','Y' --條件全部+日期
EXEC usp_BE_GetAuditList '1','0','2021-03-25 00:00:00','2021-03-25 23:59:59','-1','','','','Y' --條件全部+日期
EXEC usp_BE_GetAuditList '1','0','2021-03-24 00:00:00','2021-03-25 23:59:59','-1','','','','Y' --條件全部+日期
EXEC usp_BE_GetAuditList '-1','-1','2021-03-03 00:00:00','2021-03-03 23:59:59','-1','','','5,','' --條件全部+日期+尾數5

*****************************************************************/
ALTER PROCEDURE [dbo].[usp_BE_GetAuditList]
	@AuditMode              INT					--1:申請加入 0:變更身分
	,@HasAudit              INT					--0:未處理 1:已處理(未通過)  2:已處理(已通過)
	,@StartDate             VARCHAR(20)			--
	,@EndDate               VARCHAR(20)			--
	,@AuditReuslt           INT						--審核結果 1:未通過 2:已通過
	,@UserName              NVARCHAR(60)			--客戶姓名
	,@IDNO                  VARCHAR(10)            --身分證字號
	,@IDNOSuff              VARCHAR(100)			--身分證尾數 逗號分隔
	,@MEMRFNBR				VARCHAR(20) --20210630唐加
	,@AuditError            VARCHAR(1)				--審核資格 Y:正常 N:異常 空白:全部
AS

CREATE TABLE #TEMP
(IDNOSUFF VARCHAR(1))

DECLARE @POS INT
DECLARE @STR VARCHAR(8000)

SET @POS = 1	--初值要讓迴圈開始
SET @STR=@IDNOSuff

WHILE @POS <>0
	BEGIN
		SET @POS=CHARINDEX (',' , @STR) 

		IF @POS <>0
			BEGIN
				INSERT INTO #TEMP(IDNOSUFF)
				VALUES(SUBSTRING(@STR,1,@POS-1))
				SET @STR=SUBSTRING(@STR,@POS+1,8000)
			END
	END
	
CREATE TABLE #CUSTLIST
(
	ApplyDate DATETIME
	,ModifyDate DATETIME
	,MEMCNAME NVARCHAR(60)
	,MEMIDNO VARCHAR(20)
	,SEX VARCHAR(10)
	,AuditKind VARCHAR(10)
	,HasAudit VARCHAR(10)
	,IsNew VARCHAR(10)
	,IDNOSUFF VARCHAR(1)
	,MEMBIRTH DATETIME
	,MEMADDR NVARCHAR(250)
	,MEMEMAIL VARCHAR(200)
	,isBlock VARCHAR(1)
)

IF NOT EXISTS (SELECT * FROM #TEMP)
BEGIN
	INSERT INTO #CUSTLIST
	 SELECT 
	  ISNULL(MemberData.APPLYDT,MemberData.A_SYSDT) AS ApplyDate,
	  ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) AS ModifyDate,
	  --MemberData.MEMCNAME,--20210128唐註解
	  MEMCNAME=CASE WHEN MemberData.[MEMCNAME]='' THEN Autdit.[MEMCNAME] ELSE MemberData.[MEMCNAME] END,--20210128唐加
	  MemberData.MEMIDNO,
	  CASE --WHEN SUBSTRING(MemberData.MEMIDNO,2,1)='A' OR SUBSTRING(MemberData.MEMIDNO,2,1)='C' THEN '1'
		   --WHEN SUBSTRING(MemberData.MEMIDNO,2,1)='B' OR SUBSTRING(MemberData.MEMIDNO,2,1)='D' THEN '2'
		   WHEN SUBSTRING(MemberData.MEMIDNO,2,1)='A' OR SUBSTRING(MemberData.MEMIDNO,2,1)='C' OR SUBSTRING(MemberData.MEMIDNO,2,1)='8' THEN '1' --20210218唐加新式居留證8男9女
		   WHEN SUBSTRING(MemberData.MEMIDNO,2,1)='B' OR SUBSTRING(MemberData.MEMIDNO,2,1)='D' OR SUBSTRING(MemberData.MEMIDNO,2,1)='9' THEN '2' --20210218唐加新式居留證8男9女
		   ELSE SUBSTRING(MemberData.MEMIDNO,2,1) END AS SEX,
	  AuditKind=ISNULL(Autdit.AuditKind,1),
	  HasAudit=CASE WHEN MemberData.IrFlag=0 THEN -1 WHEN Autdit.HasAudit IS NULL AND MemberData.Audit=1 THEN 2 ELSE ISNULL(Autdit.HasAudit,0) END,
	  IsNew=ISNULL(Autdit.IsNew,0),
	  SUBSTRING(MemberData.MEMIDNO,10,1) AS IDNOSUFF,
	  --MemberData.MEMBIRTH,--20210128唐註解
	  [MEMBIRTH]=CASE WHEN MemberData.[MEMBIRTH]='' OR MemberData.[MEMBIRTH] IS NULL THEN Autdit.[MEMBIRTH] ELSE MemberData.[MEMBIRTH] END,--20210128唐加
	  --MemberData.MEMADDR,--20210128唐註解
	  [MEMADDR]=CASE WHEN MemberData.[MEMADDR]='' THEN Autdit.[MEMADDR] ELSE MemberData.[MEMADDR] END,--20210128唐加
	  MemberData.MEMEMAIL
	  ,isBlock=CASE WHEN Block.MEMIDNO IS NULL THEN 'N' ELSE 'Y' END
  FROM [TB_MemberData] MemberData WITH(NOLOCK)
  LEFT JOIN [TB_MemberDataOfAutdit] Autdit WITH(NOLOCK) ON MemberData.MEMIDNO=Autdit.MEMIDNO
  LEFT JOIN [TB_MemberDataBlock] Block WITH(NOLOCK) ON MemberData.MEMIDNO=Block.MEMIDNO AND CONVERT(CHAR(8),DATEADD(HOUR,8,GETDATE()),112) BETWEEN STADT AND ENDDT --20210302唐加區間判斷
  WHERE MemberData.MEMIDNO=CASE WHEN @IDNO='' THEN MemberData.MEMIDNO ELSE @IDNO END
		AND MemberData.MEMCNAME LIKE '%'+@UserName+'%'
		AND CASE WHEN MemberData.IrFlag=0 THEN -2  
				 WHEN Autdit.HasAudit IS NULL AND MemberData.Audit=1 THEN 2 
				 ELSE ISNULL(HasAudit,0) END=
			CASE WHEN @HasAudit=-1 THEN (CASE WHEN MemberData.IrFlag=0 THEN -2 WHEN Autdit.HasAudit IS NULL AND MemberData.Audit=1 THEN 2 ELSE ISNULL(HasAudit,0) END)
				 ELSE @HasAudit END
		-- AND HasAudit = case when @HasAudit=-1 THEN HasAudit else @HasAudit END--20210113唐加，前端塞選不會正確顯示對應數字
		AND ISNULL(IsNew,0)=CASE WHEN @AuditMode=-1 THEN ISNULL(IsNew,0) ELSE @AuditMode END
		--AND CASE WHEN IsNew=1 THEN ISNULL(Autdit.MKTime,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END --IsNew=1是新會員
		--	between CASE WHEN @StartDate='' THEN (CASE WHEN IsNew=1 THEN ISNULL(Autdit.MKTime,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END) ELSE @StartDate END 
		--	AND CASE WHEN @EndDate='' THEN (CASE WHEN IsNew=1 THEN ISNULL(Autdit.MKTime,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END) ELSE @EndDate END
		AND CASE WHEN IsNew=1 THEN ISNULL(MemberData.APPLYDT,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END --IsNew=1是新會員
			between CASE WHEN @StartDate='' THEN (CASE WHEN IsNew=1 THEN ISNULL(MemberData.APPLYDT,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END) ELSE @StartDate END 
			AND CASE WHEN @EndDate='' THEN (CASE WHEN IsNew=1 THEN ISNULL(MemberData.APPLYDT,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END) ELSE @EndDate END 
		AND MEMRFNBR=CASE @MEMRFNBR WHEN '' THEN MEMRFNBR ELSE @MEMRFNBR END --20210630唐加
END
ELSE
BEGIN
	INSERT INTO #CUSTLIST
	 SELECT 
	  ISNULL(MemberData.APPLYDT,MemberData.A_SYSDT) AS ApplyDate,
	  ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) AS ModifyDate,
	  --MemberData.MEMCNAME,--20210128唐註解
	  MEMCNAME=CASE WHEN MemberData.[MEMCNAME]='' THEN Autdit.[MEMCNAME] ELSE MemberData.[MEMCNAME] END,--20210128唐加
	  MemberData.MEMIDNO,
	  CASE --WHEN SUBSTRING(MemberData.MEMIDNO,2,1)='A' OR SUBSTRING(MemberData.MEMIDNO,2,1)='C' THEN '1'
		   --WHEN SUBSTRING(MemberData.MEMIDNO,2,1)='B' OR SUBSTRING(MemberData.MEMIDNO,2,1)='D' THEN '2'
		   WHEN SUBSTRING(MemberData.MEMIDNO,2,1)='A' OR SUBSTRING(MemberData.MEMIDNO,2,1)='C' OR SUBSTRING(MemberData.MEMIDNO,2,1)='8' THEN '1'--20210218唐加新式居留證8男9女
		   WHEN SUBSTRING(MemberData.MEMIDNO,2,1)='B' OR SUBSTRING(MemberData.MEMIDNO,2,1)='D' OR SUBSTRING(MemberData.MEMIDNO,2,1)='9' THEN '2' --20210218唐加新式居留證8男9女
		   ELSE SUBSTRING(MemberData.MEMIDNO,2,1) END AS SEX,
	  AuditKind=ISNULL(Autdit.AuditKind,1),
	  HasAudit=CASE WHEN MemberData.IrFlag=0 THEN -1 ELSE ISNULL(Autdit.HasAudit,0)END,
	  IsNew=ISNULL(Autdit.IsNew,0),
	  SUBSTRING(MemberData.MEMIDNO,10,1) AS IDNOSUFF,
	  --MemberData.MEMBIRTH,--20210128唐註解
	  [MEMBIRTH]=CASE WHEN MemberData.[MEMBIRTH]='' OR MemberData.[MEMBIRTH] IS NULL THEN Autdit.[MEMBIRTH] ELSE MemberData.[MEMBIRTH] END,--20210128唐加
	  --MemberData.MEMADDR,--20210128唐註解
	  [MEMADDR]=CASE WHEN MemberData.[MEMADDR]='' THEN Autdit.[MEMADDR] ELSE MemberData.[MEMADDR] END,--20210128唐加
	  MemberData.MEMEMAIL
	  ,isBlock=CASE WHEN Block.MEMIDNO IS NULL THEN 'N' ELSE 'Y' END
  FROM [TB_MemberData] MemberData WITH(NOLOCK)
  LEFT JOIN [TB_MemberDataOfAutdit] Autdit WITH(NOLOCK) ON MemberData.MEMIDNO=Autdit.MEMIDNO
  LEFT JOIN [TB_MemberDataBlock] Block WITH(NOLOCK) ON MemberData.MEMIDNO=Block.MEMIDNO AND CONVERT(CHAR(8),DATEADD(HOUR,8,GETDATE()),112) BETWEEN STADT AND ENDDT --20210302唐加區間判斷
  WHERE MemberData.MEMIDNO=CASE WHEN @IDNO='' THEN MemberData.MEMIDNO ELSE @IDNO END
		AND MemberData.MEMCNAME LIKE '%'+@UserName+'%'
		AND CASE WHEN MemberData.IrFlag=0 THEN -1 --IrFlag是操作步驟，0密碼設定完、-1手機驗證完、1表示基本資料以後的動作
				 ELSE ISNULL(HasAudit,0) END=
		    CASE WHEN @HasAudit=-1 THEN (CASE WHEN MemberData.IrFlag=0 THEN -1 ELSE ISNULL(HasAudit,0) END) 
			     ELSE @HasAudit END
		--AND HasAudit = case when @HasAudit=-1 THEN HasAudit else @HasAudit END--20210113唐加，前端塞選不會正確顯示對應數字
		AND ISNULL(IsNew,0)=CASE WHEN @AuditMode=-1 THEN ISNULL(IsNew,0) ELSE @AuditMode END
		--AND CASE WHEN IsNew=1 THEN ISNULL(Autdit.MKTime,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END --IsNew=1是新會員
		--	between CASE WHEN @StartDate='' THEN (CASE WHEN IsNew=1 THEN ISNULL(Autdit.MKTime,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END) ELSE @StartDate END 
		--	AND CASE WHEN @EndDate='' THEN (CASE WHEN IsNew=1 THEN ISNULL(Autdit.MKTime,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END) ELSE @EndDate END
		AND CASE WHEN IsNew=1 THEN ISNULL(MemberData.APPLYDT,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END --IsNew=1是新會員
			between CASE WHEN @StartDate='' THEN (CASE WHEN IsNew=1 THEN ISNULL(MemberData.APPLYDT,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END) ELSE @StartDate END 
			AND CASE WHEN @EndDate='' THEN (CASE WHEN IsNew=1 THEN ISNULL(MemberData.APPLYDT,MemberData.A_SYSDT) ELSE ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) END) ELSE @EndDate END 
		AND SUBSTRING(MemberData.MEMIDNO,10,1) IN (SELECT IDNOSUFF FROM #TEMP)
		AND MEMRFNBR=CASE @MEMRFNBR WHEN '' THEN MEMRFNBR ELSE @MEMRFNBR END --20210630唐加
END

--select * from #CUSTLIST where MEMIDNO='F126142705'

/*
--20210128唐註解
SELECT A.IDNO,PICCNT=COUNT(*)
INTO  #TB_tmpCrentialsPIC
FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST B ON A.IDNO=B.MEMIDNO
WHERE A.CrentialsType IN (1,2,3,4,5,6) GROUP BY IDNO

SELECT A.IDNO,PICCNT=COUNT(*)
INTO  #TB_AuditCredentials
FROM TB_AuditCredentials A WITH(NOLOCK) 
JOIN #CUSTLIST B ON A.IDNO=B.MEMIDNO
WHERE A.CrentialsType IN (8) GROUP BY IDNO

SELECT A.IDNO,PICCNT=COUNT(*)
INTO  #TB_CrentialsPIC
FROM TB_CrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST B ON A.IDNO=B.MEMIDNO
WHERE A.CrentialsType IN (1,2,3,4,5,6) GROUP BY IDNO
*/
SELECT A.IDNO,PICCNT=COUNT(*)
INTO  #TB_tmpCrentialsPIC1
FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST B ON A.IDNO=B.MEMIDNO
WHERE A.CrentialsType IN (8) GROUP BY IDNO

SELECT A.IDNO,PICCNT=COUNT(*)
INTO  #TB_AuditCredentials1
FROM TB_AuditCredentials A WITH(NOLOCK) 
JOIN #CUSTLIST B ON A.IDNO=B.MEMIDNO
WHERE A.CrentialsType IN (1,2,3,4,5,6) GROUP BY IDNO

/*
--20210227 ADD BY ADAM REASON.補上照片上傳數量判斷
SELECT A.IDNO,PICCNT=COUNT(*)
INTO #TB_tmpCrentialsPIC2		--汽車
FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO --20210315唐加
WHERE A.CrentialsType IN (1,2,3,4,7,11) 
and c.CarDriver_1=1 and c.CarDriver_2=1 and c.ID_1=1 and c.ID_2=1 and c.Self_1=1 and c.Signture=1 --20210315唐加
GROUP BY A.IDNO

--20210227 ADD BY ADAM REASON.補上照片上傳數量判斷
SELECT A.IDNO,PICCNT=COUNT(*)
INTO #TB_tmpCrentialsPIC3		--機車
FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO --20210315唐加
WHERE A.CrentialsType IN (1,2,5,6,7,11) 
and c.MotorDriver_1=1 and c.MotorDriver_2=1 and c.ID_1=1 and c.ID_2=1 and c.Self_1=1 and c.Signture=1 --20210315唐加
GROUP BY A.IDNO

--20210308 ADD BY 唐 REASON.幫亞當哥多判斷CrentialsPIC
SELECT A.IDNO,PICCNT=COUNT(*)
INTO #TB_tmpCrentialsPIC4		--汽車
FROM TB_CrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO --20210319唐加
WHERE A.CrentialsType IN (1,2,3,4,7,11) 
and c.CarDriver_1=2 and c.CarDriver_2=2 and c.ID_1=2 and c.ID_2=2 and c.Self_1=2 and c.Signture=2 --20210319唐加
GROUP BY A.IDNO

--20210308 ADD BY 唐 REASON.幫亞當哥多判斷CrentialsPIC
SELECT A.IDNO,PICCNT=COUNT(*)
INTO #TB_tmpCrentialsPIC5		--機車
FROM TB_CrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO --20210319唐加
WHERE A.CrentialsType IN (1,2,5,6,7,11) 
and c.MotorDriver_1=2 and c.MotorDriver_2=2 and c.ID_1=2 and c.ID_2=2 and c.Self_1=2 and c.Signture=2 --20210319唐加
GROUP BY A.IDNO
*/
--20210322唐大改start
--20210521唐拿掉top 1000
SELECT * INTO #CUSTLIST_500 FROM #CUSTLIST WITH(NOLOCK) ORDER BY CASE WHEN IsNew=1 THEN ApplyDate ELSE ModifyDate END DESC--因為以下code耗費太多時間，所以在這邊就先top500，而不是最後select才做

--SELECT 'TOP500',* from #CUSTLIST_500

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_PIC_1 FROM TB_CrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=1 and c.ID_1=2 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_PIC_2 FROM TB_CrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=2 and c.ID_2=2 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_PIC_7 FROM TB_CrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=7 and c.Self_1=2 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_PIC_11 FROM TB_CrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=11 and c.Signture=2 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_tmpPIC_1 FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=1 and c.ID_1=1 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_tmpPIC_2 FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=2 and c.ID_2=1 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_tmpPIC_7 FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=7 and c.Self_1=1 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_tmpPIC_11 FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=11 and c.Signture=1 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_PIC_3 FROM TB_CrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=3 and c.CarDriver_1=2 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_tmpPIC_3 FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=3 and c.CarDriver_1=1 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_PIC_4 FROM TB_CrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=4 and c.CarDriver_2=2 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_tmpPIC_4 FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=4 and c.CarDriver_2=1 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_PIC_5 FROM TB_CrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=5 and c.MotorDriver_1=2 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_tmpPIC_5 FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=5 and c.MotorDriver_1=1 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_PIC_6 FROM TB_CrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=6 and c.MotorDriver_2=2 GROUP BY A.IDNO 

SELECT A.IDNO,PICCNT=COUNT(*) INTO #TB_tmpPIC_6 FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
JOIN #CUSTLIST_500 B ON A.IDNO=B.MEMIDNO
join TB_Credentials c on A.IDNO=c.IDNO
WHERE A.CrentialsType=6 and c.MotorDriver_2=1 GROUP BY A.IDNO 
--20210322唐大改end

/*
--20210128唐加，判斷證件照、駕照、自拍照都有才屬於審核資格正常start
--SELECT A.IDNO,A.CrentialsType
--INTO #TB_CrentialsPIC_20210128
--FROM TB_CrentialsPIC A WITH(NOLOCK) 
--JOIN #CUSTLIST B ON A.IDNO=B.MEMIDNO
--WHERE A.CrentialsType IN (1,2,3,4,5,6,7)

--SELECT A.IDNO,A.CrentialsType
--INTO #TB_CrentialsPIC_20210128_2
--FROM TB_tmpCrentialsPIC A WITH(NOLOCK) 
--JOIN #CUSTLIST B ON A.IDNO=B.MEMIDNO
--WHERE A.CrentialsType IN (1,2,3,4,5,6,7)
--SELECT IDNO,PICCNT=COUNT(*) INTO #SSS FROM (SELECT * FROM #TB_CrentialsPIC_20210128 UNION SELECT * FROM #TB_CrentialsPIC_20210128_2) B GROUP BY IDNO
--20210128唐加end
*/

--20210226唐加、20210319改!! 若TB_Credentials是1就看tmp，是2就看PIC，舒婷想到的好方法我覺得不錯
declare @pic_num int --4張必備的
set @pic_num = 0
declare @pic_num2 int --汽車2張
set @pic_num2 = 0
declare @pic_num3 int --機車2張
set @pic_num3 = 0
IF EXISTS(SELECT a.IDNO FROM TB_CrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND a.CrentialsType=1 and b.ID_1=2
		  UNION 
		  SELECT a.IDNO FROM TB_tmpCrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=1 and b.ID_1=1)
BEGIN
	SET @pic_num+=1
END
IF EXISTS(SELECT a.IDNO FROM TB_CrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=2 and b.ID_2=2
		  UNION 
		  SELECT a.IDNO FROM TB_tmpCrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=2 and b.ID_2=1)
BEGIN
	SET @pic_num+=1
END
IF EXISTS(SELECT a.IDNO FROM TB_CrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=7 and b.Self_1=2
		  UNION 
		  SELECT a.IDNO FROM TB_tmpCrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=7 and b.Self_1=1)
BEGIN
	SET @pic_num+=1
END
IF EXISTS(SELECT a.IDNO FROM TB_CrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=11 and b.Signture=2
		  UNION 
		  SELECT a.IDNO FROM TB_tmpCrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=11 and b.Signture=1)
BEGIN
	SET @pic_num+=1
END
IF EXISTS(SELECT a.IDNO FROM TB_CrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=3 and b.CarDriver_1=2
		  UNION 
		  SELECT a.IDNO FROM TB_tmpCrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=3 and b.CarDriver_1=1)
BEGIN
	SET @pic_num2+=1
END
IF EXISTS(SELECT a.IDNO FROM TB_CrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=4 and b.CarDriver_2=2
		  UNION 
		  SELECT a.IDNO FROM TB_tmpCrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=4 and b.CarDriver_2=1)
BEGIN
	SET @pic_num2+=1
END
IF EXISTS(SELECT a.IDNO FROM TB_CrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=5 and b.MotorDriver_1=2
		  UNION 
		  SELECT a.IDNO FROM TB_tmpCrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=5 and b.MotorDriver_1=1)
BEGIN
	SET @pic_num3+=1
END
IF EXISTS(SELECT a.IDNO FROM TB_CrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=6 and b.MotorDriver_2=2
		  UNION 
		  SELECT a.IDNO FROM TB_tmpCrentialsPIC a join TB_Credentials b on a.IDNO=b.IDNO WHERE a.IDNO=@IDNO AND CrentialsType=6 and b.MotorDriver_2=1)
BEGIN
	SET @pic_num3+=1
END
--20210226唐加end

--select
--(SELECT @pic_num) as '4張必備的',
--(SELECT @pic_num2) as '汽車2張',
--(SELECT @pic_num3) as '機車2張'

SELECT A.*,
	  --20201125 ADD BY JERRY 增加簡易會員檢核
	  --20210121唐拿掉email條件，天0說不要的
	  --20210128唐改A.IsNew=1這段的判斷式
	  MEMO=CASE WHEN (A.MEMCNAME='' OR A.MEMBIRTH IS NULL OR A.MEMADDR='') THEN '基本資料未完成,' 
	       ELSE '' END+
		   --CASE WHEN A.IsNew=1 THEN 
		   -- 	CASE WHEN ISNULL(PIC.PICCNT,0)+ISNULL(PIC1.PICCNT,0)+ISNULL(PIC2.PICCNT,0)<4 THEN '照片不完整,' ELSE '' END
		   --ELSE '' END+
		   --20210129這是for個人的寫法，整批會錯
		   --CASE WHEN A.IsNew=1 THEN 
		   --	CASE WHEN (((SELECT COUNT(*) FROM #SSS WHERE CrentialsType IN(1,2,3,4,7))=5) OR ((SELECT COUNT(*) FROM #SSS WHERE CrentialsType IN(1,2,5,6,7))=5)) THEN '照片ok' ELSE '照片不完整,' END
		   --ELSE '' END+
		   -- CASE WHEN A.IsNew=1 THEN
		   --CASE WHEN ISNULL(PIC5.PICCNT,0)>=5 THEN '' ELSE '照片不完整,' END
		   --20210226唐改
		   --CASE WHEN @IDNO='' THEN --20210227 ADD BY ADAM REASON.非單一會員審核跑這邊判斷 --20210319拿掉A.IsNew=1的限制，因為舒婷說資料變更也要判斷
				--CASE WHEN ISNULL(PICCAR.PICCNT+PICCAR2.PICCNT,0) < 6 AND ISNULL(PICMOTO.PICCNT+PICMOTO2.PICCNT,0) < 6 THEN '照片不完整,' ELSE '' END
		   --ELSE '' END +
		   --20210322唐改
		   CASE WHEN @IDNO='' THEN 
				CASE WHEN (
						  (ISNULL(PIC_1.PICCNT,0)+ISNULL(tmpPIC_1.PICCNT,0) +ISNULL(PIC_2.PICCNT,0)+ISNULL(tmpPIC_2.PICCNT,0)+ ISNULL(PIC_7.PICCNT,0)+ISNULL(tmpPIC_7.PICCNT,0) +ISNULL(PIC_11.PICCNT,0)+ISNULL(tmpPIC_11.PICCNT,0) <> 4)
						  or  
						  ((ISNULL(PIC_3.PICCNT,0)+ISNULL(tmpPIC_3.PICCNT,0)+ ISNULL(PIC_4.PICCNT,0)+ISNULL(tmpPIC_4.PICCNT,0) <> 2) and (ISNULL(PIC_5.PICCNT,0)+ISNULL(tmpPIC_5.PICCNT,0) + ISNULL(PIC_6.PICCNT,0)+ISNULL(tmpPIC_6.PICCNT,0) <> 2))
						  )				  
						  THEN '照片不完整,' ELSE '' END
		   ELSE '' END +
		   CASE WHEN @IDNO <>'' THEN --20210227 ADD BY ADAM  REASON.這段是跑單一會員審核 --20210319拿掉A.IsNew=1的限制，因為舒婷說資料變更也要判斷
 				CASE WHEN (@pic_num<>4 OR (@pic_num2<>2 AND @pic_num3<>2)) THEN '照片不完整,' ELSE '' END
		   ELSE '' END+
		   CASE WHEN A.MEMBIRTH IS NOT NULL THEN
				CASE WHEN GETDATE() BETWEEN DATEADD(YEAR,18,A.MEMBIRTH) AND DATEADD(YEAR,20,A.MEMBIRTH) AND ISNULL((PIC3.PICCNT),0)+ISNULL((PIC4.PICCNT),0)=0 THEN '法定代理人同意書未上傳' ELSE '' END
		   ELSE '' END
INTO #CUSTLIST1
FROM #CUSTLIST_500 A
--LEFT JOIN #TB_tmpCrentialsPIC AS PIC ON A.MEMIDNO=PIC.IDNO--20210128唐註解
--LEFT JOIN #TB_AuditCredentials AS PIC1 ON A.MEMIDNO=PIC1.IDNO--20210128唐註解
--LEFT JOIN #TB_CrentialsPIC AS PIC2 ON A.MEMIDNO=PIC2.IDNO--20210128唐註解
LEFT JOIN #TB_tmpCrentialsPIC1 AS PIC3 ON A.MEMIDNO=PIC3.IDNO
LEFT JOIN #TB_AuditCredentials1 AS PIC4 ON A.MEMIDNO=PIC4.IDNO
--LEFT JOIN #SSS AS PIC5 ON A.MEMIDNO=PIC5.IDNO--20210128唐加
--20210227 ADD BY ADAM REASON.補上照片上傳數量判斷
--LEFT JOIN #TB_tmpCrentialsPIC2 AS PICCAR ON A.MEMIDNO=PICCAR.IDNO
--LEFT JOIN #TB_tmpCrentialsPIC3 AS PICMOTO ON A.MEMIDNO=PICMOTO.IDNO
----20210308 ADD BY 唐
--LEFT JOIN #TB_tmpCrentialsPIC4 AS PICCAR2 ON A.MEMIDNO=PICCAR2.IDNO
--LEFT JOIN #TB_tmpCrentialsPIC5 AS PICMOTO2 ON A.MEMIDNO=PICMOTO2.IDNO
--20210322 add by 唐
LEFT JOIN #TB_PIC_1 as PIC_1 ON A.MEMIDNO=PIC_1.IDNO
LEFT JOIN #TB_PIC_2 as PIC_2 ON A.MEMIDNO=PIC_2.IDNO
LEFT JOIN #TB_PIC_7 as PIC_7 ON A.MEMIDNO=PIC_7.IDNO
LEFT JOIN #TB_PIC_11 as PIC_11 ON A.MEMIDNO=PIC_11.IDNO
LEFT JOIN #TB_PIC_3 as PIC_3 ON A.MEMIDNO=PIC_3.IDNO
LEFT JOIN #TB_PIC_4 as PIC_4 ON A.MEMIDNO=PIC_4.IDNO
LEFT JOIN #TB_PIC_5 as PIC_5 ON A.MEMIDNO=PIC_5.IDNO
LEFT JOIN #TB_PIC_6 as PIC_6 ON A.MEMIDNO=PIC_6.IDNO
LEFT JOIN #TB_tmpPIC_1 as tmpPIC_1 ON A.MEMIDNO=tmpPIC_1.IDNO
LEFT JOIN #TB_tmpPIC_2 as tmpPIC_2 ON A.MEMIDNO=tmpPIC_2.IDNO
LEFT JOIN #TB_tmpPIC_7 as tmpPIC_7 ON A.MEMIDNO=tmpPIC_7.IDNO
LEFT JOIN #TB_tmpPIC_11 as tmpPIC_11 ON A.MEMIDNO=tmpPIC_11.IDNO
LEFT JOIN #TB_tmpPIC_3 as tmpPIC_3 ON A.MEMIDNO=tmpPIC_3.IDNO
LEFT JOIN #TB_tmpPIC_4 as tmpPIC_4 ON A.MEMIDNO=tmpPIC_4.IDNO
LEFT JOIN #TB_tmpPIC_5 as tmpPIC_5 ON A.MEMIDNO=tmpPIC_5.IDNO
LEFT JOIN #TB_tmpPIC_6 as tmpPIC_6 ON A.MEMIDNO=tmpPIC_6.IDNO

DROP TABLE #CUSTLIST
DROP TABLE #CUSTLIST_500

--SELECT * FROM #CUSTLIST1 where MEMIDNO='F126142705'
--SELECT top 500 * FROM #CUSTLIST1 
--WHERE CASE WHEN MEMO<>'' THEN 'N' ELSE 'Y' END = 
--		CASE WHEN @AuditError='' THEN CASE WHEN MEMO<>'' THEN 'N' ELSE 'Y' END 
--		ELSE @AuditError 
--		END
--ORDER BY NEWID(),CASE WHEN IsNew=1 THEN ApplyDate ELSE ModifyDate END DESC--20210304唐加隨機排序
SELECT * FROM #CUSTLIST1 
WHERE CASE WHEN MEMO<>'' THEN 'N' ELSE 'Y' END = 
		CASE WHEN @AuditError='' THEN CASE WHEN MEMO<>'' THEN 'N' ELSE 'Y' END 
		ELSE @AuditError 
		END
ORDER BY NEWID(),CASE WHEN IsNew=1 THEN ApplyDate ELSE ModifyDate END DESC--20210304唐加隨機排序

DROP TABLE #CUSTLIST1
DROP TABLE #TB_tmpCrentialsPIC1--20210128唐加
DROP TABLE #TB_AuditCredentials1--20210128唐加
--DROP TABLE #TB_CrentialsPIC_20210128--20210128唐加
--DROP TABLE #TB_CrentialsPIC_20210128_2--20210128唐加

--20210227 ADD BY ADAM REASON.補上照片上傳數量判斷
--DROP TABLE #TB_tmpCrentialsPIC2
--DROP TABLE #TB_tmpCrentialsPIC3
--20210308 ADD BY 唐
--DROP TABLE #TB_tmpCrentialsPIC4
--DROP TABLE #TB_tmpCrentialsPIC5

DROP TABLE #TB_PIC_1
DROP TABLE #TB_PIC_2
DROP TABLE #TB_PIC_7
DROP TABLE #TB_PIC_11
DROP TABLE #TB_PIC_3
DROP TABLE #TB_PIC_4
DROP TABLE #TB_PIC_5
DROP TABLE #TB_PIC_6
DROP TABLE #TB_tmpPIC_1
DROP TABLE #TB_tmpPIC_2
DROP TABLE #TB_tmpPIC_7
DROP TABLE #TB_tmpPIC_11
DROP TABLE #TB_tmpPIC_3
DROP TABLE #TB_tmpPIC_4
DROP TABLE #TB_tmpPIC_5
DROP TABLE #TB_tmpPIC_6
DROP TABLE #TEMP