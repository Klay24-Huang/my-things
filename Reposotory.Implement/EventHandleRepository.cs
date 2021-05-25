using Domain.TB.BackEnd;
using Domain.TB.Sync;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebCommon;

namespace Reposotory.Implement
{
    public class EventHandleRepository : BaseRepository
    {
        private string _connectionString;
        public EventHandleRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }

        /// <summary>
        /// 取得已發告警信件的最後一筆發送時間
        /// </summary>
        /// <returns></returns>
        public Sync_SendEventMessage GetLastSendData()
        {
            Sync_SendEventMessage Result = new Sync_SendEventMessage();
            List<Sync_SendEventMessage> lstEVMessage = new List<Sync_SendEventMessage>();
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT TOP 1 AlertID,EventType,Receiver,CarNo,HasSend,SendTime,MKTime FROM TB_AlertMailLog WITH(NOLOCK) WHERE HasSend=1 ORDER BY SendTime DESC";
            string term = "";

            SqlParameter[] para = new SqlParameter[3];
            lstEVMessage = GetObjList<Sync_SendEventMessage>(ref flag, ref lstError, SQL, para, term);
            if (lstEVMessage != null && lstEVMessage.Count > 0)
            {
                Result = lstEVMessage[0];
            }
            return Result;
        }

        /// <summary>
        /// 取得尚未發送告警MAIL的資料
        /// </summary>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public List<Sync_SendEventMessage> GetEventMessages(string SDate, string EDate)
        {
            List<Sync_SendEventMessage> lstEVMessage = new List<Sync_SendEventMessage>();
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            SqlParameter[] para = new SqlParameter[0];
            string SQL = " EXEC usp_GetAlertMailLog '" + SDate + "','" + EDate + "'";
            string term = "";

            lstEVMessage = GetObjList<Sync_SendEventMessage>(ref flag, ref lstError, SQL, para, term);

            return lstEVMessage;
        }

        /// <summary>
        /// 取得特定車號的告警事件最後一筆發放紀錄
        /// </summary>
        /// <param name="CarNo"></param>
        /// <param name="EventType"></param>
        /// <returns></returns>
        public List<Sync_SendEventMessage> GetHasSendMailList(string CarNo,int EventType)
        {
            List<Sync_SendEventMessage> ResultList = new List<Sync_SendEventMessage>();
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT TOP 1 AlertID,EventType,Receiver,CarNo,HasSend,SendTime,MKTime FROM TB_AlertMailLog WITH(NOLOCK) WHERE HasSend=1 AND SendTime is not null ";
            int nowCount = 0;
            string term = "";

            SqlParameter[] para = new SqlParameter[2];
            if (!string.IsNullOrEmpty(CarNo))
            {
                term += " AND CarNo=@CarNo ";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 10);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (EventType != 0)
            {
                if (EventType == 1 || EventType == 9 || EventType == 8) //1:沒租約但是有時速 9:無租約但引擎被發動 8:無租約但電門被啟動
                {
                    term += " AND EventType in (1, 9, 8) ";
                }
                else
                {
                    term += " AND EventType=@EventType ";
                    para[nowCount] = new SqlParameter("@EventType", SqlDbType.Int);
                    para[nowCount].Value = EventType;
                    para[nowCount].Direction = ParameterDirection.Input;
                    nowCount++;
                }
            }
            SQL += term;
            SQL += " ORDER BY SendTime DESC";

            ResultList = GetObjList<Sync_SendEventMessage>(ref flag, ref lstError, SQL, para, term);
            return ResultList;
        }

        /// <summary>
        /// 取得訂單號碼
        /// </summary>
        /// <param name="CarNo"></param>
        /// <param name="SDate"></param>
        /// <param name="EDate"></param>
        /// <returns></returns>
        public Sync_OrderMain GetOrderNumberData(string CarNo, string SDate, string EDate)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<Sync_OrderMain> ListData = new List<Sync_OrderMain>();
            Sync_OrderMain Result = new Sync_OrderMain();

            string SQL = "SELECT TOP 1 ISNULL(order_number,0) AS OrderNumber FROM TB_OrderMain WITH(NOLOCK) WHERE (car_mgt_status>0 AND car_mgt_status<16) AND cancel_status=0 ";
            int nowCount = 0;
            string term = "";

            SqlParameter[] para = new SqlParameter[3];
            if (!string.IsNullOrEmpty(CarNo))
            {
                term += " AND CarNo=@CarNo ";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 10);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (!string.IsNullOrEmpty(SDate) && !string.IsNullOrEmpty(EDate))
            {
                term += " AND start_time BETWEEN @SDate AND @EDate ";
                para[nowCount] = new SqlParameter("@SDate", SqlDbType.VarChar, 30);
                para[nowCount].Value = SDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;

                para[nowCount] = new SqlParameter("@EDate", SqlDbType.VarChar, 30);
                para[nowCount].Value = EDate;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }

            SQL += term;

            ListData = GetObjList<Sync_OrderMain>(ref flag, ref lstError, SQL, para, term);
            if (ListData != null && ListData.Count>0)
            {
                Result = ListData[0];
            }

            return Result;
        }

        public List<BE_EvTimeLine> GetMapDataByTimeLine(string CarNo, string SD, string ED)
        {
            List<BE_EvTimeLine> lstEV = new List<BE_EvTimeLine>();
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string SQL = " SELECT  ET.[CarNo],[FactoryYear],[CCNum],[GPSStatus],[Speed],[Latitude],[Longitude] ";
            SQL += "   ,[GPSTime],ISNULL([OrderNo],0) AS CarStatus  ";
            SQL += "  FROM VW_BE_GetEVTimeLine AS ET ";
            SQL += "  LEFT JOIN VW_BE_GetOrderForTimeLine AS OT ON ET.CarNo=OT.CarNo AND ((FS>=GPSTime AND FE<=GPSTime) OR (FS<=GPSTime AND FE>=GPSTime) ) ";
            int nowCount = 0;
            string term = "";

            SqlParameter[] para = new SqlParameter[3];
            if (CarNo != "")
            {
                term += "  ET.CarNo=@CarNo ";
                para[nowCount] = new SqlParameter("@CarNo", SqlDbType.VarChar, 10);
                para[nowCount].Value = CarNo;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (SD != "" && ED != "")
            {
                if (term != "") { term += " AND "; }
                term += " GPSTime BETWEEN @SD AND @ED ";
                para[nowCount] = new SqlParameter("@SD", SqlDbType.DateTime);
                para[nowCount].Value = SD;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
                para[nowCount] = new SqlParameter("@ED", SqlDbType.DateTime);
                para[nowCount].Value = ED;
                para[nowCount].Direction = ParameterDirection.Input;
                nowCount++;
            }
            if (term != "")
            {
                SQL += " Where " + term;
            }
            SQL += "  ORDER BY GPSTime ASC";

            lstEV = GetObjList<BE_EvTimeLine>(ref flag, ref lstError, SQL, para, term);
            return lstEV;
        }

        public List<BE_EvTimeLine> GetMapDataByTimeLine(Int64? OrderNo)
        {
            List<BE_EvTimeLine> lstEV = new List<BE_EvTimeLine>();
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            //string SQL = " SELECT  ET.[CarNo],[FactoryYear],[CCNum],[GPSStatus],[Speed],[Latitude],[Longitude] ";
            //SQL += "  ,[GPSTime],ISNULL([OrderNo],0) AS CarStatus  ";
            //SQL += "  FROM   VW_BE_GetOrderForTimeLine AS OT ";
            //SQL += "  INNER JOIN VW_BE_GetEVTimeLine AS ET ON ET.CarNo=OT.CarNo AND GPSTime BETWEEN FS AND FE ";
            //int nowCount = 0;
            //string term = "";

            //SqlParameter[] para = new SqlParameter[3];
            //if (OrderNo != null)
            //{
            //    Int64 OrderNum = Convert.ToInt64(OrderNo);
            //    if (OrderNum > 0)
            //    {
            //        term += "  OrderNo=@OrderNo ";
            //        para[nowCount] = new SqlParameter("@OrderNo", SqlDbType.BigInt);
            //        para[nowCount].Value = OrderNum;
            //        para[nowCount].Direction = ParameterDirection.Input;
            //    }

            //}
            //if (term != "")
            //{
            //    SQL += " Where " + term;
            //}
            //SQL += "  ORDER BY GPSTime ASC";

            SqlParameter[] para = new SqlParameter[0];
            string SQL = " EXEC usp_BE_GetMapDataByTimeLine '"+ OrderNo.ToString() + "'";
            string term = "";

            lstEV = GetObjList<BE_EvTimeLine>(ref flag, ref lstError, SQL, para, term);
            return lstEV;
        }



        //20210408唐加
        public List<BE_MapList> GetMapList(int mode, Int64? OrderNo, string carno, string start, string end)
        {
            List<BE_MapList> lstEV = new List<BE_MapList>();
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
       

            SqlParameter[] para = new SqlParameter[0];
            string SQL = " EXEC usp_BE_GetMapList '" + mode.ToString() + "'" + ",'" + OrderNo.ToString() + "'" + ",'" + carno + "'" + ",'" + start + "'" + ",'" + end + "'";
            string term = "";

            lstEV = GetObjList<BE_MapList>(ref flag, ref lstError, SQL, para, term);
            return lstEV;
        }

    }
}
