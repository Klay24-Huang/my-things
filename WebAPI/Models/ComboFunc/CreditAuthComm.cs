using Domain.Flow.Hotai;
using Domain.SP.Input;
using Domain.SP.Input.OtherService.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Wallet;
using Domain.TB.Hotai;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output;
using Domain.WebAPI.output.Taishin;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using OtherService;
using OtherService.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Bill.Input;
using WebAPI.Models.Param.Bill.Output;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Service;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Models.ComboFunc
{

    /// <summary>
    /// 信用卡公用程式
    /// </summary>
    public class CreditAuthComm
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
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
        private string ApiVerQ = ConfigurationManager.AppSettings["ApiVerQ"].ToString();
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();
        private int HotaiPayStatus = int.Parse(ConfigurationManager.AppSettings["HotaiPayStatus"]);
        private string AzureAPIBaseURL = ConfigurationManager.AppSettings["AzureAPIBaseUrl"].ToString();

        private TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
        private CommonFunc baseVerify = new CommonFunc();
        /// <summary>
        /// 取得信用卡綁定列表
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="HasBind"></param>
        /// <param name="lstBindList"></param>
        /// <param name="errCode"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool DoQueryCardList(string IDNO, ref int HasBind, ref List<Models.Param.Output.PartOfParam.CreditCardBindList> lstBindList, ref string errCode, ref string errMsg)
        {
            bool flag = true;

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
                HasBind = (Len == 0) ? 0 : 1;
                lstBindList = new List<Models.Param.Output.PartOfParam.CreditCardBindList>();

                for (int i = 0; i < Len; i++)
                {
                    Models.Param.Output.PartOfParam.CreditCardBindList obj = new Models.Param.Output.PartOfParam.CreditCardBindList()
                    {
                        AvailableAmount = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].AvailableAmount),
                        BankNo = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].BankNo),
                        CardName = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].CardName),
                        CardNumber = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].CardNumber),

                        CardToken = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].CardToken)

                    };
                    lstBindList.Add(obj);
                }
            }
            else
            {
                errCode = wsOutput.RtnCode;
                errMsg = wsOutput.RtnMessage;

            }
            return flag;
        }
        /// <summary>
        /// 使用台新交易序號查詢該筆收費狀態
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="ServerOrderNo"></param>
        /// <param name="errCode"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool DoCreditCardQuery(string IDNO, string ServerOrderNo, ref WebAPIOutput_GetPaymentInfo WSQueryAuthOutput, ref string errCode, ref string errMsg)
        {
            bool flag = true;
            PartOfGetPaymentInfo WSCreditCardQueryInput = new PartOfGetPaymentInfo()
            {
                ApiVer = ApiVerQ,
                ApposId = TaishinAPPOS,
                RequestParams = new IGetPaymentInfoRequestParams()
                {
                    ServiceTradeNo = ServerOrderNo
                },
                Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                TransNo = string.Format("{0}_{1}", IDNO, DateTime.Now.ToString("yMMddhhmmssfff"))
            };

            flag = WebAPI.DoCreditCardAuthQuery(WSCreditCardQueryInput, ref errCode, ref WSQueryAuthOutput);
            return flag;

        }
        /// <summary>
        /// 刷退
        /// </summary>
        /// <param name="tmpOrder"></param>
        /// <param name="RefundAmount"></param>
        /// <param name="Remark"></param>
        /// <param name="CardToken"></param>
        /// <param name="OriMerchantTradeNo"></param>
        /// <param name="WSRefundOutput"></param>
        /// <param name="errCode"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool DoCreditRefund(Int64 tmpOrder, string IDNO, int RefundAmount, string Remark, string CardToken, string OriMerchantTradeNo, ref WebAPIOutput_ECRefund WSRefundOutput, ref string errCode, ref string errMsg)
        {
            bool flag = true;
            PartOfECRefund WSRefundInput = new PartOfECRefund()
            {
                ApiVer = ApiVer,
                ApposId = TaishinAPPOS,
                RequestParams = new EC_RefundRequestParams()
                {
                    TradeType = "5",
                    CardToken = CardToken,
                    MerchantTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                    MerchantTradeTime = DateTime.Now.ToString("HHmmss"),
                    MerchantTradeNo = string.Format("{0}{1}_{2}", tmpOrder, "R", DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                    OriMerchantTradeNo = OriMerchantTradeNo,
                    Item = new List<EC_RefundItem>(),
                    TradeAmount = (RefundAmount * 100).ToString(),
                    ResultUrl = BindResultURL,
                    Remark1 = Remark,
                    Remark2 = "",
                    Remark3 = ""

                },
                Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString()
            };
            flag = WebAPI.DoCreditRefund(WSRefundInput, tmpOrder, IDNO, ref errCode, ref errMsg, ref WSRefundOutput);
            return flag;
        }
        /// <summary>
        /// 刷卡
        /// </summary>
        /// <param name="OrderNo">訂單編號/罰單編號</param>
        /// <param name="Amount">金額</param>
        /// <param name="CardToken">卡號token</param>
        /// <param name="PayType">付款類型
        /// <para>0:租金</para>
        /// <para>1:罰金</para>
        /// <para>2:eTag</para>
        /// <para>3:補繳</para>
        /// </param>
        /// <param name="errCode"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>

        public bool DoAuth(string OrderNo, int Amount, string CardToken, int PayType, ref string errCode, ref string errMsg, ref WebAPIOutput_Auth WSAuthOutput)
        {
            bool flag = true;
            Int64 tmpOrder = 0;
            string TmpOrder = "";
            if (PayType == 0)
            {
                tmpOrder = Convert.ToInt64(OrderNo.Replace("H", ""));
            }
            else
            {
                TmpOrder = OrderNo;
            }

            string PayTypeStr = "";
            string PaySuff = "F";
            switch (PayType)
            {
                case 0:
                    PayTypeStr = "租金";
                    PaySuff = "F_";
                    break;
                case 1:
                    PayTypeStr = "罰金";
                    PaySuff = "P_";
                    break;
                case 2:
                    PayTypeStr = "eTag";
                    PaySuff = "E_";
                    break;
                case 3:
                    PayTypeStr = "補繳";
                    PaySuff = "G_";
                    break;


            }

            Domain.WebAPI.Input.Taishin.AuthItem item = new Domain.WebAPI.Input.Taishin.AuthItem()
            {
                Amount = Amount.ToString() + "00",
                Name = string.Format("{0}{1}", OrderNo, PayTypeStr),
                NonPoint = "N",
                NonRedeem = "N",
                Price = Amount.ToString() + "00",
                Quantity = "1"
            };
            PartOfCreditCardAuth WSAuthInput = new PartOfCreditCardAuth()
            {
                ApiVer = ApiVer,
                ApposId = TaishinAPPOS,
                RequestParams = new Domain.WebAPI.Input.Taishin.AuthRequestParams()
                {
                    CardToken = CardToken,
                    InstallPeriod = "0",
                    InvoiceMark = "N",
                    Item = new List<Domain.WebAPI.Input.Taishin.AuthItem>(),
                    MerchantTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                    MerchantTradeTime = DateTime.Now.ToString("HHmmss"),
                    //  MerchantTradeNo = string.Format("{0}{1}{2}", tmpOrder, PaySuff, DateTime.Now.ToString("yMMddHHmmssf")),
                    NonRedeemAmt = Amount.ToString() + "00",
                    NonRedeemdescCode = "",
                    Remark1 = "",
                    Remark2 = "",
                    Remark3 = "",
                    ResultUrl = BindResultURL,
                    TradeAmount = Amount.ToString() + "00",
                    TradeType = "1",
                    UseRedeem = "N"
                },
                Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),

            };
            if (PayType == 0)
            {
                WSAuthInput.RequestParams.MerchantTradeNo = string.Format("{0}{1}{2}", tmpOrder, PaySuff, WSAuthInput.TimeStamp);
            }
            else
            {
                WSAuthInput.RequestParams.MerchantTradeNo = string.Format("{0}{1}{2}", TmpOrder, PaySuff, WSAuthInput.TimeStamp);
            }
            WSAuthInput.RequestParams.Item.Add(item);


            flag = WebAPI.DoCreditCardAuth(WSAuthInput, ref errCode, ref WSAuthOutput);
            return flag;
        }


        public bool DoTaishinAuth(IFN_CreditAuthRequest AuthInput, ref string errCode, ref OFN_CreditAuthResult AuthOutput)
        {
            bool flag = true;

            int CardType = 1;
            int CheckoutMode = 0;
            string IDNO = AuthInput.IDNO;
            int PayType = AuthInput.PayType;
            int Amount = AuthInput.Amount;
            Int64 OrderNo = AuthInput.OrderNo;
            int autoClose = AuthInput.autoClose;
            string funName = AuthInput.funName;
            string insUser = AuthInput.insUser;

            var FindCardResult = CheckTaishinBindCard(ref flag, IDNO, ref errCode);

            if (flag)
            {
                string CardToken = FindCardResult.cardToken;

                var payInfoApi = new PayInfoForCredit();
                var temPayTypeInfo = payInfoApi.GetPayTypeInfo(PayType);
                string PayTypeStr = temPayTypeInfo.PayTypeStr;
                string PaySuff = temPayTypeInfo.PaySuff;
                string FrontPart = temPayTypeInfo.FrontPart;

                Thread.Sleep(1000);
                Domain.WebAPI.Input.Taishin.AuthItem item = new Domain.WebAPI.Input.Taishin.AuthItem()
                {
                    Amount = Amount.ToString() + "00",
                    Name = string.Format("{0}{1}", OrderNo, PayTypeStr),
                    NonPoint = "N",
                    NonRedeem = "N",
                    Price = Amount.ToString() + "00",
                    Quantity = "1"
                };
                PartOfCreditCardAuth WSAuthInput = new PartOfCreditCardAuth()
                {
                    ApiVer = ApiVer,
                    ApposId = TaishinAPPOS,
                    RequestParams = new Domain.WebAPI.Input.Taishin.AuthRequestParams()
                    {
                        CardToken = CardToken,
                        InstallPeriod = "0",
                        InvoiceMark = "N",
                        Item = new List<Domain.WebAPI.Input.Taishin.AuthItem>(),
                        MerchantTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                        MerchantTradeTime = DateTime.Now.ToString("HHmmss"),
                        MerchantTradeNo = string.Format("{0}{1}{2}", FrontPart == "OrderNo" ? OrderNo.ToString() : IDNO, PaySuff, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                        NonRedeemAmt = Amount.ToString() + "00",
                        NonRedeemdescCode = "",
                        Remark1 = "",
                        Remark2 = "",
                        Remark3 = "",
                        ResultUrl = BindResultURL,
                        TradeAmount = Amount.ToString() + "00",
                        TradeType = "1",
                        UseRedeem = "N"
                    },
                    Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                    TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),

                };
                WSAuthInput.RequestParams.Item.Add(item);
                logger.Trace(string.Format("DoCreditCardAuth Start | input:{0}", JsonConvert.SerializeObject(WSAuthInput)));
                WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();
                flag = WebAPI.DoCreditCardAuthV3(WSAuthInput, IDNO, autoClose, funName, insUser, ref errCode, ref WSAuthOutput, AuthInput.AuthType);
                logger.Trace(string.Format("DoCreditCardAuth End | flag:{0} errCode:{1} Result:{2}", flag, errCode, JsonConvert.SerializeObject(WSAuthOutput)));
                if (flag)
                {
                    if (WSAuthOutput.RtnCode != "1000")
                    {
                        flag = false;
                        errCode = "ERR197";
                    }
                    //修正錯誤偵測
                    if (WSAuthOutput.RtnCode == "1000" && WSAuthOutput.ResponseParams?.ResultCode != "1000")
                    {
                        flag = false;
                        errCode = "ERR197";
                    }
                }

                AuthOutput.AuthCode = (WSAuthOutput.RtnCode == "1000") ? WSAuthOutput.ResponseParams.ResultCode : WSAuthOutput.RtnCode ?? "";
                AuthOutput.AuthMessage = (WSAuthOutput.RtnCode == "1000") ? WSAuthOutput.ResponseParams.ResultMessage : WSAuthOutput.RtnMessage ?? "";
                AuthOutput.CardType = CardType;
                AuthOutput.CheckoutMode = CheckoutMode;
                AuthOutput.Transaction_no = WSAuthInput.RequestParams.MerchantTradeNo;
                AuthOutput.BankTradeNo = WSAuthOutput?.ResponseParams?.ResultData?.ServiceTradeNo ?? "";
                AuthOutput.CardNo = WSAuthOutput?.ResponseParams?.ResultData?.CardNumber ?? FindCardResult.cardNumber;
                AuthOutput.AuthIdResp = WSAuthOutput?.ResponseParams?.ResultData?.AuthIdResp ?? "0000";
            }

            return flag;
        }


        public bool DoAuthV3(long OrderNo, string IDNO, int Amount, string CardToken, int PayType, ref string errCode, int autoClose, string funName, string insUser, ref WebAPIOutput_Auth WSAuthOutput)
        {
            bool flag = true;
            var payInfoApi = new PayInfoForCredit();
            var payTypeInfos = payInfoApi.GetCreditCardPayInfoColl();

            string PayTypeStr = "";
            string PaySuff = "F_";
            string FrontPart = "IDNO";

            var temPayTypeInfo = payTypeInfos.Where(p => p.PayType == PayType).FirstOrDefault();
            PayTypeStr = temPayTypeInfo?.PayTypeStr ?? PayTypeStr;
            PaySuff = temPayTypeInfo?.PayTypeCode ?? PaySuff;
            FrontPart = temPayTypeInfo?.FrontPart ?? FrontPart;
            //switch (PayType)
            //{
            //    case 0:
            //        PayTypeStr = "租金";
            //        PaySuff = "F_";
            //        break;
            //    case 1:
            //        PayTypeStr = "罰金";
            //        PaySuff = "P_";
            //        break;
            //    case 2:
            //        PayTypeStr = "eTag";
            //        PaySuff = "E_";
            //        break;
            //    case 3:
            //        PayTypeStr = "補繳";
            //        PaySuff = "G_";
            //        break;
            //}
            Thread.Sleep(1000);
            Domain.WebAPI.Input.Taishin.AuthItem item = new Domain.WebAPI.Input.Taishin.AuthItem()
            {
                Amount = Amount.ToString() + "00",
                Name = string.Format("{0}{1}", OrderNo, PayTypeStr),
                NonPoint = "N",
                NonRedeem = "N",
                Price = Amount.ToString() + "00",
                Quantity = "1"
            };
            PartOfCreditCardAuth WSAuthInput = new PartOfCreditCardAuth()
            {
                ApiVer = ApiVer,
                ApposId = TaishinAPPOS,
                RequestParams = new Domain.WebAPI.Input.Taishin.AuthRequestParams()
                {
                    CardToken = CardToken,
                    InstallPeriod = "0",
                    InvoiceMark = "N",
                    Item = new List<Domain.WebAPI.Input.Taishin.AuthItem>(),
                    MerchantTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                    MerchantTradeTime = DateTime.Now.ToString("HHmmss"),
                    //todo 0,6
                    MerchantTradeNo = string.Format("{0}{1}{2}", FrontPart == "OrderNo" ? OrderNo.ToString() : IDNO, PaySuff, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                    NonRedeemAmt = Amount.ToString() + "00",
                    NonRedeemdescCode = "",
                    Remark1 = "",
                    Remark2 = "",
                    Remark3 = "",
                    ResultUrl = BindResultURL,
                    TradeAmount = Amount.ToString() + "00",
                    TradeType = "1",
                    UseRedeem = "N"
                },
                Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),

            };
            WSAuthInput.RequestParams.Item.Add(item);

            flag = WebAPI.DoCreditCardAuthV3(WSAuthInput, IDNO, autoClose, funName, insUser, ref errCode, ref WSAuthOutput);
            logger.Trace("DoCreditCardAuth:" + JsonConvert.SerializeObject(WSAuthOutput));

            if (WSAuthOutput.RtnCode != "1000")
            {
                flag = false;
                errCode = "ERR197";
            }
            //修正錯誤偵測
            if (WSAuthOutput.RtnCode == "1000" && WSAuthOutput.ResponseParams.ResultCode != "1000")
            {
                flag = false;
                errCode = "ERR197";
            }
            return flag;
        }

        public bool DoAuthV4(IFN_CreditAuthRequest AuthInput, ref string errCode, ref OFN_CreditAuthResult AuthOutput)
        {
            logger.Trace("DoAuthV4 | Start:" + JsonConvert.SerializeObject(AuthInput));
            bool flag = true;
            if (HotaiPayStatus == 0 && AuthInput.CheckoutMode != 1)
            {
                AuthInput.CheckoutMode = 0;
            }
            switch (AuthInput.CheckoutMode)
            {
                case 0:
                    flag = DoTaishinAuth(AuthInput, ref errCode, ref AuthOutput);
                    break;
                case 1:
                    flag = DoWalletAuth(AuthInput, ref errCode, ref AuthOutput);
                    break;
                case 4:
                default:
                    flag = DoHotaiAuth(AuthInput, ref errCode, ref AuthOutput);
                    break;
            }

            logger.Trace(string.Format("DoAuthV4 | End: flag:{0} errCode:{1} Result:{2}", flag, errCode, JsonConvert.SerializeObject(AuthOutput)));
            return flag;
        }


        /// <summary>
        /// 檢查台新信用卡是否綁卡
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="IDNO">會員編號</param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public (string cardNumber, string cardToken) CheckTaishinBindCard(ref bool flag, string IDNO, ref string errCode)
        {
            (string cardNumber, string cardToken) result = ("", "");
            //20201219 ADD BY JERRY 更新綁卡查詢邏輯，改由資料庫查詢
            DataSet ds = Common.getBindingList(IDNO, ref flag, ref errCode, ref errCode);

            if (flag)
            {
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    result.cardToken = ds.Tables[0].Rows[0]["CardToken"].ToString();
                    result.cardNumber = ds.Tables[0].Rows[0]["CardNumber"].ToString();
                }
                else
                {
                    flag = false;
                    errCode = "ERR730";
                }
            }

            ds.Dispose();
            return result;
        }


        public bool DoHotaiAuth(IFN_CreditAuthRequest AuthInput, ref string errCode, ref OFN_CreditAuthResult AuthOutput)
        {
            logger.Trace("DoHotaiAuth | Strat:" + JsonConvert.SerializeObject(AuthInput));

            bool flag = true;

            int cardType = 0;
            int checkoutMode = 4;

            var FindCardResult = CheckHotaiBindCard(ref flag, AuthInput.IDNO, ref errCode);

            logger.Trace($"DoHotaiAuth | CheckHotaiBindCard |IDNO:{AuthInput.IDNO} | flag:{flag} | errCode:{errCode} |Result:{JsonConvert.SerializeObject(FindCardResult)} ");

            var WSAuthOutput = new OFN_HotaiPaymentAuth();
            if (flag)
            {
                HotaipayService hotaipayService = new HotaipayService();
                string cardToken = FindCardResult.cardToken;

                Thread.Sleep(1000);

                var WSAuthInput = new IFN_HotaiPaymentAuth
                {
                    CardToken = cardToken,
                    Amount = AuthInput.Amount,
                    OrderNo = AuthInput.OrderNo,
                    IDNO = AuthInput.IDNO,
                    AutoClose = AuthInput.autoClose,
                    PayType = AuthInput.PayType,
                    AuthType = AuthInput.AuthType,
                    PromoCode = "",
                    PRGName = AuthInput.funName,
                    insUser = AuthInput.insUser
                };

                flag = hotaipayService.DoReqPaymentAuth(WSAuthInput, ref WSAuthOutput, ref errCode);

                logger.Trace($"DoHotaiAuth | DoReqPaymentAuth | flag:{flag} | errCode:{errCode} | Result {JsonConvert.SerializeObject(WSAuthOutput)} ");
            }

            if (flag)
            {
                if (WSAuthOutput.RtnCode != "1000")
                {
                    flag = false;
                    errCode = "ERR197";
                }
            }

            AuthOutput.CardType = cardType;
            AuthOutput.CheckoutMode = checkoutMode;

            AuthOutput.AuthCode = WSAuthOutput?.AuthCode ?? "";
            AuthOutput.AuthMessage = WSAuthOutput?.AuthMessage ?? "";
            AuthOutput.BankTradeNo = WSAuthOutput?.BankTradeNo ?? "";
            AuthOutput.CardNo = WSAuthOutput?.CardNo ?? "";
            AuthOutput.Transaction_no = WSAuthOutput?.Transaction_no ?? "";

            if (!flag)
            {
                errCode = "000000";
                return DoTaishinAuth(AuthInput, ref errCode, ref AuthOutput);
            }

            logger.Trace($"DoHotaiAuth | End | flag:{flag} | errCode:{errCode} | AuthOutput : {JsonConvert.SerializeObject(AuthOutput)} ");
            return flag;

        }


        /// <summary>
        /// 檢查和泰信用卡是否綁卡
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="IDNO">會員編號</param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public (string cardNumber, string cardToken) CheckHotaiBindCard(ref bool flag, string IDNO, ref string errCode)
        {

            logger.Trace($"CheckHotaiBindCard | Strat | IDNO:{IDNO}");
            (string cardNumber, string cardToken) result = ("", "");

            HotaipayService hotaipayService = new HotaipayService();

            var input = new IFN_QueryDefaultCard
            {
                IDNO = IDNO,
                PRGName = "CheckHotaiBindCard",
                insUser = "CheckHotaiBindCard",
                LogID = 0,
            };

            var card = new HotaiCardInfo();
            flag = hotaipayService.DoQueryDefaultCard(input, ref card, ref errCode);
            logger.Trace($"CheckHotaiBindCard | DoQueryDefaultCard | input:{JsonConvert.SerializeObject(input)} | card : {JsonConvert.SerializeObject(card)}");
            if (flag)
            {
                if (card != null & !string.IsNullOrEmpty(card.CardToken))
                {
                    result.cardToken = card.CardToken;
                    result.cardNumber = card.CardNumber;
                }
                else
                {
                    flag = false;
                    errCode = "ERR730";
                }
            }
            logger.Trace($"CheckHotaiBindCard | End | input:{JsonConvert.SerializeObject(input)}  | flag : {flag} | errCode : {errCode} | result : {JsonConvert.SerializeObject(result)}");

            return result;
        }

        public bool DoWalletAuth(IFN_CreditAuthRequest AuthInput, ref string errCode, ref OFN_CreditAuthResult AuthOutput)
        {
            bool flag = true;

            int cardType = 2;
            int checkoutMode = 1;

            //var orderPayForWallet = PayWalletFlow(AuthInput.OrderNo, AuthInput.Amount, AuthInput.IDNO, AuthInput.TradeType, true, AuthInput.funName,AuthInput.LogID, AuthInput.Token, AuthInput.InputSource,AuthInput.PayType, ref errCode);
            var orderPayForWallet = PayWalletFlow(AuthInput, AuthInput.AutoStore, ref errCode);
            logger.Trace($"DoWalletAuth | Start: AuthInput:{ JsonConvert.SerializeObject(AuthInput)} | orderPayForWallet {JsonConvert.SerializeObject(orderPayForWallet)}");

            flag = orderPayForWallet.flag;

            //if(!flag)
            //{
            //    errCode = "000000";
            //    AuthInput.CheckoutMode = 4;
            //    return DoAuthV4(AuthInput, ref errCode, ref AuthOutput);
            //}

            AuthOutput = orderPayForWallet.paymentInfo;
            AuthOutput.CardType = cardType;
            AuthOutput.CheckoutMode = checkoutMode;

            //AuthOutput.AuthCode = "";
            //AuthOutput.AuthMessage = "";
            //AuthOutput.BankTradeNo = orderPayForWallet.paymentInfo?.TransId ?? "";
            //AuthOutput.CardNo = orderPayForWallet.paymentInfo?.WalletAccountID ?? "";
            //AuthOutput.Transaction_no = orderPayForWallet.paymentInfo?.StoreTransId ?? "";

            return flag;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrderNo">訂單編號</param>
        /// <param name="Amount">交易金額'</param>
        /// <param name="IDNO">帳號</param>
        /// <param name="TradeType"></param>
        /// <param name="autoStoreFlowFlag"></param>
        /// <param name="funName"></param>
        /// <param name="LogID"></param>
        /// <param name="Access_Token"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        private (bool flag, OFN_CreditAuthResult paymentInfo) PayWalletFlow(IFN_CreditAuthRequest AuthInput, bool autoStoreFlowFlag, ref string errCode)
        {
            (bool flag, OFN_CreditAuthResult paymentInfo) result = (false, new OFN_CreditAuthResult());

            logger.Trace($"PayWalletFlow | Start: AuthInput:{ JsonConvert.SerializeObject(AuthInput)} | autoStoreFlowFlag :{autoStoreFlowFlag}");

            //扣款金額
            int PayAmount = 0;
            //取得錢包狀態
            var WalletStatus = GetWalletInfo(AuthInput.IDNO, AuthInput.LogID, AuthInput.Token, AuthInput.InputSource);
            logger.Trace($"PayWalletFlow | 取得錢包狀態: WalletStatus:{ JsonConvert.SerializeObject(WalletStatus)}");
            result.flag = WalletStatus.flag;
            if (!result.flag)
            {
                //未開通
                errCode = "ERR932";
                return result;
            }

            //錢包餘額<訂單金額
            if (WalletStatus.WalletInfo.Balance < AuthInput.Amount)
            {
                bool autoStoreFlag = (WalletStatus.WalletInfo.AutoStoreFlag == 1 
                    || AuthInput.OnceStore == 1) 
                    ? true : false;

                ////如果自動儲值是on
                if (autoStoreFlowFlag && autoStoreFlag)
                {
                    //儲值.....儲值金額(訂單-錢包)
                    var storeAmount = AuthInput.Amount - WalletStatus.WalletInfo.Balance;
                    logger.Trace($"PayWalletFlow | 餘額不足 進入自動儲值 : storeAmount:{ storeAmount} ");
                    string Access_Token = "";
                    if (string.IsNullOrEmpty(AuthInput.Token))
                    {
                        CommonFunc comboFunc = new CommonFunc();
                        bool getTokenflag = comboFunc.GetTokenFromID(AuthInput.IDNO, ref Access_Token, ref errCode);
                    }
                    else
                    {
                        Access_Token = AuthInput.Token;
                    }

                    bool storeSataus = WalletStoreByCredit(storeAmount, Access_Token, AuthInput.funName, ref errCode);
                    logger.Trace($"PayWalletFlow | 自動儲值結果 進入自動儲值 : storeSataus:{ storeSataus} ");
                    if (storeSataus)
                    {
                        errCode = "000000";
                        return PayWalletFlow(AuthInput, true, ref errCode);
                        //PayWalletFlow(OrderNo, Amount, IDNO, TradeType, true, funName, LogID, Access_Token, ref errCode);
                    }
                    else
                    {
                        if (AuthInput.TradeType != "Pay_Arrear")
                        {
                            errCode = "000000";
                            return PayWalletFlow(AuthInput, false, ref errCode);
                            //return PayWalletFlow(OrderNo, Amount, IDNO, TradeType, false, funName, LogID, Access_Token, ref errCode);
                        }
                    }
                }

                PayAmount = WalletStatus.WalletInfo.Balance;
            }
            else //錢包餘額>=訂單金額
            {
                PayAmount = AuthInput.Amount;
            }

            //欠費金額判斷
            if (AuthInput.TradeType == "Pay_Arrear" || AuthInput.TradeType == "PreAuth_Motor" || AuthInput.TradeType == "PreAuth_Car")
            {
                //欠費和預售權一定要全繳
                result.flag = IsWalletPayAmountEnough(PayAmount, AuthInput.Amount);
                if (!result.flag)
                {
                    //餘額不足
                    errCode = "ERR934";
                    return result;
                }
            }
            //扣款
            //return DoWalletPay(PayAmount, IDNO, OrderNo, TradeType, funName, LogID, Access_Token, ref errCode, InputSource, PayType);
            return DoWalletPay(AuthInput, PayAmount, ref errCode);
        }

        /// <summary>
        /// 錢包扣款
        /// </summary>
        /// <param name="Amount">扣款金額</param>
        /// <param name="IDNO">扣款帳號</param>
        /// <param name="OrderNo">扣款訂單編號</param>
        /// <returns></returns>
        //private (bool flag, SPInput_WalletPay paymentInfo) DoWalletPay(int Amount, string IDNO, long OrderNo, string TradeType, string PRGName, long LogID, string Access_Token, ref string errCode,int InputSource,int PayType)
        private (bool flag, OFN_CreditAuthResult paymentInfo) DoWalletPay(IFN_CreditAuthRequest AuthInputint, int Amount, ref string errCode)
        {
            (bool flag, OFN_CreditAuthResult paymentInfo) result = (false, new OFN_CreditAuthResult());

            logger.Trace($"DoWalletPay | Start: AuthInput:{ JsonConvert.SerializeObject(AuthInputint)} | Amount :{Amount}");

            result.flag = IsWalletPayAmountEnough(Amount, 0);

            if (!result.flag)
            {
                errCode = "ERR934";
                logger.Trace($"DoWalletPay | IsWalletPayAmountEnough : result:{result.flag} | AuthInput:{ JsonConvert.SerializeObject(AuthInputint)} | Amount :{Amount}");
                return result;
            }
            Thread.Sleep(1000);
            DateTime NowTime = DateTime.Now;
            //設定錢包付款參數
            var WebAPI = new PayInfoForCredit();
            var temPayTypeInfo = WebAPI.GetPayTypeInfo(AuthInputint.PayType);
            var accountId = GetWalletAccountId(AuthInputint.IDNO, 1);
            logger.Trace($"DoWalletPay | GetWalletAccountId : accountId:{accountId} | AuthInput:{ JsonConvert.SerializeObject(AuthInputint)} | Amount :{Amount}");
            var spGetTransIdInput = new SPInput_WalletPayGetTransId
            {
                LogID = AuthInputint.LogID,
                Amount = Amount,
                OrderNo = AuthInputint.OrderNo,
                MemberID = AuthInputint.IDNO,
                WalletAccountID = accountId,
                CreditType = AuthInputint.PayType,
                PrgName = AuthInputint.funName,
                PrgUser = AuthInputint.insUser,
                TransIdLeft = string.Format("{0}{1}"
                       , temPayTypeInfo.FrontPart.Equals("OrderNo") ? AuthInputint.OrderNo.ToString() : AuthInputint.IDNO
                       , temPayTypeInfo.PaySuff
                       )
            };
            var wsp = new WalletSp();
            var reTransIdInput = wsp.sp_GetWalletPayGetTransId(spGetTransIdInput, ref errCode);

            logger.Trace($"DoWalletPay | 取得錢包扣款TransId | spGetTransIdInput:{spGetTransIdInput} |  TransIdOutput : {reTransIdInput} ");

            result.flag = reTransIdInput.flag;

            if (!result.flag)
            {
                errCode = "ERR933";
                return result;
            }

            string storeTransId = reTransIdInput.spOut?.StoreTransId ?? "";
            result.flag = string.IsNullOrEmpty(storeTransId) ? false : true;

            if (!result.flag)
            {
                errCode = "ERR933";
                return result;
            }

            WebAPI_PayTransaction wallet = SetForWalletPay(AuthInputint.IDNO, AuthInputint.OrderNo, Amount, NowTime, AuthInputint.PayType, accountId, storeTransId);
            WebAPIOutput_PayTransaction taishinResponse = null;

            logger.Trace($"DoWalletPay | SetForWalletPay | wallet:{wallet} ");

            if (result.flag)
            {
                var body = JsonConvert.SerializeObject(wallet);
                TaishinWallet WalletAPI = new TaishinWallet();
                string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                string SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                logger.Trace($"DoWalletPay | GenerateSignCode | Input:{wallet} | SignCode:{SignCode} ");

                result.flag = WalletAPI.DoPayTransaction(wallet, MerchantId, utcTimeStamp, SignCode, ref errCode, ref taishinResponse);
                logger.Trace($"DoWalletPay | DoPayTransaction |Result :{result.flag}| Input:{wallet} | MerchantId:{MerchantId} | utcTimeStamp :{utcTimeStamp} | SignCode :{SignCode} |errCode:{errCode} |taishinResponse :{taishinResponse}");

            }
            var paymentInfo = new OFN_CreditAuthResult()
            {
                AuthCode = taishinResponse.ReturnCode,
                AuthMessage = taishinResponse.Message,
                BankTradeNo = taishinResponse?.Result?.TransId ?? "",
                CardNo = taishinResponse?.Result?.AccountId ?? "",
                Transaction_no = taishinResponse?.Result?.StoreTransId ?? ""
            };

            if (result.flag)
            {
                //設定錢包付款參數寫入
                SPInput_WalletPay spInput = SetForWalletPayLog(wallet, taishinResponse, AuthInputint, NowTime);

                result.flag = wsp.sp_WalletPay(spInput, ref errCode);
                logger.Trace($"DoWalletPay | sp_WalletPay |Result :{result.flag}| Input:{spInput} | errCode:{errCode} ");
            }
            else
            {
                errCode = "ERR933";//扣款失敗
            }
            result.paymentInfo = paymentInfo;
            return result;
        }
        /// <summary>
        /// 取得錢包狀態
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="LogID"></param>
        /// <param name="Access_Token"></param>
        /// <returns></returns>
        public (bool flag, PayModeObj WalletInfo) GetWalletInfo(string IDNO, long LogID, string Access_Token, short InputSource = 2)
        {
            var lstError = new List<ErrorInfo>();
            //string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            OAPI_GetPayInfo apiOutput = null;
            (bool flag, PayModeObj WalletInfo) re = (false, new PayModeObj());


            SPInput_GetPayInfo spInput = new SPInput_GetPayInfo()
            {
                LogID = LogID,
                Token = Access_Token,
                IDNO = IDNO,
                InputSource = InputSource
            };

            var wsp = new WalletSp();
            var GetPayModeResult = wsp.sp_GetPayInfo(spInput, ref errCode);

            if (GetPayModeResult.flag && GetPayModeResult.PayMode.Count > 0)
            {
                apiOutput = GetPayModeResult.PayMode
                    .Select(t => new OAPI_GetPayInfo
                    {
                        DefPayMode = t.DefPayMode,
                        PayModeBindCount = t.PayModeBindCount,
                        PayModeList = System.Text.Json.JsonSerializer.Deserialize<List<PayModeObj>>(GetPayModeResult.PayMode[0].PayModeList)
                    }).FirstOrDefault();
            }

            PayModeObj WalletInfo = apiOutput?.PayModeList.Where(t => t.PayMode == 1).FirstOrDefault();

            if (WalletInfo?.HasBind == 1)
            {
                re.flag = true;
                re.WalletInfo = WalletInfo;
            }

            return re;
        }

        private string GetWalletAccountId(string IDNO, int cnt)
        {
            return $"{IDNO}Wallet{cnt.ToString().PadLeft(4, '0')}";
        }

        private int GetWalletHistoryMode(string TradeType)
        {
            switch (TradeType)
            {
                case "Pay_Arrear":
                    return 5;
                case "pay_Car":
                case "Pay_Motor":
                case "PreAuth_Motor":
                case "PreAuth_Car":
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 設定扣款參數
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="OrderNo"></param>
        /// <param name="Amount"></param>
        /// <param name="NowTime"></param>
        /// <returns></returns>
        private WebAPI_PayTransaction SetForWalletPay(string IDNO, long OrderNo, int Amount, DateTime NowTime, int PayType, string AccountId, string StoreTransId)
        {
            string guid = Guid.NewGuid().ToString().Replace("-", "");

            return new WebAPI_PayTransaction()
            {
                AccountId = AccountId,
                ApiVersion = "0.1.01",
                GUID = guid,
                MerchantId = MerchantId,
                POSId = "",
                SourceFrom = "9",
                StoreId = "",
                StoreName = "",
                StoreTransId = StoreTransId,
                Amount = Amount,
                BarCode = "",
                StoreTransDate = NowTime.ToString("yyyyMMddHHmmss")
            };
        }

        /// <summary>
        /// 設定歷程寫入參數
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="taishinResponse"></param>
        /// <param name="IDNO"></param>
        /// <param name="OrderNo"></param>
        /// <param name="LogID"></param>
        /// <param name="Access_Token"></param>
        /// <param name="NowTime"></param>
        /// <param name="TradeType"></param>
        /// <param name="PRGName"></param>
        /// <param name="InputSource"></param>
        /// <returns></returns>
        //private SPInput_WalletPay SetForWalletPayLog(WebAPI_PayTransaction wallet, WebAPIOutput_PayTransaction taishinResponse
        //    , string IDNO, long OrderNo, long LogID, string Access_Token, DateTime NowTime, string TradeType, string PRGName)

        private SPInput_WalletPay SetForWalletPayLog(WebAPI_PayTransaction wallet, WebAPIOutput_PayTransaction taishinResponse
        , IFN_CreditAuthRequest AuthInputint, DateTime NowTime)
        {
            return new SPInput_WalletPay()
            {
                LogID = AuthInputint.LogID,
                Token = AuthInputint.Token,
                IDNO = AuthInputint.IDNO,
                OrderNo = AuthInputint.OrderNo,
                WalletMemberID = wallet.AccountId,
                WalletAccountID = wallet.AccountId,
                Amount = wallet.Amount,
                WalletBalance = taishinResponse.Result.Amount,
                TransDate = NowTime,
                StoreTransId = taishinResponse.Result.StoreTransId,
                TransId = taishinResponse.Result.TransId,
                TradeType = AuthInputint.TradeType,
                PRGName = AuthInputint.funName,
                Mode = GetWalletHistoryMode(AuthInputint.TradeType),
                InputSource = AuthInputint.InputSource
            };

        }

        private bool IsWalletPayAmountEnough(int payAmount, int baseAmount)
        {
            if (baseAmount == 0)
                return (payAmount > baseAmount);
            return payAmount >= baseAmount;
        }


        /// <summary>
        /// 用OrderAuth裡的CarcType取得CheckoutMode
        /// </summary>
        /// <param name="cardType"></param>
        /// <returns></returns>
        public int GetCheckoutModeByCardType(int cardType)
        {
            switch (cardType)
            {
                case 0:
                    return 4;
                case 1:
                    return 0;
                case 2:
                    return 1;
                default:
                    return -1;
            }
        }
        /// <summary>
        /// 信用卡錢包儲值
        /// </summary>
        /// <param name="storeMoney"></param>
        /// <param name="accessToken"></param>
        /// <param name="funName"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool WalletStoreByCredit(int storeMoney, string accessToken, string funName, ref string errCode)
        {
            bool flag = false;
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                IAPI_WalletStoredByCredit Input = new IAPI_WalletStoredByCredit { StoreMoney = storeMoney, StoreType = 4 };
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                DateTime MKTime = DateTime.Now;
                DateTime RTime = MKTime;

                string url = $@"{AzureAPIBaseURL}api/WalletStoredByCredit";
                var resault = ApiPost.DoApiPost<JObject, IAPI_WalletStoredByCredit>(Input, url, accessToken);

                try
                {
                    if (resault.Succ)
                    {
                        RTime = DateTime.Now;
                        JsonSerializer serializer = new JsonSerializer();
                        var p =
                            (IRentAPIOutput_Generic<OAPI_WalletStoreBase>)serializer.Deserialize(new JTokenReader(resault.Data), typeof(IRentAPIOutput_Generic<OAPI_WalletStoreBase>));
                        if (p.result == 1)
                        {
                            flag = (p.result == 1) ? true : false;
                        }
                        else
                        {
                            errCode = p.ErrorCode;
                        }
                    }
                    else
                    {
                        errCode = resault.errCode;
                    }
                }
                finally
                {
                    SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                    {
                        MKTime = MKTime,
                        UPDTime = RTime,
                        WebAPIInput = JsonConvert.SerializeObject(Input),
                        WebAPIName = funName,
                        WebAPIOutput = JsonConvert.SerializeObject(resault),
                        WebAPIURL = url
                    };
                    logger.Trace($" WalletStoreByCredit | flag:{flag} | accessToken:{accessToken} | errCode:{errCode} | InsWebAPILog {JsonConvert.SerializeObject(SPInput)} ");
                    bool logflag = false;
                    new WebAPILogCommon().InsWebAPILog(SPInput, ref logflag, ref errCode, ref lstError);
                }
            }
            return flag;
        }



        //搬到Other Service
        //public (string PayTypeStr, string PaySuff, string FrontPart) GetPayTypeInfo(int payType)
        //{
        //    (string PayTypeStr, string PaySuff, string FrontPart) rtnObj = ("", "F_", "IDNO");
        //    var payTypeInfos = WebAPI.GetCreditCardPayInfoColl();

        //    string payTypeStr = "";
        //    string paySuff = "F_";
        //    string FrontPart = "IDNO";


        //    var temPayTypeInfo = payTypeInfos.Where(p => p.PayType == payType).FirstOrDefault();

        //    rtnObj.PayTypeStr = temPayTypeInfo?.PayTypeStr ?? payTypeStr;
        //    rtnObj.PaySuff = temPayTypeInfo?.PayTypeCode ?? paySuff;
        //    rtnObj.FrontPart = temPayTypeInfo?.FrontPart ?? FrontPart;
        //    return rtnObj;

        //}
    }
}