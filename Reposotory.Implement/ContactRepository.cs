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
    /// 合約相關
    /// </summary>
    public class ContactRepository : BaseRepository
    {
        private string _connectionString;
        public ContactRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<BE_OrderHistoryData> GetOrderHistory(Int64 OrderNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_OrderHistoryData> lstOrderHistory= null;


            int nowCount = 0;
            string SQL = "SELECT [OrderNum],[Descript] ,[MKTime] FROM [dbo].[TB_OrderHistory] ";

            SqlParameter[] para = new SqlParameter[10];
            string term = "";

            if (OrderNo >0)
            {
                term += (term == "") ? "" : " AND ";
                term += " OrderNum=@OrderNo";
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WITH(NOLOCK) WHERE " + term + " ORDER BY MKTime ASC;";

            }

            lstOrderHistory = GetObjList<BE_OrderHistoryData>(ref flag, ref lstError, SQL, para, term);

            return lstOrderHistory;
        }
        /// <summary>
        /// 後台訊息記錄查詢用(當條件為合約編號時）
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public BE_GetOrderPartByMessageLogQuery GetOrderPartForMessageLogQuery(Int64 OrderNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetOrderPartByMessageLogQuery> lstOrderPart = null;
            BE_GetOrderPartByMessageLogQuery obj = null;

            int nowCount = 0;
            string SQL = "SELECT OrderMain.[order_number] AS OrderNo,[CarNo],[start_time],[stop_time] ";
            SQL += ",[car_mgt_status],[booking_status],[cancel_status] ";
            SQL += ", ISNULL(OrderDetail.final_start_time, '1911-01-01 00:00:00') AS final_start_time ";
            SQL += ", ISNULL(OrderDetail.final_stop_time, '1911-01-01 00:00:00') AS final_stop_time ";
            SQL += " FROM [dbo].[TB_OrderMain] AS OrderMain WITH(NOLOCK)  ";
            SQL += " LEFT JOIN[dbo].[TB_OrderDetail] AS OrderDetail ON OrderMain.[order_number]= OrderDetail.order_number ";

            SqlParameter[] para = new SqlParameter[10];
            string term = "";

            if (OrderNo > 0)
            {
                term += (term == "") ? "" : " AND ";
                term += " OrderMain.order_number=@OrderNo";
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WHERE " + term + " ORDER BY OrderNo ASC;";

            }

            lstOrderPart = GetObjList<BE_GetOrderPartByMessageLogQuery>(ref flag, ref lstError, SQL, para, term);
            if (lstOrderPart != null)
            {
                if (lstOrderPart.Count > 0)
                {
                    obj = lstOrderPart[0];
                }
            }

            return obj;
        }
        public List<BE_GetBookingQueryForWeb> GetBookingQueryForWeb(Int64 OrderNo, string IDNO, string StationID, string CarNo, string SD, string ED)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetBookingQueryForWeb> lstOrderPart = null;


            int nowCount = 0;
            string SQL = "SELECT OrderNum,IDNO,[CarNo],[car_mgt_status],cancel_status,StationName,SD,ED ";
            SQL += " FROM VW_BE_GetOrderQueryForWeb WITH(NOLOCK)  ";


            SqlParameter[] para = new SqlParameter[10];
            string term = " car_mgt_status<4 ";
            string term2 = "";

            if (OrderNo > 0)
            {
                term += (term == "") ? "" : " AND ";
                term += " OrderNum=@OrderNo";
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (IDNO != "")
            {
                term += (term == "") ? "" : " AND ";
                term += " IDNO=@IDNO";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (StationID != "")
            {
                term += (term == "") ? "" : " AND ";
                term += " StationID=@StationID";
                para[nowCount] = new SqlParameter("@StationID", SqlDbType.VarChar, 20);
                para[nowCount].Value = StationID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (CarNo != "")
            {
                term += (term == "") ? "" : " AND ";
                term += " CarNo=@CarNo";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 20);
                para[nowCount].Value = StationID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (string.IsNullOrEmpty(SD) == false)
            {
                if (string.IsNullOrEmpty(ED) == false)
                {
                    term2 = " AND ((SD between @SD AND @ED) OR (ED between @SD AND @ED))";
                    para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 20);
                    para[nowCount].Value = SD;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                    para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 20);
                    para[nowCount].Value = ED;
                    para[nowCount].Direction = ParameterDirection.Input;
                }
                else
                {
                    term2 = " AND SD >= @SD AND  ED <= @SD";
                    para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 20);
                    para[nowCount].Value = SD;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ED) == false)
                {
                    term2 = " AND SD >= @ED AND  ED <= @ED";
                    para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 20);
                    para[nowCount].Value = SD;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
            }

            if ("" != term)
            {
                SQL += " WHERE " + term;

            }
            if ("" != term2)
            {
                SQL += term2;
            }
            SQL += " ORDER BY OrderNum ASC;";

            lstOrderPart = GetObjList<BE_GetBookingQueryForWeb>(ref flag, ref lstError, SQL, para, term);


            return lstOrderPart;
        }
        /// <summary>
        /// 後台訂單查詢使用
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <param name="IDNO"></param>
        /// <param name="StationID"></param>
        /// <param name="CarNo"></param>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <returns></returns>
        public List<BE_GetOrderQueryForWeb> GetOrderQueryForWeb(Int64 OrderNo, string IDNO, string StationID, string CarNo, string SD, string ED)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetOrderQueryForWeb> lstOrderPart = null;
       

            int nowCount = 0;
            string SQL = "SELECT OrderNum,IDNO,[CarNo],[car_mgt_status],cancel_status,StationName ";
            SQL += " FROM VW_BE_GetOrderQueryForWeb WITH(NOLOCK)  ";


            SqlParameter[] para = new SqlParameter[10];
            string term = " car_mgt_status>=4 ";
            string term2 = "";

            if (OrderNo > 0)
            {
                term += (term == "") ? "" : " AND ";
                term += " OrderNum=@OrderNo";
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (IDNO !="")
            {
                term += (term == "") ? "" : " AND ";
                term += " IDNO=@IDNO";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar,20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (StationID != "")
            {
                term += (term == "") ? "" : " AND ";
                term += " StationID=@StationID";
                para[nowCount] = new SqlParameter("@StationID", SqlDbType.VarChar, 20);
                para[nowCount].Value = StationID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (CarNo != "")
            {
                term += (term == "") ? "" : " AND ";
                term += " CarNo=@CarNo";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 20);
                para[nowCount].Value = StationID;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (string.IsNullOrEmpty(SD) == false)
            {
                if (string.IsNullOrEmpty(ED) == false)
                {
                    term2 = " AND ((SD between @SD AND @ED) OR (ED between @SD AND @ED))";
                    para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 20);
                    para[nowCount].Value = SD;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                    para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 20);
                    para[nowCount].Value = ED;
                    para[nowCount].Direction = ParameterDirection.Input;
                }
                else
                {
                    term2 = " AND SD >= @SD AND  ED <= @SD";
                    para[nowCount] = new SqlParameter("@SD", SqlDbType.VarChar, 20);
                    para[nowCount].Value = SD;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ED) == false)
                {
                    term2 = " AND SD >= @ED AND  ED <= @ED";
                    para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 20);
                    para[nowCount].Value = SD;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
            }

            if ("" != term)
            {
                SQL += " WHERE " + term;

            }
            if ("" != term2)
            {
                SQL += term2;
            }
            SQL+= " ORDER BY OrderNum ASC;";

            lstOrderPart = GetObjList<BE_GetOrderQueryForWeb>(ref flag, ref lstError, SQL, para, term);
  

            return lstOrderPart;
        }
    }
}
