using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Newtonsoft.Json;
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
    public class CheckAppVersionController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoCheckAppVersion(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "CheckAppVersionController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_CheckAppVersion apiInput = null;
            OAPI_CheckAppVersion apiOutput = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_CheckAppVersion>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.DeviceID, apiInput.APPVersion };
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
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
                string spName = new ObjType().GetSPName(ObjType.SPType.CheckAppVersion);
                SPInput_CheckAppVersion spInput = new SPInput_CheckAppVersion()
                {
                    DeviceID = apiInput.DeviceID,
                    APP = APPKind,
                    APPVersion = apiInput.APPVersion,
                    LogID = LogID,
                };
                SPOutput_CheckAppVersion spOut = new SPOutput_CheckAppVersion();
                SQLHelper<SPInput_CheckAppVersion, SPOutput_CheckAppVersion> sqlHelp = new SQLHelper<SPInput_CheckAppVersion, SPOutput_CheckAppVersion>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    apiOutput = new OAPI_CheckAppVersion()
                    {
                        MandatoryUPD = spOut.MandatoryUPD
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