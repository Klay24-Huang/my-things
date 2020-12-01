using Domain.TB;
using Domain.TB.BackEnd;
using Domain.TB.SubScript;
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
    }
}
