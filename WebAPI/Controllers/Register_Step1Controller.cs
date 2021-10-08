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

namespace WebAPI.Controllers
{
    /// <summary>
    /// 註冊步驟一，設定手機，並傳送驗證碼 20200812 recheck
    /// </summary>
    public class Register_Step1Controller : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 註冊帳號、密碼並送出簡訊
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doRegister_Step1(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "Register_Step1Controller";
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

                string[] checkList = { apiInput.IDNO, apiInput.Mobile, apiInput.DeviceID, apiInput.APPVersion };
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
                    "HIMSIRENT2@hotaimotor.com.tw");
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
                    Mode = 0,
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

            //20210928唐加
            #region 若已驗證手機被第二人驗證，發送簡訊通知前一位客人
            if (flag)
            {
                string spName = "usp_CheckMobileUse";
                SPInput_CheckMobile spInput = new SPInput_CheckMobile()
                {
                    Mobile = apiInput.Mobile,
                    LogID = LogID
                };
                SPOutput_CheckMobileUse spOut = new SPOutput_CheckMobileUse();
                SQLHelper<SPInput_CheckMobile, SPOutput_CheckMobileUse> sqlHelp = new SQLHelper<SPInput_CheckMobile, SPOutput_CheckMobileUse>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                //20211007 upd by 唐，企劃又不想發MAIL，要改用簡訊
                //HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                //WebAPIOutput_NPR260Send wsOutput = new WebAPIOutput_NPR260Send();
                //string Message = string.Format("親愛的iRent會員您好:\n" +
                //    "您目前於iRent註冊的手機號碼與其他會員重複，為維護您的權益，請您重新認證手機號碼，\n" +
                //    "若仍有問題，請您來電0800-024-550或line詢問客服，亦可至鄰近門市詢問辦理，謝謝您。\n" +
                //    "※ 本信件為系統自動發送，請勿直接回覆此信件。\n" +
                //    "和雲行動服務股份有限公司 敬上");
                //flag = hiEasyRentAPI.NPR260Send(apiInput.Mobile, Message, "", ref wsOutput);
                if (spOut.mail != "")
                {
                    SendMail("iRent會員異動通知",
                    "<img src='https://irentv2-as-verify.azurewebsites.net/images/irent.png'><br>" +
                    "親愛的iRent會員您好:<br>" +
                    "您目前於iRent註冊的手機號碼與其他會員重複，為維護您的權益，請您重新認證手機號碼，<br>" +
                    "若仍有問題，請您來電0800-024-550或line詢問客服，亦可至鄰近門市詢問辦理，謝謝您。<br>" +
                    "※ 本信件為系統自動發送，請勿直接回覆此信件。<br>" +
                    "和雲行動服務股份有限公司 敬上<br>" +
                    "<img src='https://irentv2-as-verify.azurewebsites.net/images/hims_logo.png' width='300'>",
                    spOut.mail);
                }
            }
            #endregion

            #region TB
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.Register_Step1);
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