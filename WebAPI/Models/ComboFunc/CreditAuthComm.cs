using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.output.Taishin;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WebAPI.Models.BaseFunc;

namespace WebAPI.Models.ComboFunc
{
  
    /// <summary>
    /// 信用卡公用程式
    /// </summary>
    public class CreditAuthComm
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
        private  TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
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
        public bool DoQueryCardList(string IDNO,ref int HasBind,ref List<Models.Param.Output.PartOfParam.CreditCardBindList> lstBindList,ref string errCode,ref string errMsg)
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
        /// 
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

        public bool DoAuth(string OrderNo,int Amount,string CardToken,int PayType,ref string errCode, ref string errMsg)
        {
            bool flag = true;
            Int64 tmpOrder = Convert.ToInt64(OrderNo.Replace("H", ""));
            string PayTypeStr = "";
            switch (PayType)
            {
                case 0:
                    PayTypeStr = "租金";
                    break;
                case 1:
                    PayTypeStr = "罰金";
                    break;
                case 2:
                    PayTypeStr = "eTag";
                    break;
                case 3:
                    PayTypeStr = "補繳";
                    break;
                

            }

            Domain.WebAPI.Input.Taishin.AuthItem item = new Domain.WebAPI.Input.Taishin.AuthItem()
            {
                Amount = Amount.ToString() + "00",
                Name = string.Format("{0}{1}", OrderNo,PayTypeStr),
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
                    MerchantTradeNo = string.Format("{0}F{1}", tmpOrder, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
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
            flag = WebAPI.DoCreditCardAuth(WSAuthInput, ref errCode, ref WSAuthOutput);
            return flag;
        }
    }
}