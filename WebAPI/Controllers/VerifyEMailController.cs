using Domain.Common;
using Domain.SP.Input.Register;
using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 驗證email
    /// </summary>
    public class VerifyEMailController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string key = ConfigurationManager.AppSettings["apikey"].ToString();
        private string salt = ConfigurationManager.AppSettings["salt"].ToString();
        [HttpGet]
        public Dictionary<string, object> doVerifyEMail(string VerifyCode)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "VerifyEMailController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            OAPI_Login CheckAccountAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            string EMail = "";
            #endregion
            #region 防呆
            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(string.Format("VerifyCode={0}", VerifyCode), ClientIP, funName, ref errCode, ref LogID);

                if (flag)
                {
                    //解碼
                    string Source = new AESEncrypt().doDecrypt(key, salt, VerifyCode);
                    if (Source != "")
                    {
                        IDNO = Source.Split('⊙')[0];
                        EMail = Source.Split('⊙')[1];
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR140";
                    }
                }
            }
            #endregion
            #region TB
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.VerifyEMail);
                SPInput_VerifyEMail spInput = new SPInput_VerifyEMail()
                {
                    IDNO = IDNO,
                    EMAIL = EMail,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_VerifyEMail, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_VerifyEMail, SPOutput_Base>(connetStr);
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