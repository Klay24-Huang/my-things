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
    }
}
