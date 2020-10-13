using Domain.TB.BackEnd;
using System.Collections.Generic;
using System.Data.SqlClient;
using WebCommon;

namespace Reposotory.Implement
{
    public class ManagerRepository : BaseRepository
    {
        private string _connectionString;
        public ManagerRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 取得帳號列表
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="lstError"></param>
        /// <returns></returns>
        public List<iRentManager> GetManagerList()
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<iRentManager> lstManager = null;
            string SQL = "SELECT  Account,UserName FROM TB_Manager ";
            SqlParameter[] para = new SqlParameter[2];
            string term = "";
       
            lstManager = GetObjList<iRentManager>(ref flag, ref lstError, SQL, para, term);
            return lstManager;
        }
    }
}
