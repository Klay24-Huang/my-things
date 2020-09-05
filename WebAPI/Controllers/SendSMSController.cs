using Domain.Common;
using Domain.SP.Input.Register;
using Domain.SP.Output;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    public class SendSMSController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 註冊重新發送簡訊  20200812 rechek ok
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doSendSMS(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "SendSMSController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_Register_Step1 apiInput = null;
            OAPI_Login CheckAccountAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string VerifyCode = "";
            string Contentjson = "";
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_Register_Step1>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.IDNO,  apiInput.Mobile, apiInput.DeviceID, apiInput.APPVersion };
                string[] errList = { "ERR900", "ERR900", "ERR900", "ERR900", "ERR900" };
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
                    //2.1判斷手機格式
                    if (flag)
                    {
                        flag = baseVerify.regexStr(apiInput.Mobile, CommonFunc.CheckType.Mobile);
                        if (flag == false)
                        {
                            errCode = "ERR106";
                        }
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
            #region 發送簡訊
            if (flag)
            {
                VerifyCode = baseVerify.getRand(0, 999999);
                HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                WebAPIOutput_NPR260Send wsOutput = new WebAPIOutput_NPR260Send();
                string Message = string.Format("您的手機驗證碼是：{0}", VerifyCode);
                flag = hiEasyRentAPI.NPR260Send(apiInput.Mobile, Message, "", ref wsOutput);

            }
            #endregion
            #region TB
            if (flag)
            {

                string spName = new ObjType().GetSPName(ObjType.SPType.RegisterReSendSMS);
                SPInput_Register_Step1 spInput = new SPInput_Register_Step1()
                {
                    LogID = LogID,
                    IDNO = apiInput.IDNO,
                    Mobile = apiInput.Mobile,
                    DeviceID = apiInput.DeviceID,
                    VerifyCode = VerifyCode
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_Register_Step1, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_Register_Step1, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            }
            #endregion
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, CheckAccountAPI, token);
            return objOutput;
            #endregion
        }
    }
}
