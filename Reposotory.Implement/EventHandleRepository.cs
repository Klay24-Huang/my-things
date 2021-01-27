using Domain.TB.BackEnd;
using Domain.TB.Sync;
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
    public class EventHandleRepository : BaseRepository
    {
        private string _connectionString;
        public EventHandleRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<Sync_SendEventMessage> GetEventMessages()
        {
            List<Sync_SendEventMessage> lstEVMessage = new List<Sync_SendEventMessage>();
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT AlertID,EventType,Receiver,CarNo FROM TB_AlertMailLog WITH(NOLOCK) WHERE HasSend=0 ORDER BY AlertID DESC";
            int nowCount = 0;
            string term = "";

            SqlParameter[] para = new SqlParameter[3];
            lstEVMessage = GetObjList<Sync_SendEventMessage>(ref flag, ref lstError, SQL, para, term);
            return lstEVMessage;
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
    }
}
