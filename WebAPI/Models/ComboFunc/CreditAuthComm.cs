using Domain.Flow.Hotai;
using Domain.SP.Input;
using Domain.SP.Output;
using Domain.TB.Hotai;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
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
                WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();
                flag = WebAPI.DoCreditCardAuthV3(WSAuthInput, IDNO, autoClose, funName, insUser, ref errCode, ref WSAuthOutput, AuthInput.AuthType);
                logger.Trace("DoCreditCardAuth:" + JsonConvert.SerializeObject(WSAuthOutput));
                if(flag)
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

                AuthOutput.AuthCode = 
                    (WSAuthOutput.RtnCode == "1000") ? WSAuthOutput.ResponseParams.ResultCode : WSAuthOutput.RtnCode??"";
                AuthOutput.AuthMessage = 
                    (WSAuthOutput.RtnCode == "1000") ? WSAuthOutput.ResponseParams.ResultMessage: WSAuthOutput.RtnMessage??"";

                AuthOutput.CardType = CardType;
                AuthOutput.CheckoutMode = CheckoutMode;
                AuthOutput.Transaction_no = WSAuthInput.RequestParams.MerchantTradeNo;
                AuthOutput.BankTradeNo = WSAuthOutput?.ResponseParams?.ResultData?.ServiceTradeNo??"";
                AuthOutput.CardNo = WSAuthOutput?.ResponseParams?.ResultData?.CardNumber?? FindCardResult.cardNumber;
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
            bool flag = true;
            if(HotaiPayStatus == 0)
            {
                AuthInput.CheckoutMode = 0;
            }
            switch(AuthInput.CheckoutMode)
            {
                case 0:
                    flag = DoTaishinAuth(AuthInput, ref errCode, ref AuthOutput);
                    break;
                case 4:
                default:
                    flag = DoHotaiAuth(AuthInput, ref errCode, ref AuthOutput);
                    break;
            }
            
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
            
            if(flag)
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
            bool flag = true;

            int cardType = 0;
            int checkoutMode = 4;

            var FindCardResult = CheckHotaiBindCard(ref flag, AuthInput.IDNO, ref errCode);
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
                };
                
                flag = hotaipayService.DoReqPaymentAuth(WSAuthInput,ref WSAuthOutput, ref errCode);
                    
                logger.Trace("DoHotaiAuth:" + JsonConvert.SerializeObject(WSAuthOutput));
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

            AuthOutput.AuthCode = WSAuthOutput?.AuthCode??"";
            AuthOutput.AuthMessage = WSAuthOutput?.AuthMessage??"";
            AuthOutput.BankTradeNo = WSAuthOutput?.BankTradeNo??"";
            AuthOutput.CardNo = WSAuthOutput?.CardNo??"";
            AuthOutput.Transaction_no = WSAuthOutput?.Transaction_no??"";

            if (!flag)
            {
                errCode = "000000";
                return DoTaishinAuth(AuthInput, ref errCode, ref AuthOutput);
            }

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
            (string cardNumber, string cardToken) result = ("", "");

            HotaipayService hotaipayService = new HotaipayService();

            var input = new IFN_QueryDefaultCard
            { 
                IDNO= IDNO,
                PRGName = "CheckHotaiBindCard",
                insUser = "CheckHotaiBindCard",
                LogID = 0,
            };

            var card = new HotaiCardInfo();
            flag = hotaipayService.DoQueryDefaultCard(input, ref card, ref errCode);
           
            if (flag)
            {
                if(card != null & !string.IsNullOrEmpty(card.CardToken))
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
           
            return result;
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