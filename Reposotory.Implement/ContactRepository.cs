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
    }
}
