using Domain.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得Server時間
    /// </summary>
    public class GetServerTimeController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpGet]
        public Dictionary<string, object> doGetServerTime()
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetServerTimeController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            OAPI_GetServerTime OutputAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
    
            #endregion
            #region 防呆

            if (flag)
            {

                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("no Input", ClientIP, funName, ref errCode, ref LogID);

            }
            #endregion

            #region TB
            if (flag)
            {
                // lstOut = new List<CityData>();
                try
                {
                    OutputAPI = new OAPI_GetServerTime()
                    {
                        ServerTime = DateTime.Now.ToString("yyyyMMddHHmmss")
                    };
                }
                catch (Exception ex)
                {
                    flag = false;
                    errMsg = ex.Message;
                }

            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, OutputAPI, token);
            return objOutput;
            #endregion
        }
    }
}
