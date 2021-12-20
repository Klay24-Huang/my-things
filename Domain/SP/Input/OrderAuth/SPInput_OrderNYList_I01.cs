using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OrderAuth
{
    public class SPInput_OrderNYList_I01 : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 授權狀態
        /// </summary>
        public int AuthFlg { set; get; }
        /// <summary>
        /// 授權代碼
        /// </summary>
        public string AuthCode { set; get; }
        /// <summary>
        /// 授權訊息
        /// </summary>
        public string AuthMessage { set; get; }
        /// <summary>
        /// 金流交易序號
        /// </summary>
        public string transaction_no { set; get; }
        /// <summary>
        /// 金流訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; }
        /// <summary>
        /// 信用卡號 20211218 add by adam
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// 授權碼 20211218 add by adam
        /// </summary>
        public string AuthIdResp { get; set; }
        /// <summary>
        /// 授權金額 20211218 add by adam
        /// </summary>
        public int AuthAmt { get; set; }
    }
}
