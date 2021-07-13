using Domain.TB;
using Domain.TB.BackEnd;
using Domain.TB.SubScript;
using Domain.SP.Input.Common;
using Domain.SP.Input.MonthlyRent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommon;
namespace Reposotory.Implement
{
    public class SubScriptionRepository : BaseRepository
    {
        private string _connectionString;
        public SubScriptionRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<SubScriptionForClient> GetSubScriptionForClients(string IDNO)
        {
            bool flag = true;
            List<SubScriptionForClient> objList = null;
            List<ErrorInfo> errorInfos = new List<ErrorInfo>();
            string str = "SELECT * FROM VW_SubScriptionQueryByClient ";
            SqlParameter[] sqlParameter = new SqlParameter[5];
            string str1 = "";
            int num = 0;
            if (!string.IsNullOrEmpty(IDNO))
            {
                str1 = " IDNO=@IDNO";
                sqlParameter[num] = new SqlParameter("@IDNO", SqlDbType.VarChar, 30);
                sqlParameter[num].Value = IDNO;
                sqlParameter[num].Direction = ParameterDirection.Input;
                num++;
                if ("" != str1)
                {
                    str = string.Concat(str, " WHERE ", str1);
                }
                str = string.Concat(str, "  ORDER BY startDate ASC");
                objList = this.GetObjList<SubScriptionForClient>(ref flag, ref errorInfos, str, sqlParameter, str1);
            }
            return objList;
        }


        public List<MemberAuth> GetMemberData(string IDNO)
        {
            bool flag = true;
            List<MemberAuth> objList = null;
            List<ErrorInfo> errorInfos = new List<ErrorInfo>();
            string str = "SELECT MEMIDNO,MEMPWD FROM TB_MemberData WITH(NOLOCK) ";
            SqlParameter[] sqlParameter = new SqlParameter[5];
            string str1 = "";
            int num = 0;
            if (!string.IsNullOrEmpty(IDNO))
            {
                str1 = " MEMIDNO=@IDNO";
                sqlParameter[num] = new SqlParameter("@IDNO", SqlDbType.VarChar, 30);
                sqlParameter[num].Value = IDNO;
                sqlParameter[num].Direction = ParameterDirection.Input;
                num++;
                if ("" != str1)
                {
                    str = string.Concat(str, " WHERE ", str1);
                }
                objList = this.GetObjList<MemberAuth>(ref flag, ref errorInfos, str, sqlParameter, str1);
            }
            return objList;
        }


        public List<BE_MonthlyQuery> BE_GetMonthlyMain(string IDNO, string SD, string ED, int hasPointer)
        {
            bool flag = true;
            //List<BE_MonthlyQuery> lstQuery = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string spName = "usp_GetMonthlyMain";
            SPInput_GetMonthlyMain spInput = new SPInput_GetMonthlyMain
            {
                IDNO = string.IsNullOrEmpty(IDNO) ? null : IDNO,
                hasPointer = hasPointer
            };
            if(!string.IsNullOrEmpty(SD))
            {
                spInput.SD = Convert.ToDateTime(SD);
            }
            if (!string.IsNullOrEmpty(ED))
            {
                spInput.ED = Convert.ToDateTime(ED);
            }


            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_GetMonthlyMain, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetMonthlyMain, SPOutput_Base>(this.ConnectionString);
            List<BE_MonthlyQuery> lstOut = new List<BE_MonthlyQuery>();
            DataSet ds = new DataSet();
            flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref lstOut, ref ds, ref lstError);
          
            if (flag)
            {
                if (spOut.Error == 1)
                {
                    lstOut = null;
                }
            }

            return lstOut;
        }

        public List<BE_MonthlyQuery> BE_QueryMonthlyMain(string IDNO, string SD, string ED, int hasPointer)
        {
            bool flag = true;
            List<BE_MonthlyQuery> lstQuery = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            
            string SQL = "SELECT Main.IDNO,Main.WorkDayHours,Main.HolidayHours,Main.MotoTotalHours,Main.StartDate,Main.EndDate,ISNULL(Main.SEQNO,0) AS SEQNO,ISNULL(Main.[ProjID],'') AS ProjID   ";
            SQL += ",ISNULL(Main.[ProjNM],'') AS ProjNM FROM TB_MonthlyRent AS Main  ";

            SqlParameter[] para = new SqlParameter[5];
            string term = "";
            int nowCount = 0;

            if (flag)
            {
                if (false == string.IsNullOrEmpty(IDNO))
                {
                    term = " Main.IDNO=@IDNO";
                    para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 30);
                    para[nowCount].Value = IDNO;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }

                if (false == string.IsNullOrEmpty(SD) && false == string.IsNullOrEmpty(ED))
                {
                    if ("" != term) { term += " AND "; }
                    term += " ((@SD BETWEEN Main.StartDate AND Main.EndDate) OR  (@ED BETWEEN Main.StartDate AND Main.EndDate))";
                    para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 30);
                    para[nowCount].Value = SD;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                    para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 30);
                    para[nowCount].Value = ED;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
                else if (false == string.IsNullOrEmpty(SD) && string.IsNullOrEmpty(ED))
                {
                    if ("" != term) { term += " AND "; }
                    term += " (@SD BETWEEN Main.StartDate AND Main.EndDate) ";
                    para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 30);
                    para[nowCount].Value = SD;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
                else if (string.IsNullOrEmpty(SD) && false == string.IsNullOrEmpty(ED))
                {
                    if ("" != term) { term += " AND "; }
                    term += " (@ED BETWEEN Main.StartDate AND Main.EndDate) ";
                    para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 30);
                    para[nowCount].Value = ED;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }

                if (hasPointer < 2)
                {
                    if ("" != term) { term += " AND "; }
                    if (hasPointer == 0)
                    {
                        term += "  (Main.WorkDayHours=0 AND Main.HolidayHours=0 AND Main.MotoTotalHours=0) ";
                    }
                    else
                    {
                        term += "  (Main.WorkDayHours>0 OR Main.HolidayHours>0 OR Main.MotoTotalHours>0) ";
                    }
                }

                if ("" != term)
                {
                    SQL += " WHERE " + term;
                }
                SQL += "  ORDER BY Main.IDNO ASC";

                lstQuery = GetObjList<BE_MonthlyQuery>(ref flag, ref lstError, SQL, para, term);
            }

            return lstQuery;
        }

        /// <summary>
        /// 新版報表
        /// </summary>
        /// <param name="OrderNum"></param>
        /// <param name="IDNO"></param>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <returns></returns>
        public List<BE_MonthlyReportData> GetMonthlyReportQuery(string OrderNum, string IDNO, string SD, string ED)
        {
            bool flag = true;
            List<BE_MonthlyReportData> lstQuery = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT * FROM VW_BE_GetMonthlyReportData ";

            SqlParameter[] para = new SqlParameter[5];
            string term = "";
            int nowCount = 0;

            if (flag)
            {
                if (false == string.IsNullOrEmpty(IDNO))
                {
                    term = " IDNO=@IDNO";
                    para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 30);
                    para[nowCount].Value = IDNO;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
                if (false == string.IsNullOrEmpty(OrderNum))
                {
                    if ("" != term) { term += " AND "; }
                    term += " OrderNo=@OrderNum";
                    para[nowCount] = new SqlParameter("@OrderNum", SqlDbType.VarChar, 30);
                    para[nowCount].Value = OrderNum;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }

                if (false == string.IsNullOrEmpty(SD) && false == string.IsNullOrEmpty(ED))
                {
                    if ("" != term) { term += " AND "; }
                    term += " (MKTime BETWEEN @SD AND @ED)";
                    para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 30);
                    para[nowCount].Value = SD;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                    para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 30);
                    para[nowCount].Value = ED;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
                else if (false == string.IsNullOrEmpty(SD) && string.IsNullOrEmpty(ED))
                {
                    if ("" != term) { term += " AND "; }
                    term += " MKTime>=@SD AND MKTime<=@SDE";
                    para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 30);
                    para[nowCount].Value = SD + " 00:00:00";
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                    para[nowCount] = new SqlParameter("@SDE", SqlDbType.VarChar, 30);
                    para[nowCount].Value = SD + " 23:59:59";
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
                else if (string.IsNullOrEmpty(SD) && false == string.IsNullOrEmpty(ED))
                {
                    if ("" != term) { term += " AND "; }
                    term += " MKTime>=@ED AND MKTime<=@EDE";
                    para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 30);
                    para[nowCount].Value = ED + " 00:00:00";
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                    para[nowCount] = new SqlParameter("@EDE", SqlDbType.VarChar, 30);
                    para[nowCount].Value = ED + " 23:59:59";
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }



                if ("" != term)
                {
                    SQL += " WHERE " + term;
                }
                SQL += "  ORDER BY OrderNo ASC";

                lstQuery = GetObjList<BE_MonthlyReportData>(ref flag, ref lstError, SQL, para, term);
            }
            return lstQuery;
        }

        /// <summary>
        /// 月租報表
        /// </summary>
        /// <param name="OrderNum"></param>
        /// <param name="IDNO"></param>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <returns></returns>
        public List<BE_MonthlyReportData> GetMonthlyDetail(string OrderNum, string IDNO, string SD, string ED)
        {
            bool flag = true;
            List<BE_MonthlyReportData> lstQuery = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string spName = "usp_GetMonthlyDetail";
            //string SQL = "SELECT * FROM VW_BE_GetMonthlyReportData ";

            SPInput_GetMonthlyDetail spInput = new SPInput_GetMonthlyDetail
            {
                IDNO = string.IsNullOrEmpty(IDNO) ? null : IDNO,
            };
            if (!string.IsNullOrEmpty(OrderNum))
            {
                spInput.OrderNo = Convert.ToInt64(OrderNum);
            }
            if (!string.IsNullOrEmpty(SD) && !string.IsNullOrEmpty(ED))
            {
                spInput.SD = Convert.ToDateTime(SD);
                spInput.ED = Convert.ToDateTime(ED);
            }
            else if (!string.IsNullOrEmpty(SD) && string.IsNullOrEmpty(ED))
            {
                spInput.SD = Convert.ToDateTime(SD);
                spInput.ED = spInput.SD.Value.AddDays(1).AddSeconds(-1);
            }
            else if (string.IsNullOrEmpty(SD) && !string.IsNullOrEmpty(ED))
            {
                spInput.SD = Convert.ToDateTime(ED);
                spInput.ED = spInput.SD.Value.AddDays(1).AddSeconds(-1);
            }


            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_GetMonthlyDetail, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetMonthlyDetail, SPOutput_Base>(this.ConnectionString);
            List<BE_MonthlyReportData> lstOut = new List<BE_MonthlyReportData>();
            DataSet ds = new DataSet();
            flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref lstOut, ref ds, ref lstError);

            if (flag)
            {
                if (spOut.Error == 1)
                {
                    lstOut = null;
                }
            }

            return lstOut;

        }
    }
}
