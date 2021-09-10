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
    /// <summary>
    /// 取得台新錢包明細
    /// </summary>
    public class GetAccountValueController : ApiController
    {
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();

        [HttpPost()]
        public Dictionary<string, object> DoGetAccountValue([FromBody] Dictionary<string, object> value)
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
            string funName = "DoGetAccountValue";
            Int64 LogID = 0;
            var apiInput = new IAPI_GetAccountValue();
            var outputApi = new OAPI_GetAccountValue();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            int page = 1;

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
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetAccountValue>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                    if (apiInput.page < 0)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else
                    {
                        page = (apiInput.page == 0) ? 1 : ((apiInput.page > 1) ? ((apiInput.page - 1) * 200) + 1 : 1);
                    }


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
                    //查詢錢包明細
                    DateTime NowTime = DateTime.UtcNow;
                    string guid = Guid.NewGuid().ToString().Replace("-", "");
                    
                    WebAPI_GetAccountValue wallet = new WebAPI_GetAccountValue()
                    {
                        AccountId = apiInput.AccountId,
                        ApiVersion = "0.1.01",
                        GUID = guid,
                        MerchantId = MerchantId,
                        POSId = "",
                        REQEndDate = "",
                        REQStarDate = "",
                        REQStartCount = page,
                        SourceFrom = "9",
                        StoreId = "",
                        StoreName = "",
                        StoreTransId = "",
                        TransId = ""

                    };
                    var body = JsonConvert.SerializeObject(wallet);
                    TaishinWallet WalletAPI = new TaishinWallet();
                    string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                    string SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                    WebAPIOutput_GetAccountValue output = null;
                    flag = WalletAPI.DoGetAccountValue(wallet, MerchantId, utcTimeStamp, SignCode, ref errCode, ref output);

                    if (output != null)
                        outputApi = objUti.TTMap<WebAPIOutput_GetAccountValue, OAPI_GetAccountValue>(output);

                    trace.traceAdd("outputApi", outputApi);
                }

                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(204, funName, eumTraceType.exception, trace);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}
