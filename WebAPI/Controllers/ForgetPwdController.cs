using Domain.Common;
using Domain.SP.Input.Register;
using Domain.SP.Output.Register;
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
    /// <summary>
    /// 忘記密碼 20200812 recheck ok
    /// </summary>
    public class ForgetPwdController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 忘記密碼
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doForgetPWD(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "ForgetPwdController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_ForgetPWD apiInput = null;
            OPAI_ForgetPwd apiOuptut = null;
            OAPI_Login CheckAccountAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SPOutput_ForgetPWD spOut = new SPOutput_ForgetPWD();
            Int16 APPKind = 2;
            string Contentjson = "";
            string VerifyCode = baseVerify.getRand(0, 999999);
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_ForgetPWD>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.IDNO,  apiInput.DeviceID, apiInput.APPVersion };
                string[] errList = { "ERR900", "ERR900", "ERR900" };
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
              
                string spName = new ObjType().GetSPName(ObjType.SPType.ForgetPWD);
                SPInput_ForgetPWD spInput = new SPInput_ForgetPWD()
                {
                    LogID = LogID,
                    IDNO = apiInput.IDNO,
                    DeviceID = apiInput.DeviceID,
                     VerifyCode= VerifyCode
                };
              
                SQLHelper<SPInput_ForgetPWD, SPOutput_ForgetPWD> sqlHelp = new SQLHelper<SPInput_ForgetPWD, SPOutput_ForgetPWD>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (spOut != null && !string.IsNullOrWhiteSpace(spOut.Mobile))
                {
                    apiOuptut = new OPAI_ForgetPwd()
                    {
                        Mobile = spOut.Mobile
                    };
                }

            }
            #endregion
            #region 發送簡訊
            if (flag)
            {
            
                HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                WebAPIOutput_NPR260Send wsOutput = new WebAPIOutput_NPR260Send();
                string Message = string.Format("您的手機驗證碼是：{0}", VerifyCode);
                flag = hiEasyRentAPI.NPR260Send(spOut.Mobile, Message, "", ref wsOutput);

            }
            #endregion
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOuptut, token);
            return objOutput;
            #endregion
        }
    }
}
