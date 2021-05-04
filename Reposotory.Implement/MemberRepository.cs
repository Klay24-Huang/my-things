using Domain.MemberData;
using Domain.TB.BackEnd;
using NLog.Internal;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using WebCommon;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Reposotory.Implement
{
    /// <summary>
    /// 會員資料相關
    /// </summary>
    public class MemberRepository:BaseRepository
    {
        private string _connectionString { set; get; }
        public MemberRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public RegisterData GetMemberData(string  IDNO)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<RegisterData> lstMember = null;
            RegisterData obj = null;
            int nowCount = 0;
            string SQL = "SELECT [MEMIDNO],[MEMPWD],[MEMCNAME],[MEMTEL],ISNULL([MEMBIRTH],'') AS [MEMBIRTH] ";
            SQL += " ,[MEMCITY] AS MEMAREAID,[MEMADDR],[MEMEMAIL],[CARDNO],[UNIMNO] ";
            SQL += " ,[MEMSENDCD],[CARRIERID],[NPOBAN],[HasCheckMobile],[NeedChangePWD] ";
            SQL += " ,[HasBindSocial],[IrFlag],[PayMode],[HasVaildEMail],[Audit],[RentType] ";
            SQL += " ,Case When [ID_1]=1 And [ID_2] =1 Then B.ID_1 Else 0 End ID_pic ";
            SQL += " ,Case When [CarDriver_1]=1 And [CarDriver_2]=1 Then B.CarDriver_1 Else 0 End DD_pic ";
            SQL += " ,Case When [MotorDriver_1]=1 And [MotorDriver_2]=1 Then B.MotorDriver_1 Else 0 End MOTOR_pic ";
            SQL += " ,ISNULL([Self_1],0) As AA_pic ,ISNULL([Law_Agent],0) As F01_pic";
            SQL += " FROM TB_MemberData A WITH(NOLOCK) ";
            SQL += " Left Join [TB_Credentials] B WITH(NOLOCK) on B.IDNO=A.MEMIDNO ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            if (false==string.IsNullOrWhiteSpace(IDNO))
            {
                term += " MEMIDNO=@IDNO";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar,20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WHERE " + term;
            }
     
            lstMember = GetObjList<RegisterData>(ref flag, ref lstError, SQL, para, term);
            if(lstMember!=null)
            {
                if (lstMember.Count > 0)
                {
                    obj = new RegisterData();
                    obj = lstMember[0];
                }
            }
            return obj;
        }
        public List<BE_SameMobileData> GetSameMobile()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_SameMobileData> lstMember = null;
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            string SQL = " SELECT * FROM VW_BE_GetSameMobile ORDER BY MEMTEL ASC";

            lstMember = GetObjList<BE_SameMobileData>(ref flag, ref lstError, SQL, para, term);
            return lstMember;
        }
        /// <summary>
        /// 取得相同手機號碼
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="TEL"></param>
        /// <returns></returns>
        public List<BE_SameMobileData> GetSameMobile(string IDNO,string TEL)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_SameMobileData> lstMember = null;
            SqlParameter[] para = new SqlParameter[5];
            int nowCount = 0;
            string term = "";
            string SQL = " SELECT * FROM VW_BE_GetSameMobile ";
            //if (false == string.IsNullOrWhiteSpace(IDNO) && false == string.IsNullOrWhiteSpace(TEL))
            //{
                term += " IDNO<>@IDNO AND MEMTEL=@MEMTEL AND MEMTEL<>''";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
                para[nowCount] = new SqlParameter("@MEMTEL", SqlDbType.VarChar, 20);
                para[nowCount].Value = TEL;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            //}

            if ("" != term)
            {
                SQL += " WHERE " + term;
            }
            SQL += " ORDER BY MEMTEL ASC";
            lstMember = GetObjList<BE_SameMobileData>(ref flag, ref lstError, SQL, para, term);
            return lstMember;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuditMode"></param>
        /// <param name="AuditType"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="AuditReuslt"></param>
        /// <param name="UserName"></param>
        /// <param name="IDNO"></param>
        /// <param name="IDNOSuff"></param>
        /// <returns></returns>
        public List<BE_GetAuditList> GetAuditLists(int AuditMode, int AuditType, string StartDate, string EndDate, int AuditReuslt, string UserName, string IDNO, string IDNOSuff, string AuditError)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetAuditList> lstAudits = null;
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            string term2 = "";
            //string SQL = " SELECT TOP 300 * FROM VW_GetAuditList WITH(NOLOCK) ";
            string SQL = " EXEC usp_BE_GetAuditList  '" + AuditMode.ToString() + 
                "','" + AuditType.ToString() +
                "','" + (StartDate == "" ? "" : StartDate + " 00:00:00") +
                "','" + (EndDate == "" ? "" : EndDate + " 23:59:59") +
                "','" + AuditReuslt.ToString() + 
                "','" + UserName + 
                "','" + IDNO +
                "','" + IDNOSuff +
                "','" + AuditError + "'";
            int nowCount = 0;
            //if (false == string.IsNullOrWhiteSpace(IDNO))
            //{
            //    if (term != "") { term += " AND "; }
            //    term += " MEMIDNO=@IDNO";
            //    para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
            //    para[nowCount].Value = IDNO;
            //    para[nowCount].Direction = ParameterDirection.Input;
            //    nowCount++;
            //}
            //if (false == string.IsNullOrWhiteSpace(UserName))
            //{
            //    if (term != "") { term += " AND "; }
            //    term += " MEMCNAME=@UserName";
            //    para[nowCount] = new SqlParameter("@UserName", SqlDbType.NVarChar, 20);
            //    para[nowCount].Value = UserName;
            //    para[nowCount].Direction = ParameterDirection.Input;
            //    nowCount++;
            //}
            //if (AuditMode>-1)
            //{
            //    if (term != "") { term += " AND "; }
            //    term += " IsNew =@IsNew ";
            //    para[nowCount] = new SqlParameter("@IsNew", SqlDbType.TinyInt);
            //    para[nowCount].Value = AuditMode;
            //    para[nowCount].Direction = ParameterDirection.Input;
            //    nowCount++;
            //}
            //if (AuditType > -1)
            //{
            //    if (term != "") { term += " AND "; }
            //    term += " HasAudit =@HasAudit";
            //    para[nowCount] = new SqlParameter("@HasAudit", SqlDbType.TinyInt);
            //    para[nowCount].Value = AuditType;
            //    para[nowCount].Direction = ParameterDirection.Input;
            //    nowCount++;
            //}
            //if (AuditError != "")
            //{
            //    if (term != "") { term += " AND "; }
            //    if (AuditError == "Y")
            //    {
            //        term += " MEMO =''";
            //    }
            //    else
            //    {
            //        term += " MEMO <>''";
            //    }
            //}
            //if (string.IsNullOrEmpty(StartDate) == false)
            //{
            //    if (string.IsNullOrEmpty(EndDate) == false)
            //    {
            //        //term2 = " AND ((ApplyDate between @SD AND @ED) OR (ApplyDate between @SD AND @ED))";
            //        //20201114 ADD BY ADAM REASON.申請加入看MKTime身分變更看UPDTime
            //        if (term != "") { term += " AND "; }
            //        //term += " ((" + (AuditMode==1 ? "ApplyDate" : "ModifyDate") + " between @SD AND @ED) OR (" + (AuditMode == 1 ? "ApplyDate" : "ModifyDate") + " between @SD AND @ED))";
            //        term += " (CASE WHEN IsNew=1 THEN ApplyDate ELSE ModifyDate END between @SD AND @ED)";
            //        para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 20);
            //        para[nowCount].Value = StartDate+" 00:00:00";
            //        para[nowCount].Direction = ParameterDirection.Input;
            //        nowCount++;
            //        para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 20);
            //        para[nowCount].Value = EndDate + " 23:59:59";
            //        para[nowCount].Direction = ParameterDirection.Input;
            //    }
            //    else
            //    {
            //        //term2 = " AND ApplyDate = @SD";
            //        //20201114 ADD BY ADAM REASON.申請加入看MKTime身分變更看UPDTime
            //        if (term != "") { term += " AND "; }
            //        //term += (AuditMode == 1 ? "ApplyDate" : "ModifyDate") + " = @SD";
            //        term += " CASE WHEN IsNew=1 THEN ApplyDate ELSE ModifyDate END = @SD";
            //        para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 20);
            //        para[nowCount].Value = StartDate + " 00:00:00";
            //        para[nowCount].Direction = ParameterDirection.Input;
            //        nowCount++;
            //    }
            //}
            //else
            //{
            //    if (string.IsNullOrEmpty(EndDate) == false)
            //    {
            //        //term2 = " AND ApplyDate = @ED";
            //        //20201114 ADD BY ADAM REASON.申請加入看MKTime身分變更看UPDTime
            //        if (term != "") { term += " AND "; }
            //        //term += (AuditMode == 1 ? "ApplyDate" : "ModifyDate") + " = @ED";
            //        term += " CASE WHEN IsNew=1 THEN ApplyDate ELSE ModifyDate END = @ED";
            //        para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 20);
            //        para[nowCount].Value = EndDate + " 23:59:59";
            //        para[nowCount].Direction = ParameterDirection.Input;
            //        nowCount++;
            //    }
            //}
            //if (false == string.IsNullOrWhiteSpace(IDNOSuff))
            //{
            //    if (term != "") { term += " AND "; }
            //    term += string.Format(" IDNOSUFF IN ({0}) ", IDNOSuff);

            //}

            //if ("" != term)
            //{
            //    SQL += " WHERE " + term;// " AND SD between @SD AND @ED OR ED between @SD AND @ED ";
            //}
            //if ("" != term2)
            //{
            //    SQL += term2;
            //}

            lstAudits = GetObjList<BE_GetAuditList>(ref flag, ref lstError, SQL, para, term);
            return lstAudits;
        }
        /// <summary>
        /// 取得待審核資料
        /// </summary>
        /// <param name="IDNO"></param>
        /// <returns></returns>
        public BE_AuditDetail GetAuditDetail(string IDNO)
        {
            bool flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_AuditDetail> lstAudits = null;
            BE_AuditDetail obj = null;
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            string term2 = "";
            string SQL = " SELECT * FROM VW_GetAuditDetail ";
            int nowCount = 0;
            if (false == string.IsNullOrWhiteSpace(IDNO))
            {
                if (term != "") { term += " AND "; }
                term += " MEMIDNO=@IDNO";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WHERE " + term;// " AND SD between @SD AND @ED OR ED between @SD AND @ED ";
            }
            lstAudits = GetObjList<BE_AuditDetail>(ref flag, ref lstError, SQL, para, term);
            if (lstAudits != null)
            {
                if (lstAudits.Count > 0)
                {
                    obj = new BE_AuditDetail();
                    obj = lstAudits[0];
                }
            }
            return obj;
        }
        public List<BE_AuditImage> GetAuditImage(string IDNO)
        {
            bool flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_AuditImage> lstAudits = null;
            BE_AuditDetail obj = null;
            SqlParameter[] para = new SqlParameter[0];
            string term = "";
            string term2 = "";
            //string SQL = " EXEC usp_BE_GetAuditImage '" + IDNO + "'";
            string SQL = " EXEC usp_BE_GetAuditImage_Tang '" + IDNO + "'";
            int nowCount = 0;
            lstAudits = GetObjList<BE_AuditImage>(ref flag, ref lstError, SQL, para, term);
    
            return lstAudits;
        }
        public List<BE_AuditImage> UpdateMemberName(string IDNO, string MEMNAME,string USERID)
        {
            bool flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_AuditImage> lstAudits = null;
            BE_AuditDetail obj = null;
            SqlParameter[] para = new SqlParameter[0];
            string term = "";
            string term2 = "";
            string SQL = " EXEC usp_BE_UpdateMemberName  '" + IDNO + "',"+ "N'" + MEMNAME + "','" + USERID + "'"; //20210113唐改，強制改unicode解決難字出現?問題
            int nowCount = 0;
            lstAudits = GetObjList<BE_AuditImage>(ref flag, ref lstError, SQL, para, term);

            return lstAudits;
        }

        public List<BE_AuditImage> UpdateMemberData(string IDNO, string MEMNAME, string Mobile, string Power, string MEMEMAIL, string HasVaildEMail, string MEMMSG, string USERID)
        {
            bool flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_AuditImage> lstAudits = null;
            BE_AuditDetail obj = null;
            SqlParameter[] para = new SqlParameter[0];
            string term = "";
            string term2 = "";
            string SQL = " EXEC usp_BE_UpdateMemberData  '" + IDNO + "'," +
                "N'" + MEMNAME + "'," +
                "'" + Mobile + "'," +
                "'" + Power + "'," +
                "'" + MEMEMAIL + "'," +
                "'" + HasVaildEMail + "'," +
                "'" + (MEMMSG == "1" ? "Y" : "N") + "'," +
                "'" + USERID + "'"; //20210113唐改，強制改unicode解決難字出現?問題
            int nowCount = 0;
            lstAudits = GetObjList<BE_AuditImage>(ref flag, ref lstError, SQL, para, term);

            return lstAudits;
        }
        /// <summary>
        /// 取得審核歷史
        /// </summary>
        /// <param name="IDNO"></param>
        /// <returns></returns>
        public List<BE_AuditHistory> GetAuditHistory(string IDNO)
        {
            bool flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_AuditHistory> lstAudits = null;
            BE_AuditDetail obj = null;
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            string term2 = "";
            string SQL = " SELECT * FROM VW_GetAuditHistory ";
            int nowCount = 0;
            if (false == string.IsNullOrWhiteSpace(IDNO))
            {
                if (term != "") { term += " AND "; }
                term += " IDNO=@IDNO";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WHERE " + term;// " AND SD between @SD AND @ED OR ED between @SD AND @ED ";
            }
            lstAudits = GetObjList<BE_AuditHistory>(ref flag, ref lstError, SQL, para, term);

            return lstAudits;
        }

        /// <summary>
        /// 取得安心保險清單
        /// </summary>
        /// <param name="IDNO"></param>
        /// <returns></returns>
        public List<BE_InsuranceData> GetGetInsuranceData(string IDNO)
        {
            bool flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_InsuranceData> lstAudits = null;
            BE_AuditDetail obj = null;
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            string term2 = "";
            string SQL = " SELECT * FROM VW_BE_GetInsuranceData ";
            int nowCount = 0;
            if (false == string.IsNullOrWhiteSpace(IDNO))
            {
                if (term != "") { term += " AND "; }
                term += " IDNO=@IDNO";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WHERE " + term;// " AND SD between @SD AND @ED OR ED between @SD AND @ED ";
            }
            lstAudits = GetObjList<BE_InsuranceData>(ref flag, ref lstError, SQL, para, term);

            return lstAudits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AuditMode"></param>
        /// <param name="AuditType"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="AuditReuslt"></param>
        /// <param name="UserName"></param>
        /// <param name="IDNO"></param>
        /// <param name="IDNOSuff"></param>
        /// <returns></returns>
        public List<BE_GetAuditList> ChangePassword(string IDNO, string Password)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetAuditList> lstAudits = null;
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            string term2 = "";
            //string SQL = " SELECT TOP 300 * FROM VW_GetAuditList WITH(NOLOCK) ";
            string SQL = " EXEC usp_BE_ChangePassword  '" + IDNO +"','" + Password + "'";
            int nowCount = 0;

            lstAudits = GetObjList<BE_GetAuditList>(ref flag, ref lstError, SQL, para, term);
            return lstAudits;
        }

        /// 取得黑名單手機號碼
        public string GetMobileBlock(string TEL)
        {
            //bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string lstMember = null;
            SqlParameter[] para = new SqlParameter[1];
            //int nowCount = 0;
            //string term = "";
            string SQL = $" SELECT * FROM VW_BE_GetBlockMobile where Mobile={TEL} ";

            lstMember = GetS(SQL);
            return lstMember;
        }

        /// 取得黑名單手機號碼
        public List<BE_GetEasyWalletList> GetEasyWalletList(string IDNO)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetEasyWalletList> lstAudits = null;
            SqlParameter[] para = new SqlParameter[4]; // term是空就用不到
            string term = "";
            //string SQL = $" select orderNo,ITEM,IDNO,convert(char(8),A_SYSDT,112) from EASYPAY_Order where IDNO='{IDNO}' order by U_SYSDT desc ";  //會異常，select出的名稱要和宣告的一樣
            string SQL = $" select a.orderNo, a.ITEM as projectName, a.IDNO,convert(char(8), a.orderCreateDateTime,112) as orderTime, a.merchantOrderNo,b.MEMCNAME " +
                $"from EASYPAY_Order a join TB_MemberData b on a.IDNO = b.MEMIDNO left join EASYPAY_REFUND c on a.orderNo = c.orderNo where a.IDNO = '{IDNO}' and a.redirectPaymentUrl <> '' " +
                $"and convert(char(8), a.orderCreateDateTime,112) > convert(char(8), DATEADD(day, -30, getdate()), 112) and c.orderNo is null order by a.U_SYSDT desc ";

            lstAudits = GetObjList<BE_GetEasyWalletList>(ref flag, ref lstError, SQL, para, term);
            return lstAudits;
        }

        public bool DeleteMember(string IDNO, string IRent_Only, string Account)
        {
            bool result = true;
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            if(IRent_Only == "on")
            {
                SqlParameter[] para = new SqlParameter[3];
                string term = "";
                string SQL = "Create TABLE tmp_DelMemberList(IDNO varchar(11))";
                SQL += $"insert into tmp_DelMemberList values('{IDNO}')";
                SQL += " delete TB_MemberData FROM TB_MemberData A  JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO";
                SQL += " delete TB_MemberDataOfAutdit FROM TB_MemberDataOfAutdit A  JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO";
                SQL += " delete TB_AuditHistory FROM TB_MemberDataOfAutdit A  JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO ";
                SQL += $" insert into AlreadyDeleteMember select N'測試',IDNO,DATEADD(HOUR, 8, GETDATE()),'{Account}'from tmp_DelMemberList";
                SQL += " DROP TABLE tmp_DelMemberList";

                if (Execuate(ref flag, SQL) <= 1)
                {
                    result = false;
                }

                return result;
            }
            else
            {
                string SQL = "Create TABLE tmp_DelMemberList(IDNO varchar(11))";
                SQL += $"insert into tmp_DelMemberList values('{IDNO}')";
                SQL += " delete TB_MemberData FROM TB_MemberData A  JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO";
                SQL += " delete TB_MemberDataOfAutdit FROM TB_MemberDataOfAutdit A  JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO";
                SQL += " delete TB_AuditHistory FROM TB_MemberDataOfAutdit A  JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO ";
                SQL += $" insert into AlreadyDeleteMember select N'測試',IDNO,DATEADD(HOUR, 8, GETDATE()),'{Account}'from tmp_DelMemberList";
                SQL += " DROP TABLE tmp_DelMemberList";

                if (Execuate(ref flag, SQL) <= 1)
                {
                    result = false;
                }

                this.ConnectionString = ConfigurationManager.ConnectionStrings["06VM"].ConnectionString;
                SqlParameter[] para = new SqlParameter[3];
                string SQL06 = "Create TABLE tmp_DelMemberList(IDNO varchar(11))";
                SQL06 += $" insert into tmp_DelMemberList values('{IDNO}')";
                SQL06 += " delete MEMBER_NEW FROM MEMBER_NEW A  JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO";
                SQL06 += " delete [dbo].[IRENT_GIFTMINSHIS] FROM [dbo].[IRENT_GIFTMINSHIS] A JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO ";
                SQL06 += " delete [dbo].[IRENT_GIFTMINSMF] FROM [dbo].[IRENT_GIFTMINSMF] A  JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO ";
                SQL06 += " delete [dbo].[IRENT_SIGNATURE]	FROM [dbo].[IRENT_SIGNATURE] A  JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO ";
                SQL06 += " delete [dbo].[MEMBER_API]		FROM [dbo].[MEMBER_API] A		JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO ";
                SQL06 += " delete [dbo].[MEMBER_API_LOG]	FROM [dbo].[MEMBER_API_LOG] A	JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO ";
                SQL06 += " delete [dbo].[MEMBER_VERIFY]	FROM [dbo].[MEMBER_VERIFY] A	JOIN tmp_DelMemberList B ON A.MEMIDNO = B.IDNO ";
                SQL06 += $" insert into AlreadyDeleteMember select N'測試 ',IDNO,DATEADD(HOUR,8,GETDATE()),'{Account}'from tmp_DelMemberList";
                SQL06 += " DROP TABLE tmp_DelMemberList";

                if (Execuate(ref flag, SQL) <= 1)
                {
                    result = false;
                }

                return result;
            }
        }

        public bool ChangeID(string TARGET_ID, string AFTER_ID, string Account)
        {
            bool result = true;
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SqlParameter[] para = new SqlParameter[3];
            string term = "";
            string SQL = "BEGIN TRAN";
            SQL += $" DECLARE @TARGET_IDNO	VARCHAR(10) SET @TARGET_IDNO	= '{TARGET_ID}'";
            SQL += $" DECLARE @AFTER_IDNO		VARCHAR(10) SET @AFTER_IDNO		= '{AFTER_ID}'";
            SQL += " DECLARE @NOW			DATETIME	SET @NOW			= DATEADD(HOUR,8,GETDATE())";
            SQL += " update TB_MemberData			set MEMIDNO = @AFTER_IDNO	,U_SYSDT = @NOW	where MEMIDNO = @TARGET_IDNO ";
            SQL += " update TB_MemberDataOfAutdit	set MEMIDNO = @AFTER_IDNO	,UPDTime = @NOW	where MEMIDNO = @TARGET_IDNO ";
            SQL += " update TB_MemberBySocial		set MEMIDNO = @AFTER_IDNO	,UPDTime = @NOW	where MEMIDNO = @TARGET_IDNO";
            SQL += " update TB_CrentialsPIC			set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " update TB_CrentialsPIC_NULL		set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " update TB_Credentials    		set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO ";
            SQL += " update TB_tmpCrentialsPIC		set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO ";
            SQL += " update TB_AuditHistory			set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " update TB_AuditCredentials		set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " delete TB_AuditCrentialsReject												where IDNO = @AFTER_IDNO";
            SQL += " update TB_AuditCrentialsReject	set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " update TB_OrderMain				set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " update TB_VerifyCode            set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " update TB_MemberCardBinding		set IDNO = @AFTER_IDNO		,UPDTime = @NOW	where IDNO = @TARGET_IDNO";
            SQL += " update TB_MonthlyRent			set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " update TB_MonthlyRentHistory	set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " update TB_MonthlyRentHistory_LOG set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " update TB_MemberDataBlock       set MEMIDNO = @AFTER_IDNO						where MEMIDNO = @TARGET_IDNO";
            SQL += " update TB_BookingStatusOfUser	SET IDNO = @AFTER_IDNO		,UPDTime = @NOW	where IDNO = @TARGET_IDNO";
            SQL += " update TB_BookingInsuranceOfUser set IDNO = @AFTER_IDNO,UPDTime = DATEADD(HOUR,8,GETDATE()) where  IDNO = @TARGET_IDNO";
            SQL += " update TB_BookingInsuranceOfUserHIS set IDNO = @AFTER_IDNO,UPDTime = DATEADD(HOUR,8,GETDATE()) where  IDNO = @TARGET_IDNO";
            SQL += " update TB_FeedBack				set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " update TB_FavoriteStation		set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += " update TB_PersonNotification	set IDNO = @AFTER_IDNO						where IDNO = @TARGET_IDNO";
            SQL += $" insert into TB_ChangeID_LOG (OLD_ID, NEW_ID, A_SYSDT, A_USERID) values(@TARGET_IDNO, @AFTER_IDNO, DATEADD(HOUR,8,GETDATE()), {Account})";
            SQL += " COMMIT TRAN";

            if(Execuate(ref flag, SQL) <= 1)
            {
                result = false;
            }

            this.ConnectionString = ConfigurationManager.ConnectionStrings["06VM"].ConnectionString;
            bool flag06 = false;
            string SQL06 = "BEGIN TRAN";
            SQL06 += $" DECLARE @TARGET_IDNO	VARCHAR(10) SET @TARGET_IDNO	= '{TARGET_ID}'";
            SQL06 += $" DECLARE @AFTER_IDNO		VARCHAR(10) SET @AFTER_IDNO		= '{AFTER_ID}'";
            SQL06 += " DECLARE @NOW			DATETIME	SET @NOW			= GETDATE()";
            SQL06 += " update MEMBER_NEW			set  MEMIDNO = @AFTER_IDNO,U_SYSDT = @NOW	where MEMIDNO = @TARGET_IDNO";
            SQL06 += " update MEMBER_NEW_LOG		set  MEMIDNO = @AFTER_IDNO	where MEMIDNO = @TARGET_IDNO";
            SQL06 += " update IRENT_GIFTMINSMF		set  MEMIDNO = @AFTER_IDNO	where MEMIDNO = @TARGET_IDNO";
            SQL06 += " update IRENT_GIFTMINSHIS	set  MEMIDNO = @AFTER_IDNO	where MEMIDNO = @TARGET_IDNO";
            SQL06 += $" insert into ChangeID_LOG (OLD_ID, NEW_ID, A_SYSDT, A_USERID) values(@TARGET_IDNO, @AFTER_IDNO, GETDATE(), {Account})";
            SQL06 += " COMMIT TRAN";

            if (Execuate(ref flag06, SQL06) <= 1)
            {
                result = false;
            }

            this.ConnectionString = ConfigurationManager.ConnectionStrings["01VM_LS"].ConnectionString;
            bool flag01_LS = false;
            string SQL01_LS = "BEGIN TRAN";
            SQL01_LS += $" DECLARE @TARGET_IDNO	VARCHAR(10) SET @TARGET_IDNO	= '{TARGET_ID}'";
            SQL01_LS += $" DECLARE @AFTER_IDNO		VARCHAR(10) SET @AFTER_IDNO		= '{AFTER_ID}'";
            SQL01_LS += " UPDATE LSRENTMF	SET CUSTID = @AFTER_IDNO,U_SYSDT = GETDATE()	where CUSTID = @TARGET_IDNO";
            SQL01_LS += " UPDATE IRENT_MONTH_RENTMF		SET CUSTID = @AFTER_IDNO,U_SYSDT = GETDATE()		where CUSTID = @TARGET_IDNO";
            SQL01_LS += " UPDATE IRENT_MONTH_RENTMF_LOG	SET CUSTID = @AFTER_IDNO,U_SYSDT = GETDATE()		where CUSTID = @TARGET_IDNO";
            SQL01_LS += " UPDATE IRENT_SIGNATURE			SET CUSTID = @AFTER_IDNO,U_SYSDT = GETDATE()		where CUSTID = @TARGET_IDNO";
            SQL01_LS += " UPDATE LC..LCCUBKDF				SET CUSTID = @AFTER_IDNO,U_SYSDT = GETDATE()		where CUSTID = @TARGET_IDNO";
            SQL01_LS += " UPDATE LC..LCCUSTAGREEDF		SET CUSTID = @AFTER_IDNO,U_SYSDT = GETDATE()		where CUSTID = @TARGET_IDNO";
            SQL01_LS += " UPDATE IRENT_INSURANCE_LEVEL	SET CUSTID = @AFTER_IDNO							where CUSTID = @TARGET_IDNO";
            SQL01_LS += $" insert into ChangeID_LOG (OLD_ID, NEW_ID, A_SYSDT, A_USERID) values(@TARGET_IDNO, @AFTER_IDNO, GETDATE(), {Account})";
            SQL01_LS += " COMMIT TRAN";

            if (Execuate(ref flag01_LS, SQL01_LS) <= 1)
            {
                result = false;
            }

            return result;
        }
    }
}