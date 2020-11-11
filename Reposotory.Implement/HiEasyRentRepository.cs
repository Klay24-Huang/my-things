using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebCommon;

namespace Reposotory.Implement
{
    public class HiEasyRentRepository : BaseRepository
    {
        private string _connectionString;

        public HiEasyRentRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }

        public BE_BookingControl GetBookingControl(Int64 OrderNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_BookingControl> lstOrder = null;
            BE_BookingControl obj = null;

            int nowCount = 0;
            string SQL = "SELECT * FROM VW_BE_GetBookingControlData ";


            SqlParameter[] para = new SqlParameter[10];
            string term = "";

            if (OrderNo > 0)
            {
                term += (term == "") ? "" : " AND ";
                term += " OrderNo=@OrderNo";
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WHERE " + term + " ORDER BY OrderNo ASC;";

            }

            lstOrder = GetObjList<BE_BookingControl>(ref flag, ref lstError, SQL, para, term);
            if (lstOrder != null)
            {
                if (lstOrder.Count > 0)
                {
                    obj = lstOrder[0];
                }
            }

            return obj;
        }
    }
}
