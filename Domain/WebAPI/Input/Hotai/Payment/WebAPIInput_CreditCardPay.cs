using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.WebAPI.Input.Hotai.Payment
{
    public class WebAPIInput_CreditCardPay : WebAPIInput_PaymentBase
    {
        public string CardToken { get; set; }
        /// <summary>
        /// 77465 
        /// </summary>
        public string MerID { get; set; }

        public string TerMinnalID { get; set; }

        public string  Lidm { get; set; }

        public int PurchAmt { get; set; }
        /// <summary>
        /// 付款方式;僅接受數字(，0:一般交易 1:分期付款 2:紅利抵扣)
        /// </summary>
        public string TxType { get; set; }
        /// <summary>
        /// 是否自動轉入請款黨；僅允許純數字(必要欄位是=1 否=0，固定填 1)
        /// </summary>
        public string AutoCap { get; set; }
        /// <summary>
        /// 綁定後跳轉網址
        /// </summary>
        public string RedirectUrl { get; set; }
        /// <summary>
        /// 訂單描述(非必要欄位)
        /// </summary>
        public string OrderDesc { get; set; }
        /// <summary>
        /// 持卡人身分證(非必要欄位)
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 持卡人生日(非必要欄位:MMDDYYYY)
        /// </summary>
        public string Birthday { get; set; }
        /// <summary>
        /// 使用客制化授權頁面;非必要欄位，請固定填""
        /// </summary>
        public string Customize { get; set; }

        /// <summary>
        /// 非必要欄位，如為中文字，還必須檢核僅允許BIG5 編碼。
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 分期期數(非必要欄位，請請固定填"")
        /// </summary>
        public int NumberOfPay { get; set; }
        /// <summary>
        /// 行銷活動代碼（中信提供）(非必要欄位，請請固定填"")
        /// </summary>
        public string PromoCode { get; set; }
        /// <summary>
        /// 產品代碼;紅利折一般的產品代碼。(紅利折抵一般特店必填，其餘免填)(非必要欄位，請請固定填"")
        /// </summary>
        public string ProdCode { get; set; }



    }
}
