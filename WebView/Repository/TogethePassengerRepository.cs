using Domain.TB.BackEnd;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebCommon;

namespace WebView.Repository
{
    public class TogethePassengerRepository : BaseRepository
    {
        private string _connectionString;
        public TogethePassengerRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }

        public List<BE_GetBookingQueryForWeb> GetBookingStatus(Int64 OrderNo, string IDNO, string StationID, string CarNo, string SD, string ED)
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
            if (StationID != "" && StationID != "all")
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
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (string.IsNullOrEmpty(SD) == false && SD != "")
            {
                if (string.IsNullOrEmpty(ED) == false && ED != "")
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
                if (string.IsNullOrEmpty(ED) == false && ED != "")
                {
                    term2 = " AND SD >= @ED AND  ED <= @ED";
                    para[nowCount] = new SqlParameter("@ED", SqlDbType.VarChar, 20);
                    para[nowCount].Value = ED;
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
    }
}