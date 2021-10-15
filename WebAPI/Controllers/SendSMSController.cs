using Domain.Common;
using Domain.SP.Input.Register;
using Domain.SP.Output;
using Domain.SP.Output.Register;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
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
using Domain.Sync.Input;//20210928唐加
using System.Threading.Tasks;//20210928唐加
using Domain.SP.BE.Input;//20210928唐加

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
            bool flag2 = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "SendSMSController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_SendSMS apiInput = null;
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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_SendSMS>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.IDNO, apiInput.Mobile, apiInput.DeviceID, apiInput.APPVersion };
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
            // 20210222;增加手機黑名單檢查
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.CheckMobile);
                SPInput_CheckMobile spInput = new SPInput_CheckMobile()
                {
                    LogID = LogID,
                    Mobile = apiInput.Mobile,
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_CheckMobile, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CheckMobile, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);

                //20210928唐加，若用黑名單手機註冊要發email給企劃，Adam哥最帥，幫我上版
                if (!flag)//!flag
                {
                    SendMail("【安全性通知】有黑名單手機號碼嘗試驗證", "您好，使用者(" + apiInput.IDNO + ")於(" + DateTime.Now + ")嘗試使用黑名單手機號碼(" + apiInput.Mobile + ")驗證，請密切留意。",
                    "ann420@hotaimotor.com.tw");
                    SendMail("【安全性通知】有黑名單手機號碼嘗試驗證", "您好，使用者(" + apiInput.IDNO + ")於(" + DateTime.Now + ")嘗試使用黑名單手機號碼(" + apiInput.Mobile + ")驗證，請密切留意。",
                    "nancywen@hotaimotor.com.tw");
                    SendMail("【安全性通知】有黑名單手機號碼嘗試驗證", "您好，使用者(" + apiInput.IDNO + ")於(" + DateTime.Now + ")嘗試使用黑名單手機號碼(" + apiInput.Mobile + ")驗證，請密切留意。",
                    "himsirent3@hotaimotor.com.tw");

                    //20211008唐加
                    string spName2 = "usp_BE_InsBlackList2_TEST";
                    SPInput_BE_InsBlackList spInput2 = new SPInput_BE_InsBlackList()
                    {
                        Mode = 0,
                        Mobile = apiInput.Mobile,
                        USERID = apiInput.IDNO,
                        MEMO= ""
                    };
                    SPOutput_Base spOut2 = new SPOutput_Base();
                    SQLHelper<SPInput_BE_InsBlackList, SPOutput_Base> sqlHelp2 = new SQLHelper<SPInput_BE_InsBlackList, SPOutput_Base>(connetStr);
                    flag2 = sqlHelp2.ExecuteSPNonQuery(spName2, spInput2, ref spOut2, ref lstError);
                    baseVerify.checkSQLResult(ref flag2, ref spOut2, ref lstError, ref errCode);
                }         
            }
            #endregion
            #region 發送簡訊
            if (flag)
            {
                // 判斷三分鐘內是否有未驗證的簡訊驗證碼，有的話取DB的驗證碼出來，沒有才隨機取號
                string spName = new ObjType().GetSPName(ObjType.SPType.GetVerifyCode);
                SPInput_GetVerifyCode spInput = new SPInput_GetVerifyCode()
                {
                    IDNO = apiInput.IDNO,
                    Mobile = apiInput.Mobile,
                    Mode = apiInput.Mode.HasValue ? apiInput.Mode.Value : 0,
                    LogID = LogID
                };
                SPOutput_GetVerifyCode spOut = new SPOutput_GetVerifyCode();
                SQLHelper<SPInput_GetVerifyCode, SPOutput_GetVerifyCode> sqlHelp = new SQLHelper<SPInput_GetVerifyCode, SPOutput_GetVerifyCode>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    VerifyCode = spOut.VerifyCode;
                }

                if (string.IsNullOrEmpty(VerifyCode))
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
                SPInput_ReSendSMS spInput = new SPInput_ReSendSMS()
                {
                    IDNO = apiInput.IDNO,
                    Mobile = apiInput.Mobile,
                    DeviceID = apiInput.DeviceID,
                    VerifyCode = VerifyCode,
                    Mode = apiInput.Mode.HasValue ? apiInput.Mode.Value : 0,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_ReSendSMS, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_ReSendSMS, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            }
            #endregion
            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, CheckAccountAPI, token);
            return objOutput;
            #endregion

        }
        public void SendMail(string TITLE, string MEMO, string recevie)
        {
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool flag2 = true;
            int SendFlag = 0;

            SPInput_SYNC_UPDEventMessage SPInput = new SPInput_SYNC_UPDEventMessage()
            {
                AlertID = 80345,
                HasSend = 1,
                Sender = "SendGuid",
                LogID = 0
            };
            SPOutput_Base SPOutput = new SPOutput_Base();

            try
            {
                SendMail send = new SendMail();
                flag2 = Task.Run(() => send.DoSendMail(TITLE, MEMO, recevie)).Result;

                SPInput.SendTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message);
                SendFlag = 1;
                SPInput.HasSend = 2;
            }
            finally
            {
                string SPName = "usp_SYNC_UPDSendAlertMessage";

                flag2 = new SQLHelper<SPInput_SYNC_UPDEventMessage, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                if (flag2 == false)
                {
                    if (SendFlag == 1)
                        SendFlag = 2;   //發送失敗，更新失敗
                    else
                        SendFlag = 3;   //發送成功，更新失敗
                }
            }
        }
    }
}