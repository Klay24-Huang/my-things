using Domain.Common;
using Domain.SP.Input.Register;
using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 重發驗證信 20200812 recheck ok need change send mail body
    /// </summary>
    public class ReSendEMailController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string key = ConfigurationManager.AppSettings["apikey"].ToString();
        private string salt = ConfigurationManager.AppSettings["salt"].ToString();
        [HttpPost]
        public Dictionary<string, object> doReSendEMail(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "ReSendEMailController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_ReSendEMail apiInput = null;
            OAPI_Login CheckAccountAPI = null;
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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_ReSendEMail>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.IDNO, apiInput.DeviceID, apiInput.APPVersion, apiInput.MEMEMAIL };
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
            #region 產生加密及發信
            if (flag)
            {
                SendMail send = new SendMail();
                string Source = new AESEncrypt().doEncrypt(key, salt, apiInput.IDNO + "⊙" + apiInput.MEMEMAIL);
                string Title = "iRent會員電子信箱認證通知信";

                string url = "https://irentv2-app-api.irent-ase.p.azurewebsites.net/api/VerifyEMail?VerifyCode=" + Source;

                string Body =
                    "<img src='https://verify.irent-ase.p.azurewebsites.net/images/irent.png'>" +
                    "<p>親愛的會員您好：</p>" +
                    "<p>請點擊下方連結完成電子信箱認證</p>" +
                    "<p><a href='" + url + "'>請按這裡</a></p>" +
                    "<p>不是您本人嗎?請直接忽略或刪除此信件，</p>" +
                    "<p>如有問題請撥打客服專線0800-024-550，謝謝！</p>" +
                    "<img src='https://verify.irent-ase.p.azurewebsites.net/images/hims_logo.png' width='300'>";

                flag = Task.Run(() => send.DoSendMail(Title, Body, apiInput.MEMEMAIL)).Result;
            }

            #endregion
            #region TB
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.ReSendEmail);
                SPInput_ReSendEMail spInput = new SPInput_ReSendEMail()
                {
                    LogID = LogID,
                    IDNO = apiInput.IDNO,
                    DeviceID = apiInput.DeviceID,
                    EMAIL = apiInput.MEMEMAIL
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_ReSendEMail, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_ReSendEMail, SPOutput_Base>(connetStr);
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