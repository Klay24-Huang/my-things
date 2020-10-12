using Domain.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
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
using System.Data;
using System.Threading;
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
    /// 混合付款
    /// </summary>
    public class MixAuthModeController : ApiController
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
        public Dictionary<string, object> DoMixAuthMode(Dictionary<string, object> value)
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
            string funName = "MixAuthModeController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_MixAuth apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            Int64 tmpOrder = 0;
            int Amount = 0;
            bool HasETAG = false;
            int WalletTotalAmount = 0;
            int NeedSaveAmount = 0;
            List<OrderQueryFullData> OrderDataLists = null;
            #endregion
            #region 防呆

            //flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                //寫入API Log
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MixAuth>(Contentjson);
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
        
                if (flag)
                {
                    if (apiInput.PayType == 0)
                    {
                        if (string.IsNullOrWhiteSpace(apiInput.OrderNo))
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                        else
                        {
                            if (apiInput.OrderNo.IndexOf("H") < 0)
                            {
                                flag = false;
                                errCode = "ERR900";
                            }
                            if (flag)
                            {
                                flag = Int64.TryParse(apiInput.OrderNo.Replace("H", ""), out tmpOrder);
                                if (flag)
                                {
                                    if (tmpOrder <= 0)
                                    {
                                        flag = false;
                                        errCode = "ERR900";
                                    }

                                }
                            }
                        }
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
                DateTime NowTime = DateTime.UtcNow;
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
             
                if (apiInput.PayType == 0)
                {
                    #region 取出訂單資訊
                    if (flag)
                    {
                       
                        SPInput_GetOrderStatusByOrderNo spInput = new SPInput_GetOrderStatusByOrderNo()
                        {
                            IDNO = IDNO,
                            OrderNo = tmpOrder,
                            LogID = LogID,
                            Token = Access_Token
                        };
                        string SPName = new ObjType().GetSPName(ObjType.SPType.GetOrderStatusByOrderNo);
                        SPOutput_Base spOutBase = new SPOutput_Base();
                        SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
                        OrderDataLists = new List<OrderQueryFullData>();
                        DataSet ds = new DataSet();
                        flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderDataLists, ref ds, ref lstError);
                        baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
                        //判斷訂單狀態
                        if (flag)
                        {
                            if (OrderDataLists.Count == 0)
                            {
                                flag = false;
                                errCode = "ERR203";
                            }
                        }

                    }
                    if (flag)
                    {
                        if (OrderDataLists[0].car_mgt_status >= 15)
                        {
                            flag = false;
                            errCode = "ERR209";
                        }
                        else if (OrderDataLists[0].car_mgt_status < 11)
                        {
                            flag = false;
                            errCode = "ERR210";
                        }
                        else
                        {
                            Amount = OrderDataLists[0].final_price;
                        }
                    }
                
                    #endregion
                    #region 檢查車機狀態
                    if (flag)
                    {
                        flag = new CarCommonFunc().CheckReturnCar(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                    }
                    #endregion
                    #region 查詢個人資料
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
                        #region 台新錢包查詢目前剩餘金額
                        TaishinWallet WalletAPI = new TaishinWallet();
                        if (flag)
                        {

                           
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
                            var body = JsonConvert.SerializeObject(walletStatus);
                            WebAPIOutput_GetAccountStatus statusOutput = null;
                            string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                            string SignCode = WalletAPI.GenerateSignCode(walletStatus.MerchantId, utcTimeStamp, body, APIKey);

                            flag = WalletAPI.DoGetAccountStatus(walletStatus, MerchantId, utcTimeStamp, SignCode, ref errCode, ref statusOutput);
                            if (flag)
                            {
                                if (statusOutput.ReturnCode == "0000")
                                {
                                    WalletTotalAmount = statusOutput.Result.Amount;
                                    NeedSaveAmount = Amount - WalletTotalAmount;
                                }
                            }

                        }
                        #endregion
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

                                #region 直接授權
                                if (hasFind)//有找到，可以做扣款
                                {
                                    SPInput_DonePayRent PayInput = new SPInput_DonePayRent()
                                    {
                                        IDNO = IDNO,
                                        LogID = LogID,
                                        OrderNo = tmpOrder,
                                        Token = Access_Token,
                                        transaction_no = ""
                                    };

                                    Thread.Sleep(1000);
                                    if (NeedSaveAmount > 0)
                                    {
                                        Domain.WebAPI.Input.Taishin.AuthItem item = new Domain.WebAPI.Input.Taishin.AuthItem()
                                        {
                                            Amount = Amount.ToString() + "00",
                                            Name = string.Format("{0}租金", apiInput.OrderNo),
                                            NonPoint = "N",
                                            NonRedeem = "N",
                                            Price = NeedSaveAmount.ToString() + "00",
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
                                                MerchantTradeNo = string.Format("{0}F{1}", tmpOrder, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                                NonRedeemAmt = NeedSaveAmount.ToString() + "00",
                                                NonRedeemdescCode = "",
                                                Remark1 = "",
                                                Remark2 = "",
                                                Remark3 = "",
                                                ResultUrl = BindResultURL,
                                                TradeAmount = NeedSaveAmount.ToString() + "00",
                                                TradeType = "1",
                                                UseRedeem = "N"

                                            },
                                            Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                                            TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),

                                        };
                                        WSAuthInput.RequestParams.Item.Add(item);

                                        WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();
                                        flag = WebAPI.DoCreditCardAuth(WSAuthInput, ref errCode, ref WSAuthOutput);
                                        if (WSAuthOutput.RtnCode != "1000" && WSAuthOutput.ResponseParams.ResultCode != "0000")
                                        {
                                            flag = false;
                                            errCode = "ERR197";
                                        }
                                        if (flag)
                                        {
                                            PayInput.transaction_no = WSAuthInput.RequestParams.MerchantTradeNo;
                                        }
                                        if (flag)
                                        {
                                            #region 錢包儲值
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
                                                Amount = NeedSaveAmount,
                                                AmountType = "2",
                                                StoreName = "",
                                                StoreTransDate = NowTime.ToString("yyyyMMddHHmmss"),
                                                StoreTransId = string.Format("{0}{1}", IDNO, NowTime.ToString("MMddHHmmss")),
                                                StoreId = "",
                                                Bonus = 0,
                                                BonusExpiredate = ""

                                            };
                                            var body = JsonConvert.SerializeObject(wallet);
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
                                                SPName = new ObjType().GetSPName(ObjType.SPType.HandleWallet);
                                                SQLHelper<SPInput_HandleWallet, SPOutput_Base> sqlHelpH = new SQLHelper<SPInput_HandleWallet, SPOutput_Base>(connetStr);
                                                flag = sqlHelpH.ExecuteSPNonQuery(SPName, SPInputH, ref SPoutputH, ref lstError);
                                                baseVerify.checkSQLResult(ref flag, ref SPoutputH, ref lstError, ref errCode);
                                            }
                                            else
                                            {
                                                errCode = "ERR";
                                                errMsg = output.Message;
                                                //要呼叫刷退
                                            }
                                            #endregion
                                            //使用錢包支付
                                            #region 錢包支付
                                            if (flag)
                                            {
                                                WebAPI_PayTransaction WalletPayTransaction = new WebAPI_PayTransaction()
                                                {
                                                    AccountId = SPOutput.WalletAccountID,
                                                    ApiVersion = "0.1.01",
                                                    GUID = guid,
                                                    MerchantId = MerchantId,
                                                    POSId = "",
                                                    SourceFrom = "9",
                                                    StoreId = "",
                                                    StoreName = "",
                                                    StoreTransId = string.Format("{0}F{1}", tmpOrder, NowTime.ToString("yyMMddHHmmss")),
                                                    Amount = Amount,
                                                    BarCode = "",
                                                    StoreTransDate = NowTime.ToString("yyyyMMddHHmmss")

                                                };
                                                 body = JsonConvert.SerializeObject(wallet);
                                                 WalletAPI = new TaishinWallet();
                                                 utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                                                 SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                                                WebAPIOutput_PayTransaction WalletPayOutput = null;
                                                flag = WalletAPI.DoPayTransaction(WalletPayTransaction, MerchantId, utcTimeStamp, SignCode, ref errCode, ref WalletPayOutput);
                                                if (flag)
                                                {
                                                    SPName = new ObjType().GetSPName(ObjType.SPType.DonePayRentBill);
                                                    PayInput.transaction_no = WalletPayTransaction.StoreTransId;
                                                    SPOutput_Base PayOutput = new SPOutput_Base();
                                                    SQLHelper<SPInput_DonePayRent, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_DonePayRent, SPOutput_Base>(connetStr);
                                                    flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                                                    baseVerify.checkSQLResult(ref flag, ref PayOutput, ref lstError, ref errCode);
                                                }
                                            }
                                            #endregion
                                            #endregion
                                        }

                                    }
                                    else
                                    {
                                        PayInput.transaction_no = "Free";
                                    }

                                    if (flag)
                                    {
                                        //if (OrderDataLists[0].ProjType == 4)
                                        //{
                                        //    bool Motorflag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                                        //    if (Motorflag == false)
                                        //    {
                                        //        //寫入車機錯誤
                                        //    }
                                        //}
                                        bool CarFlag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                                        if (CarFlag == false)
                                        {
                                            //寫入車機錯誤
                                        }
                                        SPName = new ObjType().GetSPName(ObjType.SPType.DonePayRentBill);
                                        SPOutput_Base PayOutput = new SPOutput_Base();
                                        SQLHelper<SPInput_DonePayRent, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_DonePayRent, SPOutput_Base>(connetStr);
                                        flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                                        baseVerify.checkSQLResult(ref flag, ref PayOutput, ref lstError, ref errCode);
                                    }

                                }
                                else
                                {
                                    flag = false;
                                    errCode = "ERR195";
                                }
                                #endregion

                            }
                        }
                        #endregion
                    }

                    #endregion
                }                
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
