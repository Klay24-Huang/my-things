using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherService.Common
{
    public class PayInfoForCredit
    {
        public List<CreditCardPayInfo> GetCreditCardPayInfoColl()
        {
            List<CreditCardPayInfo> CreditCardPayInfoColl = new List<CreditCardPayInfo>()
            {
                new CreditCardPayInfo{ PayType = 0,PayTypeStr = "租金",PayTypeCode="F_",FrontPart="OrderNo"},
                new CreditCardPayInfo{ PayType = 1,PayTypeStr = "罰金",PayTypeCode="P_",FrontPart="IDNO"},//沒在用
                new CreditCardPayInfo{ PayType = 2,PayTypeStr = "eTag",PayTypeCode="E_",FrontPart="IDNO"},//沒在用
                new CreditCardPayInfo{ PayType = 3,PayTypeStr = "補繳",PayTypeCode="G_",FrontPart="IDNO"},
                new CreditCardPayInfo{ PayType = 4,PayTypeStr = "訂閱",PayTypeCode="M_",FrontPart="IDNO"},
                new CreditCardPayInfo{ PayType = 5,PayTypeStr = "訂閱",PayTypeCode="MA_",FrontPart="IDNO"},
                new CreditCardPayInfo{ PayType = 6,PayTypeStr = "春節訂金",PayTypeCode="D_",FrontPart="OrderNo"},
                new CreditCardPayInfo{ PayType = 7,PayTypeStr = "錢包",PayTypeCode="W_",FrontPart="IDNO"},
                new CreditCardPayInfo{ PayType = 8,PayTypeStr = "錢包提領",PayTypeCode="WD_",FrontPart="IDNO"},
            };

            return CreditCardPayInfoColl;
        }

        public (string PayTypeStr, string PaySuff, string FrontPart) GetPayTypeInfo(int payType)
        {
            (string PayTypeStr, string PaySuff, string FrontPart) rtnObj = ("", "F_", "IDNO");
            var payTypeInfos = GetCreditCardPayInfoColl();

            string payTypeStr = "";
            string paySuff = "F_";
            string FrontPart = "IDNO";


            var temPayTypeInfo = payTypeInfos.Where(p => p.PayType == payType).FirstOrDefault();

            rtnObj.PayTypeStr = temPayTypeInfo?.PayTypeStr ?? payTypeStr;
            rtnObj.PaySuff = temPayTypeInfo?.PayTypeCode ?? paySuff;
            rtnObj.FrontPart = temPayTypeInfo?.FrontPart ?? FrontPart;
            return rtnObj;

        }

        public (int creditType, string OrderString, Int64 OrderNo) GetOrderInfoFromMerchantTradeNo(string ori)
        {
            (int creditType, string OrderString, Int64 OrderNo) orderInfo = (99, "", 0);
            
            var payTypeInfos = GetCreditCardPayInfoColl();

            foreach (var type in payTypeInfos)
            {
                var index = ori.IndexOf(type.PayTypeCode);
                if (index > -1)
                {
                    orderInfo.creditType = type.PayType;
                    orderInfo.OrderString = ori.Substring(0, index);
                    if (type.FrontPart == "OrderNo")
                    {
                        orderInfo.OrderNo = Convert.ToInt64(orderInfo.OrderString);
                    }
                    break;
                }
            }

            return orderInfo;
        }
    }

    
}
