using Domain.Common;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;
using WebAPI.Utils;
using WebAPI.Models.BillFunc;
using Newtonsoft.Json;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Wallet;

namespace WebAPI.Controllers
{
    //取得台新錢包狀態
    public class GetAccountStatusController : ApiController
    {
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();

        [HttpPost()]
        public Dictionary<string, object> DoGetAccountStatus([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "DoGetAccountStatus";
            Int64 LogID = 0;
            var apiInput = new IAPI_GetAccountStatus();
            var outputApi = new OAPI_GetAccountStatus();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            #endregion

            if (value == null)
                value = new Dictionary<string, object>();
            try
            {
                trace.traceAdd("apiIn", value);

                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetAccountStatus>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    //不開放訪客
                    if (flag)
                    {
                        if (isGuest)
                        {
                            flag = false;
                            errCode = "ERR101";
                        }
                    }

                    if (flag) 
                    {
                        if (apiInput == null || string.IsNullOrWhiteSpace(apiInput.AccountId))
                        {
                            flag = false;
                            errCode = "ERR902";//參數遺漏                            
                        }
                    }
                }

                #endregion

                #region token

                if (flag && isGuest == false)
                {
                    var token_in = new IBIZ_TokenCk
                    {
                        LogID = LogID,
                        Access_Token = Access_Token
                    };
                    var token_re = cr_com.TokenCk(token_in);
                    if (token_re != null)
                    {
                        flag = token_re.flag;
                        errCode = token_re.errCode;
                        lstError = token_re.lstError;
                        IDNO = token_re.IDNO;
                    }
                }

                #endregion

                #region Call台新 

                if (flag)
                {
                    //查詢錢包
                    TaishinWallet WalletAPI = new TaishinWallet();
                    DateTime NowTime = DateTime.UtcNow;
                    string guid = Guid.NewGuid().ToString().Replace("-", "");
                    WebAPI_GetAccountStatus walletStatus = new WebAPI_GetAccountStatus()
                    {
                        AccountId = apiInput.AccountId,
                        ApiVersion = "0.1.01",
                        GUID = guid,
                        MerchantId = MerchantId,
                        POSId = "",
                        SourceFrom = "9",
                        StoreId = "",
                        StoreName = ""
                    };

                    string body = JsonConvert.SerializeObject(walletStatus);
                    WebAPIOutput_GetAccountStatus statusOutput = null;
                    string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                    string SignCode = WalletAPI.GenerateSignCode(walletStatus.MerchantId, utcTimeStamp, body, APIKey);
                    flag = WalletAPI.DoGetAccountStatus(walletStatus, MerchantId, utcTimeStamp, SignCode, ref errCode, ref statusOutput);
                    if (flag)
                    {
                        if (statusOutput.ReturnCode != "0000")
                        {
                            flag = false;
                            errCode = "ERR214";
                        }
                    }

                    if (statusOutput != null)
                        outputApi = objUti.TTMap<WebAPIOutput_GetAccountStatus, OAPI_GetAccountStatus>(statusOutput);

                    trace.traceAdd("outputApi", outputApi);
                }

                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(203, funName, eumTraceType.exception, trace);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}
