/****** Object:  StoredProcedure [dbo].[usp_BE_UpdateMemberName]    Script Date: 2021/2/26 上午 10:34:33 ******/

/****************************************************************
** Name: [dbo].[usp_BE_UpdateMemberName]
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
** EXEC @Error=[dbo].[usp_BE_UpdateMemberName]    @ErrorCode OUTPUT,@ErrorMsg OUTPUT,@SQLExceptionCode OUTPUT,@SQLExceptionMsg	 OUTPUT;
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
*****************************************************************/
CREATE PROCEDURE [dbo].[usp_BE_UpdateMemberName]
	@IDNO                   VARCHAR(10)       
	,@MEMNAME               NVARCHAR(60)       
	,@USERID               VARCHAR(10)          
AS	

UPDATE TB_MemberData
SET MEMCNAME=@MEMNAME,
	U_PRGID='',
	U_USERID=@USERID,
	U_SYSDT=DATEADD(HOUR,8,GETDATE())
WHERE MEMIDNO=@IDNO

-- 20210226;新增LOG檔
INSERT INTO TB_MemberData_Log
SELECT 'U','BE_UName',DATEADD(HOUR,8,GETDATE()),* FROM TB_MemberData WHERE MEMIDNO=@IDNO;

UPDATE TB_MemberDataOfAutdit
SET MEMCNAME=@MEMNAME
WHERE MEMIDNO=@IDNO --AND HasAudit=0

-- 20210226;新增LOG檔
INSERT INTO TB_MemberDataOfAutdit_Log
SELECT 'U','BE_UName',DATEADD(HOUR,8,GETDATE()),* FROM TB_MemberDataOfAutdit WHERE MEMIDNO=@IDNO;
GO

