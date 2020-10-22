using Domain.TB;
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
    public class OtherRepository : BaseRepository
    {
        private string _connectionString;
        public OtherRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<GetFeedBackKindData> GetFeedBackKind(int  IsMotor, int star)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<GetFeedBackKindData> lstFeedBackKind = null;


            int nowCount = 0;
            string SQL = " SELECT [Star],[Descript] FROM [TB_FeedBackKind] ";

            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            if (IsMotor >= 0 && IsMotor < 2)
            {
                term += (term == "") ? "" : " AND ";
                term += " (IsMotor = @IsMotor ) ";
                para[nowCount] = new SqlParameter("@IsMotor", SqlDbType.Int);
                para[nowCount].Value = IsMotor;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (star > 0 && star < 6)
            {
                term += (term == "") ? "" : " AND ";
                term += " (Star >= @Star AND StarU<=@Star ) ";
                para[nowCount] = new SqlParameter("@Star", SqlDbType.Int);
                para[nowCount].Value = star;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

                term += (term == "") ? "" : " AND ";
                term += " use_flag=1";
         


            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term;
            }

            lstFeedBackKind = GetObjList<GetFeedBackKindData>(ref flag, ref lstError, SQL, para, term);

            return lstFeedBackKind;
        }
        public bool HandleHoilday(string[] newHoildays, string[] newShortHoildays, string[] DelStr)
        {
            bool flag = true;
            string SQL = "";
            int len = newHoildays.Length;
            int DelLen = DelStr.Length;
            if (len > 0)
            {
                for(int i = 0; i < len; i++)
                {
                    SQL += string.Format("INSERT INTO TB_Holiday([HolidayYearMonth],[HolidayDate],[use_flag])VALUES({0},{1},1);", newShortHoildays[i], newHoildays[i]);
                }
                ExecNonResponse(ref flag, SQL);
            }
            string DelSQLStr = string.Join(",", DelStr);
            SQL = "DELETE FROM TB_Holiday WHERE HolidayDate IN (" + DelSQLStr + ");";
            ExecNonResponse(ref flag, SQL);
            return flag;
        }
    }
}
