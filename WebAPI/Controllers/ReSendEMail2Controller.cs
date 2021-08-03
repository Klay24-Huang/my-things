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
    /// 重發主動取款的mail 20210803 add by 唐瑋祁
    /// </summary>
    public class ReSendEMail2Controller : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string key = ConfigurationManager.AppSettings["apikey"].ToString();
        private string salt = ConfigurationManager.AppSettings["salt"].ToString();
        [HttpPost]
        public Dictionary<string, object> doReSendEMail2(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "ReSendEMail2Controller";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_ReSendEMail2 apiInput = null;
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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_ReSendEMail2>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.MEMEMAIL };
                string[] errList = { "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                
            }
            #endregion
            #region 產生加密及發信
            if (flag)
            {
                SendMail send = new SendMail();
                //string Source = new AESEncrypt().doEncrypt(key, salt, apiInput.IDNO + "⊙" + apiInput.MEMEMAIL);
                string Title = "主動取款";

                //string url = "https://irentcar.azurefd.net/api/VerifyEMail?VerifyCode=" + Source;

                string Body =
                    "<img src='https://irentv2-as-verify.azurewebsites.net/images/irent.png'>" +
                    "<p>測試0</p>" +
                    "<p>測試1</p>" +
                    "<p>測試2</p>" +
                    "<img src='https://irentv2-as-verify.azurewebsites.net/images/hims_logo.png' width='300'>";

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
                    IDNO = "",
                    DeviceID = "",
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