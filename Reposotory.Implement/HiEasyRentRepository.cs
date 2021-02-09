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
        public BE_LandControl GetLandControl(Int64 OrderNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_LandControl> lstOrder = null;
            BE_LandControl obj = null;

            int nowCount = 0;
            string SQL = "SELECT * FROM VW_BE_GetLandControl ";


            SqlParameter[] para = new SqlParameter[10];
            string term = "";

            if (OrderNo > 0)
            {
                term += (term == "") ? "" : " AND ";
                term += " IRENTORDNO=@OrderNo";
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WHERE " + term + " ORDER BY IRENTORDNO ASC;";

            }

            lstOrder = GetObjList<BE_LandControl>(ref flag, ref lstError, SQL, para, term);
            if (lstOrder != null)
            {
                if (lstOrder.Count > 0)
                {
                    obj = lstOrder[0];
                }
            }

            return obj;
        }
        public BE_ReturnControl GetReturnControl(Int64 OrderNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_ReturnControl> lstOrder = null;
            BE_ReturnControl obj = null;

            int nowCount = 0;
            string SQL = "SELECT * FROM VW_BE_GetReturnControl ";


            SqlParameter[] para = new SqlParameter[10];
            string term = "";

            if (OrderNo > 0)
            {
                term += (term == "") ? "" : " AND ";
                term += " IRENTORDNO=@OrderNo";
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WHERE " + term + " ORDER BY IRENTORDNO ASC;";

            }

            lstOrder = GetObjList<BE_ReturnControl>(ref flag, ref lstError, SQL, para, term);
            if (lstOrder != null)
            {
                if (lstOrder.Count > 0)
                {
                    obj = lstOrder[0];
                }
            }

            return obj;
        }
        public List<BE_NPR340Retry> GetNPR340RetryByID(string IDNO)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_NPR340Retry> lstNPR340 = null;
         

            int nowCount = 0;
            string SQL = "SELECT * FROM VW_GetNPR340Retry ";


            SqlParameter[] para = new SqlParameter[10];
            string term = "";

            if (!string.IsNullOrEmpty(IDNO))
            {
                term += (term == "") ? "" : " AND ";
                term += " CUSTID=@IDNO";
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar,20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WHERE " + term + " ORDER BY CUSTID ASC,ServerTradeNo ASC;";

            }

            lstNPR340 = GetObjList<BE_NPR340Retry>(ref flag, ref lstError, SQL, para, term);
         

            return lstNPR340;
        }
        public BE_NPR136Retry GetNPR136RetryByOrderNo(Int64 OrderNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_NPR136Retry> lstNPR136 = null;
            BE_NPR136Retry obj = null;

            int nowCount = 0;
            string SQL = "SELECT * FROM VW_BE_GETNPR136Data ";


            SqlParameter[] para = new SqlParameter[10];
            string term = "";

            if (OrderNo>0)
            {
                term += (term == "") ? "" : " AND ";
                term += " IRENTORDNO=@OrderNo";
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WHERE " + term ;

            }

            lstNPR136 = GetObjList<BE_NPR136Retry>(ref flag, ref lstError, SQL, para, term);
            if (lstNPR136 != null)
            {
                if (lstNPR136.Count > 0)
                {
                    obj = new BE_NPR136Retry();
                    obj = lstNPR136[0];
                }
            }


            return obj;
        }
        public List<BE_NPR136RetryNew> GetNPR136RetryByOrderNoNew(Int64 OrderNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_NPR136RetryNew> lstNPR136 = null;
            BE_NPR136RetryNew obj = null;

            int nowCount = 0;
            string SQL = "SELECT VW.*,ISNULL(TradeVW.ArrearTaishinTradeNo,'') AS ArrearTaishinTradeNo FROM VW_BE_GETNPR136DataV2 AS VW ";
                   SQL +=" LEFT JOIN VW_NPR330QueryTrade AS TradeVW ON TradeVW.ArrearOrder = VW.IRENTORDNO ";


            SqlParameter[] para = new SqlParameter[10];
            string term = "";

            if (OrderNo > 0)
            {
                term += (term == "") ? "" : " AND ";
                term += " IRENTORDNO=@OrderNo";
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }


            if ("" != term)
            {
                SQL += " WHERE " + term;

            }

            //20210208 ADD BY ADAM REASON.
            lstNPR136 = GetObjList<BE_NPR136RetryNew>(ref flag, ref lstError, SQL, para, term);
            if (lstNPR136 != null)
            {
                if (lstNPR136.Count > 0)
                {
                    obj = new BE_NPR136RetryNew();
                    obj = lstNPR136[0];
                }
            }

            return lstNPR136;
        }
    }
}
