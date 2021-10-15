using Domain.Common;
using Domain.SP.Input.Register;
using Domain.SP.Output;
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
using Domain.SP.Output.Register;//20210928唐加
using System.Threading.Tasks;//20210928唐加


namespace WebAPI.Controllers
{
    /// <summary>
    /// 驗證簡訊驗證碼 20200812 recheck ok
    /// </summary>
    public class VerifySMSController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 驗證簡訊驗證碼
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doVerifySMS(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "VerifySMSController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_VerifySMS apiInput = null;
            OAPI_Login CheckAccountAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            Int16 Mode = 0;
            // string VerifyCode = "";
            string Contentjson = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_VerifySMS>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.IDNO, apiInput.VerifyCode, apiInput.DeviceID, apiInput.APPVersion };
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
                    flag = apiInput.Mode.HasValue;
                    if (flag)
                    {
                        flag = Int16.TryParse(apiInput.Mode.Value.ToString(), out Mode);
                        if (flag)
                        {
                            if (Mode < 0 || Mode > 3)
                            {
                                flag = false;
                                errCode = "ERR144";
                            }
                        }
                        else
                        {
                            errCode = "ERR144";
                        }
                    }
                    else
                    {
                        errCode = "ERR143";
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

            //20210928唐加
            #region 若已驗證手機被第二人驗證，發送簡訊通知前一位客人
            if (flag)
            {
                string spName = "usp_CheckMobileUse";
                SPInput_CheckMobile spInput = new SPInput_CheckMobile()
                {
                    Mobile = apiInput.IDNO,
                    LogID = LogID
                };
                SPOutput_CheckMobileUse spOut = new SPOutput_CheckMobileUse();
                SQLHelper<SPInput_CheckMobile, SPOutput_CheckMobileUse> sqlHelp = new SQLHelper<SPInput_CheckMobile, SPOutput_CheckMobileUse>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

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
                    "您目前於iRent註冊的手機號碼與其他會員重複，請您重新認證手機號碼，若仍有問題，請點選app的 [ 聯絡我們 ] 以聯絡客服，亦可至鄰近和運租車門市詢問辦理，謝謝您。<br><br>" +
                    "※ 本信件為系統自動發送，請勿直接回覆此信件。<br><br>" +
                    "和雲行動服務股份有限公司 敬上<br>" +
                    "<img src='https://irentv2-as-verify.azurewebsites.net/images/hims_logo.png' width='300'>",
                    spOut.mail);
                }
            }
            #endregion

            #region TB
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.CheckVerifyCode);
                SPInput_CheckVerifyCode spInput = new SPInput_CheckVerifyCode()
                {
                    LogID = LogID,
                    IDNO = apiInput.IDNO,
                    Mode = Mode,
                    OrderNum = "",
                    DeviceID = apiInput.DeviceID,
                    VerifyCode = apiInput.VerifyCode
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_CheckVerifyCode, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CheckVerifyCode, SPOutput_Base>(connetStr);
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