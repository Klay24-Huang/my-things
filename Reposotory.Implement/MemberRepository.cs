using Domain.MemberData;
using Domain.TB.BackEnd;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
//using NLog.Internal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
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
            SqlParameter[] para = new SqlParameter[1];
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
        public List<BE_GetAuditList> GetAuditLists(int AuditMode, int AuditType, string StartDate, string EndDate, int AuditReuslt, string UserName, string IDNO, string IDNOSuff, string AuditError, string MEMRFNBR)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetAuditList> lstAudits = null;
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            //string term2 = "";
            //string SQL = " SELECT TOP 300 * FROM VW_GetAuditList WITH(NOLOCK) ";
            string SQL = " EXEC usp_BE_GetAuditList '" + AuditMode.ToString() + 
                "','" + AuditType.ToString() +
                "','" + (StartDate == "" ? "" : StartDate + " 00:00:00") +
                "','" + (EndDate == "" ? "" : EndDate + " 23:59:59") +
                "','" + AuditReuslt.ToString() + 
                "','" + UserName + 
                "','" + IDNO +
                "','" + IDNOSuff +
                "','" + MEMRFNBR +
                "','" + AuditError + "'";
            //int nowCount = 0;
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
            SqlParameter[] para = new SqlParameter[0];
            string term = "";
            string SQL = " EXEC usp_BE_GetAuditImage_Tang  '" + IDNO + "'";
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
            //string term2 = "";
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
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
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
        /// 取得徽章
        /// </summary>
        /// <param name="IDNO"></param>
        /// <returns></returns>
        public List<BE_MileStone> GetMileStone(string IDNO)
        {
            bool flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_MileStone> lstAudits = null;
            SqlParameter[] para = new SqlParameter[0];
            string term = "";

            string SQL = " EXEC SP_GetMileStone  '" + IDNO + "'" ; 

            lstAudits = GetObjList<BE_MileStone>(ref flag, ref lstError, SQL, para, term);

            return lstAudits;
        }

        public List<BE_MileStoneDetail> GetMileStoneDetail(string IDNO)
        {
            bool flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_MileStoneDetail> lstAudits = null;
            SqlParameter[] para = new SqlParameter[0];
            string term = "";

            string SQL = " SELECT * FROM VW_GetMileStoneDetail where IDNO='" + IDNO + "' order by Action,MKTime desc";

            lstAudits = GetObjList<BE_MileStoneDetail>(ref flag, ref lstError, SQL, para, term);

            return lstAudits;
        }

        public List<BE_MemberScore> GetMemberScore(string IDNO)
        {
            bool flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_MemberScore> lstAudits = null;
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            string SQL = " SELECT * FROM VW_BE_GetMemberScore ";
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
            lstAudits = GetObjList<BE_MemberScore>(ref flag, ref lstError, SQL, para, term);

            return lstAudits;
        }

        public List<BE_GetMemberScoreFull> GetMemberScoreFull(string IDNO, string NAME, string ORDERNO, string SDATE, string EDATE)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetMemberScoreFull> lstAudits = null;
            SqlParameter[] para = new SqlParameter[4]; // term是空就用不到
            string term = "";
            string SQL = $" EXEC SP_GetMemberScoreFull '" + IDNO + "','" + NAME + "','" + ORDERNO + "','" + SDATE + "','" + EDATE + "'";

            lstAudits = GetObjList<BE_GetMemberScoreFull>(ref flag, ref lstError, SQL, para, term);
            return lstAudits;
        }

        public List<BE_GetMemberData> GetMemberData_ForScore(string ORDERNO)
        {
            bool flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetMemberData> lstAudits = null;
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            string SQL = " SELECT * FROM VW_BE_GetMemberData ";
            int nowCount = 0;
            if (false == string.IsNullOrWhiteSpace(ORDERNO))
            {
                if (term != "") { term += " AND "; }
                term += " order_number=@ORDERNO";
                para[nowCount] = new SqlParameter("@ORDERNO", SqlDbType.VarChar, 20);
                para[nowCount].Value = ORDERNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if ("" != term)
            {
                SQL += " WHERE " + term;// " AND SD between @SD AND @ED OR ED between @SD AND @ED ";
            }
            lstAudits = GetObjList<BE_GetMemberData>(ref flag, ref lstError, SQL, para, term);

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
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
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
            //List<ErrorInfo> lstError = new List<ErrorInfo>();
            string lstMember = null;
            //SqlParameter[] para = new SqlParameter[1];
            //int nowCount = 0;
            //string term = "";
            string SQL = $" SELECT * FROM VW_BE_GetBlockMobile where Mobile={TEL} ";

            lstMember = GetS(SQL);
            return lstMember;
        }

        /// 取得悠遊付專案
        public List<BE_GetEasyWalletList> GetEasyWalletList(string IDNO)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetEasyWalletList> lstAudits = null;
            SqlParameter[] para = new SqlParameter[4]; // term是空就用不到
            string term = "";
            string SQL = $" EXEC SP_GetEasyWalletList '" + IDNO + "'";

            lstAudits = GetObjList<BE_GetEasyWalletList>(ref flag, ref lstError, SQL, para, term);
            return lstAudits;
        }
        //取得悠遊付訂單
        public List<BE_Refund> GetEasyWalletOrder(string sdate, string edate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_Refund> lstAudits = null;
            SqlParameter[] para = new SqlParameter[4]; // term是空就用不到
            string term = "";
            string SQL = $" EXEC SP_GetEasyWalletOrder '"+ sdate +"','"+ edate+"'";

            lstAudits = GetObjList<BE_Refund>(ref flag, ref lstError, SQL, para, term);
            return lstAudits;
        }

        public bool IsMemberExist(string IDNO)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SqlParameter[] para = new SqlParameter[1]; // term是空就用不到
            string term = "";
            string SQL = $" SELECT * FROM TB_MemberData WITH(NOLOCK) WHERE IDNO = '{IDNO}'";
            List<BE_MemberData> result = new List<BE_MemberData>();
            result = GetObjList<BE_MemberData>(ref flag, ref lstError, SQL, para, term);
            if(result.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string checkContract(string IDNO)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SqlParameter[] para = new SqlParameter[1]; // term是空就用不到
            string term = "";
            string SQL = $" SELECT * FROM TB_OrderMain WITH(NOLOCK) WHERE IDNO = '{IDNO}'";
            List<BE_MemberData> result = new List<BE_MemberData>();
            result = GetObjList<BE_MemberData>(ref flag, ref lstError, SQL, para, term);
            if(result.Count > 0)
            {
                return "1";
            }
            else
            {
                flag = false;
                lstError = new List<ErrorInfo>();
                para = new SqlParameter[1]; // term是空就用不到
                term = "";
                SQL = " SELECT * FROM TB_OrderMain A WITH(NOLOCK)";
                SQL += " LEFT JOIN TB_OrderDetail B WITH(NOLOCK) ON A.order_number=B.order_number";
                SQL += $" WHERE IDNO = '{IDNO}' AND B.order_number IS NOT NULL ";
                SQL += " order by stop_time desc";
                result = new List<BE_MemberData>();
                result = GetObjList<BE_MemberData>(ref flag, ref lstError, SQL, para, term);
                if (result.Count == 0)
                {
                    return "0";
                }
                else
                {
                    return "2";
                }
            }
        }

        public string DeleteMember(string IDNO, string IRent_Only, string Account)
        {                
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //string apiAddress = "http://localhost:4149/api/" + "NPR010/Delete";
            string apiAddress = ConfigurationManager.AppSettings["BaseURL"] + "NPR010/Delete";
            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(apiAddress)
            };

            string EncryptStr = "";
            string sourceStr = ConfigurationManager.AppSettings["HLCkey"] + ConfigurationManager.AppSettings["userid"] + System.DateTime.Now.ToString("yyyyMMdd");
            ASCIIEncoding enc = new ASCIIEncoding();
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] shaHash = sha.ComputeHash(enc.GetBytes(sourceStr));
            EncryptStr = System.BitConverter.ToString(shaHash).Replace("-", string.Empty);


            string param = JsonConvert.SerializeObject(new
            {
                IDNO = IDNO,
                Account = Account,
                user_id = ConfigurationManager.AppSettings["userid"],
                sig = EncryptStr,
                IRentOnly = IRent_Only
            });

            HttpContent postContent = new StringContent(param, Encoding.UTF8, "application/json");
            HttpResponseMessage apiResponse = new HttpResponseMessage();
            apiResponse = client.PostAsync(apiAddress, postContent).Result;
            string rspStr = apiResponse.Content.ReadAsStringAsync().Result;
            WebAPIOutput_NPR013Reg result = JsonConvert.DeserializeObject<WebAPIOutput_NPR013Reg>(rspStr);

            if(result.Message == "處理成功")
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                conn.Open();
                SqlTransaction tran;
                tran = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = tran;
                cmd.CommandText = "usp_DeleteMember";

                SqlParameter MSG = cmd.Parameters.Add("@MSG", SqlDbType.VarChar, 100);
                MSG.Direction = ParameterDirection.Output;
                cmd.Parameters.Add("IDNO", SqlDbType.VarChar, 11).Value = IDNO;
                cmd.Parameters.Add("Account", SqlDbType.VarChar, 5).Value = Account;

                cmd.ExecuteNonQuery();
                tran.Commit();

                conn.Close();
                conn.Dispose();
            }

            return result.Message;
        }

        public void ChangeID(string TARGET_ID, string AFTER_ID, string Account)
        {

            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            SqlTransaction tran;
            tran = conn.BeginTransaction();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = tran;
            cmd.CommandText = "usp_ChangeID";

            SqlParameter MSG = cmd.Parameters.Add("@MSG", SqlDbType.VarChar, 100);
            MSG.Direction = ParameterDirection.Output;
            cmd.Parameters.Add("TARGET_IDNO", SqlDbType.VarChar, 11).Value = TARGET_ID;
            cmd.Parameters.Add("AFTER_IDNO", SqlDbType.VarChar, 11).Value = AFTER_ID;
            cmd.Parameters.Add("Account", SqlDbType.VarChar, 5).Value = Account;

            cmd.ExecuteNonQuery();
            tran.Commit();

            conn.Close();
            conn.Dispose();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string apiAddress = ConfigurationManager.AppSettings["BaseURL"] + "NPR010/ChangeID";
            HttpClient client = new HttpClient()
            {
                BaseAddress = new System.Uri(apiAddress)
            };

            string EncryptStr = "";
            string sourceStr = ConfigurationManager.AppSettings["HLCkey"] + ConfigurationManager.AppSettings["userid"] + System.DateTime.Now.ToString("yyyyMMdd");
            ASCIIEncoding enc = new ASCIIEncoding();
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] shaHash = sha.ComputeHash(enc.GetBytes(sourceStr));
            EncryptStr = System.BitConverter.ToString(shaHash).Replace("-", string.Empty);


            string param = JsonConvert.SerializeObject(new
            {
                TARGET_IDNO = TARGET_ID,
                AFTER_IDNO = AFTER_ID,
                Account = Account,
                user_id = ConfigurationManager.AppSettings["userid"],
                sig = EncryptStr
            });
            HttpContent postContent = new StringContent(param, Encoding.UTF8, "application/json");
            HttpResponseMessage apiResponse = new HttpResponseMessage();
            apiResponse = client.PostAsync(apiAddress, postContent).Result;
            string rspStr = apiResponse.Content.ReadAsStringAsync().Result;
        }

        public List<BE_ScoreBlock> GetScoreBlock(string IDNO)
        {
            bool flag = true;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_ScoreBlock> lstAudits = null;
            //BE_ScoreBlock obj = null;
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            string SQL = " select TOP 1 A.START_DT AS Sdate,A.END_DT AS Edate from TB_MemberScoreBlock A LEFT JOIN tb_memberScoreMain B ON A.MEMIDNO=B.MEMIDNO ";
            int nowCount = 0;
            if (false == string.IsNullOrWhiteSpace(IDNO))
            {
                if (term != "") { term += " AND "; }
                term += " B.ISBLOCK=1 AND B.MEMIDNO=@IDNO";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
            }
            if ("" != term)
            {
                SQL += " WHERE " + term + " ORDER BY A.A_SYSDT DESC";// " AND SD between @SD AND @ED OR ED between @SD AND @ED ";
            }
            lstAudits = GetObjList<BE_ScoreBlock>(ref flag, ref lstError, SQL, para, term);
            //if (lstAudits != null)
            //{
            //    if (lstAudits.Count > 0)
            //    {
            //        obj = new BE_ScoreBlock();
            //        obj = lstAudits[0];
            //    }
            //}
            //return obj;
            return lstAudits;
        }
    }
}