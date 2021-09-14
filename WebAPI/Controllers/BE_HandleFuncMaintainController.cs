using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.Output;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using NLog;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】功能權限設定
    /// </summary>
    public class BE_HandleFuncMaintainController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        protected static Logger logger = LogManager.GetCurrentClassLogger();//20210907唐加，記錄每支功能使用
        /// <summary>
        /// 【後台】功能權限設定
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_HandleFuncMaintain(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BE_HandleFuncMaintainController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_HandleFuncMaintain apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            DateTime StartDate = DateTime.Now, EndDate = DateTime.Now;
            string IDNO = ""; bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_HandleFuncMaintain>(Contentjson);

                //20210907唐加，記錄每支功能使用
                logger.Trace(
                    "{ReportName:'程式使用紀錄'," +
                    "LogID" + ":'" + LogID + "'," +
                    "UserID" + ":'" + apiInput.UserID + "'," +
                    "BE_ControllerName" + ":'" + "BE_HandleFuncMaintainController" + "'," +
                    "IPAddr" + ":'" + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] + "'," +
                    "FuncGroupID" + ":'" + Convert.ToInt32(apiInput.FuncGroupID) + "'," +
                    "Mode" + ":'" + apiInput.Mode + "'," +
                    "Power" + ":'" + JsonConvert.SerializeObject(apiInput.Power) + "}"
                    );
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.FuncGroupID,apiInput.Mode };
                string[] errList = { "ERR900", "ERR900","ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);

                if (flag)
                {
                    if (apiInput.Power.Count < 1)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }     
                }
            }
            #endregion

            #region TB
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.BE_HandleFunc);
                SPInput_BE_HandleFunc spInput = new SPInput_BE_HandleFunc()
                {
                    LogID = LogID,
                    UserID = apiInput.UserID,
                    FuncGroupID=Convert.ToInt32(apiInput.FuncGroupID),
                    Mode=apiInput.Mode,
                    Power=JsonConvert.SerializeObject(apiInput.Power)
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_BE_HandleFunc, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_HandleFunc, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}
