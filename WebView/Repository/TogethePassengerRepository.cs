using Domain.TB.BackEnd;
using Newtonsoft.Json;
using NLog;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using WebCommon;
using WebView.Models.Param.TogetherPassenger;

namespace WebView.Repository
{
    public class TogethePassengerRepository : BaseRepository
    {
        private string _connectionString;
        public TogethePassengerRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }

        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public List<BE_GetBookingQueryForWeb> GetBookingStatus(string OrderNo, string IDNO, string StationID, string CarNo, string SD, string ED)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<BE_GetBookingQueryForWeb> lstOrderPart = null;


            int nowCount = 0;
            string SQL = "SELECT OrderNum,IDNO,[CarNo],[car_mgt_status],cancel_status,StationName,SD,ED ";
            SQL += " FROM VW_BE_GetOrderQueryForWeb WITH(NOLOCK)  ";


            SqlParameter[] para = new SqlParameter[10];
            string term = "";
            string term2 = "";

            if (OrderNo != "")
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

        public List<OrderDetail> GetOrderInfo(string OrderNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT order_number,IDNO,MEMCNAME,MEMTEL";
            SQL += " FROM TB_OrderMain M WITH(NOLOCK) ";
            SQL += " LEFT JOIN TB_MemberData MEM ON M.IDNO = MEM.MEMIDNO ";


            int nowCount = 0;
            SqlParameter[] para = new SqlParameter[1];

            if (OrderNo != "")
            {
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.VarChar, 20);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
            }

            string term = $" WHERE order_number = @OrderNo";
            SQL += term;

            var orderInfo = GetObjList<OrderDetail>(ref flag, ref lstError, SQL, para, term);

            return orderInfo;
        }

        public string CheckInvitingStatus(string OrderNo, string IDNO)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string term = "";
            string SQL = "SELECT TOP 1 ChkType";
            SQL += " FROM TB_TogetherPassenger WITH(NOLOCK) ";

            int nowCount = 0;
            SqlParameter[] para = new SqlParameter[2];

            if (OrderNo != "")
            {
                para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.VarChar, 20);
                para[nowCount].Value = OrderNo;
                para[nowCount].Direction = ParameterDirection.Input;
                term += (term == "") ? "" : " AND ";
                term += " WHERE Order_number = @OrderNo";
                nowCount++;
            }

            if (IDNO != "")
            {
                para[nowCount] = new SqlParameter("@IDNO", SqlDbType.VarChar, 20);
                para[nowCount].Value = IDNO;
                para[nowCount].Direction = ParameterDirection.Input;
                term += (term == "") ? "" : " AND ";
                term += " MEMIDNO = @IDNO ";
            }

            SQL += term;
            var result = GetObjList<CheckInvitingStatus>(ref flag, ref lstError, SQL, para, term)[0];

            return result.ChkType;
        }

        public Error SaveInviteeResponse(string AESEncryptString)
        {

            using (HttpClient client = new HttpClient())
            {
                string url = ConfigurationManager.AppSettings["AppHost"] + "JointRentIviteeFeedBack";
                client.BaseAddress = new Uri(url);
                var tmpClass = new
                {
                    AESEncryptString = AESEncryptString
                };

                var json = JsonConvert.SerializeObject(tmpClass);

                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = new HttpResponseMessage();

                logger.Info("準備發送請求");
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                    response = client.PostAsync(url, content).GetAwaiter().GetResult();
                }
                catch(Exception ex)
                {
                    logger.Info(ex.Message);
                    logger.Info(ex);
                    logger.Info(ex.InnerException.Message);

                }

                logger.Info("請求完成");

                var result = JsonConvert.DeserializeObject<Error>(response.Content.ReadAsStringAsync().Result);

                return result;
            }
        }
    }
}
