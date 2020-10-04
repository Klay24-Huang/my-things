using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin;
using Domain.WebAPI.output.Taishin.ResultData;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
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
    /// 錢包儲值
    /// </summary>
    public class WalletStoredMoneyController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string APIToken = ConfigurationManager.AppSettings["TaishinWalletAPIToken"].ToString();
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private string BaseURL = ConfigurationManager.AppSettings["TaishinWalletBaseURL"].ToString();
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string BindResultURL = ConfigurationManager.AppSettings["BindResultURL"].ToString();
        private string BindSuccessURL = ConfigurationManager.AppSettings["BindSuccessURL"].ToString();
        private string BindFailURL = ConfigurationManager.AppSettings["BindFailURL"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVer"].ToString();
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();
        [HttpPost]
        public Dictionary<string, object> DoWalletStoredMoney(Dictionary<string, object> value)
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
            string funName = "WalletStoredMoneyController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_WalletStoredMoney apiInput = null;
            OAPI_GetCardBindList apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            #endregion
            #region 防呆

            //flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                //寫入API Log
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletStoredMoney>(Contentjson);
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                if (apiInput.Amount <= 0)
                {
                    flag = false;
                    errCode = "ERR900";
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
                #region 台新信用卡
                if (flag)
                {
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
                        if (Len > 0)
                        {
                            CardToken = wsOutput.ResponseParams.ResultData[0].CardToken;
                            hasFind = true;
                        }
                        #region 直接授權,先關閉，待2020/10/05詢問台新
                        //if (hasFind)//有找到，可以做儲值
                        //{
                        //    Thread.Sleep(1000);
                        //    AuthItem item = new AuthItem()
                        //    {
                        //        Amount = apiInput.Amount.ToString() + "00",
                        //        Name = "錢包儲值",
                        //        NonPoint = "N",
                        //        NonRedeem = "N",
                        //        Price = apiInput.Amount.ToString() + "00",
                        //        Quantity = "1"
                        //    };
                        //    PartOfCreditCardAuth WSAuthInput = new PartOfCreditCardAuth()
                        //    {
                        //        ApiVer = "1.0.2",
                        //        ApposId = TaishinAPPOS,
                        //        RequestParams = new AuthRequestParams()
                        //        {
                        //            CardToken = CardToken,
                        //            InstallPeriod = "0",
                        //            InvoiceMark = "N",
                        //            Item = new List<AuthItem>(),
                        //            MerchantTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                        //            MerchantTradeTime = DateTime.Now.ToString("HHmmss"),
                        //            MerchantTradeNo = string.Format("{0}WalletSave{1}", IDNO, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                        //            NonRedeemAmt = apiInput.Amount.ToString() + "00",
                        //            NonRedeemdescCode = "",
                        //            Remark1 = "",
                        //            Remark2 = "",
                        //            Remark3 = "",
                        //            ResultUrl = BindResultURL,
                        //            TradeAmount = apiInput.Amount.ToString() + "00",
                        //            TradeType = "1",
                        //            UseRedeem = "N"

                        //        },
                        //        Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                        //        TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),

                        //    };
                        //    WSAuthInput.RequestParams.Item.Add(item);

                        //    WebAPIOutput_DeleteCreditCardAuth WSDeleteOutput = new WebAPIOutput_DeleteCreditCardAuth();
                        //    flag = WebAPI.DoCreditCardAuth(WSAuthInput, ref errCode, ref WSDeleteOutput);
                        //    if (WSDeleteOutput.ResponseParams.ResultData.IsSuccess == false)
                        //    {
                        //        flag = false;
                        //        errCode = "ERR196";
                        //    }
                        //}
                        //else
                        //{
                        //    flag = false;
                        //    errCode = "ERR195";
                        //}
                        #endregion

                    }
                }
                #endregion
                #region 台新錢包

                if (flag)
                {
                    DateTime NowTime = DateTime.UtcNow;
                    string guid = Guid.NewGuid().ToString().Replace("-", "");
                    int nowCount = 1;
                    WebAPI_CreateAccountAndStoredMoney wallet = new WebAPI_CreateAccountAndStoredMoney()
                    {
                        AccountType = "2",
                        ApiVersion = "0.1.01",
                        CreateType = "1",
                        Email = SPOutput.Email,
                        GUID = guid,
                        ID = IDNO,
                        MemberId = string.Format("{0}Wallet{1}", IDNO, nowCount.ToString().PadLeft(4, '0')),
                        MerchantId = MerchantId,
                        Name = SPOutput.Name,
                        PhoneNo = SPOutput.PhoneNo,
                        POSId = "",
                        SourceFrom = "9",
                        Amount = apiInput.Amount,
                        AmountType = "2",
                        StoreName = "",
                        StoreTransDate = NowTime.ToString("yyyyMMddHHmmss"),
                        StoreTransId = string.Format("{0}{1}", IDNO, NowTime.ToString("MMddHHmmss")),
                        StoreId = "",
                        Bonus = 0,
                        BonusExpiredate = ""

                    };
                    var body = JsonConvert.SerializeObject(wallet);
                    TaishinWallet WalletAPI = new TaishinWallet();
                    string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                    string SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                    WebAPIOutput_StoreValueCreateAccount output = null;
                    flag = WalletAPI.DoStoreValueCreateAccount(wallet, MerchantId, utcTimeStamp, SignCode, ref errCode, ref output);
                    #region 將執行結果寫入TB
                    if (flag)
                    {
                        string formatString = "yyyyMMddHHmmss";
                        SPInput_HandleWallet SPInputH = new SPInput_HandleWallet()
                        {
                            Amount = wallet.Amount,
                            CreateDate = DateTime.ParseExact(output.Result.CreateDate, formatString, null),
                            Email = output.Result.Email,
                            IDNO = output.Result.ID,
                            LastStoreTransId = output.Result.StoreTransId,
                            LastTransDate = DateTime.ParseExact(output.Result.TransDate, formatString, null),
                            LastTransId = output.Result.TransId,
                            LogID = LogID,
                            PhoneNo = output.Result.PhoneNo,
                            Status = Convert.ToInt32(output.Result.Status),
                            Token = Access_Token,
                            TotalAmount = output.Result.Amount,
                            WalletAccountID = output.Result.AccountId,
                            WalletMemberID = output.Result.MemberId
                        };
                        SPOutput_Base SPoutputH = new SPOutput_Base();
                        SPName= new ObjType().GetSPName(ObjType.SPType.HandleWallet);
                        SQLHelper<SPInput_HandleWallet, SPOutput_Base> sqlHelpH = new SQLHelper<SPInput_HandleWallet, SPOutput_Base>(connetStr);
                        flag = sqlHelpH.ExecuteSPNonQuery(SPName, SPInputH, ref SPoutputH, ref lstError);
                        baseVerify.checkSQLResult(ref flag, ref SPoutputH, ref lstError, ref errCode);
                    }
                    else
                    {
                        errCode = "ERR";
                        errMsg = output.Message;
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
