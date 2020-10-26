using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Data;
using NLog;


/// <summary>
/// WebApiClient 的摘要描述
/// </summary>
namespace WebAPI.Utils
{
    public class WebApiClient : WebApiClientBase
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public static DataSet SPRetB(string[] serverName, string spName, object[] parms, ref string message, ref string messageLevel, ref string messageType)
        {

            logger.Debug("serverName:" + JsonConvert.SerializeObject(serverName) +
                ";spName:" + JsonConvert.SerializeObject(spName) +
                ";parms:" + JsonConvert.SerializeObject(parms));
            DataSet ds = GetSSApiSp().SPRetB(
                serverName,
                spName,
                parms,
                ref messageType,
                ref messageLevel,
                ref message
            );
            return ds;
        }
        
        public static DataSet SPExeBatchMultiArr2(string[] serverName, string spName, object[][] parms, bool isTransaction, ref string message, ref string messageLevel, ref string messageType)
        {
            logger.Debug("serverName:" + JsonConvert.SerializeObject(serverName) +
                ";spName:" + JsonConvert.SerializeObject(spName) +
                ";parms:" + JsonConvert.SerializeObject(parms));
            List<Object> tableParms = new List<Object>();
            // 將第一組參數視為一般參數
            for (int i = 0; i < parms[0].Length; i++)
            {
                tableParms.Add(parms[0][i]);
            }
            // 將第二組及之後的參數視為結構化參數
            for (int i = 1; i < parms.Length; i++)
            {
                tableParms.Add(JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(parms[i])));
            }

            DataSet result = GetSSApiSp().SpExeBatchMultiArr(
                serverName,
                spName,
                tableParms.ToArray(),
                isTransaction,
                ref messageType,
                ref messageLevel,
                ref message
            );

            return result;

        }
    }
}