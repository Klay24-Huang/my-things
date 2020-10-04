using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class WalletHistoryController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string APIToken = ConfigurationManager.AppSettings["TaishinWalletAPIToken"].ToString();
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private string BaseURL = ConfigurationManager.AppSettings["TaishinWalletBaseURL"].ToString();

        [HttpPost]
        public Dictionary<string, object> DoWalletHistory(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletHistoryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_WallHistory apiInput = null;
            OAPI_WalletHistory apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            int page = 1;
            #endregion
            #region 防呆

            //flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                //寫入API Log
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WallHistory>(Contentjson);
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
            else
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                }
            }
            #endregion
            #region TB
            //Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);

            }
            #endregion
            #region 取個人資料
            if (flag)
            {
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetWalletInfo);
                SPInput_GetWalletInfo SPInput = new SPInput_GetWalletInfo()
                {
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_GetWalletInfo SPOutput = new SPOutput_GetWalletInfo();
                SQLHelper<SPInput_GetWalletInfo, SPOutput_GetWalletInfo> sqlHelp = new SQLHelper<SPInput_GetWalletInfo, SPOutput_GetWalletInfo>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
              
                #region 台新錢包

                if (flag)
                {
                    DateTime NowTime = DateTime.UtcNow;
                    string guid = Guid.NewGuid().ToString().Replace("-", "");
                    int nowCount = 1;
                    WebAPI_GetAccountValue wallet = new WebAPI_GetAccountValue()
                    {
                        AccountId = SPOutput.WalletAccountID,
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
                        TransId = "",
                        TransStatus = ""

                    };
                    var body = JsonConvert.SerializeObject(wallet);
                    TaishinWallet WalletAPI = new TaishinWallet();
                    string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                    string SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                    WebAPIOutput_GetAccountValue output = null;
                    flag = WalletAPI.DoGetAccountValue(wallet, MerchantId, utcTimeStamp, SignCode, ref errCode, ref output);
                    #region 將執行結果寫輸出
                    if (flag)
                    {
                        if (output.ReturnCode == "0000")
                        {
                            apiOutput = new OAPI_WalletHistory()
                            {
                                TotalCount = output.Result.TotalCount
                            };
                            int len = output.Result.Detail.Count;
                            if (len > 0)
                            {
                                apiOutput.Details = new List<Models.Param.Output.PartOfParam.DetailData>();
                                for(int i = 0; i < len; i++)
                                {
                                    DetailData obj = new DetailData()
                                    {
                                        Amount = output.Result.Detail[i].Amount,
                                        TransDate = output.Result.Detail[i].TransDate,
                                        TransType = output.Result.Detail[i].TransType,
                                        TransTypeName = baseVerify.GetTransTypeName(output.Result.Detail[i].TransType)
                                    };
                                    apiOutput.Details.Add(obj);
                                    
                                }
                                
                            }
                        }
                    }
                    if (flag)
                    {
                         NowTime = DateTime.UtcNow;
                         guid = Guid.NewGuid().ToString().Replace("-", "");
                        WebAPI_GetAccountStatus walletStatus = new WebAPI_GetAccountStatus()
                        {
                            AccountId = SPOutput.WalletAccountID,
                            ApiVersion = "0.1.01",
                            GUID = guid,
                            MerchantId = MerchantId,
                            POSId = "",
                            SourceFrom = "9",
                            StoreId = "",
                            StoreName = ""
                             

                        };
                         body = JsonConvert.SerializeObject(walletStatus);
                        WebAPIOutput_GetAccountStatus statusOutput = null;
                         utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                         SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                       
                        flag = WalletAPI.DoGetAccountStatus(walletStatus, MerchantId, utcTimeStamp, SignCode, ref errCode, ref statusOutput);
                        if (flag)
                        {
                            if (statusOutput.ReturnCode == "0000")
                            {
                                apiOutput.TotalAmount = statusOutput.Result.Amount;
                            }
                        }

                    }
                    #endregion
                }
                #endregion
            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}
