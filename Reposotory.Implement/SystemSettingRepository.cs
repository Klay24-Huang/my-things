using Domain.TB.BackEnd;
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
    public class SystemSettingRepository : BaseRepository
    {
        private string _connectionString;
        public SystemSettingRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<BE_FeedBackKindData> GetFeedBackKind(string descript, int star, int ShowType)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_FeedBackKindData> lstFeedBackKind= null;


            int nowCount = 0;
            string SQL = " SELECT [FeedBackKindId] ,[Star],[Descript],[use_flag] FROM [TB_FeedBackKind] ";

            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            if (false == string.IsNullOrWhiteSpace(descript))
            {
                term += " Descript like @Descript";
                para[nowCount] = new SqlParameter("@Descript", SqlDbType.VarChar, 10);
                para[nowCount].Value = string.Format("%{0}%", descript);
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (star>0 && star<6)
            {
                term += (term == "") ? "" : " AND ";
                term += " (Star = @Star ) ";
                para[nowCount] = new SqlParameter("@Star", SqlDbType.Int);
                para[nowCount].Value = star;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (ShowType <2)
            {
                term += (term == "") ? "" : " AND ";
                term += " use_flag=@ShowType";
                para[nowCount] = new SqlParameter("@ShowType", SqlDbType.Int);
                para[nowCount].Value = ShowType;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }

            lstFeedBackKind = GetObjList<BE_FeedBackKindData>(ref flag, ref lstError, SQL, para, term);

            return lstFeedBackKind;
        }
    }
}
