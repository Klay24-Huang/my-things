using Domain.TB.Sync;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommon;

namespace Reposotory.Implement.Sync
{
   public class SendMsgRepository:BaseRepository
    {
        private string _connectionString;

        public SendMsgRepository(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<Sync_SendMessage> GetMessage()
        {
            bool flag = true;
            List<Sync_SendMessage> lstMessage = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = "SELECT NotificationID,NType,UserToken,STime,[Message],url FROM TB_PersonNotification  ";

            SqlParameter[] para = new SqlParameter[2];
            string term = "";


            if (flag)
            {


                term = "  isSend IN (0,2)   AND PushTime IS NULL  AND STime<=DATEADD(HOUR,8,GETDATE()) AND ((STime>= DATEADD(minute,-15,DATEADD(HOUR,8,GETDATE()) ) AND NType>0 AND UserToken<>'') OR NType=0) ";
                if ("" != term)
                {
                    SQL += " WHERE " + term;
                }
                SQL += "  ORDER BY OrderNum ASC";

                lstMessage = GetObjList<Sync_SendMessage>(ref flag, ref lstError, SQL, para, term);
            }

            return lstMessage;
        }
    }
}
