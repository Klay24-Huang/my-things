using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Member;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin;
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
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 付款設定
    /// </summary>
    public class SetDefPayModeController : ApiController
    {

        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string BindResultURL = ConfigurationManager.AppSettings["BindResultURL"].ToString();
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();
        [HttpPost]
        public Dictionary<string, object> DoInvoiceSetting(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "SetDefPayModeController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_SetDefPayMode apiInput = null;
            NullOutput outputApi = null;

            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int64 tmpOrder = 0;
            Int16 SettingMode = 0;  //設定發票
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_SetDefPayMode>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
       
            
                if(apiInput.DefPayMode<0 || apiInput.DefPayMode > 1)
                {
                    if(apiInput.DefPayMode>1 && apiInput.DefPayMode < 4)
                    {
                        flag = false;
                        errCode = "ERR212"; //顯示其他支付方式尚未開放
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
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
            #endregion
            #region TB
            //Token判斷
            if (flag && isGuest == false)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {

                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }
            #region 各類型判斷
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

                switch (apiInput.DefPayMode)
                {
                    case 0: //信用卡
                            //查詢綁卡
                            //送台新查詢
                        TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
                        PartOfGetCreditCardList wsInput = new PartOfGetCreditCardList()
                        {
                            ApiVer = ApiVerOther,
                            ApposId = TaishinAPPOS,
                            RequestParams = new GetCreditCardListRequestParamasData()
                            {
                                MemberId = IDNO,
                            },
                            Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                            TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                            TransNo = string.Format("{0}_{1}", IDNO, DateTime.Now.ToString("yyyyMMddhhmmss"))

                        };
                        WebAPIOutput_GetCreditCardList wsOutput = new WebAPIOutput_GetCreditCardList();
                        flag = WebAPI.DoGetCreditCardList(wsInput, ref errCode, ref wsOutput);
                        if (flag)
                        {
                            int Len = wsOutput.ResponseParams.ResultData.Count;
                            bool hasFind = false;
                            string CardToken = "";
                            if (Len <= 0)
                            {
                                flag = false;
                                errCode = "ERR214";
                            }
                        }
                      
                        break;
                    case 1:
                        //查詢錢包
                        TaishinWallet WalletAPI = new TaishinWallet();
                        DateTime NowTime = DateTime.UtcNow;                   
                        string guid = Guid.NewGuid().ToString().Replace("-", "");
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
                        
                        break;
                        //3與4待開放後再補上判斷
                }
            }
            #endregion
            #region 更新會員資料表或是寫入訂單內
            if (flag)
            {

                string SPName = new ObjType().GetSPName(ObjType.SPType.SetDefPayMode);
                SPInput_SetDefPayMode SPInput = new SPInput_SetDefPayMode()
                {

                    LogID = LogID,
                    IDNO = IDNO,
                    PayMode=apiInput.DefPayMode,
                    Token = Access_Token
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_SetDefPayMode, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_SetDefPayMode, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            }
            #endregion
            #endregion
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}
