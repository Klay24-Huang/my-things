using Domain;
using Domain.TB;
using Domain.TB.BackEnd;
using Domain.TB.Mochi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebCommon;

namespace Reposotory.Implement
{
    /// <summary>
    /// 停車場相關
    /// </summary>
    public class APILogRepository:BaseRepository
    {
        private string _connectionString { set; get; }
        public APILogRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 取得所有有效的停車場
        /// </summary>
        /// <returns></returns>
        public List<BE_APILog> GetAPILog(string IPAddress, string StartDate, string EndDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_APILog> lstAPILog = null;
            string SQL = string.Format("SELECT TOP 20 A.MKTime,B.APICName,B.APIName,A.APIInput " +
                "FROM TB_APILog A WITH(NOLOCK) " +
                "JOIN TB_APIList B WITH(NOLOCK) ON A.APIID = B.APIID " +
                "WHERE CLIENTIP = '{0}' " +
                "AND A.MKTime BETWEEN '{1}' AND '{2}' " +
                "ORDER BY A.MKTime DESC; ", IPAddress, StartDate, EndDate);
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
            lstAPILog = GetObjList<BE_APILog>(ref flag, ref lstError, SQL, para, term);
            return lstAPILog;
        }
    }
}
