using Domain.SP.Input.MonthlyRent;
using Domain.SP.Output;
using Domain.TB;
using Reposotory.Implement.Enum;
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
        /// <param name="shortTermIds">短期MonthlyRentIds(可多筆),以逗號分隔</param>
        /// <returns></returns>
        public List<MonthlyRentData> GetSubscriptionRates(string IDNO, string SD, string ED, int RateType, string shortTermIds="")
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<MonthlyRentData> lstMonthlyRent = null;
            string SQL = @"
            SELECT 
            Mode,MonType,MonthlyRentId,MonLvl,IDNO,
            CarFreeType,MotoFreeType,
            CarTotalHours,WorkDayHours,HolidayHours,
            MotoTotalHours,MotoWorkDayMins,MotoHolidayMins,
            WorkDayRateForCar,HoildayRateForCar,
            WorkDayRateForMoto,HoildayRateForMoto,
            StartDate,EndDate
            FROM TB_MonthlyRent ";
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
                term += " ((EndDate > @SD AND EndDate <= @ED) OR (StartDate >= @SD AND StartDate < @ED) OR (StartDate <= @SD AND EndDate >= @ED))";
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

            string shortTermSql = "";
            if(!string.IsNullOrEmpty(shortTermIds) && !string.IsNullOrWhiteSpace(shortTermIds))
                shortTermSql = " OR MonthlyRentId in ("+ shortTermIds +")"; 

            if ("" != term)
            {
                SQL += " WHERE " + term + shortTermSql;
            }
            SQL += "  ORDER BY StartDate ASC";

            lstMonthlyRent = GetObjList<MonthlyRentData>(ref flag, ref lstError, SQL, para, term);
            return lstMonthlyRent;
        }
        /// <summary>
        /// 還原月租記錄
        /// </summary>
        /// <param name="OrderNo">訂單編號</param>
        /// <param name="errCode">錯誤代碼</param>
        /// <param name="IDNO">身份證號</param>
        /// <param name="LogID">執行api的logid</param>
        /// <returns>
        /// <para>true:成功</para>
        /// <para>false:失敗</para>
        /// </returns>
        public bool RestoreHistory(string IDNO,Int64 OrderNo,Int64 LogID,ref string errCode)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SPName=new ObjType().GetSPName(ObjType.SPType.ClearMonthTmpHistory);
            SPInput_ClearMonthlyTmpHistory SPInput = new SPInput_ClearMonthlyTmpHistory()
            {
                IDNO = IDNO,
                LogID = LogID,
                OrderNo = OrderNo
            };
            SPOutput_Base SPOutput = new SPOutput_Base();
            SQLHelper<SPInput_ClearMonthlyTmpHistory, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_ClearMonthlyTmpHistory, SPOutput_Base>(ConnectionString);
            flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
            checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
            return flag;
        }
        public bool InsMonthlyHistory(
            string IDNO, Int64 OrderNo,Int64 MonthlyRentId,
            int UseCarTotalHours, int UseWorkDayMins, int UseHolidayMins,
            int UseMotoTotalMinutes, int UseMotoWorkDayMins, int UseMotoHolidayMins,
            Int64 LogID, ref string errCode)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SPName = new ObjType().GetSPName(ObjType.SPType.InsMonthHistory);
            SPInput_InsMonthlyHistory SPInput = new SPInput_InsMonthlyHistory()
            {
                IDNO = IDNO,
                OrderNo = OrderNo,
                MonthlyRentId = MonthlyRentId,
                UseCarTotalHours = UseCarTotalHours,
                UseWorkDayHours = UseWorkDayMins,
                UseHolidayHours = UseHolidayMins,
                UseMotoTotalHours = UseMotoTotalMinutes,        //2021128 ADD BY ADAM 
                UseMotoWorkDayMins = UseMotoWorkDayMins,
                UseMotoHolidayMins = UseMotoHolidayMins,
                LogID = LogID,
            };
            SPOutput_Base SPOutput = new SPOutput_Base();
            SQLHelper<SPInput_InsMonthlyHistory, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_InsMonthlyHistory, SPOutput_Base>(ConnectionString);
            flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
            checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
            return flag;
        }
        /// <summary>
        /// 驗證SP回傳值
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="Error"></param>
        /// <param name="ErrorCode"></param>
        /// <param name="lstError"></param>
        /// <param name="errCode"></param>
        private void checkSQLResult(ref bool flag, int Error, string ErrorCode, ref List<ErrorInfo> lstError, ref string errCode)
        {
            if (flag)
            {
                if (Error == 1)
                {
                    lstError.Add(new ErrorInfo() { ErrorCode = ErrorCode });
                    errCode = ErrorCode;
                    flag = false;
                }
                else
                {
                    if (ErrorCode != "0000")
                    {
                        lstError.Add(new ErrorInfo() { ErrorCode = ErrorCode });
                        errCode = ErrorCode;
                        flag = false;
                    }
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
        }
    }
}
