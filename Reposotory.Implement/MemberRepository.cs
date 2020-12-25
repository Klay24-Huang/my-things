using Domain.MemberData;
using Domain.TB.BackEnd;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebCommon;

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
            if (false == string.IsNullOrWhiteSpace(IDNO) && false == string.IsNullOrWhiteSpace(TEL))
            {
                term += " IDNO<>@IDNO AND  MEMTEL=@MEMTEL";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
                para[nowCount] = new SqlParameter("@MEMTEL", SqlDbType.VarChar, 20);
                para[nowCount].Value = TEL;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

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
            string SQL = " EXEC usp_BE_GetAuditImage  '"+ IDNO + "'";
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
    }
}