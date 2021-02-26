/****** Object:  StoredProcedure [dbo].[usp_BE_ChangePassword]    Script Date: 2021/2/23 下午 04:23:49 ******/

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
EXEC usp_BE_GetAuditList '','','20201216 00:00:00','20201226 23:59:59','','','','1,',''
EXEC usp_BE_GetAuditList  '-1','0','2020-12-16 00:00:00','2020-12-26 23:59:59','-1','','','1,','Y'
EXEC usp_BE_GetAuditList  '-1','-1',' 00:00:00',' 23:59:59','-1','','A127389949','',''
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_ChangePassword]   
	@IDNO                   VARCHAR(10)            
	,@PassWord                  VARCHAR(100)         
AS	

UPDATE TB_MemberData
SET MEMPWD=sys.fn_varbintohexstr(HASHBYTES('SHA2_256',@PassWord)),
	U_SYSDT=DATEADD(HOUR,8,GETDATE())
WHERE MEMIDNO=@IDNO

-- 20210226;新增LOG檔
INSERT INTO TB_MemberData_Log
SELECT 'U','BE_CPwd',DATEADD(HOUR,8,GETDATE()),* FROM TB_MemberData WHERE MEMIDNO=@IDNO;

SELECT ISNULL(Autdit.MKTime,MemberData.A_SYSDT) AS ApplyDate,
	ISNULL(Autdit.UPDTime,MemberData.U_SYSDT) AS ModifyDate,
	MemberData.MEMCNAME,
	MemberData.MEMIDNO,
	SUBSTRING(MemberData.MEMIDNO,2,1) AS SEX,
	AuditKind=ISNULL(Autdit.AuditKind,1),
	HasAudit=ISNULL(Autdit.HasAudit,2),
	IsNew=ISNULL(Autdit.IsNew,0),
	SUBSTRING(MemberData.MEMIDNO,10,1) AS IDNOSUFF,
	MemberData.MEMBIRTH,
	MemberData.MEMADDR,
	MemberData.MEMEMAIL
FROM [TB_MemberData] MemberData WITH(NOLOCK)
LEFT JOIN [TB_MemberDataOfAutdit] Autdit WITH(NOLOCK) ON MemberData.MEMIDNO=Autdit.MEMIDNO
WHERE MemberData.MEMIDNO=@IDNO
GO