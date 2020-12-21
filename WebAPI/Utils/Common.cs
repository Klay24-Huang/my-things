using System.Data;
using WebAPI.Models.Enum;

namespace WebAPI.Utils
{
    public static class Common
    {
        public static DataSet getBindingList(string IDNO,ref bool flag,ref string errCode,ref string errMsg)
        {
            object[][] parms1 = {
                        new object[] {
                            IDNO
                    }};

            DataSet ds1 = null;
            string returnMessage = "";
            string messageLevel = "";
            string messageType = "";
            string SPName = new ObjType().GetSPName(ObjType.SPType.GetBindingCard);

            ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, false, ref returnMessage, ref messageLevel, ref messageType);

            if (ds1.Tables.Count == 0)
            {
                flag = false;
                errCode = "ERR999";
                errMsg = returnMessage;
            }

            return ds1;
        }
    }
}