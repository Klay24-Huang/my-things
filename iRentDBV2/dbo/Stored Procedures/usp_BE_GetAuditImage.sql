/****** Object:  StoredProcedure [dbo].[usp_BE_GetAuditImage_Tang]    Script Date: 2021/6/4 下午 02:06:07 ******/
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
**			 |			  |
EXEC usp_BE_GetAuditImage 'A124764780'
EXEC [usp_BE_GetAuditImage_Tang] 'F125189473'
*****************************************************************/
ALTER PROCEDURE [dbo].[usp_BE_GetAuditImage_Tang]
@IDNO  VARCHAR(10)          
AS	

SELECT * INTO #TB_CrentialsPIC from TB_CrentialsPIC WITH(NOLOCK) WHERE IDNO=@IDNO
SELECT * INTO #TB_tmpCrentialsPIC from TB_tmpCrentialsPIC WITH(NOLOCK) WHERE IDNO=@IDNO
SELECT * INTO #TB_Credentials from TB_Credentials WITH(NOLOCK) WHERE IDNO=@IDNO
SELECT * INTO #TB_AuditCrentialsReject from TB_AuditCrentialsReject WITH(NOLOCK) WHERE IDNO=@IDNO

--1 ID_1
SELECT IDNO = @IDNO
,CrentialsType = CASE A.ID_1 WHEN 1 THEN 1 WHEN 0 THEN 1 WHEN -1 THEN 1 ELSE '' END
,CrentialsFile = CASE A.ID_1 WHEN 1 THEN Replace(B.CrentialsFile,'FIYHFC01','FIYHI01') WHEN 0 THEN isnull(Replace(B.CrentialsFile,'FIYHFC01','FIYHI01'),'') WHEN -1 THEN ISNULL(C.CrentialsFile,ISNULL(B.CrentialsFile,'')) ELSE '' END 
,LSFLG = 0
,AlreadyType = CASE A.ID_1 WHEN 2 THEN 1 ELSE '' END
,AlreadyFile = CASE A.ID_1 WHEN 2 THEN ISNULL(REPLACE(C.CrentialsFile,'FIYHFC01','FIYHI01'),'') ELSE '' END
,IsNew = D.IsNew
,UPDTime = CASE A.ID_1 WHEN 0 THEN isnull(B.UPDTime,'') WHEN 1 THEN B.UPDTime WHEN 2 THEN C.UPDTime WHEN -1 THEN E.UPDTime END
,AuditResult = A.ID_1
,RejectReason = CASE A.ID_1 WHEN -1 THEN E.RejectReason ELSE '' END
FROM #TB_Credentials A
LEFT JOIN #TB_tmpCrentialsPIC B ON A.IDNO=B.IDNO AND B.CrentialsType=1
LEFT JOIN #TB_CrentialsPIC C ON A.IDNO=C.IDNO AND C.CrentialsType=1
LEFT JOIN TB_MemberDataOfAutdit D ON A.IDNO=D.MEMIDNO
LEFT JOIN #TB_AuditCrentialsReject E ON A.IDNO=E.IDNO AND E.CrentialsType=1
WHERE A.IDNO=@IDNO
UNION
--2 ID_2
SELECT IDNO = @IDNO
,CrentialsType = CASE A.ID_2 WHEN 1 THEN 2 WHEN 0 THEN 2 WHEN -1 THEN 2 ELSE '' END
,CrentialsFile = CASE A.ID_2 WHEN 1 THEN Replace(B.CrentialsFile,'FIYHFC01','FIYHI01') WHEN 0 THEN isnull(Replace(B.CrentialsFile,'FIYHFC01','FIYHI01'),'') WHEN -1 THEN ISNULL(C.CrentialsFile,ISNULL(B.CrentialsFile,'')) ELSE '' END 
,LSFLG = 0
,AlreadyType = CASE A.ID_2 WHEN 2 THEN 2 ELSE '' END
,AlreadyFile = CASE A.ID_2 WHEN 2 THEN ISNULL(REPLACE(C.CrentialsFile,'FIYHFC01','FIYHI01'),'') ELSE '' END
,IsNew = D.IsNew
,UPDTime = CASE A.ID_2 WHEN 0 THEN isnull(B.UPDTime,'') WHEN 1 THEN B.UPDTime WHEN 2 THEN C.UPDTime WHEN -1 THEN E.UPDTime END
,AuditResult = A.ID_2
,RejectReason = CASE A.ID_2 WHEN -1 THEN E.RejectReason ELSE '' END
FROM #TB_Credentials A
LEFT JOIN #TB_tmpCrentialsPIC B ON A.IDNO=B.IDNO AND B.CrentialsType=2
LEFT JOIN #TB_CrentialsPIC C ON A.IDNO=C.IDNO AND C.CrentialsType=2
LEFT JOIN TB_MemberDataOfAutdit D ON A.IDNO=D.MEMIDNO
LEFT JOIN #TB_AuditCrentialsReject E ON A.IDNO=E.IDNO AND E.CrentialsType=2
WHERE A.IDNO=@IDNO
UNION
--3 CarDriver_1
SELECT IDNO = @IDNO
,CrentialsType = CASE A.CarDriver_1 WHEN 1 THEN 3 WHEN 0 THEN 3 WHEN -1 THEN 3 ELSE '' END
,CrentialsFile = CASE A.CarDriver_1 WHEN 1 THEN Replace(B.CrentialsFile,'FIYHFC01','FIYHI01') WHEN 0 THEN isnull(Replace(B.CrentialsFile,'FIYHFC01','FIYHI01'),'') WHEN -1 THEN ISNULL(C.CrentialsFile,ISNULL(B.CrentialsFile,'')) ELSE '' END 
,LSFLG = 0
,AlreadyType = CASE A.CarDriver_1 WHEN 2 THEN 3 ELSE '' END
,AlreadyFile = CASE A.CarDriver_1 WHEN 2 THEN ISNULL(REPLACE(C.CrentialsFile,'FIYHFC01','FIYHI01'),'') ELSE '' END
,IsNew = D.IsNew
,UPDTime = CASE A.CarDriver_1 WHEN 0 THEN isnull(B.UPDTime,'') WHEN 1 THEN B.UPDTime WHEN 2 THEN C.UPDTime WHEN -1 THEN E.UPDTime END
,AuditResult = A.CarDriver_1
,RejectReason = CASE A.CarDriver_1 WHEN -1 THEN E.RejectReason ELSE '' END
FROM #TB_Credentials A
LEFT JOIN #TB_tmpCrentialsPIC B ON A.IDNO=B.IDNO AND B.CrentialsType=3
LEFT JOIN #TB_CrentialsPIC C ON A.IDNO=C.IDNO AND C.CrentialsType=3
LEFT JOIN TB_MemberDataOfAutdit D ON A.IDNO=D.MEMIDNO
LEFT JOIN #TB_AuditCrentialsReject E ON A.IDNO=E.IDNO AND E.CrentialsType=3
WHERE A.IDNO=@IDNO
UNION
--4 CarDriver_2
SELECT IDNO = @IDNO
,CrentialsType = CASE A.CarDriver_2 WHEN 1 THEN 4 WHEN 0 THEN 4 WHEN -1 THEN 4 ELSE '' END
,CrentialsFile = CASE A.CarDriver_2 WHEN 1 THEN Replace(B.CrentialsFile,'FIYHFC01','FIYHI01') WHEN 0 THEN isnull(Replace(B.CrentialsFile,'FIYHFC01','FIYHI01'),'') WHEN -1 THEN ISNULL(C.CrentialsFile,ISNULL(B.CrentialsFile,'')) ELSE '' END 
,LSFLG = 0
,AlreadyType = CASE A.CarDriver_2 WHEN 2 THEN 4 ELSE '' END
,AlreadyFile = CASE A.CarDriver_2 WHEN 2 THEN ISNULL(REPLACE(C.CrentialsFile,'FIYHFC01','FIYHI01'),'') ELSE '' END
,IsNew = D.IsNew
,UPDTime = CASE A.CarDriver_2 WHEN 0 THEN isnull(B.UPDTime,'') WHEN 1 THEN B.UPDTime WHEN 2 THEN C.UPDTime WHEN -1 THEN E.UPDTime END
,AuditResult = A.CarDriver_2
,RejectReason = CASE A.CarDriver_2 WHEN -1 THEN E.RejectReason ELSE '' END
FROM #TB_Credentials A
LEFT JOIN #TB_tmpCrentialsPIC B ON A.IDNO=B.IDNO AND B.CrentialsType=4
LEFT JOIN #TB_CrentialsPIC C ON A.IDNO=C.IDNO AND C.CrentialsType=4
LEFT JOIN TB_MemberDataOfAutdit D ON A.IDNO=D.MEMIDNO
LEFT JOIN #TB_AuditCrentialsReject E ON A.IDNO=E.IDNO AND E.CrentialsType=4
WHERE A.IDNO=@IDNO
UNION
--5 MotorDriver_1
SELECT IDNO = @IDNO
,CrentialsType = CASE A.MotorDriver_1 WHEN 1 THEN 5 WHEN 0 THEN 5 WHEN -1 THEN 5 ELSE '' END
,CrentialsFile = CASE A.MotorDriver_1 WHEN 1 THEN Replace(B.CrentialsFile,'FIYHFC01','FIYHI01') WHEN 0 THEN isnull(Replace(B.CrentialsFile,'FIYHFC01','FIYHI01'),'') WHEN -1 THEN ISNULL(C.CrentialsFile,ISNULL(B.CrentialsFile,'')) ELSE '' END 
,LSFLG = 0
,AlreadyType = CASE A.MotorDriver_1 WHEN 2 THEN 5 ELSE '' END
,AlreadyFile = CASE A.MotorDriver_1 WHEN 2 THEN ISNULL(REPLACE(C.CrentialsFile,'FIYHFC01','FIYHI01'),'') ELSE '' END
,IsNew = D.IsNew
,UPDTime = CASE A.MotorDriver_1 WHEN 0 THEN isnull(B.UPDTime,'') WHEN 1 THEN B.UPDTime WHEN 2 THEN C.UPDTime WHEN -1 THEN E.UPDTime END
,AuditResult = A.MotorDriver_1
,RejectReason = CASE A.MotorDriver_1 WHEN -1 THEN E.RejectReason ELSE '' END
FROM #TB_Credentials A
LEFT JOIN #TB_tmpCrentialsPIC B ON A.IDNO=B.IDNO AND B.CrentialsType=5
LEFT JOIN #TB_CrentialsPIC C ON A.IDNO=C.IDNO AND C.CrentialsType=5
LEFT JOIN TB_MemberDataOfAutdit D ON A.IDNO=D.MEMIDNO
LEFT JOIN #TB_AuditCrentialsReject E ON A.IDNO=E.IDNO AND E.CrentialsType=5
WHERE A.IDNO=@IDNO
UNION
--6 MotorDriver_2
SELECT IDNO = @IDNO
,CrentialsType = CASE A.MotorDriver_2 WHEN 1 THEN 6 WHEN 0 THEN 6 WHEN -1 THEN 6 ELSE '' END
,CrentialsFile = CASE A.MotorDriver_2 WHEN 1 THEN Replace(B.CrentialsFile,'FIYHFC01','FIYHI01') WHEN 0 THEN isnull(Replace(B.CrentialsFile,'FIYHFC01','FIYHI01'),'') WHEN -1 THEN ISNULL(C.CrentialsFile,ISNULL(B.CrentialsFile,'')) ELSE '' END 
,LSFLG = 0
,AlreadyType = CASE A.MotorDriver_2 WHEN 2 THEN 6 ELSE '' END
,AlreadyFile = CASE A.MotorDriver_2 WHEN 2 THEN ISNULL(REPLACE(C.CrentialsFile,'FIYHFC01','FIYHI01'),'') ELSE '' END
,IsNew = D.IsNew
,UPDTime = CASE A.MotorDriver_2 WHEN 0 THEN isnull(B.UPDTime,'') WHEN 1 THEN B.UPDTime WHEN 2 THEN C.UPDTime WHEN -1 THEN E.UPDTime END
,AuditResult = A.MotorDriver_2
,RejectReason = CASE A.MotorDriver_2 WHEN -1 THEN E.RejectReason ELSE '' END
FROM #TB_Credentials A
LEFT JOIN #TB_tmpCrentialsPIC B ON A.IDNO=B.IDNO AND B.CrentialsType=6
LEFT JOIN #TB_CrentialsPIC C ON A.IDNO=C.IDNO AND C.CrentialsType=6
LEFT JOIN TB_MemberDataOfAutdit D ON A.IDNO=D.MEMIDNO
LEFT JOIN #TB_AuditCrentialsReject E ON A.IDNO=E.IDNO AND E.CrentialsType=6
WHERE A.IDNO=@IDNO
UNION
--7 Self_1
SELECT IDNO = @IDNO
,CrentialsType = CASE A.Self_1 WHEN 1 THEN 7 WHEN 0 THEN 7 WHEN -1 THEN 7 ELSE '' END
,CrentialsFile = CASE A.Self_1 WHEN 1 THEN Replace(B.CrentialsFile,'FIYHFC01','FIYHI01') WHEN 0 THEN isnull(Replace(B.CrentialsFile,'FIYHFC01','FIYHI01'),'') WHEN -1 THEN ISNULL(C.CrentialsFile,ISNULL(B.CrentialsFile,'')) ELSE '' END 
,LSFLG = 0
,AlreadyType = CASE A.Self_1 WHEN 2 THEN 7 ELSE '' END
,AlreadyFile = CASE A.Self_1 WHEN 2 THEN ISNULL(REPLACE(C.CrentialsFile,'FIYHFC01','FIYHI01'),'') ELSE '' END
,IsNew = D.IsNew
,UPDTime = CASE A.Self_1 WHEN 0 THEN isnull(B.UPDTime,'') WHEN 1 THEN B.UPDTime WHEN 2 THEN C.UPDTime WHEN -1 THEN E.UPDTime END
,AuditResult = A.Self_1
,RejectReason = CASE A.Self_1 WHEN -1 THEN E.RejectReason ELSE '' END
FROM #TB_Credentials A
LEFT JOIN #TB_tmpCrentialsPIC B ON A.IDNO=B.IDNO AND B.CrentialsType=7
LEFT JOIN #TB_CrentialsPIC C ON A.IDNO=C.IDNO AND C.CrentialsType=7
LEFT JOIN TB_MemberDataOfAutdit D ON A.IDNO=D.MEMIDNO
LEFT JOIN #TB_AuditCrentialsReject E ON A.IDNO=E.IDNO AND E.CrentialsType=7
WHERE A.IDNO=@IDNO
UNION
--8 Law_Agent
SELECT IDNO = @IDNO
,CrentialsType = CASE A.Law_Agent WHEN 1 THEN 8 WHEN 0 THEN 8 WHEN -1 THEN 8 ELSE '' END
,CrentialsFile = CASE A.Law_Agent WHEN 1 THEN Replace(B.CrentialsFile,'FIYHFC01','FIYHI01') WHEN 0 THEN isnull(Replace(B.CrentialsFile,'FIYHFC01','FIYHI01'),'') WHEN -1 THEN ISNULL(C.CrentialsFile,ISNULL(B.CrentialsFile,'')) ELSE '' END 
,LSFLG = 0
,AlreadyType = CASE A.Law_Agent WHEN 2 THEN 8 ELSE '' END
,AlreadyFile = CASE A.Law_Agent WHEN 2 THEN ISNULL(REPLACE(C.CrentialsFile,'FIYHFC01','FIYHI01'),'') ELSE '' END
,IsNew = D.IsNew
,UPDTime = CASE A.Law_Agent WHEN 0 THEN isnull(B.UPDTime,'') WHEN 1 THEN B.UPDTime WHEN 2 THEN C.UPDTime WHEN -1 THEN E.UPDTime END
,AuditResult = A.Law_Agent
,RejectReason = CASE A.Law_Agent WHEN -1 THEN E.RejectReason ELSE '' END
FROM #TB_Credentials A
LEFT JOIN #TB_tmpCrentialsPIC B ON A.IDNO=B.IDNO AND B.CrentialsType=8
LEFT JOIN #TB_CrentialsPIC C ON A.IDNO=C.IDNO AND C.CrentialsType=8
LEFT JOIN TB_MemberDataOfAutdit D ON A.IDNO=D.MEMIDNO
LEFT JOIN #TB_AuditCrentialsReject E ON A.IDNO=E.IDNO AND E.CrentialsType=8
WHERE A.IDNO=@IDNO
UNION
--9 Other_1
SELECT IDNO = @IDNO
,CrentialsType = CASE A.Other_1 WHEN 1 THEN 9 WHEN 0 THEN 9 WHEN -1 THEN 9 ELSE '' END
,CrentialsFile = CASE A.Other_1 WHEN 1 THEN Replace(B.CrentialsFile,'FIYHFC01','FIYHI01') WHEN 0 THEN isnull(Replace(B.CrentialsFile,'FIYHFC01','FIYHI01'),'') WHEN -1 THEN ISNULL(C.CrentialsFile,ISNULL(B.CrentialsFile,'')) ELSE '' END 
,LSFLG = 0
,AlreadyType = CASE A.Other_1 WHEN 2 THEN 9 ELSE '' END
,AlreadyFile = CASE A.Other_1 WHEN 2 THEN ISNULL(REPLACE(C.CrentialsFile,'FIYHFC01','FIYHI01'),'') ELSE '' END
,IsNew = D.IsNew
,UPDTime = CASE A.Other_1 WHEN 0 THEN isnull(B.UPDTime,'') WHEN 1 THEN B.UPDTime WHEN 2 THEN C.UPDTime WHEN -1 THEN E.UPDTime END
,AuditResult = A.Other_1
,RejectReason = CASE A.Other_1 WHEN -1 THEN E.RejectReason ELSE '' END
FROM #TB_Credentials A
LEFT JOIN #TB_tmpCrentialsPIC B ON A.IDNO=B.IDNO AND B.CrentialsType=9
LEFT JOIN #TB_CrentialsPIC C ON A.IDNO=C.IDNO AND C.CrentialsType=9
LEFT JOIN TB_MemberDataOfAutdit D ON A.IDNO=D.MEMIDNO
LEFT JOIN #TB_AuditCrentialsReject E ON A.IDNO=E.IDNO AND E.CrentialsType=9
WHERE A.IDNO=@IDNO
UNION
--10 Business_1
SELECT IDNO = @IDNO
,CrentialsType = CASE A.Business_1 WHEN 1 THEN 10 WHEN 0 THEN 10 WHEN -1 THEN 10 ELSE '' END
,CrentialsFile = CASE A.Business_1 WHEN 1 THEN Replace(B.CrentialsFile,'FIYHFC01','FIYHI01') WHEN 0 THEN isnull(Replace(B.CrentialsFile,'FIYHFC01','FIYHI01'),'') WHEN -1 THEN ISNULL(C.CrentialsFile,ISNULL(B.CrentialsFile,'')) ELSE '' END 
,LSFLG = 0
,AlreadyType = CASE A.Business_1 WHEN 2 THEN 10 ELSE '' END
,AlreadyFile = CASE A.Business_1 WHEN 2 THEN ISNULL(REPLACE(C.CrentialsFile,'FIYHFC01','FIYHI01'),'') ELSE '' END
,IsNew = D.IsNew
,UPDTime = CASE A.Business_1 WHEN 0 THEN isnull(B.UPDTime,'') WHEN 1 THEN B.UPDTime WHEN 2 THEN C.UPDTime WHEN -1 THEN E.UPDTime END
,AuditResult = A.Business_1
,RejectReason = CASE A.Business_1 WHEN -1 THEN E.RejectReason ELSE '' END
FROM #TB_Credentials A
LEFT JOIN #TB_tmpCrentialsPIC B ON A.IDNO=B.IDNO AND B.CrentialsType=10
LEFT JOIN #TB_CrentialsPIC C ON A.IDNO=C.IDNO AND C.CrentialsType=10
LEFT JOIN TB_MemberDataOfAutdit D ON A.IDNO=D.MEMIDNO
LEFT JOIN #TB_AuditCrentialsReject E ON A.IDNO=E.IDNO AND E.CrentialsType=10
WHERE A.IDNO=@IDNO
UNION
--11 Signture
SELECT IDNO = @IDNO
,CrentialsType = CASE A.Signture WHEN 1 THEN 11 WHEN 0 THEN 11 WHEN -1 THEN 11 ELSE '' END
,CrentialsFile = CASE A.Signture WHEN 1 THEN Replace(B.CrentialsFile,'FIYHFC01','FIYHI01') WHEN 0 THEN isnull(Replace(B.CrentialsFile,'FIYHFC01','FIYHI01'),'') WHEN -1 THEN ISNULL(C.CrentialsFile,ISNULL(B.CrentialsFile,'')) ELSE '' END 
,LSFLG = 0
,AlreadyType = CASE A.Signture WHEN 2 THEN 11 ELSE '' END
,AlreadyFile = CASE A.Signture WHEN 2 THEN ISNULL(REPLACE(C.CrentialsFile,'FIYHFC01','FIYHI01'),'') ELSE '' END
,IsNew = D.IsNew
,UPDTime = CASE A.Signture WHEN 0 THEN isnull(B.UPDTime,'') WHEN 1 THEN B.UPDTime WHEN 2 THEN C.UPDTime WHEN -1 THEN E.UPDTime END
,AuditResult = A.Signture
,RejectReason = CASE A.Signture WHEN -1 THEN E.RejectReason ELSE '' END
FROM #TB_Credentials A
LEFT JOIN #TB_tmpCrentialsPIC B ON A.IDNO=B.IDNO AND B.CrentialsType=11
LEFT JOIN #TB_CrentialsPIC C ON A.IDNO=C.IDNO AND C.CrentialsType=11
LEFT JOIN TB_MemberDataOfAutdit D ON A.IDNO=D.MEMIDNO
LEFT JOIN #TB_AuditCrentialsReject E ON A.IDNO=E.IDNO AND E.CrentialsType=11
WHERE A.IDNO=@IDNO

DROP TABLE #TB_CrentialsPIC
DROP TABLE #TB_tmpCrentialsPIC
DROP TABLE #TB_Credentials
DROP TABLE #TB_AuditCrentialsReject