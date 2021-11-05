using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Bill.Output
{
    public class OFN_CreditAuthResult
    {
        /// <summary>
        /// 支付方式
        /// </summary>
        public int CheckoutMode { get; set; }
        /// <summary>
        /// 卡片系統來源(0:和泰Pay 1:台新)
        /// </summary>
        public int CardType { get; set; }
        /// <summary>
        /// 信用卡號碼
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 請求付款時的交易單號(IRent產)
        /// </summary>
        public string Transaction_no { get; set; }

        /// <summary>
        /// 支付系統回傳的結果代碼
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// 支付系統回傳的驗證訊息
        /// </summary>
        public string AuthMessage { get; set; }
        
    }
}