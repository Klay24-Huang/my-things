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
    /// <summary>
    /// 月租相關
    /// </summary>
    public class MonthlyRentRepository : BaseRepository
    {
        private string _connectionString;
        public MonthlyRentRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        /// <summary>
        /// 取出月租剩餘時數及優惠費率
        /// </summary>
        /// <param name="IDNO">身份證</param>
        /// <param name="SD">實際取車日</param>
        /// <param name="ED">實際還車日</param>
        /// <param name="RateType">類型
        /// <para>0:汽車</para>
        /// <para>1:機車</para>
        /// </param>
        /// <returns></returns>
        public List<MonthlyRentData> GetSubscriptionRates(string IDNO, string SD, string ED, int RateType)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<MonthlyRentData> lstMonthlyRent = null;
            string SQL = "SELECT Mode,MonthlyRentId,IDNO,WorkDayHours,HolidayHours,MotoTotalHours,[WorkDayRateForCar],[HoildayRateForCar],[WorkDayRateForMoto],[HoildayRateForMoto],[StartDate],[EndDate]  FROM [TB_MonthlyRent] ";
            SqlParameter[] para = new SqlParameter[4];
            string term = "";
            int nowCount = 0;
            if (false == string.IsNullOrEmpty(IDNO))
            {
                term = " IDNO=@IDNO";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 30);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            if (false == string.IsNullOrEmpty(SD) && false == string.IsNullOrEmpty(ED))
            {
                if (term != "")
                {
                    term += " AND ";
                }
                term += " ((@SD BETWEEN  StartDate AND EndDate) OR (@ED BETWEEN  StartDate AND EndDate))";
                para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 30);
                para[nowCount].Value = SD;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
                para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 30);
                para[nowCount].Value = ED;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                term += " AND  Mode=@RateType";
                para[nowCount] = new SqlParameter("@RateType", SqlDbType.TinyInt, 1);
                para[nowCount].Value = RateType;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WHERE " + term;
            }
            SQL += "  ORDER BY StartDate ASC";

            lstMonthlyRent = GetObjList<MonthlyRentData>(ref flag, ref lstError, SQL, para, term);
            return lstMonthlyRent;
        }
    }
}
