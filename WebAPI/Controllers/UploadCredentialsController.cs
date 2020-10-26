using Domain.Common;
using Domain.SP.Input.Register;
using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 上傳證件照
    /// </summary>
    public class UploadCredentialsController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 註冊寫入會員資料
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doUploadCredentials(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "UploadCredentialsController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_UploadCredentials apiInput = null;
            OAPI_Login CheckAccountAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            Int16 CredentialType;
            DateTime Birth = DateTime.Now;
            string Contentjson = "";
            string FileName = "";
            /*
                   /// <para>1:身份證正面</para>
                    /// <para>2:身份證反面</para>
                    /// <para>3:汽車駕照正面</para>
                    /// <para>4:汽車駕照反面</para>
                    /// <para>5:機車駕證正面</para>
                    /// <para>6:機車駕證反面</para>
                    /// <para>7:自拍照</para>
                    /// <para>8:法定代理人</para>
                    /// <para>9:其他（如台大專案）</para>
             */
            string[] suff = { "", "ID_1", "ID_2", "Driver_1", "Driver_2", "Moto_1", "Moto_2", "Self_1", "F1", "Other_1" };
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_UploadCredentials>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.IDNO, apiInput.CredentialFile,  apiInput.DeviceID, apiInput.APPVersion };
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
                    //2.1判斷模式
                    if (flag)
                    {

                    }
                    //2.2判斷類型
                    if (flag)
                    {
                        flag = apiInput.CredentialType.HasValue;
                        if (false == flag)
                        {
                            errCode = "ERR111";
                        }
                        else
                        {
                            flag = Int16.TryParse(apiInput.CredentialType.Value.ToString(), out CredentialType);
                            if (flag)
                            {
                                if (CredentialType < 1 || CredentialType > 10)
                                {
                                    flag = false;
                                    errCode = "ERR112";
                                }
                            }
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
            #region Azure Storage
            if (flag)
            {
                try
                {
                    FileName = string.Format("{0}_{1}_{2}", apiInput.IDNO, suff[apiInput.CredentialType.Value], DateTime.Now.ToString("yyyyMMddHHmmss"));
                    flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.CredentialFile, FileName, "credential");
                }catch(Exception ex)
                {
                    flag = false;
                    errCode = "ERR226";
                }
            }
            #endregion
            #region TB
            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.UploadCredentials);

                //SPInput_UploadCredentials spInput = new SPInput_UploadCredentials()
                //{
                //    LogID = LogID,
                //    IDNO = apiInput.IDNO,
                //    CrentialsFile = apiInput.CredentialFile,
                //    CrentialsType = apiInput.CredentialType.Value,
                //    DeviceID = apiInput.DeviceID
                //};
                //SPOutput_Base spOut = new SPOutput_Base();
                //SQLHelper<SPInput_UploadCredentials, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_UploadCredentials, SPOutput_Base>(connetStr);
                //flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                //baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);


                object[][] parms1 = {
                        new object[] {
                            apiInput.IDNO,
                            apiInput.DeviceID,
                            apiInput.CredentialType.Value,
                            FileName,
                           // apiInput.CredentialFile, //改為直接將azure檔名寫入tb
                            LogID
                    }};

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), spName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);
               
                if (ds1.Tables.Count == 0)
                {
                    flag = false;
                    errCode = "ERR999";
                    errMsg = returnMessage;
                }
                else
                {
                    if (ds1.Tables.Count == 1)
                    {
                        baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[0].Rows[0]["Error"]), ds1.Tables[0].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);
                    }
                }
                ds1.Dispose();
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
