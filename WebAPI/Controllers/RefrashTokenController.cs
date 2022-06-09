using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 更新Token
    /// </summary>
    public class RefrashTokenController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string android = ConfigurationManager.AppSettings.Get("android");
        private string ios = ConfigurationManager.AppSettings.Get("ios");
        private int Rxpires_in = (ConfigurationManager.AppSettings.Get("Rxpires_in") == null) ? 1800 : Convert.ToInt32(ConfigurationManager.AppSettings.Get("Rxpires_in").ToString());
        private int Refrash_Rxpires_in = (ConfigurationManager.AppSettings.Get("Refrash_Rxpires_in") == null) ? 1800 : Convert.ToInt32(ConfigurationManager.AppSettings.Get("Refrash_Rxpires_in").ToString());

        [HttpPost]
        public Dictionary<string, object> doMemberLogin(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "RefrashTokenController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_RefrashToken apiInput = null;
            OAPI_RefrashToken apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_RefrashToken>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.IDNO, apiInput.RefrashToken, apiInput.DeviceID, apiInput.APPVersion };
                string[] errList = { "ERR900", "ERR900", "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (flag)
                {
                    //2.判斷格式
                    flag = baseVerify.checkIDNO(apiInput.IDNO);
                    if (false == flag)
                    {
                        errCode = "ERR103";
                    }
                }
                if (flag)
                {
                    if (apiInput.APPVersion.Split('.').Count() < 3)
                    {
                        flag = false;
                        errCode = "ERR104";
                    }
                }
                if (flag)
                {
                    flag = (apiInput.APP.HasValue);
                    if (false == flag)
                    {
                        errCode = "ERR900";
                    }
                    else
                    {
                        APPKind = apiInput.APP.Value;
                        if (APPKind < 0 || APPKind > 1)
                        {
                            flag = false;
                            errCode = "ERR105";
                        }
                    }
                }
            }
            #endregion
            #region TB
            if (flag)
            {
                //string spName = new ObjType().GetSPName(ObjType.SPType.RefrashToken);
                string spName = "usp_RefrashToken_V20220527";
                SPInput_RefrashToken spInput = new SPInput_RefrashToken()
                {
                    APP = APPKind,
                    APPVersion = apiInput.APPVersion,
                    DeviceID = apiInput.DeviceID,
                    LogID = LogID,
                    IDNO = apiInput.IDNO,
                    RefrashToken = apiInput.RefrashToken,
                    Rxpires_in = Rxpires_in,
                    Refrash_Rxpires_in = Refrash_Rxpires_in,
                    PushREGID = apiInput.PushREGID
                };
                SPOutput_RefrashToken spOut = new SPOutput_RefrashToken();
                SQLHelper<SPInput_RefrashToken, SPOutput_RefrashToken> sqlHelp = new SQLHelper<SPInput_RefrashToken, SPOutput_RefrashToken>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    token = new Token()
                    {
                        Access_token = spOut.Access_Token,
                        Refrash_token = spOut.Refrash_Token,
                        Rxpires_in = Rxpires_in,
                        Refrash_Rxpires_in = Refrash_Rxpires_in
                    };

                    apiOutput = new OAPI_RefrashToken()
                    {
                        Token = token,
                        MEMRFNBR = "ir"+spOut.MEMRFNBR 
                    };
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, null);
            return objOutput;
            #endregion
        }
    }
}