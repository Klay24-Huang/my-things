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
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.CusFun.Input;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 錢包儲值-信用卡
    /// </summary>
    public class WalletStoredByCreditController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string BindResultURL = ConfigurationManager.AppSettings["BindResultURL"].ToString();
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();

        [HttpPost]
        public Dictionary<string, object> DoWalletStoredByCredit(Dictionary<string, object> value)
        {
            #region 初始宣告
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            var carRentComm = new CarRentCommon();
            var wsp = new WalletSp();

            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletStoredByCredit";
            string TradeType = "Store_Credit"; ///交易類別
            string PRGID = "220"; //APIId


            Int16 Mode = 1;
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_WalletStoredByCredit apiInput = null;
            var apiOutput = new OAPI_WalletStoredByCredit();
            var spOutput = new SPOutput_GetWallet();
            WebAPI_CreateAccountAndStoredMoney wallet = null;
            WebAPIOutput_StoreValueCreateAccount output = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            string spName = "";
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

                #region Token判斷
                if (flag && isGuest == false)
                {
                    var token_in = new IBIZ_TokenCk
                    {
                        LogID = LogID,
                        Access_Token = Access_Token
                    };
                    var token_re = carRentComm.TokenCk(token_in);
                    if (token_re != null)
                    {
                        trace.traceAdd(nameof(token_re), token_re);
                        flag = token_re.flag;
                        errCode = token_re.errCode;
                        lstError = token_re.lstError;
                        IDNO = token_re.IDNO;
                    }
                    trace.FlowList.Add("Token判斷");
                }
                #endregion

                #region 取錢包會員資料&儲值金額限制檢核
                if (flag)
                {
                    spName = "usp_GetWallet_Q01";
                    SPInput_GetWallet spInput = new SPInput_GetWallet()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        Token = Access_Token
                    };
                    spOutput = new SPOutput_GetWallet();
                    SQLHelper<SPInput_GetWallet, SPOutput_GetWallet> sqlHelp = new SQLHelper<SPInput_GetWallet, SPOutput_GetWallet>(connetStr);
                    flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOutput, ref lstError);
                    baseVerify.checkSQLResult(ref flag, spOutput.Error, spOutput.ErrorCode, ref lstError, ref errCode);

                    if (flag && spOutput != null)
                    {
                        //錢包現存餘額上限為5萬元
                        if (apiInput.StoreMoney + spOutput.WalletBalance > 50000)
                        {
                            flag = false;
                            errCode = "ERR282";
                        }
                        //錢包單月儲值(包括受贈)上限為30萬元
                        else if (apiInput.StoreMoney + spOutput.MonthlyTransAmount > 300000)
                        {
                            flag = false;
                            errCode = "ERR280";
                        }
                    }

                    trace.traceAdd("GetWallet", new { spInput, spOutput });
                    trace.FlowList.Add("儲值金額限制檢核");

                    #region 送台新查詢信用卡→直接授權
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

                        trace.traceAdd("DoGetCreditCardList", new { wsInput, wsOutput, errCode });
                        trace.FlowList.Add("送台新查詢信用卡");

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

                            #region 直接授權
                            if (hasFind)//有找到，可以做儲值
                            {
                                Thread.Sleep(1000);
                                Domain.WebAPI.Input.Taishin.AuthItem item = new Domain.WebAPI.Input.Taishin.AuthItem()
                                {
                                    Amount = apiInput.StoreMoney.ToString() + "00",
                                    Name = "錢包儲值",
                                    NonPoint = "N",
                                    NonRedeem = "N",
                                    Price = apiInput.StoreMoney.ToString() + "00",
                                    Quantity = "1"
                                };
                                PartOfCreditCardAuth WSAuthInput = new PartOfCreditCardAuth()
                                {
                                    ApiVer = "1.0.2",
                                    ApposId = TaishinAPPOS,
                                    RequestParams = new Domain.WebAPI.Input.Taishin.AuthRequestParams()
                                    {
                                        CardToken = CardToken,
                                        InstallPeriod = "0",
                                        InvoiceMark = "N",
                                        Item = new List<Domain.WebAPI.Input.Taishin.AuthItem>(),
                                        MerchantTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                                        MerchantTradeTime = DateTime.Now.ToString("HHmmss"),
                                        MerchantTradeNo = string.Format("{0}WalletSave{1}", IDNO, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                        NonRedeemAmt = apiInput.StoreMoney.ToString() + "00",
                                        NonRedeemdescCode = "",
                                        Remark1 = "",
                                        Remark2 = "",
                                        Remark3 = "",
                                        ResultUrl = BindResultURL,
                                        TradeAmount = apiInput.StoreMoney.ToString() + "00",
                                        TradeType = "1",
                                        UseRedeem = "N"

                                    },
                                    Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                                    TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                                };
                                WSAuthInput.RequestParams.Item.Add(item);

                                WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();
                                flag = WebAPI.DoCreditCardAuth(WSAuthInput, ref errCode, ref WSAuthOutput);
                                
                                TaishinNO = WSAuthOutput.ResponseParams.ResultData.ServiceTradeNo;     //台新IR編
                                trace.traceAdd("DoCreditCardAuth", new { WSAuthInput, WSAuthOutput, errCode });
                                trace.FlowList.Add("信用卡授權");

                                if (WSAuthOutput.RtnCode != "1000" && WSAuthOutput.ResponseParams.ResultCode != "0000")
                                {
                                    flag = false;
                                    errCode = "ERR197"; //刷卡授權失敗，請洽發卡銀行
                                }
                            }
                            else
                            {
                                flag = false;
                                errCode = "ERR195"; //找不到此卡號
                            }
                            #endregion                      
                        }
                        else
                        {
                            errCode = wsOutput.RtnCode;
                            errMsg = wsOutput.RtnMessage;
                        }
                    }
                    #endregion

                    #region 台新錢包儲值
                    if (flag)
                    {
                        DateTime NowTime = DateTime.UtcNow;
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
                            CreateType = "1",
                            AmountType = "2",
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

                        if (flag == false)
                        {
                            errCode = "ERR918"; //Api呼叫失敗
                            errMsg = output.Message;
                        }

                        #region 錢包儲值失敗處理
                        #endregion

                        #region 寫入錢包紀錄
                        if (flag)
                        {
                            string formatString = "yyyyMMddHHmmss";
                            SPInput_WalletStore spInput_Wallet = new SPInput_WalletStore()
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
                                TaishinNO= TaishinNO,
                                TradeType = TradeType,
                                PRGID = PRGID,
                                Mode = Mode,
                                Token = Access_Token,
                                LogID = LogID
                            };

                            flag = wsp.sp_WalletStore(spInput_Wallet, ref errCode);

                            trace.traceAdd("WalletStore", new { spInput_Wallet, flag, errCode });
                            trace.FlowList.Add("寫入錢包紀錄");


                            apiOutput = new OAPI_WalletStoredByCredit()
                            {
                                StoreMoney = apiInput.StoreMoney,
                                StroeResult = 1,
                                Timestamp = spInput_Wallet.LastTransDate == null ? "" : string.Format("{0:yyyy/MM/dd hh:mm}", spInput_Wallet.LastTransDate )
                            };
                        }
                        #endregion

                    }
                    #endregion

                }
                #endregion

                apiOutput.StroeResult = flag ? 1 : 0;

                trace.traceAdd("TraceFinal", new { errCode, errMsg });
                carRepo.AddTraceLog(220, funName, trace, flag);
            }
            catch (Exception ex)
            {
                flag = false;
                apiOutput.StroeResult = 0;
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
