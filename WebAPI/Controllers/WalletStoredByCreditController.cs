using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.ComboFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Bill.Input;
using WebAPI.Models.Param.Bill.Output;
using WebAPI.Models.Param.CusFun.Input;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Service;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{

    public class WalletStoredByCreditController : ApiController
    {
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();


        /// <summary>
        /// 錢包儲值-信用卡
        /// </summary>
        [HttpPost]
        public Dictionary<string, object> DoWalletStoredByCredit(Dictionary<string, object> value)
        {
            #region 初始宣告
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            var wsp = new WalletSp();
            var walletService = new WalletService();
            OFN_CreditAuthResult AuthOutput = new OFN_CreditAuthResult();
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletStoredByCreditController";
            string TradeType = "Store_Credit"; ///交易類別
            int apiId = 220;


            Int16 Mode = 1;
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_WalletStoredByCredit apiInput = null;
            var spOutput = new SPOutput_GetWallet();
            WebAPI_CreateAccountAndStoredMoney wallet = null;
            WebAPIOutput_StoreValueCreateAccount output = null;
            SPInput_WalletStore spInput_Wallet = null;
            OAPI_WalletStoreBase apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            string TaishinNO = "";

            #endregion
            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆
                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

                if (flag)
                {
                    //寫入API Log
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletStoredByCredit>(Contentjson);
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    //必填檢查
                    if (apiInput.StoreType != 0 && apiInput.StoreType != 4)
                    {
                        flag = false;
                        errCode = "ERR902";
                    }

                    if (apiInput.StoreMoney <= 0)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else if (apiInput.StoreMoney > 0 && apiInput.StoreMoney < 100)
                    {
                        flag = false;
                        errCode = "ERR284";
                    }
                    else if (apiInput.StoreMoney > 50000)
                    {
                        flag = false;
                        errCode = "ERR285";
                    }

                    //不開放訪客
                    if (isGuest)
                    {
                        flag = false;
                        errCode = "ERR101";
                    }
                    trace.FlowList.Add("防呆");
                }
                #endregion
                #region TB
                #region Token判斷
                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);

                }
                #endregion
                #region 取錢包會員資料&儲值金額限制檢核
                if (flag)
                {
                    var walletInfo = walletService.CheckStoreAmtLimit(apiInput.StoreMoney, IDNO, LogID, Access_Token, ref errCode);
                    flag = walletInfo.flag;
                    spOutput = walletInfo.Info;
                }
                #endregion
                #region 信用卡授權
                var AuthInput = new IFN_CreditAuthRequest
                {
                    CheckoutMode = apiInput.StoreType,
                    OrderNo = 0,
                    IDNO = IDNO,
                    Amount = apiInput.StoreMoney,
                    PayType = 6,
                    autoClose = 0,
                    funName = funName,
                    insUser = funName,
                    AuthType = 9
                };
                CreditAuthComm creditAuthComm = new CreditAuthComm();
                flag = creditAuthComm.DoAuthV4(AuthInput, ref errCode, ref AuthOutput);
                #endregion
                #region 台新錢包儲值
                if (flag)
                {
                    DateTime NowTime = DateTime.Now;
                    string guid = Guid.NewGuid().ToString().Replace("-", "");
                    int nowCount = 1;
                    wallet = new WebAPI_CreateAccountAndStoredMoney()
                    {
                        ApiVersion = "0.1.01",
                        GUID = guid,
                        MerchantId = MerchantId,
                        POSId = "",
                        StoreId = "",
                        StoreName = "",
                        StoreTransDate = NowTime.ToString("yyyyMMddHHmmss"),
                        StoreTransId = string.Format("{0}{1}", IDNO, NowTime.ToString("MMddHHmmss")),
                        MemberId = string.Format("{0}Wallet{1}", IDNO, nowCount.ToString().PadLeft(4, '0')),
                        Name = spOutput.Name,
                        PhoneNo = spOutput.PhoneNo,
                        Email = spOutput.Email,
                        ID = IDNO,
                        AccountType = "2",
                        AmountType = "2",
                        CreateType = "1",
                        Amount = apiInput.StoreMoney,
                        Bonus = 0,
                        BonusExpiredate = "",
                        SourceFrom = "9"
                    };

                    var body = JsonConvert.SerializeObject(wallet);
                    TaishinWallet WalletAPI = new TaishinWallet();
                    string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                    string SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                    flag = WalletAPI.DoStoreValueCreateAccount(wallet, MerchantId, utcTimeStamp, SignCode, ref errCode, ref output);
                    trace.traceAdd("DoStoreValueCreateAccount", new { wallet, MerchantId, utcTimeStamp, SignCode, output, errCode });
                    trace.FlowList.Add("錢包儲值");

                    if (!flag && (output.ReturnCode == "9999" || output.ReturnCode != "0000" || output.ReturnCode != "M000"))
                    {
                        #region 寫入開戶儲值錯誤LOG
                        SPInput_InsTaishinStoredMoneyError spInput = new SPInput_InsTaishinStoredMoneyError()
                        {
                            ID = wallet.ID,
                            MemberId = wallet.MemberId,
                            Name = wallet.Name,
                            PhoneNo = wallet.PhoneNo,
                            Email = wallet.Email,
                            AccountType = wallet.AccountType,
                            CreateType = wallet.CreateType,
                            AmountType = wallet.AmountType,
                            Amount = wallet.Amount,
                            Bonus = wallet.Bonus,
                            BonusExpiredate = wallet.BonusExpiredate,
                            SourceFrom = wallet.SourceFrom,
                            StoreValueReleaseDate = wallet.StoreValueReleaseDate,
                            GiftCardBarCode = wallet.GiftCardBarCode,
                            PRGID = apiId.ToString(),
                            LogID = LogID,
                            ReturnCode = output.ReturnCode,
                            ExceptionData = output.ExceptionData,
                            Message = output.Message
                        };
                        var success = wsp.sp_InsTaishinStoredMoneyError(spInput, ref errCode);
                        trace.traceAdd("InsTaishinStoredMoneyErrorLog", new { spInput, success, errCode });
                        trace.FlowList.Add("寫入開戶儲值錯誤LOG");
                        #endregion

                        errCode = "ERR918"; //Api呼叫失敗
                        errMsg = output.Message;

                    }

                    #region 寫入錢包
                    if (flag)
                    {
                        string formatString = "yyyyMMddHHmmss";
                        spInput_Wallet = new SPInput_WalletStore()
                        {
                            IDNO = output.Result.ID,
                            WalletMemberID = output.Result.MemberId,
                            WalletAccountID = output.Result.AccountId,
                            Status = Convert.ToInt32(output.Result.Status),
                            Email = output.Result.Email,
                            PhoneNo = output.Result.PhoneNo,
                            StoreAmount = apiInput.StoreMoney,
                            WalletBalance = output.Result.Amount,
                            CreateDate = DateTime.ParseExact(output.Result.CreateDate, formatString, null),
                            LastTransDate = DateTime.ParseExact(output.Result.TransDate, formatString, null),
                            LastStoreTransId = output.Result.StoreTransId,
                            LastTransId = output.Result.TransId,
                            TaishinNO = !string.IsNullOrWhiteSpace(TaishinNO) ? TaishinNO : output.Result.TransId,
                            TradeType = TradeType,
                            PRGName = funName,
                            Mode = Mode,
                            InputSource = 1,
                            Token = Access_Token,
                            LogID = LogID
                        };

                        flag = wsp.sp_WalletStore(spInput_Wallet, ref errCode);

                        trace.traceAdd("WalletStore", new { spInput_Wallet, flag, errCode });
                        trace.FlowList.Add("寫入錢包紀錄");
                    }
                    #endregion
                }
                #endregion
                if (flag)
                {
                    apiOutput = new OAPI_WalletStoreBase()
                    {
                        StoreMoney = apiInput.StoreMoney
                    };
                }

                trace.traceAdd("TraceFinal", new { errCode, errMsg });
                carRepo.AddTraceLog(apiId, funName, trace, flag);
                #endregion
            }
            catch (Exception ex)
            {
                flag = false;
                errCode = "ERR918";
                trace.BaseMsg = ex.Message;
            }


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
