using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.Login;
using Domain.SP.Input.Register;
using Domain.SP.Output;
using Domain.SP.Output.Login;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 登入
    /// </summary>
    public class LoginController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        private string android = ConfigurationManager.AppSettings.Get("android");
        private string ios = ConfigurationManager.AppSettings.Get("ios");
        private int Rxpires_in = (ConfigurationManager.AppSettings.Get("Rxpires_in")==null)?1800:Convert.ToInt32(ConfigurationManager.AppSettings.Get("Rxpires_in").ToString());
        private int Refrash_Rxpires_in = (ConfigurationManager.AppSettings.Get("Refrash_Rxpires_in") == null) ? 1800 : Convert.ToInt32(ConfigurationManager.AppSettings.Get("Refrash_Rxpires_in").ToString());
        /// <summary>
        /// 會員登入
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doMemberLogin(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "LoginController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_Login apiInput = null;
            OAPI_Login loginAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind=2;
            string Contentjson = "";
            string FileName = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_Login>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode,ref LogID);

                string[] checkList = { apiInput.IDNO, apiInput.PWD,apiInput.DeviceID, apiInput.APPVersion }; 
                string[] errList = { "ERR900", "ERR900", "ERR900", "ERR900" };   
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName,LogID);
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
                    if (false==flag)
                    {
                        errCode = "ERR900";
                    }
                    else
                    {
                        APPKind = apiInput.APP.Value;
                        if(APPKind<0 || APPKind > 1)
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
                string spName = new ObjType().GetSPName(ObjType.SPType.MemberLogin);
                SPInput_MemberLogin SPInputMemberLogin = new SPInput_MemberLogin()
                {
                    APP = APPKind,
                    APPVersion = apiInput.APPVersion,
                    DeviceID = apiInput.DeviceID,
                    LogID = LogID,
                    MEMIDNO = apiInput.IDNO,
                    PWD = apiInput.PWD,
                    Rxpires_in = Rxpires_in,
                    Refrash_Rxpires_in = Refrash_Rxpires_in,
                    PushREGID = apiInput.PushREGID      //20201118 ADD BY ADAM REASON.push token傳入等 app改好再上
                };
                SPOutput_MemberLogin SPOutputMemberLogin = new SPOutput_MemberLogin();
                SQLHelper<SPInput_MemberLogin, SPOutput_MemberLogin> sqlHelp = new SQLHelper<SPInput_MemberLogin, SPOutput_MemberLogin>(connetStr);
                List<RegisterData> lstOut = new List<RegisterData>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, SPInputMemberLogin, ref SPOutputMemberLogin, ref lstOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag,  SPOutputMemberLogin.Error,SPOutputMemberLogin.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    token = new Token()
                    {
                        Access_token = SPOutputMemberLogin.Access_Token,
                        Refrash_token = SPOutputMemberLogin.Refrash_Token,
                        Rxpires_in = Rxpires_in,
                        Refrash_Rxpires_in = Refrash_Rxpires_in
                    };
                    loginAPI = new OAPI_Login()
                    {
                        Token = token,
                        UserData = (lstOut == null) ? null : (lstOut.Count == 0) ? null : lstOut[0]
                    };
                }
                if (flag)
                {
                    if (lstOut.Count > 0)
                    {
                        if(lstOut[0].Signture_pic!=2 && lstOut[0].SIGNATURE != "")
                        {
                            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(lstOut[0].SIGNATURE);
                            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                            Stream dataStream = httpResponse.GetResponseStream();
                            byte[] bytes;
                            using (var memoryStream = new MemoryStream())
                            {
                                dataStream.CopyTo(memoryStream);
                                bytes = memoryStream.ToArray();
                            }
                            string base64String = Convert.ToBase64String(bytes);
                            FileName = string.Format("{0}_{1}_{2}.png", apiInput.IDNO, "Signture", DateTime.Now.ToString("yyyyMMddHHmmss"));
                            flag = new AzureStorageHandle().UploadFileToAzureStorage(base64String, FileName, "credential");
                        }
                    }

                }
                if (flag)
                {
                    string spName1 = new ObjType().GetSPName(ObjType.SPType.SignatureUpdate);
                    SPInput_SignatureUpdate spInput = new SPInput_SignatureUpdate()
                    {
                        LogID = LogID,
                        IDNO = apiInput.IDNO,
                        CrentialsFile = FileName
                    };
                    SPOutput_Base spOut = new SPOutput_Base();
                    SQLHelper<SPInput_SignatureUpdate, SPOutput_Base> sqlHelp1 = new SQLHelper<SPInput_SignatureUpdate, SPOutput_Base>(connetStr);
                    flag = sqlHelp1.ExecuteSPNonQuery(spName1, spInput, ref spOut, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
                }
            }
            #endregion
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, loginAPI, null);
            return objOutput;
            #endregion
        }


    }
}