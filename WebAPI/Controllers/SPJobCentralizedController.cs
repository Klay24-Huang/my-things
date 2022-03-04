using Domain.Common;
using Domain.SP.Input.Other;
using Domain.SP.Output;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class SPJobCentralizedController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private CommonFunc baseVerify = new CommonFunc();

        // POST api/<controller>
        public Dictionary<string, object> DoJob(Dictionary<string, object> value)
        {
            logger.Trace("Init");
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "SPJobCentralizedController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            IAPI_SPJobCentralized apiInput = new IAPI_SPJobCentralized();
            NullOutput apiOutput = new NullOutput();

            Token token = null;
            baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            #endregion
            #region 防呆
            //public bool baseCheck(Dictionary<string, object> value, ref string Contentjson, ref string errCode, string funName)
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);

            List<string> allowList = new List<string>()
            {
                "usp_MonthlyRentNotifyPrepare_Q01","usp_EventMillion_Send_U01"
            };

            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_SPJobCentralized>(Contentjson);

                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

            }
            #endregion
            #region TB

            if (flag)
            {
                if (allowList.Any(p => p == apiInput.RunSPName))
                {
                    var RunFlag = RunSP(apiInput.RunSPName, ref lstError, ref errCode);
                    
                }
                else
                {
                    flag = false;
                    errCode = "ERR908";
                }
            }
            #endregion
            
            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }


        
        private bool RunSP(string SP ,ref List<ErrorInfo> lstError, ref string errCode)
        {
            
            string SPName = SP;
            SPOutput_Base spOutBase = new SPOutput_Base();
            SPInput_SPJobCentralized spInput = new SPInput_SPJobCentralized();
            SQLHelper<SPInput_SPJobCentralized, SPOutput_Base> sqlHelper = new SQLHelper<SPInput_SPJobCentralized, SPOutput_Base>(connetStr);

            var flag = sqlHelper.ExecuteSPNonQuery(SPName, spInput, ref spOutBase, ref lstError);

            if (flag == false)
            {
                logger.Trace($"RunSP SP:{SP} | error:{JsonConvert.SerializeObject(lstError)}");
            }
            baseVerify.checkSQLResult(ref flag, spOutBase.Error, spOutBase.ErrorCode, ref lstError, ref errCode);

            return flag;

        }
    }
}