using Domain.CarMachine;
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
    /// <summary>
    /// 加盟業者相關
    /// </summary>
    public class OperatorRepository:BaseRepository
    {
        private string _connectionString;
        public OperatorRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }

        public List<BE_Operator> GetOperators(string OperatorAccount, string OperatorName, string StartDate, string EndDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_Operator> lstOperators = null;
            string SQL = "SELECT   [OperatorID],[OperatorAccount],[OperatorICon],[OperatorName],[StartDate],[EndDate] FROM [TB_OperatorBase] ";
            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            int nowCount = 0;
            if (OperatorAccount != "")
            {
                term = " OperatorAccount=@OperatorAccount ";
                para[nowCount] = new SqlParameter("@OperatorAccount", SqlDbType.VarChar, 20);
                para[nowCount].Value = OperatorAccount;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (OperatorName != "")
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += " OperatorName=@OperatorName ";
                para[nowCount] = new SqlParameter("@OperatorName", SqlDbType.NVarChar, 100);
                para[nowCount].Value = OperatorName;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (StartDate != "")
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += " StartDate>=@StartDate ";
                para[nowCount] = new SqlParameter("@StartDate", SqlDbType.DateTime);
                para[nowCount].Value = StartDate+" 00:00:00";
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if (EndDate != "")
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += " EndDate<=@EndDate ";
                para[nowCount] = new SqlParameter("@EndDate", SqlDbType.DateTime);
                para[nowCount].Value = EndDate+" 23:59:59";
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE (" + term + ")";
            }
            SQL += " ORDER BY EndDate ASC";


            lstOperators = GetObjList<BE_Operator>(ref flag, ref lstError, SQL, para, term);
            return lstOperators;
        }
    }
}
