using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.TB.BackEnd;
using WebCommon;
using System.Data.SqlClient;
using System.Data;

namespace Reposotory.Implement
{
    public class MemberScore : BaseRepository
    {
        private string _connectionString;
        public MemberScore(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<BE_SCITEM> GetSCITEM()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_SCITEM> lstOperators = null;
            string SQL = "SELECT DISTINCT SCITEM FROM TB_ScoreDef where UI_STATUS<>0";
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            lstOperators = GetObjList<BE_SCITEM>(ref flag, ref lstError, SQL, para, term);
            return lstOperators;
        }

        public List<BE_SCMITEM> GetSCMITEM(string scitem)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_SCMITEM> lstUserGroup = null;
            int nowCount = 0;
            string SQL = "SELECT SCMITEM=(CASE UI_STATUS WHEN 1 THEN SCMITEM+'~('+CONVERT(varchar(10),SCORE)+')' ELSE SCMITEM+'~'+SCDITEMNO+'('+CONVERT(varchar(10),SCORE)+')' END) FROM TB_ScoreDef ";
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            term += (term == "") ? "" : " AND ";
            term += " SCITEM=@scitem and UI_STATUS<>0 ";
            para[nowCount] = new SqlParameter("@scitem", SqlDbType.VarChar, 60);
            para[nowCount].Value = scitem;
            para[nowCount].Direction = ParameterDirection.Input;
            if ("" != term)
            {
                SQL += " WHERE " + term;
            }
            lstUserGroup = GetObjList<BE_SCMITEM>(ref flag, ref lstError, SQL, para, term);
            return lstUserGroup;
        }
        //public List<BE_MemScore> GetScore(string scmitem)
        //{
        //    bool flag = false;
        //    List<ErrorInfo> lstError = new List<ErrorInfo>();
        //    List<BE_MemScore> lstUserGroup = null;
        //    int nowCount = 0;
        //    string SQL = "select top 1 ISNULL(SCORE,0) from TB_ScoreDef ";
        //    SqlParameter[] para = new SqlParameter[10];
        //    string term = "";
        //    term += (term == "") ? "" : " AND ";
        //    term += " SCMITEM=ISNULL(@scmitem,'') ";
        //    para[nowCount] = new SqlParameter("@scmitem", SqlDbType.VarChar, 60);
        //    para[nowCount].Value = scmitem;
        //    para[nowCount].Direction = ParameterDirection.Input;
        //    if ("" != term)
        //    {
        //        SQL += " WHERE " + term;
        //    }
        //    lstUserGroup = GetObjList<BE_MemScore>(ref flag, ref lstError, SQL, para, term);
        //    return lstUserGroup;
        //}
    }
}
